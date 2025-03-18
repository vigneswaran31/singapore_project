<%@ Import Namespace="System.Data"%>
<%@ Register TagPrefix="uc1" TagName="Footer" Src="Footer.ascx" %>
<%@ Register TagPrefix="uc1" TagName="TopBanner" Src="banner.ascx" %>
<%@ Page Language="c#" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head>
		<title>Manager</title>
		<link rel='stylesheet' href='style.css'>
		<style type="text/css">
		table.Grid
		{
			border-width: 5px;
			border-style: none;
			background-color: White;
			border-color: #cccccc;
			border-collapse: collapse;
		}

		table.Grid TD
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
                    Manage Administrators</h1>
                <div class="divx">
                    (Default administrator account is
                    <%=System.Configuration.ConfigurationSettings.AppSettings["DefaultAdminUsername"]%>.
                    )
                </div>
                <br />
                <asp:DataGrid runat="server" ID="DataGrid1" CssClass="Grid" BorderColor="#CCCCCC"
                    BorderStyle="None" BorderWidth="1px" BackColor="White" CellPadding="3" AutoGenerateColumns="False"
                    Width="600px" DataKeyField="LoginName">
                    <HeaderStyle Font-Bold="True" BackColor="#eeeeee"></HeaderStyle>
                    <Columns>
                        <asp:BoundColumn DataField="LoginName" HeaderText="LoginName"></asp:BoundColumn>
                        <asp:BoundColumn DataField="Password" HeaderText="Password"></asp:BoundColumn>
                        <asp:ButtonColumn Text="Remove" CommandName="Delete"></asp:ButtonColumn>
                    </Columns>
                </asp:DataGrid>
                <br />
                <div class="divx">
                    Username :
                    <asp:TextBox ID="AdminUsernameBox" runat="server" TextMode="SingleLine"></asp:TextBox>
                    <asp:Button ID="AddAdminButton" runat="server" Text="Set as administrator"></asp:Button>
                </div>
                <div style="height: 40px">
                    <div>
                        <asp:Label ID="MessageLabel" runat="server" ForeColor="Maroon" EnableViewState="False"
                            Visible="False">Label</asp:Label></div>
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
	this.AddAdminButton.DataBinding += new System.EventHandler(this.AddAdminButton_Click);
	this.AddAdminButton.Click += new System.EventHandler(this.AddAdminButton_Click);
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

	if(!IsPostBack)
	{
		DataGrid1.DataBind();
	}
}



private void DataGrid1_DataBinding(object sender, System.EventArgs e)
{
	DataTable table=new DataTable();

	using(IDbConnection conn= CuteChat.ChatProvider.Instance.CreateConnection())
	{
		conn.Open();

		IDbCommand cmd = conn.CreateCommand();
		cmd.CommandText="SELECT * FROM SampleUsers WHERE IsAdmin=1";
		using(IDataReader reader=cmd.ExecuteReader())
		{
			for(int i=0;i<reader.FieldCount;i++)
			{
				table.Columns.Add(reader.GetName(i),reader.GetFieldType(i));
			}
			object[] values=new object[reader.FieldCount];
			while(reader.Read())
			{
				reader.GetValues(values);
				table.Rows.Add(values);
			}
		}
	}

	table.AcceptChanges();

	DataGrid1.DataSource=table.DefaultView;
}

private void DataGrid1_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
{
	string loginName=(string)DataGrid1.DataKeys[e.Item.ItemIndex];

	GlobalAsax.ExampleProvider.UpdateIsAdmin(loginName,false);

	DataGrid1.DataBind();
}


private void AddAdminButton_Click(object sender, System.EventArgs e)
{
	string loginName=AdminUsernameBox.Text.Trim();
	if(!CuteChat.ChatProvider.Instance.UserExists(loginName))
	{
		MessageLabel.Text="User not exists!";
		MessageLabel.Visible=true;
		return;
	}

	GlobalAsax.ExampleProvider.UpdateIsAdmin(loginName,true);

	DataGrid1.DataBind();
}
		</script>
	</body>
</html>
