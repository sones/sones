/*
 * IGraphIO
 * Achim 'ahzf' Friedland, 2010
 */

namespace sones.GraphIO
{

    /// <summary>
    /// An interface to import to and export data from the
    /// graph database
    /// </summary>

    public interface IGraphIO : IGraphDBImport, IGraphDBExport
    {
    }

}
