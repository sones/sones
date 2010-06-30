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


/* <id name="sones GraphDB – LicensedFeatures" />
 * <copyright file="LicensedFeatures.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Daniel Kirstenpfad</developer>
 * <summary></summary>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.Serializer;
using sones.Lib.NewFastSerializer;

namespace sones.GraphDB.Licensing
{
    /// <summary>
    /// this class is an informative class to store the licensed features
    /// </summary>
    public class LicensedFeatures : IFastSerialize, IFastSerializationTypeSurrogate 
    {
        public List<FeatureIDs> Features;

        public Int32 NumberOfLicensedCPUs;

        public Int32 NumberOfLicensedRAM;

        private bool _isDirty = false;

        public UInt32 TypeCode { get { return 206; } }

        public LicensedFeatures()
        {
            Features = new List<FeatureIDs>();
            NumberOfLicensedCPUs = 1;
            NumberOfLicensedRAM = 256;
        }

        #region IFastSerialize Members

        public bool isDirty
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

        public void Serialize(ref SerializationWriter mySerializationWriter)
        {
            mySerializationWriter.WriteObject((UInt32)Features.Count);

            foreach (var entry in Features)
                mySerializationWriter.WriteObject((byte)entry);

            mySerializationWriter.WriteObject(NumberOfLicensedCPUs);
            mySerializationWriter.WriteObject(NumberOfLicensedRAM);
        }

        public void Deserialize(ref SerializationReader mySerializationReader)
        {
            Features = new List<FeatureIDs>();
            UInt32 cnt = (UInt32)mySerializationReader.ReadObject();

            for (int i = 0; i < cnt; i++)
            {
                byte entry = (byte)mySerializationReader.ReadObject();
                Features.Add((FeatureIDs)entry);
            }

            NumberOfLicensedCPUs = (Int32)mySerializationReader.ReadObject();
            NumberOfLicensedRAM = (Int32)mySerializationReader.ReadObject();
        }

        #endregion

        #region IFastSerializationTypeSurrogate Members

        public bool SupportsType(Type type)
        {
            return this.GetType() == type;
        }

        public void Serialize(SerializationWriter mySerializationWriter, object value)
        {
            LicensedFeatures thisObject = (LicensedFeatures)value;
            
            mySerializationWriter.WriteObject((UInt32)thisObject.Features.Count);

            foreach (var entry in thisObject.Features)
                mySerializationWriter.WriteObject((byte)entry);

            mySerializationWriter.WriteObject(thisObject.NumberOfLicensedCPUs);
            mySerializationWriter.WriteObject(thisObject.NumberOfLicensedRAM);
        }

        public object Deserialize(SerializationReader mySerializationReader, Type type)
        {
            LicensedFeatures thisObject = (LicensedFeatures)Activator.CreateInstance(type);
            
            thisObject.Features = new List<FeatureIDs>();
            UInt32 cnt = (UInt32)mySerializationReader.ReadObject();

            for (int i = 0; i < cnt; i++)
            {
                byte entry = (byte)mySerializationReader.ReadObject();
                thisObject.Features.Add((FeatureIDs)entry);
            }

            thisObject.NumberOfLicensedCPUs = (Int32)mySerializationReader.ReadObject();
            thisObject.NumberOfLicensedRAM = (Int32)mySerializationReader.ReadObject();

            return thisObject;
        }

        #endregion
    }
}
