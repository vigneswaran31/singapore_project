using HelpDesk.Classes;
using HelpDeskBAL;
using HelpDeskEntity;
using HelpDeskEntity.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HelpDesk.Areas.Admin.Controllers
{
    public class UsersController : AdminBaseController
    {
        #region Grid Methods

        //Get Users list
        public ActionResult Index()
        {
            ViewBag.ddlRole = new RoleBL().GetRoleFilter();
            return View();
        }

        //Get Users in Grid Format for Display in Grid.
        public string GetGridData(GridSettings grid)
        {
            try
            {
                User oUser = SiteUtility.GetCurrentUser();
                return new UserBL().GetGridData(grid,false,oUser.UserId);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #region UI

        //Load  Tab's Page for Selected User
        public ActionResult Manage(int id, string tab = "")
        {
            try
            {
                ViewBag.tab = tab;

                if (id > 0)
                    ViewBag.Role = new UserBL().GetById(id).RoleId == (int)En_Role.Operator ? En_Role.Operator.ToString() : "";

                return View(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Get User details by Id
        public ActionResult Details(int id = 0)
        {
            User user = new User();

            List<SelectListItem> lstRoles = new List<SelectListItem>();

            var Roles = new[]{
                    new SelectListItem{Text = En_Role.Admin.ToString(), Value = "1"},
                    new SelectListItem{Text = En_Role.Operator.ToString(), Value = "2"},
             };

            ViewBag.lstRoles = Roles.ToList();

            if (id > 0)
            {
                user = new UserBL().GetById(id);
            }
            return PartialView("_ManageDetails", user);
        }

        //Get User Permissions details by Id
        public ActionResult Permissions(int id)
        {
            try
            {
                ViewBag.OperatorId = id;
                ViewBag.lstPermission = new PermissionBL().GetAllPermission();
                return PartialView("_ManagePermission");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Get Current User Profile details
        public ActionResult MyProfile()
        {
            User oUser = SiteUtility.GetCurrentUser();
            return View(oUser);
        }

        //Get Operator Permission Permissions details by OperatorId

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadOperatorPermission(int OperatorId)
        {
            List<OperatorPermission> Permission = new OperatorPermissionBL().GetByOperatorId(OperatorId);
            var permissionData = Permission.Select(m => new SelectListItem()
            {
                Value = m.PermissionId.ToString(),
            });
            return Json(permissionData, JsonRequestBehavior.AllowGet);
        }


        


        #endregion

        #region CRUD Methods

        // Create New or Update Existing User in Db.
        public ActionResult Save(User oUser)
        {
            try
            {
                bool Add_Flg = new CommonBL().isNewEntry(oUser.UserId);
                if (Add_Flg)
                    new UserBL().Create(oUser);
                else
                    new UserBL().Update(oUser);

                TempData["successmsg"] = CommonMsg.Success(EntityNames.User, Add_Flg == true ? En_CRUD.Insert : En_CRUD.Update);

                if (oUser.RoleId == (int)En_Role.Operator)
                    return RedirectToAction("Manage", new { id = oUser.UserId, tab = "Details" });
                else
                    return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["errormsg"] = CommonMsg.Error();
                return RedirectToAction("Index");
            }
        }

        // Create New or Update Existing Permission in Db.
        public ActionResult SavePermission()
        {
            //update user permission
            List<int> PermissionIds = new List<int>();
            List<OperatorPermission> lstPermissions = new List<OperatorPermission>();
            if (Request.Form["permissionIds"] != null)
            {
                PermissionIds = Request.Form["permissionIds"].Split(',').Select(s => int.Parse(s)).ToList();
            }
            int OperatorId = Convert.ToInt32(Request.Form["OperatorId"]);

            if (PermissionIds.Count == 0)
            {
                new OperatorPermissionBL().Delete(OperatorId);
            }
            else
            {
                foreach (int pid in PermissionIds)
                {
                    OperatorPermission oOperatorPermission = new OperatorPermission();
                    oOperatorPermission.OperatorId = OperatorId;
                    oOperatorPermission.PermissionId = pid;
                    lstPermissions.Add(oOperatorPermission);
                }

                new OperatorPermissionBL().Create(lstPermissions);
            }
            TempData["successmsg"] = "Operator Permissions has been Updated successfully.";
            return RedirectToAction("Index");
        }

        //Update Currentr Users Profile in Db.
        public JsonResult UpdateProfile(User oUser)
        {
            try
            {
                oUser.UserId = SiteUtility.GetCurrentUser().UserId;
                oUser= new UserBL().UpdateProfile(oUser);
                HelpDesk.Classes.SiteUtility.SetSessionVariables(oUser.Email);
                return Json(new { success = true, message = CommonMsg.Success(EntityNames.Profile, En_CRUD.Update) });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = CommonMsg.Fail(EntityNames.Profile, En_CRUD.Update) });
            }
        }

        //Delete User by Id.
        public JsonResult Delete(int id)
        {
            try
            {
                bool result = new UserBL().Delete(id);

                if (result == true)
                    return Json(new { success = true, message = CommonMsg.Success(EntityNames.SupportTeam, En_CRUD.Delete) });
                else
                    return Json(new { success = false, message = CommonMsg.DependancyError() });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = CommonMsg.DependancyError() });
            }
        }

        #endregion

        //check Email by EmailId and UserId
        public JsonResult checkEmail(string Email, int UserId = 0)
        {
            try
            {
                var objUser = new UserBL().CheckEmail(Email, UserId);
                if (objUser == null)
                    return Json(true);
                else
                    return Json(false);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

    }
}