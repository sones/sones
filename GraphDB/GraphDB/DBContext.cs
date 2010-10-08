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
using sones.GraphDB.Exceptions;
using sones.GraphDB.Indices;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Plugin;
using sones.GraphDB.Session;
using sones.GraphDB.Settings;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Context;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Session;
using sones.Lib.ErrorHandling;
using sones.Lib.Settings;
using sones.Lib.Settings;

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

    public class DBContext : IDBContext
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

        public GraphAppSettings GraphAppSettings
        {
            get;
            private set;
        }

        #endregion

        #endregion

        #region Constructor

        public DBContext(GraphAppSettings myGraphAppSettings)
        {
            GraphAppSettings = myGraphAppSettings;
        }

        /// <summary>
        /// This will create a new dbContext, based on <paramref name="dbContext"/> reusing all shared data
        /// </summary>
        /// <param name="dbContext"></param>
        public DBContext(GraphAppSettings myGraphAppSettings, DBContext dbContext)
            :this(myGraphAppSettings)
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
        /// <param name="myIGraphDBSession">The filesystem where the information is stored.</param>
        /// <param name="DatabaseRootPath">The database root path.</param>
        public DBContext(GraphAppSettings myGraphAppSettings, IGraphFSSession graphFSSession, ObjectLocation myDatabaseRootPath, EntityUUID myUserID, Dictionary<String, ADBSettingsBase> myDBSettings, Boolean myRebuildIndices, DBPluginManager myDBPluginManager, DBSessionSettings sessionSettings = null)
            : this(myGraphAppSettings)
        {

            _DBPluginManager    = myDBPluginManager;

            _DBTypeManager      = new TypeManagement.DBTypeManager(graphFSSession, myDatabaseRootPath, myUserID, myDBSettings, this);
            _DBSettingsManager  = new DBSettingsManager(_DBPluginManager.Settings, myDBSettings, graphFSSession, new ObjectLocation(myDatabaseRootPath.Name, DBConstants.DBSettingsLocation));
            _DBObjectManager    = new DBObjectManager(this, graphFSSession);
            _DBIndexManager     = new DBIndexManager(graphFSSession, this);
            _SessionSettings    = sessionSettings;
            _DBObjectCache      = _DBObjectManager.GetSimpleDBObjectCache(this);
            _IGraphFSSession    = graphFSSession;

            //init types
            var initExcept = _DBTypeManager.Init(graphFSSession, myDatabaseRootPath, myRebuildIndices);

            if (initExcept.Failed())
            {
                throw new GraphDBException(initExcept.IErrors);
            }

        }

        #endregion
        
        public IDBContext CopyMe()
        {
            return new DBContext(GraphAppSettings, this);
        }


        public void CopyTo(IDBContext myDBContext)
        {
            var realContext = (DBContext)myDBContext;
            realContext._DBIndexManager = _DBIndexManager;
            realContext._DBObjectCache = _DBObjectCache;
            realContext._DBObjectManager = _DBObjectManager;
            realContext._DBPluginManager = _DBPluginManager;
            realContext._DBSettingsManager = _DBSettingsManager;
            realContext._DBTypeManager = _DBTypeManager;
            realContext._SessionSettings = _SessionSettings;
        }
    }

}
