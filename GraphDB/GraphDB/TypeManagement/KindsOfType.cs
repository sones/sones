using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.TypeManagement
{

    /// <summary>
    /// The kind of a type (Single, List)
    /// </summary>

    public enum KindsOfType
    {

        SingleNoneReference,
        SingleReference,
        ListOfNoneReferences,
        SetOfNoneReferences,
        SetOfReferences,
        //SettingAttribute,
        SpecialAttribute,

        /// <summary>
        /// At the time of GetContent we just know whether this is a list or set but not if the type is userdefined.
        /// </summary>
        UnknownSet,
        /// <summary>
        /// At the time of GetContent we just know whether this is a list or set but not if the type is userdefined.
        /// </summary>
        UnknownList,
        /// <summary>
        /// At the time of GetContent we just know whether this is a list or set but not if the type is userdefined.
        /// </summary>
        UnknownSingle
    }

}
