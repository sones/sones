using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;
using sones.Library.Commons.Transaction;
using sones.Library.Commons.Security;
using sones.GraphDB.Request;

namespace sones.GraphDB.Manager.TypeManagement
{
    internal class ExecuteEdgeTypeManager: IEdgeTypeHandler
    {
        private IDictionary<string, IVertexType> _baseTypes = new Dictionary<String, IVertexType>();
        

        #region IEdgeTypeManager Members

        IEdgeType IEdgeTypeHandler.GetEdgeType(long myTypeId, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            //#region get static types

            //if (Enum.IsDefined(typeof(BaseTypes), myTypeId) && _baseTypes.ContainsKey(((BaseTypes)myTypeId).ToString()))
            //{
            //    return _baseTypes[((BaseTypes)myTypeId).ToString()];
            //}

            //#endregion


            //#region get from fs

            //var vertex = Get(myTypeId, myTransaction, mySecurity);

            //if (vertex == null)
            //    throw new KeyNotFoundException(string.Format("A vertex type with name {0} was not found.", myTypeId));

            //return new EdgeType(vertex);

            //#endregion
            throw new NotImplementedException();

        }

        IEdgeType IEdgeTypeHandler.GetEdgeType(string myTypeName, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            //if (String.IsNullOrWhiteSpace(myTypeName))
            //    throw new ErrorHandling.EmptyEdgeTypeNameException();

            //#region get static types

            //if (_baseTypes.ContainsKey(myTypeName))
            //{
            //    return _baseTypes[myTypeName];
            //}

            //#endregion

            //#region get from fs

            //var vertex = Get(myTypeName, myTransaction, mySecurity);

            //if (vertex == null)
            //    throw new KeyNotFoundException(string.Format("A vertex type with name {0} was not found.", myTypeName));

            //return new VertexType(vertex);

            //#endregion
            throw new NotImplementedException();

        }

        IEnumerable<IEdgeType> IEdgeTypeHandler.GetAllEdgeTypes(TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        IEdgeType IEdgeTypeHandler.AddEdgeType(IEnumerable<EdgeTypePredefinition> myEdgeTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        void IEdgeTypeHandler.RemoveEdgeTypes(IEnumerable<IEdgeType> myEdgeTypes, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        void IEdgeTypeHandler.UpdateEdgeType(IEnumerable<EdgeTypePredefinition> myEdgeTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IManager Members

        void IManager.Initialize(IMetaManager myMetaManager)
        {
        }

        void IManager.Load(TransactionToken myTransaction, SecurityToken mySecurity)
        {
        }

        #endregion
    }
}
