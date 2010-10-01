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

/*
 * sones GraphDS API - CreateVertexQuery
 * (c) Achim 'ahzf' Friedland, 2009 - 2010
 */

#region Usings

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using sones.GraphDB.Structures;
using sones.GraphDB.TypeManagement;

using sones.Lib;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Result;

#endregion

namespace sones.GraphDS.API.CSharp.Fluent
{

    public class CreateVertexQuery : CreateTypeQuery
    {

        #region Data

        private String       _ExtendsString         = null;
        private String       _AbstractString        = null;
        private List<String> _AttributeStrings      = new List<String>();
        private List<String> _BackwardEdgesStrings  = new List<String>();
        private List<String> _MandatoryStrings      = new List<String>();
        private List<String> _UniqunessStrings      = new List<String>();
        private List<String> _IndicesStrings        = new List<String>();
        private String       _CommentString         = null;

        #endregion

        #region Properties

        public Boolean _IsAbstract { get; private set; }        

        #endregion

        #region Constructor(s)

        #region CreateVertexQuery(myAGraphDSSharp, myTypeName, myIsAbstract)

        public CreateVertexQuery(AGraphDSSharp myAGraphDSSharp, String myTypeName, Boolean myIsAbstract = false)
            : base(myAGraphDSSharp)
        {

            Name = myTypeName;

            if (myIsAbstract)
                _AbstractString = "ABSTRACT ";

            _IsAbstract = myIsAbstract;

        }

        #endregion

        #endregion


        #region Extends(myBaseTypeName)

        public CreateVertexQuery Extends(String myBaseTypeName)
        {
            _ExtendsString = " EXTENDS " + myBaseTypeName;
            return this;
        }

        #endregion

        #region Extends(myBaseTypeName)

        public CreateVertexQuery Extends(CreateVertexQuery myBaseGraphDBType)
        {
            _ExtendsString = " EXTENDS " + myBaseGraphDBType.Name;
            return this;
        }

        #endregion



        #region AddAttributes(...)

        // Use "mandatory" and "hyperEdge", but not "myMandatory" and "myHyperEdge" for a more 'fluent' interface!

        public CreateVertexQuery AddAttribute(String myAttributeType, String myAttributeName, Object defaultValue = null, Boolean hyperEdge = false, Boolean mandatory = false, Boolean unique = false, DBIndexTypes? index = null)
        {

            if (hyperEdge)
                _AttributeStrings.Add("SET<" + myAttributeType + "> " + myAttributeName);
            else
                _AttributeStrings.Add(myAttributeType + " " + myAttributeName);
            
            if (defaultValue != null)
                _AttributeStrings[_AttributeStrings.Count - 1] += ("=" + defaultValue.ToString());

            if (unique)
               _UniqunessStrings.Add(myAttributeName);

            if (mandatory)
                _MandatoryStrings.Add(myAttributeName);
            
            if (index != null)
                return AddIndex(myAttributeName, index);            

            return this;

        }

        public CreateVertexQuery AddAttribute(CreateVertexQuery myAttributeType, String myAttributeName, Object defaultValue = null, Boolean hyperEdge = false, Boolean mandatory = false, Boolean unique = false, DBIndexTypes? index = null)
        {

            if (hyperEdge)
                _AttributeStrings.Add("SET<" + myAttributeType.Name + "> " + myAttributeName);
            else
                _AttributeStrings.Add(myAttributeType.Name + " " + myAttributeName);

            if (defaultValue != null)
                _AttributeStrings[_AttributeStrings.Count - 1] += ("=" + defaultValue.ToString());

            if (unique)
                _UniqunessStrings.Add(myAttributeName);

            if (mandatory)
                _MandatoryStrings.Add(myAttributeName);            

            if (index != null)
                return AddIndex(myAttributeName, index);

            return this;

        }

        public CreateVertexQuery AddAttributeT<T>(String myAttributeName, T defaultValue = default(T), Boolean hyperEdge = false, Boolean mandatory = false, Boolean unique = false, DBIndexTypes? index = null)
        {

            _AttributeStrings.Add(typeof(T).Name + " " + myAttributeName);

            if (!EqualityComparer<T>.Default.Equals(defaultValue, default(T)))
                _AttributeStrings[_AttributeStrings.Count - 1] += ("=" + defaultValue.ToString());

            if (unique)
                _UniqunessStrings.Add(myAttributeName);

            if (mandatory)
                _MandatoryStrings.Add(myAttributeName);

            if (index != null)
                return AddIndex(myAttributeName, index);
            
            return this;
        }

