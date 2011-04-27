#region Usings
using System;
using System.Collections.Generic;
using sones.Plugins.SonesGQL.Aggregates;
using sones.Plugins.SonesGQL.Functions;
using sones.Plugins.Index.Interfaces;
#endregion

namespace sones.GraphDB.Interfaces
{

    /// <summary>
    /// Marks a grammar as extendable
    /// This allow to add new plugins to a grammar
    /// </summary>
    public interface IExtendableGrammar
    {

        /// <summary>
        /// Add aggregate plugins to the grammar
        /// </summary>
        /// <param name="aggregates">Enumerable of aggregates</param>
        void SetAggregates(IEnumerable<IGQLAggregate> aggregates);

        /// <summary>
        /// Add function plugins to the grammar
        /// </summary>
        /// <param name="functions">Enumerable of functions</param>
        void SetFunctions(IEnumerable<ABaseFunction> functions);

        /// <summary>
        /// Add indices plugins to the grammar
        /// </summary>
        /// <param name="indices">Enumerable of IVersionedIndexObject</param>
        void SetIndices(IEnumerable<IIndex<IComparable, Int64>> indices);

        ///// <summary>
        ///// Add importer plugins to the grammar
        ///// <seealso cref=" AGraphDBImport"/>
        ///// </summary>
        ///// <param name="graphDBImporter">Enumerable of Importer</param>
        //void SetGraphDBImporter(IEnumerable<AGraphDBImport> graphDBImporter);

    }
}
