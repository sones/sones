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
using System.Collections.Generic;

using sones.Lib;
using sones.GraphDB.Structures;

#endregion

namespace sones.GraphDS.API.CSharp
{
  
    public class AlterVertexQuery : AGraphDSQuery
    {

        #region Data

        private List<String> _AddAttributesStrings          = new List<String>();
        private List<String> _DropAttributesStrings         = new List<String>();
        private List<String> _AddBackwardEdgesStrings       = new List<String>();
        private List<String> _DropBackwardEdgesStrings      = new List<String>();        
        private List<String> _DropIndicesStrings            = new List<String>();
        private List<String> _UniqunessStrings              = new List<String>();
        private List<String> _DefineAttributesString        = new List<String>();
        private List<String> _UndefineAttributesString      = new List<String>();

        private String       _RenameTypeString              = null;
        private String       _RenameAttributeString         = null;
        private String       _RenameBackwardEdgeString      = null;
        private String       _CommentString                 = null;
        private String       _DropUniqunessString           = null;
        private String       _DropMandatoryString           = null;
        private String       _AddIndicesString              = null;
        private String       _DropIndicesString             = null;


        #endregion

        #region Constructor(s)

        #region AlterVertexQuery(myAGraphDSSharp, myTypeName)

        public AlterVertexQuery(AGraphDSSharp myAGraphDSSharp, String myTypeName)
            : base(myAGraphDSSharp)
        {
            Name = myTypeName;
        }

        #endregion

        #endregion


        #region AddAttributes(...)

        public AlterVertexQuery AddAttribute(String myAttributeType, String myAttributeName, Object defaultValue = null, DBIndexTypes? myDBIndexType = null)
        {
            String defaultValStr = String.Empty;

            if (defaultValue != null)
                defaultValStr = "=" + defaultValue.ToString();
            
            _AddAttributesStrings.Add(myAttributeType + " " + myAttributeName + defaultValStr);

            if(myDBIndexType != null)
                return AddIndices(myAttributeName, myDBIndexType:myDBIndexType);

            return this;
        }

        public AlterVertexQuery AddAttributeT<T>(String myAttributeName, T defaultValue = default(T), DBIndexTypes? myDBIndexType = null)
        {
            String defaultValStr = String.Empty;

            if (!EqualityComparer<T>.Default.Equals(defaultValue, default(T)))
                defaultValStr = "=" + defaultValue.ToString();

            _AddAttributesStrings.Add(typeof(T).Name + " " + myAttributeName + defaultValStr);
            
            if(myDBIndexType != null)
                return AddIndices(myAttributeName, myDBIndexType: myDBIndexType);

            return this;
        }

        public AlterVertexQuery AddInteger(String myAttributeName, Object defaultValue = null, DBIndexTypes? myDBIndexType = null)
        {
            String defaultValStr = String.Empty;

            if (defaultValue != null)
                defaultValStr = "=" + defaultValue.ToString();
            
            _AddAttributesStrings.Add("Integer " + myAttributeName + defaultValStr);

            if(myDBIndexType != null)
                return AddIndices(myAttributeName, myDBIndexType: myDBIndexType);

            return this;
        }
        

        public AlterVertexQuery AddString(String myAttributeName, Object defaultValue = null, DBIndexTypes? myDBIndexType = null)
        {
            String defaultValStr = String.Empty;

            if (defaultValue != null)
                defaultValStr = "='" + defaultValue.ToString() + "'";

            _AddAttributesStrings.Add("String " + myAttributeName + defaultValStr);

            if(myDBIndexType != null)
                return AddIndices(myAttributeName, myDBIndexType: myDBIndexType);

            return this;
        }
        

        public AlterVertexQuery AddDateTime(String myAttributeName, Object defaultValue = null, DBIndexTypes? myDBIndexType = null)
        {
            String defaultValStr = String.Empty;

            if (defaultValue != null)
                defaultValStr = "=" + defaultValue.ToString();
            
            _AddAttributesStrings.Add("DateTime " + myAttributeName + defaultValStr);

            if(myDBIndexType != null)
                return AddIndices(myAttributeName, myDBIndexType: myDBIndexType);

            return this;
        }

