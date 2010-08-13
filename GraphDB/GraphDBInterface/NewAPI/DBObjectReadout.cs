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
using sones.GraphDB.NewAPI;

#endregion

namespace sones.GraphDB.Structures.Result
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

    }

}
