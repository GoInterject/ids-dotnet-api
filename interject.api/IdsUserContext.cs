namespace Interject.Api
{
    public class IdsUserContext
    {
        public string MachineLoginName { get; set; }

        public string MachineName { get; set; }

        public string FullName { get; set; }

        public string UserId { get; set; }

        public string ClientId { get; set; }

        public string LoginName { get; set; }

        /// <summary>
        /// Deprecated. This will no longer be available as of version 2.5
        /// </summary>
        public string LoginAuthTypeId { get; set; }

        public string LoginDateUtc { get; set; }

        public string UserRoles { get; set; }
    }
}