        #endregion

        #region AddBackwardEdge(myDatabaseType, myReferencedAttributeName, myAttributeName)

        public AlterVertexQuery AddBackwardEdge(String myDatabaseType, String myReferencedAttributeName, String myAttributeName)
        {
            _AddBackwardEdgesStrings.Add(myDatabaseType + "." + myReferencedAttributeName + " " + myAttributeName);
            return this;
        }

        #endregion

        #region RenameBackwarEdge(myBackwarEdgeName, myNewBackwarEdgeName)

        public AlterVertexQuery RenameBackwardEdge(String myBackwardEdgeName, String myNewBackwardEdgeName)
        {
            _RenameBackwardEdgeString = " RENAME BACKWARDEDGE " + myBackwardEdgeName + " TO " + myNewBackwardEdgeName;
            return this;        
        }
        
        #endregion

        #region AddBackwardEdge(myDatabaseType, myReferencedAttributeName, myAttributeName)

        public AlterVertexQuery AddBackwardEdge(CreateVertexQuery myDatabaseType, String myReferencedAttributeName, String myAttributeName)
        {
            _AddBackwardEdgesStrings.Add(myDatabaseType.Name + "." + myReferencedAttributeName + " " + myAttributeName);
            return this;
        }

        #endregion

        #region AddIndices(myAttributeName, myIndexName,myIndexEdition,myDBIndexType)

        public AlterVertexQuery AddIndices(String myAttributeName, String myIndexName = "", String myIndexEdition = "", DBIndexTypes? myDBIndexType = null)
        {
            Boolean addOnAttr = false;

            if (_AddIndicesString == null)
            {
                _AddIndicesString += " ADD INDICES (";
            }
            else
            {
                _AddIndicesString += ",";
            }
            
            if (myIndexName != "")
            {
                _AddIndicesString += "(" + myIndexName + " ";
                addOnAttr = true;
            }

            if (myIndexEdition != "")
            {
                if (!_AddIndicesString.Contains("(("))
                    _AddIndicesString += "(";

                _AddIndicesString += "EDITION " + myIndexEdition + " ";
                addOnAttr = true;
            }

            if (myDBIndexType != null)
            {
                if (!_AddIndicesString.Contains("(("))
                    _AddIndicesString += "(";

                _AddIndicesString += "INDEXTYPE " + myDBIndexType.ToString();
                addOnAttr = true;
            }

            if (addOnAttr)
            {
                _AddIndicesString += " ON ATTRIBUTES " + myAttributeName + ")";
            }
            else
            {
                _AddIndicesString += "( " + myAttributeName + " )";
            }

            return this;
        }

        #endregion

        #region Drop Indices

        public AlterVertexQuery DropIndices(String myIndexName, String myIndexEdition = "")
        {   

            if (_DropIndicesString == null)
            {
                _DropIndicesString += " DROP INDICES (";
            }
            else
            {
                _DropIndicesString += ",";
            }


            _DropIndicesString += "(" + myIndexName + " ";            

            if (myIndexEdition != "")
            {
                _DropIndicesString += "EDITION " + myIndexEdition + " ";
            }

            _DropIndicesString += ")";
            
            return this;
        }

        #endregion

        #region Drop Unique

        public AlterVertexQuery DropUnique()
        {
            _DropUniqunessString = " DROP UNIQUE ";
            return this;
        }

        #endregion

        #region Drop Mandatory

        public AlterVertexQuery DropMandatory()
        {
            _DropMandatoryString = " DROP MANDATORY ";
            return this;
        }

        #endregion

        #region Rename Type

        public AlterVertexQuery RenameType(String myTypeName)
        {
            _RenameTypeString = " RENAME TO " + myTypeName;
            return this;
        }

        #endregion

        #region Rename Attribute

