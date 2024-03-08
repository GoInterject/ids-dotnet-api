using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public class ClaimsUpdateMiddleware
{
    private readonly RequestDelegate _next;

    public ClaimsUpdateMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // extract the existing token from the request
        var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        // convert the token to an object
        var jwt = new JwtSecurityToken(token);
        // get the "user_identity" claim
        var identity = jwt.Claims.FirstOrDefault(c => c.Type == "user_identity");
        // if the "user_identity" claim is not present, call the next middleware in the pipeline
        if (identity == null)
        {
            await _next(context);
            return;
        }
        // deserialize the claim to a UserIdentityClaim object
        var user = System.Text.Json.JsonSerializer.Deserialize<UserIdentityClaim>(identity.Value);
        // add the "ids_client_id" claim
        jwt.Payload["ids_client_id"] = user.ClientIdPublic;
        // remove the "user_identity" claim
        jwt.Payload.Remove("user_identity");
        // convert the token back to a string
        token = new JwtSecurityTokenHandler().WriteToken(jwt);
        // replace the token in the request
        context.Request.Headers["Authorization"] = "Bearer " + token;

        // Create a new ClaimsIdentity with the updated claims
        var claimsIdentity = new ClaimsIdentity(jwt.Claims, "Bearer");
        // Create a new ClaimsPrincipal with the updated ClaimsIdentity
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        // Assign the new ClaimsPrincipal to context.User
        context.User = claimsPrincipal;

        // Call the next middleware in the pipeline
        await _next(context);
    }

    internal class UserIdentityClaim
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

        public string? EnterpriseLoginCode { get; set; }

        #endregion

        #region IdentityProvider

        public int ProviderId { get; set; }
        public string ProviderScheme { get; set; } = string.Empty;
        public string ProviderName { get; set; } = string.Empty;
        public string ProviderType { get; set; } = string.Empty;

        #endregion
    }
}