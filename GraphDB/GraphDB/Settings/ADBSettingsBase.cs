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

/* <id name="sones GraphDB – Settings" />
 * <copyright file="ADBSettingsBase.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Dirk Bludau</developer>
 * <summary></summary>
 */

#region Usings

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization;

using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.PandoraTypes;

using sones.Lib.Serializer;
using sones.Lib.Settings;
using sones.Lib.NewFastSerializer;
using sones.GraphFS.DataStructures;
using sones.Lib.DataStructures.UUID;
using sones.GraphFS.Exceptions;
using sones.Lib.ErrorHandling;
using sones.GraphDB.QueryLanguage.Enums;

#endregion

namespace sones.GraphDB.Settings
{

    /// <summary>
    /// Base class for settings
    /// </summary>
    public abstract class ADBSettingsBase : ISettings, IFastSerialize, IFastSerializationTypeSurrogate
    {

        #region Properties

        public String           Name          { get; protected set; }
        public String           Description   { get; protected set; }
        public TypeUUID         Type          { get; protected set; }
        public ADBBaseObject    Default       { get; protected set; }
        public EntityUUID       OwnerID       { get; protected set; }

        #endregion

        #region Constructors

        public ADBSettingsBase()
        {
        }

        public ADBSettingsBase(String myName, String myDescription, EntityUUID myOwnerID, TypeUUID myType, ADBBaseObject myDefault, ADBBaseObject myValue)
        {
            Name        = myName;
            Description = myDescription;
            OwnerID     = myOwnerID;
            Type        = myType;
            Default     = myDefault;
            _Value      = myValue;
        }

        public ADBSettingsBase(Byte[] mySerializedData)
            : this()
        {
            if (mySerializedData == null || mySerializedData.Length == 0)
                throw new ArgumentNullException("mySerializedData must not be null or its length be zero!");

            Deserialize(mySerializedData);
        }

        public ADBSettingsBase(ADBSettingsBase myCopy)
        {
            Name        = myCopy.Name;
            Description = myCopy.Description;
            OwnerID     = myCopy.OwnerID;
            Type        = myCopy.Type;
            Default     = myCopy.Default;
            _Value      = myCopy._Value;
        }

        #endregion



        /// <summary>
        /// Type of the value.
        /// </summary>
        protected ADBBaseObject _Value;

        public abstract ADBBaseObject Value
        {
            get;
            set;
        }


        public abstract SettingUUID ID
        {
            get;
        }



        /// <summary>
        /// it's possible to define a delegate which check the value of two parameters
        /// </summary>
        /// <param name="pParam1">parameter set 1</param>
        /// <param name="pParam2">parameter set 2</param>
        /// <returns></returns>
        public delegate Boolean ValidateFunc(ADBSettingsBase pParam1, ADBSettingsBase pParam2);


        /// <summary>
        /// validates two parametersets
        /// </summary>
        /// <param name="pFunc"></param>
        /// <param name="pParam1"></param>
        /// <param name="pParam2"></param>
        /// <returns></returns>
        public Boolean doValidate(ValidateFunc pFunc, ADBSettingsBase pParam1, ADBSettingsBase pParam2)
        {
            if (pFunc != null)
                return pFunc.Invoke(pParam1, pParam2);

            return false;
        }

        public abstract ISettings Clone();


        #region IFastSerialize Members

        private bool _isDirty = false;
        
        public virtual bool isDirty
        {
            get
            {
                return _isDirty;
            }
            set
            {
                _isDirty = value;
            }
        }

        public DateTime ModificationTime
        {
            get { throw new NotImplementedException(); }
        }

        public byte[] Serialize()
        {
            #region Data

            SerializationWriter writer;

            writer = new SerializationWriter();

            #endregion

            #region Write Basics

            try
            {
                writer.WriteObject(Name);
                writer.WriteObject(Description);
                OwnerID.Serialize(ref writer);
                Type.Serialize(ref writer);
                writer.WriteObject(Default.Value);
                writer.WriteObject(_Value.Value);

                _isDirty = false;
            }
            catch (SerializationException e)
            {
                throw new SerializationException(e.Message);
            }

            #endregion

            return writer.ToArray();
        }

        public void Deserialize(byte[] mySerializedData)
        {
            #region Data
            SerializationReader reader;

            reader = new SerializationReader(mySerializedData);

            #endregion

            #region Read Basics
            try
            {
                Name                = (String) reader.ReadObject();
                Description         = (String) reader.ReadObject();
                OwnerID             = new EntityUUID();
                OwnerID.Deserialize(ref reader);
                Type                = new TypeUUID();
                Type.Deserialize(ref reader);
                Default.SetValue(reader.ReadObject());
                _Value.SetValue(reader.ReadObject());            
            }
            catch (Exception e)
            {
                throw new PandoraFSException_EntityCouldNotBeDeserialized("ADBSetting could not be deserialized!\n\n" + e);
            }
            #endregion
        }

