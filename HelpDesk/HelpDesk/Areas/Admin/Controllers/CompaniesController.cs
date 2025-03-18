using HelpDesk.Classes;
using HelpDeskBAL;
using HelpDeskEntity;
using HelpDeskEntity.Classes;
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
    public class CompaniesController : Controller
    {
        //
        // GET: /Admin/Company/
        #region Grid Methods
        public ActionResult Index()
        {
            return View();
        }

        //Load Companies in Grid Format for Display in Grid.
        public string GetGridData(GridSettings grid)
        {
            try
            {
                return new CompanyBL().GetGridData(grid);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        //Load AutoAssign in Grid Format for Display in Grid by CompanyId 
        public string GetGridDataByCompanyId(GridSettings grid, int CompanyId)
        {
            try
            {
                return new UserBL().GetGridDataByCompanyId(grid, CompanyId);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string GetContractGridDataByCompanyId(GridSettings grid, int CompanyId)
        {
            try
            {
                return new CompanyContractBL().GetGridDataByCompanyId(grid, CompanyId);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #region UI

        //Load  Tab's Page for Selected Company
        public ActionResult ManageCompany(int id, string tab = "")
        {
            ViewBag.tab = tab;
            return View(id);
        }

        //Load Selected CompanyDetail Data in Form
        public ActionResult CompanyDetail(int id)
        {
            Company oCompany = new Company();
            List<CompanyCategory> lstCategories = new CompanyCategoryBL().GetAll();
            ViewBag.ddlCategories = from Category in lstCategories
                                    select new SelectListItem
                                    {
                                        Text = Category.Name.ToString(),
                                        Value = Category.Id.ToString()
                                    };

            ViewBag.ddlParentCompany = from Parent in new CompanyBL().GetAllParents()
                                       select new SelectListItem
                                       {
                                           Text = Parent.CompanyName.ToString(),
                                           Value = Parent.CompanyId.ToString()
                                       };
            
            if (id != 0)
                oCompany = new CompanyBL().GetById(id);

            return PartialView("_CompanyDetail", oCompany);
        }

        //Load Categories List for CompanyException Update
        public ActionResult CompanyException(int id)
        {
            try
            {
                List<int> CategoryId = new CompanyCatgoryExceptionBL().GetExceptionsByCompanyId(id).Select(p => p.CategoryId).ToList();
                ViewBag.lstSprtCategories = new SupportCategoryBL().GetCategoryDDL(0);
                ViewBag.CompanyId = id;

                if (CategoryId.Count > 0)
                {
                    string SelectedCategory = "";
                    foreach (var catgryid in CategoryId)
                        SelectedCategory = SelectedCategory + catgryid + ",";

                    ViewBag.SelectedCategory = SelectedCategory.Substring(0, SelectedCategory.Count() - 1);
                }
                return PartialView("_ManageCompanyException");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult Users(int id)
        {
            ViewBag.CompanyId = id;
            return PartialView("_Users");
        }

        //Load Attachments list for Company
        public ActionResult Attachments(int id)
        {
            List<Attachment> attachments = new AttachmentsBL().GetByTypeAndId((int)En_LinkType.Company,id);
            return PartialView("_Attachments", attachments);
        }

        public ActionResult ContractHistory(int id)
        {
            ViewBag.CompanyId = id;
            return PartialView("_ContractHistory");
        }

        #endregion

        #region CRUD Methods

        #region CRUD Company Details

        //Create new or Update Existing Comapny.
        public ActionResult Save(Company oCompany)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    bool Add_Flg = new CommonBL().isNewEntry(oCompany.CompanyId);
                    if (Add_Flg)
                        new CompanyBL().Create(oCompany);
                    else
                        new CompanyBL().Update(oCompany);

                      TempData["successmsg"] = CommonMsg.Success_Update(EntityNames.Company);
                      return RedirectToAction("ManageCompany", new { id = oCompany.CompanyId, tab = "Details" });
                }
                catch (Exception ex)
                {
                    TempData["errormsg"] = CommonMsg.Error();
                    return RedirectToAction("Index");
                }
            }
            else 
            {
                    
                TempData["errormsg"] = CommonMsg.Error();
                return RedirectToAction("Index");
            }
        }

        //Delete Company by Id
        public JsonResult Delete(int id)
        {
            try
            {
                new CompanyBL().Delete(id);
                return Json(new { success = true, Message = CommonMsg.Success_Delete(EntityNames.Company) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, Message = CommonMsg.Fail_Delete(EntityNames.Company) });
            }
        }

        #endregion

        #region Company Category Exception

        //Update CategoryException for selected Comapny
        public ActionResult SaveCompCatException()
        {
            try
            {
                List<int> CategoryIds = new List<int>();
                if (Request.Form["CategoryId"]!=null)
                {
                    CategoryIds = Request.Form["CategoryId"].Split(',').Select(s => int.Parse(s)).ToList();
                }
                
                int CompanyId = Convert.ToInt32(Request.Form["CompanyId"]);

                new CompanyCatgoryExceptionBL().Delete(CompanyId);
                if (CategoryIds.Count>0)
                {
                    new CompanyCatgoryExceptionBL().Create(CategoryIds, CompanyId);
                }
                

                TempData["successmsg"] = CommonMsg.Success_Update(EntityNames.CompanyCategoryExceptions);
                return RedirectToAction("ManageCompany", new { id = CompanyId, tab = "CategoryExceptions" });
            }
            catch (Exception ex)
            {
                TempData["errormsg"] = CommonMsg.Error();
                return RedirectToAction("Index");
            }
        }

        #endregion

        #endregion

        #region Attachment


        //Insert Attachment files
        public ActionResult AddFiles()
        {
            //create orderfolder
            string CompanyId = Request.QueryString["id"];
            string uploadDirectory = Server.MapPath(Constants.CompanyImgUploadPath + "/" + CompanyId);
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
                    oAttachment.LinkId = int.Parse(CompanyId);
                    oAttachment.FileName = file.FileName;
                    oAttachment.FilePath = string.Format("{0}\\{1}", CompanyId, fileId);
                    oAttachment.LinkType = (int)En_LinkType.Company;

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
                attachmentBL.Delete(id, Constants.CompanyImgUploadPath.ToString());
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
            return File(System.IO.File.ReadAllBytes(Server.MapPath(Constants.CompanyImgUploadPath + "/" + attachment.FilePath)), contentType, attachment.FileName);
        }

        #endregion
    }
}