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


/*
 * GraphFSFactory
 * Achim Friedland, 2008 - 2010
 */

#region Usings

using System;

using sones.Lib.Singleton;
using sones.Lib.Reflection;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphFS
{

    /// <summary>
    /// A factory which uses reflection to generate a apropriate GraphFS for you.
    /// As this implements the singleton pattern, use GraphFSFactory.Instance.ActivateIGraphFS(...)
    /// </summary>
    public class GraphFSFactory : Singleton<GraphFSFactory.GraphFSFactory_internal>
    {

        /// <summary>
        /// An internal helper class for the GraphFSFactory class
        /// </summary>
        public class GraphFSFactory_internal : AutoDiscovery<IGraphFS>
        {

            #region Data

            const String _DefaultImplementation = "DiscGraphFS3";

            #endregion

            #region Constructor

            #region GraphFSFactory_internal()

            /// <summary>
            /// This constructor will autodiscover all implementations of IGraphFS
            /// </summary>
            public GraphFSFactory_internal()
            {
                FindAndRegisterImplementations(false, new String[] { "." }, t => t.Name);
            }

            #endregion

            #region GraphFSFactory_internal(myStrings)

            /// <summary>
            /// This constructor will autodiscover all implementations of IGraphFS
            /// </summary>
            public GraphFSFactory_internal(String[] myStrings)
            {
                FindAndRegisterImplementations(false, myStrings, t => t.Name);
            }

            #endregion

            #endregion

            #region ActivateIGraphFS(myImplementation)

            public Exceptional<IGraphFS> ActivateIGraphFS(String myImplementation = _DefaultImplementation)
            {

                if (myImplementation == null)
                    myImplementation = _DefaultImplementation;

                return ActivateT_protected(myImplementation);

            }

            #endregion

        }

    }

}
