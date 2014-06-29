![logo](https://bitbucket.org/christianspecht/tasko/raw/tip/img/logo128x128.png)

Tasko is a simple to-do list app that you can host on your own server.  
You can access your to-dos from everywhere with the Android app.

There are probably already hundreds of apps that can do this ([Remember the Milk](http://www.rememberthemilk.com/) for example), but this one is mainly for learning purposes (write a mobile app with a backend service).

**Note that this is not a hosted service like Remember the Milk!  
It comes with a server backend that you have to host yourself, on your own server!**

*The name "Tasko" just means ["task" in Esperanto](http://translate.google.com/#en/eo/task).  
(in order to find a good name for the project, I played around with words like "task", "todo" etc. in Google Translate)*

---

## Links

- Download page *(no downloads yet - Tasko isn't finished)*
- Report a bug:
	- [Tasko Server](https://bitbucket.org/christianspecht/tasko/issues/new)
	- [Tasko Android Client](https://bitbucket.org/christianspecht/tasko-androidclient/issues/new)
- Project pages on Bitbucket:
	- [Tasko Server](https://bitbucket.org/christianspecht/tasko)	
	- [Tasko Android Client](https://bitbucket.org/christianspecht/tasko-androidclient)

---

## Hosting

The Tasko server needs [IIS](http://www.iis.net/) and [RavenDB](http://ravendb.net/) to run.  
I'm using [Windows Azure Websites](http://azure.microsoft.com/en-us/services/web-sites/) and [RavenHQ](https://www.ravenhq.com/) for my personal instance. 

### SSL / https

By default, Tasko enforces SSL, i.e. it will only accept HTTPS requests.

**Note that Tasko will never enforce SSL when it's running on [AppHarbor](https://appharbor.com/)**, because [AppHarbor needs a custom `RequireHttps` attribute for that](https://gist.github.com/geersch/7710361) and I wasn't able to get it to work.  
You can still use HTTPS on AppHarbor, it's just that Tasko will accept HTTP as well.

If your server doesn't have SSL at all, you can enable HTTP by setting the `RequireSsl` key in the `appSettings` *(in `web.config`)* to `false`.

### Setting a SigningKey

Tasko uses [session token authentication](http://leastprivilege.com/2012/06/19/session-token-support-for-asp-net-web-api/), provided by [Thinktecture.IdentityModel](http://www.nuget.org/packages/Thinktecture.IdentityModel).
IdentityModel needs a `SigningKey` to sign the tokens it creates.  
 
You should provide a fixed key in `web.config`:

	<appSettings>
		<add key="SigningKey" value="..."/>
	</appSettings>

To create a valid key, just create a random 32-character string and convert it to Base64 *([like IdentityModel does it under the hood](https://github.com/thinktecture/Thinktecture.IdentityModel.40/blob/master/IdentityModel/Thinktecture.IdentityModel/Tokens/Http/SessionTokenConfiguration.cs#L58))*.

If you don't provide a key, a new one will be generated on each application restart, which means that all existing tokens are invalidated.
So if you want the tokens to "survive" an application restart, you should provide your own key.

### Creating users

For now, Tasko doesn't support creating new users via the API - you have to create them in RavenDB Management Studio.  
Just create a new document *(**New** -> **New Document** in the top navigation bar)*, enter `users/yourname` as the document id and the following data:
	
	{
	  "Id": "yourname",
	  "Password": "123"
	}

Here's how it should look like for the user **yourname**:

![new user](https://bitbucket.org/christianspecht/tasko/raw/tip/img/newuser.png)

---

## API Documentation

### Authentication and content type

As mentioned before, Tasko uses **session token authentication**.  
You log in once with username and password via [HTTP Basic Authentication](http://en.wikipedia.org/wiki/Basic_access_authentication#Client_side) to request a session token:

	GET /api/token
	
	Content-Type: application/json; charset=utf-8
	Authorization: Basic eW91cm5hbWU6MTIz

*(this is the user from the example above, **yourname** with password **123**)*

**Response:**

	{
	  "access_token": "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9...",
	  "expires_in": 31536000.0
	}

This token can now be used for authentication in subsequent requests. With Tasko's default settings, it will be valid for a year *(you can change that in `web.config` by setting a different value for the `TokenLifetime` key)*.

Don't forget to [set the right content type when you **send** data to the API](http://truncatedcodr.wordpress.com/2012/09/05/asp-net-web-api-always-set-content-type/).  
So a correct request header should contain these two lines:

	Content-Type: application/json; charset=utf-8
	Authorization: Session eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9...



### Load all tasks

	GET /api/tasks

**Response:**

	[
	  {
	    "Id": 1,
	    "Description": "First Task",
	    "Categories": [
	      "Category1"
	    ],
	    "CreatedAt": "2013-06-09T21:02:13.78125Z",
	    "LastEditedAt": "2013-06-09T21:02:13.78125Z",
	    "FinishedAt": null,
	    "IsFinished": false
	  },
	  {
	    "Id": 2,
	    "Description": "Second Task",
	    "Categories": [
	      "Category2"
	    ],
	    "CreatedAt": "2013-06-09T21:06:16.15625Z",
	    "LastEditedAt": "2013-06-09T21:06:16.15625Z",
	    "FinishedAt": null,
	    "IsFinished": false
	  }
	]

### Load a single task

	GET /api/tasks/1

**Response:**

	{
	  "Id": 1,
	  "Description": "First Task",
	  "Categories": [
	    "Category1"
	  ],
	  "CreatedAt": "2013-06-09T21:02:13.78125Z",
	  "LastEditedAt": "2013-06-09T21:02:13.78125Z",
	  "FinishedAt": null,
	  "IsFinished": false
	}

### Load all tasks with a specific category

	GET /api/tasks?category=Category1

**Response:**

	[
	  {
	    "Id": 1,
	    "Description": "First Task",
	    "Categories": [
	      "Category1"
	    ],
	    "CreatedAt": "2013-06-09T21:02:13.78125Z",
	    "LastEditedAt": "2013-06-09T21:02:13.78125Z",
	    "FinishedAt": null,
	    "IsFinished": false
	  }
	]

### Create a new task

	POST /api/tasks 

**Input:**

	{
		"Description": "the description",
		"Category": "the category"
	}

**Response:**

	{
	  "Id": 3,
	  "Description": "the description",
	  "Categories": [
	    "the category"
	  ],
	  "CreatedAt": "2013-06-09T22:27:34.875Z",
	  "LastEditedAt": "2013-06-09T22:27:34.875Z",
	  "FinishedAt": null,
	  "IsFinished": false
	}
	

---

### Acknowledgements

Tasko makes use of the following open source projects:

- [Android](http://www.android.com/)
- [MSBuild Community Tasks](https://github.com/loresoft/msbuildtasks)
- [NUnit](http://nunit.org/)
- [RavenDB](http://ravendb.net/)
- [Thinktecture.IdentityModel](http://www.nuget.org/packages/Thinktecture.IdentityModel)

<a name="license"></a>

---

### License

Tasko and the Tasko Android client are licensed under the MIT License. See [License.txt](https://bitbucket.org/christianspecht/tasko/raw/tip/License.txt) for details.

---

### Project Info

<script type="text/javascript" src="http://www.ohloh.net/p/633484/widgets/project_basic_stats.js"></script>  
<script type="text/javascript" src="http://www.ohloh.net/p/633484/widgets/project_languages.js"></script>