using HelpDesk.Classes;
using HelpDeskBAL;
using HelpDeskEntity;
using HelpDeskEntity.Classes;
using HelpDeskEntity.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utility;

namespace HelpDesk.Controllers
{
    public class KnowledgebaseController : Controller
    {
        private void LanguageHeader()
        {
            // Language Header
            ViewBag.LngWelcome = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.Welcome);
            ViewBag.LngChangePassword = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.ChangePassword);
            ViewBag.LngMyProfile = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.MyProfile);
            ViewBag.LngLogout = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.Logout);
        }

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

            List<KBCategory> lstAllCategory = new KBCategoryBL().GetAll();
            ViewBag.lstKBCategory = lstAllCategory;

            //Language
            ViewBag.LngSearch = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.Search);
            ViewBag.LngKBCategories = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.KBCategories);

            LanguageHeader();

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
        public ActionResult Articles(int cid)
        {
            try
            {
                var lstKBArticle = new KBArticleBL().GetByCategoryId(cid);
                ViewBag.IsSearch = false;
                List<KBCategory> lstAllCategory = new KBCategoryBL().GetAll();
                ViewBag.lstKBCategory = lstAllCategory;
                ViewBag.KBCategoryId = cid;

                //Language
                ViewBag.LngSearchResults = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.SearchResults);
                ViewBag.LngKnowledgeBaseArticles = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.KnowledgeBaseArticles);
                ViewBag.LngArticleTitle = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.ArticleTitle);
                ViewBag.LngRead = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.Read);
                ViewBag.LngNoArticlesFound = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.NoArticlesFound);
                ViewBag.LngWelcomeToOurKnowledgeBase = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.WelcomeToOurKnowledgeBase);
                ViewBag.LngThisIsAListOfArticlesFound_ = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.ThisIsAListOfArticlesFound_);

                LanguageHeader();

                return View(lstKBArticle);
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
                List<KBCategory> lstAllCategory = new KBCategoryBL().GetAll();
                ViewBag.lstKBCategory = lstAllCategory;
                ViewBag.IsSearch = true;

                //Language
                ViewBag.LngSearchResults = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.SearchResults);
                ViewBag.LngKnowledgeBaseArticles = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.KnowledgeBaseArticles);
                ViewBag.LngArticleTitle = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.ArticleTitle);
                ViewBag.LngRead = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.Read);
                ViewBag.LngNoArticlesFound = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.NoArticlesFound);
                ViewBag.LngWelcomeToOurKnowledgeBase = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.WelcomeToOurKnowledgeBase);
                ViewBag.LngThisIsAListOfArticlesFound_ = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.ThisIsAListOfArticlesFound_);

                LanguageHeader();

                return View("Articles", lstKBArticles);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Download Attachment File by Id
        public FileContentResult DownloadFile(int id)
        {
            Attachment attachment = new AttachmentsBL().GetById(id);
            string contentType = CommonFunction.GetMimeType(Path.GetExtension(attachment.FileName));
            return File(System.IO.File.ReadAllBytes(Server.MapPath(Constants.ArticleImgUploadPath + "/" + attachment.FilePath)), contentType, attachment.FileName);
        }

        //Get Article details by Article Id
        public ActionResult Article(int id)
        {
            try
            {
                var oKBArticle = new KBArticleBL().GetVwById(id);
                List<KBCategory> lstAllCategory = new KBCategoryBL().GetAll();
                ViewBag.lstKBCategory = lstAllCategory;
                ViewBag.KBCategoryId = oKBArticle.KBCategoryId;

                //Language
                ViewBag.LngAttachments = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.Attachments);
                ViewBag.LngNoFilesAttached = Utility.CommonFunction.GetLanguage(Utility.CommonFunction.DefaultLanguage, Properties.Settings.Default.NoFilesAttached);

                LanguageHeader();

                return View(oKBArticle);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}