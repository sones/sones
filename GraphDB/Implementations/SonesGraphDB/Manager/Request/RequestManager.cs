using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using sones.GraphDB.Request;
using sones.Library.ErrorHandling;

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
        private readonly BlockingCollection<APipelinableRequest> _executableRequests;

        /// <summary>
        /// The incoming requests... every incoming request is stored within this structure
        /// </summary>
        private readonly BlockingCollection<APipelinableRequest> _incomingRequests;

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
        private readonly ConcurrentDictionary<Guid, APipelinableRequest> _results;

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

            _incomingRequests = new BlockingCollection<APipelinableRequest>(queueLengthForIncomingRequests);
            _executableRequests = new BlockingCollection<APipelinableRequest>(executionQueueLength);
            _results = new ConcurrentDictionary<Guid, APipelinableRequest>();
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
        private void ValidateRequests()
        {
            var token = _cts.Token;

            try
            {
                foreach (var aPipelineRequest in _incomingRequests.GetConsumingEnumerable())
                {
                    var pipelineRequest = aPipelineRequest;

                    if (token.IsCancellationRequested)
                        break;

                    ValidateRequest(ref pipelineRequest);

                    ProcessValidRequest(ref pipelineRequest, ref token);
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
        private void ExecuteRequests()
        {
            var token = _cts.Token;

            APipelinableRequest pipelineRequest;

            try
            {
                foreach (var aPipelineRequest in _executableRequests.GetConsumingEnumerable())
                {
                    pipelineRequest = aPipelineRequest;

                    if (token.IsCancellationRequested)
                        break;

                    ExecuteRequest(ref pipelineRequest);

                    _results.TryAdd(pipelineRequest.ID, pipelineRequest);
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
        /// Gets the myResult
        /// 
        /// If there was an error during validation or execution, the corresponding exception is thrown
        /// </summary>
        /// <param name="myInterestingResult">The id of the pipelineable request</param>
        /// <returns>The myResult of the request</returns>
        public APipelinableRequest GetResult(Guid myInterestingResult)
        {
            APipelinableRequest interestingRequest;

            while (true)
            {
                if (_results.TryRemove(myInterestingResult, out interestingRequest))
                {
                    break;
                }
            }

            if (interestingRequest.Exception != null)
            {
                //throw the exception and let the user handle it

                throw interestingRequest.Exception;
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
            _tasks = (Task[]) Array.CreateInstance(typeof (Task), executionTaskCount + 1);
            var taskId = 0;

            //start the validate stage
            _tasks[taskId++] =
                f.StartNew(ValidateRequests);

            //start the execution stage
            for (var i = 0; i < executionTaskCount; i++)
            {
                _tasks[taskId++] = f.StartNew(ExecuteRequests);
            }
        }

        #endregion

        #region Complete

        /// <summary>
        /// Completes a blocking collection
        /// </summary>
        /// <param name="myCollection">The collection that should be completed</param>
        private void Complete(BlockingCollection<APipelinableRequest> myCollection)
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
        public Guid RegisterRequest(APipelinableRequest myToBeAddedRequest)
        {
            _incomingRequests.Add(myToBeAddedRequest);

            return myToBeAddedRequest.ID;
        }

        #endregion

        #region Shutdown

        /// <summary>
        /// gracefully shutdown of the requestmanager
        /// </summary>
        /// <param name="myIsGracefulshutdown">If true, the RequestManager does not accept any more Requests and processes the remaining ones. Otherwise the remaining requests are canceled asap.</param>
        public void Shutdown(Boolean myIsGracefulshutdown = true)
        {
            if (myIsGracefulshutdown)
            {
                Complete(_incomingRequests);

                while (true)
                {
                    if (_incomingRequests.IsCompleted)
                    {
                        break;
                    }
                }
                Complete(_executableRequests);
                _cts.Cancel();
            }
            else
            {
                _cts.Cancel();
                Complete(_incomingRequests);
                Complete(_executableRequests);
            }

            Task.WaitAll(_tasks);

            _results.Clear();
        }

        #endregion

        #region ProcessValidRequest

        /// <summary>
        /// Processes a valid request
        /// </summary>
        /// <param name="myPipelineRequest">A valid pipelone request</param>
        /// <param name="myToken">The cancellation token</param>
        private void ProcessValidRequest(
            ref APipelinableRequest myPipelineRequest,
            ref CancellationToken myToken)
        {
            if (_requestScheduler.ExecuteRequestInParallel(myPipelineRequest.GetRequest()))
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

                //add the request to the myResult
                _results.TryAdd(myPipelineRequest.ID, myPipelineRequest);

                #endregion
            }
        }

        #endregion

        #region ValidateRequest

        /// <summary>
        /// Validates a single request and catches exceptions
        /// </summary>
        /// <param name="pipelineRequest">The request that is going to be validated</param>
        private void ValidateRequest(ref APipelinableRequest pipelineRequest)
        {
            try
            {
                pipelineRequest.Validate(_metaManager);
            }
            catch (Exception e)
            {
                HandleErroneousRequest(ref pipelineRequest, e);
            }
        }

        #endregion

        #region ExecuteRequest

        /// <summary>
        /// Executes a single request and catches exceptions
        /// </summary>
        /// <param name="pipelineRequest">The request that is going to be executed</param>
        private void ExecuteRequest(ref APipelinableRequest pipelineRequest)
        {
            try
            {
                pipelineRequest.Execute(_metaManager);
            }
            catch (Exception e)
            {
                HandleErroneousRequest(ref pipelineRequest, e);
            }
        }

        #endregion

        #region HandleErroneousRequest

        /// <summary>
        /// Handles exceptions that occured while processing a request
        /// </summary>
        /// <param name="pipelineRequest">The request that has been processed</param>
        /// <param name="e">The exception that has been thrown</param>
        private void HandleErroneousRequest(ref APipelinableRequest pipelineRequest, Exception e)
        {
            var aSonesException = e as ASonesException ?? new UnknownException(e);

            //add the exception to the request
            pipelineRequest.Exception = aSonesException;

            _results.TryAdd(pipelineRequest.ID, pipelineRequest);
        }

        #endregion

        #endregion
    }
}