using HelpDeskBAL;
using HelpDeskEntity;
using HelpDeskEntity.Classes;
using HelpDeskEntity.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Web;

namespace HelpDesk.Classes
{
    public static class SiteUtility
    {
        public static void SetSessionVariables(string username)
        {
            User oUser = new UserBL().GetByUserName(username);
            System.Web.HttpContext.Current.Session[En_UserSession.User.ToString()] = oUser;

            OptPermission oOptPermission = new OptPermission();
            oOptPermission.PermissionId = new OperatorPermissionBL().GetOperatorPermissionsByOptId(oUser.UserId);
            System.Web.HttpContext.Current.Session[En_UserSession.OperatorPermission.ToString()] = oOptPermission;

            List<int> lstTeamId = new SupportTeamMembersBL().GetAllTeamByMemberId(oUser.UserId).Select(t => t.TeamId).ToList();
            System.Web.HttpContext.Current.Session[En_UserSession.SupportTeams.ToString()] = lstTeamId;

        }

        public static User GetCurrentUser()
        {
            return (User)System.Web.HttpContext.Current.Session[En_UserSession.User.ToString()];
        }

        public static bool IsAllow(int PermissionType)
        {
            User oUser = GetCurrentUser();
            OptPermission oOptPermission = (OptPermission)System.Web.HttpContext.Current.Session[En_UserSession.OperatorPermission.ToString()];
            if (oOptPermission.PermissionId.Contains(PermissionType))
                return true;
            else
                return false;
        }

        public static Image ResizeImage(Image Image, int Width, int Height)
        {
            int newwidth = Image.Width;
            int newheight = Image.Height;
            if (newwidth > Width || newheight > Height)
            {
                //The flips are in here to prevent any embedded image thumbnails -- usually from cameras
                //from displaying as the thumbnail image later, in other words, we want a clean
                //resize, not a grainy one.
                Image.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipX);
                Image.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipX);
                float ratio = 0;
                if (newwidth > newheight)
                {
                    ratio = (float)newwidth / (float)newheight;
                    newwidth = Width;
                    newheight = Convert.ToInt32(Math.Round((float)newwidth / ratio));

                }
                else
                {
                    ratio = (float)newheight / (float)newwidth;
                    newheight = Height;
                    newwidth = Convert.ToInt32(Math.Round((float)newheight / ratio));
                }
                var newImage = new Bitmap(newwidth, newheight);
                Graphics.FromImage(newImage).DrawImage(Image, 0, 0, newwidth, newheight);
                Bitmap bmp = new Bitmap(newImage);

                return bmp;
            }
            else
                return Image;
        }

        public static string GetImageUrl()
        {
            //return "http://localhost:2400/uploads";
            string ImagePath = ConfigurationManager.AppSettings["ImagePath"];
            return ImagePath;
        }
    }
}