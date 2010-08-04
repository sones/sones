/*
 * AAttributeRemove
 * (c) Stefan Licht, 2010
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.ErrorHandling;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.Errors;
using sones.Lib;

namespace sones.GraphDB.Managers.Structures
{

    /// <summary>
    /// Abstract class for all attribute remove definitions
    /// </summary>
    public abstract class AAttributeRemove : AAttributeAssignOrUpdateOrRemove
    {
    }

}
