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

        public AcceptType(String accept, UInt32 placeOfOccurence = 0U)
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
