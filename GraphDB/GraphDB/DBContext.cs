/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
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
* 
*/

/* <id name="DBContext" />
 * <copyright file="DBContext.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>This class holds all DB related managers and is bound to a transaction.</summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Indices;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Aggregates;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Functions;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using sones.GraphDB.QueryLanguage.Operators;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.Settings;
using sones.GraphDB.Structures;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.GraphDB.TypeManagement.SpecialTypeAttributes;
using sones.GraphDB.Warnings;
using sones.GraphFS;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Errors;
using sones.GraphFS.Exceptions;
using sones.GraphFS.Objects;
using sones.GraphFS.Session;
using sones.Lib;
using sones.Lib.DataStructures;
using sones.Lib.DataStructures.UUID;
using sones.Lib.ErrorHandling;
using sones.Lib.Serializer;
using System.Text;
using sones.GraphDB.Plugin;
using sones.Lib.Session;
using Lib;
using sones.GraphDB.Managers;
using sones.GraphDB.Transactions;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Session;


#endregion

namespace sones.GraphDB
{
    /// <summary>
    /// TODO: 
    /// - rename namespace
    /// - remove manager setters
    /// - move DBPluginManager creation to DB (and all other things which are transaction independent)
    /// - start each time with an empty Cache, TypeManager, etc to load these data based on the transaction timestamp
    /// </summary>

    public class DBContext : IContext
    {

        #region Data

        private IGraphFSSession _IGraphFSSession;

        #region Manager REMOVE SETTER AFTERWARDS

        private DBIndexManager _DBIndexManager;
        public DBIndexManager DBIndexManager
        {
            get { return _DBIndexManager; }
        }

        private DBSettingsManager _DBSettingsManager;
        public DBSettingsManager DBSettingsManager
        {
            get { return _DBSettingsManager; }
        }

        private DBPluginManager _DBPluginManager;
        public DBPluginManager DBPluginManager
        {
            get { return _DBPluginManager; }
        }

        private DBObjectManager _DBObjectManager;
        public DBObjectManager DBObjectManager
        {
            get { return _DBObjectManager; }
        }

        private DBTypeManager _DBTypeManager;
        public DBTypeManager DBTypeManager
        {
            get { return _DBTypeManager; }
        }

        private DBSessionSettings _SessionSettings;
        public DBSessionSettings SessionSettings
        {
            get { return _SessionSettings; }
            set { _SessionSettings = value; }
        }

        private DBObjectCache _DBObjectCache;
        public DBObjectCache DBObjectCache
        {
            get { return _DBObjectCache; }
            set { _DBObjectCache = value; }
        }

        #endregion

        #endregion

        #region Constructor

        public DBContext() { }

        /// <summary>
        /// This will create a new dbContext, based on <paramref name="dbContext"/> reusing all shared data
        /// </summary>
        /// <param name="dbContext"></param>
        public DBContext(DBContext dbContext)
        {

            #region Immutable objects

            _DBPluginManager = dbContext.DBPluginManager;
            _DBSettingsManager = dbContext.DBSettingsManager;
            _IGraphFSSession = dbContext._IGraphFSSession;

            #endregion

            _SessionSettings = new DBSessionSettings(dbContext.SessionSettings);
            _DBObjectManager = new ObjectManagement.DBObjectManager(this, _IGraphFSSession);

            //
            _DBIndexManager = new Indices.DBIndexManager(_IGraphFSSession, this);

            _DBTypeManager = new DBTypeManager(dbContext.DBTypeManager);

            _DBObjectCache = _DBObjectManager.GetSimpleDBObjectCache(this);

        }

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="myIPandoraDBSession">The filesystem where the information is stored.</param>
        /// <param name="DatabaseRootPath">The database root path.</param>
        public DBContext(IGraphFSSession graphFSSession, ObjectLocation myDatabaseRootPath, EntityUUID myUserID, Dictionary<String, ADBSettingsBase> myDBSettings, Boolean myRebuildIndices, DBSessionSettings sessionSettings = null)
        {

            _DBTypeManager      = new TypeManagement.DBTypeManager(graphFSSession, myDatabaseRootPath, myUserID, myDBSettings, this);

            _DBPluginManager    = new DBPluginManager(myUserID);
            _DBSettingsManager  = new DBSettingsManager(_DBPluginManager.Settings, myDBSettings, graphFSSession, new ObjectLocation(myDatabaseRootPath.Name, DBConstants.DBSettingsLocation));
            _DBObjectManager    = new DBObjectManager(this, graphFSSession);
            _DBIndexManager     = new DBIndexManager(graphFSSession, this);
            _SessionSettings    = sessionSettings;
            _DBObjectCache      = _DBObjectManager.GetSimpleDBObjectCache(this);
            _IGraphFSSession    = graphFSSession;

            //init types
            var initExcept = _DBTypeManager.Init(graphFSSession, myDatabaseRootPath, myRebuildIndices);

            if (initExcept.Failed)
            {
                throw new GraphDBException(initExcept.Errors);
            }

        }

        #endregion
        
        public void CopyTo(DBContext dbContext)
        {
            dbContext._DBIndexManager = _DBIndexManager;
            dbContext._DBObjectCache = _DBObjectCache;
            dbContext._DBObjectManager = _DBObjectManager;
            dbContext._DBPluginManager = _DBPluginManager;
            dbContext._DBSettingsManager = _DBSettingsManager;
            dbContext._DBTypeManager = _DBTypeManager;
            dbContext._SessionSettings = _SessionSettings;
        }
    }

}
