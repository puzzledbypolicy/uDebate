using System;
using System.Collections;
using System.Collections.Generic;
//
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2011
// by DotNetNuke Corporation
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
//

using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Localization;
using DotNetNuke.Modules.uDebate.Components;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Common;
using System.Web.UI.WebControls;

namespace DotNetNuke.Modules.uDebate.Controls
{

	/// <summary>
	/// A subcontrol used to upload, display and select Attachments 
	/// </summary>
	/// <remarks></remarks>
	public partial class AttachmentControl : PortalModuleBase
	{

		#region " Private Members "

		private string _baseFolder = string.Empty;
		private string _localResourceFile = string.Empty;

		private string _fileFilter = string.Empty;
		#endregion

		#region " Public Properties "

		/// <summary>
		/// Local Resource file for localization. 
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		public new string LocalResourceFile {
			get {
				string fileRoot = null;

				if (_localResourceFile == string.Empty) {
					fileRoot = this.TemplateSourceDirectory + "/" + Localization.LocalResourceDirectory + "/AttachmentControl.ascx";
				} else {
					fileRoot = _localResourceFile;
				}
				return fileRoot;
			}
			set { _localResourceFile = value; }
		}

		/// <summary>
		/// PostId
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		public int PostId {
			get {
				if ((ViewState["PostId"] != null)) {
					return Convert.ToInt32(ViewState["PostId"].ToString());
				} else {
					return -1;
				}
			}
			set { ViewState["PostId"] = Convert.ToString(value); }
		}

		/// <summary>
		/// Width
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		[System.ComponentModel.Bindable(true), System.ComponentModel.Category("Behavior"), System.ComponentModel.DefaultValue("Normal")]
		public string Width {
			get {
				if ((ViewState["Width"] != null)) {
					return ViewState["Width"].ToString();
				} else {
					return "350px";
				}
			}
			set { ViewState["Width"] = value; }
		}

		/// <summary>
		/// A list of attachmentid's bound to the control.
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		public string lstAttachmentIDs {
			get {
				if ((ViewState["lstAttachmentIDs"] != null)) {
					return ViewState["lstAttachmentIDs"].ToString();
				} else {
					return "";
				}
			}
			set { ViewState["lstAttachmentIDs"] = value; }
		}

		#endregion

		#region " Private ReadOnly Properties "

		/// <summary>
		/// Post portal root folder path setting.
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		private string BaseFolder {
			get {
				if (_baseFolder == string.Empty) {
                    _baseFolder = "/";// objConfig.AttachmentPath;
				}

				if (_baseFolder.EndsWith("/") == false)
					_baseFolder += "/";

				return _baseFolder;
			}
		}

		/// <summary>
		/// We need the ModuleID set so we can get configuration settings for the avatar control. 
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		public int ModuleID {
			get {
				if (ViewState["ModuleID"] != null) {
					return Convert.ToInt32(ViewState["ModuleID"].ToString());
				} else {
					return -1;
				}
			}
			set { ViewState["ModuleID"] = Convert.ToInt32(value); }
		}

		/// <summary>
		/// Used to identify if the RichEditor Mode has been changed
		/// </summary>
		/// <value></value>
		/// <returns>RICH/BASIC/""</returns>
		/// <remarks></remarks>
		private string EditorMode {
			get {
				if ((ViewState["EditorMode"] != null)) {
					return ViewState["EditorMode"].ToString();
				} else {
					return string.Empty;
				}
			}
			set { ViewState["EditorMode"] = value; }
		}

		/// <summary>
		/// DeleteItems Are Used to Delete files while not being locked
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		private string DeleteItems {
			get {
				if ((ViewState["DeleteItems"] != null)) {
					return ViewState["DeleteItems"].ToString();
				} else {
					return string.Empty;
				}
			}
			set { ViewState["DeleteItems"] = value; }
		}

		/// <summary>
		/// This is the user who is viewing the forum.
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
        //private ForumUserInfo CurrentForumUser {
        //    get {
        //        ForumUserController cntForumUser = new ForumUserController();
        //        return cntForumUser.GetForumUser(Users.UserController.GetCurrentUserInfo.UserID, false, ModuleID, objConfig.CurrentPortalSettings.PortalId);
        //    }
        //}

		/// <summary>
		/// The userid of the person currently using this control. 
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		private int CurrentUserID {
			get {
				if (ViewState["CurrentUserID"] != null) {
					return Convert.ToInt32(ViewState["CurrentUserID"].ToString());
				} else {
                    ViewState["CurrentUserID"] = Entities.Users.UserController.GetCurrentUserInfo().UserID.ToString();
                    return Entities.Users.UserController.GetCurrentUserInfo().UserID;
				}
			}
		}

