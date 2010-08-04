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
 * XML_IO_Extensions
 * Achim Friedland, 2009 - 2010
 */

#region Usings

using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

using sones.GraphFS.DataStructures;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.QueryLanguage.Result;
using sones.Lib;

#endregion

namespace sones.GraphDS.API.CSharp
{

    /// <summary>
    /// Extension methods to transform datastructures into an
    /// application/xml representation an vice versa.
    /// </summary>

    public static class XML_IO_Extensions
    {

        #region ToXML(this myINode)

        public static XElement ToXML(this INode myINode)
        {

            return
                new XElement("INode",
                    new XAttribute("Version",               myINode.StructureVersion),
                    new XAttribute("ObjectUUID",            myINode.ObjectUUID),

                    new XElement("CreationTime",            myINode.CreationTime),
                    new XElement("LastAccessTime",          myINode.LastAccessTime),
                    new XElement("LastModificationTime",    myINode.LastModificationTime),
                    new XElement("DeletionTime",            myINode.DeletionTime),
                    new XElement("ReferenceCount",          myINode.ReferenceCount),
                    new XElement("ObjectSize",              myINode.ObjectSize),
                    new XElement("IntegrityCheckAlgorithm", myINode.IntegrityCheckAlgorithm),
                    new XElement("EncryptionAlgorithm",     myINode.EncryptionAlgorithm),

                    new XElement("ObjectLocatorPosition",
                        new XAttribute("Length",            myINode.ObjectLocatorLength),
                        new XAttribute("NumberOfCopies",    myINode.ObjectLocatorCopies),

                        from _ExtendedPosition in myINode.ObjectLocatorPositions
                        select new XElement("ExtendedPosition",
                            new XAttribute("StorageID",     _ExtendedPosition.StorageUUID),
                            new XAttribute("Position",      _ExtendedPosition.Position)))

            );

        }

        #endregion

        #region ToXML(this ObjectLocator)

