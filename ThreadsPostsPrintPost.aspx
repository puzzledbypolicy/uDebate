<%@ Page language="c#" Inherits="DotNetNuke.Modules.uDebate.ThreadsPostsPrintPost" Codebehind="ThreadsPostsPrintPost.aspx.cs" %>
<%@ Import namespace="ATC" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
	<head>
		<title><%=ATC.Tools.GetParam("SiteName")%></title>
		<base href="<%=ATC.Tools.GetParam("RootURL")%>" />		
		<meta name="CODE_LANGUAGE" content="C#" />
		<meta name="vs_defaultClientScript" content="JavaScript" />
		<link rel="stylesheet" href="styles/styles.css" type="text/css" />
        <script type="text/javascript">
		        function OpenDialog(URL,Name, Width, Height)
		        {
			        if (document.all)
			        {
				        var ret = window.showModalDialog("Admin/Forms/OpenDialog.aspx?URL='"+URL+"'", Name, "center:yes;resizable:yes;help:no;dialogWidth:"+Width+"px;dialogHeight:"+Height+"px;status=yes;scroll:no");
				        window.location = window.location.href;
			        }
			        else
			            OpenWindow(URL,Name, Width, Height);
			        
			        return ret;
		        }
		        
                function OpenZoomedDialog(URL,Name)
                {
	                return OpenDialog(URL,Name, screen.width-20, screen.height-20);
                }
		</script>
	</head>
   
    <body>
	    <form id="Form1" method="post" runat="server">
	    <table cellpadding="0" cellspacing="0" border="0" width="100%">
                <tr>
                    <td valign="top">
                            <table cellpadding="3" cellspacing="0" border="0" width="100%" bgcolor="#7992cb" height="35">
                                <tr>
                                    <td class="forum_header_title_text">
                                        <asp:Label ID="lbMessagePost" runat="server" Text="&nbsp;" CssClass="forum_header_title_text"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        <div style="width:580px;height:640px;overflow:auto;background-color:#eeeeee;">
                            <table cellpadding="3" cellspacing="0" border="0">
                                <tr>
                                    <td>
                                        <asp:Label ID="lbBody" runat="server" Text="" Width="560" CssClass="lexis_text"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </td>
                </tr>
            </table>
                                            <br /><br />
           <button onclick="window.print();" CssClass="forum_button">Print</button>
           <button onclick="window.close();" CssClass="forum_button">Close</button>
	    </form>
    </body>
</html>