        public AlterVertexQuery RenameAttribute(String myAttributeName, String myNewAttributeName)
        {
            _RenameAttributeString = " RENAME ATTRIBUTE " + myAttributeName + " TO " + myNewAttributeName;
            return this;
        }

        #endregion

        #region Unique(params myUniqueAttributes)

        public AlterVertexQuery Unique(params String[] myUniqueAttributes)
        {

            _UniqunessStrings.Clear();
            _UniqunessStrings.AddRange(myUniqueAttributes);

            return this;

        }

        #endregion

        #region Comment

        public AlterVertexQuery Comment(String myComment)
        {

            if (myComment != null)
                _CommentString = " COMMENT = '" + myComment + "'";

            if (myComment == "")
                _CommentString = null;

            return this;

        }

        #endregion

        #region DropAttributes(myAttributeName)

        public AlterVertexQuery DropAttribute(String myAttributeName)
        {
            _DropAttributesStrings.Add(myAttributeName);
            return this;
        }

        #endregion

        #region DropBackwardEdge(myBackwardEdge)

        public AlterVertexQuery DropBackwardEdge(String myBackwardEdge)
        {
            _DropBackwardEdgesStrings.Add(myBackwardEdge);
            return this;
        }

        #endregion

        #region Define Attributes

        public AlterVertexQuery DefineAttributes(String myAttributeType, String myAttributeName)
        {
            _DefineAttributesString.Add(myAttributeType + " " + myAttributeName);
            return this;
        }

        #endregion

        #region Undefine Attributes

        public AlterVertexQuery UndefineAttributes(String myAttributeName)
        {
            _UndefineAttributesString.Add(myAttributeName);
            return this;    
        }

        #endregion


        #region GetGQLQuery()

        public override String GetGQLQuery()
        {
            _CommandString.Clear();
            _CommandString.Append("ALTER TYPE ").Append(Name).Append(" ");

            AddToCommandString(_AddAttributesStrings,     "ADD ATTRIBUTES", true);
                        
            AddToCommandString(_DropAttributesStrings,    "DROP ATTRIBUTES", true);

            AddToCommandString(_AddBackwardEdgesStrings,  "ADD BACKWARDEDGES", true);

            AddToCommandString(_DropBackwardEdgesStrings, "DROP BACKWARDEDGES", true);
            
            //add indices            
            if (_AddIndicesString != null)
            {
                _CommandString.Append(_AddIndicesString);
                _CommandString.Append(")");
                _CommandString.Append(",");
            }

            //drop indices
            if (_DropIndicesString != null)
            {
                _CommandString.Append(_DropIndicesString);
                _CommandString.Append(")");
                _CommandString.Append(",");
            }

            //rename attribute
            if (_RenameAttributeString != null)
            {
                _CommandString.Append(_RenameAttributeString);
                _CommandString.Append(",");
            }

            //rename backwardedge
            if (_RenameBackwardEdgeString != null)
            {
                _CommandString.Append(_RenameBackwardEdgeString);
                _CommandString.Append(",");
            }
            
            //rename type
            if (_RenameTypeString != null)
            {
                _CommandString.Append(_RenameTypeString);
                _CommandString.Append(",");
            }

            AddToCommandString(_DefineAttributesString, "DEFINE ATTRIBUTES", true);
            AddToCommandString(_UndefineAttributesString, "UNDEFINE ATTRIBUTES", true);

            //drop unique
            if (_DropUniqunessString != null)
            {
                _CommandString.Append(_DropUniqunessString);
                _CommandString.Append(",");
            }

            //drop mandatory
            if (_DropMandatoryString != null)
            {
                _CommandString.Append(_DropMandatoryString);
                _CommandString.Append(",");
            }

            // COMMENT
            if (_CommentString != null)
            {
                _CommandString.Append(_CommentString);
                _CommandString.Append(",");
            }

            AddToCommandString(_UniqunessStrings, "UNIQUE", true);            

            if (_CommandString[_CommandString.Length - 1] == ',')
                _CommandString.RemoveEnding(1);

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

