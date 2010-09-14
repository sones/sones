/* <id name="GraphDB – IDBShowSetting" />
 * <copyright file="IDBShowSetting.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Dirk Bludau</developer>
 * <developer>Stefan Licht</developer>
 * <summary></summary>
 */

#region Usings

using System;

#endregion

namespace sones.GraphDB.Settings
{
    public interface IDBShowSetting
    {
        /// <summary>
        /// The setting will be visible on any select etc without a specified request
        /// </summary>
        Boolean IsShown();
    }
}
