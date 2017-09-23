using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CommerceFundamentalsWeb.Models.Pages;
using CommerceFundamentalsWeb.Models.ViewModels;
using EPiServer;
using EPiServer.Core;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;
using Mediachase.Commerce.Core;
using Mediachase.Commerce.Customers;
using Mediachase.Commerce.Customers.Profile;
using Mediachase.Commerce.Security;

namespace CommerceFundamentalsWeb.Controllers
{
    public class AccountController : PageController<AccountPage>
    {
        private readonly IContentRepository _contentRepository;
        private readonly UrlResolver _urlResolver;

        public AccountController(IContentRepository contentRepository, UrlResolver urlResolver)
        {
            _contentRepository = contentRepository;
            _urlResolver = urlResolver;
        }


        public ActionResult Index(AccountPage currentPage)
        {
            AccountViewModel model = new AccountViewModel();

            return View(model);
        }

        public ActionResult Login(AccountPage currentPage, AccountViewModel model)
        {
            if (Membership.ValidateUser(model.UserName, model.Password))
            {
                var url = _urlResolver.GetUrl(ContentReference.StartPage);
                MembershipUser account = Membership.GetUser(model.UserName);
                if (account != null)
                {
                    var profile = SecurityContext.Current.CurrentUserProfile as CustomerProfileWrapper;
                    if (profile != null)
                    {
                        CreateAuthenticationCookie(ControllerContext.HttpContext, model.UserName,
                            Mediachase.Commerce.Core.AppContext.Current.ApplicationName, false);
                        return Redirect(url);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("wrong_username","Wrong username or password");
            }
            
            return View("Index",model);
        }
        
        public ActionResult CreateAccount(AccountPage currentPage, AccountViewModel model)
        {
            // The important here are the Roles and the Contact properties 
            string firstName = model.FirstName;
            string lastName = model.LastName;
            string emailAddress = model.UserName; // 
            string password = model.Password;

            MembershipUser user = null;
            
            MembershipCreateStatus createStatus;
            user = Membership.CreateUser(emailAddress, password, emailAddress,
                                         null, null, true, out createStatus);
            //CustomerContext.Current.
            // Create the Contact in ECF 
            CustomerContact customerContact = CustomerContact.CreateInstance(user);
            customerContact.FirstName = firstName;
            customerContact.LastName = lastName;
            customerContact.RegistrationSource = String.Format("{0}, {1}"
                , this.Request.Url.Host, SiteContext.Current);

            customerContact.Email = emailAddress;

            customerContact.SaveChanges();

            
            // We don't need this anymore for visitors/customers
            Roles.AddUserToRole(user.UserName, AppRoles.EveryoneRole);
            Roles.AddUserToRole(user.UserName, AppRoles.RegisteredRole);

            if (Roles.RoleExists("ClubMember"))
            {
                Roles.AddUserToRole(user.UserName, "ClubMember");
            }
            // Call for further properties to be set
            SetContactProperties(customerContact);

            return Login(currentPage, model);            
        }

        protected void SetContactProperties(CustomerContact contact)
        {
            Organization org = Organization.CreateInstance();
            org.Name = "ParentOrg";
            org.SaveChanges();
            
            contact.CustomerGroup = "Partner";

            // The custom field
            contact["Geography"] = "West";
            contact.OwnerId = org.PrimaryKeyId;

            contact.SaveChanges();
        }

        public static void CreateAuthenticationCookie(HttpContextBase httpContext, string username, string domain, bool remember)
        {
            // ... needed for cookieless authentication
            FormsAuthentication.SetAuthCookie(username, remember);
            var expirationDate = FormsAuthentication.GetAuthCookie(username, remember).Expires;

            // we need to handle ticket ourselves since we need to save session paremeters as well
            int timeout = httpContext.Session != null ? httpContext.Session.Timeout : 20;
            var ticket = new FormsAuthenticationTicket(2,
                    username,
                    DateTime.Now,
                    expirationDate == DateTime.MinValue ? DateTime.Now.AddMinutes(timeout) : expirationDate,
                    remember,
                    domain,
                    FormsAuthentication.FormsCookiePath);

            // Encrypt the ticket.
            string encTicket = FormsAuthentication.Encrypt(ticket);

            // remove the cookie, if one already exists with the same cookie name
            if (httpContext.Response.Cookies[FormsAuthentication.FormsCookieName] != null)
            {
                httpContext.Response.Cookies.Remove(FormsAuthentication.FormsCookieName);
            }

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
            cookie.HttpOnly = true;

            cookie.Path = FormsAuthentication.FormsCookiePath;
            cookie.Secure = FormsAuthentication.RequireSSL;
            if (FormsAuthentication.CookieDomain != null)
            {
                cookie.Domain = FormsAuthentication.CookieDomain;
            }

            if (ticket.IsPersistent)
            {
                cookie.Expires = ticket.Expiration;
            }

            // Create the cookie.
            httpContext.Response.Cookies.Set(cookie);
        }

    }
}