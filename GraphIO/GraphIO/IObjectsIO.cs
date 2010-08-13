/*
 * IObjectsIO
 * Achim 'ahzf' Friedland, 2010
 */

namespace sones.GraphIO
{

    /// <summary>
    /// An interface to import to and export graph objects
    /// from the graph database
    /// </summary>

    public interface IObjectsIO : IObjectsImport, IObjectsExport
    {
    }

}
