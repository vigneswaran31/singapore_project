using HelpDesk.Classes;
using HelpDeskBAL;
using HelpDeskEntity;
using HelpDeskEntity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HelpDesk.Areas.Account.Controllers
{
    public class KnowledgebaseController : BaseController
    {
        //Load Knowlwdge base Categories.
        public ActionResult Index()
        {
            List<KBCategory> lstKBCategory = new KBCategoryBL().GetAllParents();
            List<TreeModel> lstParents = new List<TreeModel>();

            foreach (var obj in lstKBCategory)
            {
                TreeModel oTreeModel = new TreeModel();
                oTreeModel.id = obj.KBCategoryId;
                oTreeModel.text = obj.CategoryName;
                oTreeModel.children = true;
                lstParents.Add(oTreeModel);
            }
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(lstParents);
            ViewBag.lstParents = jsonString;
            return View();
        }


        //Get all child Categories by Parent Category Id
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetChildNodes(int id)
        {
            List<KBCategory> lstKBCategory = new KBCategoryBL().GetChildsByParentId(id);
            var data = lstKBCategory.Select(m => new TreeModel()
            {
                id = m.KBCategoryId,
                text = m.CategoryName.ToString(),
                children = true
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //Get Articales by Category Id
        public ActionResult KBArticles(int id)
        {
            try
            {
                var lstKBArticles = new KBArticleBL().GetByCategoryId(id);
                return PartialView("_KBArticles", lstKBArticles);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Search Article by Text
        public ActionResult Search(string SearchText)
        {
            try
            {
                var lstKBArticles = new KBArticleBL().GetBySearchText(SearchText);
                return PartialView("_KBArticles", lstKBArticles);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Get Article details by Article Id
        public ActionResult ViewKBArticle(int id)
        {
            try
            {
                vw_KBArticle oKBArticle = new KBArticleBL().GetVwById(id);
                return PartialView("_ViewKBArticle", oKBArticle);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
	}
}