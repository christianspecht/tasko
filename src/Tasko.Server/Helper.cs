using System;
using System.Configuration;
using System.Security.Principal;
using System.Threading;

namespace Tasko.Server
{
    public class Helper
    {
        private static Func<IPrincipal> currentuser = () => Thread.CurrentPrincipal;

        /// <summary>
        /// Returns the current user
        /// </summary>
        public static string CurrentUser
        {
            get { return currentuser().Identity.Name; }
        }

        /// <summary>
        /// Sets the current user to principal (for unit tests!)
        /// </summary>
        /// <param name="principal"></param>
        public static void SetCurrentUser(Func<IPrincipal> principal)
        {
            currentuser = principal;
        }

        /// <summary>
        /// Determines if the app is running on AppHarbor
        /// </summary>
        /// <returns></returns>
        public static bool RunningOnAppHarbor()
        {
            // we're on AppHarbor when a configuration variable named "appharbor.commit_id" exists
            // Source: http://support.appharbor.com/kb/getting-started/managing-environments
            return !string.IsNullOrEmpty(ConfigurationManager.AppSettings["appharbor.commit_id"]);
        }
    }
}