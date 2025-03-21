<%@ Register TagPrefix="uc1" TagName="Menu" Src="Menu.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Banner" Src="Banner.ascx" %>
<%@ Page Language="C#" Inherits="CuteChat.ChatAdminPage" %>
<%@ Import Namespace="CuteChat" %>
<!DOCTYPE html>
<html lang="en">
	 <head id="Head1" runat="server">
		<title>Customize Color Picker - ASP.NET Chat Software</title>   
		<script runat="server">
override protected void OnLoad(EventArgs args)
{
	base.OnLoad(args);
	
	if(!IsPostBack)
	{
		Control[] ctrls=GetControls();
		foreach(Control ctrl in ctrls)
		{
			if(ctrl is TextBox)
			{
				TextBox tb=(TextBox)ctrl;
				string configname=tb.Attributes["ConfigName"];
				tb.Text=ChatApi.GetConfig(configname);
			}
			if(ctrl is DropDownList)
			{
				DropDownList ddl=(DropDownList)ctrl;
				string configname=ddl.Attributes["ConfigName"];
				ddl.SelectedValue=ChatApi.GetConfig(configname);
			}
		}
	}
}
Control[] GetControls()
{
	ArrayList al=new ArrayList();
	foreach(Control ctrl in Form1.Controls)
	{
		if(ctrl is WebControl)
		{
			if(((WebControl)ctrl).Attributes["ConfigName"]!=null)
				al.Add(ctrl);
		}
	}
	return (Control[])al.ToArray(typeof(Control));
}

private void ButtonUpdate_Click(object sender,EventArgs args)
{
	Control[] ctrls=GetControls();
	foreach(Control ctrl in ctrls)
	{
		if(ctrl is TextBox)
		{
			TextBox tb=(TextBox)ctrl;
			string configname=tb.Attributes["ConfigName"];
			string val=tb.Text.Trim();
			if(val=="")
				val=null;
			else
				val = val.Replace(System.Environment.NewLine, "");
			ChatApi.SetConfig(configname,val);		
		}
		if(ctrl is DropDownList)
		{
			DropDownList ddl=(DropDownList)ctrl;
			string configname=ddl.Attributes["ConfigName"];
			string val=ddl.SelectedValue;
			if(val=="")val=null;
			ChatApi.SetConfig(configname,val);
		}
	}
}
		</script>
		<link rel="stylesheet" href="style.css">
	</head>
	<body>
		<form runat="server" ID="Form1">
			<uc1:Banner id="banner1" runat="server" />
			<table width="100%" border="0" cellpadding="3" cellspacing="0">
				<tr>
					<td id="leftcolumn" valign="top">
						<uc1:Menu id="Menu1" runat="server"></uc1:Menu>
					</td>
					<td width="10">&nbsp;</td>
					<td align="left" id="content">
						<h1><img src="../images/setting.gif" border="0" alt="Configuration" align="absMiddle">Configurationn</h1>
						<table cellspacing="1" cellpadding="3" border="0" class="box" style="width:750px">
							<tr>
								<td valign=top class="boxTitle" height="30">
									Customize Color Picker
								</td>
							</tr>
							<tr>
								<td valign=top class="boxArea">									
									The Color Picker of Cute Chat by default displays a predefined set of colors. You can easily modify this default set.<br><br>
									
									<img alt="Customize Color Picker" src="../images/Chat_Customize_Color.gif" border=0>
									<br><br>
									<b>By default all colors in the following color string will be used in Cute Chat color panel. </b> <br>
									<br>#000000,#993300,#333300,#003300,#003366,#000080,#333399,#333333,<br>#800000,#FF6600,#808000,#008000,#008080,#0000FF,#666699,#808080,<br>
									#FF0000,#FF9900,#99CC00,#339966,#33CCCC,#3366FF,#800080,#999999,<br>
									#FF00FF,#FFCC00,#FFFF00,#00FF00,#00FFFF,#00CCFF,#993366,#C0C0C0,<br>
									#FF99CC,#FFCC99,#FFFF99,#CCFFCC,#CCFFFF,#99CCFF,#CC99FF,#FFFFFF<br>
									<br>
									<p><b>You can easily modify this default set by creating your own color array.</b><p>
									<br>#FF0000,#FF9900,#99CC00,#339966,#33CCCC,#3366FF,#800080,#999999,<br>
									#FF00FF,#FFCC00,#FFFF00,#00FF00,#00FFFF,#00CCFF,#993366,#C0C0C0<br><br>
									<asp:TextBox ConfigName="ColorsArray" Runat="server" Width="650" ID="ColorsArray" TextMode="MultiLine" Rows="4"></asp:TextBox>
									<br><br>
									<asp:Button ID="ButtonUpdate" OnClick="ButtonUpdate_Click" Runat="server" Text="Update"></asp:Button>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
			<div id="footer">
				<p><a href="http://cutesoft.net">Copyright 2002-2015 CuteSoft.Net. All rights reserved.</a></p>
			</div>
		</form>
	</BODY>
</html>
