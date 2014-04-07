<%@ Control Language="C#" AutoEventWireup="True" Codebehind="ThreadsPostsView.ascx.cs" Inherits="DotNetNuke.Modules.uDebate.ThreadsPostsView"%>
<%@ Register TagPrefix="dnn" TagName="texteditor" Src="~/controls/texteditor.ascx" %>
<script type="text/javascript">
    function popup(url,winname)
    {
     var width  = 640;
     var height = 480;
     var left   = (screen.width  - width)/2;
     var top    = (screen.height - height)/2;
     var params = 'width='+width+', height='+height;
     params += ', top='+top+', left='+left;
     params += ', directories=no';
     params += ', location=no';
     params += ', menubar=no';
     params += ', resizable=no';
     params += ', scrollbars=no';
     params += ', status=no';
     params += ', toolbar=no';
     newwin=window.open(url,winname, params);
     if (window.focus) {newwin.focus()}
     return false;
    }
    
    function DeletePost(path)
    {
        if (confirm('<%=ATC.Translate.String("Are you sure?","",CurrentLanguageCode) %>\n<%=ATC.Translate.String("The post will be permanently deleted!","",CurrentLanguageCode) %>'))
            window.location=path;
    }
    
</script>
<table border="0" width="100%">
    <tr>
        <td>
            <asp:Panel ID="PanelPosts" runat="server">
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td>
                            <asp:Label ID="lblConfirm" runat="server" Text="" CssClass="forum_confirmation"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:PlaceHolder ID="Forums" runat="server"></asp:PlaceHolder>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnReply" runat="server" CssClass="forum_button" OnClick="btnReply_Click" />
                            &nbsp;
                            <span id="loginRegDiv" runat="server" visible="false" class="forum_text">
                                <%= ATC.Translate.String("You must be logged in to post a reply to this thread!","",CurrentLanguageCode) %>
                                <span class='forum_link'><a href='javascript:scroll(0,0)' class='forum_link'><%=ATC.Translate.String("Login", "", CurrentLanguageCode)%></a></span> | <span class='forum_link'><a href='?page=forumregister' class='forum_link'><%=ATC.Translate.String("Register", "", CurrentLanguageCode)%></a></span>
                            </span>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Panel Visible="false" ID="PanelReply" runat="server" Height="80px" Width="100%">
                <table  width="100%" cellpadding="0" cellspacing="0" border="0">
                    <tr>
                        <asp:Label ID="lblError" runat="server" Text="" CssClass="forum_error"></asp:Label>
                    </tr>
                    <tr>
                        <td>
                            <dnn:texteditor id="txtReply" runat="server" AutoDetectLanguage="true" toolbarset="Forum_Post" Width="100%" Height="400px"></dnn:texteditor>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnCancel" CssClass="forum_button" runat="server" OnClick="btnCancel_Click" />
                            &nbsp;&nbsp;
                            <asp:Button runat="server" CssClass="forum_button" ID="btnSave" OnClick="btnSave_Click" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
    </tr>
</table>