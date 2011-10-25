/*

SMDBehavior project.
Copyright (C) 2009  Fabrice Michellonet (Fabrice dot Michellonet at gmail dot com)

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA

*/
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace FM.WCF.SMDBehavior
{
    /// <summary>
    /// SMDBehavior is responsible of adding <seealso cref="SMDMessageInspector"/> at runtime
    /// on each service decorated with corresponding xml tag.
    /// SMDBehavior implements <seealso cref="IServiceBehavior"/>
    /// </summary>
    public class SMDBehavior : IServiceBehavior
    {

        #region Properties

        /// <summary>
        /// Get or set a boolean indicating wheter the json response should be indented.
        /// </summary>
        internal bool IndentResponse { get; set; }

        /// <summary>
        /// Get or set the string representing the url template that should match
        /// during json schema (smd) request.
        /// </summary>
        internal string UrlTemplate { get; set; }

        #endregion

        #region IServiceBehavior Members

        /// <summary>
        /// AddBindingParameters method isn't implemented in SMDBehavior.
        /// </summary>
        /// <param name="serviceDescription"></param>
        /// <param name="serviceHostBase"></param>
        /// <param name="endpoints"></param>
        /// <param name="bindingParameters"></param>
        public void AddBindingParameters(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, System.ServiceModel.Channels.BindingParameterCollection bindingParameters){}

        /// <summary>
        /// This method is called at runtime to dynamically add a <seealso cref="SMDMessageInspector"/> on each
        /// <seealso cref="EndpointDispatcher"/> whose contract is decorated with the SMDBehavior xml tag.
        /// </summary>
        /// <param name="serviceDescription">The current service description.</param>
        /// <param name="serviceHostBase">The current service host.</param>
        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcher cDispatcher in serviceHostBase.ChannelDispatchers)
            {
                foreach (EndpointDispatcher endPointDispatcher in cDispatcher.Endpoints)
                {
                    foreach (ServiceEndpoint serviceEndpoint in serviceDescription.Endpoints)
                    {
                        if (serviceEndpoint.Contract.Name == endPointDispatcher.ContractName)
                        {
                            // Adding SMDMessageInstector.
                            endPointDispatcher.DispatchRuntime.MessageInspectors.Add(
                                new SMDMessageInspector(serviceEndpoint.ListenUri, serviceEndpoint.Contract, this.IndentResponse) 
                                { 
                                    UrlTemplate = this.UrlTemplate
                                }
                            );
                            break;
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Validate method isn't implemented in SMDBehavior.
        /// </summary>
        /// <param name="serviceDescription"></param>
        /// <param name="serviceHostBase"></param>
        public void Validate(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase){}

        #endregion
    }
}
