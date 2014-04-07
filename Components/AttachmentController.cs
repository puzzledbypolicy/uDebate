using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using DotNetNuke.Modules.uDebate.Data;
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

namespace DotNetNuke.Modules.uDebate.Components
{

	#region " AttachmentController "

	/// <summary>
	/// CRUD (and all db methods) Attachment Methods
	/// </summary>
	/// <remarks>Added by Skeel</remarks>
	public class AttachmentController
	{

		#region " Constructors "

		/// <summary>
		/// Instantiats the AttachmentContoller. 
		/// </summary>
		/// <remarks></remarks>
		public AttachmentController()
		{
		}

		#endregion

		#region " Private Properties "


		private string mCompleteAttachmentPath = string.Empty;
		private string CompleteAttachmentPath {
			get { return mCompleteAttachmentPath; }
			set { mCompleteAttachmentPath = value; }
		}

		#endregion

		#region " Public Methods "

		/// <summary>
		/// Gets a list of AttachmentInfo related to a Post
		/// </summary>
		/// <param name="PostID"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public List<AttachmentInfo> GetAllByPostID(int PostID)
		{

			List<AttachmentInfo> objAttachments = new List<AttachmentInfo>();
			IDataReader dr = null;
			try {
                DataProvider pro = new DotNetNuke.Modules.uDebate.Data.DataProvider();
                dr = pro.Attachment_GetAllByPostID(PostID);
				while (dr.Read()) {
					AttachmentInfo objAttachment = FillAttachmentInfo(dr);
					objAttachments.Add(objAttachment);
				}
				dr.NextResult();

			} catch (Exception ex) {
				//LogException(ex);
			} finally {
				if ((dr != null)) {
					dr.Close();
				}
			}

			return objAttachments;

		}

		/// <summary>
		/// Gets a list of AttachmentInfo related to a UserId and NOT related to any posts
		/// </summary>
		/// <param name="UserID"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public List<AttachmentInfo> GetAllByUserID(int UserID)
		{
			List<AttachmentInfo> objAttachments = new List<AttachmentInfo>();
			IDataReader dr = null;

			try {
                DataProvider pro = new DotNetNuke.Modules.uDebate.Data.DataProvider();
                dr = pro.Attachment_GetAllByUserID(UserID);
				while (dr.Read()) {
					AttachmentInfo objAttachment = FillAttachmentInfo(dr);
					objAttachments.Add(objAttachment);
				}
				dr.NextResult();

			} catch (Exception ex) {
				//LogException(ex);
			} finally {
				if ((dr != null)) {
					dr.Close();
				}
			}

			return objAttachments;

		}

		/// <summary>
		/// Adds an AttachmentInfo object
		/// </summary>
		/// <param name="objAttachment"></param>
		/// <remarks></remarks>
		public void Update(AttachmentInfo objAttachment)
		{
            DataProvider pro = new DotNetNuke.Modules.uDebate.Data.DataProvider();
            pro.Attachment_Update(objAttachment);
		}

		#endregion

		#region " Custom Hydrator "

		/// <summary>
		/// Hydrates the AttachmentInfo object
		/// </summary>
		/// <param name="dr"></param>
		/// <returns></returns>
		/// <remarks>
		/// </remarks>
		/// <history>
		/// 	[skeel]	12/22/2008	Created
		/// </history>
		private AttachmentInfo FillAttachmentInfo(IDataReader dr)
		{
			AttachmentInfo objAttachment = new AttachmentInfo();
            try
            {
                objAttachment.AttachmentID = Convert.ToInt32(dr["AttachmentID"]);
            }
            catch
            {
            }
			try {
                objAttachment.FileID = Convert.ToInt32(dr["FileID"]);
			} catch {
			}
			try {
                objAttachment.PostID = Convert.ToInt32(dr["PostID"]);
			} catch {
			}
			try {
				objAttachment.UserID = Convert.ToInt32(dr["UserID"]);
			} catch {
			}
			try {
                objAttachment.LocalFileName = Convert.ToString(dr["LocalFileName"]);
			} catch {
			}
			try {
                objAttachment.FileName = Convert.ToString(dr["FileName"]);
			} catch {
			}
			try {
                objAttachment.Extension = Convert.ToString(dr["Extension"]);
			} catch {
			}
			try {
                objAttachment.Size = Convert.ToInt32(dr["Size"]);
			} catch {
			}
			try {
                objAttachment.Inline = Convert.ToBoolean(dr["Inline"]);
			} catch {
			}
			try {
                objAttachment.Width = Convert.ToInt32(dr["Width"]);
			} catch {
			}
			try {
                objAttachment.Height = Convert.ToInt32(dr["Height"]);
			} catch {
			}

			return objAttachment;
		}

		#endregion

	}

	#endregion

}
