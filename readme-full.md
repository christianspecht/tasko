![logo](https://bitbucket.org/christianspecht/tasko/raw/tip/img/logo128x128.png)

Tasko will be a simple "to-do list" Android app with a server backend (to use the same list of tasks from multiple clients).

There are probably already hundreds of apps that can do this ([Remember the Milk](http://www.rememberthemilk.com/) for example), but this one is mainly for learning purposes (write a mobile app with a backend service).

**Note that this is not a hosted service like Remember the Milk!  
It comes with a server backend that you have to host yourself, on your own server!**

*The name "Tasko" just means ["task" in Esperanto](http://translate.google.com/#en/eo/task).  
(in order to find a good name for the project, I played around with words like "task", "todo" etc. in Google Translate)*

---

## Links

- Download page *(no downloads yet - Tasko isn't finished)*
- [Report a bug](https://bitbucket.org/christianspecht/tasko/issues/new)
- [Main project page on Bitbucket](https://bitbucket.org/christianspecht/tasko)

---

## API Documentation


If you want to make test calls to the API with a tool like [Fiddler](http://fiddler2.com/), remember to [set the right content type when you **send** data to the API](http://truncatedcodr.wordpress.com/2012/09/05/asp-net-web-api-always-set-content-type/):

	Content-Type: application/json; charset=utf-8



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

- [MSBuild Community Tasks](https://github.com/loresoft/msbuildtasks)
- [NUnit](http://nunit.org/)
- [RavenDB](http://ravendb.net/)

---

### License

Tasko is licensed under the MIT License. See [License.txt](https://bitbucket.org/christianspecht/tasko/raw/tip/License.txt) for details.

---

### Project Info

<script type="text/javascript" src="http://www.ohloh.net/p/633484/widgets/project_basic_stats.js"></script>  
<script type="text/javascript" src="http://www.ohloh.net/p/633484/widgets/project_languages.js"></script>