        public CreateVertexQuery AddEdge(CreateEdgeQuery myCreateEdgeQuery, Boolean mandatory = false, Boolean unique = false, DBIndexTypes? index = null)
        {

            if (myCreateEdgeQuery.IsHypeEdge)
                _AttributeStrings.Add("SET<" + Name + "> " + myCreateEdgeQuery.Name);
            else
                _AttributeStrings.Add(Name + " " + myCreateEdgeQuery.Name);

            if (unique)
                _UniqunessStrings.Add(myCreateEdgeQuery.Name);
            
            if (mandatory)
                _MandatoryStrings.Add(myCreateEdgeQuery.Name);            

            if (index != null)
                return AddIndex(myCreateEdgeQuery.Name, index);

            return this;

        }

        public CreateVertexQuery AddLoop(String myAttributeName, Boolean hyperEdge = false, Boolean mandatory = false, Boolean unique = false, DBIndexTypes? index = null)
        {

            if (hyperEdge)
                _AttributeStrings.Add("SET<" + Name + "> " + myAttributeName);
            else
                _AttributeStrings.Add(Name + " " + myAttributeName);

            if (unique)
                _UniqunessStrings.Add(myAttributeName);

            if (mandatory)
                _MandatoryStrings.Add(myAttributeName);

            if (index != null)
                return AddIndex(myAttributeName, index);

            return this;

        }

        public CreateVertexQuery AddInteger(String myAttributeName, Object defaultValue = null, Boolean mandatory = false, Boolean unique = false, DBIndexTypes? index = null)
        {   

            _AttributeStrings.Add("Integer " + " " + myAttributeName);

            if (defaultValue != null)
                _AttributeStrings[_AttributeStrings.Count - 1] += ("=" + defaultValue.ToString());

            if (mandatory)
                _MandatoryStrings.Add(myAttributeName);

            if (unique)
                _UniqunessStrings.Add(myAttributeName);

            if (index != null)
                return AddIndex(myAttributeName, index);

            return this;

        }

        public CreateVertexQuery AddString(String myAttributeName, Object defaultValue = null, Boolean mandatory = false, Boolean unique = false, DBIndexTypes? index = null)
        {   
            
            _AttributeStrings.Add("String " + " " + myAttributeName);

            if (defaultValue != null)
                _AttributeStrings[_AttributeStrings.Count - 1] += ("='" + defaultValue.ToString() + "'");

            if (mandatory)
                _MandatoryStrings.Add(myAttributeName);

            if (unique)
                _UniqunessStrings.Add(myAttributeName);

            if (index != null)
                return AddIndex(myAttributeName, index);

            return this;

        }

        public CreateVertexQuery AddDateTime(String myAttributeName, Object defaultValue = null, Boolean mandatory = false, Boolean unique = false, DBIndexTypes? index = null)
        {

            _AttributeStrings.Add("DateTime " + " " + myAttributeName);

            if (defaultValue != null)
                _AttributeStrings[_AttributeStrings.Count - 1] += ("=" + defaultValue.ToString());

            if (mandatory)
                _MandatoryStrings.Add(myAttributeName);

            if (unique)
                _UniqunessStrings.Add(myAttributeName);

            if (index != null)
                return AddIndex(myAttributeName, index);

            return this;

        }

        #endregion

        #region Unique(params myUniqueAttributes)

        public CreateVertexQuery Unique(params String[] myUniqueAttributes)
        {

            if (myUniqueAttributes.IsNullOrEmpty())
                throw new ArgumentNullException("myUniqueAttributes must not be null or its lenght zero!");

            _UniqunessStrings.Clear();
            _UniqunessStrings.AddRange(myUniqueAttributes);

            return this;

        }

        #endregion

        #region Comment

        public CreateVertexQuery Comment(String myComment)
        {

            if (myComment != null)
                _CommentString = " COMMENT = '" + myComment + "'";

            if (myComment == "")
                _CommentString = null;

            return this;

        }

        #endregion


        #region AddBackwardEdge(myDatabaseType, myCreateEdgeQuery, myAttributeName)

        [Obsolete]
        public CreateVertexQuery AddBackwardEdge(String myDatabaseType, String myCreateEdgeQuery, String myAttributeName)
        {
            _BackwardEdgesStrings.Add(myDatabaseType + "." + myCreateEdgeQuery + " " + myAttributeName);
            return this;
        }

        #endregion

        #region AddBackwardEdge(myDatabaseType, myCreateEdgeQuery, myAttributeName)

        [Obsolete]
        public CreateVertexQuery AddBackwardEdge(CreateVertexQuery myDatabaseType, String myCreateEdgeQuery, String myAttributeName)
        {
            _BackwardEdgesStrings.Add(myDatabaseType.Name + "." + myCreateEdgeQuery + " " + myAttributeName);
            return this;
        }

        #endregion

        #region AddBackwardEdge(myDatabaseType, myCreateEdgeQuery, myAttributeName)

        public CreateVertexQuery AddBackwardEdge(String myDatabaseType, CreateEdgeQuery myCreateEdgeQuery, String myAttributeName)
        {
            _BackwardEdgesStrings.Add(myDatabaseType + "." + myCreateEdgeQuery.Name + " " + myAttributeName);
            return this;
        }

        #endregion

