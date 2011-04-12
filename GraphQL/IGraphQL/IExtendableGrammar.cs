#region Usings



#endregion

namespace sones.GraphDB.Interfaces
{

    /// <summary>
    /// Marks a grammar as extendable
    /// This allow to add new plugins to a grammar
    /// </summary>
    public interface IExtendableGrammar
    {

        ///// <summary>
        ///// Add aggregate plugins to the grammar
        ///// </summary>
        ///// <param name="aggregates">Enumerable of aggregates</param>
        //void SetAggregates(IEnumerable<ABaseAggregate> aggregates);

        ///// <summary>
        ///// Add function plugins to the grammar
        ///// </summary>
        ///// <param name="functions">Enumerable of functions</param>
        //void SetFunctions(IEnumerable<ABaseFunction> functions);

        ///// <summary>
        ///// Add operator plugins to the grammar
        ///// </summary>
        ///// <param name="operators">Enumerable of operators</param>
        //void SetOperators(IEnumerable<ABinaryOperator> operators);

        ///// <summary>
        ///// Add setting plugins to the grammar
        ///// </summary>
        ///// <param name="settings">Enumerable of settings</param>
        //void SetSettings(IEnumerable<ADBSettingsBase> settings);

        ///// <summary>
        ///// Add edge plugins to the grammar
        ///// </summary>
        ///// <param name="edges">Enumerable of edges</param>
        //void SetEdges(IEnumerable<IEdgeType> edges);

        ///// <summary>
        ///// Add indices plugins to the grammar
        ///// </summary>
        ///// <param name="indices">Enumerable of IVersionedIndexObject</param>
        //void SetIndices(IEnumerable<AAttributeIndex> indices);

        ///// <summary>
        ///// Add importer plugins to the grammar
        ///// <seealso cref=" AGraphDBImport"/>
        ///// </summary>
        ///// <param name="graphDBImporter">Enumerable of Importer</param>
        //void SetGraphDBImporter(IEnumerable<AGraphDBImport> graphDBImporter);

    }
}
