using WebApiContrib.MessageHandlers;

namespace Tasko.Server
{
    public class TaskoBasicAuthHandler : BasicAuthenticationHandler
    {
        protected override bool Authorize(string username, string password)
        {
            if (username == "test" && password == "123")
            {
                return true;
            }

            return false;
        }

        protected override string Realm
        {
            get { return "Tasko.Server"; }
        }
    }
}