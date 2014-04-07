/*
' Copyright (c) 2010 DotNetNuke Corporation
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using System.Data;
using System;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Framework.Providers;
using Microsoft.ApplicationBlocks.Data;
using DotNetNuke.Modules.uDebate.Components;


namespace DotNetNuke.Modules.uDebate.Data
{

    /// -----------------------------------------------------------------------------
    /// <summary>
    /// An abstract class for the data access layer
    /// </summary>
    /// -----------------------------------------------------------------------------
    public class DataProvider
    {
        #region "Private Members"

private const string ProviderType = "data";
private Framework.Providers.ProviderConfiguration _providerConfiguration = Framework.Providers.ProviderConfiguration.GetProviderConfiguration(ProviderType);
private string _connectionString;
private string _providerPath;
private string _objectQualifier;
private string _databaseOwner;
private string _moduleDataPrefix = "Forum_";

private string _fullModuleQualifier;
#endregion

public DataProvider()
{
        // Read the configuration specific information for this provider
	Framework.Providers.Provider objProvider = (Framework.Providers.Provider)_providerConfiguration.Providers[_providerConfiguration.DefaultProvider];

	// This code handles getting the connection string from either the connectionString / appsetting section and uses the connectionstring section by default if it exists.  
	// Get Connection string from web.config
	_connectionString = DotNetNuke.Common.Utilities.Config.GetConnectionString();

	// If above funtion does not return anything then connectionString must be set in the dataprovider section.
	if (_connectionString == string.Empty) {
		// Use connection string specified in provider
		_connectionString = objProvider.Attributes["connectionString"];
	}

	_providerPath = objProvider.Attributes["providerPath"];

	_objectQualifier = objProvider.Attributes["objectQualifier"];
	if (_objectQualifier != string.Empty & _objectQualifier.EndsWith("_") == false) {
		_objectQualifier += "_";
	}

	_databaseOwner = objProvider.Attributes["databaseOwner"];
	if (_databaseOwner != string.Empty & _databaseOwner.EndsWith(".") == false) {
		_databaseOwner += ".";
	}

	_fullModuleQualifier = _databaseOwner + _objectQualifier + _moduleDataPrefix;

}

#region "Properties"

/// <summary>
/// The connection string used by our data provider.
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
public string ConnectionString {
	get { return _connectionString; }
}

/// <summary>
/// The path of our data provider.
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
public string ProviderPath {
	get { return _providerPath; }
}

/// <summary>
/// The object qualifier used for the provider (ie. dnn_).
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
public string ObjectQualifier {
	get { return _objectQualifier; }
}

/// <summary>
/// The database owner used for the provider (ie. dbo). 
/// </summary>
/// <value></value>
/// <returns></returns>
/// <remarks></remarks>
public string DatabaseOwner {
	get { return _databaseOwner; }
}

#endregion

        #region Shared/Static Methods

        private static DataProvider provider;

        // return the provider
        //public static DataProvider Instance()
        //{
        //    if (provider == null)
        //    {
        //        const string assembly = "DotNetNuke.Modules.uDebate.Data.SqlDataprovider,uDebate";
        //        Type objectType = Type.GetType(assembly, true, true);

        //        provider = (DataProvider)Activator.CreateInstance(objectType);
        //        DataCache.SetCache(objectType.FullName, provider);
        //    }
           
        //    Provider objProvider = ((Provider)_providerConfiguration.Providers[_providerConfiguration.DefaultProvider]);
        //    string _connectionString;
        //    if (!String.IsNullOrEmpty(objProvider.Attributes["connectionStringName"]) && !String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[objProvider.Attributes["connectionStringName"]]))
        //    {
        //        _connectionString = System.Configuration.ConfigurationManager.AppSettings[objProvider.Attributes["connectionStringName"]];
        //    }
        //    else
        //    {
        //        _connectionString = objProvider.Attributes["connectionString"];
        //    }

        //    _providerPath = objProvider.Attributes["providerPath"];

        //    _objectQualifier = objProvider.Attributes["objectQualifier"];
        //    if (_objectQualifier != string.Empty & _objectQualifier.EndsWith("_") == false)
        //    {
        //        _objectQualifier += "_";
        //    }

        //    _databaseOwner = objProvider.Attributes["databaseOwner"];
        //    if (_databaseOwner != string.Empty & _databaseOwner.EndsWith(".") == false)
        //    {
        //        _databaseOwner += ".";
        //    }

        //    _fullModuleQualifier = _databaseOwner + _objectQualifier + _moduleDataPrefix;

        //    return provider;
        //}

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Not returning class state information")]
        public static IDbConnection GetConnection()
        {
            const string providerType = "data";
            ProviderConfiguration _providerConfiguration = ProviderConfiguration.GetProviderConfiguration(providerType);

            Provider objProvider = ((Provider)_providerConfiguration.Providers[_providerConfiguration.DefaultProvider]);
            string _connectionString;
            if (!String.IsNullOrEmpty(objProvider.Attributes["connectionStringName"]) && !String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[objProvider.Attributes["connectionStringName"]]))
            {
                _connectionString = System.Configuration.ConfigurationManager.AppSettings[objProvider.Attributes["connectionStringName"]];
            }
            else
            {
                _connectionString = objProvider.Attributes["connectionString"];
            }

            IDbConnection newConnection = new System.Data.SqlClient.SqlConnection();
            newConnection.ConnectionString = _connectionString.ToString();
            newConnection.Open();
            return newConnection;
        }

        #endregion

        #region Abstract methods

        //public abstract IDataReader GetItems(int userId, int portalId);

        //public abstract IDataReader GetItem(int itemId);       

      
        #endregion



        #region "Attachments"

        public  IDataReader Attachment_GetAllByPostID(int PostID)
        {
            return (IDataReader)SqlHelper.ExecuteReader(_connectionString, "uDebate_Attachment_GetAllByPostID", PostID);
        }

        public  IDataReader Attachment_GetAllByUserID(int UserID)
        {
            return (IDataReader)SqlHelper.ExecuteReader(_connectionString, "uDebate_Attachment_GetAllByUserID", UserID);
        }

        public  void Attachment_Update(AttachmentInfo objAttachment)
        {
            SqlHelper.ExecuteNonQuery(_connectionString,  "uDebate_Attachment_Update", objAttachment.AttachmentID, objAttachment.FileID, objAttachment.PostID, objAttachment.UserID, objAttachment.LocalFileName, objAttachment.Inline);
        }

        #endregion


    }

}