using HelpDeskEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDeskBAL
{
    public class SupportTeamMembersBL
    {
        #region Get Methods

        //Get all SupportTeam Members list by TeamId
        public List<SupportTeamMember> GetByTeamId(int TeamId)
        {
            try
            {
                using(var ctx = new HelpDeskEntities())
                {
                    return ctx.SupportTeamMembers.Where(p => p.TeamId == TeamId).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Get all SupportTeam Members list by MemberId
        public List<SupportTeamMember> GetAllTeamByMemberId(int MemberId)
        {
            try
            {
                using (var ctx = new HelpDeskEntities())
                {
                    return ctx.SupportTeamMembers.Where(p => p.UserId == MemberId).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region CRUD Operations

        //Create new SupportTeam.
        public void Create(List<int> UserId,int TeamId)
        {
            try
            {
                using(var ctx = new HelpDeskEntities())
                {
                    foreach(int id in UserId)
                    {
                        SupportTeamMember oSupportTeamMember = new SupportTeamMember();
                        oSupportTeamMember.TeamId = TeamId;
                        oSupportTeamMember.UserId = id;
                        oSupportTeamMember.CreatedOn = DateTime.Now;
                        ctx.SupportTeamMembers.Add(oSupportTeamMember);
                    }
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //remove SuportTeam member from team.
        public void Delete(int TeamId)
        {
            try
            {
                using(var ctx = new HelpDeskEntities())
                {
                    var lstSupportTeamMember = ctx.SupportTeamMembers.Where(p => p.TeamId == TeamId).ToList();
                    foreach(var obj in lstSupportTeamMember)
                    {
                        ctx.SupportTeamMembers.Remove(obj);
                    }
                    ctx.SaveChanges();
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
