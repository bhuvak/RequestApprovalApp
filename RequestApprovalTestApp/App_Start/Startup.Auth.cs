using System;
using System.IdentityModel.Tokens;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;

namespace RequestApprovalTestApp
{
    using Microsoft.Azure.ActiveDirectory.ERM.Utils.Authentication.Claims;
    using System.Security.Claims;

    public partial class Startup
    {
        public ISettings Settings { get; set; }

        public void ConfigureAuth(IAppBuilder app)
        {
            var settings = new Settings();
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions { });

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = settings.ApplicationId,
                    Authority = $"https://{settings.LoginHost}/{settings.AppTenant}",
                    TokenValidationParameters = new System.IdentityModel.Tokens.TokenValidationParameters
                    {
                        // instead of using the default validation (validating against a single issuer value, as we do in line of business apps), 
                        // we inject our own multitenant validation logic
                        ValidateIssuer = false,
                        SaveSigninToken = true
                        // If the app needs access to the entire organization, then add the logic
                        // of validating the Issuer here.
                        // IssuerValidator
                    },
                    Notifications = new OpenIdConnectAuthenticationNotifications()
                    {   
                        //SecurityTokenValidated = (context) =>
                        //{
                        //    var bootstrapContext = (context.AuthenticationTicket.Identity.BootstrapContext as BootstrapContext);
                        //    if (bootstrapContext == null)
                        //    {
                        //        throw new InvalidOperationException("FooBar");
                        //    }

                        //    context.AuthenticationTicket.Identity.AddClaim(new Claim("AccessToken", bootstrapContext.Token));

                        //    // If your authentication logic is based on users then add your logic here
                        //    return Task.FromResult(0);
                        //} ,                    
                        AuthenticationFailed = (context) =>
                        {
                            // Pass in the context back to the app
                            context.OwinContext.Response.Redirect("/Home/Error");
                            context.HandleResponse(); // Suppress the exception
                            return Task.FromResult(0);
                        }
                    }
                });
        }
    }
}
