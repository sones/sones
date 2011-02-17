using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using sones.GraphDB.Request;
using sones.Security;
using sones.Transaction;

namespace sones.GraphDB.Manager
{
    /// <summary>
    /// A manager that handles requests in a pipeline
    /// </summary>
    public sealed class RequestManager
    {
        #region data

        /// <summary>
        /// The incoming requests... every incoming request is stored within this structure
        /// </summary>
        private BlockingCollection<IPipelinableRequest> _incomingRequests;

        /// <summary>
        /// This structure contains requests that had been validated successfully
        /// </summary>
        private BlockingCollection<IPipelinableRequest> _executableRequests;

        /// <summary>
        /// The structure where the results are stored
        /// </summary>
        private ConcurrentDictionary<Guid, IRequest> _results;

        private readonly MetaManager _metaManager;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new Request Manager
        /// </summary>
        /// <param name="queueLengthForIncomingRequests">This number represents the count of parallel incoming requests that are supported</param>
        /// <param name="executionQueueLength">The number of requests for the execution queue</param>
        /// <param name="executionTaskCount">The number ob tasks that work in parallel on the execution queue</param>
        /// <param name="cts">The cancellation token source</param>
        public RequestManager(int queueLengthForIncomingRequests, int executionQueueLength, int executionTaskCount, CancellationTokenSource cts, MetaManager myMetaManager)
        {
            #region init

            _incomingRequests   = new BlockingCollection<IPipelinableRequest>(queueLengthForIncomingRequests);
            _executableRequests = new BlockingCollection<IPipelinableRequest>(executionQueueLength);
            _results            = new ConcurrentDictionary<Guid, IRequest>();
            _metaManager        = myMetaManager;

            #endregion

            try
            {
                var f = new TaskFactory(CancellationToken.None, TaskCreationOptions.LongRunning, TaskContinuationOptions.None, TaskScheduler.Default);

                // + 1 because of the validate task
                Task[] tasks = (Task[])Array.CreateInstance(typeof(Task), executionTaskCount + 1);
                int taskId = 0;

                //start the validate stage
                tasks[taskId++] = f.StartNew(() => ValidateRequest(_incomingRequests, _executableRequests, _results, cts));

                //start the execution stage
                for (int i = 0; i < executionTaskCount; i++)
                {
                    tasks[taskId++] = f.StartNew(() => ExecuteRequest(_executableRequests, _results, cts));
                }

                Task.WaitAll(tasks);
            }
            finally
            {
                Complete(_incomingRequests);
                Complete(_executableRequests);
            }
        }

        #endregion

        #region stages

        #region 1

        /// <summary>
        /// Stage 1 of Request processing
        /// Validation of the incoming requests. 
        /// </summary>
        /// <param name="myIncomingRequests">The incoming requests</param>
        /// <param name="myExecuteAbleRequests">The result of this stage. Validated Requests</param>
        /// <param name="myResults">The result of the whole request. This structure is used if a request failes during validation</param>
        /// <param name="cts">Responsible for task cancellation</param>
        void ValidateRequest(
            BlockingCollection<IPipelinableRequest> myIncomingRequests,
            BlockingCollection<IPipelinableRequest> myExecuteAbleRequests,
            ConcurrentDictionary<Guid, IRequest> myResults,
            CancellationTokenSource cts)
        {
            var token = cts.Token;
            IPipelinableRequest pipelineRequest = null;
            try
            {
                foreach (var aPipelineRequest in myIncomingRequests.GetConsumingEnumerable())
                {
                    pipelineRequest = aPipelineRequest;
                    
                    if (token.IsCancellationRequested)
                        break;

                    if (pipelineRequest.Validate(_metaManager))
                    {
                        if (ExecuteRequestInParallel(pipelineRequest.SecurityToken, pipelineRequest.TransactionToken, pipelineRequest.Request))
                        {
                            myExecuteAbleRequests.Add(pipelineRequest, token);
                        }
                        else
                        {
                            //wait until the remaining stages are empty
                            Boolean everyRequestIsExecuted = false;

                            while (!everyRequestIsExecuted)
                            {
                                everyRequestIsExecuted = _executableRequests.Count == 0;
                            }

                            //execute this request
                            pipelineRequest.Execute(_metaManager);

                            //add the request to the result
                            myResults.TryAdd(pipelineRequest.ID, pipelineRequest.Request);
                        }
                    }
                    else
                    {
                        myResults.TryAdd(pipelineRequest.ID, pipelineRequest.Request);
                    }

                    pipelineRequest = null;
                }
            }
            catch (Exception e)
            {
                cts.Cancel();
                if (!(e is OperationCanceledException))
                    throw;
            }
            finally
            {
                myExecuteAbleRequests.CompleteAdding();
            }
        }

        #endregion

        #region 2

        /// <summary>
        /// Stage 2 of Request processing
        /// Execution of the incoming requests. 
        /// </summary>
        /// <param name="myExecuteAbleRequests">The already validated requests</param>
        /// <param name="myRequestResults">The result of this stage. Executed Requests</param>
        /// <param name="cts">Responsible for task cancellation</param>
        void ExecuteRequest(
            BlockingCollection<IPipelinableRequest> myExecuteAbleRequests,
            ConcurrentDictionary<Guid, IRequest> myRequestResults,
            CancellationTokenSource cts)
        {
            var token = cts.Token;
            IPipelinableRequest pipelineRequest = null;
            try
            {
                foreach (var aPipelineRequest in myExecuteAbleRequests.GetConsumingEnumerable())
                {
                    pipelineRequest = aPipelineRequest;

                    if (token.IsCancellationRequested)
                        break;

                    pipelineRequest.Execute(_metaManager);

                    myRequestResults.TryAdd(pipelineRequest.ID, pipelineRequest.Request);

                    pipelineRequest = null;
                }
            }
            catch (Exception e)
            {
                cts.Cancel();
                if (!(e is OperationCanceledException))
                    throw;
            }
            finally
            {
                //nothing to do
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Complete

        /// <summary>
        /// Completes a blocking collection
        /// </summary>
        /// <param name="myCollection">The collection that should be completed</param>
        private void Complete(BlockingCollection<IPipelinableRequest> myCollection)
        {
            if (myCollection != null)
            {
                myCollection.CompleteAdding();
            }
        }

        #endregion

        private bool ExecuteRequestInParallel(SecurityToken securityToken, TransactionToken transactionToken, IRequest iRequest)
        {
            return iRequest.AccessMode != GraphDBAccessMode.TypeChange;
        }

        #region RegisterRequest

        /// <summary>
        /// Registeres a new request
        /// </summary>
        /// <param name="myToBeAddedRequest">The request that should be registered</param>
        /// <returns>The unique id of the request</returns>
        internal void RegisterRequest(IPipelinableRequest myToBeAddedRequest)
        {
            _incomingRequests.Add(myToBeAddedRequest);
        }

        #endregion

        #endregion
    }
}
