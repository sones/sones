using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using sones.GraphDB.Interfaces;
using Irony.Ast;
using sones.GraphQL.Structure.Nodes.DDL;
using sones.GraphQL.Structure.Nodes.Expressions;
using sones.GraphQL.Structure.Nodes.DML;
using sones.GraphQL.Structure.Nodes.Settings;
using sones.GraphQL.Structure.Helper.Enums;
using sones.GraphQL.Structure.Nodes.Misc;
using sones.GraphQL.StatementNodes.DML;
using sones.GraphQL.StatementNodes.DDL;
using sones.GraphQL.StatementNodes.Settings;
using sones.GraphQL.StatementNodes.Transactions;
using sones.GraphDB;
using sones.GraphDB.TypeSystem;
using System.Globalization;
using sones.Library.PropertyHyperGraph;
using System.Collections;
using sones.Library.ErrorHandling;

namespace sones.GraphQL
{
    public sealed class SonesGQLGrammar : Grammar //, IDumpable, IExtendableGrammar
    {
        #region Data

        /// <summary>
        /// The IGraphDB instance that is used to get some information
        /// </summary>
        private readonly IGraphDB _iGraphDB;

        #region Consts

        public const String DOT = ".";
        public const String TERMINAL_BRACKET_LEFT = "(";
        public const String TERMINAL_BRACKET_RIGHT = ")";
        public const String TERMINAL_QUEUESIZE = "QUEUESIZE";
        public const String TERMINAL_WEIGHTED = "WEIGHTED";
        public const String TERMINAL_UNIQUE = "UNIQUE";
        public const String TERMINAL_MANDATORY = "MANDATORY";
        public const String TERMINAL_SORTED = "SORTED";
        public const String TERMINAL_ASC = "ASC";
        public const String TERMINAL_DESC = "DESC";
        public const String TERMINAL_TRUE = "TRUE";
        public const String TERMINAL_FALSE = "FALSE";
        public const String TERMINAL_LIST = "LIST";
        public const String TERMINAL_SET = "SET";
        public const String TERMINAL_LT = "<";
        public const String TERMINAL_GT = ">";

        #endregion

        #endregion

        #region KeyTerms/Nonterms

        #region NonTerminals

        public NonTerminal BNF_TypesOrVertices { get; private set; }

        #region class scope NonTerminal - need for IExtendableGrammar

        private NonTerminal BNF_ImportStmt;     //If no import format is found from plugin the statement must be removed
        private NonTerminal BNF_ImportFormat;

        private NonTerminal BNF_FuncCall;
        private NonTerminal BNF_FunArgs;

        private NonTerminal BNF_Aggregate;
        private NonTerminal BNF_AggregateArg;

        private NonTerminal selectionSource;    // If no aggregates where found we must remove them from selectionSource;

        private NonTerminal BNF_IndexTypeOpt;

        #endregion

        #endregion

        #region KeyTerms

        public KeyTerm S_CREATE { get; private set; }
        public KeyTerm S_comma { get; private set; }
        public KeyTerm S_dot { get; private set; }
        public KeyTerm S_ASTERISK { get; private set; }
        public KeyTerm S_RHOMB { get; private set; }
        public KeyTerm S_MINUS { get; private set; }
        public KeyTerm S_AD { get; private set; }
        public KeyTerm S_colon { get; private set; }
        public KeyTerm S_EQUALS { get; private set; }
        public KeyTerm S_QUESTIONMARK_EQUALS { get; private set; }

        #region Brackets

        public KeyTerm S_BRACKET_LEFT { get; private set; }
        public KeyTerm S_BRACKET_RIGHT { get; private set; }
        public KeyTerm S_TUPLE_BRACKET_LEFT { get; private set; }
        public KeyTerm S_TUPLE_BRACKET_RIGHT { get; private set; }
        public KeyTerm S_TUPLE_BRACKET_LEFT_EXCLUSIVE
        {
            get { return S_BRACKET_LEFT; }
        }
        public KeyTerm S_TUPLE_BRACKET_RIGHT_EXCLUSIVE
        {
            get { return S_BRACKET_RIGHT; }
        }

        #endregion

        public KeyTerm S_edgeInformationDelimiterSymbol { get; private set; }
        public KeyTerm S_edgeTraversalDelimiter { get; private set; }
        public KeyTerm S_NULL { get; private set; }
        public KeyTerm S_NOT { get; private set; }
        public KeyTerm S_UNIQUE { get; private set; }
        public KeyTerm S_WITH { get; private set; }
        public KeyTerm S_TABLE { get; private set; }
        public KeyTerm S_ALTER { get; private set; }
        public KeyTerm S_ADD { get; private set; }
        public KeyTerm S_TO { get; private set; }
        public KeyTerm S_COLUMN { get; private set; }
        public KeyTerm S_DROP { get; private set; }
        public KeyTerm S_RENAME { get; private set; }
        public KeyTerm S_CONSTRAINT { get; private set; }
        public KeyTerm S_INDEX { get; private set; }
        public KeyTerm S_INDICES { get; private set; }
        public KeyTerm S_ON { get; private set; }
        public KeyTerm S_KEY { get; private set; }
        public KeyTerm S_PRIMARY { get; private set; }
        public KeyTerm S_INSERT { get; private set; }
        public KeyTerm S_INTO { get; private set; }
        public KeyTerm S_UPDATE { get; private set; }
        public KeyTerm S_INSERTORUPDATE { get; private set; }
        public KeyTerm S_INSERTORREPLACE { get; private set; }
        public KeyTerm S_REPLACE { get; private set; }
        public KeyTerm S_SET { get; private set; }
        public KeyTerm S_REMOVE { get; private set; }
        public KeyTerm S_VALUES { get; private set; }
        public KeyTerm S_DELETE { get; private set; }
        public KeyTerm S_SELECT { get; private set; }
        public KeyTerm S_FROM { get; private set; }
        public KeyTerm S_AS { get; private set; }
        public KeyTerm S_COUNT { get; private set; }
        public KeyTerm S_JOIN { get; private set; }
        public KeyTerm S_BY { get; private set; }
        public KeyTerm S_WHERE { get; private set; }
        public KeyTerm S_TYPE { get; private set; }
        public KeyTerm S_TYPES { get; private set; }
        public KeyTerm S_VERTEX { get; private set; }
        public KeyTerm S_VERTICES { get; private set; }
        public KeyTerm S_EDITION { get; private set; }
        public KeyTerm S_INDEXTYPE { get; private set; }
        public KeyTerm S_LIST { get; private set; }
        public KeyTerm S_ListTypePrefix { get; private set; }
        public KeyTerm S_ListTypePostfix { get; private set; }
        public KeyTerm S_EXTENDS { get; private set; }
        public KeyTerm S_ATTRIBUTES { get; private set; }
        public KeyTerm S_MATCHES { get; private set; }
        public KeyTerm S_LIMIT { get; private set; }
        public KeyTerm S_DEPTH { get; private set; }
        public KeyTerm S_DEFINE { get; private set; }
        public KeyTerm S_UNDEFINE { get; private set; }
        public KeyTerm S_SHARDS { get; private set; }

        #region REF/REFUUID/...

        public KeyTerm S_REF { get; private set; }
        public KeyTerm S_REFERENCE { get; private set; }
        public KeyTerm S_REFUUID { get; private set; }
        public KeyTerm S_REFERENCEUUID { get; private set; }

        #endregion

        #region LISTOF/SETOF/SETOFUUIDS

        public KeyTerm S_LISTOF { get; private set; }
        public KeyTerm S_SETOF { get; private set; }
        public KeyTerm S_SETOFUUIDS { get; private set; }

        #endregion

        public KeyTerm S_UUID { get; private set; }
        public KeyTerm S_OFFSET { get; private set; }
        public KeyTerm S_TRUNCATE { get; private set; }
        public KeyTerm S_TRUE { get; private set; }
        public KeyTerm S_FALSE { get; private set; }
        public KeyTerm S_SORTED { get; private set; }
        public KeyTerm S_ASC { get; private set; }
        public KeyTerm S_DESC { get; private set; }

        public KeyTerm S_DESCRIBE { get; private set; }
        public KeyTerm S_QUEUESIZE { get; private set; }
        public KeyTerm S_WEIGHTED { get; private set; }
        public KeyTerm S_SETTING { get; private set; }
        public KeyTerm S_GET { get; private set; }
        public KeyTerm S_DB { get; private set; }
        public KeyTerm S_SESSION { get; private set; }
        public KeyTerm S_ATTRIBUTE { get; private set; }
        public KeyTerm S_DEFAULT { get; private set; }

        public KeyTerm S_BACKWARDEDGES { get; private set; }
        public KeyTerm S_BACKWARDEDGE { get; private set; }
        public KeyTerm S_FUNCTION { get; private set; }
        public KeyTerm S_AGGREGATE { get; private set; }
        public KeyTerm S_AGGREGATES { get; private set; }
        public KeyTerm S_SETTINGS { get; private set; }
        public KeyTerm S_FUNCTIONS { get; private set; }
        public KeyTerm S_EDGE { get; private set; }
        public KeyTerm S_EDGES { get; private set; }
        public KeyTerm S_MANDATORY { get; private set; }
        public KeyTerm S_ABSTRACT { get; private set; }

        #region Transactions

        public KeyTerm S_BEGIN { get; private set; }
        public KeyTerm S_TRANSACTION { get; private set; }
        public KeyTerm S_TRANSACTDISTRIBUTED { get; private set; }
        public KeyTerm S_TRANSACTLONGRUNNING { get; private set; }
        public KeyTerm S_TRANSACTISOLATION { get; private set; }
        public KeyTerm S_TRANSACTNAME { get; private set; }
        public KeyTerm S_TRANSACTTIMESTAMP { get; private set; }
        public KeyTerm S_TRANSACTCOMMIT { get; private set; }
        public KeyTerm S_TRANSACTROLLBACK { get; private set; }
        public KeyTerm S_TRANSACTCOMROLLASYNC { get; private set; }

        #endregion

        public KeyTerm S_REMOVEFROMLIST { get; private set; }
        public KeyTerm S_ADDTOLIST { get; private set; }
        public KeyTerm S_COMMENT { get; private set; }
        public KeyTerm S_REBUILD { get; private set; }

        #region IMPORT

        public KeyTerm S_IMPORT { get; private set; }
        public KeyTerm S_COMMENTS { get; private set; }
        public KeyTerm S_PARALLELTASKS { get; private set; }
        public KeyTerm S_VERBOSITY { get; private set; }
        public KeyTerm S_FORMAT { get; private set; }

        #endregion

        #region DUMP

        public KeyTerm S_DUMP { get; private set; }
        public KeyTerm S_EXPORT { get; private set; }
        public KeyTerm S_ALL { get; private set; }
        public KeyTerm S_GDDL { get; private set; }
        public KeyTerm S_GDML { get; private set; }
        public KeyTerm S_GQL { get; private set; }
        public KeyTerm S_CSV { get; private set; }



        #endregion

        #region LINK

        public KeyTerm S_VIA { get; private set; }
        public KeyTerm S_LINK { get; private set; }

        #endregion

        #region UNLINK

        public KeyTerm S_UNLINK { get; private set; }

        #endregion

        #endregion

        #endregion

        #region Constructor and Definitions

