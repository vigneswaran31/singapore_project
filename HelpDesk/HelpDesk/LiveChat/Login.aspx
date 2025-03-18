<%@ Page language="c#" %>
<%@ Register TagPrefix="uc1" TagName="TopBanner" Src="banner.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Footer" Src="Footer.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Login</title>
    <link rel='stylesheet' href='style.css' />
</head>
<body>
    <form id="Form1" method="post" runat="server">
        <uc1:TopBanner ID="Banner1" runat="server"></uc1:TopBanner>
        <div class="infobox" style="width: 600px; margin: 30px auto">
            <h2>
                Login
            </h2>
            <div class="padding10">
                <table cellspacing="0" border="0" cellpadding="12" style="width: 100%">
                    <tr>
                        <td align="right">
                            UserName:</td>
                        <td>
                            <asp:TextBox ID="UsernameBox" Width="200px" runat="server" TextMode="SingleLine" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Username is required"
                                ControlToValidate="UsernameBox" Display="Dynamic"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator runat="server" ControlToValidate="UsernameBox" ErrorMessage="Please keep Username one word with no spaces or special characters."
                                ValidationExpression="^[a-zA-Z0-9\._-]+$" ID="Regularexpressionvalidator1"></asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Password:</td>
                        <td>
                            <asp:TextBox ID="PasswordBox" Width="200px" TextMode="Password" runat="server" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Password is required."
                                ControlToValidate="PasswordBox" Display="Dynamic"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td>
                            <asp:CheckBox ID="LoginPersist" Text="Remember Login" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td>
                            <asp:Button ID="LoginButton" OnClick="LoginButton_Click" Text="Login" runat="server" /></td>
                    </tr>
                </table>
            </div>
        </div>
        <uc1:Footer ID="Footer1" runat="server"></uc1:Footer>
    </form>
</body>
</html>

<script runat="server">
void Page_Load(object sender, System.EventArgs e)
{
	if(Request.QueryString["Logout"]=="1")
	{
		System.Web.Security.FormsAuthentication.SignOut();
		Response.Redirect("Default.aspx");
		return;
	}

}
private void LoginButton_Click(object sender, System.EventArgs e)
{
	if(!Page.IsValid)return;

	string username=UsernameBox.Text.Trim();
	string password=PasswordBox.Text;
	
	string nickname=null;

	bool userexists=CuteChat.ChatProvider.Instance.GetUserInfo(username,ref nickname);
	
	if(!userexists)
	{
		MsgBox("Username does not exist!");
		return;
	}

	if(!CuteChat.ChatProvider.Instance.ValidateUser(username,password))
	{
		MsgBox("Wrong password!");
		return;
	}
			
	System.Web.Security.FormsAuthentication.SetAuthCookie(username,LoginPersist.Checked);
	Response.Redirect("~/Default.aspx");
	
}

public static void MsgBox(String msg)
{
	System.Web.HttpContext.Current.Response.Write("<script language='javascript'>alert('" + msg + "');</scr"+"ipt>");
}

</script> 