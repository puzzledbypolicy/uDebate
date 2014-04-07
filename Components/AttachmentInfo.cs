using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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

	/// <summary>
	/// A single instance of the AttachmentInfo Object.
	/// </summary>
	/// <remarks>Added by Skeel</remarks>
	public class AttachmentInfo
	{

		#region " Private Members "

		private int _AttachmentID;
		private int _FileID;
		private int _PostID;
		private int _UserID;
		private string _LocalFileName;
		private string _FileName;
		private string _Extension;
		private int _Size;
		private bool _Inline;
		private int _Width;

		private int _Height;
		#endregion

		#region " Public Properties "

		/// <summary>
		/// The PK AttachmentID associated with the file.
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		public int AttachmentID {
			get { return _AttachmentID; }
			set { _AttachmentID = value; }
		}

		/// <summary>
		/// The DotNetNuke FileID.
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks>All attachments must be part of the DNN File system.</remarks>
		public int FileID {
			get { return _FileID; }
			set { _FileID = value; }
		}

		/// <summary>
		/// The PostID the attachment is associated with.
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		public int PostID {
			get { return _PostID; }
			set { _PostID = value; }
		}

		/// <summary>
		/// UserID - we need this to collect "lost" attachments
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		public int UserID {
			get { return _UserID; }
			set { _UserID = value; }
		}

		/// <summary>
		/// LocalFileName - Used for the user to identify the file uploaded
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		public string LocalFileName {
			get { return _LocalFileName; }
			set { _LocalFileName = value; }
		}

		/// <summary>
		/// FileName - Actual FileName from DNN Files (generated through the GUID model)
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		public string FileName {
			get { return _FileName; }
			set { _FileName = value; }
		}

		/// <summary>
		/// Extension - We use this display either image or link depending of extension
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		public string Extension {
			get { return _Extension; }
			set { _Extension = value; }
		}

		/// <summary>
		/// Size - File size in bytes
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		public int Size {
			get { return _Size; }
			set { _Size = value; }
		}

		/// <summary>
		/// Inline will return true, if the attachment is to be placed inline the postbody
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		public bool Inline {
			get { return _Inline; }
			set { _Inline = value; }
		}

		/// <summary>
		/// Width (Will only apply for image files)
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		public int Width {
			get { return _Width; }
			set { _Width = value; }
		}

		/// <summary>
		/// Height (Will only apply for image files)
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		public int Height {
			get { return _Height; }
			set { _Height = value; }
		}

		#endregion

		#region " Public ReadOnly Properties "

		/// <summary>
		/// FormattedSize - File size in bytes
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		public string FormattedSize {
			get { return FormatBytes(_Size); }
		}

		#endregion

		#region " Private Methods "

		/// <summary>
		/// Formats an integer value presumed as bytes to bytes/KB/MB/GB
		/// </summary>
		/// <param name="Bytes"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		private string FormatBytes(int Bytes)
		{
			string strBytes = string.Empty;
			try {
				if (Bytes >= 1073741824) {
					strBytes = Strings.Format(Bytes / 1024 / 1024 / 1024, "#0.00") + " GB";
				} else if (Bytes >= 1048576) {
					strBytes = Strings.Format(Bytes / 1024 / 1024, "#0.00") + " MB";
				} else if (Bytes >= 1024) {
					strBytes = Strings.Format(Bytes / 1024, "#0.00") + " KB";
				} else if (Bytes < 1024) {
					strBytes = Conversion.Fix(Bytes) + " Bytes";
				}
			} catch (Exception ex) {
				strBytes = "0 Bytes";
			}

			return strBytes;
		}

		#endregion

	}

}
