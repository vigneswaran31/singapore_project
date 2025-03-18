<%@ Page language="c#" %>
<%@ Register TagPrefix="uc1" TagName="TopBanner" Src="banner.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Footer" Src="Footer.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head>
		<title>Live Support Dashboard</title>
		<link rel='stylesheet' href='style.css' />
	</head>
	<body>
        <form id="Form1" method="post" runat="server">
            <uc1:TopBanner ID="Banner1" runat="server"></uc1:TopBanner>
            <div class="infobox" style="width:700px; margin: 30px auto; min-height:300px;">
                <h2>
                    Dashboard
                </h2>
                <div class="padding10">
                    <table cellspacing="0" border="0" cellpadding="20" style="width:600px;margin:10px auto;">
                        <tr>
                            <td>
                                <img src="images/new-Operators.gif" alt="" style="vertical-align:middle; margin:3px;" />
                                <a style="font-size:16px;" href="ManageUsers.aspx">Manage Operators</a>
                            </td>
                            <td>
                                <img src="images/Manage-Departments.gif" alt="" style="vertical-align:middle; margin:3px;" />
                                <a style="font-size:16px;" href="ManageDepartments.aspx">Manage Departments</a>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <img src="images/moderator.gif" alt="" style="vertical-align:middle; margin:3px;" />
                                <a style="font-size:16px;" href="ManageAdministrators.aspx">Manage Administrators</a>
                            </td>
                            <td>
                                <img src="images/lock.gif" alt="" style="vertical-align:middle; margin:3px;" />
                                <a style="font-size:16px;" href="CuteSoft_Client/CuteChat/SupportAdmin/">Advanced Settings</a>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
            <uc1:Footer ID="Footer1" runat="server"></uc1:Footer>
        </form>
	</body>
</html>
<script runat="server">
protected override void OnLoad(EventArgs e)
{
	CuteChat.AppChatIdentity identity=CuteChat.ChatProvider.Instance.GetLogonIdentity();
	if(identity==null)
	{
		Response.Redirect("Login.aspx",true);
		return;
	}
	if(!CuteChat.ChatProvider.Instance.IsAdministrator(identity.UniqueId))
	{
		Response.Redirect("Login.aspx",true);
		return;
	}

	base.OnLoad (e);
}

</script>
