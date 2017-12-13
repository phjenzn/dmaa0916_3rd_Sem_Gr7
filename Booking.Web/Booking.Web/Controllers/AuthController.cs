﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Booking.Web.Models;
using Booking.Web.Helpers;
using Booking.Web.BookingAuthRemote;

namespace Booking.Web.Controllers
{
    public class AuthController : Controller
    {
        // GET: AuthController
        public ActionResult Index()
        {
            try
            {
                ViewBag.proxy = (BookingServiceRemote.ServiceClient)ServiceHelper.GetServiceClientWithCredentials();
                ViewBag.proxyError = "";
            }
            catch (Exception ex)
            {
                ViewBag.proxyError = ex.ToString();
            }

            return View(new NewUserViewModel());
        }

        [HttpPost]
        public ActionResult Index(NewUserViewModel NewUser)
        {
            bool lykkes = false;

            ViewBag.Test = NewUser.FirstName;
            AuthClient ac = null;

            BookingServiceRemote.ServiceClient client = null;

            try
            {
                client = ServiceHelper.GetServiceClientWithCredentials();
                ac = ServiceHelper.GetAuthServiceClient();
            }
            catch (Exception ex)
            {
                ViewBag.proxyError = ex.ToString();
            }

            if(client != null || ac != null)
            {
                LoginViewModel lvm = new LoginViewModel();

                ViewBag.proxy = client;

                NewUser.Email = NewUser.Email.ToString().Trim().ToLower();
                NewUser.Password = NewUser.Password.Trim();
                NewUser.FirstName = NewUser.FirstName.Trim();
                NewUser.LastName = NewUser.LastName.Trim();
                NewUser.Address = NewUser.Address.Trim();
                NewUser.Password2 = NewUser.Password2.Trim();

                if (NewUser.Email.Length >= 6 && NewUser.Password.Length >= 4 && NewUser.FirstName.Length >= 2 && NewUser.LastName.Length >= 2 && NewUser.Address.Length >= 4 && NewUser.ZipCode > 999 && NewUser.PhoneNumber > 0 && NewUser.Password == NewUser.Password2)
                {
                    lykkes = ac.CreateLogin(NewUser.Email, NewUser.Password, NewUser.FirstName, NewUser.LastName, NewUser.Address, NewUser.ZipCode, NewUser.PhoneNumber);

                    BookingAuthRemote.User hmm;
                    if (lykkes)
                    {
                        lvm.Email = NewUser.Email;
                        lvm.Password = NewUser.Password;

                        hmm = ac.Login(lvm.Email, lvm.Password);

                        lvm.Email = hmm.Email;
                        lvm.Password = hmm.Password;

                        if (hmm.Roles.First().Name.ToString().ToLower() == "admin")
                        {
                            lvm.UserType = "Admin";
                        }
                        else if (hmm.Roles.First().Name.ToString().ToLower() == "user")
                        {
                            lvm.UserType = "User";
                        }
                        else
                        {
                            lvm.UserType = "Guest";
                        }

                        AuthHelper.Login(lvm);
                    }
                }

                if (lykkes)
                {
                    return RedirectToAction("MembersOnly", "Auth");
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return View();
            }

            
        }

        public ActionResult Login()
        {
            try
            {
                ViewBag.proxy = (BookingServiceRemote.ServiceClient)ServiceHelper.GetServiceClientWithCredentials();
                ViewBag.proxyError = "";
            }
            catch (Exception ex)
            {
                ViewBag.proxyError = ex.ToString();
            }

            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel lvm)
        {
            //canLogIn = false;
            BookingAuthRemote.User hmm;

            using (var authsvc = ServiceHelper.GetAuthServiceClient())
            {
                hmm = authsvc.Login(lvm.Email, lvm.Password);
            }

            if (hmm == null)
            {
                ViewBag.StatusMessage = "Could not login with the given credentials";
                return View();
            }
            else
            {
                lvm.Email = hmm.Email;
                lvm.Password = hmm.Password;

                if (hmm.Roles.First().Name.ToString().ToLower() == "admin")
                {
                    lvm.UserType = "Admin";
                }
                else if (hmm.Roles.First().Name.ToString().ToLower() == "user")
                {
                    lvm.UserType = "User";
                }
                else
                {
                    lvm.UserType = "Guest";
                }

                AuthHelper.Login(lvm);
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Logout()
        {

            AuthHelper.Logout();
            return RedirectToAction("Index", "Home");


        }
        public ActionResult MembersOnly()
        {
            try
            {
                ViewBag.proxy = (BookingServiceRemote.ServiceClient)ServiceHelper.GetServiceClientWithCredentials();
                ViewBag.proxyError = "";
            }
            catch (Exception ex)
            {
                ViewBag.proxyError = ex.ToString();
            }

            return View();
        }
    }
}