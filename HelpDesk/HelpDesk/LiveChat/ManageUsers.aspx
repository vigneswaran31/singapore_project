<%@ Page Language="c#" %>

<%@ Register TagPrefix="uc1" TagName="Footer" Src="Footer.ascx" %>
<%@ Register TagPrefix="uc1" TagName="TopBanner" Src="banner.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Add new operators, delete existing operators</title>
    <link rel='stylesheet' href='style.css' />
    <style type="text/css">
        table.Grid {
            border-width: 5px;
            border-style: none;
            background-color: White;
            border-color: #cccccc;
            border-collapse: collapse;
        }

            table.Grid TD {
                padding: 4px 6px 4px 6px;
                border: solid 1px #cccccc;
                vertical-align: top;
            }
    </style>
</head>
<body>
  
    <form id="Form1" method="post" runat="server">
        <uc1:TopBanner ID="Banner1" runat="server"></uc1:TopBanner>
        <div style="width: 700px; margin: 30px auto">
            <h1>Manage Operators</h1>
            <asp:DataGrid runat="server" ID="DataGrid1" CssClass="Grid" BorderColor="#CCCCCC"
                BorderStyle="None" BorderWidth="1px" BackColor="White" CellPadding="3" AutoGenerateColumns="False"
                Width="600px" AllowPaging="True" DataKeyField="Name">
                <FooterStyle ForeColor="#000066" BackColor="White"></FooterStyle>
                <SelectedItemStyle Font-Bold="True" BackColor="#999999"></SelectedItemStyle>
                <HeaderStyle Font-Bold="True" BackColor="#eeeeee"></HeaderStyle>
                <Columns>
                    <asp:BoundColumn DataField="Name" HeaderText="Name"></asp:BoundColumn>
                    <asp:BoundColumn DataField="Email" HeaderText="Email"></asp:BoundColumn>

                    <asp:BoundColumn DataField="Password" HeaderText="Password"></asp:BoundColumn>
                    <asp:ButtonColumn Text="Delete" CommandName="Delete"></asp:ButtonColumn>
                </Columns>
                <PagerStyle ForeColor="#000066" BackColor="White" HorizontalAlign="Right" Mode="NumericPages"></PagerStyle>
            </asp:DataGrid>
            <div class="infobox" style="width: 600px; margin: 30px 0">
                <h2>Create Operators</h2>
                <div class="padding10">
                    <table width="100%" cellspacing="1" cellpadding="3" border="0" align="center">
                        <tr>
                            <td align="right">UserName:</td>
                            <td>
                                <asp:TextBox ID="UsernameBox" Width="200px" runat="server" TextMode="SingleLine" />
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Username is required"
                                    ControlToValidate="UsernameBox" Display="Dynamic"></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator runat="server" ControlToValidate="UsernameBox" ErrorMessage="Please keep Username one word with no spaces or special characters."
                                    ValidationExpression="^[a-zA-Z0-9\._-]+$" ID="Regularexpressionvalidator1"></asp:RegularExpressionValidator>
                            </td>
                        </tr>
                        <tr>
                            <td align="right">Password:</td>
                            <td>
                                <asp:TextBox ID="PasswordBox" Width="200px" TextMode="Password" runat="server" />
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Password is required."
                                    ControlToValidate="PasswordBox" Display="Dynamic"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td align="right">Department:</td>
                            <td>
                                <asp:DropDownList Width="200px" runat="server" ID="DepartmentBox">
                                </asp:DropDownList></td>
                        </tr>
                        <tr>
                            <td>&nbsp;</td>
                            <td>
                                <asp:LinkButton runat="server" ID="RegisterButton" Text="Create" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="text-align: center;">
                                <br />
                                <br />
                                <asp:Label ID="MessageLabel" runat="server" ForeColor="red" CssClass="MessageContainer"
                                    Visible="False" EnableViewState="False">Label</asp:Label>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
        <uc1:Footer ID="Footer1" runat="server"></uc1:Footer>
    </form>

    <script runat="server">

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
            this.DataGrid1.DataBinding += new System.EventHandler(this.DataGrid1_DataBinding);
            this.DataGrid1.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGrid1_DeleteCommand);
            this.DataGrid1.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.DataGrid1_PageIndexChanged);
            this.RegisterButton.Click += new System.EventHandler(this.RegisterButton_Click);
        }

        protected override void OnLoad(EventArgs e)
        {
            CuteChat.AppChatIdentity identity = CuteChat.ChatProvider.Instance.GetLogonIdentity();
            if (identity == null)
            {
                Response.Redirect("Login.aspx", true);
                return;
            }
            if (!CuteChat.ChatProvider.Instance.IsAdministrator(identity.UniqueId))
            {
                Response.Redirect("Login.aspx", true);
                return;
            }

            base.OnLoad(e);

            if (!IsPostBack)
            {

                CuteChat.ChatWebUtility.SetupAdministratorAgent(identity.UniqueId);

                DataGrid1.DataBind();

                DepartmentBox.DataSource = CuteChat.ChatWebUtility.LoadDepartments();
                DepartmentBox.DataTextField = "Name";
                DepartmentBox.DataValueField = "Name";
                DepartmentBox.DataBind();
            }
        }


        private void DataGrid1_DataBinding(object sender, System.EventArgs e)
        {
            DataGrid1.DataSource = global_asax.ExampleProvider.LoadUsers();
        }

        private void DataGrid1_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            string loginName = (string)DataGrid1.DataKeys[e.Item.ItemIndex];
            global_asax.ExampleProvider.DeleteUser(loginName);

            DataGrid1.DataBind();
        }

        private void DataGrid1_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            DataGrid1.CurrentPageIndex = e.NewPageIndex;
            DataGrid1.DataBind();
        }


        private void RegisterButton_Click(object sender, System.EventArgs e)
        {
            if (!Page.IsValid) return;

            string username = UsernameBox.Text.Trim();
            string password = PasswordBox.Text;

            string nickname = null;
            string dbpassword = null;
            bool isAdmin = false;
            bool userexists = global_asax.ExampleProvider.GetUserInfo(username, ref nickname, ref dbpassword, ref isAdmin);

            if (userexists)
            {
                MessageLabel.Text = "The username you entered is already being used.";
                MessageLabel.Visible = true;
                return;
            }
            if (DepartmentBox.SelectedIndex == -1)
            {
                MessageLabel.Text = "No department selected! Please manage the departments.";
                MessageLabel.Visible = true;
                return;
            }
            string deptname = DepartmentBox.SelectedValue;

            global_asax.ExampleProvider.CreateUser(username, username, password);

            CuteChat.ChatWebUtility.AddAgent(deptname, CuteChat.ChatProvider.Instance.ToUserId(username));

            //Response.Redirect("~/");
            MsgBox("User Created!");
            UsernameBox.Text = "";
            PasswordBox.Text = "";
            DataGrid1.DataBind();

        }

        public static void MsgBox(String msg)
        {
            System.Web.HttpContext.Current.Response.Write("<script language='javascript'>alert('" + msg + "');</scr" + "ipt>");
        }


    </script>

</body>
</html>
