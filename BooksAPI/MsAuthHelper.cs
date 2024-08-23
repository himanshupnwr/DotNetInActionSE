using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace BooksAPI
{
    //Setting up Swagger UI to communicate with Azure AD
    public static class MsAuthHelper
    {
        //AAD login base URL
        private const string UrlPrefix = "https://login.microsoftonline.com/";
        //Location of tenant ID in appsettings
        private const string TenantIdConfig = "Authentication:Microsoft:TenantId";
        //Location of client ID in appsettings
        private const string ClientIdConfig = "Authentication:Microsoft:ClientId";

        //Defines a flow for Swagger UI
        //Needs the configuration
        //List of custom scopes
        public static OpenApiOAuthFlow GetImplicitFlow(IConfiguration config, params string[] extraScopes)
        {
            var tenantId = GetTenantId(config);
            //Dictionary of scope name to description
            //Standard scopes
            var scopes = new Dictionary<string, string>() {
                { "openid", "" },
                { "profile", "" },
                { "email", "" }
              };
            foreach (var extraScope in extraScopes)
            {
                scopes.Add(extraScope, "");
            }

            //Get from discovery document
            return new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"{UrlPrefix}{tenantId}/oauth2/v2.0/authorize"),
                TokenUrl = new Uri($"{UrlPrefix}{tenantId}/oauth2/v2.0/token"),
                Scopes = scopes,
            };
        }

        public static string GetTenantId(IConfiguration config) => config.GetValue<string>(TenantIdConfig);

        public static string GetClientId(IConfiguration config) => config.GetValue<string>(ClientIdConfig);

        //Helper for custom scope name
        public static string AddScopePrefix(this string scopeName, IConfiguration config)
          => $"api://{GetClientId(config)}/{scopeName}";

        //Helper for JwtBearerOptions
        public static JwtBearerOptions ValidateMs(this JwtBearerOptions options, IConfiguration config)
        {
            //Must match token issuer (iss claim)
            options.Authority = $"{UrlPrefix}{GetTenantId(config)}/v2.0";
            //Must match token audience (aud claim)
            options.Audience = GetClientId(config);
            return options;
        }

        //What is a discovery document?

        //The OpenID Connect specification indicates that an identity provider must provide a publicly accessible document
        //that indicates all the important configuration.This document contains a list of important URLs, scopes available,
        //and so on.Microsoft’s document for social accounts, for example,
        //is available at https://login.microsoftonline.com/consumers/v2.0/.well-known/openid-configuration.
        //The Swagger UI configuration can take a discovery document instead of specifying URLs explicitly
        //like the authorize and token URLs configured in listing 9.18.
        //When you take this approach, the flows aren’t configurable, so this book doesn’t use it.
    }
}