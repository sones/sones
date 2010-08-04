/*
 * sones GraphDS API - CreateEdgeQuery
 * (c) Achim 'ahzf' Friedland, 2010
 */

#region Usings

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using sones.GraphDB.Structures;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDS.API.CSharp.Reflection;
using sones.Lib;

#endregion

namespace sones.GraphDS.API.CSharp
{

    public class CreateEdgeQuery : CreateTypeQuery
    {

        #region Data


        private String       _ExtendsString         = null;
        private String       _AbstractString        = null;
        private List<String> _AttributeStrings      = new List<String>();
        private List<String> _MandatoryStrings      = new List<String>();
        private String       _CommentString         = null;

        #endregion

        #region Properties

        public String        SubjectType     { get; private set; }
        public String        ObjectType      { get; private set; }

        public Boolean       IsHypeEdge      { get; private set; }
        public Boolean       IsAbstract      { get; private set; }

        #endregion

        #region Constructor(s)

        #region CreateEdgeQuery(myAGraphDSSharp, myTypeName, bidirectional, hyperEdge, abstractEdge)

        public CreateEdgeQuery(AGraphDSSharp myAGraphDSSharp, String myTypeName, Boolean hyperEdge = false, Boolean abstractEdge = false)
            : base(myAGraphDSSharp)
        {

            Name            = myTypeName;
            IsHypeEdge      = hyperEdge;
            IsAbstract      = abstractEdge;

            if (abstractEdge)
                _AbstractString = "ABSTRACT ";

        }

        #endregion

        #endregion


        #region Extends(myBaseTypeName)

        public CreateEdgeQuery Extends(String myBaseTypeName)
        {
            _ExtendsString = " EXTENDS " + myBaseTypeName;
            return this;
        }

        #endregion

        #region Extends(myBaseTypeName)

        public CreateEdgeQuery Extends(CreateEdgeQuery myBaseGraphDBType)
        {
            _ExtendsString = " EXTENDS " + myBaseGraphDBType.Name;
            return this;
        }

        #endregion


        #region From(myCreateVertexQuery)

        public CreateEdgeQuery From(CreateVertexQuery myCreateVertexQuery)
        {

            if (myCreateVertexQuery != null)
                SubjectType = myCreateVertexQuery.Name;

            return this;

        }

        #endregion

        #region To(myCreateVertexQuery)

        public CreateEdgeQuery To(CreateVertexQuery myCreateVertexQuery)
        {

            if (myCreateVertexQuery != null)
                ObjectType = myCreateVertexQuery.Name;

            return this;

        }

        #endregion

        #region ToMultiple(myCreateVertexQuery)

        public CreateEdgeQuery ToMultiple(CreateVertexQuery myCreateVertexQuery)
        {

            if (myCreateVertexQuery != null)
                ObjectType = myCreateVertexQuery.Name;

            IsHypeEdge = true;

            return this;

        }

        #endregion


        #region AddAttributes(...)

        // Use "mandatory" and "hyperEdge", but not "myMandatory" and "myHyperEdge" for a more 'fluent' interface!

        public CreateEdgeQuery AddAttribute(String myAttributeType, String myAttributeName, Object defaultValue = null, Boolean hyperEdge = false, Boolean mandatory = false, Boolean unique = false, DBIndexTypes? index = null)
        {

            if (hyperEdge)
                _AttributeStrings.Add("SET<" + myAttributeType + "> " + myAttributeName);
            else
                _AttributeStrings.Add(myAttributeType + " " + myAttributeName);
            
            if (defaultValue != null)
                _AttributeStrings[_AttributeStrings.Count - 1] += ("=" + defaultValue.ToString());

            if (mandatory)
                _MandatoryStrings.Add(myAttributeName);

            return this;

        }

