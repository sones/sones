using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;
using sones.Plugins.SonesGQL.Function.ErrorHandling;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;

namespace TypesConnect
{
    public class TypeWithProperty
    {
        public IVertexType type { get; set; }
        public IPropertyDefinition propertyDifinition { get; set; }


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
                    value.propertyDifinition = property;
                    value.type = typeVertex;

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
