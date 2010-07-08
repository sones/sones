using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Warnings
{
    public class Warning_ObsoleteGQL : GraphDBWarning
    {
        public String ObsoleteGQL { get; private set; }
        public String NewGQL      { get; private set; }

        public Warning_ObsoleteGQL(String myObsoleteGQL, String myNewGQL)
        {
            ObsoleteGQL = myObsoleteGQL;
            NewGQL = myNewGQL;
        }

        public override string ToString()
        {
            return "'" + ObsoleteGQL + "' is obsolete! Please use: '" + NewGQL + "'";
        }

    }
}
