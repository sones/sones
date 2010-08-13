/*
 * IObjectsExport
 * Achim 'ahzf' Friedland, 2010
 */

#region Usings

using System;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Objects;

#endregion

namespace sones.GraphIO
{

    /// <summary>
    /// An interface to export graph objects from the graph database
    /// </summary>

    public interface IObjectsExport : IGraphDBExport
    {

        Byte[] ExportINode(INode myINode);
        Byte[] ExportObjectLocator(ObjectLocator myObjectLocator);
        Byte[] ExportAFSObject(AFSObject myAFSObject);
        
    }

}
