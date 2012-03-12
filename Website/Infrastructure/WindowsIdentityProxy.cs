using System.Security.Principal;

namespace NuGetGallery.Infrastructure
{
    public class WindowsIdentityProxy : IIdentity
    {
        private readonly IIdentity _identity;

        public WindowsIdentityProxy(IIdentity identity)
        {
            _identity = identity;
        }

        public string T4GUserName
        {
            get { return _identity.Name.Split('\\')[1]; }
        }

        public string Name
        {
            get { return T4GUserName.Replace('.', ' '); }
        }

        public string Email
        {
            get { return string.Format("{0}@t4g.com", T4GUserName); }
        }

        public string AuthenticationType
        {
            get { return _identity.AuthenticationType; }
        }

        public bool IsAuthenticated
        {
            get { return _identity.IsAuthenticated; }
        }
    }
}