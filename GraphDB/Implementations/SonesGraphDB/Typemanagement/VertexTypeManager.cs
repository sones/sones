using System.Collections.Generic;
using sones.GraphDB.Request;
using sones.GraphDB.TypeSystem;
using sones.Library.LanguageExtensions;
using sones.Library.Transaction;

namespace sones.GraphDB.Manager.Typemanagement
{
    public sealed partial class TypeManager
    {
        #region Data

        private VertexTypeManager _VertexManager;

        #endregion

        #region VertexTypeManager

        private sealed class VertexTypeManager
        {
            #region public methods

            public IVertexType Get(string myTypeName)
            {
                throw new System.NotImplementedException();
            }

            public void Add(VertexTypeDefinition myVertexTypeDefinition, TransactionToken myTransaction)
            {
                //throws, if definitions fail
                CheckAdd(myVertexTypeDefinition.SingleEnumerable());

                //add type with private method
                throw new System.NotImplementedException();
            }

            public void Add(IEnumerable<VertexTypeDefinition> myVertexTypeDefinitions, TransactionToken myTransaction)
            {
                //throws, if definitions fail
                CheckAdd(myVertexTypeDefinitions);

                //add each type with private method
                throw new System.NotImplementedException();
            }

            public void Remove(IVertexType myVertexType, TransactionToken myTransaction)
            {
                //throws, if definitions fail
                CheckRemove(myVertexType.SingleEnumerable());

                //remove type with private method
                throw new System.NotImplementedException();
            }

            public void Remove(IEnumerable<IVertexType> myVertexTypes, TransactionToken myTransaction)
            {
                //throws, if definitions fail
                CheckRemove(myVertexTypes);

                //remove type with private method
                throw new System.NotImplementedException();
            }

            public void Update(VertexTypeDefinition myVertexTypeDefinition, TransactionToken myTransaction)
            {
                //throws, if definitions fail
                CheckUpdate(myVertexTypeDefinition.SingleEnumerable());

                //update each type with private method
                throw new System.NotImplementedException();
            }

            public void Update(IEnumerable<VertexTypeDefinition> myVertexTypeDefinitions, TransactionToken myTransaction)
            {
                //throws, if definitions fail
                CheckUpdate(myVertexTypeDefinitions);

                //update type with private method
                throw new System.NotImplementedException();
            }

            #endregion


            #region private methods

            /// <summary>
            /// Checks if a bunch of vertex type definitions, can be added faultless.
            /// </summary>
            /// <param name="myVertexTypeDefinitions">
            /// The vertex type definitions to be checked.
            /// </param>
            private void CheckAdd(IEnumerable<VertexTypeDefinition> myVertexTypeDefinitions)
            {
                throw new System.NotImplementedException();
            }

            /// <summary>
            /// Checks if a bunch of vertex types, can be removed faultless.
            /// </summary>
            /// <param name="myVertexTypeDefinitions">
            /// The vertex types to be removed.
            /// </param>
            private void CheckRemove(IEnumerable<IVertexType> myVertexTypes)
            {
                throw new System.NotImplementedException();
            }

            /// <summary>
            /// Checks if a bunch of vertex type definitions, can update existing vertex types
            /// </summary>
            /// <param name="myVertexTypeDefinitions"></param>
            private void CheckUpdate(IEnumerable<VertexTypeDefinition> myVertexTypeDefinitions)
            {
                throw new System.NotImplementedException();
            }

            #endregion

        }

        #endregion

    }

}
