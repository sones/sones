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
using System;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Text;
using Newtonsoft.Json;

namespace FM.WCF.SMDBehavior
{
    public class SMDMessageInspector : IDispatchMessageInspector
    {

        #region Fields

        private string _serviceDefinition = string.Empty;
        private Uri _listenUri;
        private string _urlTemplate;

        #endregion

        #region Properties

        /// <summary>
        /// Get or set a string representing the url template that should match
        /// during json schema (smd) request.
        /// </summary>
        internal string UrlTemplate {
            get { return _urlTemplate; }
            set { _urlTemplate = value; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Instantiate a MessageInspector that will specifically wait for SMD Requests.
        /// </summary>
        /// <param name="listenUri">The url on which this instector is bound to.</param>
        /// <param name="contract">The contract that should be serialized in json.</param>
        /// <param name="indentResponse">Indicate wether the json should be indented.</param>
        public SMDMessageInspector(Uri listenUri, ContractDescription contract, bool indentResponse)
        {

            _listenUri = listenUri;

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            using (JsonWriter jsonWriter = new JsonTextWriter(sw))
            {
                jsonWriter.Formatting = indentResponse ? Formatting.Indented : Formatting.None;

                jsonWriter.WriteStartObject();

                //jsonWriter.WritePropertyName("envelope");
                //jsonWriter.WriteValue("JSON");

                jsonWriter.WritePropertyName("target");
                jsonWriter.WriteValue(listenUri.AbsoluteUri + "/");

                jsonWriter.WritePropertyName("services");
                jsonWriter.WriteStartObject();

                foreach (OperationDescription operation in contract.Operations)
                {

                    WebGetAttribute getAttr = operation.Behaviors.Find<WebGetAttribute>();
                    WebInvokeAttribute invokeAttr = operation.Behaviors.Find<WebInvokeAttribute>();

                    if (getAttr == null && invokeAttr == null) continue;

                    serializeMethod(jsonWriter, operation.SyncMethod, getAttr, invokeAttr);

                }

                jsonWriter.WriteEndObject();
                jsonWriter.WriteEndObject();
            }

            _serviceDefinition = sb.ToString();
        }

        #endregion

        /// <summary>
        /// Serialize a .NET method definition in json.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="method"></param>
        /// <param name="getAttr"></param>
        /// <param name="invokAttr"></param>
        private void serializeMethod(JsonWriter writer, MethodInfo method, WebGetAttribute getAttr, WebInvokeAttribute invokAttr)
        {

            writer.WritePropertyName(method.Name);
            writer.WriteStartObject();

            writer.WritePropertyName("transport");
            writer.WriteValue(getAttr != null ? "GET" : invokAttr.Method);

            writer.WritePropertyName("target");
            writer.WriteValue(method.Name);

            writer.WritePropertyName("envelope");
            writer.WriteValue(getAttr != null ? "URL" : "JSON");

            writer.WritePropertyName("parameters");

            writer.WriteStartArray();

            ParameterInfo[] prams = method.GetParameters();
            foreach (ParameterInfo prm in method.GetParameters())
            {
                writer.WriteStartObject();
                
                writer.WritePropertyName("name");
                writer.WriteValue(prm.Name);

                writer.WritePropertyName("type");
                writer.WriteValue(prm.ParameterType.ToString());


                writer.WriteEndObject();
            }

            writer.WriteEndArray();
            writer.WriteEndObject();

        }


        #region IDispatchMessageInspector Members

        /// <summary>
        /// This method is called for each incoming message.
        /// If the message is an smd request then a we have to intercept it and send 
        /// the requested json.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="channel"></param>
        /// <param name="instanceContext"></param>
        /// <returns></returns>
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {

            UriTemplate ut = new UriTemplate(_urlTemplate, true);
            UriTemplateMatch results = ut.Match(new Uri(_listenUri.AbsoluteUri), request.Headers.To);
            
            bool isSmd = (results != null);

            return isSmd;
        }

        /// <summary>
        /// This method is responsible of creating the SMD response message which contains the 
        /// json representation of the passed contract.
        /// </summary>
        /// <param name="reply"></param>
        /// <param name="correlationState"></param>
        public void BeforeSendReply(ref Message reply, object correlationState)
        {

            bool isSmdRequest = (bool)correlationState;
            if (!isSmdRequest)
                return;

            Message msg = Message.CreateMessage(MessageVersion.None, "", new RawBodyWriter(_serviceDefinition));

            msg.Properties.Add(WebBodyFormatMessageProperty.Name, new WebBodyFormatMessageProperty(WebContentFormat.Raw));

            HttpResponseMessageProperty httpResponse = (HttpResponseMessageProperty)reply.Properties["httpResponse"];
            httpResponse.StatusCode = System.Net.HttpStatusCode.OK;
            httpResponse.Headers["Content-Type"] = "application/json ; charset=utf-8";
            // TODO -- Add a config param to output in text/plain content-type.
            //httpResponse.Headers["Content-Type"] = "text/plain;";
            msg.Properties.Add(HttpResponseMessageProperty.Name, httpResponse);

            reply = msg;
        }

        #endregion
    }
}