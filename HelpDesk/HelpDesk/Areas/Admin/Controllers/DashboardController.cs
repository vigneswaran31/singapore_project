using HelpDesk.Classes;
using HelpDeskBAL;
using HelpDeskEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HelpDesk.Areas.Admin.Controllers
{
    public class DashboardController : AdminBaseController
    {
        //Get Dashboard main page.
        public ActionResult Index()
        {
            DashboardCountModel oDasboardCountModel = new DashboardCountModel();
            oDasboardCountModel = new UserBL().GetAllCounts();
            return View(oDasboardCountModel);
        }
	}
}