using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MichaelsPlace.Models.Persistence
{
    public class ApplicationUser : IdentityUser
    {
        private ICollection<UserCaseItem> _caseItems;
        private ICollection<CaseUser> _cases;

        public virtual ICollection<CaseUser> CaseUsers
        {
            get { return _cases ?? (_cases = new HashSet<CaseUser>()); }
            set { _cases = value; }
        }

        public virtual ICollection<UserCaseItem> UserCaseItems
        {
            get { return _caseItems ?? (_caseItems = new HashSet<UserCaseItem>()); }
            set { _caseItems = value; }
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}