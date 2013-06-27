using System;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using Raven.Client.Document;
using Tasko.Server.Controllers;

namespace Tasko.Server
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            GlobalConfiguration.Configuration.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });

            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

            GlobalConfiguration.Configuration.MessageHandlers.Add(new TaskoBasicAuthHandler());

            RavenController.Store = new DocumentStore { ConnectionStringName = "RavenDB" };
            RavenController.Store.Initialize();
        }
    }
}