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

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace sones.GraphDB.Linq
//{

//    internal static class TypeSystem
//    {
//        internal static Type GetElementType(Type seqType)
//        {
//            Type ienum = FindIEnumerable(seqType);
//            if (ienum == null) return seqType;
//            return ienum.GetGenericArguments()[0];
//        }
//        private static Type FindIEnumerable(Type seqType)
//        {
//            if (seqType == null || seqType == typeof(string))
//                return null;
//            if (seqType.IsArray)
//                return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());
//            if (seqType.IsGenericType)
//            {
//                foreach (Type arg in seqType.GetGenericArguments())
//                {
//                    Type ienum = typeof(IEnumerable<>).MakeGenericType(arg);
//                    if (ienum.IsAssignableFrom(seqType))
//                    {
//                        return ienum;
//                    }
//                }
//            }
//            Type[] ifaces = seqType.GetInterfaces();
//            if (ifaces != null && ifaces.Length > 0)
//            {
//                foreach (Type iface in ifaces)
//                {
//                    Type ienum = FindIEnumerable(iface);
//                    if (ienum != null) return ienum;
//                }
//            }
//            if (seqType.BaseType != null && seqType.BaseType != typeof(object))
//            {
//                return FindIEnumerable(seqType.BaseType);
//            }
//            return null;
//        }
//    }

//}