        public void Serialize(ref SerializationWriter mySerializationWriter)
        {
            Serialize(mySerializationWriter, null);
        }

        public void Deserialize(ref SerializationReader mySerializationReader)
        {
            Deserialize(mySerializationReader, null);
        }

        private object Deserialize(ref SerializationReader mySerializationReader, ADBSettingsBase myValue)
        {
            #region Read Basics

            try
            {
                if (myValue != null)
                {
                    myValue.Name = (string)mySerializationReader.ReadObject();
                    myValue.Description = (string)mySerializationReader.ReadObject();
                    myValue.OwnerID = (EntityUUID)mySerializationReader.ReadObject();
                    myValue.Type = (TypeUUID)mySerializationReader.ReadObject();
                    myValue.Default.SetValue(mySerializationReader.ReadObject());
                    myValue._Value.SetValue(mySerializationReader.ReadObject());
                }
                else
                {
                    Name = (string)mySerializationReader.ReadObject();
                    Description = (string)mySerializationReader.ReadObject();
                    OwnerID = (EntityUUID)mySerializationReader.ReadObject();
                    Type = (TypeUUID)mySerializationReader.ReadObject();
                    Default.SetValue(mySerializationReader.ReadObject());
                    _Value.SetValue(mySerializationReader.ReadObject());                
                }
            }
            catch (Exception e)
            {
                throw new PandoraFSException_EntityCouldNotBeDeserialized("ADBSetting could not be deserialized!\n\n" + e);
            }

            #endregion
            return myValue;
        }

        private byte[] Serialize(ref SerializationWriter mySerializationWriter, ADBSettingsBase myValue)
        {
            #region Write Basics

            try
            {
                if (myValue != null)
                {
                    mySerializationWriter.WriteObject(myValue.Name);
                    mySerializationWriter.WriteObject(myValue.Description);
                    mySerializationWriter.WriteObject(myValue.OwnerID);
                    mySerializationWriter.WriteObject(myValue.Type);
                    mySerializationWriter.WriteObject(myValue.Default.Value);
                    mySerializationWriter.WriteObject(myValue._Value.Value);

                    _isDirty = false;
                }
                else
                {
                    mySerializationWriter.WriteObject(Name);
                    mySerializationWriter.WriteObject(Description);
                    mySerializationWriter.WriteObject(OwnerID);
                    mySerializationWriter.WriteObject(Type);
                    mySerializationWriter.WriteObject(Default.Value);
                    mySerializationWriter.WriteObject(_Value.Value);
                }                
            }
            catch (SerializationException e)
            {
                throw new SerializationException(e.Message);
            }

            #endregion

            return mySerializationWriter.ToArray();
        }

        #endregion

        #region IFastSerializationTypeSurrogate Members

        public abstract UInt32 TypeCode { get; }

        public bool SupportsType(Type type)
        {
            return GetType() == type;
        }

        public void Serialize(SerializationWriter writer, object value)
        {
            Serialize(ref writer, (ADBSettingsBase)value);
        }

        public object Deserialize(SerializationReader reader, Type type)
        {
            ADBSettingsBase thisObject = (ADBSettingsBase)Activator.CreateInstance(type);
            return Deserialize(ref reader, thisObject);
        }

        #endregion

        #region get/set/remove

        /// <summary>
        /// Sets a setting
        /// </summary>
        /// <param name="context">The database context</param>
        /// <param name="scope">The scope of the setting</param>
        /// <param name="type">The type where the setting should be set</param>
        /// <param name="attribute">The attribute where the setting should be set</param>
        /// <returns>True for success. Otherwise false.</returns>
        public abstract Exceptional<bool> Set(DBContext context, TypesSettingScope scope, GraphDBType type = null, TypeAttribute attribute = null);
        
        /// <summary>
        /// Gets a setting
        /// </summary>
        /// <param name="context">The database context</param>
        /// <param name="scope">The scope of the setting</param>
        /// <param name="type">The type where the setting should be get from</param>
        /// <param name="attribute">The attribute where the setting should be get from</param>
        /// <returns>True for success. Otherwise false.</returns>
        public abstract Exceptional<ADBSettingsBase> Get(DBContext context, TypesSettingScope scope, GraphDBType type = null, TypeAttribute attribute = null);

        /// <summary>
        /// Removes a setting
        /// </summary>
        /// <param name="context">The database context</param>
        /// <param name="scope">The scope of the setting</param>
        /// <param name="type">The type where the setting should be removed</param>
        /// <param name="attribute">The attribute where the setting should be removed</param>
        /// <returns>True for success. Otherwise false.</returns>
        public abstract Exceptional<bool> Remove(DBContext context, TypesSettingScope scope, GraphDBType type = null, TypeAttribute attribute = null);

        #endregion
    }

}
