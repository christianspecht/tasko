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
I'm using [AppHarbor](https://appharbor.com/) (with the [RavenHQ add-on](https://appharbor.com/addons/ravenhq)) for my personal instance.  


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

1. Tasko uses [HTTP Basic Authentication](http://en.wikipedia.org/wiki/Basic_access_authentication#Client_side).

2. Don't forget to [set the right content type when you **send** data to the API](http://truncatedcodr.wordpress.com/2012/09/05/asp-net-web-api-always-set-content-type/).

So a correct request header should contain these two lines:

	Content-Type: application/json; charset=utf-8
	Authorization: Basic eW91cm5hbWU6MTIz

*(this is the user from the example above, **yourname** with password **123**)*


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

---

### License

Tasko and the Tasko Android client are licensed under the MIT License. See [License.txt](https://bitbucket.org/christianspecht/tasko/raw/tip/License.txt) for details.

---

### Project Info

<script type="text/javascript" src="http://www.ohloh.net/p/633484/widgets/project_basic_stats.js"></script>  
<script type="text/javascript" src="http://www.ohloh.net/p/633484/widgets/project_languages.js"></script>