        public SonesGQLGrammar(IGraphDB iGraphDb)
            : base(false)
        {

            _iGraphDB = iGraphDb;

            #region SetLanguageFlags

            base.LanguageFlags |= LanguageFlags.CreateAst;
            //this.SetLanguageFlags(LanguageFlags.CreateAst);
            //this.SetLanguageFlags(LanguageFlags.AutoDetectTransient, false);

            #endregion

            #region Terminals

            #region Comments

            //Terminals
            var comment             = new CommentTerminal("comment", "/*", "*/");
            var lineComment = new CommentTerminal("line_comment", "--", "\n", "\r\n");
            //TODO: remove block comment, added for testing LUA-style comments
            var blockComment        = new CommentTerminal("block_comment", "--[[", "]]");
            NonGrammarTerminals.Add(comment);
            NonGrammarTerminals.Add(lineComment);
            NonGrammarTerminals.Add(blockComment);

            #endregion

            #region Available value defs: Number, String, Name

            var number              = new NumberLiteral("number", NumberOptions.AllowSign | NumberOptions.DisableQuickParse);
            number.DefaultIntTypes = new TypeCode[] { TypeCode.UInt64, TypeCode.Int64, NumberLiteral.TypeCodeBigInt };
            var string_literal      = new StringLiteral("string", "'", StringOptions.AllowsDoubledQuote | StringOptions.AllowsLineBreak);
            var location_literal    = new StringLiteral("file", "'", StringOptions.AllowsDoubledQuote | StringOptions.AllowsLineBreak | StringOptions.NoEscapes);

            var name                = new IdentifierTerminal("name", "ÄÖÜäöüß0123456789_", "ÄÖÜäöü0123456789$_");


            #endregion

            //var name_ext            = TerminalFactory.CreateSqlExtIdentifier("name_ext"); //removed, because we do not want to hav types or sth else with whitespaces, otherwise it conflicts with tupleSet

            #region Symbols

            S_CREATE = ToTerm("CREATE");
            S_comma = ToTerm(",");
            S_dot = ToTerm(".");
            S_ASTERISK = ToTerm("*");
            S_MINUS = ToTerm("-");
            S_RHOMB = ToTerm("#");
            S_AD = ToTerm("@");
            S_EQUALS = ToTerm("=");
            S_QUESTIONMARK_EQUALS = ToTerm("?=");

            S_colon = ToTerm(":");
            S_BRACKET_LEFT = ToTerm(TERMINAL_BRACKET_LEFT);
            S_BRACKET_RIGHT = ToTerm(TERMINAL_BRACKET_RIGHT);
            S_TUPLE_BRACKET_LEFT = ToTerm("[");
            S_TUPLE_BRACKET_RIGHT = ToTerm("]");
            S_edgeInformationDelimiterSymbol = ToTerm(SonesGQLConstants.EdgeInformationDelimiterSymbol);
            S_edgeTraversalDelimiter = ToTerm(SonesGQLConstants.EdgeTraversalDelimiterSymbol);
            S_NULL = ToTerm("NULL");
            S_NOT = ToTerm("NOT");
            S_UNIQUE = ToTerm("UNIQUE");
            S_WITH = ToTerm("WITH");
            S_TABLE = ToTerm("TABLE");
            S_ALTER = ToTerm("ALTER");
            S_ADD = ToTerm("ADD");
            S_TO = ToTerm("TO");
            S_COLUMN = ToTerm("COLUMN");
            S_DROP = ToTerm("DROP");
            S_RENAME = ToTerm("RENAME");
            S_CONSTRAINT = ToTerm("CONSTRAINT");
            S_INDEX = ToTerm("INDEX");
            S_INDICES = ToTerm("INDICES");
            S_ON = ToTerm("ON");
            S_KEY = ToTerm("KEY");
            S_PRIMARY = ToTerm("PRIMARY");
            S_INSERT = ToTerm("INSERT");
            S_INTO = ToTerm("INTO");
            S_UPDATE = ToTerm("UPDATE");
            S_INSERTORUPDATE = ToTerm("INSERTORUPDATE");
            S_INSERTORREPLACE = ToTerm("INSERTORREPLACE");
            S_REPLACE = ToTerm("REPLACE");
            S_SET = ToTerm(TERMINAL_SET);
            S_REMOVE = ToTerm("REMOVE");
            S_VALUES = ToTerm("VALUES");
            S_DELETE = ToTerm("DELETE");
            S_SELECT = ToTerm("SELECT");
            S_FROM = ToTerm("FROM");
            S_AS = ToTerm("AS");
            S_COUNT = ToTerm("COUNT");
            S_JOIN = ToTerm("JOIN");
            S_BY = ToTerm("BY");
            S_WHERE = ToTerm("WHERE");
            S_TYPE = ToTerm("TYPE");
            S_TYPES = ToTerm("TYPES");
            S_VERTEX = ToTerm("VERTEX");
            S_VERTICES = ToTerm("VERTICES");
            S_EDITION = ToTerm("EDITION");
            S_INDEXTYPE = ToTerm("INDEXTYPE");
            S_LIST = ToTerm(TERMINAL_LIST);
            S_ListTypePrefix = ToTerm(TERMINAL_LT);
            S_ListTypePostfix = ToTerm(TERMINAL_GT);
            S_EXTENDS = ToTerm("EXTENDS");
            S_ATTRIBUTES = ToTerm("ATTRIBUTES");
            S_MATCHES = ToTerm("MATCHES");
            S_LIMIT = ToTerm("LIMIT");
            S_DEPTH = ToTerm("DEPTH");
            S_REFERENCE = ToTerm("REFERENCE");
            S_REF = ToTerm("REF");
            S_REFUUID = ToTerm("REFUUID");
            S_REFERENCEUUID = ToTerm("REFERENCEUUID");
            S_LISTOF = ToTerm(SonesGQLConstants.LISTOF);
            S_SETOF = ToTerm(SonesGQLConstants.SETOF);
            S_SETOFUUIDS = ToTerm(SonesGQLConstants.SETOFUUIDS);
            S_UUID = ToTerm("UUID");
            S_OFFSET = ToTerm("OFFSET");
            S_TRUNCATE = ToTerm("TRUNCATE");
            S_TRUE = ToTerm(TERMINAL_TRUE);
            S_FALSE = ToTerm(TERMINAL_FALSE);
            S_SORTED = ToTerm(TERMINAL_SORTED);
            S_ASC = ToTerm(TERMINAL_ASC);
            S_DESC = ToTerm(TERMINAL_DESC);
            S_QUEUESIZE = ToTerm(TERMINAL_QUEUESIZE);
            S_WEIGHTED = ToTerm(TERMINAL_WEIGHTED);
            S_GET = ToTerm("GET");
            S_DB = ToTerm("DB");
            S_SESSION = ToTerm("SESSION");
            S_ATTRIBUTE = ToTerm("ATTRIBUTE");
            S_DEFAULT = ToTerm("DEFAULT");
            S_BACKWARDEDGE = ToTerm("BACKWARDEDGE");
            S_BACKWARDEDGES = ToTerm("BACKWARDEDGES");
            S_DESCRIBE = ToTerm("DESCRIBE");
            S_FUNCTION = ToTerm("FUNCTION");
            S_FUNCTIONS = ToTerm("FUNCTIONS");
            S_AGGREGATE = ToTerm("AGGREGATE");
            S_AGGREGATES = ToTerm("AGGREGATES");
            S_SETTING = ToTerm("SETTING");
            S_SETTINGS = ToTerm("SETTINGS");
            S_INDICES = ToTerm("INDICES");
            S_EDGE = ToTerm("EDGE");
            S_EDGES = ToTerm("EDGES");
            S_MANDATORY = ToTerm("MANDATORY");
            S_ABSTRACT = ToTerm("ABSTRACT");
            S_BEGIN = ToTerm("BEGIN");
            S_TRANSACTION = ToTerm("TRANSACTION");
            S_TRANSACTDISTRIBUTED = ToTerm(SonesGQLConstants.TRANSACTION_DISTRIBUTED);
            S_TRANSACTLONGRUNNING = ToTerm(SonesGQLConstants.TRANSACTION_LONGRUNNING);
            S_TRANSACTISOLATION = ToTerm(SonesGQLConstants.TRANSACTION_ISOLATION);
            S_TRANSACTNAME = ToTerm(SonesGQLConstants.TRANSACTION_NAME);
            S_TRANSACTTIMESTAMP = ToTerm(SonesGQLConstants.TRANSACTION_TIMESTAMP);
            S_TRANSACTROLLBACK = ToTerm(SonesGQLConstants.TRANSACTION_ROLLBACK);
            S_TRANSACTCOMMIT = ToTerm(SonesGQLConstants.TRANSACTION_COMMIT);
            S_TRANSACTCOMROLLASYNC = ToTerm(SonesGQLConstants.TRANSACTION_COMROLLASYNC);
            S_ADDTOLIST = ToTerm("+=");
            S_REMOVEFROMLIST = ToTerm("-=");
            S_DUMP = ToTerm("DUMP");
            S_EXPORT = ToTerm("EXPORT");
            S_ALL = ToTerm("ALL");
            S_GDDL = ToTerm("GDDL");
            S_GDML = ToTerm("GDML");
            S_GQL = ToTerm("GQL");
            S_CSV = ToTerm("CSV");
            S_COMMENT = ToTerm("COMMENT");
            S_REBUILD = ToTerm("REBUILD");
            S_DEFINE = ToTerm("DEFINE");
            S_UNDEFINE = ToTerm("UNDEFINE");
            S_VIA = ToTerm("VIA");
            S_LINK = ToTerm("LINK");
            S_UNLINK = ToTerm("UNLINK");
            S_SHARDS = ToTerm("SHARDS");

            #region IMPORT

            S_IMPORT = ToTerm("IMPORT");
            S_COMMENTS = ToTerm("COMMENTS");
            S_PARALLELTASKS = ToTerm("PARALLELTASKS");
            S_VERBOSITY = ToTerm("VERBOSITY");
            S_FORMAT = ToTerm("FORMAT");

            #endregion

            #endregion

            #endregion

            #region Non-Terminals

            #region ID related

            var Id                              = new NonTerminal("Id", CreateIDNode);
            var Id_simple                       = new NonTerminal("id_simple", typeof(AstNode));
            var id_typeAndAttribute             = new NonTerminal("id_typeAndAttribute");
            var idlist                          = new NonTerminal("idlist");
            var id_simpleList                   = new NonTerminal("id_simpleList");
            var id_simpleDotList                = new NonTerminal("id_simpleDotList");
            var IdOrFunc                        = new NonTerminal("IdOrFunc");
            var IdOrFuncList                    = new NonTerminal("IdOrFuncList", CreateIDNode);
            var IDOrFuncDelimiter               = new NonTerminal("IDOrFuncDelimiter");
            var dotWrapper                      = new NonTerminal("dotWrapper", CreateDotDelimiter);
            var edgeAccessorWrapper             = new NonTerminal("edgeAccessorWrapper", CreateEdgeAccessorDelimiter);
            var EdgeInformation                 = new NonTerminal("EdgeInformation", CreateEdgeInformation);
            var EdgeTraversalWithFunctions      = new NonTerminal("EdgeTraversalWithFunctions", CreateEdgeTraversal);
            var EdgeTraversalWithOutFunctions   = new NonTerminal("EdgeTraversalWithOutFunctions", CreateEdgeTraversal);

            #endregion

            #region AStatements

            var singlestmt                  = new NonTerminal("singlestmt");
            //            var stmt = new NonTerminal("stmt", typeof(StatementNode)); 
            var createTableStmt             = new NonTerminal("createTableStmt");
            var createIndexStmt             = new NonTerminal("createIndexStmt", CreateCreateIndexStatementNode);
            var alterStmt                   = new NonTerminal("alterStmt", CreateAlterStmNode);
            var dropTypeStmt                = new NonTerminal("dropTypeStmt", CreateDropTypeStmNode);
            var dropIndexStmt               = new NonTerminal("dropIndexStmt", CreateDropIndexStmNode);
            var InsertStmt                  = new NonTerminal("InsertStmt", CreateInsertStatementNode);
            var updateStmt                  = new NonTerminal("updateStmt", CreateUpdateStatementNode);
            var deleteStmt                  = new NonTerminal("deleteStmt", CreateDeleteStatementNode);
            var SelectStmtGraph             = new NonTerminal("SelectStmtGraph", CreateSelectStatementNode);
            var parSelectStmt               = new NonTerminal("parSelectStmt", CreatePartialSelectStmtNode);
            var createTypesStmt             = new NonTerminal("createTypesStmt", CreateCreateTypesStatementNode);
            var insertorupdateStmt          = new NonTerminal("insertorupdateStmt", CreateInsertOrUpdateStatementNode);
            var insertorreplaceStmt         = new NonTerminal("insertorreplaceStmt", CreateInsertOrReplaceStatementNode);
            var replaceStmt                 = new NonTerminal("replaceStmt", CreateReplaceStatementNode);
            var transactStmt                = new NonTerminal("transactStmt", CreateTransActionStatementNode);
            var commitRollBackTransactStmt  = new NonTerminal("commitRollBackTransactStmt", CreateCommitRollbackTransActionNode);
            var linkStmt                    = new NonTerminal("linkStmt", CreateLinkStmtNode);
            var unlinkStmt                  = new NonTerminal("unlinkStmt", CreateUnlinkStmt);

            #endregion

            var deleteStmtMember            = new NonTerminal("deleteStmtMember");
            var uniqueOpt                   = new NonTerminal("uniqueOpt", typeof(UniqueOptNode));
            var IndexAttributeList          = new NonTerminal("IndexAttributeList", typeof(IndexAttributeListNode));
            var IndexAttributeMember        = new NonTerminal("IndexAttributeMember", typeof(IndexAttributeNode));
            var IndexAttributeType          = new NonTerminal("IndexAttributeType");
            var orderByAttributeList        = new NonTerminal("orderByAttributeList");
            var orderByAttributeListMember  = new NonTerminal("orderByAttributeListMember");
            var AttributeOrderDirectionOpt  = new NonTerminal("AttributeOrderDirectionOpt");
            BNF_IndexTypeOpt = new NonTerminal("indexTypeOpt", typeof(IndexTypeOptNode));
            var indexNameOpt                = new NonTerminal("indextNameOpt", typeof(IndexNameOptNode));
            var editionOpt                  = new NonTerminal("editionOpt", typeof(EditionOptNode));
            var alterCmd                    = new NonTerminal("alterCmd", typeof(AlterCommandNode));
            var alterCmdList                = new NonTerminal("alterCmdList");
            var insertData                  = new NonTerminal("insertData");
            var intoOpt                     = new NonTerminal("intoOpt");
            var assignList                  = new NonTerminal("assignList");
            var whereClauseOpt              = new NonTerminal("whereClauseOpt", CreateWhereExpressionNode);
            var extendsOpt                  = new NonTerminal("extendsOpt");
            var abstractOpt                 = new NonTerminal("abstractOpt");
            var commentOpt                  = new NonTerminal("CommentOpt");
            var bulkTypeList                = new NonTerminal("bulkTypeList");
            var attributesOpt               = new NonTerminal("attributesOpt");
            var insertValuesOpt             = new NonTerminal("insertValuesOpt");
            var optionalShards              = new NonTerminal("optionalShards", typeof(ShardsNode));

            #region Expression

            var BNF_Expression              = new NonTerminal("expression", typeof(ExpressionNode));
            var expressionOfAList           = new NonTerminal("expressionOfAList", typeof(ExpressionOfAListNode));
            var BNF_ExprList                = new NonTerminal("exprList");
            var exprListOfAList             = new NonTerminal("exprListOfAList");
            var unExpr                      = new NonTerminal("unExpr", CreateUnExpressionNode);
            var unOp                        = new NonTerminal("unOp");
            var binExpr                     = new NonTerminal("binExpr", CreateBinaryExpressionNode);
            var binOp                       = new NonTerminal("binOp");
            var inExpr                      = new NonTerminal("inExpr");

            #endregion

            #region Select

            var selList                     = new NonTerminal("selList");
            var fromClauseOpt               = new NonTerminal("fromClauseOpt");
            var groupClauseOpt              = new NonTerminal("groupClauseOpt");
            var havingClauseOpt             = new NonTerminal("havingClauseOpt", typeof(HavingExpressionNode));
            var orderClauseOpt              = new NonTerminal("orderClauseOpt", typeof(OrderByNode));
            var selectionList               = new NonTerminal("selectionList");
            var selectionListElement        = new NonTerminal("selectionListElement", typeof(SelectionListElementNode));
            selectionSource = new NonTerminal("selectionSource");
            var selByType                   = new NonTerminal("selByType", CreateSelByTypeNode);
            var aliasOpt                    = new NonTerminal("aliasOpt");
            var aliasOptName                = new NonTerminal("aliasOptName");
            var selectOutputOpt             = new NonTerminal("selectOutputOpt", typeof(SelectOutputOptNode));

            #endregion

            #region Aggregates & Functions

            BNF_Aggregate = new NonTerminal("aggregate", CreateAggregateNode);
            BNF_AggregateArg = new NonTerminal("aggregateArg");
            var function                    = new NonTerminal("function", CreateFunctionCallNode);
            var functionName                = new NonTerminal("functionName");
            BNF_FunArgs = new NonTerminal("funArgs");
            BNF_FuncCall = new NonTerminal("funCall", CreateFunctionCallNode);

            #endregion

            #region Tuple

            var tuple                       = new NonTerminal("tuple", typeof(TupleNode));
            var bracketLeft                 = new NonTerminal(SonesGQLConstants.BracketLeft);
            var bracketRight                = new NonTerminal(SonesGQLConstants.BracketRight);


            #endregion

            var term                        = new NonTerminal("term");
            var notOpt                      = new NonTerminal("notOpt");

            var typeOrVertex                = new NonTerminal("typeOrVertex");
            BNF_TypesOrVertices = new NonTerminal("typesOrVertices");

            var GraphDBType                 = new NonTerminal(SonesGQLConstants.GraphDBType, CreateGraphDBTypeNode);
            var AttributeList               = new NonTerminal("AttributeList");
            var AttrDefinition              = new NonTerminal("AttrDefinition", CreateAttributeDefinitionNode);
            var ResultObject                = new NonTerminal("ResultObject");
            var ResultList                  = new NonTerminal("ResultList");
            var MatchingClause              = new NonTerminal("MatchingClause");
            var Matching                    = new NonTerminal("MatchingClause");
            var PrefixOperation             = new NonTerminal("PrefixOperation");
            var ParameterList               = new NonTerminal("ParameterList");
            var TypeList                    = new NonTerminal("TypeList", CreateTypeListNode);
            var AType                       = new NonTerminal("AType", CreateATypeNode);
            var TypeWrapper                 = new NonTerminal("TypeWrapper");

            #region Attribute changes

            var AttrAssignList              = new NonTerminal("AttrAssignList", CreateAttrAssignListNode);
            var AttrUpdateList              = new NonTerminal("AttrUpdateList", typeof(AttrUpdateOrAssignListNode));
            var AttrAssign                  = new NonTerminal("AttrAssign", typeof(AttributeAssignNode));
            var AttrRemove                  = new NonTerminal("AttrRemove", typeof(AttrRemoveNode));
            var ListAttrUpdate              = new NonTerminal("AttrUpdate");
            var AddToListAttrUpdate         = new NonTerminal("AddToListAttrUpdate", typeof(AddToListAttrUpdateNode));
            var AddToListAttrUpdateAddTo    = new NonTerminal("AddToListAttrUpdateAddTo", CreateAddToListAttrUpdateAddToNode);
            var AddToListAttrUpdateOperator = new NonTerminal("AddToListAttrUpdateOperator", CreateAddToListAttrUpdateOperatorNode);
            var RemoveFromListAttrUpdate    = new NonTerminal("RemoveFromListAttrUpdate", typeof(RemoveFromListAttrUpdateNode));
            var RemoveFromListAttrUpdateAddToRemoveFrom = new NonTerminal("RemoveFromListAttrUpdateAddToRemoveFrom", CreateRemoveFromListAttrUpdateAddToRemoveFromNode);
            var RemoveFromListAttrUpdateAddToOperator   = new NonTerminal("RemoveFromListAttrUpdateAddToOperator", CreateRemoveFromListAttrUpdateAddToOperatorNode);
            var RemoveFromListAttrUpdateScope   = new NonTerminal("RemoveFromListAttrUpdateScope", CreateRemoveFromListAttrUpdateScope);
            var AttrUpdateOrAssign              = new NonTerminal("AttrUpdateOrAssign");
            var CollectionOfDBObjects           = new NonTerminal("ListOfDBObjects", typeof(CollectionOfDBObjectsNode));
            var CollectionTuple                 = new NonTerminal("CollectionTuple", typeof(TupleNode));
            var ExtendedExpressionList          = new NonTerminal("ExtendedExpressionList");
            var ExtendedExpression              = new NonTerminal("ExtendedExpression", typeof(ExpressionOfAListNode));

            #endregion

            var Reference                   = new NonTerminal(S_REFERENCE.Symbol.Text, typeof(SetRefNode));
            var offsetOpt                   = new NonTerminal("offsetOpt", typeof(OffsetNode));
            var resolutionDepthOpt          = new NonTerminal("resolutionDepthOpt");
            var limitOpt                    = new NonTerminal("limitOpt", typeof(LimitNode));
            var SimpleIdList                = new NonTerminal("SimpleIdList");
            var bulkTypeListMember          = new NonTerminal("bulkTypeListMember", CreateBulkTypeListMemberNode);
            var bulkType                    = new NonTerminal("bulkType", CreateBulkTypeNode);
            var truncateStmt                = new NonTerminal("truncateStmt", CreateTruncateStmNode);
            var uniquenessOpt               = new NonTerminal("UniquenessOpt", typeof(UniqueAttributesOptNode));
            var mandatoryOpt                = new NonTerminal("MandatoryOpt", typeof(MandatoryOptNode));

            #region Transactions

            var TransactOptions             = new NonTerminal("TransactOptions");
            var TransactAttributes          = new NonTerminal("TransactAttributes");
            var TransactIsolation           = new NonTerminal("TransactIsolation");
            var TransactName                = new NonTerminal("TransactName");
            var TransactTimestamp           = new NonTerminal("TransactTimestamp");
            var TransactCommitRollbackOpt   = new NonTerminal("TransactCommitRollbackOpt");
            var TransactCommitRollbackType  = new NonTerminal("TransactCommitRollbackType");

            #endregion

            var Value                       = new NonTerminal("Value");
            var ValueList                   = new NonTerminal("ValueList");
            var BooleanVal                  = new NonTerminal("BooleanVal");

            var ListType                    = new NonTerminal("ListType");
            var ListParametersForExpression = new NonTerminal("ListParametersForExpression", typeof(ParametersNode));
            var LinkCondition               = new NonTerminal("LinkCondition");

            #region EdgeType

            var EdgeTypeDef                 = new NonTerminal("EdgeTypeDef", CreateEdgeTypeDefNode);
            var SingleEdgeTypeDef           = new NonTerminal("EdgeTypeDef", CreateSingleEdgeTypeDefNode);
            var DefaultValueDef             = new NonTerminal("DefaultValueDef", typeof(DefaultValueDefNode));
            var EdgeTypeParams              = new NonTerminal("EdgeTypeParams", typeof(EdgeTypeParamsNode));
            var EdgeTypeParam               = new NonTerminal("EdgeTypeParamNode", typeof(EdgeTypeParamNode));
            var EdgeType_Sorted             = new NonTerminal("ListPropertyAssign_Sorted", typeof(EdgeType_SortedNode));
            var EdgeType_SortedMember       = new NonTerminal("ListPropertyAssign_SortedMember");
            var AttrDefaultOpValue          = new NonTerminal("AttrDefaultOpValue", CreateAttrDefaultValueNode);

            #endregion

            #region Settings

            var SettingsStatement           = new NonTerminal("SettingStatement", CreateSettingStatementNode);
            var SettingTypeNode             = new NonTerminal("SettingTypeNode", CreateSettingTypeNode);
            var SettingAttrNode             = new NonTerminal("SettingAttrNode", CreateSettingAttrNode);
            var SettingScope                = new NonTerminal("SettingScope", typeof(SettingScopeNode));
            var SettingOpGet                = new NonTerminal("SettingOpGet");
            var SettingOpSet                = new NonTerminal("SettingOpSet");
            var SettingOpRemove             = new NonTerminal("SettingOpRemove");
            var SettingOperation            = new NonTerminal("SettingOperation", typeof(SettingOperationNode));
            var SettingOpSetLst             = new NonTerminal("SettingOpSetLst");
            var SettingOpGetLst             = new NonTerminal("SettingOpGetLst");
            var SettingOpRemLst             = new NonTerminal("SettingOpRemLst");
            var SettingItems                = new NonTerminal("SettingItems");
            var SettingItemsSet             = new NonTerminal("SettingItemsSet");
            var SettingItemSetVal           = new NonTerminal("SettingItemSetVal");
            var SettingItemSetLst           = new NonTerminal("SettingItemSetLst");
            var SettingTypeStmLst           = new NonTerminal("SettingTypeStmLst");
            var SettingAttrStmLst           = new NonTerminal("SettingAttrStmLst");

            #endregion

            #region BackwardEdges

            var backwardEdgesOpt            = new NonTerminal("BackwardEdges", CreateBackwardEdgesNode);
            var BackwardEdgesSingleDef      = new NonTerminal("BackwardEdgesSingleDef", CreateBackwardEdgeNode);
            var BackwardEdgesList           = new NonTerminal("BackwardEdgesList");

            #endregion

            #region Index

            var indexOptOnCreateType            = new NonTerminal("IndexOptOnCreateType");
            var indexOnCreateType               = new NonTerminal("indexOnCreateType", CreateIndexOnCreateType);
            var IndexOptOnCreateTypeMember      = new NonTerminal("IndexOptOnCreateTypeMember", CreateIndexOptOnCreateTypeMemberNode);
            var IndexOptOnCreateTypeMemberList  = new NonTerminal("IndexOptOnCreateTypeMemberList");
            var IndexDropOnAlterType            = new NonTerminal("IndexDropOnAlterType", CreateDropIndicesNode);
            var IndexDropOnAlterTypeMember      = new NonTerminal("IndexDropOnAlterTypeMember");
            var IndexDropOnAlterTypeMemberList  = new NonTerminal("IndexDropOnAlterTypeMemberList");

            #endregion

            #region Dump/Export

            var dumpStmt                        = new NonTerminal("Dump", CreateDumpNode);
            var dumpType                        = new NonTerminal("dumpType", CreateDumpTypeNode);
            var dumpFormat                      = new NonTerminal("dumpFormat", CreateDumpFormatNode);
            var typeOptionalList                = new NonTerminal("typeOptionalList");
            var dumpDestination                 = new NonTerminal("dumpDestination");

            #endregion

            #region Describe

            var DescrInfoStmt               = new NonTerminal("DescrInfoStmt", CreateDescribeNode);
            var DescrArgument               = new NonTerminal("DescrArgument");
            var DescrFuncStmt               = new NonTerminal("DescrFuncStmt", CreateDescrFunc);
            var DescrFunctionsStmt          = new NonTerminal("DescrFunctionsStmt", CreateDescrFunctions);
            var DescrAggrStmt               = new NonTerminal("DescrAggrStmt", CreateDescrAggr);
            var DescrAggrsStmt              = new NonTerminal("DescrAggrsStmt", CreateDescrAggrs);
            var DescrSettStmt               = new NonTerminal("DescrSettStmt", CreateDescrSett);
            var DescrSettItem               = new NonTerminal("DescrSettItem", CreateDescrSettItem);
            var DescrSettingsItems          = new NonTerminal("DescrSettingsItems", CreateDescrSettingsItems);
            var DescrSettingsStmt           = new NonTerminal("DescrSettingsStmt", CreateDescrSettings);
            var DescrTypeStmt               = new NonTerminal("DescrTypeStmt", CreateDescrType);
            var DescrTypesStmt              = new NonTerminal("DescrTypesStmt", CreateDescrTypes);
            var DescrIdxStmt                = new NonTerminal("DescrIdxStmt", CreateDescrIdx);
            var DescrIdxsStmt               = new NonTerminal("DescrIdxsStmt", CreateDescrIdxs);
            var DescrIdxEdtStmt             = new NonTerminal("DescrIdxEdtStmt");
            var DescrEdgeStmt               = new NonTerminal("DescrEdgeStmt", CreateDescrEdge);
            var DescrEdgesStmt              = new NonTerminal("DescrEdgesStmt", CreateDescrEdges);

            #endregion

            #region REBUILD INDICES

            var rebuildIndicesStmt          = new NonTerminal("rebuildIndicesStmt", CreateRebuildIndicesNode);
            var rebuildIndicesTypes         = new NonTerminal("rebuildIndiceTypes");

            #endregion

            #region Import

            BNF_ImportFormat = new NonTerminal("importFormat");
            BNF_ImportStmt = new NonTerminal("import", CreateImportNode);
            var paramParallelTasks  = new NonTerminal("parallelTasks", CreateParallelTaskNode);
            var paramComments       = new NonTerminal("comments", CreateCommentsNode);
            var verbosity           = new NonTerminal("verbosity", CreateVerbosityNode);
            var verbosityTypes      = new NonTerminal("verbosityTypes");

            #endregion

            #endregion

            #region Statements

            #region GQL root

            //BNF Rules
            this.Root = singlestmt;

            singlestmt.Rule = SelectStmtGraph
                            | InsertStmt
                            | alterStmt
                            | updateStmt
                            | dropTypeStmt
                            | dropIndexStmt
                            | createIndexStmt
                            | createTypesStmt
                            | deleteStmt
                            | SettingsStatement
                            | truncateStmt
                            | DescrInfoStmt
                            | insertorupdateStmt
                            | insertorreplaceStmt
                            | replaceStmt
                            | dumpStmt
                            | transactStmt
                            | commitRollBackTransactStmt
                            | rebuildIndicesStmt
                            | BNF_ImportStmt
                            | linkStmt
                            | unlinkStmt;


            #endregion

            #region misc

            #region ID

            #region wo functions

            Id_simple.Rule = name;

            EdgeTraversalWithOutFunctions.Rule = dotWrapper + Id_simple;

            Id.SetFlag(TermFlags.IsList);
            Id.Rule = Id_simple
                        | Id + EdgeTraversalWithOutFunctions;
            //old
            //Id.Rule = MakePlusRule(Id, dotWrapper, Id_simple);

            idlist.Rule = MakePlusRule(idlist, S_comma, Id);
            id_simpleList.Rule = MakePlusRule(id_simpleList, S_comma, Id_simple);
            id_simpleDotList.Rule = MakePlusRule(id_simpleDotList, S_dot, Id_simple);
            id_typeAndAttribute.Rule = TypeWrapper + S_dot + Id;

            #endregion

            #region ID_or_Func

            IdOrFunc.Rule = name
                            | BNF_FuncCall;

            dotWrapper.Rule = S_edgeTraversalDelimiter;

            edgeAccessorWrapper.Rule = S_edgeInformationDelimiterSymbol;

            //IDOrFuncDelimiter.Rule =        dotWrapper
            //                            |   edgeAccessorWrapper;

            EdgeTraversalWithFunctions.Rule = dotWrapper + IdOrFunc;

            EdgeInformation.Rule = edgeAccessorWrapper + Id_simple;

            IdOrFuncList.SetFlag(TermFlags.IsList);
            IdOrFuncList.Rule = IdOrFunc
                                    | IdOrFuncList + EdgeInformation
                                    | IdOrFuncList + EdgeTraversalWithFunctions;

            //old
            //IdOrFuncList.Rule = MakePlusRule(IdOrFuncList, IDOrFuncDelimiter, IdOrFunc);

            #endregion

            #endregion

            #region typeList

            TypeList.Rule = MakePlusRule(TypeList, S_comma, AType);

            AType.Rule = Id_simple + Id_simple
                        | Id_simple;

            //AType.Rule = Id + Id_simple
            //                | Id;

            TypeWrapper.Rule = AType;

            #endregion

            #region CreateIndexAttribute

            IndexAttributeList.Rule = MakePlusRule(IndexAttributeList, S_comma, IndexAttributeMember);

            IndexAttributeMember.Rule = IndexAttributeType;// + AttributeOrderDirectionOpt;

            IndexAttributeType.Rule = IdOrFuncList;// Id_simple | id_typeAndAttribute;

            #endregion

            #region OrderDirections

            AttributeOrderDirectionOpt.Rule = Empty
                                                | S_ASC
                                                | S_DESC;

            #endregion

            #region Boolean

            BooleanVal.Rule = S_TRUE | S_FALSE;

            #endregion

            #region Value

            Value.Rule = string_literal | number | BooleanVal;

            ValueList.Rule = MakeStarRule(ValueList, S_comma, Value);

            #endregion

            #region ListType

            ListType.Rule = S_LIST;

            ListParametersForExpression.Rule = Empty
                                         | S_colon + S_BRACKET_LEFT + ValueList + S_BRACKET_RIGHT;

            EdgeType_SortedMember.Rule = S_ASC | S_DESC;
            EdgeType_Sorted.Rule = S_SORTED + "=" + EdgeType_SortedMember;

            #endregion

            #region GraphType

            //                 SET<                   WEIGHTED  (Double, DEFAULT=2, SORTED=DESC)<   [idsimple]  >>
            EdgeTypeDef.Rule = S_SET + S_ListTypePrefix + Id_simple + S_BRACKET_LEFT + EdgeTypeParams + S_BRACKET_RIGHT + S_ListTypePrefix + Id_simple + S_ListTypePostfix + S_ListTypePostfix;
            //                       COUNTED        (Integer, DEFAULT=2)                   <   [idsimple]  >
            SingleEdgeTypeDef.Rule = Id_simple + S_BRACKET_LEFT + EdgeTypeParams + S_BRACKET_RIGHT + S_ListTypePrefix + Id_simple + S_ListTypePostfix;

            EdgeTypeParams.Rule = MakeStarRule(EdgeTypeParams, S_comma, EdgeTypeParam);
            EdgeTypeParam.Rule = Id_simple
                               | DefaultValueDef
                               | EdgeType_Sorted
                               | string_literal;

            EdgeTypeParam.SetFlag(TermFlags.IsTransient, false);

            DefaultValueDef.Rule = S_DEFAULT + "=" + Value;

            GraphDBType.Rule = Id_simple
                // LIST<[idsimple]>
                                   | S_LIST + S_ListTypePrefix + Id_simple + S_ListTypePostfix
                                   | S_SET + S_ListTypePrefix + Id_simple + S_ListTypePostfix
                                   | EdgeTypeDef
                                   | SingleEdgeTypeDef;

            #endregion

            #region AttributeList

            AttributeList.Rule = MakePlusRule(AttributeList, S_comma, AttrDefinition);

            AttrDefinition.Rule = GraphDBType + Id_simple + AttrDefaultOpValue;

            #endregion

            #region BackwardEdgesList

            BackwardEdgesList.Rule = MakePlusRule(BackwardEdgesList, S_comma, BackwardEdgesSingleDef);

            BackwardEdgesSingleDef.Rule = Id_simple + S_dot + Id_simple + Id_simple;
            //| Id_simple + S_dot + Id_simple + S_ListTypePrefix + Id_simple + S_BRACKET_LEFT + EdgeTypeParams + S_BRACKET_RIGHT + S_ListTypePostfix + Id_simple;

            #endregion

            #region id_simple list

            SimpleIdList.Rule = MakePlusRule(SimpleIdList, S_comma, Id_simple);

            #endregion

            #region expression

            //Expression
            BNF_ExprList.Rule = MakeStarRule(BNF_ExprList, S_comma, BNF_Expression);

            exprListOfAList.Rule = MakePlusRule(exprListOfAList, S_comma, expressionOfAList);

            BNF_Expression.Rule = term
                                | unExpr
                                | binExpr;

            expressionOfAList.Rule = BNF_Expression + ListParametersForExpression;


            term.Rule = IdOrFuncList                  //d.Name 
                            | string_literal      //'lala'
                            | number              //10
                //|   funcCall            //EXISTS ( SelectStatement )
                            | BNF_Aggregate           //COUNT ( SelectStatement )
                            | tuple               //(d.Name, 'Henning', (SelectStatement))
                            | parSelectStmt      //(FROM User u Select u.Name)
                            | S_TRUE
                            | S_FALSE;

            #region Tuple

            tuple.Rule = bracketLeft + BNF_ExprList + bracketRight;

            bracketLeft.Rule = S_BRACKET_LEFT | S_TUPLE_BRACKET_LEFT;
            bracketRight.Rule = S_BRACKET_RIGHT | S_TUPLE_BRACKET_RIGHT;

            #endregion

            parSelectStmt.Rule = S_BRACKET_LEFT + SelectStmtGraph + S_BRACKET_RIGHT;

            unExpr.Rule = unOp + term;

            unOp.Rule = S_NOT
                            | "+"
                            | "-"
                            | "~";

            binExpr.Rule = BNF_Expression + binOp + BNF_Expression;

            binOp.Rule = ToTerm("+")
                            | "-"
                            | "*"
                            | "/"
                            | "%" //arithmetic
                            | "&"
                            | "|"
                            | "^"                     //bit
                            | "="
                            | ">"
                            | "<"
                            | ">="
                            | "<="
                            | "<>"
                            | "!="
                            | "!<"
                            | "!>"
                            | "AND"
                            | "OR"
                            | "LIKE"
                            | S_NOT + "LIKE"
                            | "IN"
                            | "NOTIN" | "NOT_IN" | "NIN" | "!IN"
                            | "INRANGE";

            notOpt.Rule = Empty
                            | S_NOT;

            #endregion

            #region Functions & Aggregates

            //funcCall covers some psedo-operators and special forms like ANY(...), SOME(...), ALL(...), EXISTS(...), IN(...)
            //funcCall.Rule = BNF_FuncCallName + S_BRACKET_LEFT + funArgs + S_BRACKET_RIGHT;

            // The grammar will be created by IExtendableGrammer methods
            //BNF_Aggregate.Rule = Empty;
            //BNF_FuncCall.Rule = Empty;


            BNF_FunArgs.Rule = SelectStmtGraph
                            | BNF_ExprList;

            #endregion

            #region operators

            //Operators
            RegisterOperators(10, "*", "/", "%");
            RegisterOperators(9, "+", "-");
            RegisterOperators(8, "=", ">", "<", ">=", "<=", "<>", "!=", "!<", "!>", "INRANGE", "LIKE", "IN", "NOTIN", "NOT_IN", "NIN", "!IN");
            RegisterOperators(7, "^", "&", "|");
            RegisterOperators(6, "NOT");
            RegisterOperators(5, "AND", "OR");

            #region operators
            // Why this definition was twice in the GraphQL???
            //RegisterOperators(1, Associativity.Neutral, "AND", "OR");
            //RegisterOperators(2, Associativity.Neutral, "=", "!=", ">", ">=", "<", "<=", "<>", "!<", "!>", "IN", "NOTIN", "INRANGE");
            //RegisterOperators(3, "+", "-");
            //RegisterOperators(4, "*", "/");
            RegisterOperators(5, Associativity.Right, "**");
            #endregion

            #endregion

            #region prefixOperation

            PrefixOperation.Rule = Id_simple + S_BRACKET_LEFT + ParameterList + S_BRACKET_RIGHT;

            ParameterList.Rule = ParameterList + S_comma + BNF_Expression
                                    | BNF_Expression;

            #endregion

            #endregion

            #region CREATE INDEX

            createIndexStmt.Rule = S_CREATE + S_INDEX + indexNameOpt + editionOpt + S_ON + typeOrVertex + TypeWrapper + S_BRACKET_LEFT + IndexAttributeList + S_BRACKET_RIGHT + BNF_IndexTypeOpt
                | S_CREATE + S_INDEX + indexNameOpt + editionOpt + S_ON + TypeWrapper + S_BRACKET_LEFT + IndexAttributeList + S_BRACKET_RIGHT + BNF_IndexTypeOpt; // due to compatibility the  + S_TYPE is optional

            uniqueOpt.Rule = Empty | S_UNIQUE;

            editionOpt.Rule = Empty
                                | S_EDITION + Id_simple;

            BNF_IndexTypeOpt.Rule = Empty
                                | S_INDEXTYPE + Id_simple;

            indexNameOpt.Rule = Empty
                                | Id_simple;

            #endregion

            #region REBUILD INDICES

            rebuildIndicesStmt.Rule = S_REBUILD + S_INDICES + rebuildIndicesTypes;

            rebuildIndicesTypes.Rule = Empty | TypeList;

            #endregion

            #region CREATE TYPE(S)

            createTypesStmt.Rule = S_CREATE + BNF_TypesOrVertices + bulkTypeList
                                    | S_CREATE + abstractOpt + typeOrVertex + bulkType;

            typeOrVertex.Rule = S_TYPE | S_VERTEX;
            BNF_TypesOrVertices.Rule = S_TYPES | S_VERTICES;

            bulkTypeList.Rule = MakePlusRule(bulkTypeList, S_comma, bulkTypeListMember);

            bulkTypeListMember.Rule = abstractOpt + bulkType;

            bulkType.Rule = Id_simple + extendsOpt + attributesOpt + backwardEdgesOpt + uniquenessOpt + mandatoryOpt + indexOptOnCreateType + commentOpt;

            commentOpt.Rule = Empty
                                    | S_COMMENT + "=" + string_literal;

            abstractOpt.Rule = Empty
                                    | S_ABSTRACT;

            extendsOpt.Rule = Empty
                                    | S_EXTENDS + Id_simple;

            attributesOpt.Rule = Empty
                                    | S_ATTRIBUTES + S_BRACKET_LEFT + AttributeList + S_BRACKET_RIGHT;

            backwardEdgesOpt.Rule = Empty
                                    | S_BACKWARDEDGES + S_BRACKET_LEFT + BackwardEdgesList + S_BRACKET_RIGHT;

            uniquenessOpt.Rule = Empty
                                    | S_UNIQUE + S_BRACKET_LEFT + id_simpleList + S_BRACKET_RIGHT;

            mandatoryOpt.Rule = Empty
                                    | S_MANDATORY + S_BRACKET_LEFT + id_simpleList + S_BRACKET_RIGHT;

            indexOptOnCreateType.Rule = Empty
                                        | indexOnCreateType;

            indexOnCreateType.Rule = S_INDICES + S_BRACKET_LEFT + IndexOptOnCreateTypeMemberList + S_BRACKET_RIGHT
                                    | S_INDICES + IndexOptOnCreateTypeMember;

            IndexOptOnCreateTypeMemberList.Rule = MakePlusRule(IndexOptOnCreateTypeMemberList, S_comma, IndexOptOnCreateTypeMember);

            optionalShards.Rule = S_SHARDS + number | Empty;

            IndexOptOnCreateTypeMember.Rule = S_BRACKET_LEFT + indexNameOpt + editionOpt + BNF_IndexTypeOpt + S_ON + S_ATTRIBUTES + IndexAttributeList + S_BRACKET_RIGHT
                                            | S_BRACKET_LEFT + indexNameOpt + editionOpt + BNF_IndexTypeOpt + S_ON + IndexAttributeList + S_BRACKET_RIGHT // due to compatibility the  + S_ATTRIBUTES is optional
                                            | S_BRACKET_LEFT + IndexAttributeList + S_BRACKET_RIGHT;

            AttrDefaultOpValue.Rule = Empty
                                    | "=" + Value
                                    | "=" + S_LISTOF + S_BRACKET_LEFT + ValueList + S_BRACKET_RIGHT
                                    | "=" + S_SETOF + S_BRACKET_LEFT + ValueList + S_BRACKET_RIGHT;

            #endregion

            #region ALTER TYPE/VERTEX

            alterStmt.Rule = S_ALTER + typeOrVertex + Id_simple + alterCmdList + uniquenessOpt + mandatoryOpt;

            alterCmd.Rule = Empty
                            | S_ADD + S_ATTRIBUTES + S_BRACKET_LEFT + AttributeList + S_BRACKET_RIGHT
                            | S_DROP + S_ATTRIBUTES + S_BRACKET_LEFT + SimpleIdList + S_BRACKET_RIGHT
                            | S_ADD + S_BACKWARDEDGES + S_BRACKET_LEFT + BackwardEdgesList + S_BRACKET_RIGHT
                            | S_DROP + S_BACKWARDEDGES + S_BRACKET_LEFT + SimpleIdList + S_BRACKET_RIGHT
                            | S_ADD + indexOnCreateType
                            | S_DROP + IndexDropOnAlterType
                            | S_RENAME + S_ATTRIBUTE + Id_simple + S_TO + Id_simple
                            | S_RENAME + S_BACKWARDEDGE + Id_simple + S_TO + Id_simple
                            | S_RENAME + S_TO + Id_simple
                            | S_DEFINE + S_ATTRIBUTES + S_BRACKET_LEFT + AttributeList + S_BRACKET_RIGHT
                            | S_UNDEFINE + S_ATTRIBUTES + S_BRACKET_LEFT + SimpleIdList + S_BRACKET_RIGHT
                            | S_DROP + S_UNIQUE
                            | S_DROP + S_MANDATORY
                            | S_COMMENT + "=" + string_literal;

            alterCmdList.Rule = MakePlusRule(alterCmdList, S_comma, alterCmd);

            IndexDropOnAlterTypeMember.Rule = S_BRACKET_LEFT + Id_simple + editionOpt + S_BRACKET_RIGHT;

            IndexDropOnAlterTypeMemberList.Rule = MakePlusRule(IndexDropOnAlterTypeMemberList, S_comma, IndexDropOnAlterTypeMember);

            IndexDropOnAlterType.Rule = S_INDICES + IndexDropOnAlterTypeMember
                                        | S_INDICES + S_BRACKET_LEFT + IndexDropOnAlterTypeMemberList + S_BRACKET_RIGHT;

            #endregion

            #region SELECT

            SelectStmtGraph.Rule = S_FROM + TypeList + S_SELECT + selList + whereClauseOpt + groupClauseOpt + havingClauseOpt + orderClauseOpt + MatchingClause + offsetOpt + limitOpt + resolutionDepthOpt + selectOutputOpt;

            MatchingClause.Rule = Empty
                                    | MatchingClause + Matching;

            Matching.Rule = S_MATCHES + S_BRACKET_LEFT + number + S_BRACKET_RIGHT + PrefixOperation;

            resolutionDepthOpt.Rule = Empty
                                        | S_DEPTH + number;

            selectOutputOpt.Rule = Empty
                                        | "OUTPUT" + name;

            offsetOpt.Rule = Empty
                            | S_OFFSET + number;

            limitOpt.Rule = Empty
                            | S_LIMIT + number;

            selList.Rule = selectionList;

            selectionList.Rule = MakePlusRule(selectionList, S_comma, selectionListElement);

            selectionListElement.Rule = S_ASTERISK
                                        | S_RHOMB
                                        | S_MINUS
                                        | TERMINAL_LT
                                        | TERMINAL_GT
                                        | selByType
                                        | selectionSource;

            selByType.Rule = Empty
                            | S_AD + Id_simple;

            aliasOptName.Rule = Id_simple | string_literal;

            aliasOpt.Rule = Empty
                            | S_AS + aliasOptName;

            var staticSelect = new NonTerminal("staticSelect", CreateSelectValueAssignmentNode);
            staticSelect.Rule = Empty | S_EQUALS + Value | "?=" + Value;

            selectionSource.Rule = BNF_Aggregate + aliasOpt | IdOrFuncList + staticSelect + aliasOpt;
            //|   funcCall
            //|   Id;


            #region Aggregate

            //BNF_Aggregate.Rule = BNF_AggregateName + S_BRACKET_LEFT + name + S_BRACKET_RIGHT;

            BNF_AggregateArg.Rule = Id
                                    | S_ASTERISK;
            /*
            aggregateName.Rule =        S_COUNT 
                                    |   "AVG" 
                                    |   "MIN" 
                                    |   "MAX" 
                                    |   "STDEV" 
                                    |   "STDEVP" 
                                    |   "SUM" 
                                    |   "VAR" 
                                    |   "VARP";
            */
            #endregion

            #region Functions

            //function.Rule           = functionName + S_BRACKET_LEFT + term + S_BRACKET_RIGHT;

            //functionName.Rule       = FUNC_WEIGHT;

            #endregion

            whereClauseOpt.Rule = Empty
                                    | S_WHERE + BNF_Expression;

            groupClauseOpt.Rule = Empty
                                    | "GROUP" + S_BY + idlist;

            havingClauseOpt.Rule = Empty
                                    | "HAVING" + BNF_Expression;


            orderByAttributeListMember.Rule = Id
                                                | string_literal;

            orderByAttributeList.Rule = MakePlusRule(orderByAttributeList, S_comma, orderByAttributeListMember);

            orderClauseOpt.Rule = Empty
                                    | "ORDER" + S_BY + orderByAttributeList + AttributeOrderDirectionOpt;

            #endregion

            #region INSERT

            InsertStmt.Rule = S_INSERT + S_INTO + TypeWrapper + insertValuesOpt;

            insertValuesOpt.Rule = Empty
                                    | S_VALUES + S_BRACKET_LEFT + AttrAssignList + S_BRACKET_RIGHT;

            AttrAssignList.Rule = MakePlusRule(AttrAssignList, S_comma, AttrAssign);

            AttrAssign.Rule = Id + "=" + BNF_Expression
                                | Id + "=" + Reference
                                | Id + "=" + CollectionOfDBObjects;

            CollectionOfDBObjects.Rule = S_SETOF + CollectionTuple
                                            | S_LISTOF + CollectionTuple
                                            | S_SETOFUUIDS + CollectionTuple
                                            | S_SETOFUUIDS + "()"
                                            | S_SETOF + "()";

            CollectionTuple.Rule = S_BRACKET_LEFT + ExtendedExpressionList + S_BRACKET_RIGHT;

            ExtendedExpressionList.Rule = MakePlusRule(ExtendedExpressionList, S_comma, ExtendedExpression);

            ExtendedExpression.Rule = BNF_Expression + ListParametersForExpression;

            Reference.Rule = S_REFERENCE + tuple + ListParametersForExpression
                           | S_REF + tuple + ListParametersForExpression
                           | S_REFUUID + tuple + ListParametersForExpression
                           | S_REFERENCEUUID + tuple + ListParametersForExpression;

            //| S_SETREF + tupleRangeSet + ListParametersForExpression;

            #endregion

            #region UPDATE

            updateStmt.Rule = S_UPDATE + TypeWrapper + S_SET + S_BRACKET_LEFT + AttrUpdateList + S_BRACKET_RIGHT + whereClauseOpt;

            AttrUpdateList.Rule = MakePlusRule(AttrUpdateList, S_comma, AttrUpdateOrAssign);

            AttrUpdateOrAssign.Rule = AttrAssign
                                        | AttrRemove
                                        | ListAttrUpdate;

            AttrRemove.Rule = S_REMOVE + S_ATTRIBUTES + S_BRACKET_LEFT + id_simpleList + S_BRACKET_RIGHT;

            ListAttrUpdate.Rule = AddToListAttrUpdate
                                    | RemoveFromListAttrUpdate;

            AddToListAttrUpdate.Rule = AddToListAttrUpdateAddTo
                                        | AddToListAttrUpdateOperator;

            AddToListAttrUpdateAddTo.Rule = S_ADD + S_TO + Id + CollectionOfDBObjects;
            AddToListAttrUpdateOperator.Rule = Id + S_ADDTOLIST + CollectionOfDBObjects;

            RemoveFromListAttrUpdate.Rule = RemoveFromListAttrUpdateAddToRemoveFrom
                                            | RemoveFromListAttrUpdateAddToOperator;

            RemoveFromListAttrUpdateAddToRemoveFrom.Rule = S_REMOVE + S_FROM + Id + tuple;
            RemoveFromListAttrUpdateAddToOperator.Rule = Id + RemoveFromListAttrUpdateScope;
            RemoveFromListAttrUpdateScope.Rule = S_REMOVEFROMLIST + tuple | S_REMOVEFROMLIST + CollectionOfDBObjects;

            #endregion

            #region DROP TYPE

            dropTypeStmt.Rule = S_DROP + typeOrVertex + Id_simple;

            #endregion

            #region DROP INDEX

            dropIndexStmt.Rule = S_FROM + TypeWrapper + S_DROP + S_INDEX + Id_simple + editionOpt;

            #endregion

            #region TRUNCATE

            truncateStmt.Rule = S_TRUNCATE + typeOrVertex + Id_simple
                              | S_TRUNCATE + Id_simple; // Due to compatibility the  + S_TYPE is optional

            #endregion

            #region DELETE

            deleteStmtMember.Rule = Empty | idlist;
            deleteStmt.Rule = S_FROM + TypeList + S_DELETE + deleteStmtMember + whereClauseOpt;

            #endregion

            #region SETTING

            SettingsStatement.Rule = S_SETTING + SettingScope + SettingOperation;

            SettingScope.Rule = S_DB | S_SESSION | SettingTypeNode | SettingAttrNode;

            SettingTypeNode.Rule = typeOrVertex + SettingTypeStmLst;

            SettingTypeStmLst.Rule = MakePlusRule(SettingTypeStmLst, S_comma, TypeWrapper);

            SettingAttrNode.Rule = S_ATTRIBUTE + SettingAttrStmLst;

            SettingAttrStmLst.Rule = MakePlusRule(SettingAttrStmLst, S_comma, id_typeAndAttribute);

            SettingOperation.Rule = SettingOpSet | SettingOpGet | SettingOpRemove;

            SettingOpSet.Rule = S_SET + S_BRACKET_LEFT + SettingItemSetLst + S_BRACKET_RIGHT;

            SettingItemsSet.Rule = string_literal + "=" + SettingItemSetVal;

            SettingItemSetLst.Rule = MakePlusRule(SettingItemSetLst, S_comma, SettingItemsSet);

            SettingItemSetVal.Rule = number
                                        | S_DEFAULT
                                        | string_literal;

            SettingOpGet.Rule = S_GET + S_BRACKET_LEFT + SettingItems + S_BRACKET_RIGHT;

            SettingOpRemove.Rule = S_REMOVE + S_BRACKET_LEFT + SettingItems + S_BRACKET_RIGHT;

            SettingItems.Rule = MakePlusRule(SettingItems, S_comma, string_literal);

            #endregion

            #region DESCRIBE

            DescrInfoStmt.Rule = S_DESCRIBE + DescrArgument;

            DescrArgument.Rule = DescrAggrStmt | DescrAggrsStmt | DescrEdgeStmt | DescrEdgesStmt | DescrTypeStmt | DescrTypesStmt | DescrFuncStmt | DescrFunctionsStmt | DescrSettStmt | DescrSettingsStmt | DescrIdxStmt | DescrIdxsStmt;

            DescrAggrStmt.Rule = S_AGGREGATE + Id_simple;

            DescrAggrsStmt.Rule = S_AGGREGATES;

            DescrEdgeStmt.Rule = S_EDGE + Id_simple;

            DescrEdgesStmt.Rule = S_EDGES;

            DescrTypeStmt.Rule = typeOrVertex + Id_simple;

            DescrTypesStmt.Rule = S_TYPES;

            DescrFuncStmt.Rule = S_FUNCTION + Id_simple;

            DescrFunctionsStmt.Rule = S_FUNCTIONS;

            DescrSettStmt.Rule = S_SETTING + DescrSettItem | S_SETTINGS + DescrSettingsItems;

            DescrSettItem.Rule = Id_simple + Empty | Id_simple + S_ON + typeOrVertex + AType | Id_simple + S_ON + S_ATTRIBUTE + id_typeAndAttribute | Id_simple + S_ON + S_DB | Id_simple + S_ON + S_SESSION;

            DescrSettingsItems.Rule = S_ON + typeOrVertex + TypeList | S_ON + S_ATTRIBUTE + id_typeAndAttribute | S_ON + S_DB | S_ON + S_SESSION;

            DescrSettingsStmt.Rule = S_SETTINGS;

            DescrIdxStmt.Rule = S_INDEX + id_simpleDotList + DescrIdxEdtStmt;

            DescrIdxEdtStmt.Rule = Empty | Id_simple;

            DescrIdxsStmt.Rule = S_INDICES;

            #endregion

            #region INSERTORUPDATE

            insertorupdateStmt.Rule = S_INSERTORUPDATE + TypeWrapper + S_VALUES + S_BRACKET_LEFT + AttrUpdateList + S_BRACKET_RIGHT + whereClauseOpt;

            #endregion

            #region INSERTORREPLACE

            insertorreplaceStmt.Rule = S_INSERTORREPLACE + TypeWrapper + S_VALUES + S_BRACKET_LEFT + AttrAssignList + S_BRACKET_RIGHT + whereClauseOpt;

            #endregion

            #region REPLACE

            replaceStmt.Rule = S_REPLACE + TypeWrapper + S_VALUES + S_BRACKET_LEFT + AttrAssignList + S_BRACKET_RIGHT + S_WHERE + BNF_Expression;

            #endregion

            #region TRANSACTION


            #region BeginTransAction

            transactStmt.Rule = S_BEGIN + TransactOptions + S_TRANSACTION + TransactAttributes;

            TransactOptions.Rule = Empty |
                                S_TRANSACTDISTRIBUTED + S_TRANSACTLONGRUNNING |
                                S_TRANSACTDISTRIBUTED |
                                S_TRANSACTLONGRUNNING;

            TransactAttributes.Rule = Empty |
                                TransactIsolation |
                                TransactName |
                                TransactTimestamp |
                                TransactIsolation + TransactName |
                                TransactIsolation + TransactTimestamp |
                                TransactName + TransactTimestamp |
                                TransactIsolation + TransactName + TransactTimestamp;

            TransactIsolation.Rule = S_TRANSACTISOLATION + "=" + string_literal;

            TransactName.Rule = S_TRANSACTNAME + "=" + string_literal;

            TransactTimestamp.Rule = S_TRANSACTTIMESTAMP + "=" + string_literal;

            #endregion

            #region CommitRollbackTransAction

            commitRollBackTransactStmt.Rule = TransactCommitRollbackType + S_TRANSACTION + TransactCommitRollbackOpt;

            TransactCommitRollbackType.Rule = S_TRANSACTCOMMIT | S_TRANSACTROLLBACK;

            TransactCommitRollbackOpt.Rule = Empty |
                                        TransactName |
                                        S_TRANSACTCOMROLLASYNC |
                                        TransactName + S_TRANSACTCOMROLLASYNC;

            #endregion

            #endregion

            #region EXPORT/DUMP

            dumpType.Rule = Empty | S_ALL | S_GDDL | S_GDML;      // If empty => create both
            dumpFormat.Rule = Empty | S_AS + S_GQL;                 // If empty => create GQL
            typeOptionalList.Rule = Empty | BNF_TypesOrVertices + TypeList;

            dumpDestination.Rule = Empty | S_INTO + location_literal | S_TO + location_literal;

            dumpStmt.Rule = S_DUMP + typeOptionalList + dumpType + dumpFormat + dumpDestination
                                    | S_EXPORT + typeOptionalList + dumpType + dumpFormat + dumpDestination;

            #endregion

            #region IMPORT

            paramComments.Rule = S_COMMENTS + tuple | Empty;
            paramParallelTasks.Rule = S_PARALLELTASKS + "(" + number + ")" | Empty;
            verbosityTypes.Rule = ToTerm(VerbosityTypes.Silent.ToString()) | ToTerm(VerbosityTypes.Errors.ToString()) | ToTerm(VerbosityTypes.Full.ToString());
            verbosity.Rule = S_VERBOSITY + verbosityTypes | Empty;

            //BNF_ImportFormat.Rule = Empty;

            BNF_ImportStmt.Rule = S_IMPORT + S_FROM + location_literal + S_FORMAT + BNF_ImportFormat + paramParallelTasks + paramComments + offsetOpt + limitOpt + verbosity;

            #endregion

            #region LINK

            // Semantic Web Yoda-Style and human language style
            linkStmt.Rule = S_LINK + TypeWrapper + CollectionTuple + S_VIA + Id + S_TO + LinkCondition |
                            S_LINK + TypeWrapper + CollectionTuple + S_TO + LinkCondition + S_VIA + Id;

            LinkCondition.Rule = TypeWrapper + CollectionTuple;

            #endregion

            #region UNLINK

            unlinkStmt.Rule = S_UNLINK + TypeWrapper + CollectionTuple + S_VIA + Id + S_FROM + LinkCondition |
                              S_UNLINK + TypeWrapper + CollectionTuple + S_FROM + LinkCondition + S_VIA + Id;

            #endregion

            #endregion

            #region Misc

            #region punctuation

            MarkPunctuation(",", S_BRACKET_LEFT.Symbol.Text, S_BRACKET_RIGHT.Symbol.Text, "[", "]");
            //RegisterPunctuation(",", S_BRACKET_LEFT.Symbol, S_BRACKET_RIGHT.Symbol, S_TUPLE_BRACKET_LEFT.Symbol, S_TUPLE_BRACKET_RIGHT.Symbol);
            //RegisterPunctuation(",");
            //RegisterBracePair(_S_BRACKET_LEFT.Symbol, S_BRACKET_RIGHT.Symbol);
            //RegisterBracePair(_S_TUPLE_BRACKET_LEFT.Symbol, S_TUPLE_BRACKET_RIGHT.Symbol);
            //RegisterBracePair(_S_TUPLE_BRACKET_LEFT_EXCLUSIVE.Symbol, S_TUPLE_BRACKET_RIGHT.Symbol);
            //RegisterBracePair(_S_TUPLE_BRACKET_LEFT.Symbol, S_TUPLE_BRACKET_RIGHT_EXCLUSIVE.Symbol);
            //RegisterBracePair(_S_TUPLE_BRACKET_LEFT_EXCLUSIVE.Symbol, S_TUPLE_BRACKET_RIGHT_EXCLUSIVE.Symbol);

            #endregion

            base.MarkTransient(
                singlestmt, Id_simple, selList, selectionSource, BNF_Expression, term, BNF_FunArgs
                , unOp, binOp, aliasOpt, aliasOptName, orderByAttributeListMember
                , Value
                //, EdgeTypeParam
                , EdgeType_SortedMember, AttrUpdateOrAssign, ListAttrUpdate, SettingItemSetVal, DescrArgument,
                TypeWrapper //is used as a wrapper for AType
                , IdOrFunc //, IdOrFuncList
                , BNF_ExprList, BNF_AggregateArg,
                ExtendedExpressionList,
                BNF_ImportFormat, BNF_FuncCall, BNF_Aggregate, verbosityTypes,
                typeOrVertex, BNF_TypesOrVertices);

            #endregion

        }

