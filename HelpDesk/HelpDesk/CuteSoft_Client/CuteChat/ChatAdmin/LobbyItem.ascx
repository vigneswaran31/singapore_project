<%@ Control Language="c#" AutoEventWireup="false" Inherits="CuteChat.ChatCtrlBase" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Import Namespace="CuteChat" %>
<script runat="server">
string MaxIdleMinute;
string GreetingMessage;
string PasswordProtected;
string AutoAwayMinute;
string HistoryCount;
string HistoryDays;
string MaxUsers;
string LockRomm;
string AllowAnonymous;
string SortOrder;
string Integration;
string RoomName;
string RoomDescription;
string AvatarChatURL;

protected override void OnInit(EventArgs args)
{
	SetUIValues(new CuteChat.AppLobby());

	base.OnInit(args);
}

public TextBox TextBoxIntegration
{
	get
	{
		return tb_Integration;
	}
}

public void SetUIValues(IChatLobby lobby)
{
	MaxIdleMinute="Maximum amount of time, in minutes, that the user can remain idle. After the specified number of minutes have elapsed, the user will be disconnected or terminated.";
	GreetingMessage="The greeting message appears when each user joins this chat room.";
	PasswordProtected="You can create a password protected chat room that only users who know the password can enter.";
	AutoAwayMinute="Maximum amount of time, in minutes, that the user can remain idle. After the specified number of minutes have elapsed, the user status will be marked as AWAY.";
	HistoryCount="When a user joins the chat  room, Cute Chat shows the old chat record of this chat room. This defines how many records will be displayed when each user joins this chat room.";
	HistoryDays="When a user joins the chat  room, Cute Chat shows the old chat record of this chat room. This defines how many days of records will be displayed when each user joins this chat room.";
	MaxUsers="This sets the maximum number of users that can be in the chat room at once.";
	LockRomm="Locking this chat room prevents other users from entering and disrupting the conversation.";
	AllowAnonymous="If you want to allow anonymous users without logging in your site, you can check this box. The anonymous users can come into chats as guest-#.";
	SortOrder="The sort order of the chat room.";
	Integration="This field is used when developers integrate chat rooms.";
	RoomName="Provide a name for the chat room.";
	RoomDescription="Enter a description of this chat room.";
	AvatarChatURL="Default: ~/CuteSoft_Client/CuteChat/AvatarChat/CuteChat4.gif";
	
	tb_Name.Text=lobby.Title;
	tb_Description.Text=lobby.Description;
	tb_Announcement.Text=lobby.Announcement;
	tb_Integration.Text=lobby.Integration;
	tb_AvatarChatURL.Text = lobby.AvatarChatURL;
	tb_Password.Text=lobby.Password;
	
	cb_Locked.Checked=lobby.Locked;
	cb_Anonymous.Checked=lobby.AllowAnonymous;
	cb_AvatarChat.Checked=lobby.IsAvatarChat;
	
	tb_MaxOnline.Text = GetText(lobby.MaxOnlineCount);
	tb_AutoAwayMinute.Text = GetText(lobby.AutoAwayMinute);
	tb_MaxIdleMinute.Text = GetText(lobby.MaxIdleMinute);
	tb_HistoryCount.Text = GetText(lobby.HistoryCount);
	tb_HistoryDay.Text = GetText(lobby.HistoryDay);
	tb_SortIndex.Text = GetText(lobby.SortIndex);

}

public bool GetUIValues(IChatLobby lobby)
{
	if(tb_Name.Text.Trim().Length==0)
	{
		Alert("[[ChannelNameRequired]]");
		return false;
	}
	
	bool valid=true;
	
	ParseInt(tb_MaxOnline.Text,ref valid);
	ParseInt(tb_AutoAwayMinute.Text,ref valid);
	ParseInt(tb_MaxIdleMinute.Text,ref valid);
	ParseInt(tb_HistoryCount.Text,ref valid);
	ParseInt(tb_HistoryDay.Text,ref valid);
	ParseInt(tb_SortIndex.Text,ref valid);
	
	if(!valid)return false;
	
	lobby.Title=tb_Name.Text.Trim();
	lobby.Description=tb_Description.Text.Trim();
	lobby.Announcement=tb_Announcement.Text.Trim();
	lobby.AvatarChatURL=tb_AvatarChatURL.Text.Trim();
	lobby.Integration=tb_Integration.Text.Trim();
	lobby.Locked=cb_Locked.Checked;
	lobby.AllowAnonymous=cb_Anonymous.Checked;
	lobby.IsAvatarChat=cb_AvatarChat.Checked;
	lobby.Password=tb_Password.Text;
	
	lobby.MaxOnlineCount=ParseInt(tb_MaxOnline.Text,ref valid);
	lobby.AutoAwayMinute=ParseInt(tb_AutoAwayMinute.Text,ref valid);
	lobby.MaxIdleMinute=ParseInt(tb_MaxIdleMinute.Text,ref valid);
	lobby.HistoryCount=ParseInt(tb_HistoryCount.Text,ref valid);
	lobby.HistoryDay=ParseInt(tb_HistoryDay.Text,ref valid);
	lobby.SortIndex=ParseInt(tb_SortIndex.Text,ref valid);
	
	
	
	return true;
}

