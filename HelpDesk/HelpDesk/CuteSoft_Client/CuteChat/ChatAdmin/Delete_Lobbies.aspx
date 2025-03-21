<%@ Import Namespace="CuteChat" %>
<%@ Page Language="C#" Inherits="CuteChat.ChatAdminPage" %>
<%@ Register TagPrefix="uc1" TagName="Lobbies" Src="Lobbies.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Banner" Src="Banner.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Menu" Src="Menu.ascx" %>
<!DOCTYPE html>
<html lang="en">
	<head id="Head1" runat="server">
		<title>CuteChat Lobbies</title>
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<link rel="stylesheet" href="style.css">
	</head>
	<body>
		<form runat="server" ID="Form2">
			<uc1:Banner id="banner1" runat="server" />
			<table width="1000" border="0" cellpadding="3" cellspacing="0">
				<tr>
					<td id="leftcolumn" valign="top">
						<uc1:Menu id="Menu1" runat="server"></uc1:Menu>
					</td>
					<td width="10">&nbsp;</td>
					<td align="left" id="content">
						<h1><img src="../images/cup.gif" border="0" alt="Room Administration" align="absMiddle">
							Room Administration</h1>
						<h3>Delete chat rooms</h3>
						<uc1:Lobbies id="Lobbies1" runat="server"></uc1:Lobbies>
					</td>
				</tr>
			</table>			
			<div id="footer">
				<p><a href="http://cutesoft.net">Copyright 2002-2015 CuteSoft.Net. All rights reserved.</a></p>
			</div>
		</form>
	</BODY>
</html>
