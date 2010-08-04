/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
*
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
*/

/* <id name="GraphDB – Instantce Settings" />
 * <copyright file="DBInstanceSettingsManager.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Daniel Kirstenpfad</developer>
 * <summary>A class which serializes and deserializes simple settings data structures</summary>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Objects;
using sones.GraphFS.Session;
using sones.Lib.DataStructures;
using sones.Lib.ErrorHandling;
using sones.Lib.Serializer;
using sones.Lib.DataStructures.Indices;

namespace sones.GraphDB.Settings
{

    /// <summary>
    /// this class holds the setting informations of a PandoraDatabase Instance
    /// </summary>
    public class DBInstanceSettingsManager<T> where T: new ()
    {

        //private String          _SettingsObjectName; // = ".database.instancesettings";
        private IGraphFSSession _IGraphFSSession;
        private ObjectLocation  _DatabaseRootPath;

        public T Content;

        //Func<KeyValuePair<String, List<Object>>, Int32, KeyValuePair<String, Object>> myFunc = 

        private KeyValuePair<String, Object> myFunc(KeyValuePair<String, List<Object>> sourceElem, Int32 index)
        {
            return new KeyValuePair<String, Object>(sourceElem.Key, sourceElem.Value);
        }


        #region Constructor

        #region SettingsManager(myDatabaseRootPath, mySettingsObjectName, myIPandoraFS, CreateIt)

        /// <summary>
        /// Constructor of the Settings Manager
        /// </summary>
        /// <param name="DatabaseRootPath">the root Path of the Database</param>
        /// <param name="SettingsObjectName">the name of the settings object</param>
        /// <param name="PandoraVFSInstance">the VFS Instance that handles this Database</param>
        /// <param name="CreateIt">should the Instance Settings Metadata be created if it does not exist</param>
        public DBInstanceSettingsManager(ObjectLocation myDatabaseRootPath, IGraphFSSession myIGraphFS, Boolean CreateIt)
        {

            #region check if no null values has been handed over

            if (myIGraphFS == null)
                throw new GraphDBException(new Error_ArgumentNullOrEmpty("myIPandoraFS"));
            else
                _IGraphFSSession = myIGraphFS;

            _DatabaseRootPath = myDatabaseRootPath;

            if (_DatabaseRootPath == "")
                throw new GraphDBException(new Error_ArgumentNullOrEmpty("DatabaseRootPath"));

            //if (mySettingsObjectName == "")
            //    throw new GraphDBException(new Error_ArgumentNullOrEmpty("mySettingsObjectName"));

            // eventually remove the last /
            while (((String)_DatabaseRootPath)[_DatabaseRootPath.Length - 1] == FSPathConstants.PathDelimiter[0])
            {
                _DatabaseRootPath = new ObjectLocation(_DatabaseRootPath.Remove(_DatabaseRootPath.Length - 1, 1));

                if (_DatabaseRootPath == "")
                    throw new GraphDBException(new Error_ArgumentNullOrEmpty("DatabaseRootPath"));
            }

            //_SettingsObjectName = mySettingsObjectName;

            #endregion

            // check if there's actually something at the Database Root Path
            if (myIGraphFS.isIDirectoryObject(_DatabaseRootPath).Value != Trinary.TRUE)
                throw new GraphDBException(new Error_DatabaseNotFound(_DatabaseRootPath));

            else
            {

                // found the database root path, now check for the UserMetadata and read it in...
                if (myIGraphFS.ObjectStreamExists(new ObjectLocation(_DatabaseRootPath), FSConstants.USERMETADATASTREAM).Value == Trinary.TRUE)
                {
                    // found it...read it
                    ReadSettings();
                }

                else
                {

                    if (CreateIt)
                    {

                        #region create an empty Database Instance Settings Data structure with default settings and store it on the PandoraFS

                        Content = new T();
                        var Serializer = new KeyValuePairSerializer<T>();
                        var SerializedSettings = Serializer.Serialize(Content);

                        ////HACK: Do not use this!
                        //foreach (var Setting in SerializedSettings)
                        //{
                        //    myIPandoraFS.SetUserMetadatum(new ObjectLocation(_DatabaseRootPath, _SettingsObjectName), Setting.Key, Setting.Value, IndexSetStrategy.REPLACE);
                        //}

                        myIGraphFS.SetUserMetadata(new ObjectLocation(_DatabaseRootPath), SerializedSettings, IndexSetStrategy.REPLACE);

                        #endregion

                    }

                    else
                        throw new GraphDBException(new Error_DatabaseNotFound(_DatabaseRootPath));
                
                }

            }

        }

        #endregion

        #endregion


        #region ReadSettings()

        /// <summary>
        /// Reads the settings from disc
        /// </summary>
        public Exceptional ReadSettings()
        {

            var _Exceptional = new Exceptional();
            var _UserMetadataExceptional = _IGraphFSSession.GetUserMetadata(new ObjectLocation(_DatabaseRootPath));

            if (_UserMetadataExceptional != null && _UserMetadataExceptional.Success && _UserMetadataExceptional.Value != null)
            {

                var _KeyValuePairSerializer = new KeyValuePairSerializer<T>();
                
                // Deserialize the settings
                Content = (T) _KeyValuePairSerializer.DeSerialize(_UserMetadataExceptional.Value.ToDictionary(item => item.Key, item => item.Value));

            }

            else
            {
                _Exceptional = _UserMetadataExceptional.Push(new GraphDBError_SettingsNotFound(new ObjectLocation(_DatabaseRootPath)));
            }

            return _Exceptional;

        }

        #endregion

        #region WriteSettings()

        /// <summary>
        /// Writes the settings to disc while
        /// </summary>
        public Exceptional WriteSettings()
        {

            var _Exceptional             = new Exceptional();
            var _KeyValuePairSerializer  = new KeyValuePairSerializer<T>();
            var _UserMetadataExceptional = _IGraphFSSession.SetUserMetadata(new ObjectLocation(_DatabaseRootPath), _KeyValuePairSerializer.Serialize(Content), IndexSetStrategy.REPLACE);

            if (_UserMetadataExceptional.Failed)
            {
                _Exceptional = _UserMetadataExceptional.Push(new GraphDBError_CouldNotWriteSettings(new ObjectLocation(_DatabaseRootPath)));
            }

            return _Exceptional;

        }

        #endregion

        
    }
}
