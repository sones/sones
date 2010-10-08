/*
 * NotificationPriority
 * (c) Achim Friedland, 2010
 */

#region Usings

using System;
using System.Text;

#endregion

namespace sones.Notifications.Messages
{

    /// <summary>
    /// This enum defines possible notification priorities
    /// </summary>
    public enum NotificationPriority : byte
    {
        Realtime,
        High,
        Severe,
        Normal,
        Low,
    }

}
