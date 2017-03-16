using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Verifone.MES.Data.ViewModel;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Http;

namespace Verifone.MES.Site.Controllers
{
    public class ComponentController : BaseController
    {

        private readonly IStringLocalizer<ComponentController> _localizer;

        public ComponentController(IStringLocalizer<ComponentController> localizer)
        {
            _localizer = localizer;
        }
        
        #region GET
        public string a(){
            println("a");
        }
        [HttpGet]
        public IActionResult Index()
        {
            List<ViewComponents> entities = new List<ViewComponents>();
            ViewBag.SearchModel = new ViewComponentFilter() { Status = true };
            string componentSearch = HttpContext.Session.GetString("componentSearch");
            string boolSearch = HttpContext.Session.GetString("componentBoolSearch");

            if (!String.IsNullOrEmpty(boolSearch) && !String.IsNullOrEmpty(componentSearch))
            {
                entities = JsonConvert.DeserializeObject<List<ViewComponents>>(componentSearch);
            }

            HttpContext.Session.SetString("componentBoolSearch", "");
            CheckForAlerts();
            return View(entities);
        }

        [HttpGet]
        public PartialViewResult Create()
        {
            HttpContext.Session.SetString("componentBoolSearch", "true");
            return PartialView();
        }

        [HttpGet]
        public PartialViewResult Edit(int id)
        {
            HttpContext.Session.SetString("componentBoolSearch", "true");
            ViewComponents entity = GetById(id);
            return PartialView(entity);
        }

        [HttpGet]
        public PartialViewResult Delete(int id)
        {
            HttpContext.Session.SetString("componentBoolSearch", "true");
            ViewComponents entity = GetById(id);
            return PartialView(entity);
        }

        #endregion

        #region POST

        public ViewComponents GetById(int id)
        {
            ViewComponents entity = JsonConvert.DeserializeObject<ViewComponents>(GenericRequest(true, "Component", "GetById", id));
            return entity;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind("ComponentId,Name,Type,Status")] ViewComponentFilter entity)
        {
            try
            {
                List<ViewComponents> entities = JsonConvert.DeserializeObject<List<ViewComponents>>(GenericRequest(true, "Component", "Search", entity));
                HttpContext.Session.SetString("componentSearch", JsonConvert.SerializeObject(entities));
                HttpContext.Session.SetString("componentBoolSearch", "true");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                HttpContext.Session.SetString("errorMessage", ex.Message);
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind("ComponentId,Name,Type,Status")] ViewComponents entity)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    entity.Deleted = false;
                    entity.UserAccountId = GetCurrentUser();
                    entity.DateLastUpdate = DateTime.Now;

                    bool result = JsonConvert.DeserializeObject<bool>(GenericRequest(true, "Component", "Save", entity));
                    return RedirectToAction("Index");
                }

                return View(entity);
            }
            catch (Exception ex)
            {
                HttpContext.Session.SetString("errorMessage", ex.Message);
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind("ComponentId,Name,Type,Status")] ViewComponents entity)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    entity.Deleted = false;
                    entity.UserAccountId = GetCurrentUser();
                    entity.DateLastUpdate = DateTime.Now;

                    bool result = JsonConvert.DeserializeObject<bool>(GenericRequest(true, "Component", "Save", entity));
                    return RedirectToAction("Index");
                }

                return View(entity);
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
                ViewComponents entity = new ViewComponents();
                entity.ComponentId = id;
                entity.UserAccountId = GetCurrentUser();
                entity.DateLastUpdate = DateTime.Now;

                bool result = JsonConvert.DeserializeObject<bool>(GenericRequest(true, "Component", "Remove", entity));
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                HttpContext.Session.SetString("errorMessage", ex.Message);
                return RedirectToAction("Index");
            }
        }

        #endregion
    }

}
