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
    public class TicketSourceController : Controller
    {
      
        #region Grid Methods

        //
        // GET: /Admin/TicketSource/
        public ActionResult Index()
        {
            return View();
        }

        //Get TicketSource in Grid Format for Display in Grid.
        public string GetGridData(GridSettings grid)
        {
            try
            {
               return new TicketsSourcesBL().GetGridData(grid);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #region UI
        
        // Manage Ticket source
        public ActionResult Manage(int id = 0)
        {
            try
            {
                TicketsSource oTicketsSource = new TicketsSource();
                if (id > 0)
                    oTicketsSource = new TicketsSourcesBL().GetById(id);
                return View(oTicketsSource);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region CRUD Methods
       
        // Save Ticket Source
        public JsonResult Save(TicketsSource oTicketsSource)
        {
            try
            {
                bool Add_Flg = new CommonBL().isNewEntry(oTicketsSource.Id);

                if (Add_Flg)
                    new TicketsSourcesBL().Create(oTicketsSource);
                else
                    new TicketsSourcesBL().Update(oTicketsSource);

                return Json(new { success = true, message = CommonMsg.Success(EntityNames.TicketSource, Add_Flg == true ? En_CRUD.Insert : En_CRUD.Update) });

            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = CommonMsg.Error() });
            }
        }

        // Delete Ticket Source
        public JsonResult Delete(int id)
        {
            try
            {
                bool result = new TicketsSourcesBL().Delete(id);

                if (result == true)
                    return Json(new { success = true, message = CommonMsg.Success(EntityNames.TicketSource, En_CRUD.Delete) });
                else
                    return Json(new { success = false, message = CommonMsg.DependancyError() });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = CommonMsg.DependancyError() });
            }
        }

        #endregion
	}
}