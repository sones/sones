using System;
using System.Linq;
using System.Collections.Generic;
using sones.GraphQL.StructureNodes;
using sones.GraphQL.ErrorHandling;
using Irony.Parsing;
using Irony.Ast;


namespace sones.GraphQL.StructureNodes
{

    public class MandatoryOptNode : AStructureNode, IAstNodeInit
    {

        #region Data
        private List<string> _MandAttribs;
        #endregion

        #region constructor
        public MandatoryOptNode()
        {
            _MandAttribs = new List<string>();
        }
        #endregion

        public void GetContent(ParsingContext context, ParseTreeNode parseNode)
        {

            try
            {
                if (parseNode.HasChildNodes())
                {
                    if (parseNode.ChildNodes[1].HasChildNodes())
                    {
                        _MandAttribs = (from Attr in parseNode.ChildNodes[1].ChildNodes select Attr.Token.ValueString).ToList();
                    }
                }
            }

            catch(Exception ex)
            {
                throw new Exception(ex.Message,ex);  // replace through specific GraphQLException
            }

        }
        
        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            GetContent(context, parseNode);
        }

        #endregion

        #region Accessor
        public List<string> MandatoryAttribs
        { 
            get { return _MandAttribs; } 
        }
        #endregion

    }

}
