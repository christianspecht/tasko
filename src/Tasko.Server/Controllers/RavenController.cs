using System;
using System.Web.Http;
using System.Web.Http.Controllers;
using Raven.Client;
using Raven.Client.Document;

namespace Tasko.Server.Controllers
{
    public abstract class RavenController : ApiController
    {
        public IDocumentSession RavenSession { get; protected set; }

        public IDocumentStore Store
        {
            get { return LazyDocStore.Value; }
        }

        private static readonly Lazy<IDocumentStore> LazyDocStore = new Lazy<IDocumentStore>(() =>
        {
            var docStore = new DocumentStore { ConnectionStringName = "RavenDB" };
            docStore.Initialize();
            return docStore;
        });

        protected override void Initialize(HttpControllerContext controllerContext)
        {            
            this.RavenSession = this.Store.OpenSession();
            base.Initialize(controllerContext);
        }

        protected override void Dispose(bool disposing)
        {
            using (this.RavenSession)
            {
                this.RavenSession.SaveChanges();
            }
            base.Dispose(disposing);
        }
    }
}