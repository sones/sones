using sones.GraphDB.Request;

namespace sones.GraphDB.Manager
{
    /// <summary>
    /// A really simple request scheduler, should be really fast
    /// </summary>
    public sealed class SimpleRequestScheduler : IRequestScheduler
    {
        #region IRequestScheduler Members

        public bool ExecuteRequestInParallel(IRequest myRequest)
        {
            return myRequest.AccessMode != GraphDBAccessMode.TypeChange;
        }

        #endregion
    }
}
