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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mime;
using System.Globalization;

namespace sones.Networking.HTTP
{

    public class AcceptType : IComparable<AcceptType>
    {

        /// <summary>
        /// Value between 0..1, default is 1
        /// </summary>
        private Double _Quality = 1;
        public Double Quality
        {
            get { return _Quality; }
            set { _Quality = value; }
        }


        private ContentType _ContentType;
        public ContentType ContentType
        {
            get { return _ContentType; }
            set { _ContentType = value; }
        }

        private UInt32 _PlaceOfOccurence;

        public AcceptType(String accept, UInt32 placeOfOccurence = 0)
        {

            if (accept.Contains(";"))
            {
                var split = accept.Split(';');
                try
                {
                    ContentType = new ContentType(split[0]);
                    Double.TryParse(split[1].Replace("q=", "").Trim(), NumberStyles.Any, new CultureInfo("en"), out _Quality);
                }
                catch { }
            }

            else
            {
                try
                {
                    ContentType = new ContentType(accept);
                }
                catch { }
            }

            _PlaceOfOccurence = placeOfOccurence;

        }





        #region IComparable<AcceptType> Members

        public int CompareTo(AcceptType other)
        {
            if (_Quality == other._Quality)
                return _PlaceOfOccurence.CompareTo(other._PlaceOfOccurence);
            else
                return _Quality.CompareTo(other._Quality) * -1;
        }

        #endregion

        public override string ToString()
        {
            return String.Concat(_ContentType, ";", "q=", _Quality);
        }

        public override bool Equals(object obj)
        {

            if (_ContentType.Equals((obj as AcceptType).ContentType))
                return true;

            else if (_ContentType.GetMediaSubType() == "*" && _ContentType.GetMediaType().Equals((obj as AcceptType).ContentType.GetMediaType()))
                return true;

            else
                return false;

        }

        public override int GetHashCode()
        {
            return _ContentType.GetHashCode();
        }

    }

}