        #endregion

        #region Node Delegates

        private void CreateTypeList(ParsingContext context, ParseTreeNode parseNode)
        {
            var node = new TypeListNode();

            node.Init(context, parseNode);

            parseNode.AstNode = node;
        }

        private void CreateIndexOnCreateType(ParsingContext context, ParseTreeNode parseNode)
        {
            var Node = new IndexOnCreateTypeNode();

            Node.Init(context, parseNode);

            parseNode.AstNode = Node;
        }

        private void CreateDropIndicesNode(ParsingContext context, ParseTreeNode parseNode)
        {
            var Node = new IndexDropOnAlterType();

            Node.Init(context, parseNode);

            parseNode.AstNode = Node;
        }

        private void CreateIndexOptOnCreateTypeMemberNode(ParsingContext context, ParseTreeNode parseNode)
        {
            var node = new IndexOptOnCreateTypeMemberNode();

            node.Init(context, parseNode);

            parseNode.AstNode = node;
        }

        private void CreateAttrAssignListNode(ParsingContext context, ParseTreeNode parseNode)
        {
            var attrAssignListNode = new AttrAssignListNode();

            attrAssignListNode.Init(context, parseNode);

            parseNode.AstNode = attrAssignListNode;
        }

        private void CreateUnExpressionNode(ParsingContext context, ParseTreeNode parseNode)
        {
            var aUnExpressionNode = new UnaryExpressionNode();

            aUnExpressionNode.Init(context, parseNode);

            parseNode.AstNode = aUnExpressionNode;
        }

