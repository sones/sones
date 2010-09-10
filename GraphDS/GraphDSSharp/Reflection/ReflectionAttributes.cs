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
 * sones GraphDS API - ReflectionAttributes
 * (c) Achim 'ahzf' Friedland, 2009 - 2010
 */

#region Usings

using System;

using sones.GraphDB.Structures;

#endregion

namespace sones.GraphDS.API.CSharp.Reflection
{

    #region HideFromDatabase

    public class HideFromDatabase : Attribute
    {
    }

    #endregion

    #region NoAutoCreation

    public class NoAutoCreation : Attribute
    {
    }

    #endregion

    #region Mandatory

    public class Mandatory : Attribute
    {
    }

    #endregion

    #region Default

    public class Default : Attribute
    {

        public Object DefaultValue { get; private set; }

        public Default(Object myDefaultValue)
        {
            DefaultValue = myDefaultValue;
        }

    }

    #endregion

    #region Indexed

    public class Indexed : Attribute
    {

        #region Properties (or named parameters)

        public String IndexName  { get; set; }
        public String IndexType  { get; set; }
        public String IndexOrder { get; set; }

        #endregion

        #region Constructors

        public Indexed()
        {
            IndexName  = "";
            IndexOrder = "";
            IndexType  = "";
        }

        public Indexed(String myIndexType)
        {
            IndexName  = "";
            IndexOrder = "";
            IndexType  = myIndexType;
        }

        public Indexed(DBIndexTypes myDBIndexType)
        {
            IndexName  = "";
            IndexOrder = "";
            IndexType  = Enum.GetName(typeof(DBIndexTypes), myDBIndexType);
        }

        public Indexed(String myIndexName, String myIndexType)
        {
            IndexName  = myIndexName;
            IndexOrder = "";
            IndexType  = myIndexType;
        }

        public Indexed(String myIndexName, DBIndexTypes myDBIndexType)
        {
            IndexName  = myIndexName;
            IndexOrder = "";
            IndexType  = Enum.GetName(typeof(DBIndexTypes), myDBIndexType);
        }

        public Indexed(String myIndexName, String myIndexOrder, String myIndexType)
        {
            IndexName  = myIndexName;
            IndexOrder = myIndexOrder;
            IndexType  = myIndexType;
        }

        public Indexed(String myIndexName, String myIndexOrder, DBIndexTypes myDBIndexType)
        {
            IndexName  = myIndexName;
            IndexOrder = myIndexOrder;
            IndexType  = Enum.GetName(typeof(DBIndexTypes), myDBIndexType);
        }

        #endregion

    }

    #endregion

    #region BackwardEdge

    public class BackwardEdge : Attribute
    {

        #region Properties (or Named Parameters)

        public String ReferencedAttributeName { get; set; }

        #endregion

        #region Constructors

        public BackwardEdge()
        {
            ReferencedAttributeName = "";
        }

        public BackwardEdge(String myReferencedAttributeName)
        {
            ReferencedAttributeName = myReferencedAttributeName;
        }

        #endregion

    }

    #endregion

}
