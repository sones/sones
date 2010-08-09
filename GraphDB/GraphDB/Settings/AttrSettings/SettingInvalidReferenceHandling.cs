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

/* <id name="sones GraphDB – Invalid reference handling setting" />
 * <copyright file="SettingInvalidReferenceHandling.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>Setting for invalid reference handling (i.e. a friend that does not exist).</summary>
 */

#region Usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeManagement.BasicTypes;

using sones.Lib.Settings;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Exceptions;
using sones.Lib.NewFastSerializer;
using sones.Lib.DataStructures.UUID;
using sones.GraphFS.DataStructures;
using sones.GraphDB.Errors;

#endregion

namespace sones.GraphDB.Settings
{
    public class SettingInvalidReferenceHandling : APersistentSetting, IDBAttributeSetting
    {
        public static readonly SettingUUID UUID = new SettingUUID(101);

        #region TypeCode
        public override UInt32 TypeCode { get { return 526; } }
        #endregion

        public BehaviourOnInvalidReference Behaviour { get; private set; }

        public SettingInvalidReferenceHandling()
        {
            Name = "INVALIDREFERENCEHANDLING"; //uppercase
            Description = "Setting for invalid reference handling.";
            Type = DBString.UUID;
            Default = new DBString(BehaviourOnInvalidReference.ignore.ToString().ToUpper());
            this._Value = Default.Clone();
            Behaviour = BehaviourOnInvalidReference.ignore;
        }

        public SettingInvalidReferenceHandling(Byte[] mySerializedData)
            : base(mySerializedData)
        { }

        public SettingInvalidReferenceHandling(APersistentSetting myCopy)
            : base(myCopy)
        {
            Behaviour = ((SettingInvalidReferenceHandling)myCopy).Behaviour;
        }

        public override SettingUUID ID
        {
            get { return UUID; }
        }
        
        public override ADBBaseObject Value
        {
            get
            {
                return this._Value;
            }
            set
            {
                if (value is DBString)
                {
                    String valueString = ((String)((DBString)value).Value).ToLower();

                    if ((valueString == BehaviourOnInvalidReference.ignore.ToString()))
                    {
                        Behaviour = BehaviourOnInvalidReference.ignore;
                    }
                    else
                    {
                        if ((valueString == BehaviourOnInvalidReference.log.ToString()))
                        {
                            Behaviour = BehaviourOnInvalidReference.log;
                        }
                        else
                        {

                            throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true), valueString));

                        }
                    }
                }
                else
                {
                    throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true), value.GetType().Name));
                }

                this._Value = value;
            }
        }

        public override ISettings Clone()
        {
            return new SettingInvalidReferenceHandling(this);
        }

        #region IFastSerializationTypeSurrogate Members

        public new bool SupportsType(Type type)
        {
            return GetType() == type;
        }

        public new void Serialize(SerializationWriter writer, object value)
        {
            base.Serialize(writer, this);
            writer.WriteByte((Byte)Behaviour);
        }

        public new object Deserialize(SerializationReader reader, Type type)
        {
            return MyDeserialize(reader, type);
        }

        private object MyDeserialize(SerializationReader reader, System.Type type)
        {
            var result = (SettingInvalidReferenceHandling)base.Deserialize(reader, type);
            result.Behaviour = (BehaviourOnInvalidReference)reader.ReadOptimizedByte();

            return result;
        }

        #endregion
    }

    public enum BehaviourOnInvalidReference
    {
        ignore,
        log       
    }
}
