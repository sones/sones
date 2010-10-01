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
 * AlterType_AddAttributes
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;

using sones.GraphDB.Errors;
using sones.GraphDB.NewAPI;
using sones.GraphDB.Result;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.Structures.EdgeTypes;

using sones.Lib;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphDB.Managers.AlterType
{

    public class AlterType_AddAttributes : AAlterTypeCommand
    {

        private List<AttributeDefinition>    _ListOfAttributes;
        private List<BackwardEdgeDefinition> _BackwardEdgeInformation;

        public AlterType_AddAttributes(List<AttributeDefinition> listOfAttributes, List<BackwardEdgeDefinition> backwardEdgeInformation)
        {
            _ListOfAttributes = listOfAttributes;
            _BackwardEdgeInformation = backwardEdgeInformation;
        }

        public override TypesOfAlterCmd AlterType
        {
            get { return TypesOfAlterCmd.Add; }
        }

        /// <summary>
        /// Adds myAttributes to a certain graphDBType.
        /// </summary>
        /// <param name="myGraphDBType">The type that should be added some myAttributes.</param>
        /// <param name="myDBContext">The DBContext.</param>
        /// <returns>A Exceptional.</returns>
        public override Exceptional Execute(DBContext myDBContext, GraphDBType myGraphDBType)
        {

            #region check type for myAttributes

            foreach (var aAttributeDefinition in _ListOfAttributes)
            {
                if (myGraphDBType.GetTypeAttributeByName(aAttributeDefinition.AttributeName) != null)
                {
                    var aError = new Error_AttributeAlreadyExists(aAttributeDefinition.AttributeName);

                    return new Exceptional(aError);
                }
            }

            #endregion

            #region add myAttributes

            foreach (var aAttributeDefinition in _ListOfAttributes)
            {

                var typeAttributeExceptional = aAttributeDefinition.CreateTypeAttribute(myDBContext);
                if (typeAttributeExceptional.Failed())
                {
                    return typeAttributeExceptional;
                }
                var typeAttribute = typeAttributeExceptional.Value;

                try
                {
                    var theType = myDBContext.DBTypeManager.GetTypeByName(aAttributeDefinition.AttributeType.Name);
                    typeAttribute.DBTypeUUID = theType.UUID;
                    typeAttribute.RelatedGraphDBTypeUUID = myGraphDBType.UUID;

                    #region EdgeType
                    if (typeAttribute.EdgeType == null) // should never happen - delete me!
                    {
                        #region we had not defined a special EdgeType - for single reference attributes we need to set the EdgeTypeSingle NOW!
                        if (typeAttribute.KindOfType == KindsOfType.SingleReference)
                            typeAttribute.EdgeType = new EdgeTypeSingleReference(null, theType.UUID);

                        if (typeAttribute.KindOfType == KindsOfType.ListOfNoneReferences)
                            typeAttribute.EdgeType = new EdgeTypeListOfBaseObjects();

                        if (typeAttribute.KindOfType == KindsOfType.SetOfReferences || typeAttribute.KindOfType == KindsOfType.SetOfNoneReferences)
                            typeAttribute.EdgeType = new EdgeTypeSetOfReferences(null, theType.UUID);

                        #endregion
                    }
                    #endregion

                    var aTempResult = myDBContext.DBTypeManager.AddAttributeToType(myGraphDBType, typeAttribute);

                    if (!aTempResult.Success())
                    {
                        return aTempResult;
                    }
                }
                catch (Exception e)
                {
                    var aError = new Error_UnknownDBError(e);

                    return new Exceptional(aError);
                }
            }

            #endregion


            #region add BackwardEdges

            if (_BackwardEdgeInformation != null)
            {
                foreach (var beDef in _BackwardEdgeInformation)
                {
                    //GraphDBType edgeType = typeManager.GetTypeByName(tuple.TypeName);
                    //TypeAttribute edgeAttribute = edgeType.GetTypeAttributeByName(tuple.TypeAttributeName);

                    var typeAttribute = myDBContext.DBTypeManager.CreateBackwardEdgeAttribute(beDef, myGraphDBType);

                    if (typeAttribute.Failed())
                    {
                        return new Exceptional(typeAttribute);
                    }

                    // TODO: go through all DB Objects of the _TypeName and change the implicit backward edge type to the new ta.EdgeType
                    //       if the DBObject has one!

                    try
                    {
                        var aTempResult = myDBContext.DBTypeManager.AddAttributeToType(myGraphDBType, typeAttribute.Value);

                        if (aTempResult.Failed())
                        {
                            return aTempResult;
                        }
                    }
                    catch (Exception e)
                    {
                        return new Exceptional(new Error_UnknownDBError(e));
                    }
                }
            }

            #endregion

            return Exceptional.OK;

        }

        public override IEnumerable<Vertex> CreateVertex(DBContext myDBContext, GraphDBType myGraphDBType)
        {

            #region generate result

            var payload           = new Dictionary<String, Object>();
            var attributeVertices = new List<Vertex>();
            payload.Add("TYPE", myGraphDBType.Name);

            if (_BackwardEdgeInformation.IsNotNullOrEmpty())
            {

                #region backwardEdge

                payload.Add("ACTION", "ADD BACKWARDEDGES");

                foreach (var aAddedBackwardEdge in _BackwardEdgeInformation)
                {

                    var payloadPerBackwardEdge = new Dictionary<string, object>();
                    var type = myDBContext.DBTypeManager.GetTypeByName(aAddedBackwardEdge.TypeName);

                    payloadPerBackwardEdge.Add("NAME", aAddedBackwardEdge.AttributeName);
                    payloadPerBackwardEdge.Add("TYPE", type);
                    payloadPerBackwardEdge.Add("ATTRIBUTE", type.GetTypeAttributeByName(aAddedBackwardEdge.TypeAttributeName));

                    attributeVertices.Add(new Vertex(payloadPerBackwardEdge));

                }

                payload.Add("BACKWARDEDGES", new Edge(null, attributeVertices) { EdgeTypeName = "BACKWARDEDGE" });

                #endregion

            }
            else
            {

                #region attributes

                payload.Add("ACTION", "ADD ATTRIBUTES");

                foreach (var aAddedAttribute in _ListOfAttributes)
                {
                    var payloadPerAttribute = new Dictionary<String, Object>();

                    payloadPerAttribute.Add("NAME", aAddedAttribute.AttributeName);
                    payloadPerAttribute.Add("TYPE", myDBContext.DBTypeManager.GetTypeByName(aAddedAttribute.AttributeType.Name));

                    attributeVertices.Add(new Vertex(payloadPerAttribute));
                }

                payload.Add("ATTRIBUTES", new Edge(null, attributeVertices) { EdgeTypeName = "ATTRIBUTE" });

                #endregion

            }

            #endregion

            return new List<Vertex> { new Vertex(payload) };

        }

    }

}