        public static XElement ToXML(this ObjectLocator myObjectLocator)
        {

            return
                new XElement("ObjectLocator",
                    new XAttribute("Version", myObjectLocator.StructureVersion),
                    new XAttribute("ObjectUUID", myObjectLocator.ObjectUUID.ToHexString(SeperatorTypes.COLON)),

                new XElement("ObjectStreams",

                    from _ObjectStream in myObjectLocator
                    select
                        new XElement("ObjectStream",
                        new XAttribute("Name", _ObjectStream.Key),
                        new XAttribute("Type", "tobedone!"),

                        new XElement("ObjectEditions",
                        new XAttribute("DefaultEdition", _ObjectStream.Value.DefaultEditionName),

                        from _ObjectEdition in _ObjectStream.Value
                        select
                            new XElement("ObjectEdition",
                            new XAttribute("Name", _ObjectEdition.Key),
                            new XAttribute("IsDeleted", _ObjectEdition.Value.IsDeleted),

                            new XElement("ObjectRevisions",
                            new XAttribute("MinNumberOfRevisions", _ObjectEdition.Value.MinNumberOfRevisions),
                            new XAttribute("MaxNumberOfRevisions", _ObjectEdition.Value.MaxNumberOfRevisions),
                            new XAttribute("MinRevisionDelta", _ObjectEdition.Value.MinRevisionDelta),
                            new XAttribute("MaxRevisionAge", _ObjectEdition.Value.MaxRevisionAge),

                            from _ObjectRevisionsEnumerator in _ObjectEdition.Value
                            select new XElement("ObjectRevision",
                                new XAttribute("RevisionID", _ObjectRevisionsEnumerator.Key),

                                new XElement("ParentRevisions",
                                from _ParentRevisionID in _ObjectRevisionsEnumerator.Value.ParentRevisionIDs
                                select
                                    new XElement("ParentRevision",
                                    new XAttribute("RevisionID", _ParentRevisionID)



                                    )))))))));


            //                            XMLString.AppendLine("<ParentRevisions>".SpacingLeft(16));

            //                            if (ObjectEditionsEnumerator.Value._ParentRevisions.Count > 0 && ObjectEditionsEnumerator.Value._ParentRevisions.ContainsKey(ObjectRevisionsEnumerator.Key))
            //                                foreach (RevisionID _ParentRevision in ObjectEditionsEnumerator.Value._ParentRevisions[ObjectRevisionsEnumerator.Key])
            //                                    XMLString.AppendFormat("<ParentRevision Timestamp=\"{0}\" />".SpacingLeft(18), _ParentRevision.ToString()); XMLString.AppendLine();

            //                            XMLString.AppendLine("</ParentRevisions>".SpacingLeft(16));

            //                            XMLString.AppendFormat("<ObjectCopies MinNumberOfCopies=\"{0}\" MaxNumberOfCopies=\"{1}\" CacheUUID=\"{2}\">".SpacingLeft(16), ObjectRevisionsEnumerator.Value.MinNumberOfCopies, ObjectRevisionsEnumerator.Value.MaxNumberOfCopies, ObjectRevisionsEnumerator.Value.CacheUUID); XMLString.AppendLine();

            //                            #region Write ObjectStreams

            ////                            if (ObjectRevisionsEnumerator.Value.Co != null)
            //                                foreach (ObjectStream ObjectStream in ObjectRevisionsEnumerator.Value)
            //                                {

            //                                    XMLString.Append("<ObjectStream".SpacingLeft(18));
            //                                    XMLString.AppendFormat(" Algorithm=\"{0}\"",              ObjectStream.Compression);
            //                                    XMLString.AppendFormat(" ForwardErrorCorrection=\"{0}\"", ObjectStream.ForwardErrorCorrection);
            //                                    XMLString.AppendFormat(" IntegrityCheckValue=\"{0}\"",    ByteArrayHelper.ByteArrayToFormatedString(ObjectStream.IntegrityCheckValue));
            //                                    XMLString.AppendFormat(" ObjectUUID=\"{0}\"",             ObjectStream.ObjectUUID);
            //                                    XMLString.AppendFormat(" Redundancy=\"{0}\"",             ObjectStream.Redundancy);
            //                                    XMLString.AppendFormat(" ReservedLength=\"{0}\"",         ObjectStream.ReservedLength);
            //                                    XMLString.AppendFormat(" StreamLength=\"{0}\"",           ObjectStream.StreamLength);
            //                                    XMLString.AppendLine(">");

            //                                    #region Write AccessRights

            //                                    if (ObjectStream.AccessRights != null)
            //                                        if (ObjectStream.AccessRights.Count > 0)
            //                                        {

            //                                            XMLString.AppendLine("<AccessRights>".SpacingLeft(20));

            //                                            foreach (AccessRight _AccessRight in ObjectStream.AccessRights)
            //                                            {
            //                                                XMLString.AppendFormat("<AccessRight AccessFlags=\"{0}\" EncryptionParameters=\"{1}\" UserID=\"{2}\" />".SpacingLeft(20), _AccessRight.AccessFlags.ToString(), _AccessRight.EncryptionParameters.ToString(), _AccessRight.UserID); XMLString.AppendLine();
            //                                            }

            //                                            XMLString.AppendLine("</AccessRights>".SpacingLeft(20));

            //                                        }

            //                                        else XMLString.AppendLine("<AccessRights />".SpacingLeft(20));

            //                                    else XMLString.AppendLine("<AccessRights />".SpacingLeft(20));

            //                                    #endregion

            //                                    #region AvailableStorageIDs

            //                                    if (ObjectStream.AvailableStorageIDs != null)
            //                                        if (ObjectStream.AvailableStorageIDs.Count > 0)
            //                                        {


            //                                            XMLString.AppendLine("<AvailableStorageIDs>".SpacingLeft(20));

            //                                            foreach (UInt64 _StorageID in ObjectStream.AvailableStorageIDs)
            //                                            {
            //                                                XMLString.AppendFormat("<AvailableStorageID StorageID=\"{0}\">".SpacingLeft(20), _StorageID);
            //                                                XMLString.AppendLine();
            //                                            }

            //                                            XMLString.AppendLine("</AvailableStorageIDs>".SpacingLeft(20));

            //                                        }

            //                                        else XMLString.AppendLine("<AvailableStorageIDs />".SpacingLeft(20));

            //                                    else XMLString.AppendLine("<AvailableStorageIDs />".SpacingLeft(20));

            //                                    #endregion

            //                                    #region BlockIntegrityArrays

            //                                    //if (ObjectStream.BlockIntegrityArrays != null)
            //                                        //if (ObjectStream.BlockIntegrityArrays.Count > 0)
            //                                        //{


            //                                        //    XMLString.AppendLine("<BlockIntegrityArrays>".SpacingLeft(20));

            //                                        //    foreach (BlockIntegrity _BlockIntegrity in ObjectStream.BlockIntegrityArrays)
            //                                        //    {
            //                                        //        XMLString.AppendFormat("<BlockIntegrity XXX=\"{0}\">".SpacingLeft(20), _BlockIntegrity);
            //                                        //        XMLString.AppendLine();
            //                                        //    }

            //                                        //    XMLString.AppendLine("</BlockIntegrityArrays>".SpacingLeft(20));

            //                                        //}

            //                                        //else XMLString.AppendLine("<BlockIntegrityArrays />".SpacingLeft(20));

            //                                    //else
            //                                    XMLString.AppendLine("<BlockIntegrityArrays />".SpacingLeft(20));

            //                                    #endregion

            //                                    #region Write ObjectExtent

            //                                    XMLString.AppendLine("<Extents>".SpacingLeft(20));

            //                                    if (ObjectStream.Extents != null)
            //                                        foreach (ObjectExtent ObjectExtent in ObjectStream.Extents)
            //                                        {
            //                                            XMLString.Append("<Extent".SpacingLeft(22));
            //                                            XMLString.AppendFormat(" Length=\"{0}\"",               ObjectExtent.Length);
            //                                            XMLString.AppendFormat(" LogicalPosition=\"{0}\"",      ObjectExtent.LogicalPosition);
            //                                            XMLString.AppendFormat(" StorageID=\"{0}\"",            ObjectExtent.StorageID);
            //                                            XMLString.AppendFormat(" PhysicalPosition=\"{0}\"",     ObjectExtent.PhysicalPosition);
            //                                            XMLString.AppendFormat(" NextExtent_StorageID=\"{0}\"", ObjectExtent.NextExtent.StorageID);
            //                                            XMLString.AppendFormat(" NextExtent_Position=\"{0}\"",  ObjectExtent.NextExtent.Position);
            //                                            XMLString.AppendLine(" />");
            //                                        }

            //                                    XMLString.AppendLine("</Extents>".SpacingLeft(20));

            //                                    #endregion

            //                                    XMLString.AppendLine("</ObjectStream>".SpacingLeft(18));

            //                                }

            //                        #endregion

            //                        XMLString.AppendLine("</ObjectCopies>".SpacingLeft(16));
            //                        XMLString.AppendLine("</ObjectRevisions>".SpacingLeft(14));

            //                    }

            //                    #endregion

            //                    XMLString.AppendLine("</ObjectRevisions>".SpacingLeft(12));
            //                    XMLString.AppendLine("</ObjectEditions>".SpacingLeft(10));

            //                }

            //                #endregion

            //                XMLString.AppendLine("</ObjectEditions>".SpacingLeft(8));
            //                XMLString.AppendLine("</ObjectStream>".SpacingLeft(6));

            //            }

            //            XMLString.AppendLine("</ObjectStreamTypes>".SpacingLeft(4));
            //            XMLString.AppendLine("</ObjectLocator>".SpacingLeft(2));

            //            return XMLString.ToString();

        }

