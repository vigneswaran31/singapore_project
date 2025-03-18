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
    public class SupportTeamsController : AdminBaseController
    {
        #region Grid Methods

        //Get Support Teams list
        public ActionResult Index()
        {
            return View();
        }

        //Get Support Teams in Grid Format for Display in Grid.
        public string GetGridData(GridSettings grid)
        {
            try
            {
                return new SupportTeamBL().GetGridData(grid);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #region UI

        //Load  Tab's Page for Selected Support Team
        public ActionResult Manage(int id, string tab = "")
        {
            try
            {
                ViewBag.tab = tab;
                return View(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Get Support Team details by Id
        public ActionResult Details(int id)
        {
            try
            {
                SupportTeam oSupportTeam = new SupportTeam();
                if (id > 0)
                    oSupportTeam = new SupportTeamBL().GetById(id);
                return PartialView("_ManageDetails", oSupportTeam);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Get Support Team Members details by Id
        public ActionResult Members(int id)
        {
            try
            {
               List<int> UserId = new SupportTeamMembersBL().GetByTeamId(id).Select(p => p.UserId).ToList();
                ViewBag.lstUser = new UserBL().GetAllByRole(En_Role.Operator.ToString());
                ViewBag.TeamId = id;

                if (UserId.Count > 0)
                {
                    string SelectedUser = "";
                    foreach (var userid in UserId)
                        SelectedUser = SelectedUser + userid + ",";

                    ViewBag.SelectedUsers = SelectedUser.Substring(0, SelectedUser.Count() - 1);
                }

                return PartialView("_ManageMembers");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Get Support Team Permissions details by Id
        public ActionResult Permissions(int id)
        {
            try
            {
                ViewBag.TeamId = id;
                ViewBag.lstPermission = new PermissionBL().GetAllPermission();
                return PartialView("_ManagePermission");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadSupportTeamPermission(int TeamId)
        {
            List<SupportTeamPermission> Permission = new SupportTeamPermissionBL().GetByTeamId(TeamId);
            var permissionData = Permission.Select(m => new SelectListItem()
            {
                Value = m.PermissionId.ToString(),
            });
            return Json(permissionData, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region  CRUD Methods

        // Create New or Update Existing Support Team in Db.
        public ActionResult SaveDetails(SupportTeam oSupportTeam)
        {
            try
            {
                bool Add_Flg = new CommonBL().isNewEntry(oSupportTeam.TeamId);
                if (Add_Flg)
                    new SupportTeamBL().Create(oSupportTeam);
                else
                    new SupportTeamBL().Update(oSupportTeam);

                TempData["successmsg"] = CommonMsg.Success(EntityNames.SupportTeam, Add_Flg == true ? En_CRUD.Insert : En_CRUD.Update);
                return RedirectToAction("Manage", new { id = oSupportTeam.TeamId, tab = "Details" });
            }
            catch (Exception ex)
            {
                TempData["errormsg"] = CommonMsg.Error();
                return RedirectToAction("Index");
            }
        }

        // Create New or Update Existing Support Team Members in Db.
        public ActionResult SaveMembers()
        {
            try
            {
                 List<int> UserIds = new List<int>();

                if(Request.Form["UserId"] != null)
                   UserIds = Request.Form["UserId"].Split(',').Select(s => int.Parse(s)).ToList();

                int TeamId = Convert.ToInt32(Request.Form["TeamId"]);

                new SupportTeamMembersBL().Delete(TeamId);

                if (UserIds.Count > 0)
                     new SupportTeamMembersBL().Create(UserIds, TeamId);
                
                TempData["successmsg"] = CommonMsg.Success_Update(EntityNames.SupportTeamMembers);
                return RedirectToAction("Manage", new { id = TeamId, tab = "Members" });
            }
            catch (Exception ex)
            {
                TempData["errormsg"] = CommonMsg.Error();
                return RedirectToAction("Index");
            }
        }

        // Create New or Update Existing Support Team Permission in Db.
        public ActionResult SavePermission()
        {
            //update user permission
            List<int> PermissionIds = new List<int>();
            List<SupportTeamPermission> lstPermissions = new List<SupportTeamPermission>();
            if (Request.Form["permissionIds"] != null)
            {
                PermissionIds = Request.Form["permissionIds"].Split(',').Select(s => int.Parse(s)).ToList();
            }
            int TeamId = Convert.ToInt32(Request.Form["TeamId"]);

            if (PermissionIds.Count == 0)
            {
                new SupportTeamPermissionBL().Delete(TeamId);
            }
            else
            {
                foreach (int pid in PermissionIds)
                {
                    SupportTeamPermission oSupportTeamPermission = new SupportTeamPermission();
                    oSupportTeamPermission.TeamId = TeamId;
                    oSupportTeamPermission.PermissionId = pid;
                    lstPermissions.Add(oSupportTeamPermission);
                }

                new SupportTeamPermissionBL().Create(lstPermissions);
            }
            TempData["successmsg"] = "Support Team Permissions has been Updated successfully.";
            return RedirectToAction("Index");
        }

        //Delete Support Team by Id.
        public JsonResult Delete(int id = 0)
        {
            try
            {
                bool result = new SupportTeamBL().Delete(id);
                //Success
                if (result == true)
                    return Json(new { success = true, message = CommonMsg.Success(EntityNames.SupportTeam, En_CRUD.Delete) });
                else
                    return Json(new { success = false, message = CommonMsg.DependancyError() });
            }
            catch (Exception ex)
            {
                //Error
                return Json(new { success = false, message = CommonMsg.DependancyError() });
            }
        }

        #endregion
	}
}