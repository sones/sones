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
 * sones GraphDSSharp - AlterTypeQuery
 * Achim Friedland, 2009 - 2010
 */

#region Usings

using System;
using System.Text;
using System.Collections.Generic;
using sones.GraphDB.Structures;

#endregion

namespace sones.GraphDS.API.CSharp
{
  
    public class AlterTypeQuery : AGraphDSQuery
    {

        #region Data

        private List<String> _AddAttributesStrings          = new List<String>();
        private List<String> _DropAttributesStrings         = new List<String>();
        private List<String> _AddBackwardEdgesStrings       = new List<String>();
        private List<String> _DropBackwardEdgesStrings      = new List<String>();
        private List<String> _AddIndicesStrings             = new List<String>();
        private List<String> _DropIndicesStrings            = new List<String>();
        private List<String> _UniqunessStrings              = new List<String>();        

        private String       _RenameTypeString              = null;
        private String       _RenameAttributeString         = null;
        private String       _RenameBackwardEdgeString      = null;
        private String       _CommentString                 = null;
        private String       _DropUniqunessString           = null;
        private String       _DropMandatoryString           = null;


        #endregion

        #region Properties

        public String Name { get; private set; }

        #endregion

        #region Constructors

        #region AlterTypeQuery(myTypeName)

        public AlterTypeQuery(String myTypeName)
        {
            Name            = myTypeName;
            _CommandString  = new StringBuilder();
        }

        #endregion

        #region AlterTypeQuery(myIGraphDBSession, myTypeName)

        public AlterTypeQuery(AGraphDSSharp myIGraphDBSession, String myTypeName)
            : this(myTypeName)
        {
            _DBWrapper      = myIGraphDBSession;
        }

        #endregion

        #endregion


        #region AddAttributes(...)

        public AlterTypeQuery AddAttribute(String myAttributeType, String myAttributeName, Object defaultValue = null, DBIndexTypes myDBIndexType = DBIndexTypes.HashTable)
        {
            String defaultValStr = String.Empty;

            if (defaultValue != null)
                defaultValStr = "=" + defaultValue.ToString();
            
            _AddAttributesStrings.Add(myAttributeType + " " + myAttributeName + defaultValStr);
            return AddIndex(myAttributeName, myDBIndexType);
        }

        public AlterTypeQuery AddInteger(String myAttributeName, Object defaultValue = null, DBIndexTypes myDBIndexType = DBIndexTypes.HashTable)
        {
            String defaultValStr = String.Empty;

            if (defaultValue != null)
                defaultValStr = "=" + defaultValue.ToString();
            
            _AddAttributesStrings.Add("Integer " + myAttributeName + defaultValStr);
            return AddIndex(myAttributeName, myDBIndexType);
        }
        

        public AlterTypeQuery AddString(String myAttributeName, Object defaultValue = null, DBIndexTypes myDBIndexType = DBIndexTypes.HashTable)
        {
            String defaultValStr = String.Empty;

            if (defaultValue != null)
                defaultValStr = "='" + defaultValue.ToString() + "'";

            _AddAttributesStrings.Add("String " + myAttributeName + defaultValStr);
            return AddIndex(myAttributeName, myDBIndexType);
        }
        

        public AlterTypeQuery AddDateTime(String myAttributeName, Object defaultValue = null, DBIndexTypes myDBIndexType = DBIndexTypes.HashTable)
        {
            String defaultValStr = String.Empty;

            if (defaultValue != null)
                defaultValStr = "=" + defaultValue.ToString();
            
            _AddAttributesStrings.Add("DateTime " + myAttributeName + defaultValStr);
            return AddIndex(myAttributeName, myDBIndexType);
        }

        #endregion

        #region AddBackwardEdge(myDatabaseType, myReferencedAttributeName, myAttributeName)

        public AlterTypeQuery AddBackwardEdge(String myDatabaseType, String myReferencedAttributeName, String myAttributeName)
        {
            _AddBackwardEdgesStrings.Add(myDatabaseType + "." + myReferencedAttributeName + " " + myAttributeName);
            return this;
        }

        #endregion

        #region RenameBackwarEdge(myBackwarEdgeName, myNewBackwarEdgeName)

