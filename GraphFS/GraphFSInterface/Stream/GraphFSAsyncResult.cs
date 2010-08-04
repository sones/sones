using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Messaging;
using System.Threading;

namespace sones.GraphFS.Stream
{
    public class GraphFSAsyncResult : IAsyncResult //, IMessageSink
    {


        public GraphFSAsyncResult()
        {
        }


        // Summary:
        //     Gets the delegate object on which the asynchronous call was invoked.
        //
        // Returns:
        //     The delegate object on which the asynchronous call was invoked.
        public virtual object AsyncDelegate
        {
            get
            {
                return new Object();
            }
        }

        //
        // Summary:
        //     Gets the object provided as the last parameter of a BeginInvoke method call.
        //
        // Returns:
        //     The object provided as the last parameter of a BeginInvoke method call.
        public virtual object AsyncState
        {
            get
            {
                return new Object();
            }
        }

        //
        // Summary:
        //     Gets a System.Threading.WaitHandle that encapsulates Win32 synchronization
        //     handles, and allows the implementation of various synchronization schemes.
        //
        // Returns:
        //     A System.Threading.WaitHandle that encapsulates Win32 synchronization handles,
        //     and allows the implementation of various synchronization schemes.
        public virtual WaitHandle AsyncWaitHandle
        {
            get
            {
                return new GraphFSWaitHandle();
            }
        }

        //
        // Summary:
        //     Gets a value indicating whether the BeginInvoke call completed synchronously.
        //
        // Returns:
        //     true if the BeginInvoke call completed synchronously; otherwise, false.
        public virtual Boolean CompletedSynchronously
        {
            get
            {
                return true;
            }
        }

        //
        // Summary:
        //     Gets or sets a value indicating whether EndInvoke has been called on the
        //     current System.Runtime.Remoting.Messaging.AsyncResult.
        //
        // Returns:
        //     true if EndInvoke has been called on the current System.Runtime.Remoting.Messaging.AsyncResult;
        //     otherwise, false.
        public Boolean EndInvokeCalled
        {

            get
            {
                return true;
            }

            set
            {
            }

        }

        //
        // Summary:
        //     Gets a value indicating whether the server has completed the call.
        //
        // Returns:
        //     true after the server has completed the call; otherwise, false.
        public virtual Boolean IsCompleted
        {
            get
            {
                return true;
            }
        }

/*

        //
        // Summary:
        //     Gets the next message sink in the sink chain.
        //
        // Returns:
        //     An System.Runtime.Remoting.Messaging.IMessageSink interface that represents
        //     the next message sink in the sink chain.
        public IMessageSink NextSink { get; }

        // Summary:
        //     Implements the System.Runtime.Remoting.Messaging.IMessageSink interface.
        //
        // Parameters:
        //   msg:
        //     The request System.Runtime.Remoting.Messaging.IMessage interface.
        //
        //   replySink:
        //     The response System.Runtime.Remoting.Messaging.IMessageSink interface.
        //
        // Returns:
        //     No value is returned.
        public virtual IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink);

        //
        // Summary:
        //     Gets the response message for the asynchronous call.
        //
        // Returns:
        //     A remoting message that should represent a response to a method call on a
        //     remote object.
        public virtual IMessage GetReplyMessage();

        //
        // Summary:
        //     Sets an System.Runtime.Remoting.Messaging.IMessageCtrl for the current remote
        //     method call, which provides a way to control asynchronous messages after
        //     they have been dispatched.
        //
        // Parameters:
        //   mc:
        //     The System.Runtime.Remoting.Messaging.IMessageCtrl for the current remote
        //     method call.
        public virtual void SetMessageCtrl(IMessageCtrl mc);

        //
        // Summary:
        //     Synchronously processes a response message returned by a method call on a
        //     remote object.
        //
        // Parameters:
        //   msg:
        //     A response message to a method call on a remote object.
        //
        // Returns:
        //     Returns null.
        public virtual IMessage SyncProcessMessage(IMessage msg);
        */

    }
}
