﻿using System;
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

        public ActionResult Login(AccountPage currentPage, string userName, string passWord)
        {
            if (Membership.ValidateUser(userName, passWord))
            {
                MembershipUser account = Membership.GetUser(userName);
                if (userName != null)
                {
                    var profile = SecurityContext.Current.CurrentUserProfile as CustomerProfileWrapper;
                    if (profile != null)
                    {
                        CreateAuthenticationCookie(ControllerContext.HttpContext, userName, Mediachase.Commerce.Core.AppContext.Current.ApplicationName, false);
                        //return Redirect(url);
                    }
                }
            }

            // ...just for a check
            return RedirectToAction("Index", new { page = ContentReference.StartPage }.page.ToPageReference());
        }

        // ToDo: Lab incustomers module
        public ActionResult CreateAccount(AccountPage currentPage, string userName, string passWord)
        {
            MembershipCreateStatus createStatus;
            var membershipUser = Membership.CreateUser(userName, passWord, userName, null, null, true, out createStatus);

            if (createStatus != MembershipCreateStatus.Success)
            {
                return null;
            }

            var customerContact = CustomerContact.CreateInstance(membershipUser);
            customerContact.Email = customerContact.LastName = customerContact.FirstName = userName;
            customerContact.RegistrationSource = $"{Request.Url?.Host}{SiteContext.Current}";

            customerContact.SaveChanges();

            SetContactProperties(customerContact);

            return null; // for now
        }

        protected void SetContactProperties(CustomerContact contact)
        {
            var organization = Organization.CreateInstance();
            organization.Name = contact.FirstName;

            organization.SaveChanges();

            contact["Geography"] = "North";
            contact.CustomerGroup = "Partner";
            contact.OwnerId = organization.PrimaryKeyId;

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