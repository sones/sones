/* <id name="GraphDB – DBPluginManager" />
 * <copyright file="DBPluginManager.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>This class search all .dll and .exe for specific baseclass derived classes and fill the dictionaries .</summary>
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.ImportExport;
using sones.GraphDB.Indices;
using sones.GraphDB.Aggregates;
using sones.GraphDB.Functions;
using sones.GraphDB.Structures.Operators;
using sones.GraphDB.Settings;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.TypeManagement;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Objects;
using sones.Lib.ErrorHandling;
using sones.Lib;
using sones.Lib.VersionedPluginManager;

namespace sones.GraphDB.Plugin
{
    public class DBPluginManager
    {
        //NLOG: temporarily commented
        //private static Logger Logger = LogManager.GetCurrentClassLogger();

        #region Data

        PluginManager _PluginManager;

        #endregion

        #region Properties

        private Dictionary<String, ABaseFunction> _Functions;
        public Dictionary<String, ABaseFunction> Functions
        {
            get { return _Functions; }
        }

        private Dictionary<String, ABaseAggregate> _Aggregates;
        public Dictionary<String, ABaseAggregate> Aggregates
        {
            get { return _Aggregates; }
        }

        private Dictionary<String, ABinaryOperator> _Operators;
        public Dictionary<String, ABinaryOperator> Operators
        {
            get { return _Operators; }
        }

        private Dictionary<string, ADBSettingsBase> _Settings;
        public Dictionary<string, ADBSettingsBase> Settings
        {
            get { return _Settings; }
        }

        private Dictionary<string, IEdgeType> _EdgeTypes;
        public Dictionary<string, IEdgeType> EdgeTypes
        {
            get { return _EdgeTypes; }
        }

        private EntityUUID _UserID;
        public EntityUUID UserID
        {
            get { return _UserID; }
            set { _UserID = value; }
        }

        private Dictionary<string, AAttributeIndex> _Indices;
        public Dictionary<string, AAttributeIndex> Indices
        {
            get { return _Indices; }
            set { _Indices = value; }
        }

        private Dictionary<string, AGraphDBImport> _GraphDBImporter;
        public Dictionary<string, AGraphDBImport> GraphDBImporter
        {
            get { return _GraphDBImporter; }
            set { _GraphDBImporter = value; }
        }

        private Dictionary<string, AGraphDBExport> _GraphDBExporter;
        public Dictionary<string, AGraphDBExport> GraphDBExporter
        {
            get { return _GraphDBExporter; }
            set { _GraphDBExporter = value; }
        }

        #endregion

        public DBPluginManager(EntityUUID myUserID, Boolean myIncludePrivateClasses = false)
        {

            #region lookup dictionaries

            _Functions = new Dictionary<string, ABaseFunction>();
            _Aggregates = new Dictionary<string, ABaseAggregate>();
            _Operators = new Dictionary<string, ABinaryOperator>();
            _Settings = new Dictionary<string, ADBSettingsBase>();
            _EdgeTypes = new Dictionary<string, IEdgeType>();
            _Indices = new Dictionary<string, AAttributeIndex>();
            _GraphDBImporter = new Dictionary<string, AGraphDBImport>();
            _GraphDBExporter = new Dictionary<string, AGraphDBExport>();
            _UserID = myUserID;

            #endregion

            #region Register & Discover

            //FindAndFillReflections(myIncludePrivateClasses);

            // Change the version if there are ANY changes which will prevent loading the plugin.
            // As long as there are still some plugins which does not have their own assembly you need to change the compatibility of ALL plugins of the GraphDB and GraphFSInterface assembly.
            // So, if any plugin in the GraphDB changes you need to change the AssemblyVersion of the GraphDB AND modify the compatibility version of the other plugins.
            _PluginManager = new PluginManager()
                .Register<IGraphDBFunction>(IGraphDBFunctionVersionCompatibility.MinVersion, IGraphDBFunctionVersionCompatibility.MaxVersion)
                .Register<IGraphDBAggregate>(IGraphDBAggregateVersionCompatibility.MinVersion, IGraphDBAggregateVersionCompatibility.MaxVersion)
                .Register<IGraphDBSetting>(IGraphDBSettingVersionCompatibility.MinVersion, IGraphDBSettingVersionCompatibility.MaxVersion)
                .Register<ABinaryOperator>(new Version("1.0.0.0"))  // GraphDB assembly
                .Register<IEdgeType>(new Version("1.0.0.0"))        // GraphDB assembly
                .Register<AGraphDBImport>(new Version("1.0.0.0"))   // GraphDB assembly
                .Register<AGraphDBExport>(new Version("1.0.0.0"))   // GraphDB assembly

                .Register<AAttributeIndex>(new Version("1.0.0.0")) // GraphDB assembly
                ;

            _PluginManager.Discover(true, !myIncludePrivateClasses)
                .FailedAction(e => { throw new Exception(e.GetIErrorsAsString()); });

            #endregion

            #region Get all plugins and fill the lookup dictionaries

            #region Functions

            foreach (var func in _PluginManager.GetPlugins<IGraphDBFunction>())
            {
                var funcname = (func as ABaseFunction).FunctionName.ToUpper();

                #region Verify that there is no aggregate with the same name if the current function has parameters

                if (_Aggregates.ContainsKey(funcname) && (func as ABaseFunction).GetParameters().IsNotNullOrEmpty())
                {
                    throw new GraphDBException(new Error_DuplicateAggregateOrFunction(funcname, false));
                }

                #endregion

                #region Add function if the name does not exist

                if (!_Functions.ContainsKey(funcname))
                {
                    _Functions.Add(funcname, (func as ABaseFunction));
                }
                else
                {
                    throw new GraphDBException(new Error_DuplicateAggregateOrFunction(funcname));
                }

                #endregion

            }

            #endregion

            #region Aggregates

            foreach (var aggr in _PluginManager.GetPlugins<IGraphDBAggregate>())
            {
                var aggrname = (aggr as ABaseAggregate).FunctionName.ToUpper();

                #region Verify that there is no function with parameters and the same name

                if (_Functions.ContainsKey(aggrname) && _Functions[aggrname].GetParameters().IsNotNullOrEmpty())
                {
                    throw new GraphDBException(new Error_DuplicateAggregateOrFunction(aggrname));
                }

                #endregion

                #region Add aggregate if it does not exist

                if (!_Aggregates.ContainsKey(aggrname))
                {
                    _Aggregates.Add(aggrname, (aggr as ABaseAggregate));
                }
                else
                {
                    throw new GraphDBException(new Error_DuplicateAggregateOrFunction(aggrname, false));
                }

                #endregion

            }

            #endregion

            #region BinaryOperators

            foreach (var plugin in _PluginManager.GetPlugins<ABinaryOperator>())
            {
                foreach (String sym in plugin.Symbol)
                {
                    if (!_Operators.ContainsKey(sym))
                    {
                        // not in the operator list
                        _Operators.Add(sym, plugin);
                    }
                }
            }

            #endregion

            #region Settings

            foreach (var plugin in _PluginManager.GetPlugins<IGraphDBSetting>())
            {
                var newSetting = plugin as ADBSettingsBase;
                if (!_Settings.ContainsKey(newSetting.Name))
                {
                    _Settings.Add(newSetting.Name, newSetting);
                }
            }

            #endregion

            #region EdgeType

            foreach (var plugin in _PluginManager.GetPlugins<IEdgeType>())
            {
                if (!_EdgeTypes.ContainsKey(plugin.EdgeTypeName.ToLower()))
                {
                    _EdgeTypes.Add(plugin.EdgeTypeName.ToLower(), plugin);
                }
            }

            #endregion

            #region AGraphDBImport

            foreach (var plugin in _PluginManager.GetPlugins<AGraphDBImport>())
            {
                if (!_GraphDBImporter.ContainsKey(plugin.ImportFormat.ToUpper()))
                {
                    _GraphDBImporter.Add(plugin.ImportFormat.ToUpper(), plugin);
                }
            }

            #endregion

            #region AGraphDBExport

            foreach (var plugin in _PluginManager.GetPlugins<AGraphDBExport>())
            {
                if (!_GraphDBExporter.ContainsKey(plugin.ExportFormat.ToUpper()))
                {
                    _GraphDBExporter.Add(plugin.ExportFormat.ToUpper(), plugin);
                }
            }

            #endregion

            #region Index

            //foreach (var plugin in _PluginManager.GetPlugins<IVersionedIndexObject<IndexKey, ObjectUUID>>())
            foreach (var plugin in _PluginManager.GetPlugins<AAttributeIndex>())
            {
                
                //var idx = (IVersionedIndexObject<IndexKey, ObjectUUID>)Activator.CreateInstance(type.MakeGenericType(typeof(IndexKey), typeof(ObjectUUID)));
                var idx = plugin;
                if (!_Indices.ContainsKey(idx.IndexType.ToUpper()))
                {
                    _Indices.Add(idx.IndexType.ToUpper(), idx);
                }
            }

            #endregion

            #endregion
            
        }

        #region public methods

        #region BinaryOperators

        #region GetBinaryOperator(Symbol)

        /// <summary>
        /// Returns a binary operator corresponding to a certain symbol.
        /// </summary>
        /// <param name="Symbol">The symbol of the operator.</param>
        /// <returns>A binary operator object.</returns>
        public ABinaryOperator GetBinaryOperator(String Symbol)
        {

            if (!Operators.ContainsKey(Symbol))
            {
                return null;
            }

            return Operators[Symbol];

        }

        #endregion

        #region GetBinaryOperator(Label)

        /// <summary>
        /// Returns a binary operator corresponding to a certain label.
        /// </summary>
        /// <param name="Label">The label of the operator.</param>
        /// <returns>A binary operator object.</returns>
        public ABinaryOperator GetBinaryOperator(BinaryOperator Label)
        {
            ABinaryOperator baseOperator;

            try
            {
                baseOperator = Operators.Single(item => item.Value.Label == Label).Value;

                if (baseOperator == null)
                    throw new GraphDBException(new Error_OperatorDoesNotExist(Label.ToString()));
            }
            catch (Exception e)
            {
                throw new GraphDBException(new Error_UnknownDBError(e));
            }

            return baseOperator;
        }

        #endregion

        #endregion

        #region Aggregates

        #region GetAggregate(myAggregateName)

        /// <summary>
        /// Returns a binary operator corresponding to a certain symbol.
        /// </summary>
        /// <param name="Symbol">The symbol of the operator.</param>
        /// <returns>A binary operator object.</returns>
        public ABaseAggregate GetAggregate(String myAggregateName)
        {

            //return _PluginManager.GetPlugins<IGraphDBAggregate>(a => (a as ABaseAggregate).FunctionName == myAggregateName).FirstOrDefault() as ABaseAggregate;

            if (!Aggregates.ContainsKey(myAggregateName))
            {
                throw new GraphDBException(new Error_AggregateOrFunctionDoesNotExist(myAggregateName));
            }

            return Activator.CreateInstance(Aggregates[myAggregateName].GetType()) as ABaseAggregate;

        }

        #endregion

        #region GetAggregates

        /// <summary>
        /// returns all aggregates
        /// </summary>
        /// <returns>a dictionary with aggregates</returns>
        public Dictionary<string, ABaseAggregate> GetAllAggregates()
        {
            return Aggregates;
        }

        #endregion

        #region HasAggregate(myAggregateName)

        /// <summary>
        /// Returns a binary operator corresponding to a certain symbol.
        /// </summary>
        /// <param name="Symbol">The symbol of the operator.</param>
        /// <returns>A binary operator object.</returns>
        public Boolean HasAggregate(String myAggregateName)
        {

            //return _PluginManager.HasPlugins<IGraphDBAggregate>(a => (a as ABaseAggregate).FunctionName == myAggregateName);

            return Aggregates.ContainsKey(myAggregateName);

        }

        #endregion

        #endregion

        #region Functions

        #region GetFunction(myAggregateName)

        /// <summary>
        /// Returns a binary operator corresponding to a certain symbol.
        /// </summary>
        /// <param name="Symbol">The symbol of the operator.</param>
        /// <returns>A binary operator object.</returns>
        public ABaseFunction GetFunction(String myFuncName)
        {

            if (!Functions.ContainsKey(myFuncName))
            {
                throw new GraphDBException(new Error_AggregateOrFunctionDoesNotExist(myFuncName));
            }

            return Activator.CreateInstance(Functions[myFuncName].GetType()) as ABaseFunction;

        }

        #endregion

        #region GetFunctions

        /// <summary>
        /// Returns a hashset of all functions.
        /// </summary>
        /// <returns>A hashset of functions.</returns>
        public Dictionary<string, ABaseFunction> GetAllFunctions()
        {
            return Functions;
        }

        #endregion

        #region HasFunction(myFuncName)

        /// <summary>
        /// Returns a binary operator corresponding to a certain symbol.
        /// </summary>
        /// <param name="Symbol">The symbol of the operator.</param>
        /// <returns>A binary operator object.</returns>
        public Boolean HasFunction(String myFuncName)
        {

            return Functions.ContainsKey(myFuncName);

        }

        #endregion

        #endregion

        #region EdgeTypes

        #region GetEdgeType(myEdgeTypeName)

        /// <summary>
        /// Returns a new EdgeType instance corresponding to a certain name.
        /// </summary>
        /// <param name="Symbol">The EdgeType name.</param>
        /// <returns>A EdgeType instance object.</returns>
        public IEdgeType GetEdgeType(String myEdgeTypeName)
        {

            if (!EdgeTypes.ContainsKey(myEdgeTypeName.ToLower()))
            {
                throw new GraphDBException(new Error_EdgeTypeDoesNotExist(myEdgeTypeName));
            }

            return EdgeTypes[myEdgeTypeName.ToLower()].GetNewInstance();

        }

        #endregion

        #region GetEdgeType(myEdgeTypeUUID)

        /// <summary>
        /// Returns a EdgeType instance corresponding to a certain name.
        /// </summary>
        /// <param name="Symbol">The EdgeType name.</param>
        /// <returns>A EdgeType instance object.</returns>
        public IEdgeType GetEdgeType(EdgeTypeUUID myEdgeTypeUUID)
        {

            return EdgeTypes.FirstOrDefault(e => e.Value.EdgeTypeUUID == myEdgeTypeUUID).Value;

        }

        #endregion

        #region GetEdgeTypes

        /// <summary>
        /// returns a list of all edge types
        /// </summary>
        /// <returns>dictionary with edge types</returns>
        public Dictionary<string, IEdgeType> GetAllEdgeTypes()
        {
            return EdgeTypes;
        }

        #endregion

        #region HasEdgeType(myEdgeTypeName)

        /// <summary>
        /// Returns true if a EdgeType with this name exists
        /// </summary>
        /// <param name="Symbol">The EdgeType name.</param>
        /// <returns>A EdgeType instance object.</returns>
        public Boolean HasEdgeType(String myEdgeTypeName)
        {

            return EdgeTypes.ContainsKey(myEdgeTypeName.ToLower());

        }

        #endregion

        #endregion

        #region Indices

        #region HasIndex(indexName)

        /// <summary>
        /// Returns true, if there was a index with the specified name <paramref name="indexName"/>
        /// </summary>
        /// <param name="indexName">The index name.</param>
        /// <returns>True if the index exist.</returns>
        public Boolean HasIndex(String indexName)
        {

            if (String.IsNullOrEmpty(indexName))
            {
                return false;
            }

            return _Indices.ContainsKey(indexName.ToUpper());

        }

        #endregion

        #region GetIndex(indexName)

        /// <summary>
        /// Returns the index with the specified name <paramref name="indexTypeName"/>
        /// </summary>
        /// <param name="indexTypeName">The index name.</param>
        /// <returns>A new instance of the index object.</returns>
        public Exceptional<AAttributeIndex> GetIndex(String indexTypeName)
        {

            var indexName = indexTypeName.ToUpper();

            if (!_Indices.ContainsKey(indexName))
            {
                return new Exceptional<AAttributeIndex>(new Error_IndexDoesNotExist(indexTypeName, ""));
            }

            return new Exceptional<AAttributeIndex>(_Indices[indexName].GetNewInstance());

        }

        #endregion

        #region GetIndices

        /// <summary>
        /// Returns a hashset of all functions.
        /// </summary>
        /// <returns>A hashset of functions.</returns>
        public Dictionary<string, AAttributeIndex> GetAllIndices()
        {
            return _Indices;
        }

        #endregion

        #endregion

        #region Importer

        public bool HasGraphDBImporter(string importFormat)
        {

            return _GraphDBImporter.ContainsKey(importFormat.ToUpper());

        }

        public AGraphDBImport GetGraphDBImporter(string importFormat)
        {

            return _GraphDBImporter[importFormat.ToUpper()];
        
        }

        #endregion

        #region Exporter

        public bool HasGraphDBExporter(string myExportFormat)
        {

            return _GraphDBExporter.ContainsKey(myExportFormat.ToUpper());

        }

        public AGraphDBExport GetGraphDBExporter(string myExportFormat)
        {

            return Activator.CreateInstance(_GraphDBExporter[myExportFormat.ToUpper()].GetType()) as AGraphDBExport;

        }

        #endregion

        #endregion

    }
}