        #endregion

        #region ToXML(this myQueryResult)

        public static XElement ToXML(this QueryResult myQueryResult)
        {

            // root element...
            var _Query = new XElement("queryresult", new XAttribute("version", "1.0"));


            // query --------------------------------
            _Query.Add(new XElement("query", myQueryResult.Query));

            // result -------------------------------
            _Query.Add(new XElement("result", myQueryResult.ResultType));

            // duration -----------------------------
            _Query.Add(new XElement("duration", new XAttribute("resolution", "ms"), myQueryResult.Duration));

            // warnings -----------------------------
            _Query.Add(new XElement("warnings",
                from _Warning in myQueryResult.Warnings
                select
                    new XElement("warning",
                    new XAttribute("code", _Warning.GetType().Name),
                    _Warning.ToString())
                ));

            // errors -------------------------------
            _Query.Add(new XElement("errors",
                from _Error in myQueryResult.Errors
                select
                    new XElement("error",
                    new XAttribute("code", _Error.GetType().Name),
                    _Error.ToString())
                ));

            // results ------------------------------
            _Query.Add(new XElement("results",
                from _SelectionListElementResult in myQueryResult.Results
                where _SelectionListElementResult.Objects != null
                select
                    from _DBObject in _SelectionListElementResult.Objects
                    select
                        _DBObject.ToXML()
            ));

            return _Query;

        }

