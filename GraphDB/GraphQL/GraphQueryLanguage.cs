/* 
 * Copyright (c) sones GmbH. All rights reserved.
 * <developer>Henning Rauch</developer>
 * <developer>Stefan Licht</developer>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using sones.GraphDB.Aggregates;
using sones.GraphDB.Errors;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Functions;
using sones.GraphDB.GraphQL.StatementNodes;
using sones.GraphDB.GraphQL.StatementNodes.Drop;
using sones.GraphDB.GraphQL.StatementNodes.Dump;
using sones.GraphDB.GraphQL.StatementNodes.Import;
using sones.GraphDB.GraphQL.StatementNodes.Insert;
using sones.GraphDB.GraphQL.StatementNodes.InsertOrReplace;
using sones.GraphDB.GraphQL.StatementNodes.InsertOrUpdate;
using sones.GraphDB.GraphQL.StatementNodes.RebuildIndices;
using sones.GraphDB.GraphQL.StatementNodes.Replace;
using sones.GraphDB.GraphQL.StatementNodes.Select;
using sones.GraphDB.GraphQL.StatementNodes.Setting;
using sones.GraphDB.GraphQL.StatementNodes.Transaction;
using sones.GraphDB.GraphQL.StatementNodes.Truncate;
using sones.GraphDB.GraphQL.StatementNodes.Update;
using sones.GraphDB.GraphQL.StructureNodes;
using sones.GraphDB.ImportExport;
using sones.GraphDB.Indices;
using sones.GraphDB.Interfaces;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.Structures.Operators;

using sones.GraphDB.Settings;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Objects;
using sones.Lib;
using sones.Lib.ErrorHandling;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.Lib.Frameworks.Irony.Scripting.Ast;
using sones.GraphDB.GraphQL.Structure;
using sones.GraphDB.GraphQL.StatementNodes.Link;
using sones.GraphDB.GraphQL.StatementNodes.Unlink;
using sones.GraphDB.TypeManagement;


#endregion

namespace sones.GraphDB.GraphQL
{

    /// <summary>
    /// This class defines the GraphQueryLanguage.
    /// </summary>
    public class GraphQueryLanguage : Grammar, IDumpable, IExtendableGrammar
    {

        #region Consts

        public const String DOT                     = ".";
        public const String TERMINAL_BRACKET_LEFT   = "(";
        public const String TERMINAL_BRACKET_RIGHT  = ")";
        public const String TERMINAL_QUEUESIZE      = "QUEUESIZE";
        public const String TERMINAL_WEIGHTED       = "WEIGHTED";
        public const String TERMINAL_UNIQUE         = "UNIQUE";
        public const String TERMINAL_MANDATORY      = "MANDATORY";
        public const String TERMINAL_SORTED         = "SORTED";        
        public const String TERMINAL_ASC            = "ASC";
        public const String TERMINAL_DESC           = "DESC";
        public const String TERMINAL_TRUE           = "TRUE";
        public const String TERMINAL_FALSE          = "FALSE";
        public const String TERMINAL_LIST           = "LIST";
        public const String TERMINAL_SET            = "SET";
        public const String TERMINAL_LT             = "<";
        public const String TERMINAL_GT             = ">";
        
        #endregion

        #region Properties

        #region NonTerminals

        public NonTerminal BNF_TypesOrVertices { get; private set; }

        #endregion

        #region SymbolTerminals

        public SymbolTerminal S_CREATE                          { get; private set; }
        public SymbolTerminal S_comma                           { get; private set; }
        public SymbolTerminal S_dot                             { get; private set; }
        public SymbolTerminal S_ASTERISK                        { get; private set; }
        public SymbolTerminal S_RHOMB                           { get; private set; }
        public SymbolTerminal S_MINUS                           { get; private set; }
        public SymbolTerminal S_AD                              { get; private set; }
        public SymbolTerminal S_colon                           { get; private set; }
        public SymbolTerminal S_EQUALS                          { get; private set; }
        public SymbolTerminal S_QUESTIONMARK_EQUALS             { get; private set; }

        #region Brackets

        public SymbolTerminal S_BRACKET_LEFT                    { get; private set; }
        public SymbolTerminal S_BRACKET_RIGHT                   { get; private set; }
        public SymbolTerminal S_TUPLE_BRACKET_LEFT              { get; private set; }
        public SymbolTerminal S_TUPLE_BRACKET_RIGHT             { get; private set; }
        public SymbolTerminal S_TUPLE_BRACKET_LEFT_EXCLUSIVE
        {
            get { return S_BRACKET_LEFT; }
        }
        public SymbolTerminal S_TUPLE_BRACKET_RIGHT_EXCLUSIVE
        {
            get { return S_BRACKET_RIGHT; }
        }

        #endregion

        public SymbolTerminal S_edgeInformationDelimiterSymbol  { get; private set; }
        public SymbolTerminal S_edgeTraversalDelimiter          { get; private set; }
        public SymbolTerminal S_NULL                            { get; private set; }
        public SymbolTerminal S_NOT                             { get; private set; }
        public SymbolTerminal S_UNIQUE                          { get; private set; }
        public SymbolTerminal S_WITH                            { get; private set; }
        public SymbolTerminal S_TABLE                           { get; private set; }
        public SymbolTerminal S_ALTER                           { get; private set; }
        public SymbolTerminal S_ADD                             { get; private set; }
        public SymbolTerminal S_TO                              { get; private set; }
        public SymbolTerminal S_COLUMN                          { get; private set; }
        public SymbolTerminal S_DROP                            { get; private set; }
        public SymbolTerminal S_RENAME          { get; private set; }
        public SymbolTerminal S_CONSTRAINT      { get; private set; }
        public SymbolTerminal S_INDEX           { get; private set; }
        public SymbolTerminal S_INDICES         { get; private set; }
        public SymbolTerminal S_ON              { get; private set; }
        public SymbolTerminal S_KEY             { get; private set; }
        public SymbolTerminal S_PRIMARY         { get; private set; }
        public SymbolTerminal S_INSERT          { get; private set; }
        public SymbolTerminal S_INTO            { get; private set; }
        public SymbolTerminal S_UPDATE          { get; private set; }
        public SymbolTerminal S_INSERTORUPDATE  { get; private set; }
        public SymbolTerminal S_INSERTORREPLACE { get; private set; }
        public SymbolTerminal S_REPLACE         { get; private set; }
        public SymbolTerminal S_SET             { get; private set; }
        public SymbolTerminal S_REMOVE          { get; private set; }
        public SymbolTerminal S_VALUES          { get; private set; }
        public SymbolTerminal S_DELETE          { get; private set; }
        public SymbolTerminal S_SELECT          { get; private set; }
        public SymbolTerminal S_FROM            { get; private set; }
        public SymbolTerminal S_AS              { get; private set; }
        public SymbolTerminal S_COUNT           { get; private set; }
        public SymbolTerminal S_JOIN            { get; private set; }
        public SymbolTerminal S_BY              { get; private set; }
        public SymbolTerminal S_WHERE           { get; private set; }
        public SymbolTerminal S_TYPE            { get; private set; }
        public SymbolTerminal S_TYPES           { get; private set; }
        public SymbolTerminal S_VERTEX          { get; private set; }
        public SymbolTerminal S_VERTICES        { get; private set; }
        public SymbolTerminal S_EDITION         { get; private set; }
        public SymbolTerminal S_INDEXTYPE       { get; private set; }
        public SymbolTerminal S_LIST            { get; private set; }
        public SymbolTerminal S_ListTypePrefix  { get; private set; }
        public SymbolTerminal S_ListTypePostfix { get; private set; }
        public SymbolTerminal S_EXTENDS         { get; private set; }
        public SymbolTerminal S_ATTRIBUTES      { get; private set; }
        public SymbolTerminal S_MATCHES         { get; private set; }
        public SymbolTerminal S_LIMIT           { get; private set; }
        public SymbolTerminal S_DEPTH           { get; private set; }
        public SymbolTerminal S_DEFINE          { get; private set; }
        public SymbolTerminal S_UNDEFINE        { get; private set; }
        public SymbolTerminal S_SHARDS          { get; private set; }

        #region REF/REFUUID/...

        public SymbolTerminal S_REF             { get; private set; }
        public SymbolTerminal S_REFERENCE       { get; private set; }
        public SymbolTerminal S_REFUUID         { get; private set; }
        public SymbolTerminal S_REFERENCEUUID   { get; private set; }

        #endregion

        #region LISTOF/SETOF/SETOFUUIDS

        public SymbolTerminal S_LISTOF          { get; private set; }
        public SymbolTerminal S_SETOF           { get; private set; }
        public SymbolTerminal S_SETOFUUIDS      { get; private set; }
       
        #endregion

        public SymbolTerminal S_UUID            { get; private set; }
        public SymbolTerminal S_OFFSET          { get; private set; }
        public SymbolTerminal S_TRUNCATE        { get; private set; }
        public SymbolTerminal S_TRUE            { get; private set; }
        public SymbolTerminal S_FALSE           { get; private set; }
        public SymbolTerminal S_SORTED          { get; private set; }
        public SymbolTerminal S_ASC             { get; private set; }
        public SymbolTerminal S_DESC            { get; private set; }

        public SymbolTerminal S_DESCRIBE        { get; private set; }
        public SymbolTerminal S_QUEUESIZE       { get; private set; }
        public SymbolTerminal S_WEIGHTED        { get; private set; }
        public SymbolTerminal S_SETTING         { get; private set; }
        public SymbolTerminal S_GET             { get; private set; }
        public SymbolTerminal S_DB              { get; private set; }
        public SymbolTerminal S_SESSION         { get; private set; }
        public SymbolTerminal S_ATTRIBUTE       { get; private set; }
        public SymbolTerminal S_DEFAULT         { get; private set; }        

        public SymbolTerminal S_BACKWARDEDGES   { get; private set; }
        public SymbolTerminal S_BACKWARDEDGE    { get; private set; }
        public SymbolTerminal S_FUNCTION        { get; private set; }
        public SymbolTerminal S_AGGREGATE       { get; private set; }
        public SymbolTerminal S_AGGREGATES      { get; private set; }
        public SymbolTerminal S_SETTINGS        { get; private set; }
        public SymbolTerminal S_FUNCTIONS       { get; private set; }
        public SymbolTerminal S_EDGE            { get; private set; }
        public SymbolTerminal S_EDGES           { get; private set; }
        public SymbolTerminal S_MANDATORY       { get; private set; }
        public SymbolTerminal S_ABSTRACT        { get; private set; }

        #region Transactions

        public SymbolTerminal S_BEGIN                   { get; private set; }
        public SymbolTerminal S_TRANSACTION             { get; private set; }
        public SymbolTerminal S_TRANSACTDISTRIBUTED     { get; private set; }
        public SymbolTerminal S_TRANSACTLONGRUNNING     { get; private set; }
        public SymbolTerminal S_TRANSACTISOLATION       { get; private set; }
        public SymbolTerminal S_TRANSACTNAME            { get; private set; }
        public SymbolTerminal S_TRANSACTTIMESTAMP       { get; private set; }
        public SymbolTerminal S_TRANSACTCOMMIT          { get; private set; }
        public SymbolTerminal S_TRANSACTROLLBACK        { get; private set; }
        public SymbolTerminal S_TRANSACTCOMROLLASYNC    { get; private set; }

        #endregion

        public SymbolTerminal S_REMOVEFROMLIST          { get; private set; }
        public SymbolTerminal S_ADDTOLIST               { get; private set; }
        public SymbolTerminal S_COMMENT                 { get; private set; }
        public SymbolTerminal S_REBUILD                 { get; private set; }

        #region IMPORT

        public SymbolTerminal S_IMPORT                  { get; private set; }
        public SymbolTerminal S_COMMENTS                { get; private set; }
        public SymbolTerminal S_PARALLELTASKS           { get; private set; }
        public SymbolTerminal S_VERBOSITY               { get; private set; }
        public SymbolTerminal S_FORMAT                  { get; private set; }

        #endregion

        #region DUMP

        public SymbolTerminal S_DUMP            { get; private set; }
        public SymbolTerminal S_EXPORT          { get; private set; }
        public SymbolTerminal S_ALL             { get; private set; }
        public SymbolTerminal S_GDDL            { get; private set; }
        public SymbolTerminal S_GDML            { get; private set; }
        public SymbolTerminal S_GQL             { get; private set; }
        public SymbolTerminal S_CSV             { get; private set; }



        #endregion

        #region LINK

        public SymbolTerminal S_VIA                     { get; private set; }
        public SymbolTerminal S_LINK                    { get; private set; }

        #endregion

        #region UNLINK

        public SymbolTerminal S_UNLINK                  { get; private set; }

        #endregion

        #endregion

        #endregion

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

        #region Constructor and Definitions

        public GraphQueryLanguage(DBContext doNotUseMe)
            : this()
        { }

        public GraphQueryLanguage()
            : base(false)
        {

            #region SetLanguageFlags

            this.SetLanguageFlags(LanguageFlags.CreateAst);
            this.SetLanguageFlags(LanguageFlags.AutoDetectTransient, false);

            //Todo: think about this:
            //this.SetLanguageFlags(LanguageFlags.TailRecursive);

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

            var number              = new NumberLiteral("number", NumberFlags.AllowSign | NumberFlags.DisableQuickParse);
            number.DefaultIntTypes  = new TypeCode[] { TypeCode.UInt64, TypeCode.Int64, NumberLiteral.TypeCodeBigInt };
            var string_literal      = new StringLiteral("string", "'", StringFlags.AllowsDoubledQuote | StringFlags.AllowsLineBreak);
            var location_literal    = new StringLiteral("file", "'", StringFlags.AllowsDoubledQuote | StringFlags.AllowsLineBreak | StringFlags.NoEscapes);
            
            var name                = new IdentifierTerminal("name", "ÄÖÜäöüß0123456789_", "ÄÖÜäöü0123456789$_");


            #endregion

            //var name_ext            = TerminalFactory.CreateSqlExtIdentifier("name_ext"); //removed, because we do not want to hav types or sth else with whitespaces, otherwise it conflicts with tupleSet

            #region Symbols

            S_CREATE                          = Symbol("CREATE");
            S_comma                           = Symbol(",");
            S_dot                             = Symbol(".");
            S_ASTERISK                        = Symbol("*");
            S_MINUS                           = Symbol("-");
            S_RHOMB                           = Symbol("#");
            S_AD                              = Symbol("@");
            S_EQUALS                          = Symbol("=");
            S_QUESTIONMARK_EQUALS             = Symbol("?=");
           
            S_colon                           = Symbol(":");
            S_BRACKET_LEFT                    = Symbol(TERMINAL_BRACKET_LEFT);
            S_BRACKET_RIGHT                   = Symbol(TERMINAL_BRACKET_RIGHT);
            S_TUPLE_BRACKET_LEFT              = Symbol("[");
            S_TUPLE_BRACKET_RIGHT             = Symbol("]");
            S_edgeInformationDelimiterSymbol  = Symbol(DBConstants.EdgeInformationDelimiterSymbol);
            S_edgeTraversalDelimiter          = Symbol(DBConstants.EdgeTraversalDelimiterSymbol);
            S_NULL                            = Symbol("NULL");
            S_NOT                             = Symbol("NOT");
            S_UNIQUE                          = Symbol("UNIQUE");
            S_WITH                            = Symbol("WITH");
            S_TABLE                           = Symbol("TABLE");
            S_ALTER                           = Symbol("ALTER");
            S_ADD                             = Symbol("ADD");
            S_TO                              = Symbol("TO");
            S_COLUMN                          = Symbol("COLUMN");
            S_DROP                            = Symbol("DROP");
            S_RENAME                          = Symbol("RENAME");
            S_CONSTRAINT                      = Symbol("CONSTRAINT");
            S_INDEX                           = Symbol("INDEX");
            S_INDICES                         = Symbol("INDICES");
            S_ON                              = Symbol("ON");
            S_KEY                             = Symbol("KEY");
            S_PRIMARY                         = Symbol("PRIMARY");
            S_INSERT                          = Symbol("INSERT");
            S_INTO                            = Symbol("INTO");
            S_UPDATE                          = Symbol("UPDATE");
            S_INSERTORUPDATE                  = Symbol("INSERTORUPDATE");
            S_INSERTORREPLACE                 = Symbol("INSERTORREPLACE");
            S_REPLACE                         = Symbol("REPLACE");
            S_SET                             = Symbol(TERMINAL_SET);
            S_REMOVE                          = Symbol("REMOVE");
            S_VALUES                          = Symbol("VALUES");
            S_DELETE                          = Symbol("DELETE");
            S_SELECT                          = Symbol("SELECT");
            S_FROM                            = Symbol("FROM");
            S_AS                              = Symbol("AS");
            S_COUNT                           = Symbol("COUNT");
            S_JOIN                            = Symbol("JOIN");
            S_BY                              = Symbol("BY");
            S_WHERE                           = Symbol("WHERE");
            S_TYPE                            = Symbol("TYPE");
            S_TYPES                           = Symbol("TYPES");
            S_VERTEX                          = Symbol("VERTEX");
            S_VERTICES                        = Symbol("VERTICES");
            S_EDITION                         = Symbol("EDITION");
            S_INDEXTYPE                       = Symbol("INDEXTYPE");
            S_LIST                            = Symbol(TERMINAL_LIST);
            S_ListTypePrefix                  = Symbol(TERMINAL_LT);
            S_ListTypePostfix                 = Symbol(TERMINAL_GT);
            S_EXTENDS                         = Symbol("EXTENDS");
            S_ATTRIBUTES                      = Symbol("ATTRIBUTES");
            S_MATCHES                         = Symbol("MATCHES");
            S_LIMIT                           = Symbol("LIMIT");
            S_DEPTH                           = Symbol("DEPTH");
            S_REFERENCE                       = Symbol("REFERENCE");
            S_REF                             = Symbol("REF");
            S_REFUUID                         = Symbol("REFUUID");
            S_REFERENCEUUID                   = Symbol("REFERENCEUUID");
            S_LISTOF                          = Symbol(DBConstants.LISTOF);
            S_SETOF                           = Symbol(DBConstants.SETOF);
            S_SETOFUUIDS                      = Symbol(DBConstants.SETOFUUIDS);
            S_UUID                            = Symbol("UUID");
            S_OFFSET                          = Symbol("OFFSET");
            S_TRUNCATE                        = Symbol("TRUNCATE");
            S_TRUE                            = Symbol(TERMINAL_TRUE);
            S_FALSE                           = Symbol(TERMINAL_FALSE);
            S_SORTED                          = Symbol(TERMINAL_SORTED);
            S_ASC                             = Symbol(TERMINAL_ASC);
            S_DESC                            = Symbol(TERMINAL_DESC);
            S_QUEUESIZE                       = Symbol(TERMINAL_QUEUESIZE);
            S_WEIGHTED                        = Symbol(TERMINAL_WEIGHTED);
            S_GET                             = Symbol("GET");
            S_DB                              = Symbol("DB");
            S_SESSION                         = Symbol("SESSION");
            S_ATTRIBUTE                       = Symbol("ATTRIBUTE");
            S_DEFAULT                         = Symbol("DEFAULT");
            S_BACKWARDEDGE                    = Symbol("BACKWARDEDGE");
            S_BACKWARDEDGES                   = Symbol("BACKWARDEDGES");
            S_DESCRIBE                        = Symbol("DESCRIBE");
            S_FUNCTION                        = Symbol("FUNCTION");
            S_FUNCTIONS                       = Symbol("FUNCTIONS");
            S_AGGREGATE                       = Symbol("AGGREGATE");
            S_AGGREGATES                      = Symbol("AGGREGATES");
            S_SETTING                         = Symbol("SETTING");
            S_SETTINGS                        = Symbol("SETTINGS");
            S_INDICES                         = Symbol("INDICES");
            S_EDGE                            = Symbol("EDGE");
            S_EDGES                           = Symbol("EDGES");
            S_MANDATORY                       = Symbol("MANDATORY");
            S_ABSTRACT                        = Symbol("ABSTRACT");
            S_BEGIN                           = Symbol("BEGIN");
            S_TRANSACTION                     = Symbol("TRANSACTION");
            S_TRANSACTDISTRIBUTED             = Symbol(DBConstants.TRANSACTION_DISTRIBUTED);
            S_TRANSACTLONGRUNNING             = Symbol(DBConstants.TRANSACTION_LONGRUNNING);
            S_TRANSACTISOLATION               = Symbol(DBConstants.TRANSACTION_ISOLATION);
            S_TRANSACTNAME                    = Symbol(DBConstants.TRANSACTION_NAME);
            S_TRANSACTTIMESTAMP               = Symbol(DBConstants.TRANSACTION_TIMESTAMP);
            S_TRANSACTROLLBACK                = Symbol(DBConstants.TRANSACTION_ROLLBACK);
            S_TRANSACTCOMMIT                  = Symbol(DBConstants.TRANSACTION_COMMIT);
            S_TRANSACTCOMROLLASYNC            = Symbol(DBConstants.TRANSACTION_COMROLLASYNC);
            S_ADDTOLIST                       = Symbol("+=");
            S_REMOVEFROMLIST                  = Symbol("-=");
            S_DUMP                            = Symbol("DUMP");
            S_EXPORT                          = Symbol("EXPORT");
            S_ALL                             = Symbol("ALL");
            S_GDDL                            = Symbol("GDDL");
            S_GDML                            = Symbol("GDML");
            S_GQL                             = Symbol("GQL");
            S_CSV                             = Symbol("CSV");
            S_COMMENT                         = Symbol("COMMENT");
            S_REBUILD                         = Symbol("REBUILD");
            S_DEFINE                          = Symbol("DEFINE");
            S_UNDEFINE                        = Symbol("UNDEFINE");
            S_VIA                             = Symbol("VIA");
            S_LINK                            = Symbol("LINK");
            S_UNLINK                          = Symbol("UNLINK");
            S_SHARDS                          = Symbol("SHARDS");

            #region IMPORT

            S_IMPORT                          = Symbol("IMPORT");
            S_COMMENTS                        = Symbol("COMMENTS");
            S_PARALLELTASKS                   = Symbol("PARALLELTASKS");
            S_VERBOSITY                       = Symbol("VERBOSITY");
            S_FORMAT                          = Symbol("FORMAT");

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
            var uniqueOpt                   = new NonTerminal("uniqueOpt",              typeof(uniqueOptNode));
            var IndexAttributeList          = new NonTerminal("IndexAttributeList",     typeof(IndexAttributeListNode));
            var IndexAttributeMember        = new NonTerminal("IndexAttributeMember",   typeof(IndexAttributeNode));
            var IndexAttributeType          = new NonTerminal("IndexAttributeType");
            var orderByAttributeList        = new NonTerminal("orderByAttributeList");
            var orderByAttributeListMember  = new NonTerminal("orderByAttributeListMember");
            var AttributeOrderDirectionOpt  = new NonTerminal("AttributeOrderDirectionOpt");
            BNF_IndexTypeOpt                = new NonTerminal("indexTypeOpt",           typeof(IndexTypeOptNode));
            var indexNameOpt                = new NonTerminal("indextNameOpt",          typeof(IndexNameOptNode));
            var editionOpt                  = new NonTerminal("editionOpt",             typeof(EditionOptNode));
            var alterCmd                    = new NonTerminal("alterCmd",               typeof(AlterCommandNode));
            var alterCmdList                = new NonTerminal("alterCmdList");
            var insertData                  = new NonTerminal("insertData");
            var intoOpt                     = new NonTerminal("intoOpt");
            var assignList                  = new NonTerminal("assignList");
            var whereClauseOpt              = new NonTerminal("whereClauseOpt",         CreateWhereExpressionNode);
            var extendsOpt                  = new NonTerminal("extendsOpt");
            var abstractOpt                 = new NonTerminal("abstractOpt");
            var commentOpt                  = new NonTerminal("CommentOpt");
            var bulkTypeList                = new NonTerminal("bulkTypeList");
            var attributesOpt               = new NonTerminal("attributesOpt");
            var insertValuesOpt             = new NonTerminal("insertValuesOpt");
            var optionalShards              = new NonTerminal("optionalShards",         typeof(ShardsNode));

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
            selectionSource                 = new NonTerminal("selectionSource");
            var selByType                   = new NonTerminal("selByType", CreateSelByTypeNode);
            var aliasOpt                    = new NonTerminal("aliasOpt");
            var aliasOptName                = new NonTerminal("aliasOptName");
            var selectOutputOpt             = new NonTerminal("selectOutputOpt", typeof(SelectOutputOptNode));

            #endregion

            #region Aggregates & Functions

            BNF_Aggregate                   = new NonTerminal("aggregate", CreateAggregateNode);
            BNF_AggregateArg                = new NonTerminal("aggregateArg");
            var function                    = new NonTerminal("function", CreateFunctionCallNode);
            var functionName                = new NonTerminal("functionName");
            BNF_FunArgs                     = new NonTerminal("funArgs");
            BNF_FuncCall                    = new NonTerminal("funCall", CreateFunctionCallNode);

            #endregion

            #region Tuple

            var tuple                       = new NonTerminal("tuple", typeof(TupleNode));
            var bracketLeft                 = new NonTerminal(DBConstants.BracketLeft);
            var bracketRight                = new NonTerminal(DBConstants.BracketRight);
            

            #endregion

            var term                        = new NonTerminal("term");
            var notOpt                      = new NonTerminal("notOpt");

            var typeOrVertex                = new NonTerminal("typeOrVertex");
            BNF_TypesOrVertices             = new NonTerminal("typesOrVertices");

            var GraphDBType                 = new NonTerminal(DBConstants.GraphDBType, CreateGraphDBTypeNode);
            var AttributeList               = new NonTerminal("AttributeList");
            var AttrDefinition              = new NonTerminal("AttrDefinition",     CreateAttributeDefinitionNode);
            var ResultObject                = new NonTerminal("ResultObject");
            var ResultList                  = new NonTerminal("ResultList");
            var MatchingClause              = new NonTerminal("MatchingClause");
            var Matching                    = new NonTerminal("MatchingClause");
            var PrefixOperation             = new NonTerminal("PrefixOperation");
            var ParameterList               = new NonTerminal("ParameterList");
            var TypeList                    = new NonTerminal("TypeList",           CreateTypeListNode);
            var AType                       = new NonTerminal("AType",              CreateATypeNode);
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
            var AttrUpdateOrAssign          = new NonTerminal("AttrUpdateOrAssign");
            var CollectionOfDBObjects       = new NonTerminal("ListOfDBObjects", typeof(CollectionOfDBObjectsNode));
            var CollectionTuple = new NonTerminal("CollectionTuple", typeof(TupleNode));
            var ExtendedExpressionList = new NonTerminal("ExtendedExpressionList");
            var ExtendedExpression = new NonTerminal("ExtendedExpression", typeof(ExpressionOfAListNode));

            #endregion

            var Reference                   = new NonTerminal(S_REFERENCE.Symbol,   typeof(SetRefNode));
            var offsetOpt                   = new NonTerminal("offsetOpt",          typeof(OffsetNode));
            var resolutionDepthOpt          = new NonTerminal("resolutionDepthOpt");
            var limitOpt                    = new NonTerminal("limitOpt",           typeof(LimitNode));
            var SimpleIdList                = new NonTerminal("SimpleIdList");
            var bulkTypeListMember          = new NonTerminal("bulkTypeListMember", CreateBulkTypeListMemberNode);
            var bulkType                    = new NonTerminal("bulkType",           CreateBulkTypeNode);
            var truncateStmt                = new NonTerminal("truncateStmt",       CreateTruncateStmNode);
            var uniquenessOpt               = new NonTerminal("UniquenessOpt",      typeof(UniqueAttributesOptNode));
            var mandatoryOpt                = new NonTerminal("MandatoryOpt",       typeof(MandatoryOptNode));

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
            var dumpType                        = new NonTerminal("dumpType",   CreateDumpTypeNode);
            var dumpFormat                      = new NonTerminal("dumpFormat", CreateDumpFormatNode);
            var typeOptionalList                = new NonTerminal("typeOptionalList");
            var dumpDestination                 = new NonTerminal("dumpDestination");

            #endregion

            #region Describe

            var DescrInfoStmt               = new NonTerminal("DescrInfoStmt",      CreateDescribeNode);
            var DescrArgument               = new NonTerminal("DescrArgument");
            var DescrFuncStmt               = new NonTerminal("DescrFuncStmt",      CreateDescrFunc);
            var DescrFunctionsStmt          = new NonTerminal("DescrFunctionsStmt", CreateDescrFunctions);
            var DescrAggrStmt               = new NonTerminal("DescrAggrStmt",      CreateDescrAggr);
            var DescrAggrsStmt              = new NonTerminal("DescrAggrsStmt",     CreateDescrAggrs);
            var DescrSettStmt               = new NonTerminal("DescrSettStmt",      CreateDescrSett);
            var DescrSettItem               = new NonTerminal("DescrSettItem",      CreateDescrSettItem);
            var DescrSettingsItems          = new NonTerminal("DescrSettingsItems", CreateDescrSettingsItems); 
            var DescrSettingsStmt           = new NonTerminal("DescrSettingsStmt",  CreateDescrSettings);
            var DescrTypeStmt               = new NonTerminal("DescrTypeStmt",      CreateDescrType);
            var DescrTypesStmt              = new NonTerminal("DescrTypesStmt",     CreateDescrTypes);
            var DescrIdxStmt                = new NonTerminal("DescrIdxStmt",       CreateDescrIdx);
            var DescrIdxsStmt               = new NonTerminal("DescrIdxsStmt",      CreateDescrIdxs);
            var DescrIdxEdtStmt             = new NonTerminal("DescrIdxEdtStmt");
            var DescrEdgeStmt               = new NonTerminal("DescrEdgeStmt",      CreateDescrEdge);
            var DescrEdgesStmt              = new NonTerminal("DescrEdgesStmt",     CreateDescrEdges);

            #endregion

            #region REBUILD INDICES

            var rebuildIndicesStmt          = new NonTerminal("rebuildIndicesStmt", CreateRebuildIndicesNode);
            var rebuildIndicesTypes         = new NonTerminal("rebuildIndiceTypes");

            #endregion

            #region Import

            BNF_ImportFormat        = new NonTerminal("importFormat");
            BNF_ImportStmt          = new NonTerminal("import", CreateImportNode);
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

            Id.SetOption(TermOptions.IsList);
            Id.Rule =       Id_simple
                        |   Id + EdgeTraversalWithOutFunctions;
            //old
            //Id.Rule = MakePlusRule(Id, dotWrapper, Id_simple);

            Id.Description = "an id is composed by an identifier a dot and a second identifier -  or a list of them an id could be ‘U.Name’ or ‘U.Friends.Age’\n";
            idlist.Rule = MakePlusRule(idlist, S_comma, Id);
            id_simpleList.Rule = MakePlusRule(id_simpleList, S_comma, Id_simple);
            id_simpleDotList.Rule = MakePlusRule(id_simpleDotList, S_dot, Id_simple);
            id_typeAndAttribute.Rule = TypeWrapper + S_dot + Id;

            #endregion

            #region ID_or_Func

            IdOrFunc.Rule =     name 
                            |   BNF_FuncCall;

            dotWrapper.Rule = S_edgeTraversalDelimiter;

            edgeAccessorWrapper.Rule = S_edgeInformationDelimiterSymbol;

            //IDOrFuncDelimiter.Rule =        dotWrapper
            //                            |   edgeAccessorWrapper;

            EdgeTraversalWithFunctions.Rule = dotWrapper + IdOrFunc;

            EdgeInformation.Rule = edgeAccessorWrapper + Id_simple;

            IdOrFuncList.SetOption(TermOptions.IsList);
            IdOrFuncList.Rule =         IdOrFunc
                                    |   IdOrFuncList + EdgeInformation
                                    |   IdOrFuncList + EdgeTraversalWithFunctions;

            //old
            //IdOrFuncList.Rule = MakePlusRule(IdOrFuncList, IDOrFuncDelimiter, IdOrFunc);

            #endregion

            #endregion

            #region typeList

            TypeList.Rule = MakePlusRule(TypeList, S_comma, AType);
            TypeList.Description = "specify the type object to be selected for example a type list could be ‘User U’, ‘Car C’, …\n";

            AType.Rule = Id_simple + Id_simple
                        | Id_simple;

            //AType.Rule = Id + Id_simple
            //                | Id;

            TypeWrapper.Rule = AType;

            #endregion

            #region CreateIndexAttribute

            IndexAttributeList.Rule = MakePlusRule(IndexAttributeList, S_comma, IndexAttributeMember);

            IndexAttributeMember.Rule = IndexAttributeType;// + AttributeOrderDirectionOpt;

            IndexAttributeType.Rule = Id_simple | id_typeAndAttribute;

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

            EdgeTypeParam.SetOption(TermOptions.IsTransient, false);

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

            BNF_Expression.Rule =       term
                                |   unExpr
                                |   binExpr;

            expressionOfAList.Rule = BNF_Expression + ListParametersForExpression;


            term.Rule =         IdOrFuncList                  //d.Name 
                            |   string_literal      //'lala'
                            |   number              //10
                            //|   funcCall            //EXISTS ( SelectStatement )
                            |   BNF_Aggregate           //COUNT ( SelectStatement )
                            |   tuple               //(d.Name, 'Henning', (SelectStatement))
                            |   parSelectStmt      //(FROM User u Select u.Name)
                            | S_TRUE
                            | S_FALSE;

            #region Tuple

            tuple.Rule = bracketLeft + BNF_ExprList + bracketRight;

            bracketLeft.Rule = S_BRACKET_LEFT | S_TUPLE_BRACKET_LEFT;
            bracketRight.Rule = S_BRACKET_RIGHT | S_TUPLE_BRACKET_RIGHT;

            #endregion

            parSelectStmt.Rule = S_BRACKET_LEFT + SelectStmtGraph + S_BRACKET_RIGHT;

            unExpr.Rule = unOp + term;

            unOp.Rule =         S_NOT 
                            |   "+" 
                            |   "-" 
                            |   "~";

            binExpr.Rule = BNF_Expression + binOp + BNF_Expression;

            binOp.Rule =        Symbol("+") 
                            |   "-" 
                            |   "*" 
                            |   "/" 
                            |   "%" //arithmetic
                            |   "&" 
                            |   "|" 
                            |   "^"                     //bit
                            |   "=" 
                            |   ">" 
                            |   "<" 
                            |   ">=" 
                            |   "<=" 
                            |   "<>" 
                            |   "!=" 
                            |   "!<" 
                            |   "!>"
                            |   "AND" 
                            |   "OR" 
                            |   "LIKE"
                            |   S_NOT + "LIKE" 
                            |   "IN" 
                            |   "NOTIN" | "NOT_IN" | "NIN" | "!IN"
                            |   "INRANGE";

            notOpt.Rule =       Empty
                            |   S_NOT;

            #endregion

            #region Functions & Aggregates

            //funcCall covers some psedo-operators and special forms like ANY(...), SOME(...), ALL(...), EXISTS(...), IN(...)
            //funcCall.Rule = BNF_FuncCallName + S_BRACKET_LEFT + funArgs + S_BRACKET_RIGHT;

            // The grammar will be created by IExtendableGrammer methods
            //BNF_Aggregate.Rule = Empty;
            //BNF_FuncCall.Rule = Empty;


            BNF_FunArgs.Rule =      SelectStmtGraph 
                            |   BNF_ExprList;

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

            PrefixOperation.Rule =      Id_simple + S_BRACKET_LEFT + ParameterList + S_BRACKET_RIGHT;

            ParameterList.Rule =        ParameterList + S_comma + BNF_Expression
                                    |   BNF_Expression;

            #endregion

            #endregion

            #region CREATE INDEX

            createIndexStmt.Rule = S_CREATE + S_INDEX + indexNameOpt + editionOpt + S_ON + typeOrVertex + TypeWrapper + S_BRACKET_LEFT + IndexAttributeList + S_BRACKET_RIGHT + BNF_IndexTypeOpt
                | S_CREATE + S_INDEX + indexNameOpt + editionOpt + S_ON + TypeWrapper + S_BRACKET_LEFT + IndexAttributeList + S_BRACKET_RIGHT + BNF_IndexTypeOpt; // due to compatibility the  + S_TYPE is optional

            uniqueOpt.Rule = Empty | S_UNIQUE;

            editionOpt.Rule =       Empty
                                | S_EDITION + Id_simple;

            BNF_IndexTypeOpt.Rule =     Empty
                                | S_INDEXTYPE + Id_simple;

            indexNameOpt.Rule = Empty
                                |   Id_simple;

            #endregion

            #region REBUILD INDICES

            rebuildIndicesStmt.Rule = S_REBUILD + S_INDICES + rebuildIndicesTypes;

            rebuildIndicesTypes.Rule = Empty | TypeList;

            #endregion

            #region CREATE TYPE(S)

            createTypesStmt.Rule    = S_CREATE + BNF_TypesOrVertices + bulkTypeList
                                    | S_CREATE + abstractOpt + typeOrVertex + bulkType;

            typeOrVertex.Rule       = S_TYPE | S_VERTEX;
            BNF_TypesOrVertices.Rule    = S_TYPES | S_VERTICES;

            bulkTypeList.Rule       = MakePlusRule(bulkTypeList, S_comma, bulkTypeListMember);

            bulkTypeListMember.Rule = abstractOpt + bulkType;

            bulkType.Rule           = Id_simple + extendsOpt + attributesOpt + backwardEdgesOpt + uniquenessOpt + mandatoryOpt + indexOptOnCreateType + commentOpt;

            commentOpt.Rule         =   Empty
                                    |   S_COMMENT + "=" + string_literal; 

            abstractOpt.Rule        = Empty
                                    | S_ABSTRACT; 

            extendsOpt.Rule         = Empty
                                    | S_EXTENDS + Id_simple;

            attributesOpt.Rule      = Empty
                                    | S_ATTRIBUTES + S_BRACKET_LEFT + AttributeList + S_BRACKET_RIGHT;

            backwardEdgesOpt.Rule   = Empty
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

            optionalShards.Rule     = S_SHARDS + number | Empty;

            IndexOptOnCreateTypeMember.Rule = S_BRACKET_LEFT + indexNameOpt + editionOpt + BNF_IndexTypeOpt + S_ON + S_ATTRIBUTES + IndexAttributeList + S_BRACKET_RIGHT
                                            | S_BRACKET_LEFT + indexNameOpt + editionOpt + BNF_IndexTypeOpt + S_ON + IndexAttributeList + S_BRACKET_RIGHT // due to compatibility the  + S_ATTRIBUTES is optional
                                            | S_BRACKET_LEFT + IndexAttributeList + S_BRACKET_RIGHT;

            AttrDefaultOpValue.Rule = Empty
                                    | "=" + Value
                                    | "=" + S_LISTOF + S_BRACKET_LEFT + ValueList + S_BRACKET_RIGHT
                                    | "=" + S_SETOF  + S_BRACKET_LEFT + ValueList + S_BRACKET_RIGHT;

            #endregion

            #region ALTER TYPE/VERTEX

            alterStmt.Rule = S_ALTER + typeOrVertex + Id_simple + alterCmdList + uniquenessOpt + mandatoryOpt;

            alterCmd.Rule = Empty
                            | S_ADD         + S_ATTRIBUTES    + S_BRACKET_LEFT + AttributeList     + S_BRACKET_RIGHT
                            | S_DROP        + S_ATTRIBUTES    + S_BRACKET_LEFT + SimpleIdList      + S_BRACKET_RIGHT
                            | S_ADD         + S_BACKWARDEDGES + S_BRACKET_LEFT + BackwardEdgesList + S_BRACKET_RIGHT
                            | S_DROP        + S_BACKWARDEDGES + S_BRACKET_LEFT + SimpleIdList      + S_BRACKET_RIGHT
                            | S_ADD         + indexOnCreateType
                            | S_DROP        + IndexDropOnAlterType
                            | S_RENAME      + S_ATTRIBUTE     + Id_simple + S_TO + Id_simple
                            | S_RENAME      + S_BACKWARDEDGE  + Id_simple + S_TO + Id_simple
                            | S_RENAME      + S_TO + Id_simple
                            | S_DEFINE      + S_ATTRIBUTES + S_BRACKET_LEFT + AttributeList + S_BRACKET_RIGHT
                            | S_UNDEFINE    + S_ATTRIBUTES + S_BRACKET_LEFT + SimpleIdList + S_BRACKET_RIGHT 
                            | S_DROP        + S_UNIQUE
                            | S_DROP        + S_MANDATORY
                            | S_COMMENT     + "=" + string_literal;

            alterCmdList.Rule = MakePlusRule(alterCmdList, S_comma, alterCmd);

            IndexDropOnAlterTypeMember.Rule = S_BRACKET_LEFT + Id_simple + editionOpt + S_BRACKET_RIGHT;

            IndexDropOnAlterTypeMemberList.Rule = MakePlusRule(IndexDropOnAlterTypeMemberList, S_comma, IndexDropOnAlterTypeMember);

            IndexDropOnAlterType.Rule = S_INDICES + IndexDropOnAlterTypeMember
                                        | S_INDICES + S_BRACKET_LEFT + IndexDropOnAlterTypeMemberList + S_BRACKET_RIGHT;

            #endregion

            #region SELECT

            SelectStmtGraph.Rule = S_FROM + TypeList + S_SELECT + selList + whereClauseOpt + groupClauseOpt + havingClauseOpt + orderClauseOpt + MatchingClause + offsetOpt + limitOpt + resolutionDepthOpt + selectOutputOpt;
            SelectStmtGraph.Description = "The select statement is used to query the database and retrieve one or more types of objects in the database.\n";

            MatchingClause.Rule =       Empty
                                    |   MatchingClause + Matching;

            Matching.Rule =             S_MATCHES + S_BRACKET_LEFT + number + S_BRACKET_RIGHT + PrefixOperation;

            resolutionDepthOpt.Rule =       Empty
                                        |   S_DEPTH + number;

            selectOutputOpt.Rule    =       Empty
                                        |   "OUTPUT" + name;

            offsetOpt.Rule =       Empty
                            |   S_OFFSET + number;

            limitOpt.Rule =     Empty
                            |   S_LIMIT + number;

            selList.Rule =      selectionList;

            selectionList.Rule = MakePlusRule(selectionList, S_comma, selectionListElement);            

            selectionListElement.Rule =     S_ASTERISK 
                                        |   S_RHOMB 
                                        |   S_MINUS
                                        |   TERMINAL_LT 
                                        |   TERMINAL_GT
                                        |   selByType
                                        |   selectionSource;

            selByType.Rule = Empty
                            | S_AD + Id_simple;

            aliasOptName.Rule = Id_simple | string_literal;

            aliasOpt.Rule =     Empty
                            |   S_AS + aliasOptName;

            var staticSelect = new NonTerminal("staticSelect", CreateSelectValueAssignmentNode);
            staticSelect.Rule = Empty | S_EQUALS + Value | "?=" + Value;

            selectionSource.Rule = BNF_Aggregate + aliasOpt | IdOrFuncList + staticSelect + aliasOpt;
                //|   funcCall
                //|   Id;
                                

            #region Aggregate

            //BNF_Aggregate.Rule = BNF_AggregateName + S_BRACKET_LEFT + name + S_BRACKET_RIGHT;

            BNF_AggregateArg.Rule =   Id
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

            whereClauseOpt.Rule =       Empty 
                                    |   S_WHERE + BNF_Expression;

            groupClauseOpt.Rule =       Empty 
                                    |   "GROUP" + S_BY + idlist;

            havingClauseOpt.Rule =      Empty 
                                    |   "HAVING" + BNF_Expression;


            orderByAttributeListMember.Rule =       Id
                                                |   string_literal;

            orderByAttributeList.Rule = MakePlusRule(orderByAttributeList, S_comma, orderByAttributeListMember);

            orderClauseOpt.Rule =       Empty 
                                    |   "ORDER" + S_BY + orderByAttributeList + AttributeOrderDirectionOpt;

            #endregion

            #region INSERT

            InsertStmt.Rule = S_INSERT + S_INTO + TypeWrapper + insertValuesOpt;

            insertValuesOpt.Rule =      Empty
                                    |   S_VALUES + S_BRACKET_LEFT + AttrAssignList + S_BRACKET_RIGHT;

            AttrAssignList.Rule = MakePlusRule(AttrAssignList, S_comma, AttrAssign);

            AttrAssign.Rule =       Id + "=" + BNF_Expression
                                |   Id + "=" + Reference
                                |   Id + "=" + CollectionOfDBObjects;

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

            AttrUpdateOrAssign.Rule =       AttrAssign
                                        |   AttrRemove
                                        |   ListAttrUpdate;

            AttrRemove.Rule = S_REMOVE + S_ATTRIBUTES + S_BRACKET_LEFT + id_simpleList + S_BRACKET_RIGHT;

            ListAttrUpdate.Rule =       AddToListAttrUpdate
                                    |   RemoveFromListAttrUpdate;

            AddToListAttrUpdate.Rule =      AddToListAttrUpdateAddTo
                                        |   AddToListAttrUpdateOperator;

            AddToListAttrUpdateAddTo.Rule = S_ADD + S_TO + Id + CollectionOfDBObjects;
            AddToListAttrUpdateOperator.Rule = Id + S_ADDTOLIST + CollectionOfDBObjects;

            RemoveFromListAttrUpdate.Rule =         RemoveFromListAttrUpdateAddToRemoveFrom
                                            |       RemoveFromListAttrUpdateAddToOperator;

            RemoveFromListAttrUpdateAddToRemoveFrom.Rule = S_REMOVE + S_FROM + Id + tuple;
            RemoveFromListAttrUpdateAddToOperator.Rule = Id + S_REMOVEFROMLIST + tuple;


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

            SettingItemSetVal.Rule =        number 
                                        |   S_DEFAULT
                                        |   string_literal;

            SettingOpGet.Rule = S_GET + S_BRACKET_LEFT + SettingItems + S_BRACKET_RIGHT;

            SettingOpRemove.Rule = S_REMOVE + S_BRACKET_LEFT + SettingItems + S_BRACKET_RIGHT;

            SettingItems.Rule = MakePlusRule(SettingItems, S_comma, string_literal);

            #endregion

            #region DESCRIBE

            DescrInfoStmt.Rule = S_DESCRIBE + DescrArgument;

            DescrInfoStmt.Description = "This statement gives you all information about an type, a function, an index, an setting, an object, an edge or an aggregate.\n";

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

            insertorupdateStmt.Rule = S_INSERTORUPDATE + TypeWrapper + S_VALUES + S_BRACKET_LEFT + AttrAssignList + S_BRACKET_RIGHT + whereClauseOpt;

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

            dumpType.Rule           = Empty | S_ALL | S_GDDL | S_GDML;      // If empty => create both
            dumpFormat.Rule         = Empty | S_AS + S_GQL;                 // If empty => create GQL
            typeOptionalList.Rule   = Empty | BNF_TypesOrVertices + TypeList;

            dumpDestination.Rule    = Empty | S_INTO + location_literal | S_TO + location_literal;

            dumpStmt.Rule           = S_DUMP   + typeOptionalList + dumpType + dumpFormat + dumpDestination
                                    | S_EXPORT + typeOptionalList + dumpType + dumpFormat + dumpDestination;

            #endregion

            #region IMPORT

            paramComments.Rule      = S_COMMENTS + tuple | Empty;
            paramParallelTasks.Rule = S_PARALLELTASKS + "(" + number + ")" | Empty;
            verbosityTypes.Rule     = Symbol(VerbosityTypes.Silent.ToString()) | Symbol(VerbosityTypes.Errors.ToString()) | Symbol(VerbosityTypes.Full.ToString());
            verbosity.Rule          = S_VERBOSITY + verbosityTypes | Empty;

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

            RegisterPunctuation(",", S_BRACKET_LEFT.Symbol, S_BRACKET_RIGHT.Symbol, "[", "]");
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

        private void CreateTypeList(CompilerContext context, ParseTreeNode parseNode)
        {
            var node = new TypeListNode();

            node.GetContent(context, parseNode);

            parseNode.AstNode = node;
        }

        private void CreateIndexOnCreateType(CompilerContext context, ParseTreeNode parseNode)
        {
            var Node = new IndexOnCreateTypeNode();

            Node.GetContent(context, parseNode);

            parseNode.AstNode = Node;
        }

        private void CreateDropIndicesNode(CompilerContext context, ParseTreeNode parseNode)
        {
            var Node = new IndexDropOnAlterType();

            Node.GetContent(context, parseNode);

            parseNode.AstNode = Node;
        }

        private void CreateIndexOptOnCreateTypeMemberNode(CompilerContext context, ParseTreeNode parseNode)
        {
            var node = new IndexOptOnCreateTypeMemberNode();

            node.GetContent(context, parseNode);

            parseNode.AstNode = node;
        }

        private void CreateAttrAssignListNode(CompilerContext context, ParseTreeNode parseNode)
        {
            var attrAssignListNode = new AttrAssignListNode();

            attrAssignListNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)attrAssignListNode;
        }

        private void CreateUnExpressionNode(CompilerContext context, ParseTreeNode parseNode)
        {
            UnaryExpressionNode aUnExpressionNode = new UnaryExpressionNode();

            aUnExpressionNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aUnExpressionNode;
        }

        private void CreateGraphDBTypeNode(CompilerContext context, ParseTreeNode parseNode)
        {
            GraphDBTypeNode aGraphTypeNode = new GraphDBTypeNode();

            aGraphTypeNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aGraphTypeNode;
        }
 
        private void CreateBulkTypeListMemberNode(CompilerContext context, ParseTreeNode parseNode)
        {
            BulkTypeListMemberNode aBulkTypeListMemberNode = new BulkTypeListMemberNode();

            aBulkTypeListMemberNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aBulkTypeListMemberNode;
        }

        private void CreateBulkTypeNode(CompilerContext context, ParseTreeNode parseNode)
        {
            BulkTypeNode aBulkTypeNode = new BulkTypeNode();

            aBulkTypeNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aBulkTypeNode;
        }

        private void CreateReplaceStatementNode(CompilerContext context, ParseTreeNode parseNode)
        {
            ReplaceNode aReplaceNode = new ReplaceNode();

            aReplaceNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aReplaceNode;
        }

        private void CreateInsertOrReplaceStatementNode(CompilerContext context, ParseTreeNode parseNode)
        {
            InsertOrReplaceNode aInsertOrReplaceNode = new InsertOrReplaceNode();

            aInsertOrReplaceNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aInsertOrReplaceNode;
        }

        private void CreateInsertOrUpdateStatementNode(CompilerContext context, ParseTreeNode parseNode)
        {
            InsertOrUpdateNode aInsertOrUpdateNode = new InsertOrUpdateNode();

            aInsertOrUpdateNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aInsertOrUpdateNode;
        }        
        
        private void CreateCreateTypesStatementNode(CompilerContext context, ParseTreeNode parseNode)
        {

            CreateTypesNode aCreateTypesNode = new CreateTypesNode();

            aCreateTypesNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aCreateTypesNode;
        }

        private void CreateAttributeDefinitionNode(CompilerContext context, ParseTreeNode parseNode)
        {
            AttributeDefinitionNode aCreateAttributeNode = new AttributeDefinitionNode();

            aCreateAttributeNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aCreateAttributeNode;
        }

        private void CreateInsertStatementNode(CompilerContext context, ParseTreeNode parseNode)
        {

            InsertNode aInsertNode = new InsertNode();

            aInsertNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aInsertNode;
        }

        private void CreateCreateIndexStatementNode(CompilerContext context, ParseTreeNode parseNode)
        {

            CreateIndexNode aIndexNode = new CreateIndexNode();

            aIndexNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aIndexNode;

        }

        private void CreateIDNode(CompilerContext context, ParseTreeNode parseNode)
        {

            IDNode aIDNode = new IDNode();

            aIDNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aIDNode;

        }

        private void CreateDotDelimiter(CompilerContext context, ParseTreeNode parseNode)
        {
            SelectionDelimiterNode aDelimitter = new SelectionDelimiterNode();

            aDelimitter.GetContent(context, parseNode, KindOfDelimiter.Dot);

            parseNode.AstNode = (object)aDelimitter;
        }

        private void CreateEdgeInformation(CompilerContext context, ParseTreeNode parseNode)
        {
            EdgeInformationNode aEdgeInformation = new EdgeInformationNode();

            aEdgeInformation.GetContent(context, parseNode);

            parseNode.AstNode = (object)aEdgeInformation;
        }

        private void CreateEdgeTraversal(CompilerContext context, ParseTreeNode parseNode)
        {
            EdgeTraversalNode aEdgeTraversal = new EdgeTraversalNode();

            aEdgeTraversal.GetContent(context, parseNode);

            parseNode.AstNode = (object)aEdgeTraversal;
        }

        private void CreateEdgeAccessorDelimiter(CompilerContext context, ParseTreeNode parseNode)
        {
            SelectionDelimiterNode aDelimitter = new SelectionDelimiterNode();

            aDelimitter.GetContent(context, parseNode, KindOfDelimiter.EdgeInformationDelimiter);

            parseNode.AstNode = (object)aDelimitter;
        }

        private void CreateUpdateStatementNode(CompilerContext context, ParseTreeNode parseNode)
        {

            UpdateNode aUpdateNode = new UpdateNode();

            aUpdateNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aUpdateNode;

        }

        private void CreateBinaryExpressionNode(CompilerContext context, ParseTreeNode parseNode)
        {
            BinaryExpressionNode aNode = new BinaryExpressionNode();

            aNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aNode;

        }


        private void CreateWhereExpressionNode(CompilerContext context, ParseTreeNode parseNode)
        {
            WhereExpressionNode aWhereNode = new WhereExpressionNode();

            aWhereNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aWhereNode;
        }

        private void CreateSelectStatementNode(CompilerContext context, ParseTreeNode parseNode)
        {
            var aSelectStatementNode = new SelectNode();

            aSelectStatementNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aSelectStatementNode;
        }

        private void CreateAliasNode(CompilerContext context, ParseTreeNode parseNode)
        {
            var aliasNode = new AliasNode();

            aliasNode.GetContent(context, parseNode);

            parseNode.AstNode = aliasNode;
        }

        private void CreateSelectValueAssignmentNode(CompilerContext context, ParseTreeNode parseNode)
        {
            var selectValueAssignmentNode = new SelectValueAssignmentNode();

            selectValueAssignmentNode.GetContent(context, parseNode);

            parseNode.AstNode = selectValueAssignmentNode;
        }

        private void CreateSelByTypeNode(CompilerContext context, ParseTreeNode parseNode)
        {
            var aSelByTypeNode = new SelByTypeNode();

            aSelByTypeNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aSelByTypeNode;
        }

        private void CreateATypeNode(CompilerContext context, ParseTreeNode parseNode)
        {
            ATypeNode aATypeNode = new ATypeNode();

            aATypeNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aATypeNode;

            if (context.GraphListOfReferences == null)
                context.GraphListOfReferences = new List<TypeReferenceDefinition>();

            if (aATypeNode.ReferenceAndType.Reference != null && !(context.GraphListOfReferences as List<TypeReferenceDefinition>).Contains(aATypeNode.ReferenceAndType))
            {
                (context.GraphListOfReferences as List<TypeReferenceDefinition>).Add(aATypeNode.ReferenceAndType);
            }
        }

        private void CreateAlterStmNode(CompilerContext context, ParseTreeNode parseNode)
        {
            AlterTypeNode aAlterTypeStatementNode = new AlterTypeNode();

            aAlterTypeStatementNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aAlterTypeStatementNode;
        }

        private void CreatePartialSelectStmtNode(CompilerContext context, ParseTreeNode parseNode)
        {
            PartialSelectStmtNode aPartialSelectStmtNode = new PartialSelectStmtNode();

            aPartialSelectStmtNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aPartialSelectStmtNode;
        }

        private void CreateAggregateNode(CompilerContext context, ParseTreeNode parseNode)
        {
            AggregateNode aAggregateNode = new AggregateNode();

            aAggregateNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aAggregateNode;
        }

        private void CreateTypeListNode(CompilerContext context, ParseTreeNode parseNode)
        {
            TypeListNode aNode = new TypeListNode();

            aNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aNode;

        }

        #region Functions

        private void CreateFunctionCallNode(CompilerContext context, ParseTreeNode parseNode)
        {
            FuncCallNode functionCallNode = new FuncCallNode();

            functionCallNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)functionCallNode;
        }
        
        #endregion

        private void CreateDropTypeStmNode(CompilerContext context, ParseTreeNode parseNode)
        {
            DropTypeNode dropTypeNode = new DropTypeNode();

            dropTypeNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)dropTypeNode;
        }

        private void CreateDropIndexStmNode(CompilerContext context, ParseTreeNode parseNode)
        {
            DropIndexNode dropIndexNode = new DropIndexNode();

            dropIndexNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)dropIndexNode;
        }

        private void CreateTruncateStmNode(CompilerContext context, ParseTreeNode parseNode)
        {
            TruncateNode truncateNode = new TruncateNode();

            truncateNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)truncateNode;
        }

        private void CreateDeleteStatementNode(CompilerContext context, ParseTreeNode parseNode)
        {
            DeleteNode deleteNode = new DeleteNode();

            deleteNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)deleteNode;
        }

        #region Settings

        private void CreateSettingStatementNode(CompilerContext context, ParseTreeNode parseNode)
        {
            SettingNode settingNode = new SettingNode();

            settingNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)settingNode;
        }

        private void CreateSettingTypeNode(CompilerContext context, ParseTreeNode parseNode)
        {
            SettingTypeNode settingTypeNode = new SettingTypeNode();

            settingTypeNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)settingTypeNode;
        }

        private void CreateSettingAttrNode(CompilerContext context, ParseTreeNode parseNode)
        {
            SettingAttrNode settingAttrNode = new SettingAttrNode();

            settingAttrNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)settingAttrNode;
        }

        #endregion

        private void CreateBackwardEdgesNode(CompilerContext context, ParseTreeNode parseNode)
        {
            BackwardEdgesNode backwardEdgesNode = new BackwardEdgesNode();

            backwardEdgesNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)backwardEdgesNode;
        }

        private void CreateBackwardEdgeNode(CompilerContext context, ParseTreeNode parseNode)
        {
            BackwardEdgeNode backwardEdgeNode = new BackwardEdgeNode();

            backwardEdgeNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)backwardEdgeNode;
        }

        private void CreateEdgeTypeDefNode(CompilerContext context, ParseTreeNode parseNode)
        {
            EdgeTypeDefNode edgeTypeDefNode = new EdgeTypeDefNode();

            edgeTypeDefNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)edgeTypeDefNode;
        }

        private void CreateSingleEdgeTypeDefNode(CompilerContext context, ParseTreeNode parseNode)
        {
            SingleEdgeTypeDefNode edgeTypeDefNode = new SingleEdgeTypeDefNode();

            edgeTypeDefNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)edgeTypeDefNode;
        }

        #region Dump/Export

        private void CreateDumpNode(CompilerContext context, ParseTreeNode parseNode)
        {
            var dumpNode = new DumpNode();

            dumpNode.GetContent(context, parseNode);

            parseNode.AstNode = dumpNode;
        }

        private void CreateDumpTypeNode(CompilerContext context, ParseTreeNode parseNode)
        {
            var dumpTypeNode = new DumpTypeNode();

            dumpTypeNode.GetContent(context, parseNode);

            parseNode.AstNode = dumpTypeNode;
        }

        private void CreateDumpFormatNode(CompilerContext context, ParseTreeNode parseNode)
        {
            var dumpFormatNode = new DumpFormatNode();

            dumpFormatNode.GetContent(context, parseNode);

            parseNode.AstNode = dumpFormatNode;
        }

        #endregion

        private void CreateAddToListAttrUpdateAddToNode(CompilerContext context, ParseTreeNode parseNode)
        {
            var addToListAttrUpdateAddToNode = new AddToListAttrUpdateAddToNode();

            addToListAttrUpdateAddToNode.GetContent(context, parseNode);

            parseNode.AstNode = addToListAttrUpdateAddToNode;
        }

        private void CreateAddToListAttrUpdateOperatorNode(CompilerContext context, ParseTreeNode parseNode)
        {
            var addToListAttrUpdateOperatorNode = new AddToListAttrUpdateOperatorNode();

            addToListAttrUpdateOperatorNode.GetContent(context, parseNode);

            parseNode.AstNode = addToListAttrUpdateOperatorNode;
        }

        private void CreateRemoveFromListAttrUpdateAddToRemoveFromNode(CompilerContext context, ParseTreeNode parseNode)
        {
            var removeFromListAttrUpdateAddToRemoveFromNode = new RemoveFromListAttrUpdateAddToRemoveFromNode();

            removeFromListAttrUpdateAddToRemoveFromNode.GetContent(context, parseNode);

            parseNode.AstNode = removeFromListAttrUpdateAddToRemoveFromNode;
        }

        private void CreateRemoveFromListAttrUpdateAddToOperatorNode(CompilerContext context, ParseTreeNode parseNode)
        {
            var removeFromListAttrUpdateAddToOperatorNode = new RemoveFromListAttrUpdateAddToOperatorNode();

            removeFromListAttrUpdateAddToOperatorNode.GetContent(context, parseNode);

            parseNode.AstNode = removeFromListAttrUpdateAddToOperatorNode;
        }


        private void CreateAttrDefaultValueNode(CompilerContext context, ParseTreeNode parseNode)
        {
            var attrDefaultValueNode = new AttrDefaultValueNode();

            attrDefaultValueNode.GetContent(context, parseNode);

            parseNode.AstNode = attrDefaultValueNode;
        }

        #region RebuildIndices

        private void CreateRebuildIndicesNode(CompilerContext context, ParseTreeNode parseNode)
        {
            var rebuildIndicesNode = new RebuildIndicesNode();

            rebuildIndicesNode.GetContent(context, parseNode);

            parseNode.AstNode = rebuildIndicesNode;
        }

        #endregion

        #region Transaction

        private void CreateTransActionStatementNode(CompilerContext context, ParseTreeNode parseNode)
        {
            BeginTransactionNode beginTransactNode = new BeginTransactionNode();

            beginTransactNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)beginTransactNode;
        }

        private void CreateCommitRollbackTransActionNode(CompilerContext context, ParseTreeNode parseNode)
        {
            CommitRollbackTransactionNode commitRollBackNode = new CommitRollbackTransactionNode();

            commitRollBackNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)commitRollBackNode;
        }

        #endregion

        #region Describe

        private void CreateDescribeNode(CompilerContext context, ParseTreeNode parseNode)
        {
            DescribeNode DescrNode = new DescribeNode();

            DescrNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)DescrNode;
        }

        private void CreateDescrFunc(CompilerContext context, ParseTreeNode parseNode)
        {
            DescrFuncNode funcInfoNode = new DescrFuncNode();

            funcInfoNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)funcInfoNode;
        }

        private void CreateDescrFunctions(CompilerContext context, ParseTreeNode parseNode)
        {
            DescrFunctionsNode funcInfoNode = new DescrFunctionsNode();

            funcInfoNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)funcInfoNode;    
        }

        private void CreateDescrAggr(CompilerContext context, ParseTreeNode parseNode)
        {
            DescrAggrNode aggrInfoNode = new DescrAggrNode();

            aggrInfoNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aggrInfoNode;
        }

        private void CreateDescrAggrs(CompilerContext context, ParseTreeNode parseNode)
        {
            DescrAggrsNode aggrInfoNode = new DescrAggrsNode();

            aggrInfoNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aggrInfoNode;
        }

        private void CreateDescrSett(CompilerContext context, ParseTreeNode parseNode)
        {
            DescribeSettingNode settInfoNode = new DescribeSettingNode();

            settInfoNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)settInfoNode;
        }

        private void CreateDescrType(CompilerContext context, ParseTreeNode parseNode)
        {
            DescribeTypeNode typeInfoNode = new DescribeTypeNode();

            typeInfoNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)typeInfoNode;
        }

        private void CreateDescrTypes(CompilerContext context, ParseTreeNode parseNode)
        {
            DescribeTypesNode typeInfoNode = new DescribeTypesNode();

            typeInfoNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)typeInfoNode;        
        }

        private void CreateDescrIdx(CompilerContext context, ParseTreeNode parseNode)
        {
            DescribeIndexNode idxInfoNode = new DescribeIndexNode();

            idxInfoNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)idxInfoNode;
        }

        private void CreateDescrIdxs(CompilerContext context, ParseTreeNode parseNode)
        {
            DescribeIndicesNode idxInfoNode = new DescribeIndicesNode();

            idxInfoNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)idxInfoNode;
        }

        private void CreateDescrEdge(CompilerContext context, ParseTreeNode parseNode)
        { 
            DescribeEdgeNode edgeInfoNode = new DescribeEdgeNode();

            edgeInfoNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)edgeInfoNode;
        }

        private void CreateDescrEdges(CompilerContext context, ParseTreeNode parseNode)
        {
            DescribeEdgesNode edgeInfoNode = new DescribeEdgesNode();

            edgeInfoNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)edgeInfoNode;
        }

        private void CreateDescrSettItem(CompilerContext context, ParseTreeNode parseNode)
        {
            DescribeSettItemNode setItemInfoNode = new DescribeSettItemNode();

            setItemInfoNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)setItemInfoNode;
        }

        private void CreateDescrSettingsItems(CompilerContext context, ParseTreeNode parseNode)
        {
            DescribeSettingsItemsNode setItemInfoNode = new DescribeSettingsItemsNode();

            setItemInfoNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)setItemInfoNode;    
        }

        private void CreateDescrSettings(CompilerContext context, ParseTreeNode parseNode)
        {
            DescribeSettingsNode setItemInfoNode = new DescribeSettingsNode();

            setItemInfoNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)setItemInfoNode;    
        }
        #endregion

        #region Import

        private void CreateImportNode(CompilerContext context, ParseTreeNode parseNode)
        {
            var importNode = new ImportNode();

            importNode.GetContent(context, parseNode);

            parseNode.AstNode = importNode;
        }

        private void CreateParallelTaskNode(CompilerContext context, ParseTreeNode parseNode)
        {
            var parallelTaskNode = new ParallelTasksNode();

            parallelTaskNode.GetContent(context, parseNode);

            parseNode.AstNode = parallelTaskNode;
        }

        private void CreateCommentsNode(CompilerContext context, ParseTreeNode parseNode)
        {
            var commentsNode = new CommentsNode();

            commentsNode.GetContent(context, parseNode);

            parseNode.AstNode = commentsNode;
        }

        private void CreateVerbosityNode(CompilerContext context, ParseTreeNode parseNode)
        {
            var verbosityNode = new VerbosityNode();

            verbosityNode.GetContent(context, parseNode);

            parseNode.AstNode = verbosityNode;
        }

        #endregion

        #region link

        private void CreateLinkStmtNode(CompilerContext context, ParseTreeNode parseNode)
        {
            var linkNode = new LinkNode();

            linkNode.GetContent(context, parseNode);

            parseNode.AstNode = linkNode;
        }

        #endregion

        #region unlink

        private void CreateUnlinkStmt(CompilerContext context, ParseTreeNode parseNode)
        {
            var unlinkNode = new UnlinkNode();

            unlinkNode.GetContent(context, parseNode);

            parseNode.AstNode = unlinkNode;
        }

        #endregion

        #endregion

        #region IDumpable Members

        #region Export GraphDDL

        public Exceptional<List<String>> ExportGraphDDL(DumpFormats myDumpFormat, DBContext myDBContext, IEnumerable<GraphDBType> myTypesToDump)
        {
            
            var stringBuilder = new StringBuilder(String.Concat(S_CREATE.ToUpperString(), " ", S_TYPES.ToUpperString(), " "));
            var delimiter = ", ";

            foreach (var _GraphDBType in myTypesToDump)
            {
                stringBuilder.Append(String.Concat(CreateGraphDDL(myDumpFormat, _GraphDBType, myDBContext), delimiter));
            }

            var retString = stringBuilder.ToString();

            if (retString.EndsWith(delimiter))
            {
                retString = retString.Substring(0, retString.Length - delimiter.Length);
            }

            return new Exceptional<List<String>>(new List<String>() { retString });

        }

        private String CreateGraphDDL(DumpFormats myDumpFormat, GraphDBType myGraphDBType, DBContext myDBContext)
        {

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("{0} ", myGraphDBType.Name);

            if (myGraphDBType.ParentTypeUUID != null)
            {

                stringBuilder.AppendFormat("{0} {1} ", S_EXTENDS.ToUpperString(), myGraphDBType.GetParentType(myDBContext.DBTypeManager).Name);//builder.AppendLine();

                #region Not backwardEdge attributes

                if (myGraphDBType.GetFilteredAttributes(ta => !ta.IsBackwardEdge).CountIsGreater(0))
                {
                    stringBuilder.Append(S_ATTRIBUTES.ToUpperString() + S_BRACKET_LEFT.ToUpperString() + CreateGraphDDLOfAttributes(myDumpFormat, myGraphDBType.GetFilteredAttributes(ta => !ta.IsBackwardEdge), myDBContext) + S_BRACKET_RIGHT.ToUpperString() + " ");
                }

                #endregion

                #region BackwardEdge attributes

                if (myGraphDBType.GetFilteredAttributes(ta => ta.IsBackwardEdge).CountIsGreater(0))
                {
                    stringBuilder.Append(S_BACKWARDEDGES.ToUpperString() + S_BRACKET_LEFT.ToUpperString() + CreateGraphDDLOfBackwardEdges(myDumpFormat, myGraphDBType.GetFilteredAttributes(ta => ta.IsBackwardEdge), myDBContext) + S_BRACKET_RIGHT.ToUpperString() + " ");
                }

                #endregion

                #region Uniques

                if (myGraphDBType.GetUniqueAttributes().CountIsGreater(0))
                {
                    stringBuilder.Append(S_UNIQUE.ToUpperString() + S_BRACKET_LEFT.Symbol + CreateGraphDDLOfAttributeUUIDs(myDumpFormat, myGraphDBType.GetUniqueAttributes(), myGraphDBType) + S_BRACKET_RIGHT.Symbol + " ");
                }

                #endregion

                #region Mandatory attributes

                if (myGraphDBType.GetMandatoryAttributes().CountIsGreater(0))
                {
                    stringBuilder.Append(S_MANDATORY.ToUpperString() + S_BRACKET_LEFT.Symbol + CreateGraphDDLOfAttributeUUIDs(myDumpFormat, myGraphDBType.GetMandatoryAttributes(), myGraphDBType) + S_BRACKET_RIGHT.Symbol + " ");
                }

                #endregion

                #region Indices

                if (myGraphDBType.GetAllAttributeIndices(false).CountIsGreater(0))
                {
                    stringBuilder.Append(S_INDICES.ToUpperString() + S_BRACKET_LEFT.Symbol + CreateGraphDDLOfIndices(myDumpFormat, myGraphDBType.GetAllAttributeIndices(false), myGraphDBType) + S_BRACKET_RIGHT.Symbol + " ");
                }

                #endregion

            }

            return stringBuilder.ToString();

        }

        private String CreateGraphDDLOfAttributes(DumpFormats myDumpFormat, IEnumerable<TypeAttribute> myTypeAttributes, DBContext myDBContext)
        {

            var stringBuilder = new StringBuilder();
            var delimiter = ", ";

            foreach (var _Attribute in myTypeAttributes)
            {
                stringBuilder.Append(CreateGraphDDLOfAttributeDefinition(myDumpFormat, _Attribute, myDBContext));
                stringBuilder.Append(delimiter);
            }

            if (stringBuilder.Length > delimiter.Length)
            {
                stringBuilder.Remove(stringBuilder.Length - delimiter.Length, 2);
            }

            return stringBuilder.ToString();

        }

        private String CreateGraphDDLOfAttributeDefinition(DumpFormats myDumpFormat, TypeAttribute myTypeAttribute, DBContext myDBContext)
        {

            if (myTypeAttribute.EdgeType != null)
            {
                return String.Concat(myTypeAttribute.EdgeType.GetGDDL(myTypeAttribute.GetDBType(myDBContext.DBTypeManager)), " ", myTypeAttribute.Name);
            }
            else
            {
                return String.Concat(myTypeAttribute.GetDBType(myDBContext.DBTypeManager).Name, " ", myTypeAttribute.Name);
            }

        }

        private String CreateGraphDDLOfBackwardEdges(DumpFormats myDumpFormat, IEnumerable<TypeAttribute> myTypeAttributes, DBContext myDBContext)
        {

            var stringBuilder = new StringBuilder();
            var delimiter = ", ";

            foreach (var _Attribute in myTypeAttributes)
            {
                var typeAttrInfos = _Attribute.BackwardEdgeDefinition.GetTypeAndAttributeInformation(myDBContext.DBTypeManager);
                stringBuilder.Append(String.Concat(typeAttrInfos.Item1.Name, ".", typeAttrInfos.Item2.Name, " ", _Attribute.Name));
                stringBuilder.Append(delimiter);
            }

            if (stringBuilder.Length > delimiter.Length)
            {
                stringBuilder.Remove(stringBuilder.Length - delimiter.Length, 2);
            }

            return stringBuilder.ToString();

        }

        /// <summary>
        /// Add just the Attribute names
        /// </summary>
        /// <param name="myDumpFormat"></param>
        /// <param name="typeAttribute"></param>
        /// <param name="indent"></param>
        /// <param name="indentWidth"></param>
        /// <returns></returns>
        private String CreateGraphDDLOfAttributeUUIDs(DumpFormats myDumpFormat, IEnumerable<AttributeUUID> myAttributes, GraphDBType myGraphDBType)
        {

            var stringBuilder = new StringBuilder();
            var delimiter = ", ";

            foreach (var _Attribute in myAttributes)
            {
                stringBuilder.Append(myGraphDBType.GetTypeAttributeByUUID(_Attribute).Name);
                stringBuilder.Append(delimiter);
            }

            if (stringBuilder.Length > delimiter.Length)
            {
                stringBuilder.Remove(stringBuilder.Length - delimiter.Length, 2);
            }

            return stringBuilder.ToString();

        }

        /// <summary>
        /// Create the DDL for attributeIndices
        /// </summary>
        /// <param name="myDumpFormat"></param>
        /// <param name="myAttributeIndices"></param>
        /// <param name="indent"></param>
        /// <param name="indentWidth"></param>
        /// <returns></returns>
        private String CreateGraphDDLOfIndices(DumpFormats myDumpFormat, IEnumerable<AAttributeIndex> myAttributeIndices, GraphDBType myGraphDBType)
        {

            var _StringBuilder = new StringBuilder();
            var _Delimiter     = ", ";

            foreach (var _AttributeIndex in myAttributeIndices)
            {

                if (_AttributeIndex is UUIDIndex || _AttributeIndex.IndexEdition == DBConstants.UNIQUEATTRIBUTESINDEX)
                    continue;

                _StringBuilder.Append(String.Concat(S_BRACKET_LEFT, _AttributeIndex.IndexName));

                if (_AttributeIndex.IsUniqueIndex)
                {
                    _StringBuilder.Append(String.Concat(" ", S_UNIQUE.ToUpperString()));
                }

                _StringBuilder.Append(String.Concat(" ", S_EDITION.ToUpperString(), " ", _AttributeIndex.IndexEdition));

                _StringBuilder.Append(String.Concat(" ", S_INDEXTYPE.ToUpperString(), " ", _AttributeIndex.IndexType.ToString()));
                _StringBuilder.Append(String.Concat(" ", S_ON.ToUpperString(), " " + S_ATTRIBUTES.ToUpperString(), " ", CreateGraphDDLOfAttributeUUIDs(myDumpFormat, _AttributeIndex.IndexKeyDefinition.IndexKeyAttributeUUIDs, myGraphDBType)));

                _StringBuilder.Append(S_BRACKET_RIGHT);

                _StringBuilder.Append(_Delimiter);

            }

            if (_StringBuilder.Length > _Delimiter.Length)
            {
                _StringBuilder.Remove(_StringBuilder.Length - _Delimiter.Length, 2);
            }

            return _StringBuilder.ToString();

        }

        #endregion

        #region Export GraphDML

        /// <summary>
        /// Create the GraphDML of all DBObjects in the database.
        /// </summary>
        /// <param name="myDumpFormat"></param>
        /// <param name="dbContext"></param>
        /// <param name="objectManager"></param>
        /// <returns></returns>
        public Exceptional<List<String>> ExportGraphDML(DumpFormats myDumpFormat, DBContext dbContext, IEnumerable<GraphDBType> myTypesToDump)
        {

            //var _StringBuilder  = new StringBuilder();
            var queries           = new List<String>();
            var exceptional       = new Exceptional<List<String>>();

            #region Go through each type

            foreach (var graphDBType in myTypesToDump)
            {

                var UUIDIdx = graphDBType.GetUUIDIndex(dbContext.DBTypeManager);

                #region Take UUID index

                foreach (var aDBO in dbContext.DBObjectCache.LoadListOfDBObjectStreams(graphDBType, UUIDIdx.GetAllUUIDs(graphDBType, dbContext)))
                {

                    if (!aDBO.Success())
                    {
                        exceptional.PushIExceptional(aDBO);
                    }
                    else
                    {

                        var gdmlExceptional = CreateGraphDMLforDBObject(myDumpFormat, dbContext, graphDBType, aDBO.Value);

                        if (!gdmlExceptional.Success())
                        {
                            exceptional.PushIExceptional(aDBO);
                        }
                        else
                        {
                            queries.Add(gdmlExceptional.Value);
                        }

                    }
                }

                #endregion

            }

            #endregion

            //_Exceptional.Value = _StringBuilder.ToString();
            exceptional.Value = queries;

            return exceptional;

        }

        private Exceptional<String> CreateGraphDMLforDBObject(DumpFormats myDumpFormat, DBContext myDBContext, GraphDBType myGraphDBType, DBObjectStream myDBObjectStream)
        {

            var stringBuilder = new StringBuilder();
            var delimiter = ", ";

            stringBuilder.Append(String.Concat(S_INSERT.ToUpperString(), " ", S_INTO.ToUpperString(), " ", myGraphDBType.Name, " ", S_VALUES.ToUpperString(), " ", S_BRACKET_LEFT));
            stringBuilder.Append(String.Concat(S_UUID.ToUpperString(), " = '", myDBObjectStream.ObjectUUID.ToString(), "'", delimiter));

            #region CreateGraphDMLforDBODefinedAttributes

            var defAttrsDML = CreateGraphDMLforDBObjectDefinedAttributes(myDumpFormat, myDBObjectStream.GetAttributes(), myGraphDBType, myDBObjectStream, myDBContext);

            if (!defAttrsDML.Success())
            {
                return defAttrsDML;
            }

            stringBuilder.Append(defAttrsDML.Value);
            
            #endregion

            #region CreateGDMLforDBOUnDefinedAttributes

            var undefAttrs = myDBObjectStream.GetUndefinedAttributePayload(myDBContext.DBObjectManager);

            if (!undefAttrs.Success())
            {
                return new Exceptional<String>(undefAttrs);
            }

            if (undefAttrs.Value.Count > 0)
            {

                Exceptional<String> undefAttrsDML = CreateGraphDMLforDBObjectUndefinedAttributes(myDumpFormat, undefAttrs.Value, myGraphDBType, myDBObjectStream);

                if (!undefAttrsDML.Success())
                {
                    return undefAttrsDML;
                }

                stringBuilder.Append(undefAttrsDML.Value);

            }

            #endregion

            stringBuilder.RemoveSuffix(delimiter);
            stringBuilder.Append(S_BRACKET_RIGHT);

            return new Exceptional<String>(stringBuilder.ToString());

        }

        private Exceptional<String> CreateGraphDMLforDBObjectDefinedAttributes(DumpFormats myDumpFormat, IDictionary<AttributeUUID, IObject> myAttributes, GraphDBType myGraphDBType, DBObjectStream myDBObjectStream, DBContext myDBContext)
        {

            var stringBuilder = new StringBuilder();
            var delimiter = ", ";

            foreach (var attribute in myAttributes)
            {

                if (attribute.Value == null)
                {
                    continue;
                }

                var typeAttribute = myGraphDBType.GetTypeAttributeByUUID(attribute.Key);

                #region Reference attributes

                if (typeAttribute.GetDBType(myDBContext.DBTypeManager).IsUserDefined)
                {

                    #region IReferenceEdge

                    if (attribute.Value is ASetOfReferencesEdgeType)
                    {

                        #region Create edge GDML

                        stringBuilder.Append(String.Concat(typeAttribute.Name, " = ", S_SETOFUUIDS.ToUpperString(), " ", S_BRACKET_LEFT));

                        //myEdgeBuilder.Append(String.Concat(typeAttribute.Name, " = ", S_SETOF.ToUpperString(), " ", S_BRACKET_LEFT));

                        #region Create an assignment content - if edge does not contain any elements create an empty one

                        if ((attribute.Value as ASetOfReferencesEdgeType).GetAllReferenceIDs().CountIsGreater(0))
                        {

                            if (attribute.Value is ASetOfReferencesWithInfoEdgeType)
                            {

                                #region Create attribute assignments

                                foreach (var val in (attribute.Value as ASetOfReferencesWithInfoEdgeType).GetAllReferenceIDsWeighted())
                                {
                                    stringBuilder.Append(String.Concat("'", val.Item1.ToString(), "'"));
                                    if (val.Item2 != null)
                                    {
                                        stringBuilder.Append(String.Concat(S_colon, S_BRACKET_LEFT, CreateGraphDMLforADBBaseObject(myDumpFormat, val.Item2), S_BRACKET_RIGHT));
                                    }
                                    stringBuilder.Append(delimiter);
                                }
                                stringBuilder.RemoveSuffix(delimiter);

                                #endregion

                            }
                            else
                            {

                                #region Create an assignment content - if edge does not contain any elements create an empty one

                                if ((attribute.Value as ASetOfReferencesEdgeType).GetAllReferenceIDs().CountIsGreater(0))
                                {

                                    #region Create attribute assignments

                                    foreach (var val in (attribute.Value as ASetOfReferencesEdgeType).GetAllReferenceIDs())
                                    {
                                        stringBuilder.Append(String.Concat("'", val.ToString(), "'"));
                                        stringBuilder.Append(delimiter);
                                    }
                                    stringBuilder.RemoveSuffix(delimiter);

                                    #endregion

                                }

                                #endregion

                            }

                        }

                        #endregion

                        stringBuilder.Append(S_BRACKET_RIGHT);

                        #endregion

                    }

                    #endregion

                    #region SingleReference

                    else if (typeAttribute.KindOfType == KindsOfType.SingleReference)
                    {
                        stringBuilder.Append(String.Concat(typeAttribute.Name, " = ", S_REFUUID.ToUpperString(), " ", S_BRACKET_LEFT));
                        stringBuilder.Append(String.Concat("'", (attribute.Value as ASingleReferenceEdgeType).GetUUID().ToString(), "'"));
                        stringBuilder.Append(S_BRACKET_RIGHT);
                    }

                    #endregion

                    else
                    {
                        return new Exceptional<String>(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                    }

                    stringBuilder.Append(delimiter);

                }

                #endregion

                #region NonReference attributes

                else
                {

                    #region ListOfNoneReferences

                    if (typeAttribute.KindOfType == KindsOfType.ListOfNoneReferences)
                    {
                        stringBuilder.Append(String.Concat(typeAttribute.Name, " = ", S_LISTOF.ToUpperString(), " ", S_BRACKET_LEFT));
                        foreach (var val in (attribute.Value as IBaseEdge))
                        {
                            stringBuilder.Append(CreateGraphDMLforADBBaseObject(myDumpFormat, val as ADBBaseObject) + delimiter);
                        }
                        stringBuilder.RemoveSuffix(delimiter);
                        stringBuilder.Append(S_BRACKET_RIGHT);
                    }

                    #endregion

                    #region SetOfNoneReferences

                    else if (typeAttribute.KindOfType == KindsOfType.SetOfNoneReferences)
                    {
                        stringBuilder.Append(String.Concat(typeAttribute.Name, " = ", S_SETOF.ToUpperString(), " ", S_BRACKET_LEFT));
                        foreach (var val in (attribute.Value as IBaseEdge))
                        {
                            stringBuilder.Append(CreateGraphDMLforADBBaseObject(myDumpFormat, val as ADBBaseObject) + delimiter);
                        }
                        stringBuilder.RemoveSuffix(delimiter);
                        stringBuilder.Append(S_BRACKET_RIGHT);

                    }

                    #endregion

                    #region SpecialAttribute

                    else if (typeAttribute.KindOfType == KindsOfType.SpecialAttribute)
                    {
                        throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                    }

                    #endregion

                    #region Single value

                    else
                    {
                        stringBuilder.Append(String.Concat(typeAttribute.Name, " = ", CreateGraphDMLforADBBaseObject(myDumpFormat, attribute.Value as ADBBaseObject)));
                    }

                    #endregion

                    stringBuilder.Append(delimiter);

                }

                #endregion

            }

            return new Exceptional<String>(stringBuilder.ToString());

        }

        private Exceptional<String> CreateGraphDMLforDBObjectUndefinedAttributes(DumpFormats myDumpFormat, IDictionary<String, IObject> myAttributes, GraphDBType myGraphDBType, DBObjectStream myDBObjectStream)
        {

            var stringBuilder = new StringBuilder();
            var delimiter = ", ";

            foreach (var attribute in myAttributes)
            {

                #region A single value...

                if (attribute.Value is ADBBaseObject)
                {
                    stringBuilder.Append(String.Concat(attribute.Key, " = ", CreateGraphDMLforADBBaseObject(myDumpFormat, attribute.Value as ADBBaseObject)));
                }

                #endregion

                #region ..or, it is a List or Set, since the Set constraint was already verified we can use a list

                else if (attribute.Value is IBaseEdge)
                {

                    stringBuilder.Append(String.Concat(attribute.Key, " = ", S_LISTOF.ToUpperString(), " ", S_BRACKET_LEFT));

                    foreach (var val in (attribute.Value as IBaseEdge))
                    {
                        stringBuilder.Append(CreateGraphDMLforADBBaseObject(myDumpFormat, val as ADBBaseObject) + delimiter);
                    }

                    stringBuilder.RemoveSuffix(delimiter);
                    stringBuilder.Append(S_BRACKET_RIGHT);

                }

                #endregion

                else
                {
                    return new Exceptional<String>(new Error_NotImplemented(new StackTrace(true)));
                }

                stringBuilder.Append(delimiter);

            }

            return new Exceptional<String>(stringBuilder.ToString());

        }

        private String CreateGraphDMLforADBBaseObject(DumpFormats myDumpFormat, ADBBaseObject myADBBaseObject)
        {

            var dbNumber = myADBBaseObject as DBNumber;

            if (dbNumber != null)
                return dbNumber.ToString(new CultureInfo("en-US"));

            return String.Concat("'", myADBBaseObject.ToString(new CultureInfo("en-US")), "'");

        }

        #endregion

        #endregion   
    
        #region IExtendableGrammar Members

        public void SetAggregates(IEnumerable<ABaseAggregate> aggregates)
        {
            
            #region Add all plugins to the grammar

            if (aggregates.IsNullOrEmpty())
            {
                /// empty is not the best solution, Maybe: remove complete rule if no importer exist
                BNF_Aggregate.Rule = Empty;
            }
            else
            {
                foreach (var aggr in aggregates)
                {
                    //BNF_AggregateName + S_BRACKET_LEFT + aggregateArg + S_BRACKET_RIGHT;

                    var aggrRule = new NonTerminal("aggr_" + aggr.FunctionName, CreateAggregateNode);
                    aggrRule.Rule = aggr.FunctionName + S_BRACKET_LEFT + BNF_AggregateArg + S_BRACKET_RIGHT;

                    if (BNF_Aggregate.Rule == null)
                    {
                        BNF_Aggregate.Rule = aggrRule;
                    }
                    else
                    {
                        BNF_Aggregate.Rule |= aggrRule;
                    }
                }
            }

            #endregion

        }

        public void SetFunctions(IEnumerable<ABaseFunction> functions)
        {

            #region Add all plugins to the grammar

            if (functions.IsNullOrEmpty())
            {
                /// empty is not the best solution, Maybe: remove complete rule if no importer exist
                BNF_FuncCall.Rule = Empty;
            }
            else
            {
                foreach (var func in functions)
                {

                    #region Create funcNonTerminal

                    var funcNonTerminal = new NonTerminal("func" + func.FunctionName, CreateFunctionCallNode);

                    var funcParams = func.GetParameters();
                    if (funcParams.IsNullOrEmpty())
                    {
                        funcNonTerminal.Rule = func.FunctionName + S_BRACKET_LEFT + S_BRACKET_RIGHT;
                        
                    }
                    else
                    {

                        #region Do not add the arguments to the grammar - currently there is an mystic NT1 child for INSERT func

                        /* Do not add the arguments to the grammar - currently there is an mystic NT1 child for INSERT func
                        var funcArgsNonTerminal = new NonTerminal("funcArgs" + func.FunctionName);
                        funcArgsNonTerminal.Rule = BNF_ExprList;
                        foreach (var param in funcParams.SkipULong(1))
                        {
                            if (param.VariableNumOfParams)
                            {
                                funcArgsNonTerminal.Rule = BNF_ExprList;
                            }
                            else
                            {
                                funcArgsNonTerminal.Rule += S_comma + BNF_Expression;
                            }
                        }
                        funcArgsNonTerminal.SetOption(TermOptions.IsTransient);
                        */

                        #endregion

                        funcNonTerminal.Rule = func.FunctionName + S_BRACKET_LEFT + BNF_FunArgs + S_BRACKET_RIGHT;
                    }

                    #endregion

                    #region Add funcNonTerminal to the BNF_FuncCall

                    if (BNF_FuncCall.Rule == null)
                    {
                        BNF_FuncCall.Rule = funcNonTerminal;
                    }
                    else
                    {
                        BNF_FuncCall.Rule |= funcNonTerminal;
                    }
                    
                    #endregion

                }
            }

            #endregion

        }

        public void SetOperators(IEnumerable<ABinaryOperator> operators)
        {
            /* At first, the enums must be removed from operators - lot of impacts....
            #region Add all plugins to the grammar

            if (operators.IsNullOrEmpty())
            {
                /// empty is not the best solution, Maybe: remove complete import rule if no importer exist
                BNF_IndexTypeOpt.Rule = Empty;
            }
            else
            {
                foreach (var op in operators)
                {
                    if (BNF_IndexTypeOpt.Rule == null)
                    {
                        BNF_IndexTypeOpt.Rule = S_INDEXTYPE + Symbol(op.IndexName);
                    }
                    else
                    {
                        BNF_IndexTypeOpt.Rule |= S_INDEXTYPE + Symbol(op.IndexName);
                    }
                }
            }

            #endregion
            */
        }

        public void SetSettings(IEnumerable<ADBSettingsBase> settings)
        {
            throw new NotImplementedException();
        }

        public void SetEdges(IEnumerable<IEdgeType> edges)
        {
            throw new NotImplementedException();
        }

        public void SetIndices(IEnumerable<IVersionedIndexObject<IndexKey, ObjectUUID>> indices)
        {

            #region Add all plugins to the grammar

            if (indices.IsNullOrEmpty())
            {
                /// empty is not the best solution, Maybe: remove complete import rule if no importer exist
                BNF_IndexTypeOpt.Rule = Empty;
            }
            else
            {
                foreach (var idx in indices)
                {
                    if (BNF_IndexTypeOpt.Rule == null)
                    {
                        BNF_IndexTypeOpt.Rule = S_INDEXTYPE + Symbol(idx.IndexName);
                    }
                    else
                    {
                        BNF_IndexTypeOpt.Rule |= S_INDEXTYPE + Symbol(idx.IndexName);
                    }
                }
            }

            #endregion

        }

        public void SetGraphDBImporter(IEnumerable<AGraphDBImport> graphDBImporter)
        {

            #region Add all plugins to the grammar

            if (graphDBImporter.IsNullOrEmpty())
            {
                /// empty is not the best solution, Maybe: remove complete import rule if no importer exist
                BNF_ImportFormat.Rule = Empty;
                BNF_ImportStmt.Rule = Empty;
            }
            else
            {
                foreach (var importer in graphDBImporter)
                {
                    if (BNF_ImportFormat.Rule == null)
                    {
                        BNF_ImportFormat.Rule = Symbol(importer.ImportFormat);
                    }
                    else
                    {
                        BNF_ImportFormat.Rule |= Symbol(importer.ImportFormat);
                    }
                }
            }

            #endregion

        }

        #endregion

    }

}