        #region AddBackwardEdge(myDatabaseType, myCreateEdgeQuery, myAttributeName)

        public CreateVertexQuery AddBackwardEdge(CreateVertexQuery myDatabaseType, CreateEdgeQuery myCreateEdgeQuery, String myAttributeName)
        {
            _BackwardEdgesStrings.Add(myDatabaseType.Name + "." + myCreateEdgeQuery.Name + " " + myAttributeName);
            return this;
        }

        #endregion


        #region AddIndex(myAttributeName)

        public CreateVertexQuery AddIndex(String myAttributeName)
        {

            if (myAttributeName.IsNullOrEmpty())
                throw new ArgumentNullException("myAttributeName must not be null or its lenght zero!");

            _IndicesStrings.Add("(" + myAttributeName + ")");

            return this;

        }

        #endregion

        #region AddIndex(myAttributeName, myDBIndexType)

        public CreateVertexQuery AddIndex(String myAttributeName, DBIndexTypes? myDBIndexType)
        {

            if (myDBIndexType == null)
                return AddIndex(myAttributeName);

            return AddIndex(myAttributeName, myDBIndexType.ToString());

        }

        #endregion

        #region AddIndex(myIndexName, myAttributeName, myDBIndexType)

        public CreateVertexQuery AddIndex(String myIndexName, String myAttributeName, DBIndexTypes? myDBIndexType)
        {

            if (myAttributeName.IsNullOrEmpty())
                throw new ArgumentNullException("myAttributeName must not be null or its lenght zero!");

            if (myIndexName == null && myDBIndexType == null)
                return AddIndex(myAttributeName);

            if (myIndexName != null && myDBIndexType != null)
                return AddIndex(myIndexName, myAttributeName, myDBIndexType.ToString());

            if (myIndexName == null)
                return AddIndex("IDX_" + myAttributeName, myAttributeName);

            if (myDBIndexType == null)
                return AddIndex(myIndexName, myAttributeName);

            return this;

        }

        #endregion

        #region AddIndex(myAttributeName, (String) myDBIndexType)

        public CreateVertexQuery AddIndex(String myAttributeName, String myDBIndexType)
        {

            if (myAttributeName.IsNullOrEmpty())
                throw new ArgumentNullException("myAttributeName must not be null or its lenght zero!");

            if (myDBIndexType == null)
                return AddIndex(myAttributeName);

            return AddIndex("IDX_" + myAttributeName, myAttributeName, myDBIndexType.ToString());

        }

        #endregion

        #region AddIndex(myIndexName, myAttributeName, (String) myDBIndexType)

        public CreateVertexQuery AddIndex(String myIndexName, String myAttributeName, String myDBIndexType)
        {

            if (myIndexName.IsNullOrEmpty())
                throw new ArgumentNullException("myIndexName must not be null or its lenght zero!");

            if (myAttributeName.IsNullOrEmpty())
                throw new ArgumentNullException("myAttributeName must not be null or its lenght zero!");

            if (myDBIndexType.IsNullOrEmpty())
                throw new ArgumentNullException("myDBIndexType must not be null or its lenght zero!");

            if (myDBIndexType == null)
                return AddIndex(myAttributeName);

            _IndicesStrings.Add("(" + myIndexName + " INDEXTYPE " + myDBIndexType + " ON ATTRIBUTES " + myAttributeName + ")");

            return this;

        }

        #endregion


        #region Execute2(myAction)

        public CreateVertexQuery Execute2(Action<QueryResult> myAction)
        {

            if (_DBWrapper != null)
            {

                var _QueryResult = _DBWrapper.Query(this.GetGQLQuery());

                if (myAction != null)
                    myAction(_QueryResult);

                return this;

            }

            throw new ArgumentNullException("_DBWrapper is null!");

        }

        #endregion


        #region GetGQLQuery()

        public override String GetGQLQuery()
        {

            _CommandString.Clear();

            _CommandString.Append("CREATE ");

            if (_AbstractString != null)
                _CommandString.Append(_AbstractString);
            
            _CommandString.Append("VERTEX ").Append(Name);

            // EXTENDS
            if (_ExtendsString != null)
                _CommandString.Append(_ExtendsString);

            AddToCommandString(_AttributeStrings,       "ATTRIBUTES");
            AddToCommandString(_BackwardEdgesStrings,   "BACKWARDEDGES");
            AddToCommandString(_UniqunessStrings,       "UNIQUE");
            AddToCommandString(_MandatoryStrings,       "MANDATORY");
            AddToCommandString(_IndicesStrings,         "INDICES");            

            // COMMENT
            if (_CommentString != null)
                _CommandString.Append(_CommentString);

            if (_CommandString[_CommandString.Length - 1] == ',')
                _CommandString.RemoveEnding(1);

            return _CommandString.ToString();

        }

        #endregion


        #region ToString()

        public override String ToString()
        {
            return GetGQLQuery();
        }

        #endregion

    }

}

