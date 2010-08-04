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


/* GraphDSWebDAV_Service
 * (c) Stefan Licht, 2009
 * 
 * A WebDAV server implementation on the GraphFS
 * 
 * Lead programmer:
 *      Stefan Licht
 * 
 */

#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Text;
using System.Xml;
using sones.GraphFS;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Exceptions;
using sones.GraphFS.InternalObjects;
using sones.GraphFS.Objects;
using sones.GraphFS.Session;
using sones.Lib;
using sones.Lib.DataStructures.Timestamp;
using sones.Networking.HTTP;
using sones.Networking.WebDAV;
using sones.Lib.DataStructures;
using sones.Lib.ErrorHandling;


#endregion

namespace sones.GraphDS.Connectors.WebDAV
{

    public class GraphDSWebDAV_Service : IGraphDSWebDAV_Service
    {
        //private static Logger _Logger = LogManager.GetCurrentClassLogger();

        #region Readonly Definition Strings

        protected readonly String S_DAV_PREFIX = "D";
        protected readonly String S_DAV_NAMESPACE_URI = "DAV:";

        protected readonly String S_SVN_PREFIX = "svn";
        protected readonly String S_SVN_NAMESPACE_URI = "http://subversion.tigris.org/xmlns/dav/";

        protected readonly String S_DATETIME_FORMAT = "yyyy-MM-ddTHH:mm:ssZ";
        protected readonly String S_INVALID_DIRECTORIES = "|.|..|.forest|.fs|.metadata|" + FSConstants.DotSystem + "|.vfs|.uuid|";
        protected readonly String S_IMAGE_FILE = "/.~$file.gif";
        protected readonly String S_IMAGE_FOLDER = "/.~$folder.gif";
        protected readonly String S_IMAGE_SYMLINK = "/.~$symlink.gif";
        protected readonly String S_IMAGE_INLINEDATA = "/.~$inlinedata.gif";
        protected readonly String S_IMAGE_USERMETADATA = "/.~$usermetadata.gif";
        protected readonly String S_IMAGE_SYSTEMMETADATA = "/.~$systemmetadata.gif";


