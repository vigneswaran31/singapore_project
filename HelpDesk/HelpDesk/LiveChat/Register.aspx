<%@ Register TagPrefix="uc1" TagName="TopBanner" Src="banner.ascx" %>
<%@ Page language="c#" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head>
		<title>Register</title>
		<link rel='stylesheet' href='style.css' />
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<uc1:TopBanner id="Banner1" runat="server"></uc1:TopBanner>
			<div id='main'>
			<table border="0" cellspacing="0" cellpadding="4">
				<tr>
					<td>&nbsp;</td>
					<td>Register</td>
					<td>&nbsp;</td>
				</tr>
				<tr>
					<td>Username:</td>
					<td>
						<asp:TextBox Runat="server" ID="UsernameBox" TextMode="SingleLine" />
					</td>
					<td>
						<asp:RequiredFieldValidator Runat="server" ID="RequireUsername" ControlToValidate="UsernameBox" EnableClientScript="True"
							Display="Dynamic" ErrorMessage="*" />
					</td>
				</tr>
				<tr>
					<td>Password:</td>
					<td>
						<asp:TextBox Runat="server" ID="PasswordBox" TextMode="Password" />
					</td>
					<td>
						<asp:RequiredFieldValidator Runat="server" ID="RequirePasswordBox" ControlToValidate="PasswordBox" EnableClientScript="True"
							Display="Dynamic" ErrorMessage="*" />
					</td>
				</tr>
				<tr>
					<td>&nbsp;</td>
					<td>
						<asp:LinkButton Runat="server" ID="RegisterButton" Text="Create User" />
					</td>
					<td>&nbsp;</td>
				</tr>
			</table>
			<asp:Label id="MessageLabel" runat="server" ForeColor="Maroon" Visible="False" EnableViewState="False">Label</asp:Label>
			</div>
		</form>
	</body>
</html>
<script runat=server>

override protected void OnInit(EventArgs e)
{
	//
	// CODEGEN: This call is required by the ASP.NET Web Form Designer.
	//
	InitializeComponent();
	base.OnInit(e);
}

/// <summary>
/// Required method for Designer support - do not modify
/// the contents of this method with the code editor.
/// </summary>
private void InitializeComponent()
{    
	this.RegisterButton.Click += new System.EventHandler(this.RegisterButton_Click);
}

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



private void RegisterButton_Click(object sender, System.EventArgs e)
{
	if(!Page.IsValid)return;

	string username=UsernameBox.Text.Trim();
	string password=PasswordBox.Text;

	string nickname=null;
	string dbpassword=null;
	bool isAdmin=false;
	bool userexists=GlobalAsax.ExampleProvider.GetUserInfo(username,ref nickname,ref dbpassword,ref isAdmin);

	if(userexists)
	{
		MessageLabel.Text="The username already be used";
		MessageLabel.Visible=true;
		return;
	}

	GlobalAsax.ExampleProvider.CreateUser(username,username,password);

	if(Context.User.Identity.IsAuthenticated)
	{
		//Response.Redirect("~/");
		MsgBox("User Created!");
		UsernameBox.Text="";
		PasswordBox.Text="";
	}
	else
	{
		//SetAuthCookie and redirect.
		System.Web.Security.FormsAuthentication.RedirectFromLoginPage(username,false,"/");
	}

}

public static void MsgBox(String msg)
{
	System.Web.HttpContext.Current.Response.Write("<script language='javascript'>alert('" + msg + "');</scr"+"ipt>");
}

</script>
