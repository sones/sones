/* GraphLib - INotificationArguments
 * (c) Stefan Licht, 2009
 * 
 * Each implementation of ANotificationType, contains an internal struct of Arguments
 * which implements INotificationArguments. If you override the struct you must implement
 * this interface as well.
 * 
 * Lead programmer:
 *      Stefan Licht
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Notifications
{
    public interface INotificationArguments
    {
        Byte[] Serialize();
        void Deserialize(Byte[] mySerializedBytes);

        String ToString();
    }
}
