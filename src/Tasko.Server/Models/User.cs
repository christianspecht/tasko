namespace Tasko.Server.Models
{
    /// <summary>
    /// A Tasko user
    /// </summary>
    public class User
    {
        /// <summary>
        /// The user's Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The user's password
        /// </summary>
        public string Password { get; set; }
    }
}