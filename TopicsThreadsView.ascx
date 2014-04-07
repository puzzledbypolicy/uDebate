<%@ Control Language="C#" AutoEventWireup="True" Codebehind="TopicsThreadsView.ascx.cs" Inherits="DotNetNuke.Modules.uDebate.TopicsThreadsView"%>
<%@ Register TagPrefix="dnn" TagName="texteditor" Src="~/controls/texteditor.ascx" %>

<table border="0" width="100%">
    <tr>
        <td>
            <asp:PlaceHolder ID="Forums" runat="server"></asp:PlaceHolder>
        </td>
    </tr>
</table>