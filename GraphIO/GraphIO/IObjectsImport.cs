/*
 * IObjectsImport
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
    /// An interface to import graph objects into the graph database
    /// </summary>

    public interface IObjectsImport : IGraphDBImport
    {
        INode         ImportINode         (Byte[] mySerializedINode);
        ObjectLocator ImportObjectLocator (Byte[] mySerializedObjectLocator);
    }

}
