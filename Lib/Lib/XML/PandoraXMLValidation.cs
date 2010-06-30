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


/* PandoraFS - PandoraXMLValidation
 * Achim Friedland, 2009
 * 
 * Adds the enclosing pandra XML tags to any XML export.
 * 
 * Lead programmer:
 *      Achim Friedland
 * 
 * */

#region Usings

using System;
using System.Text;
using System.Xml.Linq;

using sones.Lib;
using sones.Lib.XML;
using info = sones.Lib.Information;

#endregion

namespace sones.Lib.XML
{

    public class PandoraXMLValidation
    {

        #region AddValidationInformation(myValidationType, myEmbeddedXML ...)

        public static String AddValidationInformation(XMLValidationTypes myValidationType, params XElement[] myXElements)
        {

            #region XML Header

            var _XMLString = new StringBuilder();
            _XMLString.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");

            #endregion


            #region DTD

            if (myValidationType == XMLValidationTypes.DTD)
            {

                _XMLString.AppendLine("<!DOCTYPE Pandora [");

                _XMLString.AppendLine("<!ELEMENT Pandora                 (INode|ObjectLocator)+>");
                _XMLString.AppendLine("<!ATTLIST Pandora");
                _XMLString.AppendLine("VersionString                     CDATA #REQUIRED");
                _XMLString.AppendLine("VersionMajor                      CDATA #REQUIRED");
                _XMLString.AppendLine("VersionMinor                      CDATA #REQUIRED");
                _XMLString.AppendLine("BuildNumber                       CDATA #REQUIRED");
                _XMLString.AppendLine(">");

                _XMLString.AppendLine("<!ELEMENT INode                   (CreationTime, LastAccessTime, LastModificationTime, DeletionTime, ReferenceCount, ObjectSize, IntegrityCheckAlgorithm, EncryptionAlgorithm, ObjectLocatorPosition)>");
                _XMLString.AppendLine("<!ATTLIST INode");
                _XMLString.AppendLine("Version                           CDATA #REQUIRED");
                _XMLString.AppendLine("ObjectUUID                        CDATA #REQUIRED");
                _XMLString.AppendLine(">");

                _XMLString.AppendLine("<!ELEMENT CreationTime            (#PCDATA)>");
                _XMLString.AppendLine("<!ELEMENT LastAccessTime          (#PCDATA)>");
                _XMLString.AppendLine("<!ELEMENT LastModificationTime    (#PCDATA)>");
                _XMLString.AppendLine("<!ELEMENT DeletionTime            (#PCDATA)>");
                _XMLString.AppendLine("<!ELEMENT ReferenceCount          (#PCDATA)>");
                _XMLString.AppendLine("<!ELEMENT ObjectSize              (#PCDATA)>");
                _XMLString.AppendLine("<!ELEMENT IntegrityCheckAlgorithm (#PCDATA)>");
                _XMLString.AppendLine("<!ELEMENT EncryptionAlgorithm     (#PCDATA)>");

                _XMLString.AppendLine("<!ELEMENT ObjectLocatorPosition   (ExtendedPosition)+>");
                _XMLString.AppendLine("<!ATTLIST ObjectLocatorPosition");
                _XMLString.AppendLine("Length                            CDATA #REQUIRED");
                _XMLString.AppendLine("Reserve                           CDATA #REQUIRED");
                _XMLString.AppendLine(">");

                _XMLString.AppendLine("<!ELEMENT ExtendedPosition        EMPTY>");
                _XMLString.AppendLine("<!ATTLIST ExtendedPosition");
                _XMLString.AppendLine("StorageID                         CDATA #REQUIRED");
                _XMLString.AppendLine("Position                          CDATA #REQUIRED");
                _XMLString.AppendLine(">");

                _XMLString.AppendLine("<!ELEMENT ObjectLocator           (ObjectStreamTypes)>");
                _XMLString.AppendLine("<!ATTLIST ObjectLocator");
                _XMLString.AppendLine("Version                           CDATA #REQUIRED");
                _XMLString.AppendLine("ObjectUUID                        CDATA #REQUIRED");
                _XMLString.AppendLine(">");

                _XMLString.AppendLine("<!ELEMENT ObjectStreamTypes       (ObjectStream)+>");

                _XMLString.AppendLine("<!ELEMENT ObjectStream        (ObjectEditions)>");
                _XMLString.AppendLine("<!ATTLIST ObjectStream");
                _XMLString.AppendLine("Type                              CDATA #REQUIRED");
                _XMLString.AppendLine(">");

                _XMLString.AppendLine("<!ELEMENT ObjectEditions          (ObjectEditions)+>");
                _XMLString.AppendLine("<!ATTLIST ObjectEditions");
                _XMLString.AppendLine("DefaultEdition                    CDATA #REQUIRED");
                _XMLString.AppendLine(">");

                _XMLString.AppendLine("<!ELEMENT ObjectEditions           (ObjectRevisions)>");
                _XMLString.AppendLine("<!ATTLIST ObjectEditions");
                _XMLString.AppendLine("myLogin                              CDATA #REQUIRED");
                _XMLString.AppendLine(">");

                _XMLString.AppendLine("<!ELEMENT ObjectRevisions         (ObjectRevisions)+>");
                _XMLString.AppendLine("<!ATTLIST ObjectRevisions");
                _XMLString.AppendLine("MinNumberOfRevisions              CDATA #REQUIRED");
                _XMLString.AppendLine("MaxNumberOfRevisions              CDATA #REQUIRED");
                _XMLString.AppendLine("MinRevisionDelta                  CDATA #REQUIRED");
                _XMLString.AppendLine("MaxRevisionAge                    CDATA #REQUIRED");
                _XMLString.AppendLine(">");

                _XMLString.AppendLine("<!ELEMENT ObjectRevisions          (ObjectCopies)+>");
                _XMLString.AppendLine("<!ATTLIST ObjectRevisions");
                _XMLString.AppendLine("RevisionID                        CDATA #REQUIRED");
                _XMLString.AppendLine(">");

                _XMLString.AppendLine("<!ELEMENT ObjectCopies            (ObjectStream)+>");
                _XMLString.AppendLine("<!ATTLIST ObjectCopies");
                _XMLString.AppendLine("MinNumberOfCopies                 CDATA #REQUIRED");
                _XMLString.AppendLine("MaxNumberOfCopies                 CDATA #REQUIRED");
                _XMLString.AppendLine(">");

                _XMLString.AppendLine("<!ELEMENT ObjectStream            (AccessRights, AvailableStorageIDs, BlockIntegrityArrays, Extents)>");
                _XMLString.AppendLine("<!ATTLIST ObjectStream");
                _XMLString.AppendLine("Algorithm              CDATA #REQUIRED");
                _XMLString.AppendLine("ForwardErrorCorrection   CDATA #REQUIRED");
                _XMLString.AppendLine("IntegrityCheckValue               CDATA #REQUIRED");
                _XMLString.AppendLine("ObjectUUID                        CDATA #REQUIRED");
                _XMLString.AppendLine("Redundancy               CDATA #REQUIRED");
                _XMLString.AppendLine("ReservedLength                    CDATA #REQUIRED");
                _XMLString.AppendLine("StreamLength                      CDATA #REQUIRED");
                _XMLString.AppendLine(">");

                _XMLString.AppendLine("<!ELEMENT AccessRights            EMPTY>");
                _XMLString.AppendLine("<!ELEMENT AvailableStorageIDs     EMPTY>");
                _XMLString.AppendLine("<!ELEMENT BlockIntegrityArrays    EMPTY>");
                _XMLString.AppendLine("<!ELEMENT Extents                 (Extent)+>");

                _XMLString.AppendLine("<!ELEMENT Extent                  EMPTY>");
                _XMLString.AppendLine("<!ATTLIST Extent");
                _XMLString.AppendLine("Length                            CDATA #REQUIRED");
                _XMLString.AppendLine("LogicalPosition                   CDATA #REQUIRED");
                _XMLString.AppendLine("StorageID                         CDATA #REQUIRED");
                _XMLString.AppendLine("PhysicalPosition                  CDATA #REQUIRED");
                _XMLString.AppendLine("NextExtent_StorageID              CDATA #REQUIRED");
                _XMLString.AppendLine("NextExtent_Position               CDATA #REQUIRED");
                _XMLString.AppendLine(">");

                _XMLString.AppendLine("]>");

            }

            #endregion

            #region DTD_URL

            else if (myValidationType == XMLValidationTypes.DTD_URL)
                _XMLString.AppendLine("<!DOCTYPE spec SYSTEM \"http://www.sones.de/pandora/XML/DTD/PandoraXML.dtd\">");

            #endregion

            #region Schema

            else if (myValidationType == XMLValidationTypes.Schema)
            {
            }

            #endregion

            #region Schema_URL

            else if (myValidationType == XMLValidationTypes.Schema_URL)
            {
            }

            #endregion


            #region Print <Pandora ...> ... </Pandora>

            _XMLString.AppendFormat("<Pandora VersionString=\"{0}\" VersionMajor=\"{1}\" VersionMinor=\"{2}\" BuildNumber=\"{3}\">", info.Version.VersionString, info.Version.VersionMajor, info.Version.VersionMinor, info.Version.BuildNumber); _XMLString.AppendLine();

            foreach (var _XElement in myXElements)
                _XMLString.Append(_XElement.ToString());

            _XMLString.AppendLine("</Pandora>");
            _XMLString.AppendLine("");

            #endregion

            return _XMLString.ToString();

        }

        #endregion


    }

}
