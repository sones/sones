using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphDSRemoteClient.sonesGraphDSRemoteAPI;

namespace GraphDSRemoteClient.GraphElements
{
    internal abstract class ARemoteGraphElement
    {
        #region Data

        private IServiceToken _ServiceToken;

        #endregion


        #region Getter / Setter

        public abstract String Comment { get; }

        public abstract long CreationDate { get; }

        public abstract long ModificationDate { get; }

        public abstract IDictionary<Int64, IComparable> StructuredProperties { get; }

        public abstract IDictionary<String, Object> UnstructuredProperties { get; }

        #endregion


        #region Constructor

        internal ARemoteGraphElement(IServiceToken myServiceToken)
        {
            _ServiceToken = myServiceToken;
        }

        #endregion
    }
}
