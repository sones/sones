/*
 * DescribeTypeDefinition
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Structures.Result;
using sones.GraphDB.Structures.EdgeTypes;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Errors;
using sones.GraphDB.TypeManagement;

#endregion

namespace sones.GraphDB.Managers.Structures.Describe
{
    public class DescribeTypeDefinition : ADescribeDefinition
    {
        
        #region Data

        private String _TypeName;
        
        #endregion

        #region Ctor

        public DescribeTypeDefinition(string myTypeName = null)
        {
            _TypeName = myTypeName;
        }

        #endregion

        #region ADescribeDefinition

        public override Exceptional<List<SelectionResultSet>> GetResult(DBContext myDBContext)
        {

            var result = new List<SelectionResultSet>();

            if (!String.IsNullOrEmpty(_TypeName))
            {

                #region Specific type

                var type = myDBContext.DBTypeManager.GetTypeByName(_TypeName);
                if (type != null)
                {
                    result.Add(new SelectionResultSet(GenerateOutput(myDBContext, type)));
                }
                else
                {
                    return new Exceptional<List<SelectionResultSet>>(new Error_TypeDoesNotExist(_TypeName));
                }

                #endregion

            }
            else
            {

                #region All types

                foreach (var type in myDBContext.DBTypeManager.GetAllTypes())
                {
                    result.Add(new SelectionResultSet(GenerateOutput(myDBContext, type)));
                }

                #endregion

            }

            return new Exceptional<List<SelectionResultSet>>(result);

        }
        
        #endregion

        #region GenerateOutput(myDBContext, myGraphDBType)

        /// <summary>
        /// Generate an output for an type with the attributes of the types and all parent types
        /// </summary>
        private IEnumerable<DBObjectReadout> GenerateOutput(DBContext myDBContext, GraphDBType myGraphDBType)
        {

            GraphDBType _ParentType = null;

            if (myGraphDBType.ParentTypeUUID != null)
                _ParentType = myDBContext.DBTypeManager.GetTypeByUUID(myGraphDBType.ParentTypeUUID);

            var _CurrentType = new Dictionary<String, Object>();

            _CurrentType.Add("Name", myGraphDBType.Name);
            _CurrentType.Add("UUID", myGraphDBType.UUID);
            _CurrentType.Add("Comment", myGraphDBType.Comment);
            _CurrentType.Add("Attributes", GenerateAttributeOutput(myGraphDBType, myDBContext));

            if (_ParentType != null)
                _CurrentType.Add("ParentType", GenerateOutput(myDBContext, _ParentType));

            return new List<DBObjectReadout>() { new DBObjectReadout(_CurrentType) };

        }

        /// <summary>
        /// output for the type attributes
        /// </summary>
        /// <param name="myGraphDBType">the type</param>
        /// <param name="myDBContext">typemanager</param>
        /// <returns>a list of readouts, contains the attributes</returns>
        private IEnumerable<DBObjectReadout> GenerateAttributeOutput(GraphDBType myGraphDBType, DBContext myDBContext)
        {

            var _AttributeReadout = new List<DBObjectReadout>();

            foreach (var _TypeAttribute in myGraphDBType.Attributes)
            {
                var Attributes = new Dictionary<String, Object>();
                Attributes.Add("Name", _TypeAttribute.Value.Name);
                Attributes.Add("Type", _TypeAttribute.Value.GetDBType(myDBContext.DBTypeManager));
                Attributes.Add("UUID", _TypeAttribute.Value.UUID);

                _AttributeReadout.Add(new DBObjectReadout(Attributes));
            }

            if (_AttributeReadout.Count > 0)
                return _AttributeReadout;
            else
                return null;
        }

        #endregion

    }
}
