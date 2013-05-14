using System.Web.Http;
using System.Web.Http.Controllers;
using Raven.Client;

namespace Tasko.Server.Controllers
{
    public abstract class RavenController : ApiController
    {
        public IDocumentSession RavenSession { get; set; }

        public static IDocumentStore Store { get; set; }

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            if (this.RavenSession == null)
            {
                this.RavenSession = Store.OpenSession();
            }
            base.Initialize(controllerContext);
        }

        protected override void Dispose(bool disposing)
        {
            using (this.RavenSession)
            {
                if (this.RavenSession != null)
                {
                    this.RavenSession.SaveChanges();
                }
            }
            base.Dispose(disposing);
        }
    }
}