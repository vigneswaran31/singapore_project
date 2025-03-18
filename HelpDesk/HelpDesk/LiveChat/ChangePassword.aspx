<%@ Page Language="c#" %>

<%@ Register TagPrefix="uc1" TagName="TopBanner" Src="banner.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Footer" Src="Footer.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Change Password</title>
    <link rel='stylesheet' href='style.css' />
</head>
<body>
    <form id="form1" method="post" runat="server">
        <uc1:TopBanner ID="Banner1" runat="server"></uc1:TopBanner>
        <div class="infobox" style="width: 600px; margin: 30px auto">
            <h2>
                Change Password
            </h2>
            <div class="padding10">
                <table cellspacing="0" border="0" cellpadding="8" style="width: 100%">
                    <tr>
                        <td align="right" style="width: 150px">
                            UserName:</td>
                        <td>
                            <asp:TextBox ID="ChangeUserName" Width="300px" runat="server" Enabled="False" />
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Old Password:</td>
                        <td>
                            <asp:TextBox ID="ChangeOldPassword" Width="300px" TextMode="Password" runat="server" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Old Password is required."
                                ControlToValidate="ChangeOldPassword" Display="Dynamic"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            New Password:</td>
                        <td>
                            <asp:TextBox ID="ChangePassword" Width="300px" TextMode="Password" runat="server" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="Password is required."
                                ControlToValidate="ChangePassword" Display="Dynamic"></asp:RequiredFieldValidator>
                            <asp:CompareValidator ID="CompareValidator1" runat="server" ErrorMessage="You must re-enter the new password."
                                ControlToValidate="ChangePassword2" Display="Dynamic" ControlToCompare="ChangePassword"></asp:CompareValidator>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Confirm Password:</td>
                        <td>
                            <asp:TextBox ID="ChangePassword2" Width="300px" TextMode="Password" runat="server" />
                            <asp:RequiredFieldValidator ID="Requiredfieldvalidator4" runat="server" ErrorMessage="The password and confirmation do not match."
                                ControlToValidate="ChangePassword2" Display="Dynamic"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td>
                            <asp:Button ID="ChangeButton" OnClick="ChangeButton_Click" Text="Change" runat="server" /></td>
                    </tr>
                </table>
                <br />
                <br />
                <asp:Label ID="Label1" runat="server" EnableViewState="false" Visible="false" ForeColor="maroon">label</asp:Label>
            </div>
        </div>
        <uc1:Footer ID="Footer1" runat="server"></uc1:Footer>
    </form>
</body>
</html>

<script runat="server">
    void Page_Load(object sender, System.EventArgs e)
    {
        if (!IsPostBack)
        {
            string loginname = Context.User.Identity.Name;
            string nickname = null;
            string password = null;
            bool isAdmin = false;
            GlobalAsax.ExampleProvider.GetUserInfo(loginname, ref nickname, ref password, ref isAdmin);
            ChangeUserName.Text = nickname;
        }

    }
    private void ChangeButton_Click(object sender, System.EventArgs e)
    {
        if (!this.IsValid)
        {
            return;
        }

        string loginname = Context.User.Identity.Name;
        string oldnickname = null;
        string oldpassword = null;
        bool isAdmin = false;

        GlobalAsax.ExampleProvider.GetUserInfo(loginname, ref oldnickname, ref oldpassword, ref isAdmin);

        string nickname = ChangeUserName.Text.Trim();
        string password = ChangeOldPassword.Text;
        string newpassword = ChangePassword.Text;
        string newpassword2 = ChangePassword2.Text;

        if (!loginname.Equals(nickname))
            GlobalAsax.ExampleProvider.UpdateUserName(loginname, nickname);

        if (!oldpassword.Equals(password))
        {
            LiveSupportWeb.JSHelper.MsgBox("Wrong old password!");
            return;
        }

        GlobalAsax.ExampleProvider.UpdatePassword(loginname, newpassword);
        Response.Redirect("~/default.aspx");
    }

</script>

