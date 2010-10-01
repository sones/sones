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

/* <id name="GraphDB – AttrDefaultValue Node" />
 * <copyright file="AttrDefaultValueNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Dirk Bludau</developer>
 * <summary>Contains the default value for an attribute.</summary>
 */

#region usings

using System.Linq;

using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.GraphQL.StructureNodes;

using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.BasicTypes;

using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.TypeManagement;


#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    public class AttrDefaultValueNode : AStructureNode
    {

        #region Properties

        public IObject Value { get; private set; }
        
        #endregion

        #region constructors

        public AttrDefaultValueNode()
        { }

        #endregion

        #region private helper methods

        /// <summary>
        /// check for the correct type of each list item
        /// </summary>
        /// <param name="myObjects">current object in list</param>
        /// <param name="myLastObject">last object in list</param>
        /// <returns>an ADBBaseObject</returns>
        private ADBBaseObject CheckTypeOfItems(object myObjects, ADBBaseObject myLastObject)
        {
            var baseObject = GraphDBTypeMapper.GetBaseObjectFromCSharpType(myObjects);

            if (baseObject.Type == myLastObject.Type)
            {
                return baseObject;
            }
            else
            {
                throw new GraphDBException(new Error_InvalidAttrDefaultValueAssignment(baseObject.ObjectName, myLastObject.ObjectName));
            }
        }

        #endregion

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            if (parseNode.HasChildNodes())
            {

                if (parseNode.ChildNodes.Count >= 3)
                {
                    IBaseEdge ListOfDefaults;
                    var firstListObject = GraphDBTypeMapper.GetBaseObjectFromCSharpType(parseNode.ChildNodes[2].ChildNodes[0].Token.Value);
                    

                    if (parseNode.ChildNodes[1].Token.Text.ToUpper() == DBConstants.SETOF)
                    {
                        ListOfDefaults = new EdgeTypeSetOfBaseObjects();
                    }

                    else if (parseNode.ChildNodes[1].Token.Text.ToUpper() == DBConstants.LISTOF)
                    {
                        ListOfDefaults = new EdgeTypeListOfBaseObjects();
                    }

                    else
                    {
                        throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                    }

                    ListOfDefaults.AddRange(parseNode.ChildNodes[2].ChildNodes.Select(item => CheckTypeOfItems(item.Token.Value, firstListObject)));

                    Value = ListOfDefaults;
                }
                else
                {
                    
                    var baseObject = GraphDBTypeMapper.GetBaseObjectFromCSharpType(parseNode.ChildNodes[1].Token.Value);
                    Value = (IObject)baseObject;
                }

            }

            else
            {
                Value = null;
            }

        }

    }

}
