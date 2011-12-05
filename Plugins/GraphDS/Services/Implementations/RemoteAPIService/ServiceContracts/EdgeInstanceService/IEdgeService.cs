using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using sones.Library.Commons.Security;
using sones.GraphDS.Services.RemoteAPIService.DataContracts;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.InstanceObjects;

namespace sones.GraphDS.Services.RemoteAPIService.ServiceContracts.EdgeInstanceService
{
    [ServiceContract(Namespace = sonesRPCServer.Namespace, Name = "EdgeInstanceService")]
    public interface IEdgeService 
    {
        #region Properties

        /// <summary>
        /// Returns the property of a graph element.
        /// </summary>
        /// <typeparam name="T">The type of the interesting property</typeparam>
        /// <param name="myPropertyID">The ID of the interesing property</param>
        /// <returns>A Property</returns>
        [OperationContract(Name = "GetPropertyByEdgeInstance")]
        object GetProperty(SecurityToken mySecurityToken, Int64 myTransToken, ServiceEdgeInstance myEdge, Int64 myPropertyID);


        /// <summary>
        /// Checks whether the graph element is in possession of a certain property
        /// </summary>
        /// <param name="myPropertyID">The ID of the property</param>
        /// <returns>True if the property exists, otherwise false</returns>
        [OperationContract(Name = "HasPropertyByEdgeInstance")]
        bool HasProperty(SecurityToken mySecurityToken, Int64 myTransToken, ServiceEdgeInstance myGraphElement, Int64 myPropertyID);

        /// <summary>
        /// Returns the count of the vertex properties
        /// </summary>
        /// <returns>An unsigned value</returns>
        [OperationContract(Name = "GetCountOfPropertiesByEdgeInstance")]
        int GetCountOfProperties(SecurityToken mySecurityToken, Int64 myTransToken, ServiceEdgeInstance myGraphElement);

        /// <summary>
        /// Returns all properties
        /// </summary>
        /// <param name="myFilter">A function to filter properties</param>
        /// <returns>An IEnumerable of Property/Value</returns>
        [OperationContract(Name = "GetAllPropertiesByEdgeInstance")]
        List<Tuple<Int64, object>> GetAllProperties(SecurityToken mySecurityToken, Int64 myTransToken, ServiceEdgeInstance myGraphElement);

        /// <summary>
        /// Returns a property as string
        /// </summary>
        /// <param name="myPropertyID">The id of the property</param>
        /// <returns>The string representation of the property</returns>
        [OperationContract(Name = "GetPropertyAsStringByEdgeInstance")]
        String GetPropertyAsString(SecurityToken mySecurityToken, Int64 myTransToken, ServiceEdgeInstance myGraphElement, Int64 myPropertyID);

        #endregion

        #region Unstructured data/properties

        /// <summary>
        /// Gets unstructured data of the graph element
        /// </summary>
        /// <typeparam name="T">The type of the interesting property</typeparam>
        /// <param name="myPropertyName">The name of the interesting unstructured property</param>
        /// <returns>The value of an unstructured property</returns>
        [OperationContract(Name = "GetUnstructuredPropertyByEdgeInstance")]
        object GetUnstructuredProperty(SecurityToken mySecurityToken, Int64 myTransToken, ServiceEdgeInstance myGraphElement, string myPropertyName);

        /// <summary>
        /// Checks whether the graph element is in possession of a certain unstructered property
        /// </summary>
        /// <param name="myPropertyName">The name of the unstructured property</param>
        /// <returns>True if the property exists, otherwise false</returns>
        [OperationContract(Name = "HasUnstructuredPropertyByEdgeInstance")]
        bool HasUnstructuredProperty(SecurityToken mySecurityToken, Int64 myTransToken, ServiceEdgeInstance myGraphElement, String myPropertyName);

        /// <summary>
        /// Returns the count of the unstructured vertex properties
        /// </summary>
        /// <returns>An unsigned value</returns>
        [OperationContract(Name = "GetCountOfUnstructuredPropertiesByEdgeInstance")]
        int GetCountOfUnstructuredProperties(SecurityToken mySecurityToken, Int64 myTransToken, ServiceEdgeInstance myGraphElement);

        /// <summary>
        /// Returns all unstructured properties
        /// </summary>
        /// <param name="myFilter">A function to filter properties</param>
        /// <returns>An IEnumerable of NameOfProperty/Value</returns>
        [OperationContract(Name = "GetAllUnstructuredPropertiesByEdgeInstance")]
        List<Tuple<String, Object>> GetAllUnstructuredProperties(SecurityToken mySecurityToken, Int64 myTransToken, ServiceEdgeInstance myGraphElement);

        /// <summary>
        /// Returns an unstructured property as string
        /// </summary>
        /// <param name="myPropertyName">The name of the unstructured property</param>
        /// <returns>The string representation of the property</returns>
        [OperationContract(Name = "GetUnstructuredPropertyAsStringByEdgeInstance")]
        String GetUnstructuredPropertyAsString(SecurityToken mySecurityToken, Int64 myTransToken, ServiceEdgeInstance myGraphElement, String myPropertyName);

        #endregion

        #region Comment

        /// <summary>
        /// Gets the comment of this graph element
        /// </summary>
        [OperationContract(Name="CommentByEdgeInstance")]
        String Comment(SecurityToken mySecurityToken, Int64 myTransToken, ServiceEdgeInstance myGraphElement);

        #endregion

        #region Creation date

        /// <summary>
        /// The date the graph element has been created
        /// </summary>
        [OperationContract(Name="CreationDateByEdgeInstance")]
        long CreationDate(SecurityToken mySecurityToken, Int64 myTransToken, ServiceEdgeInstance myGraphElement);

        #endregion

        #region Modification date

        /// <summary>
        /// The date the graph element has been modified the last time
        /// </summary>
        [OperationContract(Name="ModificationDateByEdgeInstance")]
        long ModificationDate(SecurityToken mySecurityToken, Int64 myTransToken, ServiceEdgeInstance myGraphElement);

        #endregion
    }
}