		/// <summary>
		/// This is the forum's configuration so it can be used by loaded controls.
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
        //public Forum.Configuration objConfig {
        //    get { return System.Configuration.GetForumConfig(ModuleID); }
        //}

		#endregion

		#region " Event Handlers "

		/// <summary>
		/// When the control is loaded, we want to make sure the cmdUpload is registered for postback because of the nature of the file upload control and security. 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks></remarks>
		protected void Page_Init(object sender, System.EventArgs e)
		{
			if (DotNetNuke.Framework.AJAX.IsInstalled()) {
				DotNetNuke.Framework.AJAX.RegisterScriptManager();
				DotNetNuke.Framework.AJAX.WrapUpdatePanelControl(pnlContainer, false);
				DotNetNuke.Framework.AJAX.RegisterPostBackControl(cmdUpload);
			}
			tblAttachment.Width = System.Web.UI.WebControls.Unit.Parse(Width).ToString();
		}

		/// <summary>
		/// When the control is loaded, we want to make sure the cmdUpload is registered for postback because of the nature of the file upload control and security. 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks></remarks>
		protected void Page_Load(object sender, System.EventArgs e)
		{
			//Localization (needed because of AJAX)
			cmdDelete.ToolTip = Localization.GetString("Delete", this.LocalResourceFile);
			cmdUpload.Text = Localization.GetString("Upload", this.LocalResourceFile);
            //plUpload.Text = Localization.GetString("plUpload", this.LocalResourceFile) + ":";
            //plUpload.HelpText = Localization.GetString("plUpload.Help", this.LocalResourceFile);
            //plAttachments.Text = Localization.GetString("plAttachments", this.LocalResourceFile) + ":";
            //plAttachments.HelpText = Localization.GetString("plAttachments.Help", this.LocalResourceFile);

			// We can only delete files while not in use, look for Items to be deleted (PostID set to -2)
			if (Page.IsPostBack == false) {
				AttachmentController cntAttachment = new AttachmentController();
				List<AttachmentInfo> lstAttachments = cntAttachment.GetAllByPostID(-2);
				if (lstAttachments.Count > 0) {
					foreach (AttachmentInfo objFile in lstAttachments) {
						DeleteItems += objFile.FileName + ";";
					}
				}

			} else {
				//Delete any files set for deletion

				if (DeleteItems != string.Empty) {
					string ParentFolderName = PortalSettings.HomeDirectoryMapPath;
					ParentFolderName += BaseFolder;
					ParentFolderName = ParentFolderName.Replace("/", "\\");
					if (ParentFolderName.EndsWith("\\") == false)
						ParentFolderName += "\\";

					char[] splitter = { ';' };
					string[] Array = DeleteItems.Split(splitter);

					foreach (string item in Array) {
						if (item != string.Empty) {
							DotNetNuke.Common.Utilities.FileSystemUtils.DeleteFile(ParentFolderName + item, PortalSettings, true);
							// Remove the filename from DeleteItems.
							// If it was not deleted, it will be picked up next time
							DeleteItems = DeleteItems.Replace(item + ";", "");
						}
					}
				}
			}
		}

