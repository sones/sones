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

/* <id name="PandoraDB – PathViewer" />
 * <copyright file="PathViewer.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Martin Junghanns</developer>
 * <developer>Michael Woidak</developer>
 * <summary>
 * Class is used to do some logging during the path search.
 * </summary>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeManagement;
//using sones.Lib.Frameworks.NLog;
using sones.GraphDB.ObjectManagement;
using sones.Lib.ErrorHandling;
using sones.Lib.DataStructures.UUID;
using sones.GraphFS.DataStructures;

namespace GraphAlgorithms.PathAlgorithm.BreadthFirstSearch
{
    class PathViewer
    {
        //Logger
        //private static Logger //_Logger = LogManager.GetCurrentClassLogger();

        //used to create output (and save some memory)
        private static StringBuilder _StringBuilder = new StringBuilder();

        /// <summary>
        /// Writes all paths included in the HashSet. Every object is represented via a given attribute.
        /// 
        /// example path between fry and morbo (attribute is "Name") 
        /// 
        /// output:
        /// Fry -> Leela -> Lrrr -> Morbo
        /// </summary>
        /// <param name="myPaths">An HashSet which contains Lists of UUIDs</param>
        /// <param name="myTypeManager">The type manager to load the DBObjects</param>
        /// <param name="myTypeAttribute">The Type Attribute</param>
        /// <param name="myAttribute">The Attribute which shall be used for output.</param>
        public static void ShowDBObjects(HashSet<List<ObjectUUID>> myPaths, DBTypeManager myTypeManager, TypeAttribute myTypeAttribute, String myAttribute, DBObjectCache myObjectCache)
        {
            AttributeUUID attributeUUID = myTypeAttribute.GetRelatedType(myTypeManager).GetTypeSpecificAttributeByName(myAttribute).UUID;
            foreach (List<ObjectUUID> path in myPaths)
            {
                StringBuilder pathString = new StringBuilder();
                Exceptional<DBObjectStream> currentDBObject;
                foreach (ObjectUUID uuid in path)
                {
                    //load from DB
                    currentDBObject = myObjectCache.LoadDBObjectStream(myTypeAttribute.GetRelatedType(myTypeManager), uuid);
                    if (currentDBObject.Failed)
                    {
                        throw new NotImplementedException();
                    }

                    pathString.Append(currentDBObject.Value.GetAttribute(attributeUUID) + " -> ");
                }
                ////_Logger.Info(pathString.ToString());
                pathString.Remove(0, pathString.Length);
            }
        }

        /// <summary>
        /// Writes an DBObject into log
        /// </summary>
        /// <param name="myObjectUUID">The UUID of the Object</param>
        /// <param name="myTypeManager">The corresponding type manager</param>
        /// <param name="myTypeAttribute">The type attribute</param>
        public static void ShowDBObject(ObjectUUID myObjectUUID, DBTypeManager myTypeManager, TypeAttribute myTypeAttribute, DBObjectCache myObjectCache)
        {
            var currentDBObject = myObjectCache.LoadDBObjectStream(myTypeAttribute.GetRelatedType(myTypeManager), myObjectUUID);
            if (currentDBObject.Failed)
            {
                throw new NotImplementedException();
            }

            ////_Logger.Info(currentDBObject.Value.ToString());                            
        }

        /// <summary>
        /// Used to write one path into the log. Prints just the identifier of that Object (mostly an UUID)
        /// </summary>
        /// <param name="myPath">The path to be written.</param>
        public static void LogPath(List<ObjectUUID> myPath)
        {
            _StringBuilder.Remove(0, _StringBuilder.Length);
            for(int i = 0; i< myPath.Count - 1 ; i++)
            {
                _StringBuilder.Append(myPath.ElementAt(i).ToString() + " -> ");
            }
            _StringBuilder.Append(myPath.Last().ToString());

            ////_Logger.Info(_StringBuilder.ToString());
        }
    }
}