string GetText(int val)
{
	if(val==-1)
		return "unlimited";
	return val.ToString();
}

int ParseInt(string text,ref bool valid)
{
	text=text.Trim();
	
	if(text=="unlimited")
		return -1;

	try
	{
		return int.Parse(text);
	}
	catch
	{
		if(valid)
		{
			Alert("[[NumberOnly]]");
		}
		valid=false;
		return 0;
	}
}

string _alertmsg;
void Alert(string msg)
{
	_alertmsg=msg;
	holder_alert.Visible=true;
}

</script>
<asp:PlaceHolder ID="holder_alert" Runat="server" Visible="False" EnableViewState="False">
	<SCRIPT>
setTimeout(function(){
alert('<%=_alertmsg%>');
},400);
	</SCRIPT>
</asp:PlaceHolder>
<style>
#ToolTip { BORDER-RIGHT: #000 1px solid; PADDING-RIGHT: 4px; BORDER-TOP: #000 1px solid; PADDING-LEFT: 4px; FONT-SIZE: 11px; Z-INDEX: 10000; LEFT: 0px; VISIBILITY: hidden; PADDING-BOTTOM: 4px; BORDER-LEFT: #000 1px solid; WIDTH: 200px; COLOR: #000; LINE-HEIGHT: 1.3; PADDING-TOP: 4px; BORDER-BOTTOM: #000 1px solid; FONT-FAMILY: verdana; POSITION: absolute; TOP: 0px; BACKGROUND-COLOR: lightyellow }
</style>
<script language="javascript">
function showToolTip(e,text){
	var ToolTip=document.getElementById("ToolTip");
	ToolTip.innerHTML=text;
	ToolTip.style.pixelLeft=(e.x+20+document.body.scrollLeft);
	ToolTip.style.pixelTop=(e.y+document.body.scrollTop);
	ToolTip.style.visibility="visible";
}
function hideToolTip(){
	var ToolTip=document.getElementById("ToolTip");
	ToolTip.style.visibility="hidden";
}
</script>
<table cellspacing="1" cellpadding="3" border="0" class="box">
	<tr>
		<td class="boxTitle" height="30">
			Chat Room Setting
		</td>
	</tr>
	<tr>
		<td class="boxArea">
			<table cellpadding="5" cellspacing="0" border="0" width="700">
				<tr>
					<td width="200">Name</td>
					<td>
						<asp:TextBox id="tb_Name" Width="320px" runat="server"></asp:TextBox>
					</td>
					<td>
                        <a href="#" class="tooltip">
    <img src="../images/help.png">
    <span>
        <img class="callout" src="../images/callout.gif" />
        <strong>Room Name</strong><br />
        <%=RoomName%>
    </span>
</a>
					</td>
				</tr>
				<tr>
					<td>Description</td>
					<td>
						<asp:TextBox id="tb_Description" Width="320px" runat="server"></asp:TextBox>
					</td><td>
                        <a href="#" class="tooltip">
    <img src="../images/help.png">
    <span>
        <img class="callout" src="../images/callout.gif" />
        <strong>Room Description</strong><br />
        <%=RoomDescription%>
    </span>
</a>
					</td>
				</tr>
				<tr>
					<td>Greeting Message</td>
					<td>
						<asp:TextBox id="tb_Announcement" runat="server" Width="320px" TextMode="MultiLine"></asp:TextBox>
					</td><td>
                        <a href="#" class="tooltip">
    <img src="../images/help.png">
    <span>
        <img class="callout" src="../images/callout.gif" />
        <strong>Greeting Message</strong><br />
        <%=GreetingMessage%>
    </span>
</a>
					</td>
				</tr>
				<tr>
					<td>Password</td>
					<td><asp:TextBox ID="tb_Password" Width="320px" Runat="server"></asp:TextBox>
					</td><td>
                        <a href="#" class="tooltip">
    <img src="../images/help.png">
    <span>
        <img class="callout" src="../images/callout.gif" />
        <strong>Room Password</strong><br />
        <%=PasswordProtected%>
    </span>
</a>
					</td>
				</tr>
				<tr>
					<td>Integration</td>
					<td><asp:TextBox id="tb_Integration" Width="320px" runat="server"></asp:TextBox>
					</td><td>
                        <a href="#" class="tooltip">
    <img src="../images/help.png">
    <span>
        <img class="callout" src="../images/callout.gif" />
        <strong>Integration</strong><br />
        <%=Integration%>
    </span>