        #endregion

        #region ToXML(this myDBObjectReadout)

        public static XElement ToXML(this DBObjectReadout myDBObjectReadout)
        {
            return myDBObjectReadout.ToXML(false);
        }

        #endregion

        #region (private) ToXML(this myDBObjectReadout, myRecursion)

        private static XElement ToXML(this DBObjectReadout myDBObjectReadout, Boolean myRecursion)
        {

            Type _AttributeType = null;
            var _AttributeTypeString = "";
            var _DBObject = new XElement("DBObject");

            DBObjectReadoutGroup _GroupedDBObjects = null;
            DBWeightedObjectReadout _WeightedDBObject = null;
            IEnumerable<DBObjectReadout> _DBObjects = null;
            IEnumerable<Object> _AttributeValueList = null;
            IGetName _IGetName = null;

            #region DBWeightedObjectReadout

            var _WeightedDBObject1 = myDBObjectReadout as DBWeightedObjectReadout;

            if (_WeightedDBObject1 != null)
            {
                _DBObject.Add(new XElement("edgelabel", new XElement("attribute", new XAttribute("name", "weight"), new XAttribute("type", _WeightedDBObject1.Weight.Type.ToString()), _WeightedDBObject1.Weight)));
            }

            #endregion

            foreach (var _Attribute in myDBObjectReadout.Attributes)
            {

                if (_Attribute.Value != null)
                {

                    #region DBObjectReadoutGroup

                    _GroupedDBObjects = _Attribute.Value as DBObjectReadoutGroup;

                    if (_GroupedDBObjects != null)
                    {

                        var _Grouped = new XElement("grouped");

                        if (_GroupedDBObjects.CorrespondingDBObjects != null)
                            foreach (var _DBObjectReadout in _GroupedDBObjects.CorrespondingDBObjects)
                                _Grouped.Add(_DBObjectReadout.ToXML());

                        _DBObject.Add(_Grouped);

                        continue;

                    }

                    #endregion

                    #region DBWeightedObjectReadout

                    _WeightedDBObject = _Attribute.Value as DBWeightedObjectReadout;

                    if (_WeightedDBObject != null)
                    {
                        _DBObject.Add(new XElement("edgelabel", new XElement("attribute", new XAttribute("name", "weight"), new XAttribute("type", _WeightedDBObject1.Weight.Type.ToString()), _WeightedDBObject1.Weight)));
                        continue;
                    }

                    #endregion

                    #region IEnumerable<DBObjectReadout>

                    _DBObjects = _Attribute.Value as IEnumerable<DBObjectReadout>;

                    if (_DBObjects != null && _DBObjects.Count() > 0)
                    {

                        var _EdgeInfo = (_Attribute.Value as Edge);
                        var _EdgeType = (_EdgeInfo != null) ? _EdgeInfo.EdgeTypeName : "";

                        var _ListAttribute = new XElement("edge",
                            new XAttribute("name", _Attribute.Key.EscapeForXMLandHTML()),
                            new XAttribute("type", _EdgeType));

                        // An edgelabel for all edges together...
                        _ListAttribute.Add(new XElement("hyperedgelabel"));

                        foreach (var _DBObjectReadout in _DBObjects)
                            _ListAttribute.Add(_DBObjectReadout.ToXML());

                        _DBObject.Add(_ListAttribute);
                        continue;

                    }

                    #endregion

                    #region Attribute Type (may be generic!)

                    _AttributeType = _Attribute.Value.GetType();

                    if (_AttributeType.IsGenericType)
                    {
                        _AttributeTypeString = _AttributeType.Name;
                        _AttributeTypeString = _AttributeTypeString.Substring(0, _AttributeTypeString.IndexOf('`')).ToUpper();
                        _AttributeTypeString += "&lt;";
                        _AttributeTypeString += _AttributeType.GetGenericArguments()[0].Name;
                        _AttributeTypeString += "&gt;";
                    }

                    else
                        _AttributeTypeString = _AttributeType.Name;

                    #endregion

                    #region Add result to _DBObject

                    var _AttributeTag = new XElement("attribute",
                        new XAttribute("name", _Attribute.Key.EscapeForXMLandHTML()),
                        new XAttribute("type", _AttributeTypeString)
                    );

                    _DBObject.Add(_AttributeTag);

                    #endregion

                    #region Attribute value and attribute value lists

                    _AttributeValueList = _Attribute.Value as IEnumerable<Object>;

                    if (_AttributeValueList != null)
                    {
                        foreach (var _Value in _AttributeValueList)
                        {

                            // _Value.ToString() may not always return the information we need!
                            _IGetName = _Value as IGetName;
                            if (_IGetName != null)
                                _AttributeTag.Add(new XElement("item", _IGetName.Name));
                            else
                                _AttributeTag.Add(new XElement("item", _Value.ToString().EscapeForXMLandHTML()));

                        }
                    }

                    else
                    {

                        // _Attribute.Value.ToString() may not always return the information we need!
                        _IGetName = _Attribute.Value as IGetName;

                        if (_IGetName != null)
                            _AttributeTag.Value = _IGetName.Name;
                        else
                            _AttributeTag.Value = _Attribute.Value.ToString().EscapeForXMLandHTML();

                    }

                    #endregion

                }

            }

            return _DBObject;

        }