        public AlterTypeQuery RenameBackwardEdge(String myBackwardEdgeName, String myNewBackwardEdgeName)
        {
            _RenameBackwardEdgeString = " RENAME BACKWARDEDGE " + myBackwardEdgeName + " TO " + myNewBackwardEdgeName;
            return this;        
        }
        
        #endregion

        #region AddBackwardEdge(myDatabaseType, myReferencedAttributeName, myAttributeName)

        public AlterTypeQuery AddBackwardEdge(CreateTypeQuery myDatabaseType, String myReferencedAttributeName, String myAttributeName)
        {
            _AddBackwardEdgesStrings.Add(myDatabaseType.Name + "." + myReferencedAttributeName + " " + myAttributeName);
            return this;
        }

        #endregion

        #region AddIndex(myAttributeName)

        public AlterTypeQuery AddIndex(String myAttributeName, DBIndexTypes myDBIndexType = DBIndexTypes.HashTable)
        {
            _AddIndicesStrings.Add(myAttributeName + " " + myDBIndexType.ToString());
            return this;
        }

        #endregion

        #region Drop Unique

        public AlterTypeQuery DropUnique()
        {
            _DropUniqunessString = " DROP UNIQUE ";
            return this;
        }

        #endregion

        #region Drop Mandatory

        public AlterTypeQuery DropMandatory()
        {
            _DropMandatoryString = " DROP MANDATORY ";
            return this;
        }

        #endregion

        #region Rename Type

        public AlterTypeQuery RenameType(String myTypeName)
        {
            _RenameTypeString = " RENAME TO " + myTypeName;
            return this;
        }

        #endregion

        #region Rename Attribute

        public AlterTypeQuery RenameAttribute(String myAttributeName, String myNewAttributeName)
        {
            _RenameAttributeString = " RENAME ATTRIBUTE " + myAttributeName + " TO " + myNewAttributeName;
            return this;
        }

        #endregion

        #region Unique(params myUniqueAttributes)

        public AlterTypeQuery Unique(params String[] myUniqueAttributes)
        {

            _UniqunessStrings.Clear();
            _UniqunessStrings.AddRange(myUniqueAttributes);

            return this;

        }

        #endregion

        #region Comment

        public AlterTypeQuery Comment(String myComment)
        {

            if (myComment != null)
                _CommentString = " COMMENT = '" + myComment + "'";

            if (myComment == "")
                _CommentString = null;

            return this;

        }

        #endregion

        #region DropAttributes(myAttributeName)

        public AlterTypeQuery DropAttribute(String myAttributeName)
        {
            _DropAttributesStrings.Add(myAttributeName);
            return this;
        }

        #endregion

        #region DropBackwardEdge(myBackwardEdge)

        public AlterTypeQuery DropBackwardEdge(String myBackwardEdge)
        {
            _DropBackwardEdgesStrings.Add(myBackwardEdge);
            return this;
        }

        #endregion


        #region GetGQLQuery()

        public override String GetGQLQuery()
        {

            _CommandString.Clear();
            _CommandString.Append("ALTER TYPE ").Append(Name).Append(" ");

            AddToCommandString(_AddAttributesStrings,     "ADD ATTRIBUTES");
            AddToCommandString(_DropAttributesStrings,    "DROP ATTRIBUTES");
            AddToCommandString(_AddBackwardEdgesStrings,  "ADD BACKWARDEDGES");
            AddToCommandString(_DropBackwardEdgesStrings, "DROP BACKWARDEDGES");
            AddToCommandString(_UniqunessStrings,         "UNIQUE");

            //rename attribute
            if (_RenameAttributeString != null)
                _CommandString.Append(_RenameAttributeString);

            //rename backwardedge
            if (_RenameBackwardEdgeString != null)
                _CommandString.Append(_RenameBackwardEdgeString);
            
            //rename type
            if (_RenameTypeString != null)
                _CommandString.Append(_RenameTypeString);

            //drop unique
            if (_DropUniqunessString != null)
                _CommandString.Append(_DropUniqunessString);

            //drop mandatory
            if (_DropMandatoryString != null)
                _CommandString.Append(_DropMandatoryString);

            // COMMENT
            if (_CommentString != null)
                _CommandString.Append(_CommentString);


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