        protected Byte[] S_IMAGE_FILE_BYTEARRAY = new Byte[970] { 71, 73, 70, 56, 57, 97, 16, 0, 16, 0, 247, 0, 0, 122, 122, 122, 134, 134, 134, 144, 144, 144, 157, 157, 157, 172, 172, 172, 173, 173, 173, 177, 177, 177, 179, 179, 179, 181, 181, 181, 184, 184, 184, 186, 186, 186, 187, 187, 187, 188, 188, 188, 189, 189, 189, 191, 191, 191, 192, 192, 192, 197, 197, 197, 202, 202, 202, 224, 223, 223, 230, 229, 229, 241, 240, 240, 243, 241, 241, 242, 242, 241, 243, 242, 242, 244, 242, 242, 244, 243, 243, 245, 244, 243, 245, 244, 244, 246, 245, 245, 247, 246, 245, 246, 246, 246, 248, 246, 246, 248, 247, 247, 248, 248, 247, 249, 248, 248, 250, 249, 249, 250, 250, 249, 251, 250, 250, 252, 251, 250, 252, 251, 251, 252, 252, 251, 252, 252, 252, 254, 254, 253, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 44, 0, 0, 0, 0, 16, 0, 16, 0, 0, 8, 175, 0, 83, 164, 120, 64, 176, 160, 65, 17, 2, 19, 62, 56, 129, 66, 160, 10, 21, 40, 84, 68, 88, 96, 33, 97, 10, 7, 37, 24, 58, 124, 8, 193, 3, 3, 10, 9, 27, 144, 200, 216, 48, 197, 67, 5, 9, 18, 40, 72, 200, 64, 196, 72, 19, 37, 31, 78, 144, 176, 82, 160, 130, 16, 46, 75, 192, 76, 248, 48, 65, 194, 4, 31, 112, 142, 208, 25, 83, 133, 79, 129, 8, 60, 4, 21, 49, 180, 68, 76, 4, 9, 15, 112, 232, 176, 116, 196, 72, 20, 13, 15, 36, 52, 176, 97, 106, 213, 171, 40, 12, 36, 44, 144, 97, 195, 6, 170, 32, 152, 142, 60, 81, 32, 33, 1, 12, 101, 207, 126, 72, 155, 147, 64, 194, 1, 23, 224, 154, 229, 224, 129, 46, 137, 1, 9, 5, 84, 176, 0, 87, 67, 215, 190, 56, 5, 36, 12, 64, 97, 112, 225, 195, 32, 66, 4, 72, 8, 160, 178, 229, 203, 150, 83, 4, 4, 0, 59 };
        protected Byte[] S_IMAGE_FOLDER_BYTEARRAY = new Byte[1024] { 71, 73, 70, 56, 57, 97, 16, 0, 16, 0, 247, 0, 0, 73, 165, 211, 93, 185, 221, 195, 166, 67, 193, 166, 75, 196, 168, 69, 196, 169, 73, 198, 172, 78, 202, 175, 68, 200, 175, 84, 202, 176, 69, 204, 178, 71, 204, 179, 77, 206, 180, 74, 201, 178, 91, 208, 183, 78, 215, 184, 78, 210, 186, 82, 212, 184, 86, 212, 189, 86, 214, 190, 92, 215, 191, 93, 214, 191, 94, 216, 186, 82, 217, 188, 86, 218, 191, 92, 196, 176, 99, 203, 182, 97, 205, 185, 104, 206, 188, 110, 214, 191, 96, 208, 190, 116, 222, 193, 78, 215, 192, 90, 214, 192, 93, 215, 192, 94, 221, 193, 80, 223, 195, 82, 216, 192, 94, 217, 195, 95, 218, 196, 95, 217, 193, 96, 218, 194, 98, 219, 195, 99, 218, 196, 103, 220, 194, 99, 220, 197, 101, 222, 198, 105, 221, 200, 100, 223, 201, 107, 221, 201, 111, 209, 192, 120, 213, 198, 127, 220, 199, 114, 220, 199, 115, 218, 200, 126, 220, 205, 121, 224, 197, 86, 225, 199, 91, 227, 202, 96, 228, 205, 102, 224, 203, 105, 225, 202, 110, 226, 204, 111, 225, 206, 108, 224, 201, 112, 226, 204, 112, 225, 204, 118, 224, 206, 119, 228, 205, 114, 227, 207, 125, 230, 208, 109, 228, 209, 112, 231, 212, 117, 232, 211, 115, 234, 215, 122, 234, 213, 125, 233, 216, 121, 236, 219, 125, 219, 207, 172, 229, 210, 129, 228, 212, 128, 230, 214, 142, 232, 213, 135, 236, 215, 128, 238, 217, 131, 236, 218, 130, 239, 219, 134, 238, 222, 129, 237, 223, 135, 235, 217, 139, 232, 219, 137, 238, 222, 137, 241, 221, 137, 241, 222, 136, 234, 220, 165, 236, 225, 146, 240, 224, 132, 242, 224, 137, 242, 227, 136, 243, 229, 138, 244, 228, 141, 245, 230, 141, 245, 231, 142, 240, 226, 144, 242, 229, 151, 240, 231, 155, 243, 233, 158, 244, 234, 153, 235, 224, 180, 235, 225, 184, 244, 236, 163, 245, 236, 165, 247, 239, 172, 246, 238, 174, 248, 236, 174, 242, 232, 191, 247, 241, 170, 249, 242, 177, 249, 245, 176, 251, 246, 177, 249, 244, 182, 250, 244, 183, 251, 247, 180, 251, 247, 187, 250, 245, 188, 252, 249, 191, 246, 241, 198, 253, 250, 194, 236, 236, 236, 246, 242, 229, 248, 245, 234, 252, 250, 232, 253, 251, 233, 254, 253, 244, 252, 252, 251, 252, 252, 252, 255, 255, 252, 255, 255, 255, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 44, 0, 0, 0, 0, 16, 0, 16, 0, 0, 8, 229, 0, 15, 29, 146, 99, 134, 76, 152, 46, 92, 172, 80, 153, 178, 36, 203, 28, 129, 135, 12, 149, 185, 67, 104, 13, 150, 27, 124, 254, 104, 196, 35, 5, 226, 161, 50, 122, 244, 32, 66, 196, 99, 70, 156, 62, 123, 242, 16, 241, 8, 50, 100, 162, 23, 50, 158, 228, 169, 3, 39, 8, 203, 144, 34, 79, 120, 40, 2, 231, 141, 154, 30, 30, 199, 132, 180, 83, 8, 4, 7, 33, 106, 208, 156, 129, 225, 81, 140, 30, 59, 116, 6, 73, 216, 0, 228, 204, 150, 42, 62, 188, 64, 4, 3, 213, 141, 31, 8, 26, 92, 84, 81, 146, 196, 72, 11, 136, 87, 232, 184, 73, 243, 197, 65, 3, 22, 101, 119, 0, 82, 1, 177, 201, 218, 47, 90, 24, 32, 192, 176, 67, 71, 14, 64, 41, 32, 50, 97, 171, 5, 138, 2, 3, 23, 114, 224, 32, 1, 8, 5, 68, 36, 120, 161, 12, 73, 80, 192, 2, 137, 15, 31, 2, 148, 128, 120, 164, 240, 144, 24, 7, 8, 60, 192, 252, 1, 0, 5, 136, 63, 36, 199, 88, 113, 64, 192, 104, 204, 35, 106, 64, 140, 98, 34, 68, 133, 14, 7, 6, 68, 16, 65, 97, 2, 13, 54, 30, 5, 181, 177, 177, 32, 131, 147, 64, 30, 5, 6, 4, 0, 59 };
        protected Byte[] S_IMAGE_SYMLINK_BYTEARRAY = new Byte[1024] { 71, 73, 70, 56, 57, 97, 16, 0, 16, 0, 247, 0, 0, 117, 117, 118, 116, 118, 119, 118, 118, 121, 122, 122, 122, 126, 127, 127, 0, 56, 133, 2, 58, 135, 48, 93, 156, 127, 127, 128, 122, 148, 189, 131, 131, 131, 134, 134, 134, 136, 136, 137, 141, 141, 142, 144, 144, 144, 147, 147, 148, 148, 147, 148, 148, 148, 148, 148, 148, 149, 149, 151, 153, 157, 157, 157, 172, 172, 172, 173, 173, 173, 177, 177, 177, 179, 179, 179, 181, 181, 181, 184, 184, 184, 186, 186, 186, 187, 187, 187, 188, 188, 188, 189, 189, 189, 191, 191, 191, 134, 157, 194, 145, 166, 199, 157, 175, 205, 181, 194, 217, 192, 192, 192, 197, 197, 197, 202, 202, 202, 202, 208, 218, 204, 211, 220, 221, 221, 221, 224, 223, 223, 230, 229, 229, 233, 233, 233, 235, 235, 235, 240, 240, 240, 241, 242, 241, 247, 245, 245, 246, 246, 246, 248, 247, 247, 249, 248, 247, 249, 248, 248, 250, 249, 249, 250, 250, 249, 250, 250, 250, 251, 251, 251, 252, 251, 250, 252, 251, 251, 252, 252, 251, 252, 252, 252, 253, 253, 253, 254, 254, 253, 254, 254, 254, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 44, 0, 0, 0, 0, 16, 0, 16, 0, 0, 8, 196, 0, 121, 240, 32, 65, 176, 160, 65, 26, 2, 19, 146, 208, 177, 67, 160, 15, 31, 59, 124, 152, 224, 240, 34, 33, 143, 15, 55, 24, 58, 124, 88, 34, 70, 7, 23, 9, 61, 216, 200, 216, 144, 199, 195, 13, 26, 52, 108, 72, 216, 129, 198, 200, 28, 37, 31, 174, 80, 177, 82, 224, 134, 25, 46, 111, 192, 76, 248, 80, 3, 143, 0, 10, 16, 52, 128, 16, 65, 130, 209, 9, 27, 125, 2, 248, 209, 162, 135, 83, 167, 63, 36, 196, 204, 192, 99, 41, 139, 4, 5, 178, 22, 232, 33, 225, 198, 142, 134, 24, 170, 254, 96, 193, 35, 132, 86, 174, 35, 191, 94, 16, 203, 194, 236, 89, 8, 53, 70, 234, 176, 32, 22, 69, 129, 3, 33, 116, 108, 125, 64, 35, 103, 5, 177, 34, 12, 140, 184, 129, 67, 7, 143, 6, 50, 250, 218, 160, 32, 246, 4, 136, 2, 35, 18, 50, 136, 33, 3, 167, 3, 177, 41, 110, 104, 190, 33, 176, 1, 12, 202, 51, 22, 240, 16, 0, 160, 180, 233, 210, 4, 6, 168, 86, 205, 35, 32, 0, 59, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        protected Byte[] S_IMAGE_INLINEDATA_BYTEARRAY = new Byte[1024] { 71, 73, 70, 56, 57, 97, 16, 0, 16, 0, 247, 0, 0, 117, 117, 118, 116, 118, 119, 118, 118, 121, 0, 56, 133, 2, 58, 135, 48, 93, 156, 122, 147, 181, 73, 165, 211, 93, 185, 221, 137, 131, 111, 139, 134, 111, 142, 135, 110, 130, 128, 119, 132, 128, 112, 135, 131, 114, 137, 133, 115, 142, 136, 112, 143, 137, 115, 142, 137, 117, 136, 136, 127, 144, 139, 118, 145, 140, 121, 214, 190, 92, 215, 191, 93, 206, 188, 110, 208, 190, 116, 222, 193, 78, 215, 192, 90, 221, 193, 80, 223, 195, 82, 216, 192, 94, 218, 196, 95, 217, 193, 96, 218, 194, 98, 219, 195, 99, 220, 197, 101, 221, 200, 100, 223, 201, 107, 209, 192, 120, 213, 198, 127, 220, 199, 114, 220, 199, 115, 220, 205, 121, 224, 197, 86, 225, 199, 91, 227, 202, 96, 228, 205, 102, 224, 203, 105, 225, 202, 110, 226, 204, 111, 226, 204, 112, 225, 204, 118, 228, 205, 114, 227, 207, 125, 230, 208, 109, 232, 211, 115, 234, 215, 122, 234, 213, 125, 142, 141, 129, 139, 140, 140, 147, 145, 128, 147, 146, 130, 148, 148, 145, 142, 160, 182, 144, 164, 189, 134, 157, 194, 157, 175, 205, 178, 189, 203, 179, 190, 205, 229, 210, 129, 232, 213, 135, 236, 215, 128, 238, 217, 131, 236, 218, 130, 239, 219, 134, 237, 223, 135, 235, 217, 139, 238, 222, 137, 241, 221, 137, 241, 222, 136, 234, 220, 165, 242, 224, 137, 243, 229, 138, 244, 228, 141, 245, 230, 141, 245, 231, 142, 240, 226, 144, 242, 229, 151, 243, 233, 158, 244, 234, 153, 235, 224, 180, 245, 236, 165, 247, 239, 172, 246, 238, 174, 248, 236, 174, 242, 232, 191, 249, 242, 177, 249, 245, 176, 251, 246, 177, 249, 244, 182, 250, 244, 183, 251, 247, 180, 251, 247, 187, 250, 245, 188, 252, 249, 191, 218, 221, 215, 217, 222, 215, 238, 230, 203, 239, 231, 204, 238, 231, 206, 243, 233, 201, 240, 232, 203, 240, 233, 204, 244, 236, 207, 241, 235, 212, 243, 236, 208, 243, 236, 209, 243, 237, 210, 243, 237, 211, 244, 236, 208, 245, 237, 209, 244, 238, 211, 246, 238, 213, 253, 250, 194, 246, 240, 214, 247, 242, 215, 245, 241, 220, 248, 243, 216, 249, 243, 218, 249, 244, 219, 250, 245, 220, 250, 246, 221, 251, 247, 222, 236, 236, 236, 248, 244, 224, 251, 249, 228, 251, 249, 229, 252, 250, 230, 252, 250, 231, 253, 251, 232, 253, 251, 233, 253, 252, 234, 253, 251, 240, 254, 253, 244, 252, 252, 251, 252, 252, 252, 255, 255, 252, 255, 255, 255, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 44, 0, 0, 0, 0, 16, 0, 16, 0, 0, 8, 232, 0, 35, 69, 242, 82, 101, 74, 148, 39, 78, 148, 32, 57, 146, 131, 201, 23, 129, 145, 32, 81, 17, 195, 40, 203, 18, 21, 103, 246, 104, 28, 99, 4, 98, 36, 42, 101, 202, 72, 146, 244, 226, 68, 23, 52, 102, 200, 208, 240, 8, 50, 228, 36, 18, 38, 138, 144, 1, 195, 69, 6, 203, 144, 34, 63, 100, 168, 193, 101, 11, 22, 24, 30, 165, 132, 12, 243, 104, 3, 134, 25, 88, 174, 88, 41, 1, 49, 0, 131, 9, 58, 122, 248, 80, 240, 64, 2, 143, 38, 73, 98, 64, 17, 8, 128, 80, 163, 69, 137, 28, 205, 145, 195, 167, 2, 142, 27, 54, 70, 112, 29, 164, 200, 192, 128, 183, 3, 244, 80, 176, 225, 162, 144, 8, 174, 130, 16, 29, 2, 2, 55, 79, 132, 22, 44, 10, 133, 224, 26, 104, 47, 220, 1, 120, 32, 172, 232, 80, 8, 4, 87, 64, 106, 6, 20, 248, 1, 103, 192, 157, 5, 26, 52, 32, 240, 192, 245, 143, 16, 2, 68, 252, 188, 89, 227, 38, 65, 230, 3, 23, 184, 246, 73, 19, 100, 192, 16, 56, 108, 226, 52, 200, 204, 33, 5, 87, 67, 121, 234, 212, 177, 243, 166, 13, 29, 7, 23, 44, 160, 208, 34, 80, 0, 128, 227, 200, 143, 239, 240, 24, 41 };
        protected Byte[] S_IMAGE_USERMETADATA_BYTEARRAY = new Byte[1024] { 71, 73, 70, 56, 57, 97, 16, 0, 16, 0, 247, 0, 0, 122, 122, 122, 115, 135, 154, 134, 134, 134, 144, 144, 144, 157, 157, 157, 135, 158, 177, 132, 157, 180, 137, 161, 183, 146, 165, 181, 172, 172, 172, 173, 173, 173, 177, 177, 177, 179, 179, 179, 181, 181, 181, 184, 184, 184, 186, 186, 186, 187, 187, 187, 188, 188, 188, 189, 189, 189, 191, 191, 191, 150, 176, 199, 155, 178, 199, 164, 189, 209, 166, 190, 208, 185, 205, 224, 192, 192, 192, 197, 197, 197, 202, 202, 202, 198, 213, 223, 216, 216, 218, 200, 214, 224, 200, 217, 234, 224, 223, 223, 224, 225, 227, 230, 229, 229, 229, 229, 232, 234, 238, 242, 241, 240, 240, 242, 241, 241, 242, 242, 241, 242, 242, 242, 243, 243, 244, 243, 243, 246, 244, 242, 242, 244, 243, 243, 245, 244, 243, 245, 244, 244, 246, 245, 245, 246, 246, 246, 244, 246, 248, 248, 247, 246, 248, 247, 247, 248, 247, 248, 249, 248, 247, 249, 248, 248, 250, 249, 249, 250, 250, 249, 251, 250, 250, 251, 251, 251, 252, 251, 250, 252, 251, 251, 252, 252, 251, 252, 252, 252, 254, 254, 253, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 44, 0, 0, 0, 0, 16, 0, 16, 0, 0, 8, 202, 0, 125, 248, 200, 144, 65, 5, 65, 131, 5, 51, 216, 16, 200, 48, 3, 15, 9, 29, 124, 64, 236, 33, 97, 3, 132, 19, 12, 125, 76, 200, 17, 98, 132, 143, 142, 63, 66, 104, 128, 17, 161, 4, 67, 9, 56, 114, 240, 232, 33, 240, 199, 143, 7, 14, 28, 60, 96, 24, 193, 70, 202, 29, 44, 125, 184, 20, 1, 98, 166, 192, 0, 30, 128, 2, 245, 64, 180, 168, 3, 129, 36, 46, 36, 189, 112, 224, 3, 206, 150, 63, 142, 6, 80, 26, 160, 170, 5, 10, 67, 139, 54, 240, 17, 131, 68, 0, 23, 46, 100, 28, 8, 64, 35, 101, 15, 150, 12, 124, 76, 13, 192, 129, 131, 135, 177, 89, 137, 46, 16, 120, 33, 128, 9, 20, 46, 198, 166, 176, 113, 35, 37, 15, 5, 106, 17, 96, 56, 64, 184, 66, 129, 184, 30, 18, 8, 196, 128, 160, 106, 0, 3, 21, 94, 192, 152, 97, 195, 38, 1, 129, 58, 132, 6, 45, 202, 121, 0, 67, 1, 37, 76, 156, 88, 193, 162, 133, 11, 201, 51, 106, 8, 96, 8, 160, 181, 235, 215, 174, 125, 4, 4, 0, 59, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        protected Byte[] S_IMAGE_SYSTEMMETADATA_BYTEARRAY = new Byte[1024] { 71, 73, 70, 56, 57, 97, 16, 0, 16, 0, 247, 0, 0, 122, 122, 122, 115, 135, 154, 134, 134, 134, 144, 144, 144, 157, 157, 157, 135, 158, 177, 132, 157, 180, 137, 161, 183, 146, 165, 181, 172, 172, 172, 173, 173, 173, 177, 177, 177, 179, 179, 179, 181, 181, 181, 184, 184, 184, 186, 186, 186, 187, 187, 187, 188, 188, 188, 189, 189, 189, 191, 191, 191, 150, 176, 199, 155, 178, 199, 164, 189, 209, 166, 190, 208, 185, 205, 224, 192, 192, 192, 197, 197, 197, 202, 202, 202, 198, 213, 223, 216, 216, 218, 200, 214, 224, 200, 217, 234, 224, 223, 223, 224, 225, 227, 230, 229, 229, 229, 229, 232, 234, 238, 242, 241, 240, 240, 242, 241, 241, 242, 242, 241, 242, 242, 242, 243, 243, 244, 243, 243, 246, 244, 242, 242, 244, 243, 243, 245, 244, 243, 245, 244, 244, 246, 245, 245, 246, 246, 246, 244, 246, 248, 248, 247, 246, 248, 247, 247, 248, 247, 248, 249, 248, 247, 249, 248, 248, 250, 249, 249, 250, 250, 249, 251, 250, 250, 251, 251, 251, 252, 251, 250, 252, 251, 251, 252, 252, 251, 252, 252, 252, 254, 254, 253, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 44, 0, 0, 0, 0, 16, 0, 16, 0, 0, 8, 202, 0, 125, 248, 200, 144, 65, 5, 65, 131, 5, 51, 216, 16, 200, 48, 3, 15, 9, 29, 124, 64, 236, 33, 97, 3, 132, 19, 12, 125, 76, 200, 17, 98, 132, 143, 142, 63, 66, 104, 128, 17, 161, 4, 67, 9, 56, 114, 240, 232, 33, 240, 199, 143, 7, 14, 28, 60, 96, 24, 193, 70, 202, 29, 44, 125, 184, 20, 1, 98, 166, 192, 0, 30, 128, 2, 245, 64, 180, 168, 3, 129, 36, 46, 36, 189, 112, 224, 3, 206, 150, 63, 142, 6, 80, 26, 160, 170, 5, 10, 67, 139, 54, 240, 17, 131, 68, 0, 23, 46, 100, 28, 8, 64, 35, 101, 15, 150, 12, 124, 76, 13, 192, 129, 131, 135, 177, 89, 137, 46, 16, 120, 33, 128, 9, 20, 46, 198, 166, 176, 113, 35, 37, 15, 5, 106, 17, 96, 56, 64, 184, 66, 129, 184, 30, 18, 8, 196, 128, 160, 106, 0, 3, 21, 94, 192, 152, 97, 195, 38, 1, 129, 58, 132, 6, 45, 202, 121, 0, 67, 1, 37, 76, 156, 88, 193, 162, 133, 11, 201, 51, 106, 8, 96, 8, 160, 181, 235, 215, 174, 125, 4, 4, 0, 59, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        #endregion

        #region IGraphDSWebDAV_Service Members

        public void DoPROPFIND(string destination)
        {

            var content = new Byte[0];
            var _Body = HTTPServer.HTTPContext.RequestBody;
            var header = HTTPServer.HTTPContext.RequestHeader;
            HTTPHeader respHeader = null;

            var destinationObjectStreamTypes = GetDestinationObjectStreamTypes(header);

            if (header.HttpStatusCode != HTTPStatusCodes.OK)
            {
                respHeader = CreateHeader(HTTPStatusCodes.NotImplemented, 0);
            }
            else if (destinationObjectStreamTypes == null)
            {
                respHeader = CreateHeader(HTTPStatusCodes.NotFound, 0);
            }
            else
            {
                PropfindProperties PropfindProperties = PropfindProperties.NONE;
                if (_Body.Length > 0)
                {
                    PropfindProperties = ParsePropfindBody(_Body);
                }

                content = CreatePropfindResponse(header, PropfindProperties, destinationObjectStreamTypes);

                // Clients may submit a Depth Header with a value of "0", "1", "1,noroot" or "infinity". A PROPFIND Method without a Depth Header acts as if a Depth Header value of "infinity" was included.
                respHeader = CreateHeader(HTTPStatusCodes.MultiStatus, content.ULongLength(), new ContentType(MediaTypeNames.Text.Xml + "; charset=utf-8"));

            }

            Byte[] HeaderBytes = respHeader.ToBytes();

            //Byte[] Response = new Byte[HeaderBytes.Length + content.Length];

            HTTPServer.HTTPContext.WriteToResponseStream(HeaderBytes, 0, HeaderBytes.Length);
            HTTPServer.HTTPContext.WriteToResponseStream(content, 0, content.Length);

            //Array.Copy(HeaderBytes, Response, HeaderBytes.Length);
            //Array.Copy(content, 0, Response, HeaderBytes.Length, content.Length);

            //HTTPServer.HTTPContext.WriteToResponseStream(Response, 0, Response.Length);
        }

        public void DoMKCOL(string destination)
        {
            
            var content = new Byte[0];
            var _Body = HTTPServer.HTTPContext.RequestBody;
            var header = HTTPServer.HTTPContext.RequestHeader;
            HTTPHeader respHeader = null;

            var destinationObjectStreamTypes = GetDestinationObjectStreamTypes(header);

            if (header.HttpStatusCode != HTTPStatusCodes.OK)
            {
                respHeader = CreateHeader(HTTPStatusCodes.NotImplemented, 0);
            }
            
            #region MKCOL - Create Directory

            if (_IGraphFSSession.isIDirectoryObject(new ObjectLocation(header.Destination)).Value != Trinary.TRUE)
            {

                var _CreateDirectoryExceptional = _IGraphFSSession.CreateDirectoryObject(new ObjectLocation(header.Destination));

                if (_CreateDirectoryExceptional == null || _CreateDirectoryExceptional.Failed)
                {
                    respHeader = CreateHeader(HTTPStatusCodes.FailedDependency, content.ULongLength(), new ContentType(MediaTypeNames.Text.Plain + "; charset=utf-8"));
                }

                else
                {
                    respHeader = CreateHeader(HTTPStatusCodes.Created, content.ULongLength(), new ContentType(MediaTypeNames.Text.Plain + "; charset=utf-8"));
                    respHeader.Headers.Add("Location", header.FullHTTPDestinationPath());
                }

            }
            else
            {
                respHeader = CreateHeader(HTTPStatusCodes.PreconditionFailed, content.ULongLength(), new ContentType(MediaTypeNames.Text.Plain + "; charset=utf-8"));
            }

            #endregion

            Byte[] HeaderBytes = respHeader.ToBytes();

            //Byte[] Response = new Byte[HeaderBytes.Length + content.Length];

            HTTPServer.HTTPContext.WriteToResponseStream(HeaderBytes, 0, HeaderBytes.Length);
            HTTPServer.HTTPContext.WriteToResponseStream(content, 0, content.Length);
        }

        public void DoDELETE(string destination)
        {

            var content = new Byte[0];
            var _Body = HTTPServer.HTTPContext.RequestBody;
            var header = HTTPServer.HTTPContext.RequestHeader;
            HTTPHeader respHeader = null;

            if (_IGraphFSSession.ObjectExists(new ObjectLocation(header.Destination)).Value == Trinary.TRUE)
            {
                var isDir = _IGraphFSSession.isIDirectoryObject(new ObjectLocation(header.Destination));
                if (isDir.Success && isDir.Value == Trinary.TRUE)
                {
                    _IGraphFSSession.RemoveDirectoryObject(new ObjectLocation(header.Destination), true);
                    respHeader = CreateHeader(HTTPStatusCodes.NoContent, content.ULongLength());
                }
                else
                {

                    var isFile = _IGraphFSSession.ObjectStreamExists(new ObjectLocation(header.Destination), FSConstants.FILESTREAM);
                    if (isFile.Success && isFile.Value == Trinary.TRUE)
                    {
                        _IGraphFSSession.RemoveFSObject(new ObjectLocation(header.Destination), FSConstants.FILESTREAM, null, null);
                        respHeader = CreateHeader(HTTPStatusCodes.NoContent, content.ULongLength());
                    }
                    else
                    {
                        #region NotImplemented
                        respHeader = CreateHeader(HTTPStatusCodes.NotImplemented, content.ULongLength(), new ContentType(MediaTypeNames.Text.Plain + "; charset=utf-8"));
                        #endregion
                    }
                }
            }
            else
            {
                respHeader = CreateHeader(HTTPStatusCodes.PreconditionFailed, content.ULongLength(), new ContentType(MediaTypeNames.Text.Plain + "; charset=utf-8"));
            }

            Byte[] HeaderBytes = respHeader.ToBytes();
            HTTPServer.HTTPContext.WriteToResponseStream(HeaderBytes, 0, HeaderBytes.Length);
            HTTPServer.HTTPContext.WriteToResponseStream(content, 0, content.Length);
        }

        public void DoGET(string destination)
        {

            var content = new Byte[0];
            var _Body = HTTPServer.HTTPContext.RequestBody;
            var header = HTTPServer.HTTPContext.RequestHeader;
            HTTPHeader respHeader = null;
            
            var _DestinationObjectStreamTypes = GetDestinationObjectStreamTypes(header);
            if (_DestinationObjectStreamTypes == null)
            {
                respHeader = CreateHeader(HTTPStatusCodes.NotFound, 0);
            }
            else if (_DestinationObjectStreamTypes.Contains(FSConstants.INLINEDATA))
            {
                content = CreateGetInlineDataResponse(DirectoryHelper.GetObjectPath(header.Destination), DirectoryHelper.GetObjectName(header.Destination));

                respHeader = CreateHeader(HTTPStatusCodes.OK, content.ULongLength(), new ContentType(MediaTypeNames.Application.Octet));
            }
            else if (_DestinationObjectStreamTypes.Contains(FSConstants.FILESTREAM))
            {
                content = CreateGetFileResponse(header);

                respHeader = CreateHeader(HTTPStatusCodes.OK, content.ULongLength(), new ContentType(MediaTypeNames.Application.Octet));
            }
            else
            {
                respHeader = CreateHeader(HTTPStatusCodes.BadRequest, content.ULongLength(), new ContentType(MediaTypeNames.Application.Octet));
            }

            Byte[] HeaderBytes = respHeader.ToBytes();
            HTTPServer.HTTPContext.WriteToResponseStream(HeaderBytes, 0, HeaderBytes.Length);
            HTTPServer.HTTPContext.WriteToResponseStream(content, 0, content.Length);
        }

        public void DoPUT(string destination)
        {

            var content = new Byte[0];
            var _Body = HTTPServer.HTTPContext.RequestBody;
            var header = HTTPServer.HTTPContext.RequestHeader;
            HTTPHeader respHeader = null;
            
            #region Second PUT request containing the Data - the Header contains an If condition with the LockToken

            if (header.Headers["If"] != null)
            {

                String LockToken = RessourceLock.ParseToken(header.Headers["If"]);

                if (RessourceLock.Contains(LockToken))
                {

                    try
                    {

                        _IGraphFSSession.StoreFSObject(new FileObject() { ObjectLocation = new ObjectLocation(header.Destination), ObjectData = _Body }, true);

                    }
                    catch (Exception Ex)
                    {
                        Console.WriteLine(Ex);
                        throw Ex;
                    }

                    respHeader = CreateHeader(HTTPStatusCodes.OK, content.ULongLength(), new ContentType(MediaTypeNames.Text.Plain + "; charset=utf-8"));
                    respHeader.Headers.Add("Lock-Token", String.Concat("opaquelocktoken:", LockToken, TimestampNonce.AsString(S_DATETIME_FORMAT)));

                }
                else
                {

                    respHeader = CreateHeader(HTTPStatusCodes.PreconditionFailed, content.ULongLength(), new ContentType(MediaTypeNames.Text.Plain + "; charset=utf-8"));

                }

                //if (HeaderInfos["Lock-Token"].Split(new char[]{))

            }

            #endregion

            else

            #region First PUT request - just create an empty FILESTREAM ressource

            {
                Exceptional<Trinary> FileExists = null;
                try
                {
                    FileExists = _IGraphFSSession.ObjectStreamExists(new ObjectLocation(FSConstants.FILESTREAM), header.Destination);
                }
                catch (PandoraFSException_ObjectNotFound E)
                {
                    //_Logger.ErrorException(Header.Destination, E);
                    respHeader = CreateHeader(HTTPStatusCodes.Conflict, content.ULongLength(), new ContentType(MediaTypeNames.Text.Plain + "; charset=utf-8"));
                }

                if (FileExists != null && FileExists.Success && FileExists.Value != Trinary.TRUE)
                {

                    #region Store file

                    try
                    {

                        _IGraphFSSession.StoreFSObject(new FileObject() { ObjectLocation = new ObjectLocation(header.Destination), ObjectData = _Body }, true);

                    }
                    catch (Exception Ex)
                    {
                        Console.WriteLine(Ex);
                        throw Ex;
                    }

                    // This is just a dummy LockToken
                    String LockToken = RessourceLock.CreateLockToken();
                    //RessourceLock.LockRessource(LockToken, _Destination);

                    respHeader = CreateHeader(HTTPStatusCodes.Created, content.ULongLength(), new ContentType(MediaTypeNames.Text.Plain + "; charset=utf-8"));
                    respHeader.Headers.Add("Location", header.FullHTTPDestinationPath());
                    respHeader.Headers.Add("Lock-Token", String.Concat("opaquelocktoken:", LockToken, TimestampNonce.AsString(S_DATETIME_FORMAT)));
                    
                    #endregion

                }
                else
                {
                
                    #region The PUT request neither contains the If condition nor the file is new and does not exist

                    respHeader = CreateHeader(HTTPStatusCodes.PreconditionFailed, content.ULongLength(), new ContentType(MediaTypeNames.Text.Plain + "; charset=utf-8"));
                    
                    #endregion

                }

            }

            #endregion

            Byte[] HeaderBytes = respHeader.ToBytes();
            HTTPServer.HTTPContext.WriteToResponseStream(HeaderBytes, 0, HeaderBytes.Length);
            HTTPServer.HTTPContext.WriteToResponseStream(content, 0, content.Length);
  
        }

        public void DoOPTIONS(string destination)
        {

            var content = new Byte[0];
            var body = HTTPServer.HTTPContext.RequestBody;
            var header = HTTPServer.HTTPContext.RequestHeader;
            HTTPHeader respHeader = null;
            
            #region OPTIONS

            if (header.ClientType == ClientTypes.SVN)
            {

                content = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"utf-8\"?><D:options-response xmlns:D=\"DAV:\"><D:activity-collection-set><D:href>" + DirectoryHelper.Combine(header.Destination, "!svn/act/") + "</D:href></D:activity-collection-set></D:options-response>");

                respHeader = CreateHeader(HTTPStatusCodes.OK, content.ULongLength(), new ContentType(MediaTypeNames.Text.Xml + "; charset=utf-8"));

                respHeader.Headers.Add("Allow", "OPTIONS, GET, HEAD, POST, DELETE, TRACE, PROPFIND, PROPPATCH, COPY, MOVE, LOCK, UNLOCK, CHECKOUT ");
                respHeader.Headers.Add("DAV", "version-control,checkout,working-resource");
                respHeader.Headers.Add("DAV", "http://subversion.tigris.org/xmlns/dav/svn/depth");
                respHeader.Headers.Add("DAV", "http://subversion.tigris.org/xmlns/dav/svn/log-revprops");
                respHeader.Headers.Add("DAV", "http://subversion.tigris.org/xmlns/dav/svn/partial-replay");
                respHeader.Headers.Add("DAV", "http://subversion.tigris.org/xmlns/dav/svn/mergeinfo");
                respHeader.Headers.Add("DAV", "<http://apache.org/dav/propset/fs/1>");

            }
            else
            {

                respHeader = CreateHeader(HTTPStatusCodes.OK, 0, new ContentType(MediaTypeNames.Text.Plain + "; charset=utf-8"));
                //HeaderString.AppendLine("Allow: OPTIONS, GET, POST, HEAD, COPY, PROPFIND, BROWSE, INDEX, PUT, DELETE, MOVE, SAVE, MKCOL, MKDIR, RMDIR ");
                respHeader.Headers.Add("Allow", "OPTIONS, GET, HEAD, COPY, PROPFIND, PUT, MOVE, MKCOL, DELETE ");

            }

            #endregion

            Byte[] HeaderBytes = respHeader.ToBytes();
            HTTPServer.HTTPContext.WriteToResponseStream(HeaderBytes, 0, HeaderBytes.Length);
            HTTPServer.HTTPContext.WriteToResponseStream(content, 0, content.Length);
      
        }

        public void DoPROPPATCH(string destination)
        {

            var content = new Byte[0];
            var body = HTTPServer.HTTPContext.RequestBody;
            var header = HTTPServer.HTTPContext.RequestHeader;
            HTTPHeader respHeader = null;
            
            #region PROPPATCH

            #region PROPPATCH with an previous LOCK (from file creation)

            if (header.Headers["If"] != null)
            {
                String IfCondition = header.Headers["If"];
                if (IfCondition.Contains("opaquelocktoken"))
                {
                    String LockToken = RessourceLock.ParseToken(IfCondition);
                    if (RessourceLock.Contains(LockToken))
                    {

                        //TODO: Parse FileAttributes and save them
                        content = CreateProppatchResponse(header, "Win32FileAttributes", "Win32LastModifiedTime", "Win32CreationTime", "Win32LastAccessTime");

                        respHeader = CreateHeader(HTTPStatusCodes.MultiStatus, content.ULongLength(), new ContentType(MediaTypeNames.Text.Xml + "; charset=utf-8"));

                    }
                    else
                    {

                        respHeader = CreateHeader(HTTPStatusCodes.PreconditionFailed, content.ULongLength(), new ContentType(MediaTypeNames.Text.Plain + "; charset=utf-8"));

                    }
                }
                else
                {
                
                    respHeader = CreateHeader(HTTPStatusCodes.PreconditionFailed, content.ULongLength(), new ContentType(MediaTypeNames.Text.Plain + "; charset=utf-8"));

                }
            }

            #endregion

            #region PROPPATCH without an previous LOCK (from directory creation)

            else
            {

                //TODO: Parse FileAttributes and save them
                content = CreateProppatchResponse(header, "Win32FileAttributes", "Win32LastModifiedTime", "Win32CreationTime", "Win32LastAccessTime");

                respHeader = CreateHeader(HTTPStatusCodes.MultiStatus, content.ULongLength(), new ContentType(MediaTypeNames.Text.Xml + "; charset=utf-8"));

            }

            #endregion

            #endregion PROPPATCH

            Byte[] HeaderBytes = respHeader.ToBytes();
            HTTPServer.HTTPContext.WriteToResponseStream(HeaderBytes, 0, HeaderBytes.Length);
            HTTPServer.HTTPContext.WriteToResponseStream(content, 0, content.Length);
      
        }

        public void DoHEAD(string destination)
        {

            var content = new Byte[0];
            var body = HTTPServer.HTTPContext.RequestBody;
            var header = HTTPServer.HTTPContext.RequestHeader;
            HTTPHeader respHeader = null;
            
            #region HEAD - is a GET without a body

            var destinationObjectStreamTypes = GetDestinationObjectStreamTypes(header);
            if (destinationObjectStreamTypes == null)
            {
                respHeader = CreateHeader(HTTPStatusCodes.NotFound, 0);
            }
            if (destinationObjectStreamTypes.Contains(FSConstants.INLINEDATA))
            {
                respHeader = CreateHeader(HTTPStatusCodes.OK, content.ULongLength(), new ContentType(MediaTypeNames.Application.Octet));
            }
            else if (destinationObjectStreamTypes.Contains(FSConstants.FILESTREAM))
            {
                respHeader = CreateHeader(HTTPStatusCodes.OK, content.ULongLength(), new ContentType(MediaTypeNames.Application.Octet));
            }


            #endregion

            Byte[] HeaderBytes = respHeader.ToBytes();
            HTTPServer.HTTPContext.WriteToResponseStream(HeaderBytes, 0, HeaderBytes.Length);
            HTTPServer.HTTPContext.WriteToResponseStream(content, 0, content.Length);
      
        }

        /// <summary>
        /// Currently not implemented
        /// </summary>
        /// <param name="destination"></param>
        public void DoPOST(string destination)
        {

            var content = new Byte[0];
            var body = HTTPServer.HTTPContext.RequestBody;
            var header = HTTPServer.HTTPContext.RequestHeader;
            HTTPHeader respHeader = null;

            #region NotImplemented

            respHeader = CreateHeader(HTTPStatusCodes.NotImplemented, 0, new ContentType(MediaTypeNames.Text.Plain + "; charset=utf-8"));

            #endregion

            Byte[] HeaderBytes = respHeader.ToBytes();
            HTTPServer.HTTPContext.WriteToResponseStream(HeaderBytes, 0, HeaderBytes.Length);
            HTTPServer.HTTPContext.WriteToResponseStream(content, 0, content.Length);

        }

        public void DoCOPY(string destination)
        {

            var content = new Byte[0];
            var body = HTTPServer.HTTPContext.RequestBody;
            var header = HTTPServer.HTTPContext.RequestHeader;
            HTTPHeader respHeader = null;

            if (header.Headers["Destination"] != null)
            {
                String NewLocation = header.Headers["Destination"].Replace(header.GetFullHTTPHost(), "").Trim();
                /*
                Boolean Overwrite = false;
                if (Header.Headers["Overwrite"] != null && Header.Headers["Overwrite"] == "PT")
                    Overwrite = true;
                */
                #region Copy a Directory

                if (_IGraphFSSession.isIDirectoryObject(new ObjectLocation(header.Destination)).Value == Trinary.TRUE)
                {

                    if (_IGraphFSSession.isIDirectoryObject(new ObjectLocation(NewLocation)).Value != Trinary.TRUE)
                    {
                        _IGraphFSSession.CreateDirectoryObject(new ObjectLocation(NewLocation));

                        respHeader = CreateHeader(HTTPStatusCodes.Created, content.ULongLength(), new ContentType(MediaTypeNames.Text.Plain + "; charset=utf-8"));
                        respHeader.Headers.Add("Location", header.GetFullHTTPHost() + NewLocation);

                    }
                    else
                    {

                        respHeader = CreateHeader(HTTPStatusCodes.PreconditionFailed, content.ULongLength(), new ContentType(MediaTypeNames.Text.Plain + "; charset=utf-8"));

                    }
                }

                #endregion

                #region Copy a File

                else if (_IGraphFSSession.ObjectStreamExists(new ObjectLocation(FSConstants.FILESTREAM), header.Destination).Value)
                {

                    if (_IGraphFSSession.ObjectStreamExists(new ObjectLocation(FSConstants.FILESTREAM), NewLocation).Value != Trinary.TRUE)
                    {

                        AFSObject FileObject = _IGraphFSSession.GetFSObject<FileObject>(new ObjectLocation(header.Destination), FSConstants.FILESTREAM, null, null, 0, false).Value;
                        FileObject.ObjectLocation = new ObjectLocation(NewLocation);
                        _IGraphFSSession.StoreFSObject(FileObject, true);

                        respHeader = CreateHeader(HTTPStatusCodes.Created, content.ULongLength(), new ContentType(MediaTypeNames.Text.Plain + "; charset=utf-8"));
                        respHeader.Headers.Add("Location", header.GetFullHTTPHost() + NewLocation);

                    }
                    else
                    {

                        respHeader = CreateHeader(HTTPStatusCodes.PreconditionFailed, content.ULongLength(), new ContentType(MediaTypeNames.Text.Plain + "; charset=utf-8"));

                    }
                }

                #endregion
                
                else
                {

                    respHeader = CreateHeader(HTTPStatusCodes.NotFound, content.ULongLength(), new ContentType(MediaTypeNames.Text.Plain + "; charset=utf-8"));

                }
            }
            else
            {

                respHeader = CreateHeader(HTTPStatusCodes.PreconditionFailed, content.ULongLength(), new ContentType(MediaTypeNames.Text.Plain + "; charset=utf-8"));

            }

            Byte[] HeaderBytes = respHeader.ToBytes();
            HTTPServer.HTTPContext.WriteToResponseStream(HeaderBytes, 0, HeaderBytes.Length);
            HTTPServer.HTTPContext.WriteToResponseStream(content, 0, content.Length);

        }

        public void DoMOVE(string destination)
        {

            var content = new Byte[0];
            var body = HTTPServer.HTTPContext.RequestBody;
            var header = HTTPServer.HTTPContext.RequestHeader;
            HTTPHeader respHeader = null;


            if (header.Headers["Destination"] != null)
            {
                String NewLocation = header.Headers["Destination"].Replace(header.GetFullHTTPHost(), "").Trim();

                //Boolean Overwrite = false;
                //if (header.Headers["Overwrite"] != null && header.Headers["Overwrite"] == "PT")
                //    Overwrite = true;

                #region Move a Directory

                if (_IGraphFSSession.isIDirectoryObject(new ObjectLocation(header.Destination)).Value == Trinary.TRUE)
                {

                    if (_IGraphFSSession.isIDirectoryObject(new ObjectLocation(NewLocation)).Value != Trinary.TRUE)
                    {
                        //_IGraphFSSession.CreateDirectoryObject(new ObjectLocation(NewLocation));
                        //_IGraphFSSession.RemoveDirectoryObject(new ObjectLocation(Header.Destination), true);

                        _IGraphFSSession.RenameFSObject(new ObjectLocation(header.Destination), new ObjectLocation(NewLocation).Name);

                        // TODO: Remove old Directory
                        //_PandoraVFS.DeleteDirectoryObject(Header.Destination);

                        respHeader = CreateHeader(HTTPStatusCodes.Created, content.ULongLength(), new ContentType(MediaTypeNames.Text.Plain + "; charset=utf-8"));
                        respHeader.Headers.Add("Location", header.GetFullHTTPHost() + NewLocation);

                    }
                    else
                    {

                        respHeader = CreateHeader(HTTPStatusCodes.PreconditionFailed, content.ULongLength(), new ContentType(MediaTypeNames.Text.Plain + "; charset=utf-8"));

                    }

                }

                #endregion

                #region Move a File

                else if (_IGraphFSSession.ObjectStreamExists(new ObjectLocation(header.Destination), FSConstants.FILESTREAM).Value)
                {

                    if (_IGraphFSSession.ObjectStreamExists(new ObjectLocation(NewLocation), FSConstants.FILESTREAM).Value != Trinary.TRUE)
                    {

                        //APandoraObject FileObject = _IGraphFSSession.GetObject<FileObject>(new ObjectLocation(Header.Destination), FSConstants.FILESTREAM, null, null, 0, false);
                        //FileObject.ObjectLocation = new ObjectLocation(NewLocation);
                        //_IGraphFSSession.StoreObject(FileObject, Overwrite);
                        //_IGraphFSSession.RemoveObject(new ObjectLocation(Header.Destination), FSConstants.FILESTREAM, null, null);

                        _IGraphFSSession.RenameFSObject(new ObjectLocation(header.Destination), new ObjectLocation(NewLocation).Name);

                        respHeader = CreateHeader(HTTPStatusCodes.Created, content.ULongLength(), new ContentType(MediaTypeNames.Text.Plain + "; charset=utf-8"));
                        respHeader.Headers.Add("Location", header.GetFullHTTPHost() + NewLocation);


                    }
                    else
                    {

                        respHeader = CreateHeader(HTTPStatusCodes.PreconditionFailed, content.ULongLength(), new ContentType(MediaTypeNames.Text.Plain + "; charset=utf-8"));

                    }

                }

                #endregion
                
                else
                {

                    respHeader = CreateHeader(HTTPStatusCodes.NotFound, content.ULongLength(), new ContentType(MediaTypeNames.Text.Plain + "; charset=utf-8"));

                }

            }
            else
            {

                respHeader = CreateHeader(HTTPStatusCodes.PreconditionFailed, content.ULongLength(), new ContentType(MediaTypeNames.Text.Plain + "; charset=utf-8"));

            }

            Byte[] HeaderBytes = respHeader.ToBytes();
            HTTPServer.HTTPContext.WriteToResponseStream(HeaderBytes, 0, HeaderBytes.Length);
            HTTPServer.HTTPContext.WriteToResponseStream(content, 0, content.Length);

        }

        public void DoLOCK(string destination)
        {

            var content = new Byte[0];
            var body = HTTPServer.HTTPContext.RequestBody;
            var header = HTTPServer.HTTPContext.RequestHeader;
            HTTPHeader respHeader = null;

            if (RessourceLock.RessourceIsLocked(header.Destination))
            {
                respHeader = CreateHeader(HTTPStatusCodes.Locked, 0, new ContentType(MediaTypeNames.Text.Plain + "; charset=utf-8"));
            }
            else
            {

                // A successful lock request to an unmapped URL MUST result in the creation of a locked (non-collection)
                // resource with empty content
                if (_IGraphFSSession.ObjectStreamExists(new ObjectLocation(header.Destination), FSConstants.FILESTREAM).Value != Trinary.TRUE)
                    _IGraphFSSession.StoreFSObject(new FileObject() { ObjectLocation = new ObjectLocation(header.Destination), ObjectData = new Byte[0] }, true);

                //sones.Pandora.Lib.Networking 
                content = CreateLockResponse(header, body, header.GetDepth());

                respHeader = CreateHeader(HTTPStatusCodes.OK, content.ULongLength(), new ContentType(MediaTypeNames.Text.Plain + "; charset=utf-8"));

            }

            Byte[] HeaderBytes = respHeader.ToBytes();
            HTTPServer.HTTPContext.WriteToResponseStream(HeaderBytes, 0, HeaderBytes.Length);
            HTTPServer.HTTPContext.WriteToResponseStream(content, 0, content.Length);

        }

        public void DoUNLOCK(string destination)
        {

            var content = new Byte[0];
            var body = HTTPServer.HTTPContext.RequestBody;
            var header = HTTPServer.HTTPContext.RequestHeader;
            HTTPHeader respHeader = null;

            if (header.Headers["Lock-Token"] != null)
            {

                String LockToken = RessourceLock.ParseToken(header.Headers["Lock-Token"]);
                if (RessourceLock.Contains(LockToken))
                {
                    RessourceLock.UnLockRessource(LockToken);

                    respHeader = CreateHeader(HTTPStatusCodes.OK, content.ULongLength(), new ContentType(MediaTypeNames.Text.Plain + "; charset=utf-8"));
                }
                else
                {
                    respHeader = CreateHeader(HTTPStatusCodes.PreconditionFailed, content.ULongLength(), new ContentType(MediaTypeNames.Text.Plain + "; charset=utf-8"));
                }

            }

            Byte[] HeaderBytes = respHeader.ToBytes();
            HTTPServer.HTTPContext.WriteToResponseStream(HeaderBytes, 0, HeaderBytes.Length);
            HTTPServer.HTTPContext.WriteToResponseStream(content, 0, content.Length);

        }

        /// <summary>
        /// Currently not implemented
        /// </summary>
        /// <param name="destination"></param>
        public void DoTRACE(string destination)
        {

            var content = new Byte[0];
            var body = HTTPServer.HTTPContext.RequestBody;
            var header = HTTPServer.HTTPContext.RequestHeader;
            HTTPHeader respHeader = null;

            #region NotImplemented

            respHeader = CreateHeader(HTTPStatusCodes.NotImplemented, 0, new ContentType(MediaTypeNames.Text.Plain + "; charset=utf-8"));

            #endregion

            Byte[] HeaderBytes = respHeader.ToBytes();
            HTTPServer.HTTPContext.WriteToResponseStream(HeaderBytes, 0, HeaderBytes.Length);
            HTTPServer.HTTPContext.WriteToResponseStream(content, 0, content.Length);

        }


        #endregion

        #region Fields

        IGraphFSSession _IGraphFSSession;

        #endregion

        #region Constructors

        public GraphDSWebDAV_Service()
        {
        }

        public GraphDSWebDAV_Service(IGraphFSSession graphFSSession)
        {
            _IGraphFSSession = graphFSSession;
        }

        #endregion

        #region Response

        private HashSet<String> GetDestinationObjectStreamTypes(HTTPHeader header)
        {
            // check validity of folder
            try
            {

                var streams = _IGraphFSSession.GetObjectStreams(new ObjectLocation(header.Destination));
                if (streams.Failed)
                {
                    return null;
                }

                return new HashSet<string>(streams.Value);

                // We found a InlineData Element
                //if (_DestinationObjectStreamTypes.Contains(FSConstants.INLINEDATA))
                //    _DestinationObjectLocator = _IGraphFSSession.ExportObjectLocator(new ObjectLocation(DirectoryHelper.GetObjectPath(Header.Destination)));
                //else
                //    _DestinationObjectLocator = _IGraphFSSession.ExportObjectLocator(new ObjectLocation(Header.Destination));
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Create a response without any body. You should use this for any error 
        /// </summary>
        /// <param name="HTTPStatusCodes"></param>
        /// <returns>The Response without any body (content)</returns>
        private Byte[] GetResponseWithoutBody(HTTPStatusCodes httpStatusCodes)
        {

            return CreateHeader(httpStatusCodes, 0).ToBytes();

        }

        /// <summary>
        /// Parses a webDAV request body for some properties and return them
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        private PropfindProperties ParsePropfindBody(Byte[] body)
        {
            PropfindProperties FoundPropfindProperties = PropfindProperties.NONE;
            String BodyString = Encoding.UTF8.GetString(body);

            try
            {
                XmlDocument XmlDocument = new XmlDocument();
                XmlDocument.LoadXml(BodyString);

                String NamespacePrefix = XmlDocument.GetPrefixOfNamespace(S_DAV_NAMESPACE_URI);
                if (NamespacePrefix == "") NamespacePrefix = S_DAV_PREFIX;

                XmlNamespaceManager XmlNamespaceManager = new XmlNamespaceManager(XmlDocument.NameTable);
                XmlNamespaceManager.AddNamespace(NamespacePrefix, S_DAV_NAMESPACE_URI);

                if (XmlDocument.SelectSingleNode(String.Concat("/", NamespacePrefix, ":propfind/", NamespacePrefix, ":allprop"), XmlNamespaceManager) != null)
                    return PropfindProperties.AllProp;

                XmlNode Node = XmlDocument.SelectSingleNode(String.Concat("/",NamespacePrefix,":propfind/",NamespacePrefix,":prop"), XmlNamespaceManager);

                if (Node != null)
                {
                    foreach (XmlNode ChildNodes in Node.ChildNodes)
                    {
                        String Prop = ChildNodes.LocalName;
                        switch (Prop.ToLower())
                        {
                            case "creationdate":
                                FoundPropfindProperties |= PropfindProperties.Creationdate;
                                break;
                            case "displayname":
                                FoundPropfindProperties |= PropfindProperties.Displayname;
                                break;
                            case "getcontentlanguage":
                                FoundPropfindProperties |= PropfindProperties.Getcontentlanguage;
                                break;
                            case "getcontentlength":
                                FoundPropfindProperties |= PropfindProperties.Getcontentlength;
                                break;
                            case "getcontenttype":
                                FoundPropfindProperties |= PropfindProperties.Getcontenttype;
                                break;
                            case "getetag":
                                FoundPropfindProperties |= PropfindProperties.Getetag;
                                break;
                            case "getlastmodified":
                                FoundPropfindProperties |= PropfindProperties.Getlastmodified;
                                break;
                            case "lockdiscovery":
                                FoundPropfindProperties |= PropfindProperties.Lockdiscovery;
                                break;
                            case "resourcetype":
                                FoundPropfindProperties |= PropfindProperties.Resourcetype;
                                break;
                            case "supportedlock":
                                FoundPropfindProperties |= PropfindProperties.Supportedlock;
                                break;

                            #region SVN

                            case "version-controlled-configuration":
                                FoundPropfindProperties |= PropfindProperties.VersionControlledConfiguration;
                                break;
                            case "baseline-relative-path":
                                FoundPropfindProperties |= PropfindProperties.BaselineRelativePath;
                                break;
                            case "repository-uuid":
                                FoundPropfindProperties |= PropfindProperties.RepositoryUuid;
                                break;
                            case "checked-in":
                                FoundPropfindProperties |= PropfindProperties.CheckedIn;
                                break;
                            case "baseline-collection":
                                FoundPropfindProperties |= PropfindProperties.BaselineCollection;
                                break;
                            case "version-Name":
                                FoundPropfindProperties |= PropfindProperties.VersionName;
                                break;
                            case "creator-displayname":
                                FoundPropfindProperties |= PropfindProperties.CreatorDisplayname;
                                break;
                            case "deadprop-count":
                                FoundPropfindProperties |= PropfindProperties.DeadpropCount;
                                break;

                            #endregion

                            default:
                                //FoundPropfindProperties |= PropfindProperties.NONE;
                                break;
                        }
                        
                    }
                }
            }
            catch
            {
                return PropfindProperties.NONE;
            }

            return FoundPropfindProperties;
        }

        #region Response Elements Header + Body

        #region CreateHeader(HTTPStatusCode, ContentLength)

        /// <summary>
        ///  Create the Header for a Response
        /// </summary>
        /// <param name="httpStatusCode">The HTTP Status of the Request (200 OK | 207 MULTI-STATUS)</param>
        /// <param name="contentLength">The Body length</param>
        /// <returns></returns>
        private HTTPHeader CreateHeader(HTTPStatusCodes httpStatusCode, UInt64 contentLength, ContentType contentType = null)
        {
            var header = HTTPServer.HTTPContext.ResponseHeader;
            header.HttpStatusCode = httpStatusCode;
            header.CacheControl = "no-cache";

            header.ServerName = "sones GraphDS WebDAV";
            header.ContentLength = contentLength;
            header.Headers.Add("DAV", "1, 2, 3");

            if (contentType != null)
            {
                header.ContentType = contentType;
            }

            // MS specific: DAV and MS-FP/4.0 (refers to Microsoft FrontPage Server)
            // header.Headers.Add("MS-Author-Via, "DAV");

            if (HTTPServer.HTTPContext.RequestHeader.KeepAlive)
            {
                header.KeepAlive = true;
            }

            return header;

        }

        #endregion

        #region WebDAV Method GET

        private Byte[] CreateGetFileResponse(HTTPHeader header, params string[] properties)
        {

            return _IGraphFSSession.GetFSObject<FileObject>(new ObjectLocation(header.Destination), FSConstants.FILESTREAM, null, null, 0, false).Value.ObjectData;

        }

        private Byte[] CreateGetInlineDataResponse(params string[] properties)
        {
            DirectoryObject DirectoryObject = _IGraphFSSession.GetFSObject<DirectoryObject>(new ObjectLocation(properties[0]), FSConstants.DIRECTORYSTREAM, null, null, 0, false).Value;

            return Encoding.ASCII.GetBytes(DirectoryObject.GetInlineData(properties[1]).ToHexString());

        }

        #endregion

        #region WebDAV Method PROPFIND

        /// <summary>
        /// Create a repsonse for a PROPFIND request (Directory(File)-Listing
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        private Byte[] CreatePropfindResponse(HTTPHeader header, PropfindProperties propfindProperties, HashSet<String> _DestinationObjectStreamTypes, params string[] properties)
        {

            #region Create XmlDocument

            XmlDocument XmlDocument = new XmlDocument();

            //XmlDeclaration xmlDeclaration = XmlDocument.CreateXmlDeclaration("1.0", "utf-8", "");
            //XmlDocument.AppendChild(xmlDeclaration);

            
            XmlElement Root = XmlDocument.CreateElement(S_DAV_PREFIX, "multistatus", S_DAV_NAMESPACE_URI);
            //Root.SetAttribute("xmlns:" + S_SVN_PREFIX, S_SVN_NAMESPACE_URI);

            if (header.GetDepth() == WebDAVDepth.Depth0)
            {
                Root.AppendChild(CreateDepth0Response(header, XmlDocument, propfindProperties, _DestinationObjectStreamTypes));
            }

            Boolean IsLegalDir = true;
            if (S_INVALID_DIRECTORIES.Contains(String.Concat("|", DirectoryHelper.GetObjectName(header.Destination), "|")))
                IsLegalDir = false;

            var directoryObjectR = _IGraphFSSession.GetFSObject<DirectoryObject>(new ObjectLocation(header.Destination), FSConstants.DIRECTORYSTREAM, null, null, 0, false);
            // uncommented because _IGraphFSSession.isIDirectoryObject is odd
            //if (IsLegalDir && _IGraphFSSession.isIDirectoryObject(new ObjectLocation(header.Destination)) == Trinary.TRUE)
            if (IsLegalDir && directoryObjectR.Success)
            {
                #region root elements

                if (header.GetDepth() != WebDAVDepth.Depth0)
                {
                    // Get Content of the Current Directory
                    var DirectoryObject = directoryObjectR.Value;
                    foreach (DirectoryEntryInformation actualDirectoryEntry in DirectoryObject.GetExtendedDirectoryListing())
                    {
                        //if (((String)DirectoryEntries["ObjectName"]).Contains(".forest") || ((String)DirectoryEntries["ObjectName"]).Contains(".fs") || ((String)DirectoryEntries["ObjectName"]).Contains(".metadata") || ((String)DirectoryEntries["ObjectName"]).Contains(".revisions") || ((String)DirectoryEntries["ObjectName"]).Contains(".vfs"))
                        if (S_INVALID_DIRECTORIES.Contains(String.Concat("|", actualDirectoryEntry.Name)))
                            continue;

                        String ObjectDestination = header.Destination + (header.Destination.EndsWith("/") ? "" : FSPathConstants.PathDelimiter) + actualDirectoryEntry.Name;

                        if (actualDirectoryEntry.Streams.Contains(FSConstants.DIRECTORYSTREAM))
                        {
                            try
                            {
                                IDirectoryObject CurDirectoryObject = _IGraphFSSession.GetFSObject<DirectoryObject>(new ObjectLocation(ObjectDestination), FSConstants.DIRECTORYSTREAM, null, null, 0, false).Value;
                                String HRef = header.FullHTTPDestinationPath() + (header.FullHTTPDestinationPath().EndsWith("/") ? "" : FSPathConstants.PathDelimiter) + actualDirectoryEntry.Name;
                                XmlElement XmlElement = CreateResponseElement_Dir(header, XmlDocument, HRef, actualDirectoryEntry.Name, CurDirectoryObject, propfindProperties);
                                Root.AppendChild(XmlElement);
                            }
                            catch
                            {
                            }
                        }

                        else if (actualDirectoryEntry.Streams.Contains(FSConstants.FILESTREAM))
                        {
                            String HRef = header.FullHTTPDestinationPath() + (header.FullHTTPDestinationPath().EndsWith("/") ? "" : FSPathConstants.PathDelimiter) + actualDirectoryEntry.Name;
                            //INode INode = _IGraphFSSession.ExportINode(new ObjectLocation(ObjectDestination));
                            UInt64 Size = (UInt64)_IGraphFSSession.GetFSObject<FileObject>(new ObjectLocation(ObjectDestination), FSConstants.FILESTREAM, null, null, 0, false).Value.ObjectData.Length;
                            XmlElement ResponseElement_File = CreateResponseElement_File(header, XmlDocument, HRef, actualDirectoryEntry.Name, System.Net.Mime.MediaTypeNames.Text.Plain, Size, propfindProperties);
                            Root.AppendChild(ResponseElement_File);
                        }

                        else if (actualDirectoryEntry.Streams.Contains(FSConstants.SYMLINK))
                        {
                            String HRef = header.FullHTTPDestinationPath() + (header.FullHTTPDestinationPath().EndsWith("/") ? "" : FSPathConstants.PathDelimiter) + actualDirectoryEntry.Name;
                            Root.AppendChild(CreateResponseElement_Dir(header, XmlDocument, HRef, actualDirectoryEntry.Name, null, propfindProperties));
                        }

                        else if (actualDirectoryEntry.Streams.Contains(FSConstants.INLINEDATA))
                        {

                            String HRef = header.FullHTTPDestinationPath() + (header.FullHTTPDestinationPath().EndsWith("/") ? "" : FSPathConstants.PathDelimiter) + actualDirectoryEntry.Name;
                            Byte[] Inlinedata = DirectoryObject.GetInlineData(actualDirectoryEntry.Name);

                            // Return CreationTime and LastModificationTime of the DirectoryObject!
                            var _CreationTime = DateTime.Now;
                            var _LastModificationTime = DateTime.Now;

                            var _APandoraObject = DirectoryObject as AFSObject;

                            if (_APandoraObject != null)
                                if (_APandoraObject.INodeReference != null)
                                {
                                    _CreationTime = new DateTime((Int64)_APandoraObject.INodeReference.CreationTime);
                                    _LastModificationTime = new DateTime((Int64)_APandoraObject.INodeReference.LastModificationTime);
                                }

                            Root.AppendChild(
                                CreateResponseElement_File(
                                    header,
                                    XmlDocument,
                                    HRef,
                                    actualDirectoryEntry.Name,
                                    System.Net.Mime.MediaTypeNames.Text.Plain,
                                    (UInt64)Inlinedata.Length,
                                    _LastModificationTime,
                                    _CreationTime,
                                    propfindProperties
                                )
                            );

                        }

                        //else
                        //    Root.AppendChild(CreateResponseElement_Dir(XmlDocument, actualDirectoryEntry.StreamTypes + ":" + actualDirectoryEntry.myLogin, actualDirectoryEntry.StreamTypes + actualDirectoryEntry.myLogin, null, myPropfindProperties));
                    }

                }

                #endregion
            }

            XmlDocument.AppendChild(Root);

            #endregion

            #region Stream XmlDocument to ByteArray

            XmlWriterSettings settings;
            settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;

            using (MemoryStream stream = new MemoryStream())
            {

                using (XmlWriter writer = XmlWriter.Create(stream, settings))
                {

                    XmlDocument.WriteContentTo(writer);

                    writer.Flush();

                    return CleanContent(stream.ToArray());

                }

            }

            #endregion

        }

        #endregion

        #region WebDAV Method PUT

        /// <summary>
        /// Create a repsonse for a PUT request - uploading a file
        /// </summary>
        /// <param name="Properties"></param>
        /// <returns></returns>
        private Byte[] CreateErrorResponse(HTTPHeader header, String statusText)
        {

            #region Create XmlDocument

            XmlDocument XmlDocument = new XmlDocument();

            XmlElement Root = XmlDocument.CreateElement(S_DAV_PREFIX, "multistatus", S_DAV_NAMESPACE_URI);
            Root.SetAttribute("xmlns:Z", "urn:schemas-microsoft-com:");

            #region Create Response Element

            XmlElement ElemResponse = XmlDocument.CreateElement(S_DAV_PREFIX, "response", S_DAV_NAMESPACE_URI);
            
            XmlElement ElemHref = XmlDocument.CreateElement(S_DAV_PREFIX, "href", S_DAV_NAMESPACE_URI);
            ElemHref.InnerText = header.FullHTTPDestinationPath();
            ElemResponse.AppendChild(ElemHref);

            XmlElement ElemStatus = XmlDocument.CreateElement(S_DAV_PREFIX, "status", S_DAV_NAMESPACE_URI);
            ElemStatus.InnerText = statusText;
            ElemResponse.AppendChild(ElemStatus);

            #endregion

            Root.AppendChild(ElemResponse);

            XmlDocument.AppendChild(Root);

            #endregion

            #region Stream XmlDocument to ByteArray

            XmlWriterSettings settings;
            settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;

            using (MemoryStream stream = new MemoryStream())
            {

                using (XmlWriter writer = XmlWriter.Create(stream, settings))
                {

                    XmlDocument.WriteContentTo(writer);

                    writer.Flush();

                    return CleanContent(stream.ToArray());

                }

            }

            #endregion


        }

        #endregion

        #region WebDAV PROPFIND properties

        /// <summary>
        /// Add XML Part for a Lockdiscovery PROPFIND request (into /D:propstat/D:prop)
        /// </summary>
        /// <param name="parentElement">The Parent element (usually prop) where the lockdiscovery element is attached to</param>
        /// <param name="target">The Target Destination</param>
        /// <param name="target">The Target LockRoot - is a full qualified http path</param>
        private void AddPropfindLockdiscoveryElements(XmlNode parentElement, String target, String lockRootTarget)
        {

            #region Create XmlDocument

            XmlDocument XmlDocument = parentElement.OwnerDocument;

            XmlElement Root = XmlDocument.CreateElement(S_DAV_PREFIX, "lockdiscovery", S_DAV_NAMESPACE_URI);

            if (RessourceLock.RessourceIsLocked(target))
            {
                
                XmlElement ActiveLock = XmlDocument.CreateElement(S_DAV_PREFIX, "activelock", S_DAV_NAMESPACE_URI);

                    XmlElement LockType = XmlDocument.CreateElement(S_DAV_PREFIX, "locktype", S_DAV_NAMESPACE_URI);
                    LockType.AppendChild(XmlDocument.CreateElement(S_DAV_PREFIX, "write", S_DAV_NAMESPACE_URI));

                    XmlElement LockScope = XmlDocument.CreateElement(S_DAV_PREFIX, "lockscope", S_DAV_NAMESPACE_URI);
                    LockScope.AppendChild(XmlDocument.CreateElement(S_DAV_PREFIX, "exclusive", S_DAV_NAMESPACE_URI));

                    XmlElement LockDepth = XmlDocument.CreateElement(S_DAV_PREFIX, "depth", S_DAV_NAMESPACE_URI);
                    LockDepth.InnerText = "0";

                    XmlElement LockOwner = XmlDocument.CreateElement(S_DAV_PREFIX, "owner", S_DAV_NAMESPACE_URI);
                    XmlElement LockHref = XmlDocument.CreateElement(S_DAV_PREFIX, "href", S_DAV_NAMESPACE_URI);
                    LockHref.InnerText = "no";
                    LockOwner.AppendChild(LockHref);

                    XmlElement LockTimeout = XmlDocument.CreateElement(S_DAV_PREFIX, "timeout", S_DAV_NAMESPACE_URI);
                    LockTimeout.InnerText = "Second-" + ((Int64)RessourceLock.GetLockedRessource(target).ExpiresIn.TotalSeconds).ToString();

                    XmlElement LockToken = XmlDocument.CreateElement(S_DAV_PREFIX, "locktoken", S_DAV_NAMESPACE_URI);
                    XmlElement LockTokenHref = XmlDocument.CreateElement(S_DAV_PREFIX, "href", S_DAV_NAMESPACE_URI);
                    LockTokenHref.InnerText = "opaquelocktoken:" + RessourceLock.GetLockToken(target);// +TimestampNonce.AsString(S_DATETIME_FORMAT); //20090323T091747Z";
                    LockToken.AppendChild(LockTokenHref);

                    XmlElement LockRoot = XmlDocument.CreateElement(S_DAV_PREFIX, "lockroot", S_DAV_NAMESPACE_URI);
                    XmlElement LockRootHref = XmlDocument.CreateElement(S_DAV_PREFIX, "href", S_DAV_NAMESPACE_URI);
                    //LockRootHref.InnerText = FullHTTPDestinationPath;
                    LockRootHref.InnerText = lockRootTarget;
                    LockRoot.AppendChild(LockRootHref);

                ActiveLock.AppendChild(LockType);
                ActiveLock.AppendChild(LockScope);
                ActiveLock.AppendChild(LockDepth);
                ActiveLock.AppendChild(LockOwner);
                ActiveLock.AppendChild(LockTimeout);
                ActiveLock.AppendChild(LockToken);
                ActiveLock.AppendChild(LockRoot);

                Root.AppendChild(ActiveLock);

            }

            parentElement.AppendChild(Root);

            #endregion

        }


        /// <summary>
        /// Add XML Part for a SupportedLock PROPFIND request (into /D:propstat/D:prop)
        /// </summary>
        /// <param name="Properties"></param>
        /// <returns></returns>
        private void AddPropfindSupportedLockElements(XmlNode parentElement)
        {

            #region Create XmlDocument

            XmlDocument XmlDocument = parentElement.OwnerDocument;

            XmlElement Root = XmlDocument.CreateElement(S_DAV_PREFIX, "supportedlock", S_DAV_NAMESPACE_URI);
            XmlElement LockEntry1 = XmlDocument.CreateElement(S_DAV_PREFIX, "lockentry", S_DAV_NAMESPACE_URI);
            XmlElement LockType = XmlDocument.CreateElement(S_DAV_PREFIX, "locktype", S_DAV_NAMESPACE_URI);
            LockType.AppendChild(XmlDocument.CreateElement(S_DAV_PREFIX, "write", S_DAV_NAMESPACE_URI));
            XmlElement LockScope = XmlDocument.CreateElement(S_DAV_PREFIX, "lockscope", S_DAV_NAMESPACE_URI);
            LockScope.AppendChild(XmlDocument.CreateElement(S_DAV_PREFIX, "exclusive", S_DAV_NAMESPACE_URI));

            LockEntry1.AppendChild(LockType);
            LockEntry1.AppendChild(LockScope);

            Root.AppendChild(LockEntry1);

            parentElement.AppendChild(Root);

            #endregion

        }

        #endregion

        #region WebDAV Method LOCK

        /// <summary>
        /// Create a repsonse for a LOCK request - introducing a file copy (get)
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        private Byte[] CreateLockResponse(HTTPHeader header, Byte[] body, WebDAVDepth depth, params string[] properties)
        {

            #region Get Owner

            XmlDocument OwnerXmlDocument = new XmlDocument();
            OwnerXmlDocument.LoadXml(Encoding.UTF8.GetString(body));

            String NamespacePrefix = OwnerXmlDocument.GetPrefixOfNamespace(S_DAV_NAMESPACE_URI);
            if (NamespacePrefix == "") NamespacePrefix = S_DAV_PREFIX;

            XmlNamespaceManager XmlNamespaceManager = new XmlNamespaceManager(OwnerXmlDocument.NameTable);
            XmlNamespaceManager.AddNamespace(NamespacePrefix, S_DAV_NAMESPACE_URI);

            XmlElement OwnerRoot = OwnerXmlDocument.DocumentElement;
            XmlNode OwnerNode = OwnerRoot.SelectSingleNode(String.Concat("/", S_DAV_PREFIX, ":lockinfo/", S_DAV_PREFIX, ":owner/", S_DAV_PREFIX, ":href"), XmlNamespaceManager);

            String Owner = OwnerNode.InnerText;

            #endregion


            String LockTokenString = String.Concat("{" + Guid.NewGuid().ToString() + "}");
            TimeSpan LockLifetime;
            try
            {
                LockLifetime = TimeSpan.FromSeconds(Double.Parse(header.Headers["Timeout"].Substring(header.Headers["Timeout"].IndexOf('-') + 1)));
            }
            catch
            {
                LockLifetime = TimeSpan.FromSeconds(60 * 60);
            }
            RessourceLock.LockRessource(LockTokenString, header.Destination, LockLifetime);

            #region Create XmlDocument

            XmlDocument XmlDocument = new XmlDocument();

            XmlElement Root = XmlDocument.CreateElement(S_DAV_PREFIX, "prop", S_DAV_NAMESPACE_URI);
            AddPropfindLockdiscoveryElements(Root, header.Destination, header.FullHTTPDestinationPath());

           XmlDocument.AppendChild(Root);

            #endregion

            #region Stream XmlDocument to ByteArray

            XmlWriterSettings settings;
            settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;

            using (MemoryStream stream = new MemoryStream())
            {

                using (XmlWriter writer = XmlWriter.Create(stream, settings))
                {

                    XmlDocument.WriteContentTo(writer);

                    writer.Flush();

                    return CleanContent(stream.ToArray());

                }

            }

            #endregion

        }

        #endregion

        #region WebDAV Method PUT

        /// <summary>
        /// Create a repsonse for a PUT request - uploading a file
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        private Byte[] CreatePutResponse(HTTPHeader header, Byte[] body, params string[] properties)
        {

            #region Get Owner

            XmlDocument OwnerXmlDocument = new XmlDocument();
            OwnerXmlDocument.LoadXml(Encoding.UTF8.GetString(body));

            String NamespacePrefix = OwnerXmlDocument.GetPrefixOfNamespace(S_DAV_NAMESPACE_URI);
            if (NamespacePrefix == "") NamespacePrefix = S_DAV_PREFIX;

            XmlNamespaceManager XmlNamespaceManager = new XmlNamespaceManager(OwnerXmlDocument.NameTable);
            XmlNamespaceManager.AddNamespace(NamespacePrefix, S_DAV_NAMESPACE_URI);

            XmlElement OwnerRoot = OwnerXmlDocument.DocumentElement;
            XmlNode OwnerNode = OwnerRoot.SelectSingleNode(String.Concat("/", S_DAV_PREFIX, ":lockinfo/", S_DAV_PREFIX, ":owner/", S_DAV_PREFIX, ":href"), XmlNamespaceManager);

            String Owner = OwnerNode.InnerText;

            #endregion

            #region Create XmlDocument

            XmlDocument XmlDocument = new XmlDocument();

            XmlElement Root = XmlDocument.CreateElement(S_DAV_PREFIX, "prop", S_DAV_NAMESPACE_URI);
            XmlElement LockDiscovery = XmlDocument.CreateElement(S_DAV_PREFIX, "lockdiscovery", S_DAV_NAMESPACE_URI);
            XmlElement ActiveLock = XmlDocument.CreateElement(S_DAV_PREFIX, "activelock", S_DAV_NAMESPACE_URI);
            XmlElement LockType = XmlDocument.CreateElement(S_DAV_PREFIX, "locktype", S_DAV_NAMESPACE_URI);
            LockType.AppendChild(XmlDocument.CreateElement(S_DAV_PREFIX, "write", S_DAV_NAMESPACE_URI));
            XmlElement LockScope = XmlDocument.CreateElement(S_DAV_PREFIX, "lockscope", S_DAV_NAMESPACE_URI);
            LockType.AppendChild(XmlDocument.CreateElement(S_DAV_PREFIX, "exclusive", S_DAV_NAMESPACE_URI));

            Dictionary<String, String> MoreProps = new Dictionary<String, String>();
            MoreProps.Add("depth", "0");
            MoreProps.Add("owner", Owner);
            MoreProps.Add("timeout", "Second-3600");
            XmlElement ElemMoreActivelockProps = CreatePropstatElement(header, XmlDocument, MoreProps, header.Destination);

            #region LockToken

            XmlElement LockToken = XmlDocument.CreateElement(S_DAV_PREFIX, "locktoken", S_DAV_NAMESPACE_URI);
            XmlElement LockTokenHRef = XmlDocument.CreateElement(S_DAV_PREFIX, "href", S_DAV_NAMESPACE_URI);
            //"opaquelocktoken:{A2E9F1BD-47DB-487E-AA85-8A10ACFA391D}20090323T091747Z";
            LockTokenHRef.InnerText = "opaquelocktoken:{" + Guid.NewGuid().ToString() + "}" + TimestampNonce.AsString(S_DATETIME_FORMAT); //20090323T091747Z";
            LockToken.AppendChild(LockTokenHRef);

            #endregion

            ActiveLock.AppendChild(LockType);
            ActiveLock.AppendChild(LockScope);
            ActiveLock.AppendChild(ElemMoreActivelockProps);
            ActiveLock.AppendChild(LockToken);

            LockDiscovery.AppendChild(ActiveLock);
            Root.AppendChild(LockDiscovery);

            XmlDocument.AppendChild(Root);

            #endregion

            #region Stream XmlDocument to ByteArray

            XmlWriterSettings settings;
            settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;

            using (MemoryStream stream = new MemoryStream())
            {

                using (XmlWriter writer = XmlWriter.Create(stream, settings))
                {

                    XmlDocument.WriteContentTo(writer);

                    writer.Flush();

                    return stream.ToArray();

                }

            }

            #endregion

        }

        #endregion

        #region WebDAV Method PROPPATCH

        /// <summary>
        /// Create a repsonse for a PROPPATCH request - after uploading a file
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        private Byte[] CreateProppatchResponse(HTTPHeader header, params string[] properties)
        {

            #region Create XmlDocument

            XmlDocument XmlDocument = new XmlDocument();

            XmlElement Root = XmlDocument.CreateElement(S_DAV_PREFIX, "multistatus", S_DAV_NAMESPACE_URI);
            Root.SetAttribute("xmlns:Z", "urn:schemas-microsoft-com:");

            #region Create Response Element

            XmlElement ElemResponse = XmlDocument.CreateElement(S_DAV_PREFIX, "response", S_DAV_NAMESPACE_URI);
            XmlElement ElemHref = XmlDocument.CreateElement(S_DAV_PREFIX, "href", S_DAV_NAMESPACE_URI);
            ElemHref.InnerText = header.FullHTTPDestinationPath();

            ElemResponse.AppendChild(ElemHref);

            foreach (String Property in properties)
            {
                XmlElement ElemPropstat = XmlDocument.CreateElement(S_DAV_PREFIX, "propstat", S_DAV_NAMESPACE_URI);
                
                XmlElement ElemStatus = XmlDocument.CreateElement(S_DAV_PREFIX, "status", S_DAV_NAMESPACE_URI);
                ElemStatus.InnerText = "HTTP/1.1 200 OK";

                XmlElement ElemProp = XmlDocument.CreateElement(S_DAV_PREFIX, "prop", S_DAV_NAMESPACE_URI);
                ElemProp.AppendChild(XmlDocument.CreateElement("Z", Property, "urn:schemas-microsoft-com:"));

                ElemPropstat.AppendChild(ElemProp);
                ElemPropstat.AppendChild(ElemStatus);

                ElemResponse.AppendChild(ElemPropstat);
            }


            #endregion

            Root.AppendChild(ElemResponse);

            XmlDocument.AppendChild(Root);

            #endregion

            #region Stream XmlDocument to ByteArray

            XmlWriterSettings settings;
            settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;

            using (MemoryStream stream = new MemoryStream())
            {

                using (XmlWriter writer = XmlWriter.Create(stream, settings))
                {

                    XmlDocument.WriteContentTo(writer);

                    writer.Flush();

                    return CleanContent(stream.ToArray());

                }

            }

            #endregion

        }

        #endregion

        #region XML Elements

        /// <summary>
        /// Create a response for Depth 0 request - just a root info
        /// </summary>
        /// <param name="xmlDocument"></param>
        /// <returns></returns>
        private XmlElement CreateDepth0Response(HTTPHeader header, XmlDocument xmlDocument, PropfindProperties propfindProperties, HashSet<String> destinationObjectStreamTypes)
        {
            var XMLElemResponse = xmlDocument.CreateElement(S_DAV_PREFIX, "response", S_DAV_NAMESPACE_URI);
            if(header.IsSVNClient)
                XMLElemResponse.SetAttribute("xmlns:"+S_SVN_PREFIX, S_SVN_NAMESPACE_URI);

            #region response elements

            var XMLElemHref = xmlDocument.CreateElement(S_DAV_PREFIX, "href", S_DAV_NAMESPACE_URI);

            if (header.IsSVNClient)
                XMLElemHref.InnerText = header.RawUrl;
            else
                XMLElemHref.InnerText = header.FullHTTPDestinationPath();

            var Props = new Dictionary<String, String>();

            // Should return ALL props
            var DisplayName = DirectoryHelper.GetObjectName(header.Destination);
            if (DisplayName == String.Empty) DisplayName = FSPathConstants.PathDelimiter;

            if (!header.IsSVNClient)
            {

                if (!destinationObjectStreamTypes.Contains(FSConstants.INLINEDATA))
                {

                    //var ObjectLocator = _IGraphFSSession.ExportObjectLocator(new ObjectLocation(Header.Destination));

                    if (destinationObjectStreamTypes.Contains(FSConstants.DIRECTORYSTREAM))
                    {
                        Props.Add(PropfindProperties.Getcontenttype.ToString(), System.Net.Mime.MediaTypeNames.Application.Octet);
                        Props.Add(PropfindProperties.Getcontentlength.ToString(), "0");
                        Props.Add(PropfindProperties.Resourcetype.ToString(), "collection");

                        // Do we want to add some MS specific data?
                        if (header.ClientType == ClientTypes.MicrosoftWebDAVMiniRedir)
                        {
                            //Props.Add("isFolder", "f");
                            //If the element contains the 'collection' child element plus additional unrecognized elements, it should generally be treated as a collection. If the element contains no recognized child elements, it should be treated as a non-collection resource
                            //Props.Add("isCollection", "1");
                            //Props.Add("ishidden", "0");
                        }
                    }
                    else if (destinationObjectStreamTypes.Contains(FSConstants.FILESTREAM))
                    {
                        Props.Add(PropfindProperties.Getcontentlength.ToString(), _IGraphFSSession.GetFSObject<FileObject>(new ObjectLocation(header.Destination), FSConstants.FILESTREAM, null, null, 0, false).Value.ObjectData.Length.ToString());
                    }

                    //if ((myPropfindProperties == PropfindProperties.NONE) || ((myPropfindProperties & PropfindProperties.Getlastmodified) == PropfindProperties.Getlastmodified))
                    //{
                        //if (ObjectLocator.INodeReference != null)
                        //    Props.Add(PropfindProperties.Getlastmodified.ToString(), GetConvertedDateTime(ObjectLocator.INodeReference.LastModificationTime).ToString(S_DATETIME_FORMAT));
                        //else
                        //    Props.Add(PropfindProperties.Getlastmodified.ToString(), GetConvertedDateTime(ObjectLocator.ModificationTime.Ticks).ToString(S_DATETIME_FORMAT));

                    //}
                    //if ((myPropfindProperties == PropfindProperties.NONE) || ((myPropfindProperties & PropfindProperties.Creationdate) == PropfindProperties.Creationdate))
                    //{
                    //    Props.Add(PropfindProperties.Creationdate.ToString(), GetConvertedDateTime(ObjectLocator.INodeReference.CreationTime).ToString(S_DATETIME_FORMAT));
                    //}
                }

                if ((propfindProperties == PropfindProperties.NONE) || ((propfindProperties & PropfindProperties.Displayname) == PropfindProperties.Displayname))
                    Props.Add(PropfindProperties.Displayname.ToString(), DisplayName);
                //Props.Add(PropfindProperties.Getcontentlanguage.ToString(), ""); // If no Content-Language is specified, the default is that the content is intended for all language audiences.
                //Props.Add(PropfindProperties.Getetag.ToString(), CacheUUID.NewGuid().ToString()); // to identify a single ressource for update purposes (see If-Match)

                if ((propfindProperties == PropfindProperties.NONE) || ((propfindProperties & PropfindProperties.Lockdiscovery) == PropfindProperties.Lockdiscovery))
                    Props.Add(PropfindProperties.Lockdiscovery.ToString(), "");
                if ((propfindProperties == PropfindProperties.NONE) || ((propfindProperties & PropfindProperties.Supportedlock) == PropfindProperties.Supportedlock))
                    Props.Add(PropfindProperties.Supportedlock.ToString(), "");

            }
            else // if (Header.IsSVNClient)
            {
                Boolean AllProps = ((propfindProperties & PropfindProperties.AllProp) == PropfindProperties.AllProp);

                if (destinationObjectStreamTypes.Contains(FSConstants.DIRECTORYSTREAM))
                {
                    if (AllProps || (propfindProperties & PropfindProperties.Resourcetype) == PropfindProperties.Resourcetype)
                        Props.Add(PropfindProperties.Resourcetype.ToString(), "collection");
                    if (AllProps) Props.Add(PropfindProperties.Getcontenttype.ToString(), System.Net.Mime.MediaTypeNames.Application.Octet);
                }

                if (AllProps)
                {
                    if (destinationObjectStreamTypes.Contains(FSConstants.INLINEDATA))
                    {
                        //ObjectLocator ObjectLocator = _IGraphFSSession.ExportObjectLocator(new ObjectLocation(Header.Destination));

                        //Props.Add(PropfindProperties.Creationdate.ToString(), GetConvertedDateTime(ObjectLocator.INodeReference.CreationTime).ToString(S_DATETIME_FORMAT));
                        //Props.Add(PropfindProperties.Getlastmodified.ToString(), GetConvertedDateTime(ObjectLocator.INodeReference.LastModificationTime).ToString(S_DATETIME_FORMAT));
                    }

                    Props.Add(PropfindProperties.CheckedIn.ToString(), "ver");                      // handled in special way
                    Props.Add(PropfindProperties.VersionControlledConfiguration.ToString(), ""); // handled in special way
                    Props.Add(PropfindProperties.Getetag.ToString(), Guid.NewGuid().ToString()); // to identify a single ressource for update purposes (see If-Match)
                    Props.Add(PropfindProperties.VersionName.ToString(), "37491");
                    Props.Add(PropfindProperties.CreatorDisplayname.ToString(), "Stefan");
                    Props.Add(PropfindProperties.BaselineRelativePath.ToString(), "");
                    Props.Add(PropfindProperties.RepositoryUuid.ToString(), "612f8ebc-c883-4be0-9ee0-a4e9ef946e3a");//CacheUUID.NewGuid().ToString());
                    Props.Add(PropfindProperties.DeadpropCount.ToString(), "1");
                    Props.Add(PropfindProperties.Lockdiscovery.ToString(), "");
                }

                if ((propfindProperties & PropfindProperties.CheckedIn) == PropfindProperties.CheckedIn)
                    Props.Add(PropfindProperties.CheckedIn.ToString(), "");                      // handled in special way
                if ((propfindProperties & PropfindProperties.VersionControlledConfiguration) == PropfindProperties.VersionControlledConfiguration)
                    Props.Add(PropfindProperties.VersionControlledConfiguration.ToString(), ""); // handled in special way
                if ((propfindProperties & PropfindProperties.BaselineRelativePath) == PropfindProperties.BaselineRelativePath)
                    Props.Add(PropfindProperties.BaselineRelativePath.ToString(), "");
                if ((propfindProperties & PropfindProperties.RepositoryUuid) == PropfindProperties.RepositoryUuid)
                    Props.Add(PropfindProperties.RepositoryUuid.ToString(), "612f8ebc-c883-4be0-9ee0-a4e9ef946e3a");//CacheUUID.NewGuid().ToString());
                if ((propfindProperties & PropfindProperties.BaselineCollection) == PropfindProperties.BaselineCollection)
                    Props.Add(PropfindProperties.BaselineCollection.ToString(), "");
                if ((propfindProperties & PropfindProperties.VersionName) == PropfindProperties.VersionName)
                    Props.Add(PropfindProperties.VersionName.ToString(), "");
            
            }

            XmlElement ElemPropstat = CreatePropstatElement(header, xmlDocument, Props, header.Destination);

            #endregion
            XMLElemResponse.AppendChild(XMLElemHref);
            XMLElemResponse.AppendChild(ElemPropstat);

            return XMLElemResponse;
        }
        
        /// <summary>
        /// Create a usual Response Element for an directory
        /// </summary>
        /// <param name="xmlDocument"></param>
        /// <param name="hRef"></param>
        /// <param name="displayname"></param>
        /// <returns></returns>
        private XmlElement CreateResponseElement_Dir(HTTPHeader header, XmlDocument xmlDocument, String hRef, String displayname, IDirectoryObject directoryObject, PropfindProperties propfindProperties)
        {

            String CreationDate = "";
            String LastModificationDate = "";

            //if (DirectoryObject != null && DirectoryObject.INodeReference != null)
            //{
            //    CreationDate = GetConvertedDateTime(DirectoryObject.INodeReference.CreationTime).ToString(S_DATETIME_FORMAT);
            //    LastModificationDate = GetConvertedDateTime(DirectoryObject.INodeReference.LastModificationTime).ToString(S_DATETIME_FORMAT);
            //}

            XmlElement ElemResponse = xmlDocument.CreateElement(S_DAV_PREFIX, "response", S_DAV_NAMESPACE_URI);
            #region response elements

            XmlElement ElemHref = xmlDocument.CreateElement(S_DAV_PREFIX, "href", S_DAV_NAMESPACE_URI);
            ElemHref.InnerText = hRef;

            Dictionary<String, String> Props = new Dictionary<String, String>();
            /*
            if (myPropfindProperties == PropfindProperties.NONE)
            {
                // Should return ALL props

                Props.Add(PropfindProperties.Creationdate.ToString(), CreationDate);//, "2008-12-29T15:28:29Z");
                Props.Add(PropfindProperties.Displayname.ToString(), Displayname);
                Props.Add(PropfindProperties.Getcontentlanguage.ToString(), ""); // If no Content-Language is specified, the default is that the content is intended for all language audiences.
                Props.Add(PropfindProperties.Getcontentlength.ToString(), "0");
                Props.Add(PropfindProperties.Getcontenttype.ToString(), System.Net.Mime.MediaTypeNames.Application.Octet);
                Props.Add(PropfindProperties.Getetag.ToString(), CacheUUID.NewGuid().ToString()); // to identify a single ressource for update purposes (see If-Match)
                Props.Add(PropfindProperties.Getlastmodified.ToString(), LastModificationDate);//"2009-02-09T08:11:12Z");
                Props.Add(PropfindProperties.Lockdiscovery.ToString(), "");
                Props.Add(PropfindProperties.Resourcetype.ToString(), "collection");
                Props.Add(PropfindProperties.Supportedlock.ToString(), "");

                // Do we want to add some MS specific data?
                if (Header.ClientType == ClientTypes.MicrosoftWebDAVMiniRedir)
                {
                    //Props.Add("isFolder", "f");
                    //If the element contains the 'collection' child element plus additional unrecognized elements, it should generally be treated as a collection. If the element contains no recognized child elements, it should be treated as a non-collection resource
                    //Props.Add("isCollection", "1");
                    //Props.Add("ishidden", "0");
                }

            }else{
                */
            if ((propfindProperties == PropfindProperties.NONE) || ((propfindProperties & PropfindProperties.Creationdate)       == PropfindProperties.Creationdate))
                Props.Add(PropfindProperties.Creationdate.ToString(), CreationDate);
            if ((propfindProperties == PropfindProperties.NONE) || ((propfindProperties & PropfindProperties.Displayname)        == PropfindProperties.Displayname))
                Props.Add(PropfindProperties.Displayname.ToString(), displayname);
            if ((propfindProperties == PropfindProperties.NONE) || ((propfindProperties & PropfindProperties.Getcontentlanguage) == PropfindProperties.Getcontentlanguage))
                Props.Add(PropfindProperties.Getcontentlanguage.ToString(), "");
            if ((propfindProperties == PropfindProperties.NONE) || ((propfindProperties & PropfindProperties.Getcontentlength)   == PropfindProperties.Getcontentlength))
                Props.Add(PropfindProperties.Getcontentlength.ToString(), "0");
            if ((propfindProperties == PropfindProperties.NONE) || ((propfindProperties & PropfindProperties.Getcontenttype)     == PropfindProperties.Getcontenttype))
                Props.Add(PropfindProperties.Getcontenttype.ToString(), System.Net.Mime.MediaTypeNames.Application.Octet);
            if ((propfindProperties == PropfindProperties.NONE) || ((propfindProperties & PropfindProperties.Getetag)            == PropfindProperties.Getetag))
                Props.Add(PropfindProperties.Getetag.ToString(), Guid.NewGuid().ToString());
            if ((propfindProperties == PropfindProperties.NONE) || ((propfindProperties & PropfindProperties.Getlastmodified)    == PropfindProperties.Getlastmodified))
                Props.Add(PropfindProperties.Getlastmodified.ToString(), LastModificationDate);
            if ((propfindProperties == PropfindProperties.NONE) || ((propfindProperties & PropfindProperties.Lockdiscovery)      == PropfindProperties.Lockdiscovery))
                Props.Add(PropfindProperties.Lockdiscovery.ToString(), "");
            if ((propfindProperties == PropfindProperties.NONE) || ((propfindProperties & PropfindProperties.Resourcetype)       == PropfindProperties.Resourcetype))
                Props.Add(PropfindProperties.Resourcetype.ToString(), "collection");
            if ((propfindProperties == PropfindProperties.NONE) || ((propfindProperties & PropfindProperties.Supportedlock)      == PropfindProperties.Supportedlock))
                Props.Add(PropfindProperties.Supportedlock.ToString(), "");
            //}

            XmlElement ElemPropstat = CreatePropstatElement(header, xmlDocument, Props, hRef.Replace(header.GetFullHTTPHost(), ""));

            #endregion
            ElemResponse.AppendChild(ElemHref);
            ElemResponse.AppendChild(ElemPropstat);

            return ElemResponse;
        }

        /// <summary>
        /// Create a usual Response Element for a File
        /// </summary>
        /// <param name="xmlDocument">The Parent XmlDocument</param>
        /// <param name="hRef">The Full HTTP reference to the File</param>
        /// <param name="displayname">The display Name (currently not use in WebDAV MS Explorer Client)</param>
        /// <param name="mediaTypeName">The MIMEType</param>
        /// <param name="FilestreamObject">A File Object</param>
        /// <returns></returns>
        private XmlElement CreateResponseElement_File(HTTPHeader header, XmlDocument xmlDocument, String hRef, String displayname, String mediaTypeName, UInt64 size, PropfindProperties propfindProperties)
        {

            //DateTime CreationDate = GetConvertedDateTime(INode.CreationTime);
            //DateTime LastModificationDate = GetConvertedDateTime(INode.LastModificationTime);

            DateTime CreationDate = DateTime.Now;
            DateTime LastModificationDate = DateTime.Now;

            return CreateResponseElement_File(header, xmlDocument, hRef, displayname, mediaTypeName, size, CreationDate, LastModificationDate, propfindProperties);

        }

        /// <summary>
        /// Create a usual Response Element for a File or InlineData
        /// </summary>
        /// <param name="xmlDocument"></param>
        /// <param name="hRef"></param>
        /// <param name="displayname"></param>
        /// <returns></returns>
        private XmlElement CreateResponseElement_File(HTTPHeader header, XmlDocument xmlDocument, String hRef, String displayname, String mediaTypeName, UInt64 contentLength, DateTime creationDate, DateTime lastModificationDate, PropfindProperties propfindProperties)
        {

            // need to convert the DateTime because WebDAV needs this as Zulu converted Timestamp
            //LastModificationDate = TimeZoneInfo.ConvertTime(LastModificationDate, TimeZoneInfo.Utc);
            //CreationDate = TimeZoneInfo.ConvertTime(CreationDate, TimeZoneInfo.Utc);

            XmlElement ElemResponse = xmlDocument.CreateElement(S_DAV_PREFIX, "response", S_DAV_NAMESPACE_URI);

            #region Add response elements

            XmlElement ElemHref = xmlDocument.CreateElement(S_DAV_PREFIX, "href", S_DAV_NAMESPACE_URI);
            ElemHref.InnerText = hRef;

            Dictionary<String, String> Props = new Dictionary<String, String>();

            //Props.Add("displayname", Displayname);
            /*
            if (myPropfindProperties == PropfindProperties.NONE)
            {
                Props.Add(PropfindProperties.Creationdate.ToString(), CreationDate.ToString(S_DATETIME_FORMAT));//, "2008-12-29T15:28:29Z");
                Props.Add(PropfindProperties.Displayname.ToString(), Displayname);
                //Props.Add(PropfindProperties.Getcontentlanguage.ToString(), ""); // If no Content-Language is specified, the default is that the content is intended for all language audiences.
                Props.Add(PropfindProperties.Getcontentlength.ToString(), ContentLength.ToString());
                //Props.Add(PropfindProperties.Getcontenttype.ToString(), MediaTypeName);
                //Props.Add(PropfindProperties.Getetag.ToString(), CacheUUID.NewGuid().ToString()); // to identify a single ressource for update purposes (see If-Match)
                Props.Add(PropfindProperties.Getlastmodified.ToString(), LastModificationDate.ToString(S_DATETIME_FORMAT));//"2009-02-09T08:11:12Z");
                Props.Add(PropfindProperties.Lockdiscovery.ToString(), "");
                //Props.Add(PropfindProperties.Resourcetype.ToString(), ""); // is empty for files!
                Props.Add(PropfindProperties.Supportedlock.ToString(), "");
            }
            else
            {
             * */
            if ((propfindProperties == PropfindProperties.NONE) || ((propfindProperties & PropfindProperties.Creationdate)       == PropfindProperties.Creationdate)) 
                Props.Add(PropfindProperties.Creationdate.ToString(), creationDate.ToString(S_DATETIME_FORMAT));
            if ((propfindProperties == PropfindProperties.NONE) || ((propfindProperties & PropfindProperties.Displayname)        == PropfindProperties.Displayname))
                Props.Add(PropfindProperties.Displayname.ToString(), displayname);
            if ((propfindProperties == PropfindProperties.NONE) || ((propfindProperties & PropfindProperties.Getcontentlanguage) == PropfindProperties.Getcontentlanguage))
                Props.Add(PropfindProperties.Getcontentlanguage.ToString(), "");
            if ((propfindProperties == PropfindProperties.NONE) || ((propfindProperties & PropfindProperties.Getcontentlength)   == PropfindProperties.Getcontentlength))
                Props.Add(PropfindProperties.Getcontentlength.ToString(), contentLength.ToString());
            if ((propfindProperties == PropfindProperties.NONE) || ((propfindProperties & PropfindProperties.Getcontenttype)     == PropfindProperties.Getcontenttype))
                Props.Add(PropfindProperties.Getcontenttype.ToString(), mediaTypeName);
            if ((propfindProperties == PropfindProperties.NONE) || ((propfindProperties & PropfindProperties.Getetag)            == PropfindProperties.Getetag))
                Props.Add(PropfindProperties.Getetag.ToString(), Guid.NewGuid().ToString());
            if ((propfindProperties == PropfindProperties.NONE) || ((propfindProperties & PropfindProperties.Getlastmodified)    == PropfindProperties.Getlastmodified))
                Props.Add(PropfindProperties.Getlastmodified.ToString(), lastModificationDate.ToString(S_DATETIME_FORMAT));
            if ((propfindProperties == PropfindProperties.NONE) || ((propfindProperties & PropfindProperties.Lockdiscovery)      == PropfindProperties.Lockdiscovery))
                Props.Add(PropfindProperties.Lockdiscovery.ToString(), "");
            //if ((myPropfindProperties == PropfindProperties.NONE) || ((myPropfindProperties & PropfindProperties.Resourcetype)       == PropfindProperties.Resourcetype))
            //    Props.Add(PropfindProperties.Resourcetype.ToString(), "");
            if ((propfindProperties == PropfindProperties.NONE) || ((propfindProperties & PropfindProperties.Supportedlock)      == PropfindProperties.Supportedlock))
                Props.Add(PropfindProperties.Supportedlock.ToString(), "");

            // Do we want to add some MS specific data?
            if (header.ClientType == ClientTypes.MicrosoftWebDAVMiniRedir)
            {
                //Props.Add("isFolder", "f");
                //If the element contains the 'collection' child element plus additional unrecognized elements, it should generally be treated as a collection. If the element contains no recognized child elements, it should be treated as a non-collection resource
                //Props.Add("isCollection", "1");
                //Props.Add("ishidden", "0");
            }
            //}

            XmlElement ElemPropstat = CreatePropstatElement(header, xmlDocument, Props, hRef);

            #endregion

            ElemResponse.AppendChild(ElemHref);
            ElemResponse.AppendChild(ElemPropstat);

            return ElemResponse;
        }
        
        /// <summary>
        /// Create a WebDAV propstat Element
        /// </summary>
        /// <param name="xmlDocument">The XMLDocument where this propstat element will be added to</param>
        /// <param name="props">A list of properties where the key is the property Name and value is the content (innerText) or Empty</param>
        /// <param name="status">The HTTP status, usually "HTTP/1.1 200 OK"</param>
        /// <returns>An XmlElement containing all Properties etc.</returns>
        private XmlElement CreatePropstatElement(HTTPHeader header, XmlDocument xmlDocument, Dictionary<String, String> props, String status, String target)
        {

            XmlElement ElemPropstat = xmlDocument.CreateElement(S_DAV_PREFIX, "propstat", S_DAV_NAMESPACE_URI);
            
            #region propstat elements

            XmlElement ElemStatus = xmlDocument.CreateElement(S_DAV_PREFIX, "status", S_DAV_NAMESPACE_URI);
            ElemStatus.InnerText = status;

            XmlElement ElemProp = xmlDocument.CreateElement(S_DAV_PREFIX, "prop", S_DAV_NAMESPACE_URI);

            #region prop elements

            foreach (KeyValuePair<String, String> PropItem in props)
            {

                if (PropItem.Key == PropfindProperties.Lockdiscovery.ToString())
                    AddPropfindLockdiscoveryElements(ElemProp, target.Replace(header.GetFullHTTPHost(), ""), target);
                else if (PropItem.Key == PropfindProperties.Lockdiscovery.ToString())
                    AddPropfindSupportedLockElements(ElemProp);
                else if (PropItem.Key == PropfindProperties.Resourcetype.ToString())
                {
                    XmlElement ElemResourceType = xmlDocument.CreateElement(S_DAV_PREFIX, PropItem.Key.ToLower(), S_DAV_NAMESPACE_URI);
                    //If the element contains the 'collection' child element plus additional unrecognized elements, it should generally be treated as a collection. If the element contains no recognized child elements, it should be treated as a non-collection resource
                    XmlElement ElemCollection = xmlDocument.CreateElement(S_DAV_PREFIX, PropItem.Value, S_DAV_NAMESPACE_URI);
                    ElemResourceType.AppendChild(ElemCollection);
                    
                    ElemProp.AppendChild(ElemResourceType);
                }

                #region SVN prop elements

                else if (PropItem.Key == PropfindProperties.VersionControlledConfiguration.ToString())
                {

                    XmlElement XmlVCC = xmlDocument.CreateElement(S_DAV_PREFIX, "version-controlled-configuration", S_DAV_NAMESPACE_URI);
                    XmlElement XmlVCCHref = xmlDocument.CreateElement(S_DAV_PREFIX, "href", S_DAV_NAMESPACE_URI);
                    XmlVCCHref.InnerText = DirectoryHelper.Combine(target.Replace(header.GetFullHTTPHost(), ""), "!svn/vcc/default");
                    XmlVCC.AppendChild(XmlVCCHref);

                    ElemProp.AppendChild(XmlVCC);

                }
                else if (PropItem.Key == PropfindProperties.CheckedIn.ToString())
                {

                    XmlElement XmlCI = xmlDocument.CreateElement(S_DAV_PREFIX, "checked-in", S_DAV_NAMESPACE_URI);
                    XmlElement XmlCIHref = xmlDocument.CreateElement(S_DAV_PREFIX, "href", S_DAV_NAMESPACE_URI);
                    
                    if(PropItem.Value == "ver")
                        XmlCIHref.InnerText = DirectoryHelper.Combine(target.Replace(header.GetFullHTTPHost(), ""), "!svn/ver/37491");
                    else
                        XmlCIHref.InnerText = DirectoryHelper.Combine(target.Replace(header.GetFullHTTPHost(), ""), "!svn/bln/37491");

                    XmlCI.AppendChild(XmlCIHref);

                    ElemProp.AppendChild(XmlCI);

                }
                else if (PropItem.Key == PropfindProperties.BaselineRelativePath.ToString())
                {

                    XmlElement Eleme = xmlDocument.CreateElement(S_SVN_PREFIX, "baseline-relative-path", S_SVN_NAMESPACE_URI);
                    ElemProp.AppendChild(Eleme);

                }
                else if (PropItem.Key == PropfindProperties.RepositoryUuid.ToString())
                {

                    XmlElement Eleme = xmlDocument.CreateElement(S_SVN_PREFIX, "repository-uuid", S_SVN_NAMESPACE_URI);
                    Eleme.InnerText = PropItem.Value;
                    ElemProp.AppendChild(Eleme);

                }
                else if (PropItem.Key == PropfindProperties.BaselineCollection.ToString())
                {

                    XmlElement XmlCI = xmlDocument.CreateElement(S_DAV_PREFIX, "baseline-collection", S_DAV_NAMESPACE_URI);
                    XmlElement XmlCIHref = xmlDocument.CreateElement(S_DAV_PREFIX, "href", S_DAV_NAMESPACE_URI);
                    if (header.SVNParameters != null && header.SVNParameters.Contains("bln"))
                        XmlCIHref.InnerText = DirectoryHelper.Combine(target.Replace(header.GetFullHTTPHost(), ""), "!svn/bc/37491");
                    else
                        XmlCIHref.InnerText = DirectoryHelper.Combine(target.Replace(header.GetFullHTTPHost(), ""), "!svn/bln/37491");

                    XmlCI.AppendChild(XmlCIHref);

                    ElemProp.AppendChild(XmlCI);

                }
                else if (PropItem.Key == PropfindProperties.VersionName.ToString())
                {

                    XmlElement XmlCI = xmlDocument.CreateElement(S_DAV_PREFIX, "version-Name", S_DAV_NAMESPACE_URI);
                    if (header.SVNParameters != null && header.SVNParameters.Contains("bln"))
                    {
                        XmlCI.InnerText = "37491";
                    }
                    else
                    {
                        XmlElement XmlCIHref = xmlDocument.CreateElement(S_DAV_PREFIX, "href", S_DAV_NAMESPACE_URI);
                        XmlCIHref.InnerText = "37491";
                        XmlCI.AppendChild(XmlCIHref);
                    }
                    ElemProp.AppendChild(XmlCI);

                }
                #endregion

                else
                {

                    XmlElement XmlElement = xmlDocument.CreateElement(S_DAV_PREFIX, PropItem.Key.ToLower(), S_DAV_NAMESPACE_URI);
                    if (PropItem.Value != null && PropItem.Value != String.Empty)
                        XmlElement.InnerText = PropItem.Value;

                    ElemProp.AppendChild(XmlElement);
                }

            }
            
            #endregion

            #endregion

            ElemPropstat.AppendChild(ElemStatus);
            ElemPropstat.AppendChild(ElemProp);

            return ElemPropstat;

        }

        /// <summary>
        /// Create a WebDAV propstat Element
        /// </summary>
        /// <param name="xmlDocument">The XMLDocument where this propstat element will be added to</param>
        /// <param name="props">A list of properties where the key is the property Name and value is the content (innerText) or Empty</param>
        /// <returns>An XmlElement containing all Properties etc.</returns>
        private XmlElement CreatePropstatElement(HTTPHeader header, XmlDocument xmlDocument, Dictionary<String, String> props, String target)
        {

            return CreatePropstatElement(header, xmlDocument, props, "HTTP/1.1 200 OK", target);

        }

        #endregion

        #endregion

        #endregion

        #region helpers

        protected Byte[] CleanContent(Byte[] content)
        {
            //HACK: The XML Byte Output containing 3 bad bytes at the beginning - maybe UTF8 Bom?
            if (content[0] == 239)
            {
                Byte[] NewContent = new Byte[content.Length - 3];
                Array.Copy(content, 3, NewContent, 0, NewContent.Length);
                content = NewContent;
            }
            return content;
        }

        protected DateTime GetConvertedDateTime(Int64 timestamp)
        {
            return TimeZoneInfo.ConvertTime(new DateTime(timestamp), TimeZoneInfo.Utc);
        }

        protected DateTime GetConvertedDateTime(UInt64 timestamp)
        {
            return TimeZoneInfo.ConvertTime(new DateTime((Int64)timestamp), TimeZoneInfo.Utc);
        }

        #endregion

    }
}
