using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Verifone.MES.Data.ViewModel;

namespace Verifone.MES.Site.Controllers
{

    public class FamilyController : BaseController
    {
        private readonly IStringLocalizer<FamilyController> _localizer;

        public FamilyController(IStringLocalizer<FamilyController> localizer)
        {
            _localizer = localizer;
        }
        #region GET
        public FamilyController(IStringLocalizer<FamilyController> localizer)
        {
            _localizer = localizer;
        }
        [HttpGet]
        public IActionResult Index()
        {
            List<ViewFamilyFilter> entities = new List<ViewFamilyFilter>();
            ViewBag.SearchFamily = new ViewFamilyFilter();
            string familySearch = HttpContext.Session.GetString("FamilySearch");
            string boolSearch = HttpContext.Session.GetString("FamilyBoolSearch");

            if (!String.IsNullOrEmpty(boolSearch) && !String.IsNullOrEmpty(familySearch))
            {
                entities = JsonConvert.DeserializeObject<List<ViewFamilyFilter>>(familySearch);
            }

            HttpContext.Session.SetString("FamilyBoolSearch", "");
            CheckForAlerts();
            return View(entities);
        }

        [HttpGet]
        public PartialViewResult Create()
        {

            HttpContext.Session.SetString("FamilyBoolSearch", "true");
            return PartialView();
        }

        [HttpGet]
        public PartialViewResult Edit(int id)
        {
            HttpContext.Session.SetString("FamilyBoolSearch", "true");
            ViewFamily entity = GetById(id);
            return PartialView(entity);
        }

        [HttpGet]
        public PartialViewResult Delete(int id)
        {
            HttpContext.Session.SetString("FamilyBoolSearch", "true");
            ViewFamily entity = GetById(id);
            return PartialView(entity);
        }
        #endregion

        #region POST
        [HttpPost]
        public ActionResult Index([Bind("Id,Name,Description")] ViewFamilyFilter entity)
        {

            try
            {
                List<ViewFamilyFilter> entities = JsonConvert.DeserializeObject<List<ViewFamilyFilter>>(GenericRequest(true, "Family", "Search", entity));
                HttpContext.Session.SetString("FamilySearch", JsonConvert.SerializeObject(entities));
                HttpContext.Session.SetString("FamilyBoolSearch", "true");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                HttpContext.Session.SetString("errorMessage", ex.Message);
                return RedirectToAction("Index");
            }

        }

        [HttpPost]
        public ActionResult Create([Bind("Id,Name,Description")] ViewFamily entity)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool result = JsonConvert.DeserializeObject<bool>(GenericRequest(true, "Family", "Save", entity));
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
        public ActionResult Edit([Bind("Id,Name,Description")] ViewFamily entity)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //entity.Deleted = false;
                    //entity.UserAccountId = GetCurrentUser();
                    //entity.DateLastUpdate = DateTime.Now;

                    bool result = JsonConvert.DeserializeObject<bool>(GenericRequest(true, "Family", "Save", entity));
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
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                ViewId entity = new ViewId();
                entity.Id = id;
                bool result = JsonConvert.DeserializeObject<bool>(GenericRequest(true, "Family", "Delete", entity));
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                HttpContext.Session.SetString("errorMessage", ex.Message);
                return RedirectToAction("Index");
            }
        }

        public ViewFamily GetById(int id)
        {
            ViewFamily entity = JsonConvert.DeserializeObject<ViewFamily>(GenericRequest(true, "Family", "GetById", id));
            return entity;
        }

        #endregion
    }
}