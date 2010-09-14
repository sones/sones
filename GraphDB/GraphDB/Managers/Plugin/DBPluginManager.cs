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

namespace sones.GraphDB.Plugin
{
    public class DBPluginManager
    {
        //NLOG: temporarily commented
        //private static Logger Logger = LogManager.GetCurrentClassLogger();
        
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

        private Dictionary<string, IVersionedIndexObject<IndexKey, ObjectUUID>> _Indices;
        public Dictionary<string, IVersionedIndexObject<IndexKey, ObjectUUID>> Indices
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
            _Functions       = new Dictionary<string, ABaseFunction>();
            _Aggregates      = new Dictionary<string, ABaseAggregate>();
            _Operators       = new Dictionary<string, ABinaryOperator>();
            _Settings        = new Dictionary<string, ADBSettingsBase>();
            _EdgeTypes       = new Dictionary<string, IEdgeType>();
            _Indices         = new Dictionary<string, IVersionedIndexObject<IndexKey, ObjectUUID>>();
            _GraphDBImporter = new Dictionary<string, AGraphDBImport>();
            _GraphDBExporter = new Dictionary<string, AGraphDBExport>();
            _UserID          = myUserID;

            FindAndFillReflections(myIncludePrivateClasses);
        }

        #region Reflection

        private void FindAndFillReflections(Boolean myIncludePrivateClasses = false)
        {
            //NLOG: temporarily commented
            //Logger.Trace("Starting find all ABinaryOperator, ABaseAggregate, ABaseFunction");

            foreach (string fileOn in Directory.GetFiles("."))
            {

                FileInfo file = new FileInfo(fileOn);

                //Preliminary check, must be .dll
                if ((file.Extension.Equals(".dll")) || (file.Extension.Equals(".exe")))
                {

                    #region Get all types from assembly

                    System.Type[] allTypes = null;
                    try
                    {
                        allTypes = Assembly.LoadFrom(file.FullName).GetTypes();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("[DBPluginmanager Startup] Reflection on " + file.Name + " failed: " + ex.Message);
                        //NLOG: temporarily commented
                        //Logger.ErrorException("[AggregatesManager Startup] Reflection on " + file.Name + " failed ", ex);
                    }

                    if (allTypes.IsNullOrEmpty()) continue;

                    #endregion

                    foreach (System.Type type in allTypes)
                    {

                        try
                        {

                            String fullTypeName = type.Name;

                            // check for correct base
                            bool correctbase = false;

                            if (type != null)
                            {
                                if (type.BaseType == null)
                                    continue;
                                if (type.IsAbstract)
                                    continue;
                                if (!myIncludePrivateClasses && (type.IsNestedPrivate || type.IsNotPublic))
                                    continue;
                                

                                #region Check base type iterative

                                System.Type basetype = type.BaseType;

                                while (!correctbase)
                                {
                                    if (basetype == typeof(System.Object))
                                        break;

                                    if (
                                        basetype == typeof(ABaseFunction)
                                     || basetype == typeof(ABaseAggregate)
                                     || basetype == typeof(ABinaryOperator)
                                     || basetype == typeof(ADBSettingsBase)
                                     || type.GetInterface(typeof(IEdgeType).Name) != null
                                     || (basetype == typeof(AFSObject) && type.GetInterface(typeof(IVersionedIndexObject<IndexKey, ObjectUUID>).Name) != null)
                                     || basetype == typeof(AGraphDBImport)
                                     || basetype == typeof(AGraphDBExport)
                                        )
                                        correctbase = true;
                                    else
                                    {
                                        basetype = basetype.BaseType;
                                    }

                                }

                                #endregion

                                #region Check the base type, create instance and store it

                                // if this type has the correct base type
                                if (correctbase)
                                {
                                    #region ABaseFunction

                                    if (basetype == typeof(ABaseFunction))
                                    {
                                        ABaseFunction newABaseFunc = (ABaseFunction)Activator.CreateInstance(type);
                                        
                                        #region Verify that there is no aggregate with the same name if the current function has parameters

                                        if (_Aggregates.ContainsKey(newABaseFunc.FunctionName.ToUpper()) && newABaseFunc.GetParameters().IsNotNullOrEmpty())
                                        {
                                            throw new GraphDBException(new Error_DuplicateAggregateOrFunction(newABaseFunc.FunctionName.ToUpper(), false));
                                        }

                                        #endregion

                                        if (!_Functions.ContainsKey(newABaseFunc.FunctionName.ToUpper()))
                                        {
                                            // not in the operator list
                                            _Functions.Add(newABaseFunc.FunctionName.ToUpper(), newABaseFunc);
                                        }
                                        else
                                        {
                                            throw new GraphDBException(new Error_DuplicateAggregateOrFunction(newABaseFunc.FunctionName.ToUpper()));
                                            //NLOG: temporarily commented
                                            //Logger.Warn("Function with Name '" + newABaseFunc.FunctionName + "' already added!");
                                        }

                                    }

                                    #endregion

                                    #region ABaseAggregate

                                    else if (basetype == typeof(ABaseAggregate))
                                    {
                                        ABaseAggregate newABaseAggregate = (ABaseAggregate)Activator.CreateInstance(type);

                                        #region Verify that there is no function with parameters and the same name

                                        if (_Functions.ContainsKey(newABaseAggregate.FunctionName.ToUpper()) && _Functions[newABaseAggregate.FunctionName.ToUpper()].GetParameters().IsNotNullOrEmpty())
                                        {
                                            throw new GraphDBException(new Error_DuplicateAggregateOrFunction(newABaseAggregate.FunctionName.ToUpper()));
                                        }
                                        
                                        #endregion

                                        if (!_Aggregates.ContainsKey(newABaseAggregate.FunctionName))
                                        {
                                            // not in the operator list
                                            _Aggregates.Add(newABaseAggregate.FunctionName.ToUpper(), newABaseAggregate);
                                        }
                                        else
                                        {
                                            throw new GraphDBException(new Error_DuplicateAggregateOrFunction(newABaseAggregate.FunctionName.ToUpper(), false));
                                            //NLOG: temporarily commented
                                            //Logger.Warn("Aggregate with Name '" + newABaseAggregate.FunctionName + "' already added!");
                                        }
                                    }

                                    #endregion

                                    #region ABinaryOperator

                                    else if (basetype == typeof(ABinaryOperator))
                                    {
                                        ABinaryOperator newBinaryOperator = (ABinaryOperator)Activator.CreateInstance(type);

                                        foreach (String sym in newBinaryOperator.Symbol)
                                        {
                                            if (!_Operators.ContainsKey(sym))
                                            {
                                                // not in the operator list
                                                _Operators.Add(sym, newBinaryOperator);
                                            }
                                            else
                                            {
                                                //NLOG: temporarily commented
                                                //Logger.Warn("Operator with Symbol '" + sym + "' already added!");
                                            }
                                        }
                                    }

                                    #endregion

                                    #region ADBSettingsBase

                                    else if (basetype == typeof(ADBSettingsBase))
                                    {
                                        ADBSettingsBase newSetting = (ADBSettingsBase)Activator.CreateInstance(type);

                                        if (!_Settings.ContainsKey(newSetting.Name))
                                        {
                                            //if (newSetting.OwnerID == null)
                                            //    newSetting.OwnerID = UserID;
                                            _Settings.Add(newSetting.Name, newSetting);
                                        }
                                        else
                                        {
                                            //NLOG: temporarily commented
                                            //Logger.Warn("Setting with name '" + newSetting.Name + "' already added!");
                                        }
                                    }

                                    #endregion

                                    #region IEdgeType

                                    else if (type.GetInterface(typeof(IEdgeType).Name) != null)
                                    {
                                        IEdgeType newAEdgeType = (IEdgeType)Activator.CreateInstance(type);
                                        if (!_EdgeTypes.ContainsKey(newAEdgeType.EdgeTypeName))
                                        {
                                            // not in the operator list
                                            _EdgeTypes.Add(newAEdgeType.EdgeTypeName.ToLower(), newAEdgeType);
                                        }
                                        else
                                        {
                                            //NLOG: temporarily commented
                                            //Logger.Warn("EdgeType with Name '" + newAEdgeType.EdgeTypeName + "' already added!");
                                        }
                                    }

                                    #endregion

                                    #region AGraphDBImport

                                    else if (basetype == typeof(AGraphDBImport))
                                    {
                                        try
                                        {
                                            var importer = (AGraphDBImport)Activator.CreateInstance(type);
                                            if (!_GraphDBImporter.ContainsKey(importer.ImportFormat.ToUpper()))
                                            {
                                                _GraphDBImporter.Add(importer.ImportFormat.ToUpper(), importer);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Debug.WriteLine("[DBPluginmanager] Failed to create instance of type {0}: {1}", type.Name, ex.Message);
                                        }
                                    }

                                    #endregion

                                    #region AGraphDBExport

                                    else if (basetype == typeof(AGraphDBExport))
                                    {
                                        try
                                        {
                                            var exporter = (AGraphDBExport)Activator.CreateInstance(type);
                                            if (!_GraphDBExporter.ContainsKey(exporter.ExportFormat.ToUpper()))
                                            {
                                                _GraphDBExporter.Add(exporter.ExportFormat.ToUpper(), exporter);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Debug.WriteLine("[DBPluginmanager] Failed to create instance of type {0}: {1}", type.Name, ex.Message);
                                        }
                                    }

                                    #endregion

                                    #region Check the interface

                                    else if (basetype == typeof(AFSObject) && type.GetInterface(typeof(IVersionedIndexObject<IndexKey, ObjectUUID>).Name) != null)
                                    {
                                        try
                                        {
                                            var idx = (IVersionedIndexObject<IndexKey, ObjectUUID>)Activator.CreateInstance(type.MakeGenericType(typeof(IndexKey), typeof(ObjectUUID)));
                                            if (!_Indices.ContainsKey(idx.IndexName.ToUpper()))
                                            {
                                                _Indices.Add(idx.IndexName.ToUpper(), idx);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Debug.WriteLine("[DBPluginmanager] Failed to create instance of type {0}: {1}", type.Name, ex.Message);
                                        }
                                    }

                                    #endregion
                                }

                                #endregion

                            }

                        }
                        catch (GraphDBException gex)
                        {
                            throw gex; // do not catch (and hide) GraphDBExceptions
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("[DBPluginmanager Startup] Reflection on " + file.Name + " and type [" + type + "] failed: " + ex.Message);
                            //NLOG: temporarily commented
                            //Logger.ErrorException("[AggregatesManager Startup] Reflection on " + file.Name + " failed ", ex);
                        }

                    }


                }
            }
            //NLOG: temporarily commented
            //Logger.Trace("Finished find all ABinaryOperator, ABaseAggregate, ABaseFunction");

        }
        #endregion

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
        public Exceptional<IVersionedIndexObject<IndexKey, ObjectUUID>> GetIndex(String indexTypeName)
        {

            var indexName = indexTypeName.ToUpper();

            if (!_Indices.ContainsKey(indexName))
            {
                return new Exceptional<IVersionedIndexObject<IndexKey, ObjectUUID>>(new Error_IndexDoesNotExist(indexTypeName, ""));
            }

            return new Exceptional<IVersionedIndexObject<IndexKey, ObjectUUID>>(_Indices[indexName].GetNewInstance2());

        }

        #endregion

        #region GetIndices

        /// <summary>
        /// Returns a hashset of all functions.
        /// </summary>
        /// <returns>A hashset of functions.</returns>
        public Dictionary<string, IVersionedIndexObject<IndexKey, ObjectUUID>> GetAllIndices()
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
