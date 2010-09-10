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
            if (Header == null) mySerializationWriter.WriteObject(0);
            else
            {
                mySerializationWriter.WriteObject(Header.Count);
                foreach (var header in Header)
                {
                    mySerializationWriter.WriteObject(header.Key);
                    mySerializationWriter.WriteObject(header.Value);
                }
            }

            // save list of "Data"
            if (Data == null) mySerializationWriter.WriteObject(0);
            else
            {
                mySerializationWriter.WriteObject(Data.Count);
                if (Data.Count > 0)
                    mySerializationWriter.WriteObject(Data[0].Count());
                foreach (var attr in Data)
                    mySerializationWriter.WriteObject(attr);
            }

            // save iResultType
            mySerializationWriter.WriteObject(iResultType);

            // save List of "Errors"
            if (Errors == null) mySerializationWriter.WriteObject(0);
            else
            {
                mySerializationWriter.WriteObject(Errors.Count);
                foreach (var attr in Errors)
                {
                    mySerializationWriter.WriteObject(attr);
                }
            }
        }

        public void Deserialize(ref SerializationReader mySerializationReader)
        {
            // load list of header
            int count = (int)mySerializationReader.ReadObject();
            Header = new List<KeyValuePair<string, object>>();
            for (int i = 0; i < count; i++)
            {
                Header.Add(new KeyValuePair<String, Object>((String)mySerializationReader.ReadObject(), mySerializationReader.ReadObject()));
            }
            

            // load data
            count = (int) mySerializationReader.ReadObject();
            int iCols = (int) mySerializationReader.ReadObject();
            Data = new List<object[]>();
            Object[] line = null;
            for (int i = 0; i < count; i++)
            {
                line = new Object[iCols];
                for (int j = 0; j < iCols; j++)
                {
                    line[j] = mySerializationReader.ReadObject();
                }
                Data.Add(line);
            }

            // load iResultType
            iResultType = (int) mySerializationReader.ReadObject();

            // load list of Errors
            count = (int) mySerializationReader.ReadObject();
            Errors = new List<string>();
            for (int i = 0; i < count; i++)
            {
                Errors.Add((String) mySerializationReader.ReadObject());
            }
        }
        #endregion
    }
}