<%@ Control Language="c#" %>
<div id="header">
    <table style="height:88px; width:820px; margin-left:auto; margin-right:auto;" cellpadding="10">
        <tr>
            <td style="width: 250px;">
                <br />
                <a href="default.aspx">
                    <asp:image runat="server" id="logo" imageurl="~/images/yoursite.png" BorderWidth="0" />
                </a>
            </td>
            <td>
			   <br />
				<!--- add the live support chat button  -->
				<script src='<%=ResolveUrl("~/CuteSoft_Client/CuteChat/")%>Support-Image-Button.js.aspx'></script>
             
				
				<!--- add the live support visitor monitor tag  -->
				<script src='<%=ResolveUrl("~/CuteSoft_Client/CuteChat/")%>Support-Visitor-monitor.js.aspx'></script>
            </td>
            <td style="text-align:right">
				<asp:hyperlink runat="server" text="Home" navigateurl="~/Default.aspx" id="LinkHome" />&nbsp;|&nbsp;
				<%if(Context.User.Identity.IsAuthenticated){%>
					<%if(CuteChat.ChatWebUtility.CurrentIdentityIsAdministrator){%>
					 <a href="ChangePassword.aspx">Change password</a>&nbsp;|&nbsp;
					 <a href="Manager.aspx">Dashboard</a>&nbsp;|&nbsp;
					 <a href="Login.aspx?Logout=1">Logout</a>
					<%}%>
				<%}else{%>
					 <a href="Login.aspx">Login</a>
				<%}%>
            </td>
         </tr>
    </table>
</div>

<script runat="server">
	void Page_Load(object sender, System.EventArgs e)
	 {
	    // Put user code to initialize the page here
		CuteChat.AppChatIdentity identity = CuteChat.ChatProvider.Instance.GetLogonIdentity();	
	}
</script>

