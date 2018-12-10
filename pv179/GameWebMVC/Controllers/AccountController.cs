﻿using BL.DTO;
using BL.Facades;
using GameWebMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace GameWebMVC.Controllers
{
    public class AccountController : Controller
    {
        #region Facades
        public AccountFacade AccountFacade { get; set; }
        #endregion

        #region Index
        public async Task<ActionResult> Index()
        {
            var account = await AccountFacade.GetAccountAsync(Guid.Parse(User.Identity.Name));
            return View(account);
        }
        #endregion

        #region Registration
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Register(AccountCreateDto accountCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            try
            {
                var id = await AccountFacade.RegisterAccount(accountCreateDto);

                var authTicket = new FormsAuthenticationTicket(1, id.ToString(), DateTime.Now,
                    DateTime.Now.AddMinutes(30), false, "");
                string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                HttpContext.Response.Cookies.Add(authCookie);

                return RedirectToAction("Create", "Character");
            }
            catch (ArgumentException e)
            {
                ModelState.AddModelError(e.ParamName, "Účet s daným nemom alebo emailom už existuje!");
                return View();
            }
            catch
            {
                return View();
            }

        }
        #endregion

        #region Login
        public ActionResult Login()
        {
            if (!string.IsNullOrEmpty(User.Identity.Name))
            {
                return RedirectToAction("Index", "Character");
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(AccountLoginModel login, string returnUrl)
        {

            (bool success, Guid id, string roles) = await AccountFacade.Login(login.usernameOrEmail, login.password);
            if (success)
            {
                var authTicket = new FormsAuthenticationTicket(1, id.ToString(), DateTime.Now,
                    DateTime.Now.AddMinutes(30), false, roles);
                string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                HttpContext.Response.Cookies.Add(authCookie);

                var decodedUrl = "";
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    decodedUrl = Server.UrlDecode(returnUrl);
                }

                if (Url.IsLocalUrl(decodedUrl))
                {
                    return Redirect(decodedUrl);
                }
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", "Wrong username or password!");
            return View();
        }
        #endregion

        #region Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region Edit
        public async Task<ActionResult> Edit()
        {
            var account = await AccountFacade.GetAccountAsync(Guid.Parse(User.Identity.Name));
            var accountCreateDto = new AccountCreateDto
            {
                Username = account.Username,
                Email = account.Email,
                Password = ""
            };
            return View(accountCreateDto);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(AccountCreateDto account)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await AccountFacade.EditAccountAsync(Guid.Parse(User.Identity.Name), account);
                    return RedirectToAction("Index");
                }
                catch
                {
                    return View();
                }
            }
            return View();
            
        }
        #endregion

        #region Actions
        public ActionResult GetUserName()
        {
            var syncContext = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(null);

            var account = AccountFacade.GetAccountAsync(Guid.Parse(User.Identity.Name)).Result;

            SynchronizationContext.SetSynchronizationContext(syncContext);

            return Content(account?.Username);
        }
        #endregion Actions
    }
}
