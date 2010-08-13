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
using sones.GraphDB.Structures.Result;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.BasicTypes;

using sones.Lib.Frameworks.Irony.Parsing;

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
