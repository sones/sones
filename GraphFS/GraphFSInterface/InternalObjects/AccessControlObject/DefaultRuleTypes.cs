/* GraphFS - DefaultRuleTypes
 * (c) Achim Friedland, 2009
 * 
 * Lead programmer:
 *      Achim Friedland
 * 
 * */

#region Usings

using System;
using System.Text;

#endregion

namespace sones.GraphFS.InternalObjects
{

    public enum DefaultRuleTypes : byte
    {
        DENY_OVER_ALLOW,
        ALLOW_OVER_DENY
    }

}