        private void CreateGraphDBTypeNode(ParsingContext context, ParseTreeNode parseNode)
        {
            GraphDBTypeNode aGraphTypeNode = new GraphDBTypeNode();

            aGraphTypeNode.Init(context, parseNode);

            parseNode.AstNode = aGraphTypeNode;
        }

        private void CreateBulkTypeListMemberNode(ParsingContext context, ParseTreeNode parseNode)
        {
            BulkTypeListMemberNode aBulkTypeListMemberNode = new BulkTypeListMemberNode();

            aBulkTypeListMemberNode.Init(context, parseNode);

            parseNode.AstNode = aBulkTypeListMemberNode;
        }

        private void CreateBulkTypeNode(ParsingContext context, ParseTreeNode parseNode)
        {
            BulkTypeNode aBulkTypeNode = new BulkTypeNode();

            aBulkTypeNode.Init(context, parseNode);

            parseNode.AstNode = aBulkTypeNode;
        }

        private void CreateReplaceStatementNode(ParsingContext context, ParseTreeNode parseNode)
        {
            ReplaceNode aReplaceNode = new ReplaceNode();

            aReplaceNode.Init(context, parseNode);

            parseNode.AstNode = aReplaceNode;
        }

        private void CreateInsertOrReplaceStatementNode(ParsingContext context, ParseTreeNode parseNode)
        {
            InsertOrReplaceNode aInsertOrReplaceNode = new InsertOrReplaceNode();

            aInsertOrReplaceNode.Init(context, parseNode);

            parseNode.AstNode = aInsertOrReplaceNode;
        }

