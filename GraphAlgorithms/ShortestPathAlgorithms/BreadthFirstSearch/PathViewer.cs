/* <id name="GraphDB – PathViewer" />
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
using sones.GraphDB.ObjectManagement;
using sones.Lib.ErrorHandling;
using sones.GraphFS.DataStructures;

namespace sones.GraphAlgorithms.PathAlgorithm.BreadthFirstSearch
{

    public class PathViewer
    {

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

            var attributeUUID = myTypeAttribute.GetRelatedType(myTypeManager).GetTypeSpecificAttributeByName(myAttribute).UUID;

            foreach (var path in myPaths)
            {

                var pathString = new StringBuilder();
                Exceptional<DBObjectStream> currentDBObject;
                
                foreach (var _ObjectUUID in path)
                {
                    //load from DB
                    currentDBObject = myObjectCache.LoadDBObjectStream(myTypeAttribute.GetRelatedType(myTypeManager), _ObjectUUID);
                    if (currentDBObject.Failed())
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
            
            if (currentDBObject.Failed())
                throw new NotImplementedException();

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