        #endregion



        #region BuildXMLDocument(params myXElements)

        public static XDocument BuildXMLDocument(params XElement[] myXElements)
        {

            var _XMLDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"));

            var _Sones       = new XElement("sones",   new XAttribute("version", "1.0"));
            var _GraphDB     = new XElement("GraphDB", new XAttribute("version", "1.0"));

            foreach (var _XElement in myXElements)
                _GraphDB.Add(_XElement);

            _Sones.Add(_GraphDB);
            _XMLDocument.Add(_Sones);

            return _XMLDocument;

        }

        #endregion

        #region XMLDocument2String(this myXDocument)

        public static String XMLDocument2String(this XDocument myXDocument)
        {

         //   var _StringWriter = new StringWriter();

         //   var _XmlWriterSettings = new XmlWriterSettings()
         //   {
         //       Encoding         = Encoding.UTF8,
         //   //    ConformanceLevel = ConformanceLevel.Document,
         //   //    Indent           = true
         //   };

         //   var writer = XmlWriter.Create(_StringWriter, _XmlWriterSettings);
         //   myXDocument.Save(writer);

         //   return _StringWriter.GetStringBuilder().ToString();

            var _String = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" + Environment.NewLine;
            _String += myXDocument.ToString();

            return _String;

        }

        #endregion

    }

}