        private void CreateInsertOrUpdateStatementNode(ParsingContext context, ParseTreeNode parseNode)
        {
            InsertOrUpdateNode aInsertOrUpdateNode = new InsertOrUpdateNode();

            aInsertOrUpdateNode.Init(context, parseNode);

            parseNode.AstNode = aInsertOrUpdateNode;
        }

        private void CreateCreateTypesStatementNode(ParsingContext context, ParseTreeNode parseNode)
        {

            CreateTypesNode aCreateTypesNode = new CreateTypesNode();

            aCreateTypesNode.Init(context, parseNode);

            parseNode.AstNode = aCreateTypesNode;
        }

        private void CreateAttributeDefinitionNode(ParsingContext context, ParseTreeNode parseNode)
        {
            AttributeDefinitionNode aCreateAttributeNode = new AttributeDefinitionNode();

            aCreateAttributeNode.Init(context, parseNode);

            parseNode.AstNode = aCreateAttributeNode;
        }

        private void CreateInsertStatementNode(ParsingContext context, ParseTreeNode parseNode)
        {

            InsertNode aInsertNode = new InsertNode();

            aInsertNode.Init(context, parseNode);

            parseNode.AstNode = aInsertNode;
        }

        private void CreateCreateIndexStatementNode(ParsingContext context, ParseTreeNode parseNode)
        {

            CreateIndexNode aIndexNode = new CreateIndexNode();

            aIndexNode.Init(context, parseNode);

            parseNode.AstNode = aIndexNode;

        }

