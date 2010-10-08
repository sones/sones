/*
 * DBCLI_DBTYPEINFO
 * (c) Henning Rauch, 2009 - 2010
 */

#region Usings

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using sones.GraphDB.TypeManagement;
using sones.GraphDS.Connectors.CLI;
using sones.Lib.ErrorHandling;
using sones.GraphDS.API.CSharp;
using sones.GraphFS.Errors;

#endregion

namespace sones.GraphDB.Connectors.GraphDBCLI
{

    /// <summary>
    /// This command enables the user to get informations concerning the myAttributes of a given type.
    /// </summary>

    public class DBCLI_DBTYPEINFO : AllAdvancedDBCLICommands
    {

        #region Constructor

        public DBCLI_DBTYPEINFO()
        {

            // Command name and description
            InitCommand("DBTYPEINFO",
                        "Returns myAttributes of a given GraphType",
                        "Returns myAttributes of a given GraphType");

            // BNF rule
            CreateBNFRule(CLICommandSymbolTerminal + GraphTypeNT);

        }

        #endregion

        #region Execute Command

        public override Exceptional Execute(AGraphDSSharp myAGraphDSSharp, ref String myCurrentPath, Dictionary<String, List<AbstractCLIOption>> myOptions, String myInputString)
        {

            if (myAGraphDSSharp == null)
                return new Exceptional(new GraphDSError("myAGraphDSSharp must not be null!"));

            _CancelCommand = false;

            var typeInterestedIn = myOptions.ElementAt(1).Value[0].Option;

            using (var transaction = myAGraphDSSharp.IGraphDBSession.BeginTransaction())
            {

                var _ActualType = ((DBContext)transaction.GetDBContext()).DBTypeManager.GetTypeByName(typeInterestedIn);

                while (_ActualType != null)
                {

                    if (_ActualType.UUID.ToString().Length > 4)
                        WriteLine("(by " + _ActualType.Name + " [UUID: " + _ActualType.UUID.ToString().Substring(0, 4) + "..])");
                    else
                        WriteLine("(by " + _ActualType.Name + " [UUID: " + _ActualType.UUID.ToString() + "])");

                    if (!_ActualType.Attributes.Count.Equals(0))
                    {
                        var maxLengthName = _ActualType.Attributes.Values.Max(a => a.Name.Length);
                        foreach (var _TypeAttribute in _ActualType.Attributes.Values)
                        {
                            StringBuilder sb = new StringBuilder();

                            sb.Append(_TypeAttribute.Name.PadRight(maxLengthName + 3));
                            sb.Append("");

                            if (_TypeAttribute.TypeCharacteristics.IsBackwardEdge)
                            {
                                sb.Append("BACKWARDEDGE<");
                                var beTypeAttr = ((DBContext)transaction.GetDBContext()).DBTypeManager.GetTypeAttributeByEdge(_TypeAttribute.BackwardEdgeDefinition);
                                sb.Append(beTypeAttr.GetRelatedType(((DBContext)transaction.GetDBContext()).DBTypeManager).Name + "." + beTypeAttr.Name);
                                sb.Append(">");
                            }
                            else
                            {

                                if (_TypeAttribute.KindOfType == KindsOfType.ListOfNoneReferences)
                                    sb.Append("LIST<");

                                if (_TypeAttribute.KindOfType == KindsOfType.SetOfReferences || _TypeAttribute.KindOfType == KindsOfType.SetOfNoneReferences)
                                    sb.Append("SET<");

                                sb.Append(((DBContext)transaction.GetDBContext()).DBTypeManager.GetTypeByUUID(_TypeAttribute.DBTypeUUID).Name);

                                if (_TypeAttribute.KindOfType == KindsOfType.ListOfNoneReferences || _TypeAttribute.KindOfType == KindsOfType.SetOfNoneReferences || _TypeAttribute.KindOfType == KindsOfType.SetOfReferences)
                                    sb.Append(">");
                            }

                            sb.Append(" ");
                            sb.Append(_TypeAttribute.TypeCharacteristics.ToString());

                            sb.Append(" [UUID: ");
                            sb.Append(_TypeAttribute.UUID.ToString().Substring(0, 4));
                            sb.Append("..]");

                            WriteLine(sb.ToString());
                        }
                    }

                    else
                        WriteLine("No myAttributes for this type.");

                    WriteLine(Environment.NewLine);

                    if (_ActualType.ParentTypeUUID == null)
                        break;

                    _ActualType = ((DBContext)transaction.GetDBContext()).DBTypeManager.GetTypeByUUID(_ActualType.ParentTypeUUID);

                }
            }

            return Exceptional.OK;

        }

        #endregion

    }

}
