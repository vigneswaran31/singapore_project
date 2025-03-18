<%@ Import Namespace="System.Data"%>
<%@ Register TagPrefix="uc1" TagName="Footer" Src="Footer.ascx" %>
<%@ Register TagPrefix="uc1" TagName="TopBanner" Src="banner.ascx" %>
<%@ Page Language="c#" ClassName="ManageLiveSupport" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Manager</title>
    <link rel='stylesheet' href='style.css' />
    <style type="text/css">
		#DataGrid2 table.Grid, #DataGrid31 table.Grid
		{
			border-width: 5px;
			border-style: none;
			background-color: White;
			border-color: #cccccc;
			border-collapse: collapse;
		}

		#DataGrid2 table.Grid TD, #DataGrid31 table.Grid TD
		{
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
            <h1>
                Manage Departments</h1>
            <div class="divx">
                Department :
                <asp:TextBox ID="DepartmentBox" runat="server" TextMode="SingleLine"></asp:TextBox>
                <asp:Button ID="AddDepartmentButton" runat="server" Text="Add new department"></asp:Button>
            </div>
            <br />
            <asp:DataGrid runat="server" ID="DataGrid2" CellPadding="5" BorderWidth="0" CssClass="Grid" AutoGenerateColumns="False"
                Width="700px" DataKeyField="Name">
                <HeaderStyle Font-Bold="True" BackColor="#eeeeee"></HeaderStyle>
                <Columns>
                    <asp:TemplateColumn HeaderText="Department">
                        <ItemTemplate>
                            <table width="100%" cellpadding="5" cellspacing="0" border="0">
                                <tr>
                                    <td style="width:180px">
                                        <asp:Literal runat="server" ID="DepartmentLabel" Text='<%# DataBinder.Eval(Container.DataItem,"Name") %>' />
                                    </td>
                                    <td>
                                        <asp:DataGrid ID="DataGrid3" runat="server" CssClass="Grid" BorderColor="#CCCCCC"
                                            BorderStyle="None" BorderWidth="1px" BackColor="White" CellPadding="3" AutoGenerateColumns="False"
                                            DataKeyField="LoginName" Width="400px">
                                            <HeaderStyle BackColor="#eeeeee"></HeaderStyle>
                                            <Columns>
                                                <asp:BoundColumn HeaderText="Agent" DataField="DisplayName" />
                                                <asp:ButtonColumn Text="Delete" CommandName="Delete">
                                                    <ItemStyle Width="60px" />
                                                </asp:ButtonColumn>
                                            </Columns>
                                        </asp:DataGrid>
                                        <br />
                                        <asp:TextBox ID="AgentBox" runat="server" />
                                        <asp:Button ID="AddAgentButton" runat="server" Text="Add new agent" />
                                    </td>
                                </tr>
                            </table>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:ButtonColumn Text="Delete" CommandName="Delete">
                        <ItemStyle VerticalAlign="Top"></ItemStyle>
                    </asp:ButtonColumn>
                </Columns>
            </asp:DataGrid>
            <asp:Label ID="MessageLabel" runat="server" ForeColor="Maroon" EnableViewState="False"
                Visible="False">Label</asp:Label>
            <br />
            <br />
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
            this.DataGrid2.ItemCreated += new System.Web.UI.WebControls.DataGridItemEventHandler(this.DataGrid2_ItemCreated);
            this.DataGrid2.DataBinding += new System.EventHandler(this.DataGrid2_DataBinding);
            this.DataGrid2.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGrid2_DeleteCommand);
            this.AddDepartmentButton.DataBinding += new System.EventHandler(this.AddDepartmentButton_Click);
            this.AddDepartmentButton.Click += new System.EventHandler(this.AddDepartmentButton_Click);
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
                DataGrid2.DataBind();
            }
        }



        private void DataGrid2_DataBinding(object sender, System.EventArgs e)
        {
            DataGrid2.DataSource = CuteChat.ChatWebUtility.LoadDepartments();
        }

        private void DataGrid2_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            string name = (string)DataGrid2.DataKeys[e.Item.ItemIndex];

            CuteChat.ChatWebUtility.RemoveDepartment(name);

            DataGrid2.DataBind();
        }

        private void AddDepartmentButton_Click(object sender, System.EventArgs e)
        {
            string name = DepartmentBox.Text.Trim();
            if (name.Length == 0) return;


            CuteChat.ChatWebUtility.AddDepartment(name);

            DataGrid2.DataBind();
        }

        private void DataGrid2_ItemCreated(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
        {
            if (e.Item.ItemIndex == -1) return;

            DataGrid2ItemScope scope = new DataGrid2ItemScope();
            scope.ManageLiveSupport = this;
            scope.Item = e.Item;
            scope.Init();
        }

        class DataGrid2ItemScope
        {
            public ManageLiveSupport ManageLiveSupport;
            public DataGridItem Item;

            public DataGrid DataGrid3;
            public TextBox AgentBox;
            public Button AddAgentButton;

            public void Init()
            {
                Control template = Item.Cells[0];
                DataGrid3 = (DataGrid)template.FindControl("DataGrid3");
                AgentBox = (TextBox)template.FindControl("AgentBox");
                AddAgentButton = (Button)template.FindControl("AddAgentButton");

                DataGrid3.DataBinding += new EventHandler(DataGrid3_DataBinding);
                DataGrid3.DeleteCommand += new DataGridCommandEventHandler(DataGrid3_DeleteCommand);
                AddAgentButton.Click += new EventHandler(AddAgentButton_Click);
            }

            private void DataGrid3_DataBinding(object sender, EventArgs e)
            {
                string deptname = (string)ManageLiveSupport.DataGrid2.DataKeys[Item.ItemIndex];

                CuteChat.SupportDepartment[] departments = CuteChat.ChatWebUtility.LoadDepartments();

                foreach (CuteChat.SupportDepartment department in departments)
                {
                    if (department.Name == deptname)
                    {
                        DataTable table = new DataTable();

                        table.Columns.Add("LoginName", typeof(string));
                        table.Columns.Add("DisplayName", typeof(string));

                        foreach (CuteChat.SupportAgent agent in department.Agents)
                        {
                            string loginName = CuteChat.ChatProvider.Instance.FromUserId(agent.UserId);
                            string dispName = null;
                            CuteChat.ChatProvider.Instance.GetUserInfo(loginName, ref dispName);
                            table.Rows.Add(new object[] { loginName, dispName });
                        }

                        table.AcceptChanges();

                        DataGrid3.DataSource = table.DefaultView;
                    }
                }
            }

            private void DataGrid3_DeleteCommand(object source, DataGridCommandEventArgs e)
            {
                string deptname = (string)ManageLiveSupport.DataGrid2.DataKeys[Item.ItemIndex];
                string loginName = (string)DataGrid3.DataKeys[e.Item.ItemIndex];
                CuteChat.ChatWebUtility.RemoveAgent(deptname, CuteChat.ChatProvider.Instance.ToUserId(loginName));

                DataGrid3.DataBind();
            }

            private void AddAgentButton_Click(object sender, EventArgs e)
            {
                string name = AgentBox.Text.Trim();
                if (name.Length == 0) return;

                if (!CuteChat.ChatProvider.Instance.UserExists(name))
                {
                    ManageLiveSupport.MessageLabel.Text = "User not exists!";
                    ManageLiveSupport.MessageLabel.Visible = true;
                    return;
                }

                string deptname = (string)ManageLiveSupport.DataGrid2.DataKeys[Item.ItemIndex];
                CuteChat.ChatWebUtility.AddAgent(deptname, CuteChat.ChatProvider.Instance.ToUserId(name));

                DataGrid3.DataBind();
            }

        }
    </script>

</body>
</html>
