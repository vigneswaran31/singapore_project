using HelpDeskEntity;
using HelpDeskEntity.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HelpDeskBAL
{
    public class CommentBL
    {
        #region Get Methods

        //Get Operator Comments List records by Support TicketId.
        public List<vw_Comments> GetOperatorComment(Guid TicketId)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.vw_Comments.Where(p => p.TicketId == TicketId).OrderByDescending(p=>p.CreatedOn).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Get Customer Comments List records by Support TicketId.
        public List<vw_Comments> GetCustomerComment(Guid TicketId)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.vw_Comments.Where(p => p.TicketId == TicketId && p.IsPrivate == false).OrderByDescending(p => p.CreatedOn).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region CRUD Operations

        //Create a new Comment record.
        public Comment Create(Comment oComment)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    oComment.CreatedOn = DateTime.Now;
                    ctx.Comments.Add(oComment);
                    ctx.SaveChanges();
                    return oComment;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Delete all Comments record by TicketId.
        public void Delete(Guid id)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    List<Comment> lstComments = ctx.Comments.Where(c => c.TicketId == id).ToList();
                   foreach (Comment item in lstComments)
                   {
                       ctx.Comments.Remove(item);
                       ctx.SaveChanges();
                   }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
