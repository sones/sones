/*
 * GraphFSFactory
 * (c) Achim Friedland, 2008 - 2010
 */

#region Usings

using System;
using System.Linq;

using sones.Lib.Singleton;
using sones.Lib.Reflection;
using sones.Lib.ErrorHandling;
using System.Collections.Generic;
using sones.GraphFS.Errors;
using sones.Lib.Settings;
using sones.Lib.VersionedPluginManager;
using sones.Lib;

#endregion

namespace sones.GraphFS
{

    /// <summary>
    /// A factory which uses reflection to generate a apropriate GraphFS for you.
    /// As this implements the singleton pattern, use GraphFSFactory.Instance.ActivateIGraphFS(...)
    /// </summary>
    public class GraphFSFactory : Singleton<GraphFSFactory>
    {

        #region Data

        const String _DefaultImplementation = "DiscGraphFS3";

        #endregion

        #region Constructor

        #region GraphFSFactory()

        /// <summary>
        /// This constructor is public due to the Singleton pattern
        /// </summary>
        public GraphFSFactory()
        {

        }

        #endregion

        #endregion

        #region GetRegisteredPlugins

        /// <summary>
        /// Get all implementations of the IGraphFS. This will throw exceptions for any plugin version incompatibility
        /// </summary>
        /// <param name="myGraphAppSettings"></param>
        /// <returns></returns>
        public IEnumerable<IGraphFS> GetRegisteredPlugins(GraphAppSettings myGraphAppSettings = null)
        {

            var graphAppSettings = myGraphAppSettings ?? new GraphAppSettings();

            var pluginManager = new PluginManager()
                            .Register<IGraphFS>(IGraphFSVersionCompatibility.MinVersion, IGraphFSVersionCompatibility.MaxVersion, null, graphAppSettings)
                            .Discover(true, true);

            return pluginManager.Value.GetPlugins<IGraphFS>();

        }
        
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

        /// <summary>
        /// Activate a new instance of a IGraphFS <paramref name="myImplementation"/>
        /// </summary>
        /// <param name="myGraphAppSettings">The GraphAppSettings instance</param>
        /// <param name="myImplementation">The IGraphFS implementation</param>
        /// <param name="IGraphFSParameters">Some optional parameters</param>
        /// <returns></returns>
        public Exceptional<IGraphFS> ActivateIGraphFS(GraphAppSettings myGraphAppSettings, String myImplementation = _DefaultImplementation, IDictionary<String, Object> IGraphFSParameters = null)
        {

            #region Set data

            Exceptional<IGraphFS> _Exceptional = new Exceptional<IGraphFS>();

            if (myImplementation == null)
                myImplementation = _DefaultImplementation;

            #endregion

            #region Discover plugins

            var pm = new PluginManager()
                .Register<IGraphFS>(IGraphFSVersionCompatibility.MinVersion, IGraphFSVersionCompatibility.MaxVersion, null, myGraphAppSettings)
                .Discover(false, true);

            if (pm.Failed())
            {
                return _Exceptional.PushIExceptionalT(pm);
            }

            #endregion

            #region Get FS implementation 

            var fs = pm.Value.GetPlugins<IGraphFS>((ifs) => ifs.GetType().Name == myImplementation).FirstOrDefault();

            if (fs == null)
            {
                return _Exceptional.PushIErrorT(new GraphFSError("Could not find GraphFS implementation \"" + myImplementation + "\""));
            }

            #endregion

            #region SetIGraphFSParametersDictionary

            if (IGraphFSParameters != null)
            {
                _Exceptional.PushIExceptional(SetIGraphFSParametersDictionary(fs, IGraphFSParameters));
            }
            
            #endregion

            _Exceptional.Value = fs;

            return _Exceptional;

        }

        #endregion


    }

}