		/// <summary>
		/// This uploads a file which generates a GUID name, uses original image extension as save type. 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks></remarks>
		protected void cmdUpload_Click(System.Object sender, System.EventArgs e)
		{
            lblMessage.Text = string.Empty;
			try {
				// if no file is selected exit
				if (fuFile.PostedFile.FileName == string.Empty) {
					return;
				}

                // Maximum attachment filesize = 5MB
				if (fuFile.PostedFile.InputStream.Length > (/*objConfig.MaxAttachmentSize*/5000 * 1024)) {
					lblMessage.Text = Localization.GetString("MaxFileSize", this.LocalResourceFile) /*+ objConfig.MaxAttachmentSize.ToString()*/ + "5 MB";
					return;
				}

				string FileName = System.IO.Path.GetFileName(fuFile.PostedFile.FileName);

				//Get destination folder as mappath
				string ParentFolderName = PortalSettings.HomeDirectoryMapPath;
				//ParentFolderName += BaseFolder;
				ParentFolderName = ParentFolderName.Replace("/", "\\");
				if (ParentFolderName.EndsWith("\\") == false)
					ParentFolderName += "\\";

                string strExtension = System.IO.Path.GetExtension(fuFile.PostedFile.FileName).Replace(".", "");
				if (_fileFilter != string.Empty /*& Strings.InStr("," + _fileFilter.ToLower(), "," + strExtension.ToLower()) == 0*/) {
					// trying to upload a file not allowed for current filter
					lblMessage.Text = string.Format(Localization.GetString("UploadError", this.LocalResourceFile), _fileFilter, strExtension);
				}

				if (lblMessage.Text == string.Empty) {
                    //ParentFolderName = "C:\\inetpub\\wwwroot\\DNN6\\Portals\\0\\";
					string destFileName = Guid.NewGuid().ToString().Replace("-", "") + "." + strExtension;
					//lblMessage.Text = DotNetNuke.Common.Utilities.FileSystemUtils.UploadFile(ParentFolderName, fuFile.PostedFile, false);
                    
					if (lblMessage.Text != string.Empty)
						return;

                    //Rename the file using the GUID model					
                    //FileSystemUtils.MoveFile(ParentFolderName + FileName, ParentFolderName + destFileName, PortalSettings);
                    
                    destFileName = FileName;

					//Now get the FileID from DNN Filesystem
					int myFileID = 0;
                    ArrayList fileList = Globals.GetFileList(PortalId, strExtension);//, false, ParentFolderName, false);
					foreach (FileItem objFile in fileList) {
						if (objFile.Text == destFileName) {
							myFileID = Convert.ToInt32(objFile.Value);
						}
					}

					if (myFileID > 0) {
						//Now save the Attachment info
						AttachmentInfo objAttach = new AttachmentInfo();
						var _with1 = objAttach;
						_with1.PostID = PostId;
						_with1.UserID = UserId;
						_with1.FileID = myFileID;
						_with1.LocalFileName = FileName;
						_with1.Inline = false;

						AttachmentController cntAttachment = new AttachmentController();
						cntAttachment.Update(objAttach);
						BindFileList();
					}
				}
			} catch (Exception exc) {
				//ProcessModuleLoadException(this, exc);
                Response.Write(exc);
			}
		}

		/// <summary>
		/// Deletes the selected uploaded file, using the DNN filesystem 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks></remarks>
		protected void cmdDelete_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			lblMessage.Text = string.Empty;
			if (lstAttachments.SelectedIndex == -1)
				return;
			bool doDelete = false;
			try {
				doDelete = true;

				//Okay, we can go a head and delete

				if (doDelete == true) {
					string ParentFolderName = PortalSettings.HomeDirectoryMapPath;
					ParentFolderName += BaseFolder;
					ParentFolderName = ParentFolderName.Replace("/", "\\");
					if (ParentFolderName.EndsWith("\\") == false)
						ParentFolderName += "\\";
					DotNetNuke.Common.Utilities.FileSystemUtils.DeleteFile(ParentFolderName + lstAttachments.SelectedItem.Value, PortalSettings, true);
					BindFileList();

				}

			} catch (Exception ex) {
				// for security reasons, capture the problem but DON'T show the end user.
				//LogException(ex);
			}
		}

		#endregion

		#region " Interfaces "

		/// <summary>
		/// This runs only the first time the control is loaded via Ajax. 
		/// </summary>
		/// <remarks></remarks>
		public void LoadInitialView()
		{
			//Not get the host filefilter
            _fileFilter = Entities.Host.Host.FileExtensions;
			//Bind the lists
			BindFileList();
		}

		#endregion

		#region " Private Methods "

		/// <summary>
		/// Binds a list of files to a drop down list. 
		/// </summary>
		/// <remarks></remarks>
		private void BindFileList()
		{
			lstAttachments.Items.Clear();

			AttachmentController cntAttachment = new AttachmentController();
			List<AttachmentInfo> lstFiles = null;

			if (PostId > 0) {
				lstFiles = cntAttachment.GetAllByPostID(PostId);
				//Check for "lost" uploads from previous uncompleted posts and add them to the list
				List<AttachmentInfo> lstTemp = cntAttachment.GetAllByUserID(UserId);
				if (lstTemp.Count > 0) {
					lstFiles.AddRange(lstTemp);
				}
			} else {
				//Check for "lost" uploads from previous uncompleted posts and add them to the list
				lstFiles = cntAttachment.GetAllByUserID(UserId);
			}


			foreach (AttachmentInfo objFile in lstFiles) {
				ListItem lstItem = new ListItem();
				lstItem.Text = objFile.LocalFileName;
				lstItem.Value = objFile.FileName;
				lstAttachments.Items.Add(lstItem);
				lstAttachmentIDs += objFile.AttachmentID.ToString() + ";";

                ViewState["attachment"] = objFile.AttachmentID; 
			}
                      

		}
		public AttachmentControl()
		{
			Load += Page_Load;
			Init += Page_Init;
		}

		#endregion

	}

}
