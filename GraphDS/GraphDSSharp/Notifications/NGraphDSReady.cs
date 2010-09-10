/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
* 
*/

/*
 * GraphDS - NGraphDSReady
 * (c) Achim 'ahzf' Friedland, 2010
 */

#region Usings

using System;
using System.Text;

using sones.Notifications;
using sones.Lib.NewFastSerializer;

#endregion


namespace sones.GraphDS.API.CSharp.Notifications
{

    /// <summary>
    /// Notifies about an ready to use GraphDS instance.
    /// </summary>

    public class NGraphDSReady : NGraphDS
    {

        public new class Arguments : INotificationArguments
        {
            
            #region Properties

            public String Message { get; set; }

            #endregion

            #region INotificationArguments Members

            public Byte[] Serialize()
            {

                var _SerializationWriter = new SerializationWriter();
                _SerializationWriter.WriteObject(Message);

                return _SerializationWriter.ToArray();

            }

            public void Deserialize(Byte[] mySerializedBytes)
            {

                var _SerializationReader = new SerializationReader(mySerializedBytes);
                Message = (String) _SerializationReader.ReadObject();

            }

            #endregion

        }


        #region INotificationType Members

        public override String Description
        {
            get { return "Notifies about an ready to use GraphDS instance."; }
        }

        public override INotificationArguments GetEmptyArgumentInstance()
        {
            return new Arguments();
        }

        #endregion
    
    }

}
