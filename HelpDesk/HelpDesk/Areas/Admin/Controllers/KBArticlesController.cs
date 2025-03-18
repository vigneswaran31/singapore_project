using HelpDesk.Classes;
using HelpDeskBAL;
using HelpDeskEntity;
using HelpDeskEntity.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utility;

namespace HelpDesk.Areas.Admin.Controllers
{
    public class KBArticlesController : AdminBaseController
    {
        //
        // GET: /Admin/KBArticle/
        #region Grid Methods

        //Get Articles list
        public ActionResult Index()
        {
            return View();
        }

        //Get Articals in Grid Format for Display in Grid.
        public string GetGridData(GridSettings grid)
        {
            try
            {
                return new KBArticleBL().GetAllArticles(grid);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #region UI

        //Get Artical page Tabs
        public ActionResult Manage(int id, string tab = "")
        {
            ViewBag.tab = tab;
            return View(id);
        }

        //Get Articles details by Id
        public ActionResult ManageArticle(int id = 0)
        {
            KBArticle Article = new KBArticle();
            ViewBag.lstCategories = new KBCategoryBL().GetCategoryDDL(2);

            if (id > 0)
                Article = new KBArticleBL().GetById(id);

            List<Attachment> attachments = new AttachmentsBL().GetByTypeAndId((int)En_LinkType.KBArticle, id);
            ViewBag.Attachments = JsonConvert.SerializeObject(attachments);

            return PartialView("_ManageArticle", Article);
        }

        //Get Attachments list by Artical ID
        public ActionResult Attachments(int id)
        {
            List<Attachment> attachments = new AttachmentsBL().GetByTypeAndId((int)En_LinkType.KBArticle, id);
            return PartialView("_Attachments", attachments);
        }

        #endregion

        #region  CRUD Methods

        // Create New or Update Existing Article in Db.
        [ValidateInput(false)]

        public JsonResult Save(KBArticle Article, FormCollection collection)
        {
            try
            {
                bool Add_Flg = new CommonBL().isNewEntry(Article.ArticleId);

                if (Add_Flg)
                {
                    User User = (User)Session[En_UserSession.User.ToString()];
                    Article.AuthorId = User.UserId;
                    new KBArticleBL().Create(Article);
                }
                else
                    new KBArticleBL().Update(Article);

                return Json(new { success = true, message = CommonMsg.Success(EntityNames.Article, Add_Flg == true ? En_CRUD.Insert : En_CRUD.Update), id = Article.ArticleId });

            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = CommonMsg.Error() });
            }
        }

        //Delete Articles
        public JsonResult Delete(int id = 0)
        {
            try
            {
                new KBArticleBL().Delete(id);
                return Json(new { success = true, message = CommonMsg.Success(EntityNames.Article, En_CRUD.Delete) });
            }
            catch (Exception ex)
            {
                //Error
                return Json(new { success = false, message = CommonMsg.Fail_Delete(EntityNames.Article) });
            }
        }

        #endregion


        #region Attachment

        //Insert Attachment files
        public ActionResult AddFiles()
        {
            //create orderfolder
            string ArticleId = Request.QueryString["id"];
            string uploadDirectory = Server.MapPath(Constants.ArticleImgUploadPath + "/" + ArticleId);
            if (!System.IO.Directory.Exists(uploadDirectory))
                System.IO.Directory.CreateDirectory(uploadDirectory);

            string fileId = "";
            foreach (string fileName in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[fileName];
                if (file != null && file.ContentLength > 0)
                {
                    fileId = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                    if (CommonFunction.IsImage(file.FileName))
                    {
                        Image actualImg = Image.FromStream(file.InputStream);
                        actualImg.Save(string.Format("{0}\\{1}", uploadDirectory, fileId));

                        //create thumb and save
                        var thumbImg = CommonFunction.ResizeImage(actualImg, 160, 160);
                        string thumbImgName = Path.GetFileNameWithoutExtension(fileId) + "_thumb" + Path.GetExtension(fileId);
                        thumbImg.Save(string.Format("{0}\\{1}", uploadDirectory, thumbImgName));
                    }
                    else
                    {   //save file
                        file.SaveAs(string.Format("{0}\\{1}", uploadDirectory, fileId));
                    }

                    //save to db
                    Attachment oAttachment = new Attachment();
                    oAttachment.LinkId = int.Parse(ArticleId);
                    oAttachment.FileName = file.FileName;
                    oAttachment.FilePath = string.Format("{0}\\{1}", ArticleId, fileId);
                    oAttachment.LinkType = (int)En_LinkType.KBArticle;

                    new AttachmentsBL().Create(oAttachment);
                }
            }

            return Json(new { Message = string.Empty });
        }

        // delete attchments
        public JsonResult DeleteFile(int id)
        {
            try
            {
                AttachmentsBL attachmentBL = new AttachmentsBL();
                attachmentBL.Delete(id, Constants.ArticleImgUploadPath.ToString());
                return Json(new { success = true, message = CommonMsg.Success(EntityNames.Attachment, En_CRUD.Delete) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = CommonMsg.Fail(EntityNames.Attachment, En_CRUD.Update) });
            }
        }

        // download file
        public FileContentResult DownloadFile(int id)
        {
            Attachment attachment = new AttachmentsBL().GetById(id);
            string contentType = CommonFunction.GetMimeType(Path.GetExtension(attachment.FileName));
            return File(System.IO.File.ReadAllBytes(Server.MapPath(Constants.ArticleImgUploadPath + "/" + attachment.FilePath)), contentType, attachment.FileName);
        }

        #endregion
    }
}