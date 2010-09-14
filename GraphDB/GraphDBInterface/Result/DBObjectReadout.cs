/* <id name="GraphDB – DBObject Readout" />
 * <copyright file="DBObjectReadout.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>Carries information of DBObjects but without their whole functionality.</summary>
 */

#region Usings

using System;
using System.Text;
using System.Collections.Generic;

#endregion

namespace sones.GraphDBInterface.Result
{

    /// <summary>
    /// Carries information of DBObjects but without their whole functionality.
    /// </summary>
    [Obsolete("Please use DBVertex!")]
    public class DBObjectReadout //: DBVertex
    {

        #region Properties

        public IDictionary<String, Object> Attributes { get; private set; }

        #endregion

        #region Constructor

        public DBObjectReadout()
        {
            Attributes = new Dictionary<String, Object>();
        }

        public DBObjectReadout(IDictionary<String, Object> myAttributes)
        {
            Attributes = myAttributes;
        }

        #endregion


        #region this[myAttribute]

        public Object this[String myAttribute]
        {

            get
            {

                Object _Object = null;

                Attributes.TryGetValue(myAttribute, out _Object);

                return _Object;

            }

        }

        #endregion


        #region ToString()

        public override String ToString()
        {

            var _ReturnValue = new StringBuilder(Attributes.Count + " Attributes: ");

            foreach (var _KeyValuePair in Attributes)
                _ReturnValue.Append(_KeyValuePair.Key + " = '" + _KeyValuePair.Value + "', ");

            _ReturnValue.Length = _ReturnValue.Length - 2;

            return _ReturnValue.ToString();

        }

        #endregion

        #region Equals Overrides

        public override Boolean Equals(System.Object obj)
        {

            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            if (obj is DBObjectReadout)
            {
                return Equals((DBObjectReadout)obj);
            }
            else
            {
                return false;
            }
        }

        public Boolean Equals(DBObjectReadout p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            return CompareAttributes(this.Attributes, p.Attributes);
        }

        private bool CompareAttributes(IDictionary<string, object> iDictionary_1, IDictionary<string, object> iDictionary_2)
        {
            if (iDictionary_1.Count != iDictionary_2.Count)
            {
                return false;
            }

            if (iDictionary_1.Count == 0)
            {
                return true;
            }

            foreach (var aOuterResult in iDictionary_1)
            {
                var equals = (iDictionary_2.ContainsKey(aOuterResult.Key) && (iDictionary_2[aOuterResult.Key].Equals(aOuterResult.Value)));

                if (!equals)
                {
                    return false;
                }
            }

            return true;
        }

        public static Boolean operator ==(DBObjectReadout a, DBObjectReadout b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Equals(b);
        }

        public static Boolean operator !=(DBObjectReadout a, DBObjectReadout b)
        {
            return !(a == b);
        }

        #endregion

    }

}
