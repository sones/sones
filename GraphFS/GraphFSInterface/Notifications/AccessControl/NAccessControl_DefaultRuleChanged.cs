/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
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
*/


/* PandoraFS - NAccessControl_DefaultRuleChanged
 * (c) Henning Rauch, 2009
 * 
 * Notifies about a changed ACCESSCONTROLSTREAM. The DefaultRule property has been changed.
 * 
 * Lead programmer:
 *      Henning Rauch
 * 
 * */

#region Usings

using System;
using System.Text;

using sones.Notifications;
using sones.Lib.Serializer;

using sones.Lib.NewFastSerializer;

#endregion

namespace sones.GraphFS.Notification
{

    /// <summary>
    /// Notifies about a changed ACCESSCONTROLSTREAM. The DefaultRule property has been changed.
    /// </summary>
    public class NAccessControl_DefaultRuleChanged : NAccessControl
    {

        public new class Arguments : INotificationArguments
        {

            public String ObjectLocation;
            public Byte   NewDefaultRule;


            #region Constructors

            public Arguments()
            {
            }

            public Arguments(String myObjectLocation, Byte myNewDefaultRule)
            {
                ObjectLocation = myObjectLocation;
                NewDefaultRule = myNewDefaultRule;
            }

            #endregion

            #region INotificationArguments Members

            public Byte[] Serialize()
            {

                var _SerializationWriter = new SerializationWriter();

                _SerializationWriter.WriteObject(ObjectLocation);
                _SerializationWriter.WriteObject(NewDefaultRule);

                return _SerializationWriter.ToArray();

            }

            public void Deserialize(byte[] mySerializedBytes)
            {

                var _SerializationReader = new SerializationReader(mySerializedBytes);

                ObjectLocation = (String) _SerializationReader.ReadObject();
                NewDefaultRule = (Byte)   _SerializationReader.ReadObject();

            }

            #endregion

        }

        #region ANotificationType

        public override String Description
        {
            get { return "Notifies about a change concerning the DefaultRule Property."; }
        }

        public override INotificationArguments GetEmptyArgumentInstance()
        {
            return new Arguments();
        }

        #endregion

    }
}