        private void CreateIDNode(ParsingContext context, ParseTreeNode parseNode)
        {
            IDNode aIDNode = new IDNode();

            aIDNode.Init(context, parseNode);

            parseNode.AstNode = aIDNode;

        }

        private void CreateDotDelimiter(ParsingContext context, ParseTreeNode parseNode)
        {
            SelectionDelimiterNode aDelimitter = new SelectionDelimiterNode();

            aDelimitter.Init(context, parseNode);

            aDelimitter.SetDelimiter(KindOfDelimiter.Dot);

            parseNode.AstNode = aDelimitter;
        }

        private void CreateEdgeInformation(ParsingContext context, ParseTreeNode parseNode)
        {
            EdgeInformationNode aEdgeInformation = new EdgeInformationNode();

            aEdgeInformation.Init(context, parseNode);

            parseNode.AstNode = aEdgeInformation;
        }

        private void CreateEdgeTraversal(ParsingContext context, ParseTreeNode parseNode)
        {
            EdgeTraversalNode aEdgeTraversal = new EdgeTraversalNode();

            aEdgeTraversal.Init(context, parseNode);

            parseNode.AstNode = aEdgeTraversal;
        }

        private void CreateEdgeAccessorDelimiter(ParsingContext context, ParseTreeNode parseNode)
        {
            SelectionDelimiterNode aDelimitter = new SelectionDelimiterNode();

            aDelimitter.Init(context, parseNode);

            aDelimitter.SetDelimiter(KindOfDelimiter.EdgeInformationDelimiter);

            parseNode.AstNode = aDelimitter;
        }

        private void CreateUpdateStatementNode(ParsingContext context, ParseTreeNode parseNode)
        {

            UpdateNode aUpdateNode = new UpdateNode();

            aUpdateNode.Init(context, parseNode);

            parseNode.AstNode = aUpdateNode;

        }

        private void CreateBinaryExpressionNode(ParsingContext context, ParseTreeNode parseNode)
        {
            BinaryExpressionNode aNode = new BinaryExpressionNode();

            aNode.Init(context, parseNode);

            parseNode.AstNode = aNode;

        }


        private void CreateWhereExpressionNode(ParsingContext context, ParseTreeNode parseNode)
        {
            WhereExpressionNode aWhereNode = new WhereExpressionNode();

            aWhereNode.Init(context, parseNode);

            parseNode.AstNode = aWhereNode;
        }

        private void CreateSelectStatementNode(ParsingContext context, ParseTreeNode parseNode)
        {
            var aSelectStatementNode = new SelectNode();

            aSelectStatementNode.Init(context, parseNode);

            parseNode.AstNode = aSelectStatementNode;
        }

        private void CreateAliasNode(ParsingContext context, ParseTreeNode parseNode)
        {
            var aliasNode = new AliasNode();

            aliasNode.Init(context, parseNode);

            parseNode.AstNode = aliasNode;
        }

        private void CreateSelectValueAssignmentNode(ParsingContext context, ParseTreeNode parseNode)
        {
            var selectValueAssignmentNode = new SelectValueAssignmentNode();

            selectValueAssignmentNode.Init(context, parseNode);

            parseNode.AstNode = selectValueAssignmentNode;
        }

        private void CreateSelByTypeNode(ParsingContext context, ParseTreeNode parseNode)
        {
            var aSelByTypeNode = new SelByTypeNode();

            aSelByTypeNode.Init(context, parseNode);

            parseNode.AstNode = aSelByTypeNode;
        }

        private void CreateATypeNode(ParsingContext context, ParseTreeNode parseNode)
        {
            ATypeNode aATypeNode = new ATypeNode();

            aATypeNode.Init(context, parseNode);

            parseNode.AstNode = aATypeNode;
        }

        private void CreateAlterStmNode(ParsingContext context, ParseTreeNode parseNode)
        {
            AlterTypeNode aAlterTypeStatementNode = new AlterTypeNode();

            aAlterTypeStatementNode.Init(context, parseNode);

            parseNode.AstNode = aAlterTypeStatementNode;
        }

        private void CreatePartialSelectStmtNode(ParsingContext context, ParseTreeNode parseNode)
        {
            PartialSelectStmtNode aPartialSelectStmtNode = new PartialSelectStmtNode();

            aPartialSelectStmtNode.Init(context, parseNode);

            parseNode.AstNode = aPartialSelectStmtNode;
        }

        private void CreateAggregateNode(ParsingContext context, ParseTreeNode parseNode)
        {
            AggregateNode aAggregateNode = new AggregateNode();

            aAggregateNode.Init(context, parseNode);

            parseNode.AstNode = aAggregateNode;
        }

        private void CreateTypeListNode(ParsingContext context, ParseTreeNode parseNode)
        {
            TypeListNode aNode = new TypeListNode();

            aNode.Init(context, parseNode);

            parseNode.AstNode = aNode;

        }

        #region Functions

        private void CreateFunctionCallNode(ParsingContext context, ParseTreeNode parseNode)
        {
            FuncCallNode functionCallNode = new FuncCallNode();

            functionCallNode.Init(context, parseNode);

            parseNode.AstNode = functionCallNode;
        }

        #endregion

        private void CreateDropTypeStmNode(ParsingContext context, ParseTreeNode parseNode)
        {
            DropTypeNode dropTypeNode = new DropTypeNode();

            dropTypeNode.Init(context, parseNode);

            parseNode.AstNode = dropTypeNode;
        }

        private void CreateDropIndexStmNode(ParsingContext context, ParseTreeNode parseNode)
        {
            DropIndexNode dropIndexNode = new DropIndexNode();

            dropIndexNode.Init(context, parseNode);

            parseNode.AstNode = dropIndexNode;
        }

        private void CreateTruncateStmNode(ParsingContext context, ParseTreeNode parseNode)
        {
            TruncateNode truncateNode = new TruncateNode();

            truncateNode.Init(context, parseNode);

            parseNode.AstNode = truncateNode;
        }

        private void CreateDeleteStatementNode(ParsingContext context, ParseTreeNode parseNode)
        {
            DeleteNode deleteNode = new DeleteNode();

            deleteNode.Init(context, parseNode);

            parseNode.AstNode = deleteNode;
        }

        #region Settings

        private void CreateSettingStatementNode(ParsingContext context, ParseTreeNode parseNode)
        {
            SettingNode settingNode = new SettingNode();

            settingNode.Init(context, parseNode);

            parseNode.AstNode = settingNode;
        }

        private void CreateSettingTypeNode(ParsingContext context, ParseTreeNode parseNode)
        {
            SettingTypeNode settingTypeNode = new SettingTypeNode();

            settingTypeNode.Init(context, parseNode);

            parseNode.AstNode = settingTypeNode;
        }

        private void CreateSettingAttrNode(ParsingContext context, ParseTreeNode parseNode)
        {
            SettingAttrNode settingAttrNode = new SettingAttrNode();

            settingAttrNode.Init(context, parseNode);

            parseNode.AstNode = settingAttrNode;
        }

        #endregion

        private void CreateBackwardEdgesNode(ParsingContext context, ParseTreeNode parseNode)
        {
            BackwardEdgesNode backwardEdgesNode = new BackwardEdgesNode();

            backwardEdgesNode.Init(context, parseNode);

            parseNode.AstNode = backwardEdgesNode;
        }

        private void CreateBackwardEdgeNode(ParsingContext context, ParseTreeNode parseNode)
        {
            BackwardEdgeNode backwardEdgeNode = new BackwardEdgeNode();

            backwardEdgeNode.Init(context, parseNode);

            parseNode.AstNode = backwardEdgeNode;
        }

        private void CreateEdgeTypeDefNode(ParsingContext context, ParseTreeNode parseNode)
        {
            EdgeTypeDefNode edgeTypeDefNode = new EdgeTypeDefNode();

            edgeTypeDefNode.Init(context, parseNode);

            parseNode.AstNode = edgeTypeDefNode;
        }

        private void CreateSingleEdgeTypeDefNode(ParsingContext context, ParseTreeNode parseNode)
        {
            SingleEdgeTypeDefNode edgeTypeDefNode = new SingleEdgeTypeDefNode();

            edgeTypeDefNode.Init(context, parseNode);

            parseNode.AstNode = edgeTypeDefNode;
        }

        #region Dump/Export

        private void CreateDumpNode(ParsingContext context, ParseTreeNode parseNode)
        {
            var dumpNode = new DumpNode();

            dumpNode.Init(context, parseNode);

            parseNode.AstNode = dumpNode;
        }

        private void CreateDumpTypeNode(ParsingContext context, ParseTreeNode parseNode)
        {
            var dumpTypeNode = new DumpTypeNode();

            dumpTypeNode.Init(context, parseNode);

            parseNode.AstNode = dumpTypeNode;
        }

        private void CreateDumpFormatNode(ParsingContext context, ParseTreeNode parseNode)
        {
            var dumpFormatNode = new DumpFormatNode();

            dumpFormatNode.Init(context, parseNode);

            parseNode.AstNode = dumpFormatNode;
        }

