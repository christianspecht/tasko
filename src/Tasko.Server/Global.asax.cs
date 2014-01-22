using System;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using Raven.Client.Document;
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

            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

            GlobalConfiguration.Configuration.Filters.Add(new AuthorizeAttribute());

            RavenController.Store = new DocumentStore { ConnectionStringName = "RavenDB" };
            RavenController.Store.Initialize();

            var authConfig = new AuthenticationConfiguration
            {
                DefaultAuthenticationScheme = "Basic",
                EnableSessionToken = true,
                SendWwwAuthenticateResponseHeader = true,
                SessionToken = new SessionTokenConfiguration()
                {
                    SigningKey = new SigningKey().Get()
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