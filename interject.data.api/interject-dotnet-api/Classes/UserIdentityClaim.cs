
namespace Interject.DataApi
{
    public class UserIdentityClaim
    {
        #region User

        public int UserId { get; set; }
        public string UserIdPublic { get; set; } = string.Empty;
        public string LoginName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        #endregion

        #region Client

        public string ClientIdPublic { get; set; } = string.Empty;
        public int ClientId { get; set; }

        #endregion

        #region Client.EnterpriseLogin

        public string EnterpriseLoginCode { get; set; } = string.Empty;

        #endregion

        #region IdentityProvider

        public int ProviderId { get; set; }
        public string ProviderScheme { get; set; } = string.Empty;
        public string ProviderName { get; set; } = string.Empty;
        public string ProviderType { get; set; } = string.Empty;

        #endregion
    }
}