        #endregion

        private void CreateAddToListAttrUpdateAddToNode(ParsingContext context, ParseTreeNode parseNode)
        {
            var addToListAttrUpdateAddToNode = new AddToListAttrUpdateAddToNode();

            addToListAttrUpdateAddToNode.Init(context, parseNode);

            parseNode.AstNode = addToListAttrUpdateAddToNode;
        }

        private void CreateAddToListAttrUpdateOperatorNode(ParsingContext context, ParseTreeNode parseNode)
        {
            var addToListAttrUpdateOperatorNode = new AddToListAttrUpdateOperatorNode();

            addToListAttrUpdateOperatorNode.Init(context, parseNode);

            parseNode.AstNode = addToListAttrUpdateOperatorNode;
        }

        private void CreateRemoveFromListAttrUpdateAddToRemoveFromNode(ParsingContext context, ParseTreeNode parseNode)
        {
            var removeFromListAttrUpdateAddToRemoveFromNode = new RemoveFromListAttrUpdateAddToRemoveFromNode();

            removeFromListAttrUpdateAddToRemoveFromNode.Init(context, parseNode);

            parseNode.AstNode = removeFromListAttrUpdateAddToRemoveFromNode;
        }

        private void CreateRemoveFromListAttrUpdateAddToOperatorNode(ParsingContext context, ParseTreeNode parseNode)
        {
            var removeFromListAttrUpdateAddToOperatorNode = new RemoveFromListAttrUpdateAddToOperatorNode();

            removeFromListAttrUpdateAddToOperatorNode.Init(context, parseNode);

            parseNode.AstNode = removeFromListAttrUpdateAddToOperatorNode;
        }


        private void CreateAttrDefaultValueNode(ParsingContext context, ParseTreeNode parseNode)
        {
            var attrDefaultValueNode = new AttrDefaultValueNode();

            attrDefaultValueNode.Init(context, parseNode);

            parseNode.AstNode = attrDefaultValueNode;
        }

        private void CreateRemoveFromListAttrUpdateScope(ParsingContext context, ParseTreeNode parseNode)
        {
            var removeFromListAttrUpdateScopeNode = new RemoveFromListAttrUpdateScopeNode();

            removeFromListAttrUpdateScopeNode.Init(context, parseNode);

            parseNode.AstNode = removeFromListAttrUpdateScopeNode;
        }

        #region RebuildIndices

        private void CreateRebuildIndicesNode(ParsingContext context, ParseTreeNode parseNode)
        {
            var rebuildIndicesNode = new RebuildIndicesNode();

            rebuildIndicesNode.Init(context, parseNode);

            parseNode.AstNode = rebuildIndicesNode;
        }

        #endregion

        #region Transaction

        private void CreateTransActionStatementNode(ParsingContext context, ParseTreeNode parseNode)
        {
            BeginTransactionNode beginTransactNode = new BeginTransactionNode();

            beginTransactNode.Init(context, parseNode);

            parseNode.AstNode = beginTransactNode;
        }

        private void CreateCommitRollbackTransActionNode(ParsingContext context, ParseTreeNode parseNode)
        {
            CommitRollbackTransactionNode commitRollBackNode = new CommitRollbackTransactionNode();

            commitRollBackNode.Init(context, parseNode);

            parseNode.AstNode = commitRollBackNode;
        }

        #endregion

        #region Describe

        private void CreateDescribeNode(ParsingContext context, ParseTreeNode parseNode)
        {
            DescribeNode DescrNode = new DescribeNode();

            DescrNode.Init(context, parseNode);

            parseNode.AstNode = DescrNode;
        }

        private void CreateDescrFunc(ParsingContext context, ParseTreeNode parseNode)
        {
            DescrFuncNode funcInfoNode = new DescrFuncNode();

            funcInfoNode.Init(context, parseNode);

            parseNode.AstNode = funcInfoNode;
        }

        private void CreateDescrFunctions(ParsingContext context, ParseTreeNode parseNode)
        {
            DescrFunctionsNode funcInfoNode = new DescrFunctionsNode();

            funcInfoNode.Init(context, parseNode);

            parseNode.AstNode = funcInfoNode;
        }

        private void CreateDescrAggr(ParsingContext context, ParseTreeNode parseNode)
        {
            DescrAggrNode aggrInfoNode = new DescrAggrNode();

            aggrInfoNode.Init(context, parseNode);

            parseNode.AstNode = aggrInfoNode;
        }

        private void CreateDescrAggrs(ParsingContext context, ParseTreeNode parseNode)
        {
            DescrAggrsNode aggrInfoNode = new DescrAggrsNode();

            aggrInfoNode.Init(context, parseNode);

            parseNode.AstNode = aggrInfoNode;
        }

        private void CreateDescrSett(ParsingContext context, ParseTreeNode parseNode)
        {
            DescribeSettingNode settInfoNode = new DescribeSettingNode();

            settInfoNode.Init(context, parseNode);

            parseNode.AstNode = settInfoNode;
        }

        private void CreateDescrType(ParsingContext context, ParseTreeNode parseNode)
        {
            DescribeTypeNode typeInfoNode = new DescribeTypeNode();

            typeInfoNode.Init(context, parseNode);

            parseNode.AstNode = typeInfoNode;
        }

        private void CreateDescrTypes(ParsingContext context, ParseTreeNode parseNode)
        {
            DescribeTypesNode typeInfoNode = new DescribeTypesNode();

            typeInfoNode.Init(context, parseNode);

            parseNode.AstNode = typeInfoNode;
        }

        private void CreateDescrIdx(ParsingContext context, ParseTreeNode parseNode)
        {
            DescribeIndexNode idxInfoNode = new DescribeIndexNode();

            idxInfoNode.Init(context, parseNode);

            parseNode.AstNode = idxInfoNode;
        }

        private void CreateDescrIdxs(ParsingContext context, ParseTreeNode parseNode)
        {
            DescribeIndicesNode idxInfoNode = new DescribeIndicesNode();

            idxInfoNode.Init(context, parseNode);

            parseNode.AstNode = idxInfoNode;
        }

        private void CreateDescrEdge(ParsingContext context, ParseTreeNode parseNode)
        {
            DescribeEdgeNode edgeInfoNode = new DescribeEdgeNode();

            edgeInfoNode.Init(context, parseNode);

            parseNode.AstNode = edgeInfoNode;
        }

        private void CreateDescrEdges(ParsingContext context, ParseTreeNode parseNode)
        {
            DescribeEdgesNode edgeInfoNode = new DescribeEdgesNode();

            edgeInfoNode.Init(context, parseNode);

            parseNode.AstNode = edgeInfoNode;
        }

        private void CreateDescrSettItem(ParsingContext context, ParseTreeNode parseNode)
        {
            DescribeSettItemNode setItemInfoNode = new DescribeSettItemNode();

            setItemInfoNode.Init(context, parseNode);

            parseNode.AstNode = setItemInfoNode;
        }

        private void CreateDescrSettingsItems(ParsingContext context, ParseTreeNode parseNode)
        {
            DescribeSettingsItemsNode setItemInfoNode = new DescribeSettingsItemsNode();

            setItemInfoNode.Init(context, parseNode);

            parseNode.AstNode = setItemInfoNode;
        }

        private void CreateDescrSettings(ParsingContext context, ParseTreeNode parseNode)
        {
            DescribeSettingsNode setItemInfoNode = new DescribeSettingsNode();

            setItemInfoNode.Init(context, parseNode);

            parseNode.AstNode = setItemInfoNode;
        }
        #endregion

        #region Import

        private void CreateImportNode(ParsingContext context, ParseTreeNode parseNode)
        {
            var importNode = new ImportNode();

            importNode.Init(context, parseNode);

            parseNode.AstNode = importNode;
        }

        private void CreateParallelTaskNode(ParsingContext context, ParseTreeNode parseNode)
        {
            var parallelTaskNode = new ParallelTasksNode();

            parallelTaskNode.Init(context, parseNode);

            parseNode.AstNode = parallelTaskNode;
        }

        private void CreateCommentsNode(ParsingContext context, ParseTreeNode parseNode)
        {
            var commentsNode = new CommentsNode();

            commentsNode.Init(context, parseNode);

            parseNode.AstNode = commentsNode;
        }

        private void CreateVerbosityNode(ParsingContext context, ParseTreeNode parseNode)
        {
            var verbosityNode = new VerbosityNode();

            verbosityNode.Init(context, parseNode);

            parseNode.AstNode = verbosityNode;
        }

        #endregion

        #region link

        private void CreateLinkStmtNode(ParsingContext context, ParseTreeNode parseNode)
        {
            var linkNode = new LinkNode();

            linkNode.Init(context, parseNode);

            parseNode.AstNode = linkNode;
        }

        #endregion

        #region unlink

        private void CreateUnlinkStmt(ParsingContext context, ParseTreeNode parseNode)
        {
            var unlinkNode = new UnlinkNode();

            unlinkNode.Init(context, parseNode);

            parseNode.AstNode = unlinkNode;
        }

        #endregion

        #endregion

        #region IDumpable Members

        #region Export GraphDDL

        public List<String> ExportGraphDDL(IEnumerable<IVertexType> myTypesToDump)
        {

            var stringBuilder = new StringBuilder(String.Concat(S_CREATE.ToUpperString(), " ", S_TYPES.ToUpperString(), " "));
            var delimiter = ", ";

            foreach (var vertexType in myTypesToDump)
            {
                stringBuilder.Append(String.Concat(CreateGraphDDL(vertexType), delimiter));
            }

            var retString = stringBuilder.ToString();

            if (retString.EndsWith(delimiter))
            {
                retString = retString.Substring(0, retString.Length - delimiter.Length);
            }

            return new List<String> { retString };

        }

        #region private helper

        private String CreateGraphDDL(IVertexType myVertexType)
        {

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("{0} ", myVertexType.Name);

            #region parent type

            if (myVertexType.HasParentVertexType)
            {
                stringBuilder.AppendFormat("{0} {1} ", S_EXTENDS.ToUpperString(), myVertexType.GetParentVertexType.Name);//builder.AppendLine();
            }

            #endregion

            #region attributes

            if (myVertexType.GetAttributeDefinitions(false).Count() > 0)
            {
                #region !incomingEdges

                if (myVertexType.GetAttributeDefinitions(false).Any(aAttribute => aAttribute.Kind != AttributeType.IncomingEdge))
                {
                    //so, there are attributes that are no incoming edges

                    stringBuilder.Append(S_ATTRIBUTES.ToUpperString() + S_BRACKET_LEFT.ToUpperString());

                    #region properties

                    if (myVertexType.GetAttributeDefinitions(false).Any(aAttribute => aAttribute.Kind == AttributeType.Property))
                    {
                        stringBuilder.Append(CreateGraphDDLOfProperties(myVertexType.GetPropertyDefinitions(false)) + " ");
                    }

                    #endregion

                    #region outgoing edges

                    if (myVertexType.GetAttributeDefinitions(false).Any(aAttribute => aAttribute.Kind == AttributeType.OutgoingEdge))
                    {
                        stringBuilder.Append(CreateGraphDDLOfOutgoingEdges(myVertexType.GetOutgoingEdgeDefinitions(false), myVertexType) + " ");
                    }

                    #endregion

                    stringBuilder.Append(S_BRACKET_RIGHT.ToUpperString() + " ");

                }

                #endregion

                #region incomingEdges

                if (myVertexType.GetAttributeDefinitions(false).Any(aAttribute => aAttribute.Kind == AttributeType.IncomingEdge))
                {
                    stringBuilder.Append(S_BACKWARDEDGES.ToUpperString() + S_BRACKET_LEFT.ToUpperString() + CreateGraphDDLOfIncomingEdges(myVertexType.GetIncomingEdgeDefinitions(false)) + S_BRACKET_RIGHT.ToUpperString() + " ");
                }

                #endregion
            }

            #endregion

            #region Uniques

            if (myVertexType.GetUniqueDefinitions(false).Count() > 0)
            {
                stringBuilder.Append(S_UNIQUE.ToUpperString() + S_BRACKET_LEFT.Symbol + CreateGraphDDLOfUniqueAttributes(myVertexType.GetUniqueDefinitions(false)) + S_BRACKET_RIGHT.Symbol + " ");
            }

            #endregion

            #region Mandatory attributes

            if (myVertexType.GetPropertyDefinitions(false).Any(aProperty => aProperty.IsMandatory))
            {
                stringBuilder.Append(S_MANDATORY.ToUpperString() + S_BRACKET_LEFT.Symbol + CreateGraphDDLOfMandatoryAttributes(myVertexType.GetPropertyDefinitions(false).Where(aProperty => aProperty.IsMandatory)) + S_BRACKET_RIGHT.Symbol + " ");
            }

            #endregion

            #region Indices

            if (myVertexType.GetIndexDefinitions(false).Count() > 0)
            {
                stringBuilder.Append(S_INDICES.ToUpperString() + S_BRACKET_LEFT.Symbol + CreateGraphDDLOfIndices(myVertexType.GetIndexDefinitions(false), myVertexType) + S_BRACKET_RIGHT.Symbol + " ");
            }

            #endregion

            return stringBuilder.ToString();

        }

        private String CreateGraphDDLOfIndices(IEnumerable<IIndexDefinition> myIndexDefinitions, IVertexType myVertexType)
        {
            var _StringBuilder = new StringBuilder();
            var _Delimiter     = ", ";

            foreach (var _AttributeIndex in myIndexDefinitions)
            {

                if (_AttributeIndex.IsUserdefined)
                    continue;

                _StringBuilder.Append(String.Concat(S_BRACKET_LEFT, _AttributeIndex.Name));

                _StringBuilder.Append(String.Concat(" ", S_EDITION.ToUpperString(), " ", _AttributeIndex.Edition));

                _StringBuilder.Append(String.Concat(" ", S_INDEXTYPE.ToUpperString(), " ", _AttributeIndex.IndexTypeName));
                _StringBuilder.Append(String.Concat(" ", S_ON.ToUpperString(), " " + S_ATTRIBUTES.ToUpperString(), " ", GetIndexedPropertyNames(_AttributeIndex.IndexedProperties)));

                _StringBuilder.Append(S_BRACKET_RIGHT);

                _StringBuilder.Append(_Delimiter);

            }

            if (_StringBuilder.Length > _Delimiter.Length)
            {
                _StringBuilder.Remove(_StringBuilder.Length - _Delimiter.Length, 2);
            }

            return _StringBuilder.ToString();
        }

