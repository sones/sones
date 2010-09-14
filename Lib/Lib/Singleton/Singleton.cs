/* <id name="GraphLib – Singleton<T>" />
 * <copyright file="Singleton.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Lib.Singleton
{

    /// <summary>
    /// This pattern will make it easy to create a Singleton class.
    /// Just derive this class and you will have a static lazy and thread safe singleton instance.
    /// </summary>
    /// <typeparam name="T">The type of the class</typeparam>
    public class Singleton<T> 
        where T : new()
    {

        /// <summary>
        /// The singleton instance
        /// </summary>
        public static readonly T Instance = new T();

    }

}
