/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;

namespace sones.Plugins.SonesGQL.Function.ErrorHandling
{
    public sealed class FunctionParameterTypeMismatchException : ASonesQLFunctionException
    {
        
        #region data        

        public Type ExpectedType { get; private set; }
        public Type CurrentType { get; private set; }

        #endregion

        #region constructor

        public FunctionParameterTypeMismatchException(Type myExpectedType, Type myCurrentType)
        {
            ExpectedType = myExpectedType;
            CurrentType = myCurrentType;

            _msg = String.Format("Function parameter type mismatch! Expected type \"{0}\" dos not match \"{1}\"!", ExpectedType, CurrentType);
        }
        
        #endregion
       
    }
}