        private String GetIndexedPropertyNames(IEnumerable<IAttributeDefinition> myIndexedProperties)
        {
            var stringBuilder = new StringBuilder();
            var delimiter = ", ";

            foreach (var aIndexedProperty in myIndexedProperties)
            {
                stringBuilder.Append(aIndexedProperty.Name);
                stringBuilder.Append(delimiter);
            }

            if (stringBuilder.Length > delimiter.Length)
            {
                stringBuilder.Remove(stringBuilder.Length - delimiter.Length, 2);
            }

            return stringBuilder.ToString();
        }

        private String CreateGraphDDLOfMandatoryAttributes(IEnumerable<IPropertyDefinition> myMandatoryAttributeDefinitions)
        {
            //TODO: Add default values

            var stringBuilder = new StringBuilder();
            var delimiter = ", ";

            foreach (var aMandatoryAttribute in myMandatoryAttributeDefinitions)
            {
                stringBuilder.Append(aMandatoryAttribute.Name);
                stringBuilder.Append(delimiter);
            }

            if (stringBuilder.Length > delimiter.Length)
            {
                stringBuilder.Remove(stringBuilder.Length - delimiter.Length, 2);
            }

            return stringBuilder.ToString();
        }

        private String CreateGraphDDLOfUniqueAttributes(IEnumerable<IUniqueDefinition> myUniqueAttributeDefinitions)
        {
            var stringBuilder = new StringBuilder();
            var delimiter = ", ";

            foreach (var aUniquenessDefinition in myUniqueAttributeDefinitions)
            {
                //TODO: handle uniqueness on multiple attributes

                stringBuilder.Append(aUniquenessDefinition.GetUniqueAttributeDefinitions().First().Name);
                stringBuilder.Append(delimiter);
            }

            if (stringBuilder.Length > delimiter.Length)
            {
                stringBuilder.Remove(stringBuilder.Length - delimiter.Length, 2);
            }

            return stringBuilder.ToString();
        }

        private String CreateGraphDDLOfIncomingEdges(IEnumerable<IIncomingEdgeDefinition> myIncomingEdgeDefinitions)
        {
            var stringBuilder = new StringBuilder();
            var delimiter = ", ";

            foreach (var _Attribute in myIncomingEdgeDefinitions)
            {
                stringBuilder.Append(String.Concat(_Attribute.RelatedEdgeDefinition.SourceVertexType.Name, ".", _Attribute.RelatedEdgeDefinition.Name, " ", _Attribute.Name));
                stringBuilder.Append(delimiter);
            }

            if (stringBuilder.Length > delimiter.Length)
            {
                stringBuilder.Remove(stringBuilder.Length - delimiter.Length, 2);
            }

            return stringBuilder.ToString();
        }

        private String CreateGraphDDLOfOutgoingEdges(IEnumerable<IOutgoingEdgeDefinition> myOutgoingEdgeDefinitions, IVertexType myIVertexType)
        {
            var stringBuilder = new StringBuilder();
            var delimiter = ", ";

            foreach (var aOutgoingEdgeDefinition in myOutgoingEdgeDefinitions)
            {
                stringBuilder.Append(String.Concat(GetGraphDDL(aOutgoingEdgeDefinition.EdgeType, myIVertexType), " ", aOutgoingEdgeDefinition.Name));

                stringBuilder.Append(delimiter);
            }

            if (stringBuilder.Length > delimiter.Length)
            {
                stringBuilder.Remove(stringBuilder.Length - delimiter.Length, 2);
            }

            return stringBuilder.ToString();
        }

        private string GetGraphDDL(IEdgeType iEdgeType, IVertexType myIVertexType)
        {
            throw new NotImplementedException();
        }

        private String CreateGraphDDLOfProperties(IEnumerable<IPropertyDefinition> myPropertyDefinitions)
        {
            var stringBuilder = new StringBuilder();
            var delimiter = ", ";

            foreach (var _Attribute in myPropertyDefinitions)
            {
                stringBuilder.Append(String.Concat(_Attribute.BaseType.Name.ToUpper() , " ", _Attribute.Name));

                stringBuilder.Append(delimiter);
            }

            if (stringBuilder.Length > delimiter.Length)
            {
                stringBuilder.Remove(stringBuilder.Length - delimiter.Length, 2);
            }

            return stringBuilder.ToString();
        }

        #endregion

        #endregion

        #region Export GraphDML

        /// <summary>
        /// Create the GraphDML of all DBObjects in the database.
        /// </summary>
        /// <param name="myDumpFormat"></param>
        /// <param name="dbContext"></param>
        /// <param name="objectManager"></param>
        /// <returns></returns>
        public List<String> ExportGraphDML(IEnumerable<IVertexType> myTypesToDump)
        {
            var queries = new List<String>();

            #region Go through each type

            foreach (var aVertexType in myTypesToDump)
            {
                var propertyDefinitions = aVertexType.GetPropertyDefinitions(true).ToDictionary(key => key.AttributeID, value => value);


                foreach (var aVertex in GetAllVertices(aVertexType))
                {
                    queries.Add(CreateGraphDMLforIVertex(aVertexType, aVertex, propertyDefinitions));
                }
            }

            #endregion

            return queries;
        }

        private string CreateGraphDMLforIVertex(IVertexType myVertexType, IVertex myVertex,
             Dictionary<long, IPropertyDefinition> myPropertyDefinitions)
        {
            var stringBuilder = new StringBuilder();
            var delimiter = ", ";

            stringBuilder.Append(String.Concat(S_INSERT.ToUpperString(), " ", S_INTO.ToUpperString(), " ", myVertexType.Name, " ", S_VALUES.ToUpperString(), " ", S_BRACKET_LEFT));
            stringBuilder.Append(String.Concat(S_UUID.ToUpperString(), " = '", myVertex.VertexID.ToString(), "'", delimiter));

            #region standard attributes (creationDate, ...)

            string standardProperties = CreateGraphDMLforVertexStandardProperties(myVertex);

            stringBuilder.Append(standardProperties);

            #endregion

            #region properties (age, list<String>, ...)

            string defAttrsDML = CreateGraphDMLforVertexDefinedProperties(myVertex.GetAllProperties(), myPropertyDefinitions);

            stringBuilder.Append(defAttrsDML);

            #endregion

            #region unstructured data
            #endregion

            #region outgoing edges

            #endregion



            //#region CreateGDMLforDBOUnDefinedAttributes

            //var undefAttrs = myDBObjectStream.GetUndefinedAttributePayload(myDBContext.DBObjectManager);

            //if (!undefAttrs.Success())
            //{
            //    return new Exceptional<String>(undefAttrs);
            //}

            //if (undefAttrs.Value.Count > 0)
            //{

            //    Exceptional<String> undefAttrsDML = CreateGraphDMLforDBObjectUndefinedAttributes(myDumpFormat, undefAttrs.Value, myGraphDBType, myDBObjectStream);

            //    if (!undefAttrsDML.Success())
            //    {
            //        return undefAttrsDML;
            //    }

            //    stringBuilder.Append(undefAttrsDML.Value);

            //}

            //#endregion

            //stringBuilder.RemoveSuffix(delimiter);
            //stringBuilder.Append(S_BRACKET_RIGHT);

            //return new Exceptional<String>(stringBuilder.ToString());

            return stringBuilder.ToString();
        }

        private string CreateGraphDMLforVertexStandardProperties(IVertex myVertex)
        {
            var stringBuilder = new StringBuilder();
            var delimiter = ", ";

            #region Comment

            #endregion

            #region Creation date

            #endregion

            #region Modification date

            #endregion

            #region RevisionID

            #endregion

            #region Edition

            #endregion

            return stringBuilder.ToString();
        }

        private string CreateGraphDMLforVertexDefinedProperties(
            IEnumerable<Tuple<long, object>> myStructuredProperties,
            Dictionary<long, IPropertyDefinition> myPropertyDefinitions)
        {
            var stringBuilder = new StringBuilder();
            var delimiter = ", ";

            foreach (var attribute in myStructuredProperties)
            {
                if (attribute.Item2 == null)
                {
                    continue;
                }

                var typeAttribute = myPropertyDefinitions[attribute.Item1];

                switch (typeAttribute.Multiplicity)
                {
                    case TypesOfMultiplicity.Single:

                        #region Single

                        stringBuilder.Append(String.Concat(typeAttribute.Name, " = ", CreateGraphDMLforSingleAttribute(attribute.Item2)));

                        #endregion

                        break;

                    case TypesOfMultiplicity.List:

                        #region List

                        stringBuilder.Append(String.Concat(typeAttribute.Name, " = ", S_LISTOF.ToUpperString(), " ", S_BRACKET_LEFT));
                        foreach (var val in (attribute.Item2 as ICollection))
                        {
                            stringBuilder.Append(CreateGraphDMLforSingleAttribute(val) + delimiter);
                        }
                        stringBuilder.RemoveSuffix(delimiter);
                        stringBuilder.Append(S_BRACKET_RIGHT);

                        #endregion

                        break;
                    case TypesOfMultiplicity.Set:

                        #region Set

                        stringBuilder.Append(String.Concat(typeAttribute.Name, " = ", S_SETOF.ToUpperString(), " ", S_BRACKET_LEFT));
                        foreach (var val in (attribute.Item2 as ICollection))
                        {
                            stringBuilder.Append(CreateGraphDMLforSingleAttribute(val) + delimiter);
                        }
                        stringBuilder.RemoveSuffix(delimiter);
                        stringBuilder.Append(S_BRACKET_RIGHT);

                        #endregion

                        break;
                    default:

                        throw new UnknownException(new NotImplementedException("This should never happen"));
                }

                stringBuilder.Append(delimiter);
            }
           
            return stringBuilder.ToString();
        }

        private string CreateGraphDMLforSingleAttribute(object mySingleAttribute)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<IVertex> GetAllVertices(IVertexType myVertexType)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

        #region IExtendableGrammar Members

        //public void SetAggregates(IEnumerable<ABaseAggregate> aggregates)
        //{

        //    #region Add all plugins to the grammar

        //    if (aggregates.IsNullOrEmpty())
        //    {
        //        /// empty is not the best solution, Maybe: remove complete rule if no importer exist
        //        BNF_Aggregate.Rule = Empty;
        //    }
        //    else
        //    {
        //        foreach (var aggr in aggregates)
        //        {
        //            //BNF_AggregateName + S_BRACKET_LEFT + aggregateArg + S_BRACKET_RIGHT;

        //            var aggrRule = new NonTerminal("aggr_" + aggr.FunctionName, CreateAggregateNode);
        //            aggrRule.Rule = aggr.FunctionName + S_BRACKET_LEFT + BNF_AggregateArg + S_BRACKET_RIGHT;

        //            if (BNF_Aggregate.Rule == null)
        //            {
        //                BNF_Aggregate.Rule = aggrRule;
        //            }
        //            else
        //            {
        //                BNF_Aggregate.Rule |= aggrRule;
        //            }
        //        }
        //    }

        //    #endregion

        //}

        //public void SetFunctions(IEnumerable<ABaseFunction> functions)
        //{

        //    #region Add all plugins to the grammar

        //    if (functions.IsNullOrEmpty())
        //    {
        //        /// empty is not the best solution, Maybe: remove complete rule if no importer exist
        //        BNF_FuncCall.Rule = Empty;
        //    }
        //    else
        //    {
        //        foreach (var func in functions)
        //        {

        //            #region Create funcNonTerminal

        //            var funcNonTerminal = new NonTerminal("func" + func.FunctionName, CreateFunctionCallNode);

        //            var funcParams = func.GetParameters();
        //            if (funcParams.IsNullOrEmpty())
        //            {
        //                funcNonTerminal.Rule = func.FunctionName + S_BRACKET_LEFT + S_BRACKET_RIGHT;

        //            }
        //            else
        //            {

        //                #region Do not add the arguments to the grammar - currently there is an mystic NT1 child for INSERT func

        //                /* Do not add the arguments to the grammar - currently there is an mystic NT1 child for INSERT func
        //                var funcArgsNonTerminal = new NonTerminal("funcArgs" + func.FunctionName);
        //                funcArgsNonTerminal.Rule = BNF_ExprList;
        //                foreach (var param in funcParams.SkipULong(1))
        //                {
        //                    if (param.VariableNumOfParams)
        //                    {
        //                        funcArgsNonTerminal.Rule = BNF_ExprList;
        //                    }
        //                    else
        //                    {
        //                        funcArgsNonTerminal.Rule += S_comma + BNF_Expression;
        //                    }
        //                }
        //                funcArgsNonTerminal.SetOption(TermOptions.IsTransient);
        //                */

        //                #endregion

        //                funcNonTerminal.Rule = func.FunctionName + S_BRACKET_LEFT + BNF_FunArgs + S_BRACKET_RIGHT;
        //            }

        //            #endregion

        //            #region Add funcNonTerminal to the BNF_FuncCall

        //            if (BNF_FuncCall.Rule == null)
        //            {
        //                BNF_FuncCall.Rule = funcNonTerminal;
        //            }
        //            else
        //            {
        //                BNF_FuncCall.Rule |= funcNonTerminal;
        //            }

        //            #endregion

        //        }
        //    }

        //    #endregion

        //}

        //public void SetIndices(IEnumerable<AAttributeIndex> indices)
        //{

        //    #region Add all plugins to the grammar

        //    if (indices.IsNullOrEmpty())
        //    {
        //        /// empty is not the best solution, Maybe: remove complete import rule if no importer exist
        //        BNF_IndexTypeOpt.Rule = Empty;
        //    }
        //    else
        //    {
        //        foreach (var idx in indices)
        //        {
        //            if (BNF_IndexTypeOpt.Rule == null)
        //            {
        //                BNF_IndexTypeOpt.Rule = S_INDEXTYPE + ToTerm(idx.IndexType);
        //            }
        //            else
        //            {
        //                BNF_IndexTypeOpt.Rule |= S_INDEXTYPE + ToTerm(idx.IndexType);
        //            }
        //        }
        //    }

        //    #endregion

        //}

        //public void SetGraphDBImporter(IEnumerable<AGraphDBImport> graphDBImporter)
        //{

        //    #region Add all plugins to the grammar

        //    if (graphDBImporter.IsNullOrEmpty())
        //    {
        //        /// empty is not the best solution, Maybe: remove complete import rule if no importer exist
        //        BNF_ImportFormat.Rule = Empty;
        //        BNF_ImportStmt.Rule = Empty;
        //    }
        //    else
        //    {
        //        foreach (var importer in graphDBImporter)
        //        {
        //            if (BNF_ImportFormat.Rule == null)
        //            {
        //                BNF_ImportFormat.Rule = ToTerm(importer.ImportFormat);
        //            }
        //            else
        //            {
        //                BNF_ImportFormat.Rule |= ToTerm(importer.ImportFormat);
        //            }
        //        }
        //    }

        //    #endregion

        //}

        #endregion
    }
}
