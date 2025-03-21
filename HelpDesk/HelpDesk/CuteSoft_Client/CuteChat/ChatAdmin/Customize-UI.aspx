<%@ Register TagPrefix="uc1" TagName="Menu" Src="Menu.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Banner" Src="Banner.ascx" %>
<%@ Page Language="C#" Inherits="CuteChat.ChatAdminPage" %>
<%@ Import Namespace="CuteChat" %>
<!DOCTYPE html>
<html lang="en">
	<head id="Head1" runat="server">
		<title>Customize User Interface - ASP.NET Chat Software</title>
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
					if(val=="")val=null;
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
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<link rel="stylesheet" href="style.css" />
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
						<table cellspacing="1" cellpadding="3" border="0" class="box">
							<tr>
								<td valign=top class="boxTitle" height="30">
									Customize User Interface <a href="#chat-layout" style="color:#666666">(UI 
														Screenshot)</a>
								</td>
							</tr>
							<tr>
								<td valign=top class="boxArea">	
									<table cellSpacing="0" cellPadding="5" width="750" border="0">
										<tr bgcolor="#ffffff">
											<td>Display Header Panel</td>
											<td>
												<asp:DropDownList ConfigName="DisplayHeaderPanel" Runat="server" Width="200" ID="Dropdownlist2">
													<asp:ListItem Value="">Default (True)</asp:ListItem>
													<asp:ListItem Value="True">True</asp:ListItem>
													<asp:ListItem Value="False">False</asp:ListItem>
												</asp:DropDownList>
											</td>
										</tr>
										<tr bgcolor="#f5f5f5">
											<td>Display Logo</td>
											<td><asp:DropDownList ConfigName="DisplayLogo" Runat="server" Width="200" ID="Dropdownlist3">
													<asp:ListItem Value="">Default (True)</asp:ListItem>
													<asp:ListItem Value="True">True</asp:ListItem>
													<asp:ListItem Value="False">False</asp:ListItem>
												</asp:DropDownList>
											</td>
										</tr>
										<tr bgcolor="#ffffff">
											<td>Display Top Banner</td>
											<td><asp:DropDownList ConfigName="DisplayTopBanner" Runat="server" Width="200" ID="Dropdownlist4">
													<asp:ListItem Value="">Default (True)</asp:ListItem>
													<asp:ListItem Value="True">True</asp:ListItem>
													<asp:ListItem Value="False">False</asp:ListItem>
												</asp:DropDownList>
											</td>
										</tr>
										<tr bgcolor="#f5f5f5">
											<td>Display Bottom Banner</td>
											<td><asp:DropDownList ConfigName="DisplayBottomBanner" Runat="server" Width="200" ID="Dropdownlist5">
													<asp:ListItem Value="">Default (False)</asp:ListItem>
													<asp:ListItem Value="True">True</asp:ListItem>
													<asp:ListItem Value="False">False</asp:ListItem>
												</asp:DropDownList>
											</td>
										</tr>
										<tr bgcolor="#ffffff">
											<td>Display Room List</td>
											<td><asp:DropDownList ConfigName="DisplayLobbyList" Runat="server" Width="200" ID="Dropdownlist22">
													<asp:ListItem Value="">Default (True)</asp:ListItem>
													<asp:ListItem Value="True">True</asp:ListItem>
													<asp:ListItem Value="False">False</asp:ListItem>
												</asp:DropDownList>
											</td>
										</tr>
										</tr><tr bgcolor="#f5f5f5">
											<td>Display Room List (Embed Mode)</td>
											<td><asp:DropDownList ConfigName="DisplayLobbyListEmbed" Runat="server" Width="200" ID="Dropdownlist6">
													<asp:ListItem Value="">Default (True)</asp:ListItem>
													<asp:ListItem Value="True">True</asp:ListItem>
													<asp:ListItem Value="False">False</asp:ListItem>
												</asp:DropDownList>
											</td>
										</tr>
										<!--
										<tr bgcolor="#ffffff">
											<td>Customize Skins</td>
											<td>											
												<asp:TextBox ConfigName="ChannelSkins" Runat="server" Width="200" ID="Textbox21"></asp:TextBox>
												<br>
												Defalult: "<%=ChatConsts.DefaultChannelSkins%>"
											</td>
										</tr>
										-->
										<tr bgcolor="#f5f5f5">
											<td nowrap>Show avatar before message</td>
											<td><asp:DropDownList ConfigName="GlobalShowAvatarBeforeMessage" Runat="server" Width="200" ID="Dropdownlist7"
													NAME="Dropdownlist1">
													<asp:ListItem Value="">Default (True)</asp:ListItem>
													<asp:ListItem Value="True">True</asp:ListItem>
													<asp:ListItem Value="False">False</asp:ListItem>
												</asp:DropDownList>
											</td>
										</tr>
										<tr bgcolor="#ffffff">
											<td nowrap>Show timestamps before each message</td>
											<td><asp:DropDownList ConfigName="GlobalShowTimeStampBeforeMessage" Runat="server" Width="200" ID="Dropdownlist8"
													NAME="Dropdownlist1">
													<asp:ListItem Value="">Default (False)</asp:ListItem>
													<asp:ListItem Value="True">True</asp:ListItem>
													<asp:ListItem Value="False">False</asp:ListItem>
												</asp:DropDownList>
											</td>
										</tr>
										<tr bgcolor="#ffffff">
											<td nowrap>Show timestamps in Web Messenger</td>
											<td><asp:DropDownList ConfigName="GlobalShowTimeStampWebMessenger" Runat="server" Width="200" ID="Dropdownlist21"
													NAME="Dropdownlist1">
													<asp:ListItem Value="">Default (True)</asp:ListItem>
													<asp:ListItem Value="True">True</asp:ListItem>
													<asp:ListItem Value="False">False</asp:ListItem>
												</asp:DropDownList>
											</td>
										</tr>
										<tr bgcolor="#f5f5f5">
											<td nowrap>Show bold button</td>
											<td><asp:DropDownList ConfigName="GlobalShowBoldButton" Runat="server" Width="200" ID="Dropdownlist9"
													NAME="Dropdownlist1">
													<asp:ListItem Value="">Default (True)</asp:ListItem>
													<asp:ListItem Value="True">True</asp:ListItem>
													<asp:ListItem Value="False">False</asp:ListItem>
												</asp:DropDownList>
											</td>
										</tr>
										<tr bgcolor="#ffffff">
											<td nowrap>Show italic button</td>
											<td><asp:DropDownList ConfigName="GlobalShowItalicButton" Runat="server" Width="200" ID="Dropdownlist10"
													NAME="Dropdownlist1">
													<asp:ListItem Value="">Default (True)</asp:ListItem>
													<asp:ListItem Value="True">True</asp:ListItem>
													<asp:ListItem Value="False">False</asp:ListItem>
												</asp:DropDownList>
											</td>
										</tr>
										<tr bgcolor="#f5f5f5">
											<td nowrap>Show underline button</td>
											<td><asp:DropDownList ConfigName="GlobalShowUnderlineButton" Runat="server" Width="200" ID="Dropdownlist11"
													NAME="Dropdownlist1">
													<asp:ListItem Value="">Default (True)</asp:ListItem>
													<asp:ListItem Value="True">True</asp:ListItem>
													<asp:ListItem Value="False">False</asp:ListItem>
												</asp:DropDownList>
											</td>
										</tr>
										<tr bgcolor="#ffffff">
											<td nowrap>Show color button</td>
											<td><asp:DropDownList ConfigName="GlobalShowColorButton" Runat="server" Width="200" ID="Dropdownlist12"
													NAME="Dropdownlist1">
													<asp:ListItem Value="">Default (True)</asp:ListItem>
													<asp:ListItem Value="True">True</asp:ListItem>
													<asp:ListItem Value="False">False</asp:ListItem>
												</asp:DropDownList>
											</td>
										</tr>
										<tr bgcolor="#f5f5f5">
											<td nowrap>Show font name dropdown</td>
											<td><asp:DropDownList ConfigName="GlobalShowFontName" Runat="server" Width="200" ID="Dropdownlist13" NAME="Dropdownlist1">
													<asp:ListItem Value="">Default (True)</asp:ListItem>
													<asp:ListItem Value="True">True</asp:ListItem>
													<asp:ListItem Value="False">False</asp:ListItem>
												</asp:DropDownList>
											</td>
										</tr>
										<tr bgcolor="#ffffff">
											<td nowrap>Show font size dropdown</td>
											<td><asp:DropDownList ConfigName="GlobalShowFontSize" Runat="server" Width="200" ID="Dropdownlist14" NAME="Dropdownlist1">
													<asp:ListItem Value="">Default (True)</asp:ListItem>
													<asp:ListItem Value="True">True</asp:ListItem>
													<asp:ListItem Value="False">False</asp:ListItem>
												</asp:DropDownList>
											</td>
										</tr>
										<tr bgcolor="#f5f5f5">
											<td nowrap>Show emotion button</td>
											<td><asp:DropDownList ConfigName="GlobalShowEmotion" Runat="server" Width="200" ID="Dropdownlist15" NAME="Dropdownlist1">
													<asp:ListItem Value="">Default (True)</asp:ListItem>
													<asp:ListItem Value="True">True</asp:ListItem>
													<asp:ListItem Value="False">False</asp:ListItem>
												</asp:DropDownList>
											</td>
										</tr>
										<tr bgcolor="#f5f5f5">
											<td nowrap>Show control panel button</td>
											<td><asp:DropDownList ConfigName="GlobalShowControlPanel" Runat="server" Width="200" ID="Dropdownlist16"
													NAME="Dropdownlist1">
													<asp:ListItem Value="">Default (True)</asp:ListItem>
													<asp:ListItem Value="True">True</asp:ListItem>
													<asp:ListItem Value="False">False</asp:ListItem>
												</asp:DropDownList></td>
										</tr>
										<tr bgcolor="#ffffff">
											<td nowrap>Show skin button</td>
											<td><asp:DropDownList ConfigName="GlobalShowSkinButton" Runat="server" Width="200" ID="Dropdownlist17"
													NAME="Dropdownlist1">
													<asp:ListItem Value="">Default (True)</asp:ListItem>
													<asp:ListItem Value="True">True</asp:ListItem>
													<asp:ListItem Value="False">False</asp:ListItem>
												</asp:DropDownList></td>
										</tr>
										<tr bgcolor="#ffffff">
											<td nowrap>Show sign out button</td>
											<td><asp:DropDownList ConfigName="GlobalShowSignoutButton" Runat="server" Width="200" ID="Dropdownlist1"
													NAME="Dropdownlist1">
													<asp:ListItem Value="">Default (True)</asp:ListItem>
													<asp:ListItem Value="True">True</asp:ListItem>
													<asp:ListItem Value="False">False</asp:ListItem>
												</asp:DropDownList></td>
										</tr>
										<tr bgcolor="#f5f5f5">
											<td nowrap>Show system messages</td>
											<td><asp:DropDownList ConfigName="GlobalShowSystemMessages" Runat="server" Width="200" ID="Dropdownlist18"
													NAME="Dropdownlist1">
													<asp:ListItem Value="">Default (True)</asp:ListItem>
													<asp:ListItem Value="True">True</asp:ListItem>
													<asp:ListItem Value="False">False</asp:ListItem>
												</asp:DropDownList></td>
										</tr>
										<tr bgcolor="#ffffff">
											<td nowrap>Show system errors</td>
											<td><asp:DropDownList ConfigName="GlobalShowSystemErrors" Runat="server" Width="200" ID="Dropdownlist19"
													NAME="Dropdownlist1">
													<asp:ListItem Value="">Default (False)</asp:ListItem>
													<asp:ListItem Value="True">True</asp:ListItem>
													<asp:ListItem Value="False">False</asp:ListItem>
												</asp:DropDownList></td>
										</tr>
										<tr bgcolor="#f5f5f5">
											<td nowrap>Show typing indicator
											</td>
											<td><asp:DropDownList ConfigName="GlobalShowTypingIndicator" Runat="server" Width="200" ID="Dropdownlist20"
													NAME="Dropdownlist1">
													<asp:ListItem Value="">Default (True)</asp:ListItem>
													<asp:ListItem Value="True">True</asp:ListItem>
													<asp:ListItem Value="False">False</asp:ListItem>
												</asp:DropDownList></td>
										</tr>
									</table>
								</td>
							</tr>
						</table>						
						<br>
						<asp:Button ID="ButtonUpdate" OnClick="ButtonUpdate_Click" Runat="server" Text="Update"></asp:Button>
						<br>
						<br>
						<a id="chat-layout" name="chat-layout"></a>
						<p>
							<a href="../images/chat-layout-full.gif" title="click to view a bigger image"><img src="../images/chat-layout-760.gif" border="0"></a>
						</p>
					</td> 
				</tr> 
			</table>
			<div id="footer">
				<p><a href="http://cutesoft.net">Copyright 2002-2015 CuteSoft.Net. All rights reserved.</a></p>
			</div>
		</form>
	</BODY>
</html>
