using System.Configuration;

namespace Tasko.Server
{
    public class Helper
    {
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