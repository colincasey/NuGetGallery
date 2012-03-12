using System;
using System.Security.Principal;
using System.Threading;
using System.Web;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using NuGetGallery.Infrastructure;

[assembly: WebActivator.PreApplicationStartMethod(typeof(NuGetGallery.AuthenticationModule), "Start")]
namespace NuGetGallery
{
    public class AuthenticationModule : IHttpModule
    {
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(AuthenticationModule));
        }

        public void Init(HttpApplication context)
        {
            context.AuthenticateRequest += OnAuthenticateRequest;
        }

        void OnAuthenticateRequest(object sender, EventArgs e)
        {
            var context = HttpContext.Current;
            var request = HttpContext.Current.Request;

            if (request.IsAuthenticated)
            {
                var userSvc = (IUserService) Container.Kernel.GetService(typeof(IUserService));
                var identity = new WindowsIdentityProxy(context.User.Identity);
                var user = userSvc.FindByUsername(identity.Name);

                if (user == null)
                {
                    user = userSvc.Create(identity.Name, "Password", identity.Email);
                    user.ConfirmEmailAddress();
                }

                var principal = new GenericPrincipal(identity, new[] { Constants.AdminRoleName });
                context.User = Thread.CurrentPrincipal = principal;
            }
        }

        public void Dispose()
        {
        }
    }
}