using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using HelpDesk.Classes;
using CuteChat;
using System.Data;
using System.Collections;
using HelpDeskEntity.Classes;
using HelpDeskBAL;
using HelpDeskEntity;
using HelpDeskEntity.Models;

namespace HelpDesk
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            if (User.Identity.IsAuthenticated)
            {
                SiteUtility.SetSessionVariables(User.Identity.Name);
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            Response.Clear();

            HttpException httpException = exception as HttpException;
            string action;

            if (httpException != null)
            {
                switch (httpException.GetHttpCode())
                {
                    case 404:
                        // page not found
                        action = "HttpError404";
                        break;
                    case 500:
                        // server error
                        action = "HttpError500";
                        break;
                    default:
                        action = "General";
                        break;
                }
            }
            else
                action = "General";

            // clear error on server
            Server.ClearError();
            
            Response.Redirect(String.Format("~/Error/{0}?message={1}", action, exception.Message));
        }

        #region Live Chat Provider
        public override void Init()
        {
            base.Init();

            lock (typeof(CuteChat.ChatSystem))
            {
                if (!CuteChat.ChatSystem.HasStarted)
                {
                    CuteChat.ChatProvider.Instance = new ExampleProvider();
                    CuteChat.ChatSystem.Start(new CuteChat.AppSystem());
                }
            }
        }

        public class ExampleProvider : ChatProvider
        {            
            static string LowerUserName = "LOWER(Email)";
            static string LowerLoginName = "LOWER(Email)";

            static string SqlEncode(string value)
            {
                if (value == null) return "NULL";
                return "'" + value.ToString().Replace("'", "''") + "'";
            }

            public override string GetConnectionString()
            {
                return System.Configuration.ConfigurationManager.AppSettings["ConnectionString"];
            }

            /// <summary>
            /// Get the information of the current user
            /// </summary>
            public override AppChatIdentity GetLogonIdentity()
            {
                if(!HttpContext.Current.User.Identity.IsAuthenticated)
                    return null;
                
                return new AppChatIdentity(HttpContext.Current.User.Identity.Name, false, 
                    ToUserId(HttpContext.Current.User.Identity.Name), HttpContext.Current.Request.UserHostAddress);
            }

            /// <summary>
            /// find the username by the displayname
            /// </summary>
            public override string FindUserLoginName(string UserName)
            {
                if (UserName == null) return null;

                using (IDbConnection conn = CreateConnection())
                {
                    conn.Open();

                    using (IDbCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT Name FROM Users WHERE RoleId in (1,2) AND " + LowerUserName + "=" + SqlEncode(UserName.ToLower());

                        using (IDataReader sdr = cmd.ExecuteReader())
                        {
                            if (sdr.Read())
                            {
                                return sdr.GetString(0);
                            }

                        }
                    }
                }

                return null;
            }

            /// <summary>
            /// get the information from the user
            /// This function is very important and be called very frequently. 
            /// </summary>
            public override bool GetUserInfo(string loginName, ref string UserName, ref bool isAdmin)
            {
                string password = null;
                return GetUserInfo(loginName, ref UserName, ref password, ref isAdmin);
            }

            /// <summary>
            /// validate the user , and set the cookie
            /// </summary>
            public override bool ValidateUser(string loginName, string pwd)
            {
                string UserName = null;
                string password = null;
                bool isAdmin = false;
                bool exists = GetUserInfo(loginName, ref UserName, ref password, ref isAdmin);
                pwd = Utility.CommonFunction.GetSHAHash(pwd, loginName);
                if (!exists) return false;
                if (password != pwd) return false;

                System.Web.Security.FormsAuthentication.SetAuthCookie(loginName, false, HttpRuntime.AppDomainAppVirtualPath);

                return true;
            }

            /// <summary>
            /// This function is very important and be called very frequently. 
            /// </summary>
            public static bool GetUserInfo(string loginName, ref string UserName, ref string password, ref bool isAdmin)
            {
                loginName = loginName.ToLower();
                Hashtable table = (Hashtable)System.Web.HttpRuntime.Cache["UserInfo:" + loginName];
                if (table == null)
                {
                    using (System.Data.IDbConnection conn = Instance.CreateConnection())
                    {
                        conn.Open();
                        using (System.Data.IDbCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "SELECT * FROM Users WHERE RoleId in (1,2) AND " + LowerLoginName + "=" + SqlEncode(loginName);

                            using (System.Data.IDataReader sdr = cmd.ExecuteReader())
                            {
                                if (!sdr.Read())
                                    return false;

                                table = new Hashtable(new CaseInsensitiveHashCodeProvider(), new CaseInsensitiveComparer());
                                for (int i = 0; i < sdr.FieldCount; i++)
                                    table[sdr.GetName(i)] = sdr.IsDBNull(i) ? null : sdr.GetValue(i);
                            }
                        }
                    }

                    System.Web.HttpRuntime.Cache["UserInfo:" + loginName] = table;
                }
                UserName = table["Email"].ToString();
                password = table["Password"].ToString();
                isAdmin = true;//(1 == Convert.ToInt32(table["RoleId"]));//if admin
                if (!isAdmin) //if not admin check permission for operator
                {
                    List<OperatorPermission> lstPermissions = new OperatorPermissionBL().GetByOperatorId((int)table["UserId"]);
                    if (lstPermissions.Count <= 0)
                        return false;
                    else if (lstPermissions.Where(p => p.PermissionId == (int)En_Permission.AllowSupportChat).FirstOrDefault() == null)
                        return false;
                }
                return true;
            }

        }

        #endregion
    }
}
