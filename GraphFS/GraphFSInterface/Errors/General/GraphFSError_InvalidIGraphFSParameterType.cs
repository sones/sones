/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
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
* 
*/

/*
 * GraphFSError_InvalidIGraphFSParameterType
 * (c) Achim Friedland, 2010
 */

#region Usings

using System;

#endregion

namespace sones.GraphFS.Errors
{

    public class GraphFSError_InvalidIGraphFSParameterType : GraphFSError
    {

        #region Properties

        public IGraphFS IGraphFS        { get; private set; }
        public String   ParameterName   { get; private set; }
        public Type     ParameterType   { get; private set; }
        public Type     WrongType       { get; private set; }

        #endregion

        #region Constructor

        #region GraphFSError_InvalidIGraphFSParameterType(myIGraphFS, myParameterName, myParameterType, myWrongType)

        public GraphFSError_InvalidIGraphFSParameterType(IGraphFS myIGraphFS, String myParameterName, Type myParameterType, Type myWrongType)
        {
            IGraphFS      = myIGraphFS;
            ParameterName = myParameterName;
            ParameterType = myParameterType;
            Message       = String.Format("IGraphFS parameter/property '{0}.{1}' expected to be of type '{2}', not of type '{3}'!", myIGraphFS.GetType().Name, myParameterName, myParameterType.Name, myWrongType.Name);
        }

        #endregion

        #endregion

    }

}
