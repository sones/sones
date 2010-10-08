using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace sones.Lib.DataStructures
{

    public static class TrinaryExtensionMethods
    {

        public static int CompareTo(this Boolean myBoolean, Trinary myTrinary)
        {

            if (myTrinary == Trinary.FALSE    && myBoolean == false)
                return 0;

            if (myTrinary == Trinary.DELETED  && myBoolean == false)
                return 0;

            if (myTrinary == Trinary.TRUE     && myBoolean == true)
                return 0;

            return 1;

        }

    }

}
