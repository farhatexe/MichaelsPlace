using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MichaelsPlace.Models.Persistence;
using MichaelsPlace.Services.Messaging;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace MichaelsPlace.Infrastructure.Identity
{
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
            UserValidator = new UserValidator<ApplicationUser>(this)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            UserLockoutEnabledByDefault = true;
            DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            this.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            this.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            EmailService = new DevelopmentEmailService();
            SmsService = new DevelopmentSmsService();
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }

        public async Task<IdentityResult> EnsureHasClaimAsync(string userId, string claimType, string claimValue)
        {
            var claims = await GetClaimsAsync(userId);
            if (claims.Any(c => c.Type == claimType && c.Value == claimValue))
            {
                return IdentityResult.Success;
            }
            return await AddClaimAsync(userId, new Claim(claimType, claimValue));
        }

        public async Task<IdentityResult> EnsureDoesNotHaveClaimAsync(string userId, string claimType, string claimValue)
        {
            var claim = (await GetClaimsAsync(userId)).FirstOrDefault(c => c.Type == claimType && c.Value == claimValue);
            if (claim != null)
            {
                return await RemoveClaimAsync(userId, new Claim(claimType, claimValue));
            }
            return IdentityResult.Success;
        }

        public override Task<IdentityResult> CreateAsync(ApplicationUser user)
        {
            EnsurePersonExistsOnCreate(user);
            return base.CreateAsync(user);
        }

        public override Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
        {
            EnsurePersonExistsOnCreate(user);
            return base.CreateAsync(user, password);
        }
        
        private void EnsurePersonExistsOnCreate(ApplicationUser user)
        {
            if (user.Person == null)
            {
                user.Person = new Person
                              {
                                  EmailAddress = user.Email,
                                  PhoneNumber = user.PhoneNumber
                              };
            }
        }
    }
}