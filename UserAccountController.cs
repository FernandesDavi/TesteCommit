using System.Collections.Generic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Verifone.MES.Data.Core;
using Verifone.MES.Data.Entity;
using Verifone.MES.Data.ViewModel;
using System.Linq;
using System.Net.Http;
using System.Net;
using Verifone.MES.Data.Resource;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Http;
using System;
using Newtonsoft.Json;

namespace Verifone.MES.Site.Controllers
{

    public class UserAccountController : BaseController
    {
        private readonly IStringLocalizer<UserAccountController> _localizer;

        public UserAccountController(IStringLocalizer<UserAccountController> localizer)
        {
            _localizer = localizer;
        }
        
        [HttpGet]
        public IActionResult NoAccess()
        {
            return PartialView("_NoAccess");
        }

        [HttpGet]
        public IActionResult Index()
        {
            var view = new ViewUserAccountIndex();
            CheckForAlerts();
            return View(view);
        }

        [HttpGet]
        public IActionResult Login()
        {
            var view = new ViewUserAccountLogin();
            CheckForAlerts();
            return View(view);
        }

        [HttpGet]
        public PartialViewResult Create()
        {            
            return PartialView();
        }

        [HttpGet]
        public PartialViewResult Edit(int id)
        {
            ViewUserAccountRegister view = GetById(id);            
            return PartialView(view);
        }

        [HttpGet]
        public PartialViewResult Delete(int id)
        {
            ViewUserAccountRegister view = GetById(id);
            return PartialView(view);
        }
                
        public ViewUserAccountRegister GetById(int id)
        {
            ViewUserAccountRegister view = JsonConvert.DeserializeObject<ViewUserAccountRegister>(GenericRequest(true, "UserAccount", "GetById", id));
            return view;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ViewUserAccountFilter view)
        {

            try
            {
                var viewIndex = new ViewUserAccountIndex();

                if (ModelState.IsValid)
                {
                    
                    if (ModelState.IsValid)
                    {
                        viewIndex.List = JsonConvert.DeserializeObject<List<ViewUserAccountRegister>>(GenericRequest(true, "UserAccount", "Search", view));
                    }
                }
                return View("Index", viewIndex);
            }
            catch (Exception ex)
            {
                HttpContext.Session.SetString("errorMessage", ex.Message);
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ViewUserAccountRegister view)
        {
            try
            {
                //var viewIndex = new ViewUserAccountIndex();
                //viewIndex.

                if (ModelState.IsValid)
                {
                    string result = JsonConvert.DeserializeObject<string>(GenericRequest(true, "UserAccount", "Save", view));
                    return RedirectToAction("Index");
                }
                
                return View(view);
            }
            catch (Exception ex)
            {
                HttpContext.Session.SetString("errorMessage", ex.Message);
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Login(ViewUserAccountLogin view)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var ListViewUserScreen = JsonConvert.DeserializeObject<ViewUserScreenArea>(GenericRequest(true, "UserAccount", "Authentication", view));
                    var serialisedDate = JsonConvert.SerializeObject(ListViewUserScreen);

                    //TODO Session to Cookies
                    //There are other ways to login

                    HttpContext.Session.SetString("Token", serialisedDate);                    

                    return RedirectToAction("Index", "Home");
                }

                return View(view);
            }
            catch (Exception ex)
            {
                HttpContext.Session.SetString("errorMessage", ex.Message);
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ViewUserAccountRegister view)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = JsonConvert.DeserializeObject<string>(GenericRequest(true, "UserAccount", "Save", view));
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                HttpContext.Session.SetString("errorMessage", ex.Message);
                return RedirectToAction("Index");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ViewUserAccountRegister view = new ViewUserAccountRegister();
                    view.UserAccountId = GetCurrentUser();
                    bool result = JsonConvert.DeserializeObject<bool>(GenericRequest(true, "UserAccount", "Remove", view));
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                HttpContext.Session.SetString("errorMessage", ex.Message);
                return RedirectToAction("Index");
            }
        }
        

    }
}