</a>
					</td>
				</tr>
				<tr>
					<td>Max online users</td>
					<td><asp:TextBox id="tb_MaxOnline" Width="320px" runat="server"></asp:TextBox>
					</td><td>
                        <a href="#" class="tooltip">
    <img src="../images/help.png">
    <span>
        <img class="callout" src="../images/callout.gif" />
        <strong>Max Users</strong><br />
        <%=MaxUsers%>
    </span>
</a>
					</td>
				</tr>
				<tr>
					<td>Sort Order</td>
					<td><asp:TextBox ID="tb_SortIndex" Width="320px" Runat="server" />
					</td><td>
                        <a href="#" class="tooltip">
    <img src="../images/help.png">
    <span>
        <img class="callout" src="../images/callout.gif" />
        <strong>Sort Order</strong><br />
        <%=SortOrder%>
    </span>
</a>
					</td>
				</tr>
				<tr>
					<td>Auto Away Minute</td>
					<td><asp:TextBox id="tb_AutoAwayMinute" Width="320px" runat="server"></asp:TextBox>
					</td><td>
                        <a href="#" class="tooltip">
    <img src="../images/help.png">
    <span>
        <img class="callout" src="../images/callout.gif" />
        <strong>Auto Away Minute</strong><br />
        <%=AutoAwayMinute%>
    </span>
</a>
					</td>
				</tr>
				<tr>
					<td>Max Idle Minute</td>
					<td><asp:TextBox ID="tb_MaxIdleMinute" Width="320px" Runat="server"></asp:TextBox>
					</td><td>
                        <a href="#" class="tooltip">
    <img src="../images/help.png">
    <span>
        <img class="callout" src="../images/callout.gif" />
        <strong>Max Idle Minute</strong><br />
        <%=MaxIdleMinute%>
    </span>
</a>
					</td>
				</tr>
				<tr>
					<td>History Count</td>
					<td><asp:TextBox ID="tb_HistoryCount" Width="320px" Runat="server"></asp:TextBox>
					</td><td>
                        <a href="#" class="tooltip">
    <img src="../images/help.png">
    <span>
        <img class="callout" src="../images/callout.gif" />
        <strong>History Count</strong><br />
        <%=HistoryCount%>
    </span>
</a>
					</td>
				</tr>
				<tr>
					<td>History Days</td>
					<td><asp:TextBox id="tb_HistoryDay" Width="320px" runat="server"></asp:TextBox>
					</td><td>
                        <a href="#" class="tooltip">
    <img src="../images/help.png">
    <span>
        <img class="callout" src="../images/callout.gif" />
        <strong>History Days</strong><br />
        <%=HistoryDays%>
    </span>
</a>
					</td>
				</tr>
				<tr>
					<td>Lock this Room</td>
					<td><asp:CheckBox id="cb_Locked" runat="server"></asp:CheckBox>
					</td><td>
                        <a href="#" class="tooltip">
    <img src="../images/help.png">
    <span>
        <img class="callout" src="../images/callout.gif" />
        <strong>Lock this Room</strong><br />
        <%=LockRomm%>
    </span>
</a>
					</td>
				</tr>
				<tr>
					<td>Allow Anonymous</td>
					<td><asp:CheckBox id="cb_Anonymous" runat="server"></asp:CheckBox>
					</td><td>
                        <a href="#" class="tooltip">
    <img src="../images/help.png">
    <span>
        <img class="callout" src="../images/callout.gif" />
        <strong>Allow Anonymous</strong><br />
        <%=AllowAnonymous%>
    </span>
</a>
					</td>
				</tr>
				<tr>
					<td>Avatar Chat Mode</td>
					<td>
						<asp:CheckBox id="cb_AvatarChat" runat="server"></asp:CheckBox></td>
					<td>
                        <a href="#" class="tooltip">
    <img src="../images/help.png">
    <span>
        <img class="callout" src="../images/callout.gif" />
        <strong>Avatar Chat Mode</strong><br />
        When enabled, Avatar Chat Mode will display.
    </span>
</a>
					</td>
				</tr>
				<tr>
					<td>Url of Room Picture</td>
					<td>
						<asp:TextBox Runat="server" Width="320px" ID="tb_AvatarChatURL"></asp:TextBox>
					</td><td>
                        <a href="#" class="tooltip">
    <img src="../images/help.png">
    <span>
        <img class="callout" src="../images/callout.gif" />
        <strong>Url of Room Picture</strong><br />
        <%=AvatarChatURL%>
    </span>
</a>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
<div id="ToolTip"></div>
