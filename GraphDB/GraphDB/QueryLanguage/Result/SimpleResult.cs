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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.Serializer;
using sones.Lib.NewFastSerializer;

namespace sones.GraphDB.QueryLanguage.Result
{    
    public class SimpleResult : IEnumerable<Object[]>, IFastSerialize
    {
        public List<Object[]> Data = null;
        public List<KeyValuePair<String, Object>> Header = null;

        public const int Successful = 0;
        public const int Failed = -1;
        public int iResultType = Successful;

        public List<String> Errors = null;

        public SimpleResult() { }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator<object[]> IEnumerable<object[]>.GetEnumerator()
        {
            throw new NotImplementedException();
        }


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
            // save list of "Header" (List<KeyValuePair<String, Object>>)
            if (Header == null)
            {
                mySerializationWriter.WriteUInt32(0);
            }
            else
            {
                mySerializationWriter.WriteUInt32((UInt32)Header.Count);
                foreach (var header in Header)
                {
                    mySerializationWriter.WriteString(header.Key);
                    mySerializationWriter.WriteObject(header.Value);
                }
            }

            // save list of "Data"
            if (Data == null)
            {
                mySerializationWriter.WriteUInt32(0);
            }
            else
            {
                mySerializationWriter.WriteUInt32((UInt32)Data.Count);
                if (Data.Count > 0)
                {
                    mySerializationWriter.WriteUInt32((UInt32)Data[0].Count());
                }
                foreach (var attr in Data)
                    mySerializationWriter.WriteObject(attr);
            }

            // save iResultType
            mySerializationWriter.WriteInt32(iResultType);

            // save List of "Errors"
            if (Errors == null)
            {
                mySerializationWriter.WriteUInt32(0);
            }
            else
            {
                mySerializationWriter.WriteUInt32((UInt32)Errors.Count);

                foreach (var attr in Errors)
                {
                    mySerializationWriter.WriteString(attr);
                }
            }
        }

        public void Deserialize(ref SerializationReader mySerializationReader)
        {
            // load list of header
            UInt32 count = mySerializationReader.ReadUInt32();
            Header = new List<KeyValuePair<string, object>>();

            for (int i = 0; i < count; i++)
            {
                Header.Add(new KeyValuePair<String, Object>(mySerializationReader.ReadString(), mySerializationReader.ReadObject()));
            }
            

            // load data
            count = mySerializationReader.ReadUInt32();
            UInt32 iCols = mySerializationReader.ReadUInt32();

            Data = new List<object[]>();
            Object[] line = null;

            for (UInt32 i = 0; i < count; i++)
            {
                line = new Object[iCols];
                for (int j = 0; j < iCols; j++)
                {
                    line[j] = mySerializationReader.ReadObject();
                }
                Data.Add(line);
            }

            // load iResultType
            iResultType = mySerializationReader.ReadInt32();

            // load list of Errors
            count = mySerializationReader.ReadUInt32();
            Errors = new List<string>();
            for (int i = 0; i < count; i++)
            {
                Errors.Add(mySerializationReader.ReadString());
            }
        }
        #endregion
    }
}