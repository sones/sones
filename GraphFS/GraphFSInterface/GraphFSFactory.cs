/*
 * GraphFSFactory
 * (c) Achim Friedland, 2008 - 2010
 */

#region Usings

using System;

using sones.Lib.Singleton;
using sones.Lib.Reflection;
using sones.Lib.ErrorHandling;
using System.Collections.Generic;
using sones.GraphFS.Errors;
using sones.Lib.Settings;

#endregion

namespace sones.GraphFS
{

    /// <summary>
    /// A factory which uses reflection to generate a apropriate GraphFS for you.
    /// As this implements the singleton pattern, use GraphFSFactory.Instance.ActivateIGraphFS(...)
    /// </summary>
    public class GraphFSFactory : Singleton<GraphFSFactory.GraphFSFactory_internal>
    {

        /// <summary>
        /// An internal helper class for the GraphFSFactory class
        /// </summary>
        public class GraphFSFactory_internal : AutoDiscovery<IGraphFS>
        {

            #region Data

            const String _DefaultImplementation = "DiscGraphFS3";

            #endregion

            #region Constructor

            #region GraphFSFactory_internal()

            /// <summary>
            /// This constructor will autodiscover all implementations of IGraphFS
            /// </summary>
            public GraphFSFactory_internal()
            {
                FindAndRegisterImplementations(false, new String[] { "." }, t => t.Name);
            }

            #endregion

            #region GraphFSFactory_internal(myStrings)

            /// <summary>
            /// This constructor will autodiscover all implementations of IGraphFS
            /// </summary>
            public GraphFSFactory_internal(String[] myStrings)
            {
                FindAndRegisterImplementations(false, myStrings, t => t.Name);
            }

            #endregion

            #endregion


            #region SetIGraphFSParametersDictionary()

            /// <summary>
            /// Set IGraphFS properties based on IGraphFSParametersDictionary
            /// </summary>
            public Exceptional SetIGraphFSParametersDictionary(IGraphFS myIGraphFS, IDictionary<String, Object> IGraphFSParameters)
            {

                foreach (var _GraphFSParameter in IGraphFSParameters)
                {

                    var _IGraphFSProperty = myIGraphFS.GetType().GetProperty(_GraphFSParameter.Key);

                    try
                    {

                        if (_IGraphFSProperty != null)
                            _IGraphFSProperty.SetValue(myIGraphFS, _GraphFSParameter.Value, null);

                    }
                    catch (Exception)
                    {
                        return new Exceptional(new GraphFSError_InvalidIGraphFSParameterType(myIGraphFS, _GraphFSParameter.Key, _IGraphFSProperty.PropertyType, _GraphFSParameter.Value.GetType()));
                    }

                }

                return Exceptional.OK;

            }

            #endregion

            #region ActivateIGraphFS(myImplementation)

            public Exceptional<IGraphFS> ActivateIGraphFS(GraphAppSettings myGraphAppSettings, String myImplementation = _DefaultImplementation, IDictionary<String, Object> IGraphFSParameters = null)
            {

                if (myImplementation == null)
                    myImplementation = _DefaultImplementation;

                var _Exceptional = ActivateT_protected(myImplementation, myGraphAppSettings);
                if (_Exceptional.IsInvalid())
                    return _Exceptional;

                if (IGraphFSParameters != null)
                {
                    var _Exceptional2 = SetIGraphFSParametersDictionary(_Exceptional.Value, IGraphFSParameters);
                    if (_Exceptional2.Failed())
                        return _Exceptional2.Convert<IGraphFS>();
                }

                return _Exceptional;

            }

            #endregion

        }

    }

}