        public CreateEdgeQuery AddAttribute(CreateEdgeQuery myAttributeType, String myAttributeName, Object defaultValue = null, Boolean hyperEdge = false, Boolean mandatory = false, Boolean unique = false, DBIndexTypes? index = null)
        {

            if (hyperEdge)
                _AttributeStrings.Add("SET<" + myAttributeType.Name + "> " + myAttributeName);
            else
                _AttributeStrings.Add(myAttributeType.Name + " " + myAttributeName);

            if (defaultValue != null)
                _AttributeStrings[_AttributeStrings.Count - 1] += ("=" + defaultValue.ToString());

            if (mandatory)
                _MandatoryStrings.Add(myAttributeName);            

            return this;

        }

        public CreateEdgeQuery AddAttributeT<T>(String myAttributeName, T defaultValue = default(T), Boolean hyperEdge = false, Boolean mandatory = false, Boolean unique = false, DBIndexTypes? index = null)
        {

            _AttributeStrings.Add(typeof(T).Name + " " + myAttributeName);

            if (!EqualityComparer<T>.Default.Equals(defaultValue, default(T)))
                _AttributeStrings[_AttributeStrings.Count - 1] += ("=" + defaultValue.ToString());

            if (mandatory)
                _MandatoryStrings.Add(myAttributeName);
            
            return this;
        }

        public CreateEdgeQuery AddLoop(String myAttributeName, Boolean hyperEdge = false, Boolean mandatory = false, Boolean unique = false, DBIndexTypes? index = null)
        {

            if (hyperEdge)
                _AttributeStrings.Add("SET<" + Name + "> " + myAttributeName);
            else
                _AttributeStrings.Add(Name + " " + myAttributeName);

            if (mandatory)
                _MandatoryStrings.Add(myAttributeName);            

            return this;

        }

        public CreateEdgeQuery AddInteger(String myAttributeName, Object defaultValue = null, Boolean mandatory = false, Boolean unique = false, DBIndexTypes? index = null)
        {   

            _AttributeStrings.Add("Integer " + " " + myAttributeName);

            if (defaultValue != null)
                _AttributeStrings[_AttributeStrings.Count - 1] += ("=" + defaultValue.ToString());

            if (mandatory)
                _MandatoryStrings.Add(myAttributeName);

            return this;

        }

        public CreateEdgeQuery AddString(String myAttributeName, Object defaultValue = null, Boolean mandatory = false, Boolean unique = false, DBIndexTypes? index = null)
        {   
            
            _AttributeStrings.Add("String " + " " + myAttributeName);

            if (defaultValue != null)
                _AttributeStrings[_AttributeStrings.Count - 1] += ("='" + defaultValue.ToString() + "'");

            if (mandatory)
                _MandatoryStrings.Add(myAttributeName);

            return this;

        }

        public CreateEdgeQuery AddDateTime(String myAttributeName, Object defaultValue = null, Boolean mandatory = false, Boolean unique = false, DBIndexTypes? index = null)
        {

            _AttributeStrings.Add("DateTime " + " " + myAttributeName);

            if (defaultValue != null)
                _AttributeStrings[_AttributeStrings.Count - 1] += ("=" + defaultValue.ToString());

            if (mandatory)
                _MandatoryStrings.Add(myAttributeName);

            return this;

        }

        #endregion

        #region Comment

        public CreateEdgeQuery Comment(String myComment)
        {

            if (myComment != null)
                _CommentString = " COMMENT = '" + myComment + "'";

            if (myComment == "")
                _CommentString = null;

            return this;

        }

        #endregion


        #region Execute2(myAction)

        public CreateEdgeQuery Execute2(Action<QueryResult> myAction)
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
            
            _CommandString.Append("EDGE ").Append(Name);

            // EXTENDS
            if (_ExtendsString != null)
                _CommandString.Append(_ExtendsString);

            AddToCommandString(_AttributeStrings,       "ATTRIBUTES");
            AddToCommandString(_MandatoryStrings,       "MANDATORY");

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

