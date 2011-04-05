using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.TypeManagement.BaseTypes
{
    /// <summary>
    /// This class is base class for all predefined vertex and edge types.
    /// </summary>
    internal abstract class TypeBase
    {
        #region Data

        private readonly IEnumerable<IAttributeDefinition> _attributes;
        private readonly Dictionary<String, IPropertyDefinition> _properties;
        private readonly Dictionary<String, IOutgoingEdgeDefinition> _outgoing;
        private readonly Dictionary<String, IIncomingEdgeDefinition> _incoming;

        #endregion

        #region c'tor

        protected TypeBase(IEnumerable<IAttributeDefinition> myAttributes)
        {
            _attributes = myAttributes;
            _properties   = _attributes.OfType<IPropertyDefinition>().ToDictionary(prop => prop.Name);
            _outgoing = _attributes.OfType<IOutgoingEdgeDefinition>().ToDictionary(edge => edge.Name);
            _incoming = _attributes.OfType<IIncomingEdgeDefinition>().ToDictionary(edge => edge.Name);
        }

        #endregion

        #region protected methods

        protected IEnumerable<IAttributeDefinition> GetAttributeDefinitions()
        {
            return _attributes;
        }

        protected IEnumerable<IPropertyDefinition> GetPropertyDefinitions()
        {
            return _properties.Values;
        }

        protected IIncomingEdgeDefinition GetIncomingEdgeDefinition(string myEdgeName)
        {
            IIncomingEdgeDefinition result;
            _incoming.TryGetValue(myEdgeName, out result);
            return result;
        }

        protected IEnumerable<IIncomingEdgeDefinition> GetIncomingEdgeDefinitions()
        {
            return _incoming.Values;
        }

        protected IOutgoingEdgeDefinition GetOutgoingEdgeDefinition(string myEdgeName)
        {
            IOutgoingEdgeDefinition result;
            _outgoing.TryGetValue(myEdgeName, out result);
            return result;
        }

        protected IEnumerable<IOutgoingEdgeDefinition> GetOutgoingEdgeDefinitions()
        {
            return _outgoing.Values;
        }

        #endregion
    }
}