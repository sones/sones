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

/* 
 * sones GraphDS API - CreateIndex
 * Achim Friedland, 2009 - 2010
 */

#region Usings

using System;
using System.Text;
using System.Collections.Generic;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.Structures;

#endregion

namespace sones.GraphDS.API.CSharp
{

    public class CreateIndexQuery : AGraphDSQuery
    {

        #region Data

        private String       _OnString          = null;
        private String       _EditionString     = null;
        private String       _IndexTypeString   = null;
        private List<String> _AttributeStrings  = new List<String>();

        #endregion

        #region Constructors

        #region CreateIndexQuery(myAGraphDSSharp, myIndexName)

        public CreateIndexQuery(AGraphDSSharp myAGraphDSSharp, String myIndexName = null)
            : base(myAGraphDSSharp)
        {
            Name = myIndexName;
        }

        #endregion

        #endregion


        #region OnVertex(myTypeName)

        public CreateIndexQuery OnVertex(String myTypeName)
        {
            _OnString = "ON VERTEX " + myTypeName;
            return this;
        }

        #endregion

        #region Edition(myEditionName)

        public CreateIndexQuery Edition(String myEditionName)
        {
            _EditionString = "EDITION " + myEditionName;
            return this;
        }

        #endregion

        #region Attribute(myAttributeName, ...)

        public CreateIndexQuery Attribute(String myAttributeName)
        {
            _AttributeStrings.Add(myAttributeName);
            return this;
        }

        public CreateIndexQuery Attribute(String myAttributeName, String myOrder)
        {
            _AttributeStrings.Add(myAttributeName + " " + myOrder);
            return this;
        }

        #endregion

        #region IndexType(myIndexType)

        public CreateIndexQuery IndexType(DBIndexTypes myIndexType)
        {
            _IndexTypeString = "INDEXTYPE " + myIndexType.ToString();
            return this;
        }

        #endregion

        #region IndexType(myIndexType)

        public CreateIndexQuery IndexType(String myIndexType)
        {
            _IndexTypeString = "INDEXTYPE " + myIndexType;
            return this;
        }

        #endregion


        #region GetGQLQuery()

        public override String GetGQLQuery()
        {

            _CommandString.Clear();
            _CommandString.Append("CREATE INDEX ").Append(Name).Append(" ");

            if (_OnString == null)
                throw new ArgumentException("Invalid CREATE INDEX command!");

            if (Name == null || Name == "")
            {

                _CommandString.Append("IDX_");

                if (_AttributeStrings.Count > 0)
                {

                    var Anzahl = _AttributeStrings.Count;

                    for (var i = 0; i < Anzahl - 1; i++)
                        _CommandString.Append(_AttributeStrings[i].Replace(" ", "_") + "_");

                    _CommandString.Append(_AttributeStrings[Anzahl - 1] + " ");

                }

                else
                    throw new ArgumentException("Invalid CREATE INDEX command!");

            }

            if (_EditionString != null)
                _CommandString.Append(_EditionString + " ");

            _CommandString.Append(_OnString + " ");            

            // Add list of attributes
            if (_AttributeStrings.Count == 0)
                throw new ArgumentException("Invalid CREATE INDEX command!");

            AddToCommandString(_AttributeStrings, "");            

            if (_IndexTypeString != null)
                _CommandString.Append(" " + _IndexTypeString);

            return _CommandString.ToString();

        }

        #endregion

        #region ToString()

        public override string ToString()
        {
            return GetGQLQuery();
        }

        #endregion

    }

}

