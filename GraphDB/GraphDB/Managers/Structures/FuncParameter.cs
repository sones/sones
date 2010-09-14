using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement;

namespace sones.GraphDB.Managers.Structures
{

    public struct FuncParameter
    {
        private IObject _Value;
        public IObject Value
        {
            get { return _Value; }
        }

        private TypeAttribute _TypeAttribute;
        public TypeAttribute TypeAttribute
        {
            get { return _TypeAttribute; }
        }

        public FuncParameter(IObject myValue, TypeAttribute myTypeAttribute)
        {
            _Value = myValue;
            _TypeAttribute = myTypeAttribute;
        }

        public FuncParameter(IObject myValue)
            : this(myValue, null)
        { }
    }

}
