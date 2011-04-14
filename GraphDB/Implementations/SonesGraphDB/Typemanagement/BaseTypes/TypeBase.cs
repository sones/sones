using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.TypeManagement.BaseTypes
{
    /// <summary>
    /// This class is base class for all predefined parentVertex and IncomingEdge types.
    /// </summary>
    internal abstract class TypeBase
    {
        #region Data

        private readonly Dictionary<String, IAttributeDefinition> _attributes;
        private readonly IEnumerable<IAttributeDefinition> _ownAttributes;
        private readonly IEnumerable<IPropertyDefinition> _ownProperties;
        private readonly IEnumerable<IOutgoingEdgeDefinition> _ownOutgoing;
        private readonly IEnumerable<IIncomingEdgeDefinition> _ownIncoming;
        private readonly IEnumerable<IAttributeDefinition> _allAttributes;
        private readonly IEnumerable<IPropertyDefinition> _allProperties;
        private readonly IEnumerable<IOutgoingEdgeDefinition> _allOutgoing;
        private readonly IEnumerable<IIncomingEdgeDefinition> _allIncoming;
        private readonly bool _ownHasAttributes;
        private readonly bool _ownHasProperties;
        private readonly bool _ownHasOutgoing;
        private readonly bool _ownHasIncoming;
        private readonly bool _allHasAttributes;
        private readonly bool _allHasProperties;
        private readonly bool _allHasOutgoing;
        private readonly bool _allHasIncoming;



        #endregion

        #region c'tor

        protected TypeBase(IEnumerable<IAttributeDefinition> myAttributes, IEnumerable<IAttributeDefinition> myAncestorAttributes)
        {
            _ownAttributes = myAttributes;
            _allAttributes = Enumerable.Union(myAttributes, myAncestorAttributes);

            _attributes = _allAttributes.ToDictionary(att => att.Name);

            _ownProperties = _ownAttributes.OfType<IPropertyDefinition>().ToArray();
            _ownOutgoing   = _ownAttributes.OfType<IOutgoingEdgeDefinition>().ToArray();
            _ownIncoming   = _ownAttributes.OfType<IIncomingEdgeDefinition>().ToArray();

            _allProperties = _allAttributes.OfType<IPropertyDefinition>().ToArray();
            _allOutgoing   = _allAttributes.OfType<IOutgoingEdgeDefinition>().ToArray();
            _allIncoming   = _allAttributes.OfType<IIncomingEdgeDefinition>().ToArray();


            _ownHasAttributes = _ownAttributes.Count() > 0;
            _ownHasProperties = _ownProperties.Count() > 0;
            _ownHasOutgoing   = _ownOutgoing.Count() > 0;
            _ownHasIncoming   = _ownIncoming.Count() > 0;
            _allHasAttributes = _allAttributes.Count() > 0;
            _allHasProperties = _allProperties.Count() > 0;
            _allHasOutgoing   = _allOutgoing.Count() > 0;
            _allHasIncoming   = _allIncoming.Count() > 0;

        }

        #endregion

        #region protected methods

        protected IEnumerable<IAttributeDefinition> GetAttributeDefinitions(bool myIncludeAncestor)
        {
            return (myIncludeAncestor)? _allAttributes : _ownAttributes;
        }

        protected IEnumerable<IPropertyDefinition> GetPropertyDefinitions(bool myIncludeAncestor)
        {
            return (myIncludeAncestor) ? _allProperties : _ownProperties;
        }

        protected IEnumerable<IIncomingEdgeDefinition> GetIncomingEdgeDefinitions(bool myIncludeAncestor)
        {
            return (myIncludeAncestor) ? _allIncoming : _ownIncoming;
        }

        protected IEnumerable<IOutgoingEdgeDefinition> GetOutgoingEdgeDefinitions(bool myIncludeAncestor)
        {
            return (myIncludeAncestor) ? _allOutgoing : _ownOutgoing;
        }

        protected bool HasAttributeDefinitions(bool myIncludeAncestor)
        {
            return (myIncludeAncestor) ? _allHasAttributes : _ownHasAttributes;
        }

        protected bool HasPropertyDefinitions(bool myIncludeAncestor)
        {
            return (myIncludeAncestor) ? _allHasProperties : _ownHasProperties;
        }

        protected bool HasIncomingDefinitions(bool myIncludeAncestor)
        {
            return (myIncludeAncestor) ? _allHasIncoming : _ownHasIncoming;
        }

        protected bool HasOutgoingDefinitions(bool myIncludeAncestor)
        {
            return (myIncludeAncestor) ? _allHasOutgoing : _ownHasOutgoing;
        }

        protected IOutgoingEdgeDefinition GetOutgoingEdgeDefinition(string myEdgeName)
        {
            return GetAttributeDefinition(myEdgeName) as IOutgoingEdgeDefinition;
        }

        protected IIncomingEdgeDefinition GetIncomingEdgeDefinition(string myEdgeName)
        {
            return GetAttributeDefinition(myEdgeName) as IIncomingEdgeDefinition;
        }

        protected IPropertyDefinition GetPropertyDefinition(string myPropertyName)
        {
            return GetAttributeDefinition(myPropertyName) as IPropertyDefinition;
        }

        protected IAttributeDefinition GetAttributeDefinition(string myAttributeName)
        {
            IAttributeDefinition result;
            _attributes.TryGetValue(myAttributeName, out result);
            return result;
        }

        #endregion

    }
}