using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using sones.GraphDB.Request;

namespace sones.GraphDB.Manager
{
    /// <summary>
    /// A manager that handles requests in a pipeline
    /// </summary>
    public sealed class RequestManager
    {
        #region data

        /// <summary>
        /// This structure contains requests that had been validated successfully
        /// </summary>
        private readonly BlockingCollection<IPipelinableRequest> _executableRequests;

        /// <summary>
        /// The incoming requests... every incoming request is stored within this structure
        /// </summary>
        private readonly BlockingCollection<IPipelinableRequest> _incomingRequests;

        /// <summary>
        /// The meta manager that contains all relevant manager
        /// </summary>
        private readonly MetaManager _metaManager;

        /// <summary>
        /// The scheduler which decides whether some requests are executed in parallel
        /// </summary>
        private readonly IRequestScheduler _requestScheduler;

        /// <summary>
        /// The structure where the results are stored
        /// </summary>
        private readonly ConcurrentDictionary<Guid, IRequest> _results;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new Request Manager
        /// </summary>
        /// <param name="queueLengthForIncomingRequests">This number represents the count of parallel incoming requests that are supported</param>
        /// <param name="executionQueueLength">The number of requests for the execution queue</param>
        /// <param name="executionTaskCount">The number ob tasks that work in parallel on the execution queue</param>
        /// <param name="myMetaManager">The meta manager that contains all relevant manager</param>
        /// <param name="myRequestScheduler">The scheduler which decides whether some requests are executed in parallel</param>
        /// <param name="cts">The cancellation token source</param>
        public RequestManager(
            int queueLengthForIncomingRequests,
            int executionQueueLength,
            int executionTaskCount,
            MetaManager myMetaManager,
            IRequestScheduler myRequestScheduler,
            CancellationTokenSource cts)
        {
            #region init

            _incomingRequests = new BlockingCollection<IPipelinableRequest>(queueLengthForIncomingRequests);
            _executableRequests = new BlockingCollection<IPipelinableRequest>(executionQueueLength);
            _results = new ConcurrentDictionary<Guid, IRequest>();
            _metaManager = myMetaManager;
            _requestScheduler = myRequestScheduler;

            #endregion

            try
            {
                var f = new TaskFactory(CancellationToken.None, TaskCreationOptions.LongRunning,
                                        TaskContinuationOptions.None, TaskScheduler.Default);

                // + 1 because of the validate task
                var tasks = (Task[]) Array.CreateInstance(typeof (Task), executionTaskCount + 1);
                int taskId = 0;

                //start the validate stage
                tasks[taskId++] =
                    f.StartNew(() => ValidateRequest(_incomingRequests, _executableRequests, _results, cts));

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

        #region stage 1

        /// <summary>
        /// Stage 1 of Request processing
        /// Validation of the incoming requests. 
        /// 
        /// BEWARE!!! Cannot be executed more than once BEWARE!!!
        /// </summary>
        /// <param name="myIncomingRequests">The incoming requests</param>
        /// <param name="myExecuteAbleRequests">The result of this stage. Validated Requests</param>
        /// <param name="myResults">The result of the whole request. This structure is used if a request failes during validation</param>
        /// <param name="cts">Responsible for task cancellation</param>
        private void ValidateRequest(
            BlockingCollection<IPipelinableRequest> myIncomingRequests,
            BlockingCollection<IPipelinableRequest> myExecuteAbleRequests,
            ConcurrentDictionary<Guid, IRequest> myResults,
            CancellationTokenSource cts)
        {
            CancellationToken token = cts.Token;

            IPipelinableRequest pipelineRequest = null;

            try
            {
                foreach (IPipelinableRequest aPipelineRequest in myIncomingRequests.GetConsumingEnumerable())
                {
                    pipelineRequest = aPipelineRequest;

                    if (token.IsCancellationRequested)
                        break;

                    if (pipelineRequest.Validate(_metaManager))
                    {
                        #region valid

                        if (_requestScheduler.ExecuteRequestInParallel(pipelineRequest.Request))
                        {
                            #region execute in parallel

                            //so the request is valid and the request scheduler agrees to execute it in parallel

                            myExecuteAbleRequests.Add(pipelineRequest, token);

                            #endregion
                        }
                        else
                        {
                            #region dedicated single execution

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

                            #endregion
                        }

                        #endregion
                    }
                    else
                    {
                        #region invalid

                        //the request is invalid and so it is transfered to the results

                        myResults.TryAdd(pipelineRequest.ID, pipelineRequest.Request);

                        #endregion
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

        #region stage 2

        /// <summary>
        /// Stage 2 of Request processing
        /// Execution of the incoming requests. 
        /// </summary>
        /// <param name="myExecuteAbleRequests">The already validated requests</param>
        /// <param name="myRequestResults">The result of this stage. Executed Requests</param>
        /// <param name="cts">Responsible for task cancellation</param>
        private void ExecuteRequest(
            BlockingCollection<IPipelinableRequest> myExecuteAbleRequests,
            ConcurrentDictionary<Guid, IRequest> myRequestResults,
            CancellationTokenSource cts)
        {
            CancellationToken token = cts.Token;

            IPipelinableRequest pipelineRequest = null;

            try
            {
                foreach (IPipelinableRequest aPipelineRequest in myExecuteAbleRequests.GetConsumingEnumerable())
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