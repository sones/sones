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

        /// <summary>
        /// The cancellation token source
        /// </summary>
        private CancellationTokenSource _cts;

        /// <summary>
        /// The pipeline tasks
        /// </summary>
        private Task[] _tasks;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new Request Manager
        /// </summary>
        /// <param name="queueLengthForIncomingRequests">This number represents the count of parallel incoming requests that are supported</param>
        /// <param name="executionQueueLength">The number of requests for the execution queue</param>
        /// <param name="myMetaManager">The meta manager that contains all relevant manager</param>
        /// <param name="myRequestScheduler">The scheduler which decides whether some requests are executed in parallel</param>
        public RequestManager(
            int queueLengthForIncomingRequests,
            int executionQueueLength,
            MetaManager myMetaManager,
            IRequestScheduler myRequestScheduler)
        {
            #region init

            _incomingRequests = new BlockingCollection<IPipelinableRequest>(queueLengthForIncomingRequests);
            _executableRequests = new BlockingCollection<IPipelinableRequest>(executionQueueLength);
            _results = new ConcurrentDictionary<Guid, IRequest>();
            _metaManager = myMetaManager;
            _requestScheduler = myRequestScheduler;

            #endregion
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
        private void ValidateRequest()
        {
            CancellationToken token = _cts.Token;

            IPipelinableRequest pipelineRequest = null;

            try
            {
                foreach (var aPipelineRequest in _incomingRequests.GetConsumingEnumerable())
                {
                    pipelineRequest = aPipelineRequest;

                    if (token.IsCancellationRequested)
                        break;

                    if (pipelineRequest.Validate(_metaManager))
                    {
                        #region valid

                        ProcessValidRequest(ref pipelineRequest, ref token);

                        #endregion
                    }
                    else
                    {
                        #region invalid

                        //the request is invalid and so it is transfered to the results

                        _results.TryAdd(pipelineRequest.ID, pipelineRequest.Request);

                        #endregion
                    }

                    pipelineRequest = null;
                }
            }
            catch (Exception e)
            {
                _cts.Cancel();
                if (!(e is OperationCanceledException))
                    throw;
            }
            finally
            {
                _executableRequests.CompleteAdding();
            }
        }



        #endregion

        #region stage 2

        /// <summary>
        /// Stage 2 of Request processing
        /// Execution of the incoming requests. 
        /// </summary>
        private void ExecuteRequest()
        {
            CancellationToken token = _cts.Token;

            IPipelinableRequest pipelineRequest = null;

            try
            {
                foreach (var aPipelineRequest in _executableRequests.GetConsumingEnumerable())
                {
                    pipelineRequest = aPipelineRequest;

                    if (token.IsCancellationRequested)
                        break;

                    pipelineRequest.Execute(_metaManager);

                    _results.TryAdd(pipelineRequest.ID, pipelineRequest.Request);

                    pipelineRequest = null;
                }
            }
            catch (Exception e)
            {
                _cts.Cancel();
                if (!(e is OperationCanceledException))
                    throw;
            }
        }

        #endregion

        #endregion

        #region Methods

        #region GetResult

        /// <summary>
        /// Gets the resulting request corresponding
        /// </summary>
        /// <param name="myInterestingResult">The id of the pipelineable request</param>
        /// <returns>The result of the request</returns>
        public IRequest GetResult(Guid myInterestingResult)
        {
            IRequest interestingRequest = null;

            while (true)
            {
                if (_results.TryGetValue(myInterestingResult, out interestingRequest))
                {
                    break;
                }
            }

            return interestingRequest;
        }

        #endregion

        #region Init

        /// <summary>
        /// Initializes the request manager
        /// </summary>
        /// <param name="executionTaskCount">The number ob tasks that work in parallel on the execution queue</param>
        /// <param name="cts">The cancellation token source</param>
        public void Init(int executionTaskCount, CancellationTokenSource cts)
        {
            _cts = cts;

            var f = new TaskFactory(CancellationToken.None, TaskCreationOptions.LongRunning,
                                    TaskContinuationOptions.None, TaskScheduler.Default);

            // + 1 because of the validate task
            _tasks = (Task[])Array.CreateInstance(typeof(Task), executionTaskCount + 1);
            int taskId = 0;

            //start the validate stage
            _tasks[taskId++] =
                f.StartNew(ValidateRequest);

            //start the execution stage
            for (int i = 0; i < executionTaskCount; i++)
            {
                _tasks[taskId++] = f.StartNew(ExecuteRequest);
            }
        }

        #endregion

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
        public void RegisterRequest(IPipelinableRequest myToBeAddedRequest)
        {
            _incomingRequests.Add(myToBeAddedRequest);
        }

        #endregion

        #region Shutdown

        /// <summary>
        /// gracefully shutdown of the requestmanager
        /// </summary>
        public void Shutdown()
        {
            _cts.Cancel();
            Complete(_incomingRequests);
            Complete(_executableRequests);

            Task.WaitAll(_tasks);
        }

        #endregion

        #region ProcessValidRequest

        /// <summary>
        /// Processes a valid request
        /// </summary>
        /// <param name="myPipelineRequest">A valid pipelone request</param>
        /// <param name="myToken">The cancellation token</param>
        private void ProcessValidRequest(
            ref IPipelinableRequest myPipelineRequest,
            ref CancellationToken myToken)
        {
            if (_requestScheduler.ExecuteRequestInParallel(myPipelineRequest.Request))
            {
                #region execute in parallel

                //so the request is valid and the request scheduler agrees to execute it in parallel

                _executableRequests.Add(myPipelineRequest, myToken);

                #endregion
            }
            else
            {
                #region dedicated single execution

                //wait until the remaining stages are empty
                var everyRequestIsExecuted = false;

                while (!everyRequestIsExecuted)
                {
                    everyRequestIsExecuted = _executableRequests.Count == 0;
                }

                //execute this request
                myPipelineRequest.Execute(_metaManager);

                //add the request to the result
                _results.TryAdd(myPipelineRequest.ID, myPipelineRequest.Request);

                #endregion
            }
        }

        #endregion

        #endregion
    }
}