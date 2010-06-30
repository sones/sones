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


/* <id name="sones GraphDB – interesting attributes" />
 * <copyright file="InterestingAttributes.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <developer>Stefan Licht</developer>
 * <summary></summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;

using sones.GraphDB.TypeManagement;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;

using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Statements.Select;
using sones.GraphFS.Session;
using sones.Lib.Session;
using sones.GraphDB.TypeManagement.SpecialTypeAttributes;
using sones.GraphDB.Settings;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalClasses.Statements
{

    public class InterestingAttribute
    {
        AStructureNode _InterestingNode;
        public AStructureNode InterestingNode
        {
            get { return _InterestingNode; }
            set { _InterestingNode = value; }
        }

        AttributeUUID _AttributeUUID;
        public AttributeUUID AttributeUUID
        {
            get { return _AttributeUUID; }
            set { _AttributeUUID = value; }
        }

        IDNode _AttributeIDNode;
        public IDNode AttributeIDNode
        {
            get { return _AttributeIDNode; }
            set 
            { 
                _AttributeIDNode = value;

                _level = _AttributeIDNode.Level;
            }
        }

        String _Alias;
        public String Alias
        {
            get {
                if (_Alias != null)
                    return _Alias;
                else
                    return _AttributeIDNode.LastAttribute.Name;
            }
            set { _Alias = value; }
        }

        private int _level = 0;
        public int Level
        {
            get 
            {
                return _level;
            }
            set
            {
                _level = value;
            }
        }

        /// <summary>
        /// If we do not have a IDNode this will contains the selected AttrName
        /// </summary>
        String _AttributeName;
        public String AttributeName
        {
            get {
                if (_AttributeName != null)
                {
                    return _AttributeName;
                }
                else
                {
                    if (_AttributeIDNode.LastAttribute != null)
                    {
                        _AttributeName = _AttributeIDNode.LastAttribute.Name;

                        return _AttributeIDNode.LastAttribute.Name;
                    }
                    else
                    {
                        _AttributeName = SpecialTypeAttribute_UUID.AttributeName;
                        return _AttributeName;
                    }
                }  
            }
            set { _AttributeName = value; }
        }

        private Int64 _Depth;
        public Int64 Depth
        {
            get { return _Depth; }
            set { _Depth = value; }
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public class InterestingAttributes
    {

        #region Data

        List<InterestingAttribute> _aggreateNodesOfListReferences = new List<InterestingAttribute>();
        public List<InterestingAttribute> AggreateNodesOfListReferences
        {
            get { return _aggreateNodesOfListReferences; }
        }
        
        List<InterestingAttribute> _aggregateNodes = new List<InterestingAttribute>();
        public List<InterestingAttribute> AggregateNodes
        {
            get { return _aggregateNodes; }
        }
       
        List<InterestingAttribute> _funcCallNode = new List<InterestingAttribute>();
        public List<InterestingAttribute> FuncCallNode
        {
            get { return _funcCallNode; }
        }

        List<InterestingAttribute> _groupNodes = new List<InterestingAttribute>();
        public List<InterestingAttribute> GroupNodes
        {
            get { return _groupNodes; }
        }

        List<InterestingAttribute> _interestingAttribts = new List<InterestingAttribute>();
        /// <summary>
        /// usual selected attributes
        /// </summary>
        public List<InterestingAttribute> InterestingAttribts
        {
            get { return _interestingAttribts; }
        }

        Boolean _IsAsteriskSelection = false;
        public Boolean IsAsteriskSelection
        {
            get { return _IsAsteriskSelection; }
            set { _IsAsteriskSelection = value; }
        }

        private Int32 _MaxLevel;
        public Int32 MaxLevel
        {
            get { return _MaxLevel; }
        }

        SessionSettings _SessionToken;

        SelectSettingCache _SelectSettingCache;

        #endregion

        #region constructor

        public InterestingAttributes(SessionSettings myToken, SelectSettingCache myCache)
        {
            _interestingAttribts = new List<InterestingAttribute>();
            _SessionToken = myToken;
            _SelectSettingCache = myCache;
        }

        public InterestingAttributes(IDNode IDOfAttribute, String AliasOfAttribute, SelectSettingCache myCache, DBContext context)
        {
            AddInterestingAttribute(IDOfAttribute, AliasOfAttribute, context); 
            _SelectSettingCache = myCache;
        }

        #endregion

        #region public methods

        public void AddAggregateOfListReference(AggregateNode myAggregateNode, String myAlias, DBContext context)
        {
            InterestingAttribute _aInterestingAttribute = new InterestingAttribute();
            _aInterestingAttribute.Alias = myAlias;
            _aInterestingAttribute.AttributeIDNode = myAggregateNode.Expressions[0] as IDNode;
            _aInterestingAttribute.AttributeUUID = _aInterestingAttribute.AttributeIDNode.LastAttribute.UUID;
            _aInterestingAttribute.InterestingNode = myAggregateNode;

            ExtractDepth(ref _aInterestingAttribute, _aInterestingAttribute.AttributeIDNode, context);

            _MaxLevel = Math.Max(_MaxLevel, _aInterestingAttribute.Level);

            _aggreateNodesOfListReferences.Add(_aInterestingAttribute);
        }

        private void ExtractDepth(ref InterestingAttribute _aInterestingAttribute, IDNode myIDNode, DBContext context)
        {
            if (myIDNode.LastAttribute != null)
            {
                if (myIDNode.LastAttribute.KindOfType != KindsOfType.SpecialAttribute)
                {
                    var depthValue = _SelectSettingCache.GetValue(myIDNode.LastAttribute.GetRelatedType(context.DBTypeManager), myIDNode.LastAttribute, SettingDepth.UUID, context).Value;

                    if (depthValue is Int64)
                    {
                        _aInterestingAttribute.Depth = (Int64)depthValue;
                    }
                }
            }
        }

        public void AddAggregate(AggregateNode myAggregateNode, String myAlias, DBContext context)
        {
            InterestingAttribute _aInterestingAttribute = new InterestingAttribute();
            _aInterestingAttribute.Alias = myAlias;
            if (myAggregateNode.Expressions[0] is IDNode)
            {
                _aInterestingAttribute.AttributeIDNode = (myAggregateNode.Expressions[0] as IDNode);
            }
            else
            {
                _aInterestingAttribute.AttributeIDNode = (myAggregateNode.Expressions[0] as BinaryExpressionNode).ContainingIDNodes.First();
            }
            _aInterestingAttribute.InterestingNode = myAggregateNode;

            ExtractDepth(ref _aInterestingAttribute, _aInterestingAttribute.AttributeIDNode, context);

            _MaxLevel = Math.Max(_MaxLevel, _aInterestingAttribute.Level);

            _aggregateNodes.Add(_aInterestingAttribute);
        }

        public void AddGroupNode(IDNode myGroupIDNode, DBContext context)
        {
            InterestingAttribute _aInterestingAttribute = new InterestingAttribute();
            _aInterestingAttribute.AttributeIDNode = myGroupIDNode;
            _aInterestingAttribute.AttributeUUID = myGroupIDNode.LastAttribute.UUID;

            ExtractDepth(ref _aInterestingAttribute, myGroupIDNode, context);

            _groupNodes.Add(_aInterestingAttribute);
        }

        public void AddFunction(FuncCallNode myFuncCallNode, String myAlias, DBContext context)
        {
            InterestingAttribute _aInterestingAttribute = new InterestingAttribute();
            _aInterestingAttribute.Alias = myAlias;
            _aInterestingAttribute.InterestingNode = myFuncCallNode;
            IDNode tempIDNode;

            foreach (Object expr in myFuncCallNode.Expressions)
            {
                if (expr != null && expr is IDNode)
                {
                    tempIDNode = (IDNode)expr;

                    _MaxLevel = Math.Max(_MaxLevel, tempIDNode.Level);

                    _aInterestingAttribute.Level = tempIDNode.Level;

                    ExtractDepth(ref _aInterestingAttribute, tempIDNode, context);
                }
            }

            _funcCallNode.Add(_aInterestingAttribute);
        }

        public void AddInterestingAttribute(IDNode aIDNode, String AliasOfAttribute, DBContext context)
        {
            InterestingAttribute _aInterestingAttribute = new InterestingAttribute();
            _aInterestingAttribute.Alias = AliasOfAttribute;
            _aInterestingAttribute.AttributeIDNode = aIDNode;
            _aInterestingAttribute.AttributeUUID = aIDNode.LastAttribute.UUID;
            _aInterestingAttribute.Level = aIDNode.Level;

            ExtractDepth(ref _aInterestingAttribute, aIDNode, context);

            _MaxLevel = Math.Max(_MaxLevel, _aInterestingAttribute.Level);

            _interestingAttribts.Add(_aInterestingAttribute);
        }

        #endregion

    }
}
