using System;
using sones.Library.VersionedPluginManager;

namespace sones.GraphQL
{
    #region IGraphQLVersionCompatibility

    /// <summary>
    /// A static implementation of the compatible IGraphQL plugin versions. 
    /// Defines the min and max version for all IGraphQL implementations which will be activated used this IGraphQL.
    /// </summary>
    internal static class IGraphQLVersionCompatibility
    {
        public static Version MinVersion
        {
            get
            {
                return new Version("2.0.0.0");
            }
        }
        public static Version MaxVersion
        {
            get
            {
                return new Version("2.0.0.0");
            }
        }
    }

    #endregion

    /// <summary>
    /// The interface for all graph query languages
    /// </summary>
    public interface IGraphQL : IQueryableLanguage, IPluginable
    {
        
    }
}