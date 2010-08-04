using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.TypeManagement;

namespace sones.GraphDB.Managers.Structures
{

    public struct FuncParameter
    {
        private AObject _Value;
        public AObject Value
        {
            get { return _Value; }
        }

        private TypeAttribute _TypeAttribute;
        public TypeAttribute TypeAttribute
        {
            get { return _TypeAttribute; }
        }

        public FuncParameter(AObject myValue, TypeAttribute myTypeAttribute)
        {
            _Value = myValue;
            _TypeAttribute = myTypeAttribute;
        }

        public FuncParameter(AObject myValue)
            : this(myValue, null)
        { }
    }

}
