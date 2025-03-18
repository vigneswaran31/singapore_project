<%@ Page Language="c#" %>

<%@ Register TagPrefix="uc1" TagName="TopBanner" Src="banner.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Footer" Src="Footer.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Live Support Home</title>
    <link rel='stylesheet' href='style.css' />
</head>
<body>
    <form id="Form1" method="post" runat="server">
        <uc1:TopBanner ID="Banner1" runat="server"></uc1:TopBanner>
        <table cellpadding="8" style="margin-left:auto; margin-right:auto;width:820px;">
            <tr>
                <td id="left-column">
                    <div class="infobox">
                        <h2>
                            Cute Live Support</h2>
                        <div class="padding10">
                            <div style="text-align: center">
                                <img src="images/phone_girl.jpg" alt="Deliver live customer support on your site!" /></div>
                            <br />
                            A real-time online chat support. Let your customer reach out and speak with your
                            representatives.
                        </div>
                    </div>
                    <div>
                        <div class="infobox">
                            <h2>
                                Hosted Edition?</h2>
                            <div class="padding10">
                                1. Low Cost<br />
                                2. Easy Setup<br />
                                3. Works On Any Web Site<br /><br />
                                Check <a href="https://mylivechat.com" target="_blank" style="font-size: 12px;">mylivechat</a>
                            </div>
                        </div>
                    </div>
                </td>
                <td>
                    <div class="infobox">
                        <h2>
                            Configurating Cute live support</h2>
                        <div class="padding10">
                            <p>
                                Log in as "admin" using the default admin credentials, click the <a href="Manager.aspx" style="font-size: 12px;">Dashboard</a>
                                button to add/remove operators, manage departments. The default admin credentials
                                are:
                                <br />
                                <br />
                                UserName: admin
                                <br />
                                Password: &nbsp;admin</p>
                        </div>
                    </div>
                    <div class="infobox">
                        <h2>
                            Adding chat button and monitor tag to web pages</h2>
                        <div class="padding10">
                            <p>
                                Chat Button appears as a graphic button with online or offline images. In order
                                to track a visitor on a specific page in your web site and invite them to chat,
                                the page must include monitor tag. This tag is invisible to the visitors.
                            </p>
                            We created three examples showing how to add chat button and monitor tag to your
                            web pages.
                            <ul>
                                <li><a href="default.aspx">default.aspx (banner.ascx)</a> </li>
                                <li><a href="test.htm">test.htm</a> </li>
                                <li><a href="test.asp">test.asp</a> </li>
                            </ul>
                            <br />
                        </div>
                    </div>
                </td>
            </tr>
        </table>
        <uc1:Footer ID="Footer1" runat="server"></uc1:Footer>
    </form>
</body>
</html>
