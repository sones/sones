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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib;

namespace sones.GraphDB.Errors
{
    public class Error_InvalidUndefinedAttributes : GraphDBAttributeError
    {
        public String AttrName { get; private set; }
        public List<String> ListOfAttrNames { get; private set; }

        public Error_InvalidUndefinedAttributes(String myAttrName)
        {
            AttrName = myAttrName;
        }

        public Error_InvalidUndefinedAttributes(List<String> myListOfAttrNames)
        {
            ListOfAttrNames = myListOfAttrNames;
        }

        public override string ToString()
        {
            if (ListOfAttrNames.IsNullOrEmpty())
            {
                return String.Format("The object does not contain an undefined attribute with name \" {0} \".", AttrName);
            }
            else
            {
                String retVal = ListOfAttrNames[0];

                for (int i = 1; i < ListOfAttrNames.Count; i++)
                    retVal += "," + ListOfAttrNames[i];

                return String.Format("The object does not contains the undefined attributes \" {0} \".", retVal);
            }
        }
    
    }
}
