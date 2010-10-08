/*
 * GraphFSError_InvalidIGraphFSParameterType
 * (c) Achim Friedland, 2010
 */

#region Usings

using System;

#endregion

namespace sones.GraphFS.Errors
{

    public class GraphFSError_InvalidIGraphFSParameterType : GraphFSError
    {

        #region Properties

        public IGraphFS IGraphFS        { get; private set; }
        public String   ParameterName   { get; private set; }
        public Type     ParameterType   { get; private set; }
        public Type     WrongType       { get; private set; }

        #endregion

        #region Constructor

        #region GraphFSError_InvalidIGraphFSParameterType(myIGraphFS, myParameterName, myParameterType, myWrongType)

        public GraphFSError_InvalidIGraphFSParameterType(IGraphFS myIGraphFS, String myParameterName, Type myParameterType, Type myWrongType)
        {
            IGraphFS      = myIGraphFS;
            ParameterName = myParameterName;
            ParameterType = myParameterType;
            Message       = String.Format("IGraphFS parameter/property '{0}.{1}' expected to be of type '{2}', not of type '{3}'!", myIGraphFS.GetType().Name, myParameterName, myParameterType.Name, myWrongType.Name);
        }

        #endregion

        #endregion

    }

}
