/* 
 * IJSON_IO
 * Achim 'ahzf' Friedland, 2009 - 2010
 */

#region Usings

using System;

#endregion

namespace sones.GraphIO.JSON
{

    /// <summary>
    /// A transformation interface for user-defined objects into an
    /// application/json representation and vice versa.
    /// </summary>

    public interface IJSON_IO
    {

        /// <summary>
        /// Serialize this object as JSON
        /// </summary>
        String ToJSON();

        /// <summary>
        /// Deserialize the content of this object from the given JSON
        /// </summary>
        void FromJSON(String myJSON);

    }

}
