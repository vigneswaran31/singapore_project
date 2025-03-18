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
    public class ConfigsController : AdminBaseController
    {
        //Get Config details list.
        public ActionResult Index()
        {
           List<Config> lst = new ConfigsBL().GetAll();
           return View(lst);
        }

        //Create new or Update Existing Config details
        public ActionResult Save(List<Config> lstConfig)
        {
            try
            {
                new ConfigsBL().Update(lstConfig);
                TempData["successmsg"] = CommonMsg.Success_Update(EntityNames.SiteConfig);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["errormsg"] = CommonMsg.Error();
                return RedirectToAction("Index");
            }
        }
	}
}