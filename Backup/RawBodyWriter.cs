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
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;

namespace FM.WCF.SMDBehavior
{

    /// <summary>
    /// The RawBodyWriter class allow to write raw content in the message's body.
    /// RawBodyWriter extends <seealso cref="BodyWriter"/> class.
    /// </summary>
    internal class RawBodyWriter : BodyWriter
    {

        #region Fields

        private byte[] _messageContent;

        #endregion

        #region Constructors

        /// <summary>
        /// Create an instance of RawBodyWriter.
        /// </summary>
        /// <param name="messageContent">The body message content.</param>
        internal RawBodyWriter(byte[] messageContent) : base(false)
        {
            _messageContent = messageContent;
        }

        /// <summary>
        /// Create an instance of RawBodyWriter.
        /// </summary>
        /// <param name="messageContent">The body message content.</param>
        internal RawBodyWriter(string messageContent) : this(Encoding.UTF8.GetBytes(messageContent)) { }

        #endregion

        /// <summary>
        /// This method is called during message serialization to actually write 
        /// the message body content.
        /// <remarks>The trick is to use the WriteBase64 write's method to emit raw content.</remarks>
        /// </summary>
        /// <param name="writer">The XmlDictionaryWriter to write to.</param>
        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            writer.WriteStartElement("Binary", string.Empty);
            writer.WriteBase64(_messageContent, 0, _messageContent.Length);
            writer.WriteEndElement();
        }
    }
}
