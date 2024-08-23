
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace BooksAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var config = builder.Configuration;

            builder.Services.AddDbContext<CatalogContext>(options =>
            {
                var connStr = config.GetConnectionString("Catalog");
                options.UseSqlite(connStr);
            });

            //builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

            builder.Services.AddControllers();
            //Configures the Swagger UI
            builder.Services.AddSwaggerGen(options =>
            {
                var scheme = new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    //OpenID Connect is built on OAuth 2.0.
                    Type = SecuritySchemeType.OAuth2,
                    //HTTP bearer auth scheme
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    //Bearer token format
                    BearerFormat = "JWT",
                    //Where to put bearer token
                    In = ParameterLocation.Header,
                    Flows = new OpenApiOAuthFlows
                    {
                        //Enables only implicit flow
                        //Name of your custom scope
                        Implicit = MsAuthHelper.GetImplicitFlow(config, "DotNetInActionAuth".AddScopePrefix(config))
                    }
                };
                //Definition name must match
                options.AddSecurityDefinition("token", scheme);
                options.AddSecurityRequirement(new OpenApiSecurityRequirement { {
                    new OpenApiSecurityScheme {
                        Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id = "token",
                        }
                    },
                    Array.Empty<string>()
                    }
                });
            });

            //The default settings for HSTS in ASP.NET Core don’t use the preload feature.
            //Preload is a registry of sites maintained by Google but used by all major web browsers.
            //If you attempt to visit a site with an HTTP address and the browser finds that site on the registry,
            //preload changes the URL to HTTPS without sending the HTTP request and getting the redirect with the HSTS header.
            //You can use a public site to check the preload settings of any domain: https://hstspreload.org.
            //To use preload, configure HSTS in the services collection
            builder.Services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = DateTime.UtcNow.AddYears(1) - DateTime.UtcNow;
                options.ExcludedHosts.Add("test.manningcatalog.net");
            });

            //The JWT is like a JSON object divided into the three parts: header, payload, and signature.
            //The header indicates the type of token and what algorithm was used to sign it.
            //The signature is compared to the signing key (made available from a link in the discovery document)
            //and can be used to ensure that the token’s contents weren’t tampered with.
            //The payload is like an object in which every property is called a claim. 

            //Uses custom JWT config
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => options.ValidateMs(config));

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AuthenticatedUsers", policyBuilder =>
                {
                    policyBuilder.RequireAuthenticatedUser().AddAuthenticationSchemes(
                        JwtBearerDefaults.AuthenticationScheme);
                });
                options.AddPolicy("OnlyMe", policyBuilder =>
                {
                    policyBuilder.RequireAuthenticatedUser().AddAuthenticationSchemes(
                        JwtBearerDefaults.AuthenticationScheme).AddRequirements(new OnlyMeRequirement());
                });
            });

            builder.Services.AddSingleton < IAuthorizationHandler, OnlyMeRequirementHandler>();

            var app = builder.Build();

            //the .NET command-line interface (CLI) includes a command that installs a development certificate:
            //dotnet dev-certs https --trust
            if (app.Environment.IsProduction())
            {
                app.UseHsts();
            }

            using (var scope = app.Services.CreateScope())
            {
                using (var dbContext = scope.ServiceProvider.GetRequiredService<CatalogContext>())
                {
                    dbContext.Database.EnsureCreated();
                }
            }
            //This line responds to any HTTP requests with a redirect status code to the same URL in HTTPS
            app.UseHttpsRedirection();
            app.MapControllers();
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseAuthentication();
            app.UseAuthorization();

            app.Run();

            //ASP.NET Core recommends using the HTTP Strict Transport Security (HSTS) protocol.
            //That way, browsers visiting the site will record that the whole site should be HTTPS instead of
            //testing and caching individual URLs. Put simply, HSTS is a means to tell clients (browsers)
            //that the site they’re visiting is accessible only via HTTPS,
            //which it does by including a response header called Strict-Transport-Security.
        }
    }
}
