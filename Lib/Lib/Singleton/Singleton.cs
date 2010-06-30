/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
*
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
*/


/* <id name="PandoraLib – Singleton<T>" />
 * <copyright file="Singleton.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
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
