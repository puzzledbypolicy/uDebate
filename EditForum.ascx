<%@ Control Language="C#" Inherits="DotNetNuke.Modules.uDebate.EditForum" AutoEventWireup="false" Codebehind="EditForum.ascx.cs" %>
<%@ Register TagPrefix="atc" Namespace="ATC.WebControls" Assembly="ATC.WebControls" %>

<table cellspacing="3" cellpadding="3" class="MainTable">
    <tr>
        <td class="gridMainCaption" colspan="2">
            <asp:Label ID="headerLbl" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td colspan="2" class="gridToolBar" style="padding-bottom: 4px; padding-top: 2px; height: 24px" valign="top" align="left">
            <asp:LinkButton ID="btnNew" runat="server" CssClass="dnnPrimaryAction" OnClick="btnNew_Click" resourcekey="New">Add New </asp:LinkButton>
            &nbsp;&nbsp;
            <asp:LinkButton ID="deleteBtn" runat="server" CssClass="dnnPrimaryAction" OnClick="deleteBtn_Click" resourcekey="Delete">Delete</asp:LinkButton>
            </td>
    </tr>
    <tr>
				<td colspan="2"><asp:Label ID="lblMessage" runat="server" Visible="true"></asp:Label>
				</td></tr>
    <tr>
        <td valign="middle" style="width: 110px; height: 24px;">
            <asp:Label ID="Label1" runat="server" resourcekey="Title" >Title</asp:Label>
        </td>
        <td>
            <asp:TextBox ID="descTxt" runat="server" Width="330px"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td valign="top" style="width: 110px;">
            <asp:Label ID="Label2" runat="server" Text="Summary" resourcekey="Summary"></asp:Label>
        </td>
        <td>
            <asp:TextBox ID="summaryTxt" runat="server" Width="330px" Height="150px" TextMode="MultiLine"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td valign="middle" style="width: 110px;">
            <asp:Label ID="lbURL" runat="server" Text="Active" resourcekey="Active"></asp:Label>
        </td>
        <td>
            &nbsp;<asp:CheckBox ID="activeChk" runat="server" Checked="true" />
        </td>
    </tr>
    <tr>
        <td valign="middle" style="width: 110px;">
            <asp:Label ID="lblPosition" runat="server" Text="Position"></asp:Label>
        </td>
        <td>
            &nbsp;<asp:DropDownList ID="dropDownPosition" runat="server" >
                <asp:ListItem Value="1" Text="1" Selected="True"></asp:ListItem>
                <asp:ListItem Value="2" Text="2"></asp:ListItem>
                <asp:ListItem Value="3" Text="3"></asp:ListItem>
                <asp:ListItem Value="4" Text="4"></asp:ListItem>
                <asp:ListItem Value="5" Text="5"></asp:ListItem>
                <asp:ListItem Value="6" Text="6"></asp:ListItem>
                <asp:ListItem Value="7" Text="7"></asp:ListItem>
                <asp:ListItem Value="8" Text="8"></asp:ListItem>
                <asp:ListItem Value="9" Text="9"></asp:ListItem>
                <asp:ListItem Value="10" Text="10"></asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td valign="middle" style="width: 110px;">
            <asp:Label ID="lblUser" runat="server" Text="Admin User" Visible="false"></asp:Label>
        </td>
        <td>
            &nbsp;<asp:DropDownList ID="ddUser" runat="server" Visible="false"/>
        </td>
    </tr>
    <tr>
        <td height="100%" colspan="2">
        </td>
    </tr>
    <tr>
        <td colspan="2" class="gridToolBar" style="padding-bottom: 4px; padding-top: 2px;
            height: 24px" valign="top" align="left">
            <asp:LinkButton ID="saveBtn" runat="server" CssClass="dnnPrimaryAction" OnClick="saveBtn_Click" resourcekey="Save">Save</asp:LinkButton>
            &nbsp;&nbsp;
            <asp:LinkButton ID="cancelBtn" runat="server" CssClass="dnnPrimaryAction" OnClick="cancelBtn_Click" resourcekey="Cancel">Cancel</asp:LinkButton>
        
        </td>
    </tr>
</table>

<script type="text/javascript">
    jQuery(document).ready(function() {
        jQuery('#<%= deleteBtn.ClientID %>').dnnConfirm({
            text: 'The topic will be deleted. Are you sure?',
            yesText: '<%= Localization.GetString("Yes.Text", Localization.SharedResourceFile) %>',
            noText: '<%= Localization.GetString("No.Text", Localization.SharedResourceFile) %>',
            title: '<%= Localization.GetString("Confirm.Text", Localization.SharedResourceFile) %>'
        });
    });    
</script>

