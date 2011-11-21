/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;
using sones.Plugins.SonesGQL.Function.ErrorHandling;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;

namespace sones.Plugins.SonesGQL.Functions.TypesConnect
{
    public class TypeWithProperty
    {
        public IVertexType Type { get; set; }
        public IPropertyDefinition PropertyDefinition { get; set; }


        #region StringParser

        public List<TypeWithProperty> StringParser(String current_string, IGraphDB myDB, SecurityToken mySecurityToken, Int64 myTransactionToken)
        {
            current_string = current_string.Replace(" ","");

            List<TypeWithProperty> list = new List<TypeWithProperty>();
            bool endFlag = false;
            int EndPos = 0;

                do
                {

                    EndPos = current_string.IndexOf(',');
                    if (EndPos == -1)
                    {
                        EndPos = current_string.Length;
                        endFlag = true;
                    }

                    var typeVertexString = current_string.Substring(0, current_string.IndexOf('.'));


                    IVertexType typeVertex = null;

                    try
                    {

                        typeVertex = myDB.GetVertexType<IVertexType>(
                                                         mySecurityToken,
                                                         myTransactionToken,
                                                         new sones.GraphDB.Request.RequestGetVertexType(typeVertexString),
                                                         (statistics, type) => type);
                    }
                    catch
                    {
                        throw new InvalidFunctionParameterException("edgeType", "Object reference not set to an instance of an object.", "null");
                    }
                    var propertyIDString = current_string.Substring(current_string.IndexOf('.') + 1, EndPos - current_string.IndexOf('.') - 1);
                    var property = typeVertex.GetPropertyDefinition(propertyIDString);
                     if (property==null)
                         throw new InvalidFunctionParameterException("Property", "Property: " + propertyIDString + " not exist in VertexType:" + typeVertexString, "null");
                    TypeWithProperty value = new TypeWithProperty();
                    value.PropertyDefinition = property;
                    value.Type = typeVertex;

                    if (!list.Contains(value))
                        list.Add(value);

                    if (!endFlag)
                        current_string = current_string.Substring(EndPos + 1);

                }
                while (endFlag != true);
 
            return list;
        }
    }
    #endregion
}
