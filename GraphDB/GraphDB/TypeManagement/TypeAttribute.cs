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

/* <id name="sones GraphDB – TypeAttribute" />
 * <copyright file="TypeAttribute.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>This class holds all information about an specific attribute of a DB type.</summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.Settings;
using sones.GraphDB.Structures;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.Lib;
using sones.Lib.ErrorHandling;
using sones.Lib.NewFastSerializer;
using sones.Lib.Serializer;
using sones.Lib.Session;
using sones.GraphDB.QueryLanguage.Enums;

#endregion

namespace sones.GraphDB.TypeManagement
{

    /// <summary>
    /// The kind of a type (Single, List)
    /// </summary>
    
    public enum KindsOfType
    {
        SingleReference,
        ListOfNoneReferences,
        SetOfNoneReferences,
        SetOfReferences,
        //SettingAttribute,
        SpecialAttribute
    }

    /// <summary>
    /// This class holds all information about an specific attribute of a DB type.
    /// </summary>
    
    public class TypeAttribute : IFastSerialize
    {

        //NLOG: temporarily commented
        //private static Logger Logger = LogManager.GetCurrentClassLogger();

        #region constructor

        public TypeAttribute()
        {
            _TypeCharacteristics = new TypeCharacteristics();
            _UUID = new AttributeUUID();
            _Settings = new Dictionary<string, ADBSettingsBase>();
        }
    
        
        public TypeAttribute(AttributeUUID myAttributeUUID)
        {
            _TypeCharacteristics = new TypeCharacteristics();
            _UUID = myAttributeUUID;
            _Settings = new Dictionary<string, ADBSettingsBase>();
            _RelatedPandoraTypeUUID = null;
        }

        public TypeAttribute(Byte[] mySerializedData) : this()
        {
            if (mySerializedData == null || mySerializedData.Length == 0)
                throw new ArgumentNullException("mySerializedData must not be null or its length be zero!");

            //Deserialize(mySerializedData);
            throw new NotImplementedException();
        }

        #endregion

        #region Properties

        #region Settings

        private Dictionary<string, ADBSettingsBase> _Settings;

        public Dictionary<string, ADBSettingsBase> GetObjectSettings
        {
            get { return _Settings; }
        }

        #endregion

        /// <summary>
        /// The PandoraType
        /// </summary>
        public AttributeUUID UUID
        {
            get { return _UUID; }
            set { _UUID = value; }
        }
        protected AttributeUUID _UUID;

        /// <summary>
        /// The PandoraTypeUUID
        /// </summary>
        public TypeUUID DBTypeUUID
        {
            get { return _DBTypeUUID; }
            set { _DBTypeUUID = value; }
        }
        private TypeUUID _DBTypeUUID;

        private GraphDBType _dbType = null;

        /// <summary>
        /// The kind of type (single, list)
        /// </summary>
        public KindsOfType KindOfType
        {
            get { return _KindOfType; }
            set { _KindOfType = value; }
        }
        private KindsOfType _KindOfType;
        
        /// <summary>
        /// The characteristic (queue, unique, weighted) of the List-Typed Attribute
        /// </summary>
        public TypeCharacteristics TypeCharacteristics
        {
            get { return _TypeCharacteristics; }
            set { _TypeCharacteristics = value; }
        }
        private TypeCharacteristics _TypeCharacteristics;

        /// <summary>
        /// The Name of the Attribute
        /// </summary>
        public String Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        protected String _Name;
                
        public TypeUUID RelatedPandoraTypeUUID
        {
            get
            {
                return _RelatedPandoraTypeUUID;
            }

            set
            {
                _RelatedPandoraTypeUUID = value;
            }
        }
        private TypeUUID _RelatedPandoraTypeUUID;

        private GraphDBType _RelatedPandoraType = null;

        /// <summary>
        /// If this attribute is an BackwardEdge attribute, than this hold the information about the Edge
        /// </summary>
        public EdgeKey BackwardEdgeDefinition
        {
            get { return _BackwardEdgeDefinition; }
            set { _BackwardEdgeDefinition = value; }
        }
        private EdgeKey _BackwardEdgeDefinition;
        
        
        public Boolean IsBackwardEdge
        {
            get
            {
                return TypeCharacteristics.IsBackwardEdge;
            }
        }

        private AEdgeType _EdgeType;
        public AEdgeType EdgeType
        {
            get
            {
                return _EdgeType;
            }
            set { _EdgeType = value; }
        }

        private AObject _DefaultValue;
        public AObject DefaultValue
        {
            get 
            {
                return _DefaultValue;
            }
            set { _DefaultValue = value; }
        }

        #endregion

        #region IFastSerialize Members

        public bool isDirty
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public DateTime ModificationTime
        {
            get { throw new NotImplementedException(); }
        }

        public void Serialize(ref SerializationWriter mySerializationWriter)
        {
            #region Write

            if (mySerializationWriter != null)
            {
                _UUID.Serialize(ref mySerializationWriter);
                _DBTypeUUID.Serialize(ref mySerializationWriter);
                mySerializationWriter.WriteObject((Byte)_KindOfType);
                _TypeCharacteristics.Serialize(ref mySerializationWriter);
                
                mySerializationWriter.WriteObject(_Name);

                mySerializationWriter.WriteObject(_EdgeType != null);
                if (_EdgeType != null)
                    mySerializationWriter.WriteObject(_EdgeType);

                _RelatedPandoraTypeUUID.Serialize(ref mySerializationWriter);

                if (_TypeCharacteristics.IsBackwardEdge)
                    mySerializationWriter.WriteObject(_BackwardEdgeDefinition);

                mySerializationWriter.WriteObject(_DefaultValue);

                #region Write Settings

                mySerializationWriter.WriteObject((UInt32)_Settings.Count);
                foreach (KeyValuePair<string, ADBSettingsBase> pSetting in _Settings)
                    mySerializationWriter.WriteObject(pSetting.Value);

                #endregion

            }

            #endregion
        }

        public void Deserialize(ref SerializationReader mySerializationReader)
        {
            UInt32 _Capacity;

            #region Read

            try
            {
                if (mySerializationReader != null)
                {
                    _UUID = new AttributeUUID();
                    _UUID.Deserialize(ref mySerializationReader);
                    _DBTypeUUID = new TypeUUID();
                    _DBTypeUUID.Deserialize(ref mySerializationReader);
                    _KindOfType = (KindsOfType)(Byte)mySerializationReader.ReadObject();
                    _TypeCharacteristics = new TypeCharacteristics();
                    _TypeCharacteristics.Deserialize(ref mySerializationReader);

                    _Name = (String)mySerializationReader.ReadObject();

                    Boolean hasEdgeType = (Boolean)mySerializationReader.ReadObject();
                    if (hasEdgeType)
                    {                        
                        try
                        {
                            _EdgeType = (AEdgeType)mySerializationReader.ReadObject();
                        }
                        catch(Exception ex)
                        {
                            //NLOG: temporarily commented
                            //Logger.Fatal("Could not deserialize EdgeType");
                            throw new GraphDBException(new Error_UnknownDBError(ex));
                        }

                    }

                    _RelatedPandoraTypeUUID = new TypeUUID();
                    _RelatedPandoraTypeUUID.Deserialize(ref mySerializationReader);
                    if (_TypeCharacteristics.IsBackwardEdge)
                        _BackwardEdgeDefinition = (EdgeKey)mySerializationReader.ReadObject();

                    _DefaultValue = (AObject)mySerializationReader.ReadObject();

                    #region Read Settings

                    _Capacity = (UInt32)mySerializationReader.ReadObject();

                    _Settings = new Dictionary<string, ADBSettingsBase>();

                    for (UInt32 i = 0; i < _Capacity; i++)
                    {
                        ADBSettingsBase _SettingObject = (ADBSettingsBase)mySerializationReader.ReadObject();
                        if (_SettingObject != null)
                            _Settings.Add(_SettingObject.Name, _SettingObject);
                        
                        /*Type settingType = (Type)mySerializationReader.ReadObject();
                        ADBSettingsBase _SettingObject = null;
                        try
                        {
                            _SettingObject = (ADBSettingsBase)Activator.CreateInstance(settingType);
                        }
                        catch
                        {
                            Logger.Error("Could not create an instance of setting " + settingType.ToString());
                        }
                        Byte[] Bytes = (Byte[])mySerializationReader.ReadObject();
                        if (_SettingObject != null)
                        {
                            _SettingObject.Deserialize(new SerializationReader(Bytes));
                            _Settings.Add(_SettingObject.Name, _SettingObject);
                        }*/
                    }
                    #endregion
                }
            }
            catch (Exception e)
            {
                throw new SerializationException("The Setting could not be deserialized!\n\n" + e);
            }

            #endregion
        }

        #endregion

        #region public methods

        #region settings

        public Exceptional<bool> SetPersistentSetting(string myName, ADBSettingsBase mySetting, DBTypeManager myTypeManager)
        {
            if (!_Settings.ContainsKey(myName))
                _Settings.Add(myName, (ADBSettingsBase)mySetting.Clone());
            else
            {
                _Settings[myName] = (ADBSettingsBase)mySetting.Clone();
            }

            var FlushExcept = myTypeManager.FlushType(GetRelatedType(myTypeManager));

            if (FlushExcept.Failed)
                return new Exceptional<bool>(FlushExcept);

            return new Exceptional<bool>(true);
        }

        public Exceptional<bool> RemovePersistentSetting(string myName, DBTypeManager myTypeManager)
        {
            _Settings.Remove(myName);

            var FlushExcept = myTypeManager.FlushType(GetRelatedType(myTypeManager));

            if (FlushExcept.Failed)
                return new Exceptional<bool>(FlushExcept);

            return new Exceptional<bool>(true);
        }

        public ADBSettingsBase GetPersistentSetting(string mySettingName)
        {
            if (_Settings.ContainsKey(mySettingName))
            {
                return (ADBSettingsBase)_Settings[mySettingName].Clone();
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region DefaultValue
                
        public AObject GetDefaultValue(DBContext dbContext)
        {
            if (KindOfType == KindsOfType.SingleReference)
            {
                if (GetDBType(dbContext.DBTypeManager).IsUserDefined)
                {
                    return (AObject)_EdgeType.GetNewInstance();
                }
                else
                {
                    var val = ((ADBBaseObject)PandoraTypeMapper.GetADBBaseObjectFromUUID(DBTypeUUID));
                    val.SetValue(DBObjectInitializeType.Default);
                    return val as AObject;
                }
            }

            if (KindOfType == KindsOfType.SetOfReferences)
                return (AObject)_EdgeType.GetNewInstance();

            return null;
        }

        #endregion

        public ADBBaseObject GetADBBaseObjectType(DBTypeManager myDBTypeManager)
        {

            if (PandoraTypeMapper.IsBasicType(GetDBType(myDBTypeManager).Name))
                return PandoraTypeMapper.GetPandoraObjectFromTypeName(GetDBType(myDBTypeManager).Name);

            return null;

        }

        public GraphDBType GetDBType(DBTypeManager myDBTypeManager)
        {

            if (_dbType == null)
                _dbType = myDBTypeManager.GetTypeByUUID(_DBTypeUUID);
            
            return _dbType;

        }

        public GraphDBType GetRelatedType(DBTypeManager myDBTypeManager)
        {

            if (_RelatedPandoraType == null)
            {
                if (!IsBackwardEdge)
                    _RelatedPandoraType = myDBTypeManager.GetTypeByUUID(_RelatedPandoraTypeUUID);

                else
                    _RelatedPandoraType = myDBTypeManager.GetTypeByUUID(_BackwardEdgeDefinition.TypeUUID);
            }

            return _RelatedPandoraType;

        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            TypeAttribute objType = (TypeAttribute)obj;
            return this.UUID.CompareTo(objType.UUID);
        }

        #endregion

        #region toString

        public override string ToString()
        {
            return ToString("|");
        }

        public string ToString(String mySeperator)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(_Name);

            sb.Append(" : ");

            if (_TypeCharacteristics.IsBackwardEdge)
            {
                sb.Append("BACKWARDEDGE<");

                sb.Append(_BackwardEdgeDefinition.ToString());

                sb.Append(">");
            }
            else
            {

                if (_KindOfType == KindsOfType.ListOfNoneReferences)
                    sb.Append("LIST<");

                if (_KindOfType == KindsOfType.SetOfNoneReferences || _KindOfType == KindsOfType.SetOfReferences)
                    sb.Append("SET<");

                sb.Append(_DBTypeUUID.ToString());

                if (_KindOfType == KindsOfType.SetOfReferences || _KindOfType == KindsOfType.SetOfNoneReferences || _KindOfType == KindsOfType.ListOfNoneReferences)
                    sb.Append(">");
            }

            var tCharcts = _TypeCharacteristics.ToString();
            if (tCharcts != String.Empty)
            {
                sb.Append(mySeperator);
                sb.Append(_TypeCharacteristics.ToString());
            }

            sb.Append(mySeperator);
            sb.Append("[UUID: ");
            sb.Append(_DBTypeUUID.ToString().Firststring(3));
            sb.Append("]");

            return sb.ToString();
        }

        #endregion

        #endregion
    }
}
