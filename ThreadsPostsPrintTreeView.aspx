<%@ Page language="c#" Inherits="DotNetNuke.Modules.uDebate.ThreadsPostsPrintTreeView" CodeBehind="DotNetNuke.Modules.uDebate.ThreadsPostsPrintTreeView.cs" %>
<%@ Import namespace="ATC" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
	<head>
		<title><%=ATC.Tools.GetParam("SiteName")%></title>
		<base href="<%=ATC.Tools.GetParam("RootURL")%>" />		
		<meta name="CODE_LANGUAGE" content="C#" />
		<meta name="vs_defaultClientScript" content="JavaScript" />
		<link rel="stylesheet" href="desktopModules/uDebate/module.css" type="text/css" />
	</head>

    <body>
	    <form id="Form1" method="post" runat="server">
	    <table width="100%" cellpadding="0" cellspacing="0" border="0" runat="server" id="tblForums">
	            <tr>
	                <td><asp:Label ID="lbThreadTitle" runat="server" Text="" Font-Bold="true" Font-Names="Verdana" Font-Size="Medium"></asp:Label><br /><br /></td>
	            </tr>
                <tr>
                    <td width="400" valign="top" align="left">
	                    <asp:TreeView  
                          ID="TreeView1"
                          ExpandDepth="0" 
                          PopulateNodesFromClient="False"
                          ShowLines="True" 
                          Width="320" 
                          runat="server"                            
                          EnableViewState="False" 
                          ImageSet="BulletedList"                            
                          LineImagesFolder="DesktopModules/uDebate/images/TreeLineImages">
                            <SelectedNodeStyle Font-Bold="True" Font-Names="Tahoma" Font-Size="12px" />
                             <NodeStyle HorizontalPadding="3px" CssClass="thread_post_post_text" />                           
                        </asp:TreeView>
                    </td>
                 </tr>
           </table><br /><br />
           <button onclick="window.print();" CssClass="forum_button">Print</button>
           <button onclick="window.close();" CssClass="forum_button">Close</button>
          
	    </form>
	    
    </body>
</html>
