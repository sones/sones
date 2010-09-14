using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeManagement;

namespace sones.GraphDB.Errors
{
    public class Error_InvalidAttributeKind : GraphDBAttributeError
    {
        public KindsOfType[] ExpectedKindsOfType { get; private set; }
        public KindsOfType CurrentKindsOfType { get; private set; }

        public Error_InvalidAttributeKind()
        {
            ExpectedKindsOfType = new KindsOfType[0];
        }

        public Error_InvalidAttributeKind(KindsOfType myCurrentKindsOfType, params KindsOfType[] myExpectedKindsOfType)
            : this()
        {
            ExpectedKindsOfType = myExpectedKindsOfType;
            CurrentKindsOfType = myCurrentKindsOfType;
        }

        public override string ToString()
        {
            return String.Format("The given kind \"{0}\" does not match the expected \"{0}\"", CurrentKindsOfType,
                ExpectedKindsOfType.Aggregate<KindsOfType, StringBuilder>(new StringBuilder(), (result, elem) => { result.AppendFormat("{0},", elem); return result; }));
        }
    }
}
