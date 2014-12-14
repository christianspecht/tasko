using Raven.Client.Document;
using System;
using System.Configuration;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using Tasko.Server.Controllers;
using Tasko.Server.Models;
using Thinktecture.IdentityModel.Tokens.Http;

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

            GlobalConfiguration.Configuration.Routes.MapHttpRoute(
                name: "DefaultApiWithAction",
                routeTemplate: "api/{controller}/{id}/{action}");

            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

            GlobalConfiguration.Configuration.Filters.Add(new AuthorizeAttribute());

            RavenController.Store = new DocumentStore { ConnectionStringName = "RavenDB" };
            RavenController.Store.Initialize();

            bool requireSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["RequireSsl"]);
            int tokenLifetime = Convert.ToInt32(ConfigurationManager.AppSettings["TokenLifetime"]);

            var authConfig = new AuthenticationConfiguration
            {
                EnableSessionToken = true,
                RequireSsl = requireSsl && !Helper.RunningOnAppHarbor(),
                SessionToken = new SessionTokenConfiguration()
                {
                    SigningKey = new SigningKey().Get(),
                    DefaultTokenLifetime = TimeSpan.FromDays(tokenLifetime)
                }
            };

            authConfig.AddBasicAuthentication((username, password) =>
            {
                var session = RavenController.Store.OpenSession();
                var user = session.Load<User>("users/" + username);

                if (user != null)
                {
                    return user.Password == password;
                }

                return false;
            });

            GlobalConfiguration.Configuration.MessageHandlers.Add(new AuthenticationHandler(authConfig));
        }
    }
}