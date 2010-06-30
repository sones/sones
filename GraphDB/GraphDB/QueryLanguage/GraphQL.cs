/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
*
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
*/

/* 
 * Copyright (c) sones GmbH 2007-2010
 * <developer>Henning Rauch</developer>
 * <developer>Stefan Licht</developer>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

using sones.GraphDB.Exceptions;
using sones.GraphDB.Indices;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Statements;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Statements.Drop;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Statements.Dump;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Statements.InsertOrReplace;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Statements.Replace;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Statements.Select;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Statements.Setting;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Statements.Transaction;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.Structures.EdgeTypes;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.Lib;
using sones.Lib.ErrorHandling;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.Lib.Frameworks.Irony.Scripting.Ast;

#endregion

namespace sones.GraphDB.QueryLanguage
{

    /// <summary>
    /// This class defines the GraphQueryLanguage.
    /// </summary>
    public class GraphQL : Grammar, IDumpable
    {

        #region Consts

        public const String DOT = ".";
        public const String TERMINAL_BRACKET_LEFT = "(";
        public const String TERMINAL_BRACKET_RIGHT = ")";
        public const String TERMINAL_QUEUESIZE  = "QUEUESIZE";
        public const String TERMINAL_WEIGHTED   = "WEIGHTED";
        public const String TERMINAL_UNIQUE     = "UNIQUE";
        public const String TERMINAL_MANDATORY  = "MANDATORY";
        public const String TERMINAL_SORTED     = "SORTED";        
        public const String TERMINAL_ASC        = "ASC";
        public const String TERMINAL_DESC       = "DESC";
        public const String TERMINAL_TRUE       = "TRUE";
        public const String TERMINAL_FALSE      = "FALSE";
        public const String TERMINAL_LIST       = "LIST";
        public const String TERMINAL_SET        = "SET";
        public const String TERMINAL_LT         = "<";
        public const String TERMINAL_GT         = ">";
        
        #endregion

        #region Properties

        private SymbolTerminal _S_CREATE;
        public SymbolTerminal   S_CREATE
        {
            get { return _S_CREATE; }
        }

        private SymbolTerminal _S_comma;
        public SymbolTerminal   S_comma
        {
            get { return _S_comma; }
        }

        private SymbolTerminal _S_dot;
        public SymbolTerminal   S_dot
        {
            get { return _S_dot; }
        }
        private SymbolTerminal _S_colon;
        public SymbolTerminal   S_colon
        {
            get { return _S_colon; }
        }

        #region Brackets

        private SymbolTerminal _S_BRACKET_LEFT;
        public SymbolTerminal S_BRACKET_LEFT
        {
            get { return _S_BRACKET_LEFT; }
        }
        private SymbolTerminal _S_BRACKET_RIGHT;
        public SymbolTerminal S_BRACKET_RIGHT
        {
            get { return _S_BRACKET_RIGHT; }
        }
        private SymbolTerminal _S_TUPLE_BRACKET_LEFT;
        public SymbolTerminal S_TUPLE_BRACKET_LEFT
        {
            get { return _S_TUPLE_BRACKET_LEFT; }
        }
        private SymbolTerminal _S_TUPLE_BRACKET_RIGHT;
        public SymbolTerminal S_TUPLE_BRACKET_RIGHT
        {
            get { return _S_TUPLE_BRACKET_RIGHT; }
        }
        public SymbolTerminal S_TUPLE_BRACKET_LEFT_EXCLUSIVE
        {
            get { return _S_BRACKET_LEFT; }
        }
        public SymbolTerminal S_TUPLE_BRACKET_RIGHT_EXCLUSIVE
        {
            get { return _S_BRACKET_RIGHT; }
        }

        #endregion

        private SymbolTerminal _S_edgeInformationDelimiterSymbol;
        public SymbolTerminal   S_edgeInformationDelimiterSymbol
        {
            get { return _S_edgeInformationDelimiterSymbol; }
        }
        private SymbolTerminal _S_edgeTraversalDelimiter;
        public SymbolTerminal   S_edgeTraversalDelimiter
        {
            get { return _S_edgeTraversalDelimiter; }
        }
        private SymbolTerminal _S_NULL;
        public SymbolTerminal   S_NULL
        {
            get { return _S_NULL; }
        }
        private SymbolTerminal _S_NOT;
        public SymbolTerminal   S_NOT
        {
            get { return _S_NOT; }
        }
        private SymbolTerminal _S_UNIQUE;
        public SymbolTerminal   S_UNIQUE
        {
            get { return _S_UNIQUE; }
        }
        private SymbolTerminal _S_WITH;
        public SymbolTerminal   S_WITH
        {
            get { return _S_WITH; }
        }
        private SymbolTerminal _S_TABLE;
        public SymbolTerminal   S_TABLE
        {
            get { return _S_TABLE; }
        }
        private SymbolTerminal _S_ALTER;
        public SymbolTerminal   S_ALTER
        {
            get { return _S_ALTER; }
        }
        private SymbolTerminal _S_ADD;
        public SymbolTerminal   S_ADD
        {
            get { return _S_ADD; }
        }
        private SymbolTerminal _S_TO;
        public SymbolTerminal   S_TO
        {
            get { return _S_TO; }
        }
        private SymbolTerminal _S_COLUMN;
        public SymbolTerminal   S_COLUMN
        {
            get { return _S_COLUMN; }
        }
        private SymbolTerminal _S_DROP;
        public SymbolTerminal   S_DROP
        {
            get { return _S_DROP; }
        }
        private SymbolTerminal _S_RENAME;
        public SymbolTerminal   S_RENAME
        {
            get { return _S_RENAME; }
        }
        private SymbolTerminal _S_CONSTRAINT;
        public SymbolTerminal   S_CONSTRAINT
        {
            get { return _S_CONSTRAINT; }
        }
        private SymbolTerminal _S_INDEX;
        public SymbolTerminal   S_INDEX
        {
            get { return _S_INDEX; }
        }
        private SymbolTerminal _S_INDICES;
        public SymbolTerminal   S_INDICES
        {
            get { return _S_INDICES; }
        }
        private SymbolTerminal _S_ON;
        public SymbolTerminal   S_ON
        {
            get { return _S_ON; }
        }
        private SymbolTerminal _S_KEY;
        public SymbolTerminal   S_KEY
        {
            get { return _S_KEY; }
        }
        private SymbolTerminal _S_PRIMARY;
        public SymbolTerminal   S_PRIMARY
        {
            get { return _S_PRIMARY; }
        }
        private SymbolTerminal _S_INSERT;
        public SymbolTerminal   S_INSERT
        {
            get { return _S_INSERT; }
        }
        private SymbolTerminal _S_INTO;
        public SymbolTerminal   S_INTO
        {
            get { return _S_INTO; }
        }
        private SymbolTerminal _S_UPDATE;
        public SymbolTerminal   S_UPDATE
        {
            get { return _S_UPDATE; }
        }
        private SymbolTerminal _S_INSERTORUPDATE;
        public SymbolTerminal   S_INSERTORUPDATE
        {
            get { return _S_INSERTORUPDATE; }
        }
        private SymbolTerminal _S_INSERTORREPLACE;
        public SymbolTerminal   S_INSERTORREPLACE
        {
            get { return _S_INSERTORREPLACE; }
        }
        private SymbolTerminal _S_INSERTIFNOTEXIST;
        public SymbolTerminal S_INSERTIFNOTEXIST
        {
            get { return _S_INSERTIFNOTEXIST; }
        }
        private SymbolTerminal _S_REPLACE;
        public SymbolTerminal   S_REPLACE
        {
            get { return _S_REPLACE; }
        }
        private SymbolTerminal _S_SET;
        public SymbolTerminal   S_SET
        {
            get { return _S_SET; }
        }
        private SymbolTerminal _S_REMOVE;
        public SymbolTerminal   S_REMOVE
        {
            get { return _S_REMOVE; }
        }
        private SymbolTerminal _S_VALUES;
        public SymbolTerminal   S_VALUES
        {
            get { return _S_VALUES; }
        }
        private SymbolTerminal _S_DELETE;
        public SymbolTerminal   S_DELETE
        {
            get { return _S_DELETE; }
        }
        private SymbolTerminal _S_SELECT;
        public SymbolTerminal   S_SELECT
        {
            get { return _S_SELECT; }
        }
        private SymbolTerminal _S_FROM;
        public SymbolTerminal   S_FROM
        {
            get { return _S_FROM; }
        }
        private SymbolTerminal _S_AS;
        public SymbolTerminal   S_AS
        {
            get { return _S_AS; }
        }
        private SymbolTerminal _S_COUNT;
        public SymbolTerminal   S_COUNT
        {
            get { return _S_COUNT; }
        }
        private SymbolTerminal _S_JOIN;
        public SymbolTerminal   S_JOIN
        {
            get { return _S_JOIN; }
        }
        private SymbolTerminal _S_BY;
        public SymbolTerminal   S_BY
        {
            get { return _S_BY; }
        }
        private SymbolTerminal _S_WHERE;
        public SymbolTerminal   S_WHERE
        {
            get { return _S_WHERE; }
        }
        private SymbolTerminal _S_TYPE;
        public SymbolTerminal   S_TYPE
        {
            get { return _S_TYPE; }
        }
        private SymbolTerminal _S_TYPES;
        public SymbolTerminal   S_TYPES
        {
            get { return _S_TYPES; }
        }
        private SymbolTerminal _S_EDITION;
        public SymbolTerminal   S_EDITION
        {
            get { return _S_EDITION; }
        }
        private SymbolTerminal _S_INDEXTYPE;
        public SymbolTerminal   S_INDEXTYPE
        {
            get { return _S_INDEXTYPE; }
        }
        private SymbolTerminal _S_LIST;
        public SymbolTerminal   S_LIST
        {
            get { return _S_LIST; }
        }
        private SymbolTerminal _S_ListTypePrefix;
        public SymbolTerminal   S_ListTypePrefix
        {
            get { return _S_ListTypePrefix; }
        }
        private SymbolTerminal _S_ListTypePostfix;
        public SymbolTerminal   S_ListTypePostfix
        {
            get { return _S_ListTypePostfix; }
        }
        private SymbolTerminal _S_EXTENDS;
        public SymbolTerminal   S_EXTENDS
        {
            get { return _S_EXTENDS; }
        }
        private SymbolTerminal _S_ATTRIBUTES;
        public SymbolTerminal   S_ATTRIBUTES
        {
            get { return _S_ATTRIBUTES; }
        }
        private SymbolTerminal _S_MATCHES;
        public SymbolTerminal   S_MATCHES
        {
            get { return _S_MATCHES; }
        }
        private SymbolTerminal _S_LIMIT;
        public SymbolTerminal   S_LIMIT
        {
            get { return _S_LIMIT; }
        }
        private SymbolTerminal _S_DEPTH;
        public SymbolTerminal   S_DEPTH
        {
            get { return _S_DEPTH; }
        }

        #region REFERENCE former SETREF

        private SymbolTerminal _S_REFERENCE;
        public SymbolTerminal S_REFERENCE
        {
            get { return _S_REFERENCE; }
        }
        private SymbolTerminal _S_REF;
        public SymbolTerminal S_REF
        {
            get { return _S_REF; }
        }

        #endregion

        #region REFERENCEUUID

        private SymbolTerminal _S_REFUUID;
        public SymbolTerminal S_REFUUID
        {
            get { return _S_REFUUID; }
        }

        private SymbolTerminal _S_REFERENCEUUID;
        public SymbolTerminal S_REFERENCEUUID
        {
            get { return _S_REFERENCEUUID; }
        }

        #endregion

        #region LISTOF/SETOF/SETOFUUIDS

        private SymbolTerminal _S_LISTOF;
        public SymbolTerminal S_LISTOF
        {
            get { return _S_LISTOF; }
        }
        private SymbolTerminal _S_SETOF;
        public SymbolTerminal S_SETOF
        {
            get { return _S_SETOF; }
        }
        private SymbolTerminal _S_SETOFUUIDS;
        public SymbolTerminal S_SETOFUUIDS
        {
            get { return _S_SETOFUUIDS; }
        } 
        #endregion

        private SymbolTerminal _S_UUID;
        public SymbolTerminal   S_UUID
        {
            get { return _S_UUID; }
        }
        private SymbolTerminal _S_OFFSET;
        public SymbolTerminal   S_OFFSET
        {
            get { return _S_OFFSET; }
        }
        private SymbolTerminal _S_TRUNCATE;
        public SymbolTerminal   S_TRUNCATE
        {
            get { return _S_TRUNCATE; }
        }
        private SymbolTerminal _S_TRUE;
        public SymbolTerminal   S_TRUE
        {
            get { return _S_TRUE; }
        }
        private SymbolTerminal _S_FALSE;
        public SymbolTerminal   S_FALSE
        {
            get { return _S_FALSE; }
        }
        private SymbolTerminal _S_SORTED;
        public SymbolTerminal   S_SORTED
        {
            get { return _S_SORTED; }
        }
        private SymbolTerminal _S_ASC;
        public SymbolTerminal   S_ASC
        {
            get { return _S_ASC; }
        }
        private SymbolTerminal _S_DESC;

        public SymbolTerminal   S_DESC
        {
            get { return _S_DESC; }
        }
        private SymbolTerminal _S_QUEUESIZE;
        public SymbolTerminal   S_QUEUESIZE
        {
            get { return _S_QUEUESIZE; }
        }
        private SymbolTerminal _S_WEIGHTED;
        public SymbolTerminal   S_WEIGHTED
        {
            get { return _S_WEIGHTED; }
        }
        private SymbolTerminal _S_SETTING;
        public SymbolTerminal   S_SETTING
        {
            get { return _S_SETTING; }
        }
        private SymbolTerminal _S_GET;
        public SymbolTerminal   S_GET
        {
            get { return _S_GET; }
        }
        private SymbolTerminal _S_DB;
        public SymbolTerminal   S_DB
        {
            get { return _S_DB; }
        }
        private SymbolTerminal _S_SESSION;
        public SymbolTerminal   S_SESSION
        {
            get { return _S_SESSION; }
        }
        private SymbolTerminal _S_ATTRIBUTE;
        public SymbolTerminal   S_ATTRIBUTE
        {
            get { return _S_ATTRIBUTE; }
        }
        private SymbolTerminal _S_DEFAULT;
        public SymbolTerminal   S_DEFAULT
        {
            get { return _S_DEFAULT; }
        }
        private SymbolTerminal _S_BACKWARDEDGES;

        public SymbolTerminal   S_BACKWARDEDGES
        {
            get { return _S_BACKWARDEDGES; }
        }
        private SymbolTerminal _S_BACKWARDEDGE;
        public SymbolTerminal   S_BACKWARDEDGE
        {
            get { return _S_BACKWARDEDGE; }
        }
        private SymbolTerminal _S_DESCRIBE;
        public SymbolTerminal   S_DESCRIBE
        {
            get { return _S_DESCRIBE; }
        }
        private SymbolTerminal _S_DESCFUNC;
        public SymbolTerminal   S_DESCFUNC
        {
            get { return _S_DESCFUNC; }
        }
        private SymbolTerminal _S_DESCAGGR;
        public SymbolTerminal   S_DESCAGGR
        {
            get { return _S_DESCAGGR; }
        }
        private SymbolTerminal _S_DESCAGGRS;
        public SymbolTerminal   S_DESCAGGRS
        {
            get { return _S_DESCAGGRS; }
        }
        private SymbolTerminal _S_DESCSETT;
        public SymbolTerminal   S_DESCSETT
        {
            get { return _S_DESCSETT; }
        }
        private SymbolTerminal _S_DESCSETTINGS;
        public SymbolTerminal   S_DESCSETTINGS
        {
            get { return _S_DESCSETTINGS; }
        }
        private SymbolTerminal _S_DESCTYPE;
        public SymbolTerminal   S_DESCTYPE
        {
            get { return _S_DESCTYPE; }
        }
        private SymbolTerminal _S_DESCTYPES;
        public SymbolTerminal   S_DESCTYPES
        {
            get { return _S_DESCTYPES; }
        }
        private SymbolTerminal _S_DESCFUNCTIONS;
        public SymbolTerminal   S_DESCFUNCTIONS
        {
            get { return _S_DESCFUNCTIONS; }
        }
        private SymbolTerminal _S_DESCIDX;
        public SymbolTerminal   S_DESCIDX
        {
            get { return _S_DESCIDX; }
        }
        private SymbolTerminal _S_DESCIDXS;
        public SymbolTerminal   S_DESCIDXS
        {
            get { return _S_DESCIDXS; }
        }
        private SymbolTerminal _S_DESCEDGE;
        public SymbolTerminal   S_DESCEDGE
        {
            get { return _S_DESCEDGE; }
        }
        private SymbolTerminal _S_DESCEDGES;
        public SymbolTerminal   S_DESCEDGES
        {
            get { return _S_DESCEDGES; }
        }
        private SymbolTerminal _S_MANDATORY;
        public SymbolTerminal   S_MANDATORY
        {
            get { return _S_MANDATORY; }
        }
        private SymbolTerminal _S_ABSTRACT;
        public SymbolTerminal   S_ABSTRACT
        {
            get { return _S_ABSTRACT; }
        }

        #region Transactions

        private SymbolTerminal _S_TRANSACTBEGIN;
        public SymbolTerminal S_TRANSACTBEGIN
        {
            get { return _S_TRANSACTBEGIN; }
        }
        private SymbolTerminal _S_TRANSACT;
        public SymbolTerminal S_TRANSACT
        {
            get { return _S_TRANSACT; }
        }
        private SymbolTerminal _S_TRANSACTDISTRIBUTED;
        public SymbolTerminal S_TRANSACTDISTRIBUTED
        {
            get { return _S_TRANSACTDISTRIBUTED; }
        }
        private SymbolTerminal _S_TRANSACTLONGRUNNING;
        public SymbolTerminal S_TRANSACTLONGRUNNING
        {
            get { return _S_TRANSACTLONGRUNNING; }
        }
        private SymbolTerminal _S_TRANSACTISOLATION;
        public SymbolTerminal S_TRANSACTISOLATION
        {
            get { return _S_TRANSACTISOLATION; }
        }
        private SymbolTerminal _S_TRANSACTNAME;
        public SymbolTerminal S_TRANSACTNAME
        {
            get { return S_TRANSACTNAME; }
        }
        private SymbolTerminal _S_TRANSACTTIMESTAMP;
        public SymbolTerminal S_TRANSACTTIMESTAMP
        {
            get { return _S_TRANSACTTIMESTAMP; }
        }
        private SymbolTerminal _S_TRANSACTCOMMIT;
        public SymbolTerminal S_TRANSACTCOMMIT
        {
            get { return _S_TRANSACTCOMMIT; }
        }
        private SymbolTerminal _S_TRANSACTROLLBACK;
        public SymbolTerminal S_TRANSACTROLLBACK
        {
            get { return _S_TRANSACTROLLBACK; }
        }
        private SymbolTerminal _S_TRANSACTCOMROLLASYNC;
        public SymbolTerminal S_TRANSACTCOMROLLASYNC
        {
            get { return _S_TRANSACTCOMROLLASYNC; }
        } 

        #endregion

        private SymbolTerminal _S_REMOVEFROMLIST;
        public SymbolTerminal S_REMOVEFROMLIST
        {
            get { return _S_REMOVEFROMLIST; }
        }
        private SymbolTerminal _S_ADDTOLIST;
        public SymbolTerminal S_ADDTOLIST
        {
            get { return _S_ADDTOLIST; }
        }
        private SymbolTerminal _S_COMMENT;
        public SymbolTerminal S_COMMENT
        {
            get { return _S_COMMENT; }
        }
        private SymbolTerminal _S_REBUILD;
        public SymbolTerminal S_REBUILD
        {
            get { return _S_REBUILD; }
        }

        #region DUMP

        private SymbolTerminal _S_DUMP;
        public SymbolTerminal S_DUMP
        {
            get { return _S_DUMP; }
        }


        private SymbolTerminal _S_DUMP_TYPE_ALL;
        public SymbolTerminal S_DUMP_TYPE_ALL
        {
            get { return _S_DUMP_TYPE_ALL; }
        }

        private SymbolTerminal _S_DUMP_TYPE_GDDL;
        public SymbolTerminal S_DUMP_TYPE_GDDL
        {
            get { return _S_DUMP_TYPE_GDDL; }
        }

        private SymbolTerminal _S_DUMP_TYPE_GDML;
        public SymbolTerminal S_DUMP_TYPE_GDML
        {
            get { return _S_DUMP_TYPE_GDML; }
        }


        private SymbolTerminal _S_DUMP_FORMAT_GQL;
        public SymbolTerminal S_DUMP_FORMAT_GQL
        {
            get { return _S_DUMP_FORMAT_GQL; }
        }

        private SymbolTerminal _S_DUMP_FORMAT_CSV;
        public SymbolTerminal S_DUMP_FORMAT_CSV
        {
            get { return _S_DUMP_FORMAT_CSV; }
        }        

        #endregion

        #endregion

        #region Constructor and definitions

        #region GraphQL() and definitions

        public GraphQL(DBContext doNotUseMe)
            : this()
        { }

        public GraphQL()
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
            var name                = new IdentifierTerminal("name", "ÄÖÜäöüß0123456789_", "ÄÖÜäöü0123456789$_");


            #endregion

            //var name_ext            = TerminalFactory.CreateSqlExtIdentifier("name_ext"); //removed, because we do not want to hav types or sth else with whitespaces, otherwise it conflicts with tupleSet

            #region Symbols

            _S_CREATE                          = Symbol("CREATE");
            _S_comma                           = Symbol(",");
            _S_dot                             = Symbol(".");
            _S_colon                           = Symbol(":");
            _S_BRACKET_LEFT                    = Symbol(TERMINAL_BRACKET_LEFT);
            _S_BRACKET_RIGHT                   = Symbol(TERMINAL_BRACKET_RIGHT);
            _S_TUPLE_BRACKET_LEFT              = Symbol("[");
            _S_TUPLE_BRACKET_RIGHT             = Symbol("]");
            _S_edgeInformationDelimiterSymbol  = Symbol(DBConstants.EdgeInformationDelimiterSymbol);
            _S_edgeTraversalDelimiter          = Symbol(DBConstants.EdgeTraversalDelimiterSymbol);
            _S_NULL                            = Symbol("NULL");
            _S_NOT                             = Symbol("NOT");
            _S_UNIQUE                          = Symbol("UNIQUE");
            _S_WITH                            = Symbol("WITH");
            _S_TABLE                           = Symbol("TABLE");
            _S_ALTER                           = Symbol("ALTER");
            _S_ADD                             = Symbol("ADD");
            _S_TO                              = Symbol("TO");
            _S_COLUMN                          = Symbol("COLUMN");
            _S_DROP                            = Symbol("DROP");
            _S_RENAME                          = Symbol("RENAME");
            _S_CONSTRAINT                      = Symbol("CONSTRAINT");
            _S_INDEX                           = Symbol("INDEX");
            _S_INDICES                         = Symbol("INDICES");
            _S_ON                              = Symbol("ON");
            _S_KEY                             = Symbol("KEY");
            _S_PRIMARY                         = Symbol("PRIMARY");
            _S_INSERT                          = Symbol("INSERT");
            _S_INTO                            = Symbol("INTO");
            _S_UPDATE                          = Symbol("UPDATE");
            _S_INSERTORUPDATE                  = Symbol("INSERTORUPDATE");
            _S_INSERTORREPLACE                 = Symbol("INSERTORREPLACE");
            _S_INSERTIFNOTEXIST                = Symbol("INSERTIFNOTEXIST");
            _S_REPLACE                         = Symbol("REPLACE");
            _S_SET                             = Symbol(TERMINAL_SET);
            _S_REMOVE                          = Symbol("REMOVE");
            _S_VALUES                          = Symbol("VALUES");
            _S_DELETE                          = Symbol("DELETE");
            _S_SELECT                          = Symbol("SELECT");
            _S_FROM                            = Symbol("FROM");
            _S_AS                              = Symbol("AS");
            _S_COUNT                           = Symbol("COUNT");
            _S_JOIN                            = Symbol("JOIN");
            _S_BY                              = Symbol("BY");
            _S_WHERE                           = Symbol("WHERE");
            _S_TYPE                            = Symbol("TYPE");
            _S_TYPES                           = Symbol("TYPES");
            _S_EDITION                         = Symbol("EDITION");
            _S_INDEXTYPE                       = Symbol("INDEXTYPE");
            _S_LIST                            = Symbol(TERMINAL_LIST);
            _S_ListTypePrefix                  = Symbol(TERMINAL_LT);
            _S_ListTypePostfix                 = Symbol(TERMINAL_GT);
            _S_EXTENDS                         = Symbol("EXTENDS");
            _S_ATTRIBUTES                      = Symbol("ATTRIBUTES");
            _S_MATCHES                         = Symbol("MATCHES");
            _S_LIMIT                           = Symbol("LIMIT");
            _S_DEPTH                           = Symbol("DEPTH");
            _S_REFERENCE                       = Symbol("REFERENCE");
            _S_REF                             = Symbol("REF");
            _S_REFUUID                         = Symbol("REFUUID");
            _S_REFERENCEUUID                   = Symbol("REFERENCEUUID");
            _S_LISTOF                          = Symbol(DBConstants.LISTOF);
            _S_SETOF                           = Symbol(DBConstants.SETOF);
            _S_SETOFUUIDS                      = Symbol(DBConstants.SETOFUUIDS);
            _S_UUID                            = Symbol("UUID");
            _S_OFFSET                          = Symbol("OFFSET");
            _S_TRUNCATE                        = Symbol("TRUNCATE");
            _S_TRUE                            = Symbol(TERMINAL_TRUE);
            _S_FALSE                           = Symbol(TERMINAL_FALSE);
            _S_SORTED                          = Symbol(TERMINAL_SORTED);
            _S_ASC                             = Symbol(TERMINAL_ASC);
            _S_DESC                            = Symbol(TERMINAL_DESC);
            _S_QUEUESIZE                       = Symbol(TERMINAL_QUEUESIZE);
            _S_WEIGHTED                        = Symbol(TERMINAL_WEIGHTED);
            _S_SETTING                         = Symbol("SETTING");
            _S_GET                             = Symbol("GET");
            _S_DB                              = Symbol("DB");
            _S_SESSION                         = Symbol("SESSION");
            _S_ATTRIBUTE                       = Symbol("ATTRIBUTE");
            _S_DEFAULT                         = Symbol("DEFAULT");
            _S_BACKWARDEDGES                   = Symbol("BACKWARDEDGES");
            _S_BACKWARDEDGE                    = Symbol("BACKWARDEDGE");
            _S_DESCRIBE                        = Symbol("DESCRIBE");
            _S_DESCFUNC                        = Symbol("FUNCTION");
            _S_DESCAGGR                        = Symbol("AGGREGATE");
            _S_DESCAGGRS                       = Symbol("AGGREGATES");
            _S_DESCSETT                        = Symbol("SETTING");
            _S_DESCSETTINGS                    = Symbol("SETTINGS");
            _S_DESCTYPE                        = Symbol("TYPE");
            _S_DESCTYPES                       = Symbol("TYPES");
            _S_DESCFUNCTIONS                   = Symbol("FUNCTIONS");
            _S_DESCIDX                         = Symbol("INDEX");
            _S_DESCIDXS                        = Symbol("INDICES");
            _S_DESCEDGE                        = Symbol("EDGE");
            _S_DESCEDGES                       = Symbol("EDGES");
            _S_MANDATORY                       = Symbol("MANDATORY");
            _S_ABSTRACT                        = Symbol("ABSTRACT");
            _S_TRANSACTBEGIN                   = Symbol("BEGIN");
            _S_TRANSACT                        = Symbol("TRANSACTION");
            _S_TRANSACTDISTRIBUTED             = Symbol(DBConstants.TRANSACTION_DISTRIBUTED);
            _S_TRANSACTLONGRUNNING             = Symbol(DBConstants.TRANSACTION_LONGRUNNING);
            _S_TRANSACTISOLATION               = Symbol(DBConstants.TRANSACTION_ISOLATION);
            _S_TRANSACTNAME                    = Symbol(DBConstants.TRANSACTION_NAME);
            _S_TRANSACTTIMESTAMP               = Symbol(DBConstants.TRANSACTION_TIMESTAMP);
            _S_TRANSACTROLLBACK                = Symbol(DBConstants.TRANSACTION_ROLLBACK);
            _S_TRANSACTCOMMIT                  = Symbol(DBConstants.TRANSACTION_COMMIT);
            _S_TRANSACTCOMROLLASYNC            = Symbol(DBConstants.TRANSACTION_COMROLLASYNC);
            _S_ADDTOLIST                       = Symbol("+=");
            _S_REMOVEFROMLIST                  = Symbol("-=");
            _S_DUMP                            = Symbol("DUMP");
            _S_DUMP_TYPE_ALL                   = Symbol("ALL");
            _S_DUMP_TYPE_GDDL                  = Symbol("GDDL");
            _S_DUMP_TYPE_GDML                  = Symbol("GDML");
            _S_DUMP_FORMAT_GQL                 = Symbol("GQL");
            _S_DUMP_FORMAT_CSV                 = Symbol("CSV");
            _S_COMMENT                         = Symbol("COMMENT");
            _S_REBUILD                         = Symbol("REBUILD");

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
            var SelectStmtPandora           = new NonTerminal("SelectStmtPandora", CreateSelectStatementNode);
            var parSelectStmt               = new NonTerminal("parSelectStmt", CreatePartialSelectStmtNode);
            var createTypesStmt             = new NonTerminal("createTypesStmt", CreateCreateTypesStatementNode);
            var insertorupdateStmt          = new NonTerminal("insertorupdateStmt", CreateInsertOrUpdateStatementNode);
            var insertorreplaceStmt         = new NonTerminal("insertorreplaceStmt", CreateInsertOrReplaceStatementNode);
            var replaceStmt                 = new NonTerminal("replaceStmt", CreateReplaceStatementNode);
            var transactStmt                = new NonTerminal("transactStmt", CreateTransActionStatementNode);
            var commitRollBackTransactStmt  = new NonTerminal("commitRollBackTransactStmt", CreateCommitRollbackTransActionNode);
            
            #endregion

            var deleteStmtMember            = new NonTerminal("deleteStmtMember");
            var uniqueOpt                   = new NonTerminal("uniqueOpt", typeof(uniqueOptNode));
            var IndexAttributeList          = new NonTerminal("IndexAttributeList", typeof(CreateIndexAttributeListNode));
            var IndexAttributeMember        = new NonTerminal("IndexAttributeMember", typeof(CreateIndexAttributeNode));
            var IndexAttributeType          = new NonTerminal("IndexAttributeType");
            var orderByAttributeList        = new NonTerminal("orderByAttributeList");
            var orderByAttributeListMember  = new NonTerminal("orderByAttributeListMember");
            var AttributeOrderDirectionOpt  = new NonTerminal("AttributeOrderDirectionOpt");
            var indexTypeOpt                = new NonTerminal("indexTypeOpt", typeof(IndexTypeOptNode));
            var indexNameOpt                = new NonTerminal("indextNameOpt", typeof(IndexNameOptNode));
            var editionOpt                  = new NonTerminal("editionOpt", typeof(EditionOptNode));
            var alterCmd                    = new NonTerminal("alterCmd", typeof(AlterCommandNode));
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

            #region Expression

            var expression                  = new NonTerminal("expression", typeof(ExpressionNode));
            var expressionOfAList           = new NonTerminal("expressionOfAList", typeof(ExpressionOfAListNode));
            var exprList                    = new NonTerminal("exprList");
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
            var selectionSource             = new NonTerminal("selectionSource");
            var asOpt                       = new NonTerminal("asOpt");
            var aliasOpt                    = new NonTerminal("aliasOpt");
            var aliasOptName                = new NonTerminal("aliasOptName");
            var selectOutputOpt             = new NonTerminal("selectOutputOpt", typeof(SelectOutputOptNode));

            #endregion

            #region Aggregates & Functions

            var aggregate                   = new NonTerminal("aggregate", CreateAggregateNode);
            var aggregateArg                = new NonTerminal("aggregateArg");
            var aggregateName               = new NonTerminal("aggregateName");
            var function                    = new NonTerminal("function", CreateFunctionCallNode);
            var functionName                = new NonTerminal("functionName");

            #endregion

            #region Tuple

            var tuple                       = new NonTerminal("tuple", typeof(TupleNode));
            var bracketLeft                 = new NonTerminal(DBConstants.BracketLeft);
            var bracketRight                = new NonTerminal(DBConstants.BracketRight);
            

            #endregion

            var term                        = new NonTerminal("term");
            var notOpt                      = new NonTerminal("notOpt");
            var funcCall                    = new NonTerminal("funCall", CreateFunctionCallNode);
            var funArgs                     = new NonTerminal("funArgs");
            var GraphDBType                 = new NonTerminal(DBConstants.GraphDBType, CreateGraphDBTypeNode);
            var AttributeList               = new NonTerminal("AttributeList");
            var AttrDefinition              = new NonTerminal("AttrDefinition", CreateAttributeDefinitionNode);
            var ResultObject                = new NonTerminal("ResultObject");
            var ResultList                  = new NonTerminal("ResultList");
            var MatchingClause              = new NonTerminal("MatchingClause");
            var Matching                    = new NonTerminal("MatchingClause");
            var PrefixOperation             = new NonTerminal("PrefixOperation");
            var ParameterList               = new NonTerminal("ParameterList");
            var TypeList                    = new NonTerminal("TypeList");
            var AType                       = new NonTerminal("AType", CreateATypeNode);
            var TypeWrapper                 = new NonTerminal("TypeWrapper");
            var AttrAssignList              = new NonTerminal("AttrAssignList");
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

            

            var Reference                   = new NonTerminal(_S_REFERENCE.Symbol, typeof(SetRefNode));
            var offsetOpt                   = new NonTerminal("offsetOpt", typeof(OffsetNode));
            var resolutionDepthOpt          = new NonTerminal("resolutionDepthOpt");
            var limitOpt                    = new NonTerminal("limitOpt", typeof(LimitNode));
            var SimpleIdList                = new NonTerminal("SimpleIdList");
            var bulkTypeListMember          = new NonTerminal("bulkTypeListMember", CreateBulkTypeListMemberNode);
            var bulkType                    = new NonTerminal("bulkType", CreateBulkTypeNode);
            var truncateStmt                = new NonTerminal("truncateStmt", CreateTruncateStmNode);
            var uniquenessOpt               = new NonTerminal("UniquenessOpt", typeof(UniqueAttributesOptNode));
            var mandatoryOpt                = new NonTerminal("MandatoryOpt", typeof(MandatoryOptNode));
            var TransactOptions             = new NonTerminal("TransactOptions");
            var TransactAttributes          = new NonTerminal("TransactAttributes");
            var TransactIsolation           = new NonTerminal("TransactIsolation");
            var TransactName                = new NonTerminal("TransactName");
            var TransactTimestamp           = new NonTerminal("TransactTimestamp");
            var TransactCommitRollbackOpt   = new NonTerminal("TransactCommitRollbackOpt");
            var TransactCommitRollbackType  = new NonTerminal("TransactCommitRollbackType");            
            
            var Value                       = new NonTerminal("Value");
            var ValueList                   = new NonTerminal("ValueList");
            var BooleanVal                  = new NonTerminal("BooleanVal");            

            var ListType                    = new NonTerminal("ListType");
            var ListParametersForExpression = new NonTerminal("ListParametersForExpression", typeof(ParametersNode));

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

            var indexOptOnCreateType        = new NonTerminal("IndexOptOnCreateType");
            var IndexOptOnCreateTypeMember = new NonTerminal("IndexOptOnCreateTypeMember", typeof(IndexOptOnCreateTypeMemberNode));
            var IndexOptOnCreateTypeMemberList = new NonTerminal("IndexOptOnCreateTypeMemberList");

            #endregion

            #region Dump

            var dumpStmt                        = new NonTerminal("Dump", CreateDumpNode);

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
            var DescrObjStmt                = new NonTerminal("DescrObjStmt", CreateDescrObj);
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

            #endregion

            #region Statements

            #region root

            //BNF Rules
            this.Root = singlestmt;

            singlestmt.Rule = SelectStmtPandora
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
                            | rebuildIndicesStmt;
                            

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
            idlist.Rule = MakePlusRule(idlist, _S_comma, Id);
            id_simpleList.Rule = MakePlusRule(id_simpleList, _S_comma, Id_simple);
            id_simpleDotList.Rule = MakePlusRule(id_simpleDotList, _S_dot, Id_simple);
            id_typeAndAttribute.Rule = TypeWrapper + _S_dot + Id;

            #endregion

            #region ID_or_Func

            IdOrFunc.Rule =     name 
                            |   funcCall;

            dotWrapper.Rule = _S_edgeTraversalDelimiter;

            edgeAccessorWrapper.Rule = _S_edgeInformationDelimiterSymbol;

            IDOrFuncDelimiter.Rule =        dotWrapper
                                        |   edgeAccessorWrapper;

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

            TypeList.Rule = MakePlusRule(TypeList, _S_comma, AType);
            TypeList.Description = "specify the type object to be selected for example a type list could be ‘User U’, ‘Car C’, …\n";

            AType.Rule = Id_simple + Id_simple
                        | Id_simple;

            //AType.Rule = Id + Id_simple
            //                | Id;

            TypeWrapper.Rule = AType;

            #endregion

            #region CreateIndexAttribute

            IndexAttributeList.Rule = MakePlusRule(IndexAttributeList, _S_comma, IndexAttributeMember);

            IndexAttributeMember.Rule = IndexAttributeType;// + AttributeOrderDirectionOpt;

            IndexAttributeType.Rule = Id_simple | id_typeAndAttribute;

            #endregion

            #region OrderDirections

            AttributeOrderDirectionOpt.Rule = Empty
                                                | _S_ASC
                                                | _S_DESC;

            #endregion

            #region Boolean

            BooleanVal.Rule = _S_TRUE | _S_FALSE;

            #endregion

            #region Value

            Value.Rule = string_literal | number | BooleanVal;

            ValueList.Rule = MakeStarRule(ValueList, _S_comma, Value);

            #endregion

            #region ListType

            ListType.Rule = _S_LIST;

            ListParametersForExpression.Rule = Empty
                                         | _S_colon + _S_BRACKET_LEFT + ValueList + _S_BRACKET_RIGHT;

            EdgeType_SortedMember.Rule = _S_ASC | _S_DESC;
            EdgeType_Sorted.Rule = _S_SORTED + "=" + EdgeType_SortedMember;

            #endregion

            #region PandoraType

            //                 SET<                   WEIGHTED  (Double, DEFAULT=2, SORTED=DESC)<   [idsimple]  >>
            EdgeTypeDef.Rule = _S_SET + _S_ListTypePrefix + Id_simple + _S_BRACKET_LEFT + EdgeTypeParams + _S_BRACKET_RIGHT + _S_ListTypePrefix + Id_simple + _S_ListTypePostfix + _S_ListTypePostfix;
            //                       COUNTED        (Integer, DEFAULT=2)                   <   [idsimple]  >
            SingleEdgeTypeDef.Rule = Id_simple + _S_BRACKET_LEFT + EdgeTypeParams + _S_BRACKET_RIGHT + _S_ListTypePrefix + Id_simple + _S_ListTypePostfix;

            EdgeTypeParams.Rule = MakeStarRule(EdgeTypeParams, _S_comma, EdgeTypeParam);
            EdgeTypeParam.Rule = Id_simple
                               | DefaultValueDef
                               | EdgeType_Sorted
                               | string_literal;

            EdgeTypeParam.SetOption(TermOptions.IsTransient, false);

            DefaultValueDef.Rule = _S_DEFAULT + "=" + Value;

            GraphDBType.Rule = Id_simple
                                   // LIST<[idsimple]>
                                   | _S_LIST + _S_ListTypePrefix + Id_simple + _S_ListTypePostfix
                                   | _S_SET + _S_ListTypePrefix + Id_simple + _S_ListTypePostfix
                                   | EdgeTypeDef
                                   | SingleEdgeTypeDef;

            #endregion

            #region AttributeList

            AttributeList.Rule = MakePlusRule(AttributeList, _S_comma, AttrDefinition);

            AttrDefinition.Rule = GraphDBType + Id_simple + AttrDefaultOpValue;

            #endregion

            #region BackwardEdgesList

            BackwardEdgesList.Rule = MakePlusRule(BackwardEdgesList, _S_comma, BackwardEdgesSingleDef);

            BackwardEdgesSingleDef.Rule = Id_simple + _S_dot + Id_simple + Id_simple;
                                            //| Id_simple + _S_dot + Id_simple + _S_ListTypePrefix + Id_simple + _S_BRACKET_LEFT + EdgeTypeParams + _S_BRACKET_RIGHT + _S_ListTypePostfix + Id_simple;

            #endregion

            #region id_simple list

            SimpleIdList.Rule = MakePlusRule(SimpleIdList, _S_comma, Id_simple);

            #endregion

            #region expression

            //Expression
            exprList.Rule = MakeStarRule(exprList, _S_comma, expression);

            exprListOfAList.Rule = MakePlusRule(exprListOfAList, _S_comma, expressionOfAList);

            expression.Rule =       term
                                |   unExpr
                                |   binExpr;

            expressionOfAList.Rule = expression + ListParametersForExpression;


            term.Rule =         IdOrFuncList                  //d.Name 
                            |   string_literal      //'lala'
                            |   number              //10
                            //|   funcCall            //EXISTS ( SelectStatement )
                            |   aggregate           //COUNT ( SelectStatement )
                            |   tuple               //(d.Name, 'Henning', (SelectStatement))
                            |   parSelectStmt      //(FROM User u Select u.Name)
                            | _S_TRUE
                            | _S_FALSE;

            #region Tuple

            tuple.Rule = bracketLeft + exprList + bracketRight;

            bracketLeft.Rule = _S_BRACKET_LEFT | _S_TUPLE_BRACKET_LEFT;
            bracketRight.Rule = _S_BRACKET_RIGHT | _S_TUPLE_BRACKET_RIGHT;

            #endregion

            parSelectStmt.Rule = _S_BRACKET_LEFT + SelectStmtPandora + _S_BRACKET_RIGHT;

            unExpr.Rule = unOp + term;

            unOp.Rule =         _S_NOT 
                            |   "+" 
                            |   "-" 
                            |   "~";

            binExpr.Rule = expression + binOp + expression;

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
                            |   _S_NOT + "LIKE" 
                            |   "IN" 
                            |   "NOTIN" | "NOT_IN" | "NIN" | "!IN"
                            |   "INRANGE";

            notOpt.Rule =       Empty
                            |   _S_NOT;

            #endregion

            #region Functions

            //funcCall covers some psedo-operators and special forms like ANY(...), SOME(...), ALL(...), EXISTS(...), IN(...)
            funcCall.Rule = name + _S_BRACKET_LEFT + funArgs + _S_BRACKET_RIGHT;

            funArgs.Rule =      SelectStmtPandora 
                            |   exprList;

            #endregion

            #region operators

            //Operators
            RegisterOperators(10, "*", "/", "%");
            RegisterOperators(9, "+", "-");
            RegisterOperators(8, "=", ">", "<", ">=", "<=", "<>", "!=", "!<", "!>", "INRANGE");
            RegisterOperators(7, "^", "&", "|");
            RegisterOperators(6, "NOT");
            RegisterOperators(5, "AND");
            RegisterOperators(4, "OR", "LIKE", "IN", "NOTIN", "NOT_IN", "NIN", "!IN");

            #endregion

            #region prefixOperation

            PrefixOperation.Rule =      Id_simple + _S_BRACKET_LEFT + ParameterList + _S_BRACKET_RIGHT;

            ParameterList.Rule =        ParameterList + _S_comma + expression
                                    |   expression;

            #endregion

            #endregion

            #region CREATE INDEX

            createIndexStmt.Rule = _S_CREATE + _S_INDEX + indexNameOpt + editionOpt + _S_ON + TypeWrapper + _S_BRACKET_LEFT + IndexAttributeList + _S_BRACKET_RIGHT + indexTypeOpt;
            uniqueOpt.Rule = Empty | _S_UNIQUE;

            editionOpt.Rule =       Empty
                                | _S_EDITION + Id_simple;

            indexTypeOpt.Rule =     Empty
                                | _S_INDEXTYPE + Id_simple;

            indexNameOpt.Rule = Empty
                                |   Id_simple;

            #endregion

            #region CREATE TYPE(S)

            createTypesStmt.Rule    = _S_CREATE + _S_TYPES + bulkTypeList
                                    | _S_CREATE +  abstractOpt + _S_TYPE + bulkType;

            bulkTypeList.Rule       = MakePlusRule(bulkTypeList, _S_comma, bulkTypeListMember);

            bulkTypeListMember.Rule = abstractOpt + bulkType;

            bulkType.Rule           = Id_simple + extendsOpt + attributesOpt + backwardEdgesOpt + uniquenessOpt + mandatoryOpt + indexOptOnCreateType + commentOpt;

            commentOpt.Rule         =   Empty
                                    |   _S_COMMENT + "=" + string_literal; 

            abstractOpt.Rule        = Empty
                                    | _S_ABSTRACT; 

            extendsOpt.Rule         = Empty
                                    | _S_EXTENDS + Id_simple;

            attributesOpt.Rule      = Empty
                                    | _S_ATTRIBUTES + _S_BRACKET_LEFT + AttributeList + _S_BRACKET_RIGHT;

            backwardEdgesOpt.Rule   = Empty
                                    | _S_BACKWARDEDGES + _S_BRACKET_LEFT + BackwardEdgesList + _S_BRACKET_RIGHT;

            uniquenessOpt.Rule = Empty
                                    | _S_UNIQUE + _S_BRACKET_LEFT + id_simpleList + _S_BRACKET_RIGHT;

            mandatoryOpt.Rule = Empty
                                    | _S_MANDATORY + _S_BRACKET_LEFT + id_simpleList + _S_BRACKET_RIGHT;

            indexOptOnCreateType.Rule = Empty
                                    | _S_INDICES + _S_BRACKET_LEFT + IndexOptOnCreateTypeMemberList + _S_BRACKET_RIGHT
                                    | _S_INDICES + IndexOptOnCreateTypeMember;

            IndexOptOnCreateTypeMemberList.Rule = MakePlusRule(IndexOptOnCreateTypeMemberList, _S_comma, IndexOptOnCreateTypeMember);

            IndexOptOnCreateTypeMember.Rule = _S_BRACKET_LEFT + indexNameOpt + editionOpt + indexTypeOpt + _S_ON + IndexAttributeList + _S_BRACKET_RIGHT
                                            | _S_BRACKET_LEFT + IndexAttributeList + _S_BRACKET_RIGHT;

            AttrDefaultOpValue.Rule = Empty
                                    | "=" + Value
                                    | "=" + _S_LISTOF + _S_BRACKET_LEFT + ValueList + _S_BRACKET_RIGHT
                                    | "=" + _S_SETOF + _S_BRACKET_LEFT + ValueList + _S_BRACKET_RIGHT;
            #endregion

            #region ALTER TYPE

            alterStmt.Rule = _S_ALTER + _S_TYPE + Id_simple + alterCmd + uniquenessOpt + mandatoryOpt;

            alterCmd.Rule = Empty
                            | _S_ADD + _S_ATTRIBUTES + _S_BRACKET_LEFT + AttributeList + _S_BRACKET_RIGHT
                            | _S_DROP + _S_ATTRIBUTES + _S_BRACKET_LEFT + SimpleIdList + _S_BRACKET_RIGHT
                            | _S_ADD + _S_BACKWARDEDGES + _S_BRACKET_LEFT + BackwardEdgesList + _S_BRACKET_RIGHT
                            | _S_DROP + _S_BACKWARDEDGES + _S_BRACKET_LEFT + SimpleIdList + _S_BRACKET_RIGHT
                            | _S_RENAME + _S_ATTRIBUTE + Id_simple + _S_TO + Id_simple
                            | _S_RENAME + _S_BACKWARDEDGE + Id_simple + _S_TO + Id_simple
                            | _S_RENAME + _S_TO + Id_simple
                            | _S_DROP + _S_UNIQUE
                            | _S_DROP + _S_MANDATORY
                            | _S_COMMENT + "=" + string_literal;
            #endregion

            #region SELECT

            SelectStmtPandora.Rule = _S_FROM + TypeList + _S_SELECT + selList + whereClauseOpt + groupClauseOpt + havingClauseOpt + orderClauseOpt + MatchingClause + offsetOpt + limitOpt + resolutionDepthOpt + selectOutputOpt;
            SelectStmtPandora.Description = "The select statement is used to query the database and retrieve one or more types of objects in the database.\n";

            MatchingClause.Rule =       Empty
                                    |   MatchingClause + Matching;

            Matching.Rule =             _S_MATCHES + _S_BRACKET_LEFT + number + _S_BRACKET_RIGHT + PrefixOperation;

            resolutionDepthOpt.Rule =       Empty
                                        |   _S_DEPTH + number;

            selectOutputOpt.Rule    =       Empty
                                        |   "OUTPUT" + name;

            offsetOpt.Rule =       Empty
                            |   _S_OFFSET + number;

            limitOpt.Rule =     Empty
                            |   _S_LIMIT + number;

            selList.Rule =      selectionList;

            selectionList.Rule = MakePlusRule(selectionList, _S_comma, selectionListElement);            

            selectionListElement.Rule =     "*"
                                        |   selectionSource + aliasOpt; 

            aliasOptName.Rule = Id_simple | string_literal;

            aliasOpt.Rule =     Empty
                            |   _S_AS + aliasOptName;

            selectionSource.Rule = aggregate
                //|   funcCall
                //|   Id;
                                | IdOrFuncList;

            #region Aggregate

            aggregate.Rule = aggregateName + _S_BRACKET_LEFT + aggregateArg + _S_BRACKET_RIGHT;

            aggregateArg.Rule =     Id
                                |   "*";

            aggregateName.Rule =        _S_COUNT 
                                    |   "AVG" 
                                    |   "MIN" 
                                    |   "MAX" 
                                    |   "STDEV" 
                                    |   "STDEVP" 
                                    |   "SUM" 
                                    |   "VAR" 
                                    |   "VARP";

            #endregion

            #region Functions

            //function.Rule           = functionName + _S_BRACKET_LEFT + term + _S_BRACKET_RIGHT;

            //functionName.Rule       = FUNC_WEIGHT;

            #endregion

            whereClauseOpt.Rule =       Empty 
                                    |   _S_WHERE + expression;

            groupClauseOpt.Rule =       Empty 
                                    |   "GROUP" + _S_BY + idlist;

            havingClauseOpt.Rule =      Empty 
                                    |   "HAVING" + expression;


            orderByAttributeListMember.Rule =       Id
                                                |   string_literal;

            orderByAttributeList.Rule = MakePlusRule(orderByAttributeList, _S_comma, orderByAttributeListMember);

            orderClauseOpt.Rule =       Empty 
                                    |   "ORDER" + _S_BY + orderByAttributeList + AttributeOrderDirectionOpt;

            #endregion

            #region INSERT

            InsertStmt.Rule = _S_INSERT + _S_INTO + TypeWrapper + insertValuesOpt;

            insertValuesOpt.Rule =      Empty
                                    |   _S_VALUES + _S_BRACKET_LEFT + AttrAssignList + _S_BRACKET_RIGHT;

            AttrAssignList.Rule = MakePlusRule(AttrAssignList, _S_comma, AttrAssign);

            AttrAssign.Rule =       Id + "=" + expression
                                |   Id + "=" + Reference
                                |   Id + "=" + CollectionOfDBObjects;

            CollectionOfDBObjects.Rule = _S_SETOF + CollectionTuple
                                            | _S_LISTOF + CollectionTuple
                                            | _S_SETOFUUIDS + CollectionTuple
                                            | _S_SETOF + "()";

            CollectionTuple.Rule = _S_BRACKET_LEFT + ExtendedExpressionList + _S_BRACKET_RIGHT;

            ExtendedExpressionList.Rule = MakePlusRule(ExtendedExpressionList, _S_comma, ExtendedExpression);

            ExtendedExpression.Rule = expression + ListParametersForExpression;

            Reference.Rule = _S_REFERENCE + tuple + ListParametersForExpression
                           | _S_REF + tuple + ListParametersForExpression
                           | _S_REFUUID + tuple + ListParametersForExpression
                           | _S_REFERENCEUUID + tuple + ListParametersForExpression;

                //| _S_SETREF + tupleRangeSet + ListParametersForExpression;

            #endregion

            #region UPDATE

            updateStmt.Rule = _S_UPDATE + TypeWrapper + _S_SET + _S_BRACKET_LEFT + AttrUpdateList + _S_BRACKET_RIGHT + whereClauseOpt;

            AttrUpdateList.Rule = MakePlusRule(AttrUpdateList, _S_comma, AttrUpdateOrAssign);

            AttrUpdateOrAssign.Rule =       AttrAssign
                                        |   AttrRemove
                                        |   ListAttrUpdate;

            AttrRemove.Rule = _S_REMOVE + _S_ATTRIBUTES + _S_BRACKET_LEFT + id_simpleList + _S_BRACKET_RIGHT;

            ListAttrUpdate.Rule =       AddToListAttrUpdate
                                    |   RemoveFromListAttrUpdate;

            AddToListAttrUpdate.Rule =      AddToListAttrUpdateAddTo
                                        |   AddToListAttrUpdateOperator;

            AddToListAttrUpdateAddTo.Rule = _S_ADD + _S_TO + Id + CollectionOfDBObjects;
            AddToListAttrUpdateOperator.Rule = Id + _S_ADDTOLIST + CollectionOfDBObjects;

            RemoveFromListAttrUpdate.Rule =         RemoveFromListAttrUpdateAddToRemoveFrom
                                            |       RemoveFromListAttrUpdateAddToOperator;

            RemoveFromListAttrUpdateAddToRemoveFrom.Rule = _S_REMOVE + _S_FROM + Id + tuple;
            RemoveFromListAttrUpdateAddToOperator.Rule = Id + _S_REMOVEFROMLIST + tuple;


            #endregion

            #region DROP TYPE

            dropTypeStmt.Rule = _S_DROP + _S_TYPE + Id_simple;

            #endregion

            #region DROP INDEX

            dropIndexStmt.Rule = _S_FROM + TypeWrapper + _S_DROP + _S_INDEX + Id_simple + editionOpt;

            #endregion

            #region TRUNCATE

            truncateStmt.Rule = _S_TRUNCATE + Id_simple;

            #endregion

            #region DELETE

            deleteStmtMember.Rule = Empty | idlist;
            deleteStmt.Rule = _S_FROM + TypeList + _S_DELETE + deleteStmtMember + whereClauseOpt;

            #endregion

            #region SETTING

            SettingsStatement.Rule = _S_SETTING + SettingScope + SettingOperation;

            SettingScope.Rule = _S_DB | _S_SESSION | SettingTypeNode | SettingAttrNode;

            SettingTypeNode.Rule = _S_TYPE + SettingTypeStmLst;

            SettingTypeStmLst.Rule = MakePlusRule(SettingTypeStmLst, _S_comma, TypeWrapper);

            SettingAttrNode.Rule = _S_ATTRIBUTE + SettingAttrStmLst;

            SettingAttrStmLst.Rule = MakePlusRule(SettingAttrStmLst, _S_comma, id_typeAndAttribute);

            SettingOperation.Rule = SettingOpSet | SettingOpGet | SettingOpRemove;

            SettingOpSet.Rule = _S_SET + _S_BRACKET_LEFT + SettingItemSetLst + _S_BRACKET_RIGHT;

            SettingItemsSet.Rule = string_literal + "=" + SettingItemSetVal;

            SettingItemSetLst.Rule = MakePlusRule(SettingItemSetLst, _S_comma, SettingItemsSet);

            SettingItemSetVal.Rule =        number 
                                        |   _S_DEFAULT
                                        |   string_literal;

            SettingOpGet.Rule = _S_GET + _S_BRACKET_LEFT + SettingItems + _S_BRACKET_RIGHT;

            SettingOpRemove.Rule = _S_REMOVE + _S_BRACKET_LEFT + SettingItems + _S_BRACKET_RIGHT;

            SettingItems.Rule = MakePlusRule(SettingItems, _S_comma, string_literal);

            #endregion

            #region DESCRIBE

            DescrInfoStmt.Rule = _S_DESCRIBE + DescrArgument;
            DescrInfoStmt.Description = "This statement gives you all information about an type, a function, an index, an setting, an object, an edge or an aggregate.\n";

            DescrArgument.Rule = DescrAggrStmt | DescrAggrsStmt | DescrEdgeStmt | DescrEdgesStmt | DescrTypeStmt | DescrTypesStmt | DescrFuncStmt | DescrFunctionsStmt | DescrSettStmt | DescrSettingsStmt | DescrIdxStmt | DescrIdxsStmt;

            DescrAggrStmt.Rule = _S_DESCAGGR + Id_simple;

            DescrAggrsStmt.Rule = _S_DESCAGGRS;

            DescrEdgeStmt.Rule = _S_DESCEDGE + Id_simple;

            DescrEdgesStmt.Rule = _S_DESCEDGES;

            DescrTypeStmt.Rule = _S_DESCTYPE + Id_simple;

            DescrTypesStmt.Rule = _S_DESCTYPES;

            DescrFuncStmt.Rule = _S_DESCFUNC + Id_simple;

            DescrFunctionsStmt.Rule = _S_DESCFUNCTIONS;

            DescrSettStmt.Rule = _S_DESCSETT + DescrSettItem | _S_DESCSETTINGS + DescrSettingsItems;

            DescrSettItem.Rule = Id_simple + Empty | Id_simple + _S_ON + _S_TYPE + AType | Id_simple + _S_ON + _S_ATTRIBUTE + id_typeAndAttribute | Id_simple + _S_ON + _S_DB | Id_simple + _S_ON + _S_SESSION;

            DescrSettingsItems.Rule = _S_ON + _S_TYPE + TypeList | _S_ON + _S_ATTRIBUTE + id_typeAndAttribute | _S_ON + _S_DB | _S_ON + _S_SESSION;

            DescrSettingsStmt.Rule = _S_DESCSETTINGS;

            DescrIdxStmt.Rule = _S_DESCIDX + id_simpleDotList + DescrIdxEdtStmt;

            DescrIdxEdtStmt.Rule = Empty | Id_simple;

            DescrIdxsStmt.Rule = _S_DESCIDXS;
            
            #endregion

            #region INSERTORUPDATE

            insertorupdateStmt.Rule = _S_INSERTORUPDATE + TypeWrapper + _S_VALUES + _S_BRACKET_LEFT + AttrAssignList + _S_BRACKET_RIGHT + whereClauseOpt;

            #endregion

            #region INSERTORREPLACE

            insertorreplaceStmt.Rule = _S_INSERTORREPLACE + TypeWrapper + _S_VALUES + _S_BRACKET_LEFT + AttrAssignList + _S_BRACKET_RIGHT + whereClauseOpt;

            #endregion

            #region REPLACE

            replaceStmt.Rule = _S_REPLACE + TypeWrapper + _S_VALUES + _S_BRACKET_LEFT + AttrAssignList + _S_BRACKET_RIGHT + _S_WHERE + expression;

            #endregion

            #region DUMP

            var dumpType   = new NonTerminal("dumpType",   CreateDumpTypeNode);
            var dumpFormat = new NonTerminal("dumpFormat", CreateDumpFormatNode);

            dumpType.Rule   = _S_DUMP_TYPE_ALL | _S_DUMP_TYPE_GDDL | _S_DUMP_TYPE_GDML | Empty;     // If empty => create both
            dumpFormat.Rule = _S_AS + _S_DUMP_FORMAT_GQL | _S_AS + _S_DUMP_FORMAT_CSV | Empty;      // if empty => create GQL
            //dumpFormat.Rule = _S_AS + MakePlusRule(_S_DUMP_FORMAT_GQL, _S_DUMP_FORMAT_CSV) | Empty;      // if empty => create GQL
            dumpStmt.Rule   = _S_DUMP + dumpType + dumpFormat;

            #endregion

            #region TRANSACTION


            #region BeginTransAction

            transactStmt.Rule = _S_TRANSACTBEGIN + TransactOptions + _S_TRANSACT + TransactAttributes;

            TransactOptions.Rule = Empty |
                                _S_TRANSACTDISTRIBUTED + _S_TRANSACTLONGRUNNING |
                                _S_TRANSACTDISTRIBUTED |
                                _S_TRANSACTLONGRUNNING;

            TransactAttributes.Rule = Empty |
                                TransactIsolation |
                                TransactName | 
                                TransactTimestamp |
                                TransactIsolation + TransactName |
                                TransactIsolation + TransactTimestamp |
                                TransactName + TransactTimestamp |
                                TransactIsolation + TransactName + TransactTimestamp;

            TransactIsolation.Rule = _S_TRANSACTISOLATION + "=" + string_literal;

            TransactName.Rule = _S_TRANSACTNAME + "=" + string_literal;

            TransactTimestamp.Rule = _S_TRANSACTTIMESTAMP + "=" + string_literal;

            #endregion

            #region CommitRollbackTransAction            
            
            commitRollBackTransactStmt.Rule = TransactCommitRollbackType + _S_TRANSACT + TransactCommitRollbackOpt;

            TransactCommitRollbackType.Rule = _S_TRANSACTCOMMIT | _S_TRANSACTROLLBACK;
            
            TransactCommitRollbackOpt.Rule = Empty |
                                        TransactName |
                                        _S_TRANSACTCOMROLLASYNC |
                                        TransactName + _S_TRANSACTCOMROLLASYNC;

            #endregion            

            #endregion

            #region Rebuild Indices

            rebuildIndicesStmt.Rule = _S_REBUILD + _S_INDICES + rebuildIndicesTypes;

            rebuildIndicesTypes.Rule = Empty | TypeList;

            #endregion

            #endregion

            #region misc

            #region operators
            RegisterOperators(1, Associativity.Neutral, "AND", "OR");
            RegisterOperators(2, Associativity.Neutral, "=", "!=", ">", ">=", "<", "<=", "<>", "!<", "!>", "IN", "NOTIN", "INRANGE");
            RegisterOperators(3, "+", "-");
            RegisterOperators(4, "*", "/");
            RegisterOperators(5, Associativity.Right, "**");
            #endregion

            #region punctuation

            RegisterPunctuation(",", _S_BRACKET_LEFT.Symbol, _S_BRACKET_RIGHT.Symbol, "[", "]");
            //RegisterPunctuation(",", _S_BRACKET_LEFT.Symbol, _S_BRACKET_RIGHT.Symbol, _S_TUPLE_BRACKET_LEFT.Symbol, _S_TUPLE_BRACKET_RIGHT.Symbol);
            //RegisterPunctuation(",");
            //RegisterBracePair(_S_BRACKET_LEFT.Symbol, _S_BRACKET_RIGHT.Symbol);
            //RegisterBracePair(_S_TUPLE_BRACKET_LEFT.Symbol, _S_TUPLE_BRACKET_RIGHT.Symbol);
            //RegisterBracePair(_S_TUPLE_BRACKET_LEFT_EXCLUSIVE.Symbol, _S_TUPLE_BRACKET_RIGHT.Symbol);
            //RegisterBracePair(_S_TUPLE_BRACKET_LEFT.Symbol, _S_TUPLE_BRACKET_RIGHT_EXCLUSIVE.Symbol);
            //RegisterBracePair(_S_TUPLE_BRACKET_LEFT_EXCLUSIVE.Symbol, _S_TUPLE_BRACKET_RIGHT_EXCLUSIVE.Symbol);

            #endregion

            base.MarkTransient(
                singlestmt, Id_simple, selList, selectionSource, aggregateName, expression, term, funArgs
                , unOp, binOp, aliasOpt, aliasOptName, orderByAttributeListMember
                , Value
                //, EdgeTypeParam
                , EdgeType_SortedMember, AttrUpdateOrAssign, ListAttrUpdate, SettingItemSetVal, DescrArgument,
                TypeWrapper //is used as a wrapper for AType
                , IdOrFunc //, IdOrFuncList
                , exprList, aggregateArg,
                ExtendedExpressionList
                );

            #endregion
        
        }


        #endregion

        #endregion

        #region Node Delegates

        private void CreateUnExpressionNode(CompilerContext context, ParseTreeNode parseNode)
        {
            UnaryExpressionNode aUnExpressionNode = new UnaryExpressionNode();

            aUnExpressionNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aUnExpressionNode;
        }

        private void CreateGraphDBTypeNode(CompilerContext context, ParseTreeNode parseNode)
        {
            GraphDBTypeNode aPandoraTypeNode = new GraphDBTypeNode();

            aPandoraTypeNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aPandoraTypeNode;
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

        private void CreateATypeNode(CompilerContext context, ParseTreeNode parseNode)
        {
            ATypeNode aATypeNode = new ATypeNode();

            aATypeNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)aATypeNode;

            if (context.PandoraListOfReferences == null)
                context.PandoraListOfReferences = new Dictionary<string, object>();

            if (aATypeNode.Reference != null && !context.PandoraListOfReferences.ContainsKey(aATypeNode.Reference))
            {
                context.PandoraListOfReferences.Add(aATypeNode.Reference, (object)aATypeNode);
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

        private void CreateFunctionCallNode(CompilerContext context, ParseTreeNode parseNode)
        {
            FuncCallNode functionCallNode = new FuncCallNode();

            functionCallNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)functionCallNode;
        }

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


        private void CreateDescrObj(CompilerContext context, ParseTreeNode parseNode)
        {
            DescribeObjectNode objInfoNode = new DescribeObjectNode();

            objInfoNode.GetContent(context, parseNode);

            parseNode.AstNode = (object)objInfoNode;
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

        #endregion


        #region IDumpable Members

        #region Export GraphDDL

        public Exceptional<List<String>> ExportGraphDDL(DumpFormats myDumpFormat, DBContext myDBContext)
        {

            var _StringBuilder = new StringBuilder(String.Concat(S_CREATE.ToUpperString(), " ", S_TYPES.ToUpperString(), " "));
            var _List = new StringBuilder(String.Concat(S_CREATE.ToUpperString(), " ", S_TYPES.ToUpperString(), " "));
            var delimiter = ", ";

            foreach (var _GraphDBType in myDBContext.DBTypeManager.GetAllTypes(false))
                _StringBuilder.Append(String.Concat(CreateGraphDDL(myDumpFormat, _GraphDBType, myDBContext), delimiter));

            var _String = _StringBuilder.ToString();

            if (_String.EndsWith(delimiter))
                _String = _String.Substring(0, _String.Length - delimiter.Length);

            return new Exceptional<List<String>>(new List<String>() { _String });

        }

        private String CreateGraphDDL(DumpFormats myDumpFormat, GraphDBType myGraphDBType, DBContext myDBContext)
        {

            var _StringBuilder = new StringBuilder();
            _StringBuilder.AppendFormat("{0} ", myGraphDBType.Name);

            if (myGraphDBType.ParentTypeUUID != null)
            {

                _StringBuilder.AppendFormat("{0} {1} ", _S_EXTENDS.ToUpperString(), myGraphDBType.GetParentType(myDBContext.DBTypeManager).Name);//builder.AppendLine();

                #region Not backwardEdge attributes

                if (myGraphDBType.GetSpecificAttributes(ta => !ta.IsBackwardEdge).CountIsGreater(0))
                {
                    _StringBuilder.Append(_S_ATTRIBUTES.ToUpperString() + _S_BRACKET_LEFT.ToUpperString() + CreateGraphDDLOfAttributes(myDumpFormat, myGraphDBType.GetSpecificAttributes(ta => !ta.IsBackwardEdge), myDBContext) + _S_BRACKET_RIGHT.ToUpperString() + " ");
                }

                #endregion

                #region BackwardEdge attributes

                if (myGraphDBType.GetSpecificAttributes(ta => ta.IsBackwardEdge).CountIsGreater(0))
                {
                    _StringBuilder.Append(_S_BACKWARDEDGES.ToUpperString() + _S_BRACKET_LEFT.ToUpperString() + CreateGraphDDLOfBackwardEdges(myDumpFormat, myGraphDBType.GetSpecificAttributes(ta => ta.IsBackwardEdge), myDBContext) + _S_BRACKET_RIGHT.ToUpperString() + " ");
                }

                #endregion

                #region Uniques

                if (myGraphDBType.GetUniqueAttributes().CountIsGreater(0))
                {
                    _StringBuilder.Append(_S_UNIQUE.ToUpperString() + _S_BRACKET_LEFT.Symbol + CreateGraphDDLOfAttributeUUIDs(myDumpFormat, myGraphDBType.GetUniqueAttributes(), myGraphDBType) + _S_BRACKET_RIGHT.Symbol + " ");
                }

                #endregion

                #region Mandatory attributes

                if (myGraphDBType.GetMandatoryAttributes().CountIsGreater(0))
                {
                    _StringBuilder.Append(_S_MANDATORY.ToUpperString() + _S_BRACKET_LEFT.Symbol + CreateGraphDDLOfAttributeUUIDs(myDumpFormat, myGraphDBType.GetMandatoryAttributes(), myGraphDBType) + _S_BRACKET_RIGHT.Symbol + " ");
                }

                #endregion

                #region Indices

                if (myGraphDBType.GetAllAttributeIndices(false).CountIsGreater(0))
                {
                    _StringBuilder.Append(_S_INDICES.ToUpperString() + _S_BRACKET_LEFT.Symbol + CreateGraphDDLOfIndices(myDumpFormat, myGraphDBType.GetAllAttributeIndices(false), myGraphDBType) + _S_BRACKET_RIGHT.Symbol + " ");
                }

                #endregion

            }

            return _StringBuilder.ToString();

        }

        private String CreateGraphDDLOfAttributes(DumpFormats myDumpFormat, IEnumerable<TypeAttribute> myTypeAttributes, DBContext myDBContext)
        {

            var _StringBuilder = new StringBuilder();
            var delimiter = ", ";

            foreach (var _Attribute in myTypeAttributes)
            {
                _StringBuilder.Append(CreateGraphDDLOfAttributeDefinition(myDumpFormat, _Attribute, myDBContext));
                _StringBuilder.Append(delimiter);
            }

            if (_StringBuilder.Length > delimiter.Length)
                _StringBuilder.Remove(_StringBuilder.Length - delimiter.Length, 2);

            return _StringBuilder.ToString();

        }

        private String CreateGraphDDLOfAttributeDefinition(DumpFormats myDumpFormat, TypeAttribute myTypeAttribute, DBContext myDBContext)
        {

            if (myTypeAttribute.EdgeType != null)
                return String.Concat(myTypeAttribute.EdgeType.GetGDDL(myTypeAttribute.GetDBType(myDBContext.DBTypeManager)), " ", myTypeAttribute.Name);

            else
                return String.Concat(myTypeAttribute.GetDBType(myDBContext.DBTypeManager).Name, " ", myTypeAttribute.Name);

        }

        private String CreateGraphDDLOfBackwardEdges(DumpFormats myDumpFormat, IEnumerable<TypeAttribute> myTypeAttributes, DBContext myDBContext)
        {

            var _StringBuilder = new StringBuilder();
            var delimiter = ", ";

            foreach (var _Attribute in myTypeAttributes)
            {
                var typeAttrInfos = _Attribute.BackwardEdgeDefinition.GetTypeAndAttributeInformation(myDBContext);
                _StringBuilder.Append(String.Concat(typeAttrInfos.Item1.Name, ".", typeAttrInfos.Item2.Name, " ", _Attribute.Name));
                _StringBuilder.Append(delimiter);
            }

            if (_StringBuilder.Length > delimiter.Length)
                _StringBuilder.Remove(_StringBuilder.Length - delimiter.Length, 2);

            return _StringBuilder.ToString();

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

            var _StringBuilder = new StringBuilder();
            var delimiter = ", ";

            foreach (var _Attribute in myAttributes)
            {
                _StringBuilder.Append(myGraphDBType.GetTypeAttributeByUUID(_Attribute).Name);
                _StringBuilder.Append(delimiter);
            }

            if (_StringBuilder.Length > delimiter.Length)
                _StringBuilder.Remove(_StringBuilder.Length - delimiter.Length, 2);

            return _StringBuilder.ToString();

        }

        /// <summary>
        /// Create the DDL for attributeIndices
        /// </summary>
        /// <param name="myDumpFormat"></param>
        /// <param name="myAttributeIndices"></param>
        /// <param name="indent"></param>
        /// <param name="indentWidth"></param>
        /// <returns></returns>
        private String CreateGraphDDLOfIndices(DumpFormats myDumpFormat, IEnumerable<AttributeIndex> myAttributeIndices, GraphDBType myGraphDBType)
        {

            var _StringBuilder = new StringBuilder();
            var delimiter = ", ";

            foreach (var _AttributeIndex in myAttributeIndices)
            {

                if (_AttributeIndex.IsUuidIndex || _AttributeIndex.IndexEdition == DBConstants.UNIQUEATTRIBUTESINDEX)
                    continue;

                _StringBuilder.Append(String.Concat(_S_BRACKET_LEFT, _AttributeIndex.IndexName));

                if (_AttributeIndex.IsUniqueIndex)
                    _StringBuilder.Append(String.Concat(" ", _S_UNIQUE.ToUpperString()));

                _StringBuilder.Append(String.Concat(" ", _S_EDITION.ToUpperString(), " ", _AttributeIndex.IndexEdition));

                _StringBuilder.Append(String.Concat(" ", _S_INDEXTYPE.ToUpperString(), " ", _AttributeIndex.IndexType.ToString()));
                _StringBuilder.Append(String.Concat(" ", _S_ON.ToUpperString(), " ", CreateGraphDDLOfAttributeUUIDs(myDumpFormat, _AttributeIndex.IndexKeyDefinition.IndexKeyAttributeUUIDs, myGraphDBType)));

                _StringBuilder.Append(_S_BRACKET_RIGHT);

                _StringBuilder.Append(delimiter);

            }

            if (_StringBuilder.Length > delimiter.Length)
                _StringBuilder.Remove(_StringBuilder.Length - delimiter.Length, 2);

            return _StringBuilder.ToString();

        }

        #endregion

        #region Export GraphDML

        /// <summary>
        /// Create the GraphDML of all DBObjects in the database.
        /// </summary>
        /// <param name="myDumpFormat"></param>
        /// <param name="myDBContext"></param>
        /// <param name="objectManager"></param>
        /// <returns></returns>
        public Exceptional<List<String>> ExportGraphDML(DumpFormats myDumpFormat, DBContext myDBContext)
        {

            //var _StringBuilder  = new StringBuilder();
            var _List           = new List<String>();
            var _Exceptional    = new Exceptional<List<String>>();
            var _DBObject_Edges = new List<String>();

            #region Go through each type

            foreach (var _GraphDBType in myDBContext.DBTypeManager.GetAllTypes(false))
            {

                var _IndexReference = _GraphDBType.GetUUIDIndex(myDBContext.DBTypeManager).GetIndexReference(myDBContext.DBIndexManager);

                if (!_IndexReference.Success)
                    return new Exceptional<List<String>>(_IndexReference);

                #region Take UUID index

                foreach (var _IndexEntry in _IndexReference.Value)
                {

                    #region Load DBObject and create GraphDML

                    foreach (var _DBObject in _IndexEntry.Value)
                    {

                        var _DBObjectStream = myDBContext.DBObjectManager.LoadDBObject(_GraphDBType, _DBObject);

                        if (!_DBObjectStream.Success)
                            _Exceptional.AddErrorsAndWarnings(_DBObjectStream);

                        else
                        {

                            var _GDMLExceptional = CreateGraphDMLforDBObject(myDumpFormat, myDBContext, _GraphDBType, _DBObjectStream.Value, _DBObject_Edges);

                            if (!_GDMLExceptional.Success)
                                _Exceptional.AddErrorsAndWarnings(_DBObjectStream);

                            else
                                //_StringBuilder.AppendLine(_GDMLExceptional.Value);
                                _List.Add(_GDMLExceptional.Value);

                        }

                    }

                    #endregion

                }

                #endregion

            }

            #endregion

            #region Append all edges as UPDATE GQL

            // after dumping all objects we will add the edges
            if (_DBObject_Edges.Count > 0)
                //_StringBuilder.AppendLine(_DBObject_Edges.ToString());
                _List.AddRange(_DBObject_Edges);

            #endregion

            //_Exceptional.Value = _StringBuilder.ToString();
            _Exceptional.Value = _List;

            return _Exceptional;

        }

        private Exceptional<String> CreateGraphDMLforDBObject(DumpFormats myDumpFormat, DBContext myDBContext, GraphDBType myGraphDBType, DBObjectStream myDBObjectStream, List<String> myEdges)
        {

            var _StringBuilder = new StringBuilder();
            var delimiter = ", ";

            _StringBuilder.Append(String.Concat(_S_INSERT.ToUpperString(), " ", _S_INTO.ToUpperString(), " ", myGraphDBType.Name, " ", _S_VALUES.ToUpperString(), " ", _S_BRACKET_LEFT));
            _StringBuilder.Append(String.Concat(_S_UUID.ToUpperString(), " = '", myDBObjectStream.ObjectUUID.ToString(), "'", delimiter));

            #region CreateGraphDMLforDBODefinedAttributes

            var edges = new StringBuilder();

            var defAttrsDML = CreateGraphDMLforDBObjectDefinedAttributes(myDumpFormat, myDBObjectStream.GetAttributes(), myGraphDBType, myDBObjectStream, edges, myDBContext);

            if (!defAttrsDML.Success)
                return defAttrsDML;

            _StringBuilder.Append(defAttrsDML.Value);

            #region For edges create UPDATE command

            if (edges.Length > 0)
            {
                edges.RemoveEnding(delimiter);
                myEdges.Add(String.Concat(_S_UPDATE.ToUpperString(), " ", myGraphDBType.Name, " ", _S_SET.ToUpperString(), " ", _S_BRACKET_LEFT, edges.ToString(), _S_BRACKET_RIGHT, " ", _S_WHERE.ToUpperString(), " ", _S_UUID.ToUpperString(), " = '", myDBObjectStream.ObjectUUID.ToString(), "'"));
            }

            #endregion

            #endregion

            #region CreateGDMLforDBOUnDefinedAttributes

            var undefAttrs = myDBObjectStream.GetUndefinedAttributes(myDBContext.DBObjectManager);

            if (!undefAttrs.Success)
                return new Exceptional<String>(undefAttrs);

            if (undefAttrs.Value.Count > 0)
            {

                Exceptional<String> undefAttrsDML = CreateGraphDMLforDBObjectUndefinedAttributes(myDumpFormat, undefAttrs.Value, myGraphDBType, myDBObjectStream);
                
                if (!undefAttrsDML.Success)
                    return undefAttrsDML;

                _StringBuilder.Append(undefAttrsDML.Value);

            }

            #endregion

            _StringBuilder.RemoveEnding(delimiter);
            _StringBuilder.Append(_S_BRACKET_RIGHT);

            return new Exceptional<String>(_StringBuilder.ToString());

        }

        private Exceptional<String> CreateGraphDMLforDBObjectDefinedAttributes(DumpFormats myDumpFormat, IDictionary<AttributeUUID, AObject> myAttributes, GraphDBType myGraphDBType, DBObjectStream myDBObjectStream, StringBuilder myEdgeBuilder, DBContext myDBContext)
        {

            var _StringBuilder = new StringBuilder();
            var delimiter = ", ";

            foreach (var _Attribute in myAttributes)
            {

                if (_Attribute.Value == null)
                    continue;

                var typeAttribute = myGraphDBType.GetTypeAttributeByUUID(_Attribute.Key);

                #region Reference attributes

                if (typeAttribute.GetDBType(myDBContext.DBTypeManager).IsUserDefined)
                {

                    #region SetOfReferences
                    if (_Attribute.Value is ASetReferenceEdgeType)
                    {

                        #region Create edge GDML

                        myEdgeBuilder.Append(String.Concat(typeAttribute.Name, " = ", _S_SETOF.ToUpperString(), " ", _S_BRACKET_LEFT));

                        #region Create an assignment content - if edge does not contain any elements create an empty one

                        if ((_Attribute.Value as ASetReferenceEdgeType).GetEdges().CountIsGreater(0))
                        {

                            #region Create attribute assignments

                            foreach (var val in (_Attribute.Value as ASetReferenceEdgeType).GetEdges())
                            {
                                myEdgeBuilder.Append(String.Concat(_S_UUID.ToUpperString(), " = '", val.Item1.ToString(), "'"));
                                if (val.Item2 != null)
                                {
                                    myEdgeBuilder.Append(String.Concat(_S_colon, _S_BRACKET_LEFT, CreateGraphDMLforADBBaseObject(myDumpFormat, val.Item2), _S_BRACKET_RIGHT));
                                }
                                myEdgeBuilder.Append(delimiter);
                            }
                            myEdgeBuilder.RemoveEnding(delimiter);

                            #endregion

                        }

                        #endregion

                        myEdgeBuilder.Append(_S_BRACKET_RIGHT);

                        #endregion

                    }

                    #endregion

                    #region SingleReference

                    else if (typeAttribute.KindOfType == KindsOfType.SingleReference)
                    {
                        myEdgeBuilder.Append(String.Concat(typeAttribute.Name, " = ", _S_REFERENCE.ToUpperString(), " ", _S_BRACKET_LEFT));
                        myEdgeBuilder.Append(String.Concat(_S_UUID.ToUpperString(), " = '", (_Attribute.Value as ASingleReferenceEdgeType).GetUUID().ToString(), "'"));
                        myEdgeBuilder.Append(_S_BRACKET_RIGHT);
                    }

                    #endregion

                    else
                    {
                        return new Exceptional<String>(new Errors.Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                    }

                    myEdgeBuilder.Append(delimiter);

                }

                #endregion

                #region NonReference attributes

                else
                {

                    #region ListOfNoneReferences

                    if (typeAttribute.KindOfType == KindsOfType.ListOfNoneReferences)
                    {
                        _StringBuilder.Append(String.Concat(typeAttribute.Name, " = ", _S_LISTOF.ToUpperString(), " ", _S_BRACKET_LEFT));
                        foreach (var val in (_Attribute.Value as AListBaseEdgeType))
                        {
                            _StringBuilder.Append(CreateGraphDMLforADBBaseObject(myDumpFormat, val as ADBBaseObject) + delimiter);
                        }
                        _StringBuilder.RemoveEnding(delimiter);
                        _StringBuilder.Append(_S_BRACKET_RIGHT);
                    }

                    #endregion

                    #region SetOfNoneReferences

                    else if (typeAttribute.KindOfType == KindsOfType.SetOfNoneReferences)
                    {
                        _StringBuilder.Append(String.Concat(typeAttribute.Name, " = ", _S_SETOF.ToUpperString(), " ", _S_BRACKET_LEFT));
                        foreach (var val in (_Attribute.Value as AListBaseEdgeType))
                        {
                            _StringBuilder.Append(CreateGraphDMLforADBBaseObject(myDumpFormat, val as ADBBaseObject) + delimiter);
                        }
                        _StringBuilder.RemoveEnding(delimiter);
                        _StringBuilder.Append(_S_BRACKET_RIGHT);

                    }

                    #endregion

                    #region SpecialAttribute

                    else if (typeAttribute.KindOfType == KindsOfType.SpecialAttribute)
                    {
                        throw new GraphDBException(new Errors.Error_NotImplemented(new System.Diagnostics.StackTrace(true)));
                    }

                    #endregion

                    #region Single value

                    else
                    {
                        _StringBuilder.Append(String.Concat(typeAttribute.Name, " = ", CreateGraphDMLforADBBaseObject(myDumpFormat, _Attribute.Value as ADBBaseObject)));
                    }

                    #endregion

                    _StringBuilder.Append(delimiter);

                }

                #endregion

            }

            return new Exceptional<String>(_StringBuilder.ToString());

        }

        private Exceptional<String> CreateGraphDMLforDBObjectUndefinedAttributes(DumpFormats myDumpFormat, IDictionary<String, AObject> myAttributes, GraphDBType myGraphDBType, DBObjectStream myDBObjectStream)
        {

            var _StringBuilder = new StringBuilder();
            var delimiter = ", ";

            foreach (var _Attribute in myAttributes)
            {

                #region A single value...

                if (_Attribute.Value is ADBBaseObject)
                    _StringBuilder.Append(String.Concat(_Attribute.Key, " = ", CreateGraphDMLforADBBaseObject(myDumpFormat, _Attribute.Value as ADBBaseObject)));

                #endregion

                #region ..or, it is a List or Set, since the Set constraint was already verified we can use a list

                else if (_Attribute.Value is AListBaseEdgeType)
                {

                    _StringBuilder.Append(String.Concat(_Attribute.Key, " = ", _S_LISTOF.ToUpperString(), " ", _S_BRACKET_LEFT));

                    foreach (var val in (_Attribute.Value as AListBaseEdgeType))
                        _StringBuilder.Append(CreateGraphDMLforADBBaseObject(myDumpFormat, val as ADBBaseObject) + delimiter);

                    _StringBuilder.RemoveEnding(delimiter);
                    _StringBuilder.Append(_S_BRACKET_RIGHT);

                }

                #endregion

                else
                    return new Exceptional<String>(new Errors.Error_NotImplemented(new StackTrace(true)));

                _StringBuilder.Append(delimiter);

            }

            return new Exceptional<String>(_StringBuilder.ToString());

        }

        private String CreateGraphDMLforADBBaseObject(DumpFormats myDumpFormat, ADBBaseObject myADBBaseObject)
        {

            var _DBNumber = myADBBaseObject as DBNumber;

            if (_DBNumber != null)
                return _DBNumber.ToString(new CultureInfo("en-US"));

            return String.Concat("'", myADBBaseObject.ToString(new CultureInfo("en-US")), "'");

        }

        #endregion

        #endregion   

    }

}
