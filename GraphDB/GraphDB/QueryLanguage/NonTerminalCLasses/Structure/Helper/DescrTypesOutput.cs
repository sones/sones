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



#region Usings

using System;
using System.Collections.Generic;

using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.TypeManagement;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure.Helper
{

    /// <summary>
    /// Generates the output of the describe type(s) command
    /// </summary>    
    public class DescrTypesOutput
    {

        #region Constructor

        public DescrTypesOutput()
        {
        }

        #endregion


        #region AttrOutput

        /// <summary>
        /// output for the type attributes
        /// </summary>
        /// <param name="myGraphDBType">the type</param>
        /// <param name="myDBContext">typemanager</param>
        /// <returns>a list of readouts, contains the attributes</returns>
        private IEnumerable<DBObjectReadout> GenerateAttributeOutput(GraphDBType myGraphDBType, DBContext myDBContext)
        {

            var _AttributeReadout = new List<DBObjectReadout>();            

            foreach (var _TypeAttribute in myGraphDBType.Attributes)
            {                                
                var Attributes = new Dictionary<String, Object>();
                Attributes.Add("Name", _TypeAttribute.Value.Name);
                Attributes.Add("Type", _TypeAttribute.Value.GetDBType(myDBContext.DBTypeManager));
                Attributes.Add("UUID", _TypeAttribute.Value.UUID);

                _AttributeReadout.Add(new DBObjectReadout(Attributes));
            }

            if(_AttributeReadout.Count > 0)
                return _AttributeReadout;
            else
                return null;
        }

        #endregion

        #region GenerateOutput(myDBContext, myGraphDBType, myName)

        /// <summary>
        /// Generate an output for an type with the attributes of the types and all parent types
        /// </summary>
        /// <param name="myDBContext">typemanager</param>
        /// <param name="myGraphDBType">the type</param>
        /// <returns>a list of readouts with the attributes and the parent types</returns>
        public IEnumerable<DBObjectReadout> GenerateOutput(DBContext myDBContext, GraphDBType myGraphDBType)
        {

            GraphDBType _ParentType = null;

            if (myGraphDBType.ParentTypeUUID != null)
                _ParentType = myDBContext.DBTypeManager.GetTypeByUUID(myGraphDBType.ParentTypeUUID);

            var _CurrentType = new Dictionary<String, Object>();

            _CurrentType.Add("Name",         myGraphDBType.Name);
            _CurrentType.Add("UUID",         myGraphDBType.UUID);
            _CurrentType.Add("Comment",      myGraphDBType.Comment);
            _CurrentType.Add("Attributes",   GenerateAttributeOutput(myGraphDBType, myDBContext));

            if (_ParentType != null)
                _CurrentType.Add("ParentType", GenerateOutput(myDBContext, _ParentType));

            return new List<DBObjectReadout>() { new DBObjectReadout(_CurrentType) };

        }

        #endregion

    }
}
