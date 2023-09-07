namespace Interject.Api
{
    public class IdsUserContext
    {
        /// <summary>
        /// The login name of the user
        /// </summary>
        public string MachineLoginName { get; set; }

        /// <summary>
        /// The name of the machine being used
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// The full name of the user
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// The Interject User ID of the user
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// The Interject Client ID of the user
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// The login name (email) of the user
        /// </summary>
        public string LoginName { get; set; }

        /// <summary>
        /// Deprecated. This will no longer be available as of version 2.5.
        /// </summary>
        public string LoginAuthTypeId { get; set; }

        /// <summary>
        /// The UTC date and time of the user's login
        /// </summary>
        public string LoginDateUtc { get; set; }

        /// <summary>
        /// A list of roles of the user
        /// </summary>
        public string UserRoles { get; set; }
    }
}