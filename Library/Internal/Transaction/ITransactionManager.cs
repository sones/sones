using sones.GraphFS;
namespace sones.Library.Transaction
{
    /// <summary>
    /// The interface for all transaction managers
    /// 
    /// ITransactionable : Begin/Commit/... transaction
    /// IVertexHandler   : Handle vertex interaction
    /// </summary>
    public interface ITransactionManager : ITransactionable, IVertexStore
    {
        //hab keine ahnung was hier rein muss
    }
}