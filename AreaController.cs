using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Verifone.MES.Data.ViewModel;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Http;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Verifone.MES.Site.Controllers
{
    public class AreaController : BaseController
    {
        private readonly IStringLocalizer<AreaController> _localizer;

        public AreaController(IStringLocalizer<AreaController> localizer)
        {
            _localizer = localizer;
        }

        // GET: /<controller>/
        #region GET
        [HttpGet]
        public IActionResult Index()
        {
            List<ViewAreaFilter> view = new List<ViewAreaFilter>();
            ViewBag.AreaType = GetAllAreaType().Select(x => new SelectListItem { Text = x.Name, Value = x.AreaTypeId.ToString() }).ToArray();
            ViewBag.SerchArea = new ViewAreaFilter();

            string componentSearch = HttpContext.Session.GetString("AreaSearch");
            string boolSearch = HttpContext.Session.GetString("AreaBoolSearch");

            if (!String.IsNullOrEmpty(boolSearch) && !String.IsNullOrEmpty(componentSearch))
            {
                view = JsonConvert.DeserializeObject<List<ViewAreaFilter>>(componentSearch);
            }

            HttpContext.Session.SetString("AreaBoolSearch", "");
            CheckForAlerts();
            return View(view);
        }
        [HttpGet]
        public PartialViewResult Create()
        {
            ViewBag.AreaType = GetAllAreaType().Select(x => new SelectListItem { Text = x.Name, Value = x.AreaTypeId.ToString() }).ToArray();
            HttpContext.Session.SetString("AreaBoolSearch", "true");

            return PartialView();
        }

        [HttpGet]
        public PartialViewResult Edit(int id)
        {
            HttpContext.Session.SetString("AreaBoolSearch", "true");
            ViewArea entity = GetById(id);
            ViewBag.AreaType = GetAllAreaType().Select(x => new SelectListItem { Text = x.Name, Value = x.AreaTypeId.ToString() }).ToArray();
            return PartialView(entity);
        }

        [HttpGet]
        public PartialViewResult Delete(int id)
        {
            ViewArea entity = GetById(id);
            HttpContext.Session.SetString("AreaBoolSearch", "true");
            return PartialView(entity);
        }
        #endregion

        #region POST
        [HttpPost]
        public ActionResult Index([Bind("AreaId,Name,TypeName,TypeId,Code")] ViewAreaFilter entity)
        {
            if (entity.TypeId == 0)
            {
                entity.TypeId = null;
            }
            try
            {
                ViewArea viewArea = new ViewArea();

                viewArea.TypeId = entity.TypeId;
                viewArea.Name = entity.Name;
                viewArea.Code = entity.Code;
                CheckForAlerts();

                List<ViewAreaFilter> entities = JsonConvert.DeserializeObject<List<ViewAreaFilter>>(GenericRequest(true, "Area", "Search", entity));
                HttpContext.Session.SetString("AreaSearch", JsonConvert.SerializeObject(entities));
                HttpContext.Session.SetString("AreaBoolSearch", "true");
                ViewBag.AreaType = GetAllAreaType().Select(x => new SelectListItem { Text = x.Name, Value = x.AreaTypeId.ToString() }).ToArray();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                HttpContext.Session.SetString("errorMessage", ex.Message);
                return RedirectToAction("Index");
            }
           
        }

        public ViewArea GetById(int id)
        {
            ViewArea view = JsonConvert.DeserializeObject<ViewArea>(GenericRequest(true, "Area", "GetById", id));
            return view;
        }

        public List<ViewArea> GetAll()
        {
            List<ViewArea> view = JsonConvert.DeserializeObject<List<ViewArea>>(GenericRequest(true, "Area", "Getall", null));
            return view;
        }

        public ViewAreaType GetAreaTypeById(int id)
        {
            ViewAreaType view = JsonConvert.DeserializeObject<ViewAreaType>(GenericRequest(true, "AreaType", "GetById", id));
            return view;
        }

        public List<ViewAreaType> GetAllAreaType()
        {
            List<ViewAreaType> view = new List<ViewAreaType>();
            view.Add(new ViewAreaType { Name = "Select", AreaTypeId = 0 });
            view.AddRange(JsonConvert.DeserializeObject<List<ViewAreaType>>(GenericRequest(true, "AreaType", "GetAll", null)));
            return view;
        }

        [HttpPost]
        public ActionResult Create([Bind("Name,TypeId,Code")] ViewArea entity)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ViewArea entityArea = new ViewArea();
                    entityArea.Deleted = false;
                    entityArea.UserAccountId = GetCurrentUser();
                    entityArea.DateLastUpdate = DateTime.Now;
                    entityArea.Code = entity.Code;
                    entityArea.Name = entity.Name;
                    entityArea.TypeId = entity.TypeId;

                    bool result = JsonConvert.DeserializeObject<bool>(GenericRequest(true, "Area", "Save", entityArea));
                    return RedirectToAction("Index");
                }

                ViewBag.AreaType = GetAllAreaType().Select(x => new SelectListItem { Text = x.Name, Value = x.AreaTypeId.ToString() }).ToArray();
                return View(entity);
            }
            catch (Exception ex)
            {
                HttpContext.Session.SetString("errorMessage", ex.Message);
                return RedirectToAction("Index");
            }
           
        }

        [HttpPost]
        public ActionResult Edit([Bind("AreaId,Name,TypeId,Code")] ViewAreaFilter entity)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ViewArea entityArea = new ViewArea();
                    entityArea.Deleted = false;
                    entityArea.UserAccountId = GetCurrentUser();
                    entityArea.DateLastUpdate = DateTime.Now;
                    entityArea.Code = entity.Code;
                    entityArea.Name = entity.Name;
                    entityArea.TypeId = entity.TypeId;
                    entityArea.AreaId = entity.AreaId;


                    bool result = JsonConvert.DeserializeObject<bool>(GenericRequest(true, "Area", "Save", entityArea));
                    return RedirectToAction("Index");
                }

                ViewBag.AreaType = GetAllAreaType().Select(x => new SelectListItem { Text = x.Name, Value = x.AreaTypeId.ToString() }).ToArray();
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
                ViewArea entity = new ViewArea();
                entity.AreaId = id;
                bool result = JsonConvert.DeserializeObject<bool>(GenericRequest(true, "Area", "Remove", entity));
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
