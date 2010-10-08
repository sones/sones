/* Lib - Methods for objects that can be estimated by size
 * (c) sones Team, 2009
 * 
 * 
 * Lead programmer:
 *      Henning Rauch
 * 
 * */

#region Usings

using System;

#endregion

namespace sones.Lib
{

    public interface IEstimable
    {
        UInt64 GetEstimatedSize();
    }

}
