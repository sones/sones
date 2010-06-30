#region License
/* **********************************************************************************
 * Copyright (c) Roman Ivantsov
 * This source code is subject to terms and conditions of the MIT License
 * for CLIrony. A copy of the license can be found in the License.txt file
 * at the root of this distribution. 
 * By using this source code in any fashion, you are agreeing to be bound by the terms of the 
 * MIT License.
 * You must not remove this notice from this software.
 * **********************************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;


namespace sones.Lib.Frameworks.CLIrony.Compiler.Lalr {
  // Parser class implements LALR(1) parser DFM. Its behavior is controlled by the state transition graph
  // with root in Data.InitialState. Each state contains a dictionary of parser actions indexed by input 
  // element (token or non-terminal node). 
  public class Parser : IParser {

    #region Constructors
    public Parser(Grammar grammar) {
      ParserControlDataBuilder builder = new ParserControlDataBuilder(grammar);
      Data = builder.Build();
    }
    #endregion

    #region Properties and fields: Data, Stack, _context, Input, CurrentState, LineCount, TokenCount
    public readonly ParserControlData Data;
    public readonly ParserStack Stack = new ParserStack();

    private CompilerContext _context;
    private bool _caseSensitive;

    public IEnumerator<Token> Input {
      [System.Diagnostics.DebuggerStepThrough]
      get { return _input; }
    } IEnumerator<Token> _input;

    public Token CurrentToken  {
      [System.Diagnostics.DebuggerStepThrough]
      get { return _currentToken; }
    } Token  _currentToken;

    public ParserState CurrentState {
      [System.Diagnostics.DebuggerStepThrough]
      get { return _currentState; }
    } ParserState  _currentState;


    public int LineCount {
      [System.Diagnostics.DebuggerStepThrough]
      get { return _currentLine; }
    } int  _currentLine;

    public int TokenCount  {
      [System.Diagnostics.DebuggerStepThrough]
      get { return _tokenCount; }
    } int  _tokenCount;

    #endregion

    #region Events: ParserAction, TokenReceived
    public event EventHandler<ParserActionEventArgs> ActionSelected;
    public event EventHandler<TokenEventArgs> TokenReceived;
    TokenEventArgs _tokenArgs = new TokenEventArgs(null); //declare as field and reuse it to avoid generating garbage

    protected void OnTokenReceived(Token token) {
      if (TokenReceived == null) return;
      _tokenArgs.Token = token;
      TokenReceived(this, _tokenArgs);
    }
    #endregion

    #region Parsing methods
    private void Reset() {
      Stack.Reset();
      _currentState = Data.InitialState;
      _currentLine = 0;
      _tokenCount = 0;
      _context.Errors.Clear();
    }


    TokenList _previewBuffer = new TokenList();
    private Token ReadToken() {
      while (_input.MoveNext()) {
        Token result = _input.Current;
        _tokenCount++;
        _currentLine = result.Span.Start.Line + 1;
        if (TokenReceived != null)
          OnTokenReceived(result);
        if (result.Terminal.IsSet(TermOptions.IsNonGrammar))
          continue;
        return result;
      }//while
      //Return EOF
      return null; 
    }

    private void NextToken() {
      if (_previewBuffer.Count > 0) {
        _currentToken = _previewBuffer[0];
        _previewBuffer.RemoveAt(0);
      } else 
        _currentToken = ReadToken();
      //  if null, we reached end of file; return EOF token.
      if (_currentToken == null) 
        _currentToken = Token.Create(Grammar.Eof, _context, new SourceLocation(0, _currentLine - 1, 0), string.Empty);
      if (_context.OptionIsSet(CompilerOptions.CollectTokens))
        _context.Tokens.Add(_currentToken);
    }//method

    public Token PreviewSymbols(StringList symbols) {
      //First check the preview buffer
      foreach (Token token in _previewBuffer) {
        if (symbols.Contains(token.Text)) return token; 
      }
      //Now read from input while saving all tokens in preview buffer
      Token tkn;
      while((tkn = ReadToken()) != null) {
        _previewBuffer.Add(tkn);
        if (symbols.Contains(tkn.Text)) return tkn; 
      } 
      return null;
    }//method

    public AstNode ParseNonDeterministic(CompilerContext context, List<List<Token>> _TokenStream)
    {

        #region INPUT EXCEPTIONS

        if (context == null || _TokenStream == null)
        {
            throw new ArgumentNullException();
        }

        #endregion

        //an empty TokenStream should result in null
        if (_TokenStream.Count.Equals(0))
        {
            return null;
        }
        else
        {
            if (_TokenStream.Count.Equals(1) && _TokenStream[0][0].Term.Equals(Grammar.Eof))
            {

                return null;

            }
            else
            {

                #region Data

                AstNode result = null;
                List<Token> FilteredTokenStream = null;
                int LastIndex = 0;

                #endregion

                #region _TokenStream extraction

                FilteredTokenStream = CheckPlausibility(context, _TokenStream);

                #endregion

                LastIndex = FilteredTokenStream.Count;

                if (!FilteredTokenStream[LastIndex - 1].Term.Equals(Grammar.Eof))
                    FilteredTokenStream.Add(_TokenStream[LastIndex][0]);

                result = Parse(context, FilteredTokenStream);

                return result;
            }
        }
    }

    public List<String> GetCorrectElements(CompilerContext _CompilerContext, List<List<Token>> _TokenStream)
    {
        #region INPUT EXCEPTIONS

        if (_CompilerContext == null || _TokenStream == null)
        {
            throw new ArgumentNullException();
        }

        #endregion

        #region Data

        List<String> CorrectElements = new List<string>();
        List<Token> FilteredTokenStream = null;

        #endregion

        if (_TokenStream.Count.Equals(0))
        {
            return CorrectElements;
        }
        else
        {
            if (_TokenStream.Count.Equals(1) && _TokenStream[0][0].Term.Equals(Grammar.Eof))
            {
                return CorrectElements;
            }
            else
            {

                FilteredTokenStream = CheckPlausibility(_CompilerContext, _TokenStream);

                foreach (Token aToken in FilteredTokenStream)
                {
                    if (!aToken.Term.Equals(Grammar.Eof))
                    {
                        if (aToken.Term.GetType().Equals(typeof(StringLiteral)))
                        {
                            int CurrentElementLength = aToken.GetContent().Length;
                            String TempString = "";
                            TempString = aToken.GetContent().Insert(0, "'");
                            TempString = TempString.Insert(CurrentElementLength + 1, "'");

                            CorrectElements.Add(TempString);
                        }
                        else
                        {
                            CorrectElements.Add(aToken.GetContent());
                        }
                    }
                }


                return CorrectElements;
            }
        }
    }

    public AstNode Parse(CompilerContext context, IEnumerable<Token> tokenStream) {
      _context = context;
      _caseSensitive = _context.Compiler.Grammar.CaseSensitive;
      Reset();
      _input = tokenStream.GetEnumerator();
      NextToken();
      while (true) {
        if (_currentState == Data.FinalState) {
          AstNode result = Stack[0].Node;
          //Check transient status
          if (result.Term.IsSet(TermOptions.IsTransient) && result.ChildNodes.Count == 1)
            result = result.ChildNodes[0];
          Stack.Reset();
          return result;
        }
        //check for scammer error
        if (_currentToken.IsError()) {
          if (!Recover()) return null; 
          continue;
        }
        //Get action
        ActionRecord action = GetCurrentAction();
        if (action == null) {
          ReportParseError();
          if (!Recover())
            return null; //did not recover
          continue;
        }//action==null

        if (action.HasConflict())
          action = (ActionRecord) Data.Grammar.OnActionConflict(this, _currentToken, action);
        this.OnActionSelected(_currentToken, action);
        switch (action.ActionType) {
          case ParserActionType.Operator:
            if (GetActionTypeForOperation(_currentToken) == ParserActionType.Shift)
              goto case ParserActionType.Shift;
            else
              goto case ParserActionType.Reduce;

          case ParserActionType.Shift:
            ExecuteShiftAction(action);
            break;

          case ParserActionType.Reduce:
            ExecuteReduceAction(action);
            break;
        }//switch
      }//while
    }//Parse
    #endregion

    #region autocompletion
    public List<ParserReturn> GetPossibleTokens(CompilerContext MyCompilerContext, List<List<Token>> tokenStream, String InputString)
    {
        #region INPUT EXCEPTIONS

        if (MyCompilerContext == null || tokenStream == null || InputString == null)
        {
            throw new ArgumentNullException();
        }

        #endregion

        #region Data
        CompilerContext     _CompilerContext    = MyCompilerContext;
        String              _InputString        = InputString;
        List<Token>         FilteredTokenStream = null;
        String              ToBeCompleted       = "";
        Boolean             GetAllTerminals     = true;
        List<ParserReturn>  PossibleTerminalsTemp;
        List<ParserReturn>  PossibleTerminalsResult = new List<ParserReturn>();
        #endregion

        #region collect possible terminals

        //reasonableness of tokenStream
        FilteredTokenStream = CheckPlausibility(MyCompilerContext, tokenStream);

        //get the string that has to be completed
        ToBeCompleted = GetLastWord(FilteredTokenStream, _InputString);

        if (ToBeCompleted.Length.Equals(0)) GetAllTerminals = false;

        //collect all the possible options corresponding to the current state
        PossibleTerminalsTemp = GetPossibleTerminals(MyCompilerContext, FilteredTokenStream, GetAllTerminals);
        
        #endregion

        #region match terminals with input

        //get all matching Terminals that start with the InputString
        foreach (ParserReturn _aReturnValue in PossibleTerminalsTemp)
        {
            if (_aReturnValue.name.ToLower().StartsWith(ToBeCompleted.ToLower()))
            {
                PossibleTerminalsResult.Add(_aReturnValue);
            }
        }

        #endregion

        if (PossibleTerminalsResult.Count.Equals(1))
        {

            #region consider the case of only one possible token

            #region new Name
            String Option = PossibleTerminalsResult[0].name.Split(' ')[0].ToLower();
            int CountChars;
            int FirstNode;

            CountChars = Option.Length - ToBeCompleted.Length;
            FirstNode = ToBeCompleted.Length;

            String OptionSubstring = Option.Substring(FirstNode, CountChars);
            #endregion

            #region Create new Return Value

            ParserReturn newReturnValue = new ParserReturn(OptionSubstring, PossibleTerminalsResult[0].description, PossibleTerminalsResult[0].typeOfLiteral, PossibleTerminalsResult[0].isUsedForAutoCompletion, false);

            #endregion

            PossibleTerminalsResult.RemoveAt(0);
            PossibleTerminalsResult.Add(newReturnValue);

        }

        #endregion

        return PossibleTerminalsResult;
    }

    private List<Token> CheckPlausibility(CompilerContext MyCompilerContext, List<List<Token>> _TokenLists)
    {

        #region data
        _context = MyCompilerContext;
        _caseSensitive = _context.Compiler.Grammar.CaseSensitive;
        List<Token> _tokenStream = new List<Token>();
        Boolean DoNotTakeIt = false;

        //build up temporary token strean
        List<Token> _tokenStreamTemp = null;

        _tokenStreamTemp = SummarizeNonTerminals(_TokenLists, MyCompilerContext);

        if (_tokenStreamTemp.Count > 0)
        {
            if (_tokenStreamTemp[0].Term.Equals(Grammar.SyntaxError) || _tokenStreamTemp[0].Term.Equals(Grammar.Eof))
            {
                return _tokenStream;
            }
        }

        _input = _tokenStreamTemp.GetEnumerator();

        int _actualToken = 0;
        //Token _NextToken = Token.Create(new Terminal("foobar"), _context, new SourceLocation(), "bot");
        Token _NextToken = null;

        int _tokenStreamCount = _tokenStreamTemp.Count;

        #endregion


        #region calibration
        //resets the statemachine
        Reset();

        //point to the first token
        NextToken();
        #endregion

        //checking first element
        if (_currentState.Actions.ContainsKey(_tokenStreamTemp[0].Term.Name.ToLower()))
        {
            _tokenStream.Add(_tokenStreamTemp[0]);

            Boolean Change = true;

            while (true)
            {
                Boolean Ambiguity = false;

                if (_actualToken.Equals(0)) _NextToken = _tokenStreamTemp[_actualToken + 1];

                //if there are more than one possibilities to take as token
                //Todo: expansion of the ambiguity processing for non-literals
                if (_TokenLists[_actualToken + 1].Count > 1) Ambiguity = true;

                if (!_NextToken.Term.Equals(Grammar.Eof) && !_NextToken.Term.Equals(Grammar.SyntaxError) && Change)
                {
                    //we do not see any SyntaxError or the end of the line, so we get the 
                    // possible terminals for the next step from the current action

                    #region moving forward
                    //Get action
                    ActionRecord action = GetCurrentAction();
                    this.OnActionSelected(_currentToken, action);

                    switch (action.ActionType)
                    {
                        case ParserActionType.Shift:
                            ExecuteShiftAction(action);

                            Type _NextTokenType = _NextToken.Term.GetType();

                            #region Ambiguity handling
                            if (Ambiguity)
                            {
                                List<NumberLiteral> currentActionNumberLiterals = new List<NumberLiteral>();
                                List<StringLiteral> currentActionStringLiterals = new List<StringLiteral>();

                                #region Literal handling

                                if (_NextToken.Category.Equals(TokenCategory.Literal))
                                {
                                    Boolean RightChoise = false;

                                    if (_NextTokenType.Equals(typeof(StringLiteral)))
                                    {
                                        #region StringLiteral handling

                                        #region find every possible stringLiteral
                                        foreach (StringLiteral aStringLiteral in FilterTypeFromAction(action.NewState.Actions, typeof(StringLiteral), MyCompilerContext))
                                        {
                                            currentActionStringLiterals.Add(aStringLiteral);
                                        }
                                        #endregion

                                        #region StringLiteral matching
                                        foreach (StringLiteral aStringLiteral in currentActionStringLiterals)
                                        {

                                            if (_NextToken.Terminal.Name.Equals(aStringLiteral.Name))
                                            {
                                                RightChoise = true;
                                                break;
                                            }
                                        }
                                        #endregion

                                        #region choose another StringLiteral
                                        if (!RightChoise)
                                        {
                                            if (currentActionStringLiterals.Count.Equals(0))
                                            {
                                                DoNotTakeIt = true;
                                            }
                                            else
                                            {
                                                foreach (StringLiteral aStringLiteral in currentActionStringLiterals)
                                                {
                                                    String CorrectStringLiteralName = aStringLiteral.Name;

                                                    foreach (Token aPossibleToken in _TokenLists[_actualToken + 1])
                                                    {
                                                        if (aPossibleToken.Terminal.Name.Equals(CorrectStringLiteralName))
                                                        {
                                                            _NextToken = aPossibleToken;
                                                            _tokenStreamTemp.Insert(_actualToken + 1, aPossibleToken);
                                                            _tokenStreamTemp.RemoveAt(_actualToken + 2);
                                                            _input = _tokenStreamTemp.GetEnumerator();
                                                            for (int i = 0; i < _actualToken + 2; i++)
                                                                NextToken();
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }//!RightChoise ?
                                        #endregion

                                        #endregion
                                    }//StringLiteral ?
                                    else
                                    {
                                        #region NumberLiteral handling

                                        #region find every possible numberLiteral
                                        foreach (NumberLiteral aNumberLiteral in FilterTypeFromAction(action.NewState.Actions, typeof(NumberLiteral), MyCompilerContext))
                                        {
                                            currentActionNumberLiterals.Add(aNumberLiteral);
                                        }
                                        #endregion

                                        #region NumberLiteral matching
                                        foreach (NumberLiteral aNumberLiteral in currentActionNumberLiterals)
                                        {

                                            if (_NextToken.Terminal.Name.Equals(aNumberLiteral.Name))
                                            {
                                                RightChoise = true;
                                                break;
                                            }
                                        }
                                        #endregion

                                        #region choose another NumberLiteral
                                        if (!RightChoise)
                                        {
                                            if (currentActionNumberLiterals.Count.Equals(0))
                                            {
                                                DoNotTakeIt = true;
                                            }
                                            else
                                            {
                                                foreach (NumberLiteral aNumberLiteral in currentActionNumberLiterals)
                                                {
                                                    String CorrectNumberLiteralName = aNumberLiteral.Name;

                                                    foreach (Token aPossibleToken in _TokenLists[_actualToken + 1])
                                                    {
                                                        if (aPossibleToken.Terminal.Name.Equals(CorrectNumberLiteralName))
                                                        {
                                                            _NextToken = aPossibleToken;
                                                            _tokenStreamTemp.Insert(_actualToken + 1, aPossibleToken);
                                                            _tokenStreamTemp.RemoveAt(_actualToken + 2);
                                                            _input = _tokenStreamTemp.GetEnumerator();
                                                            for (int i = 0; i < _actualToken + 2; i++)
                                                                NextToken();
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }//!RightChoise ?
                                        #endregion

                                        #endregion
                                    }//NumberLiteral !
                                }//Literal ?

                                #endregion
                            }//Ambiguity ?
                            #endregion

                            //every matching action and every Literal are appended
                            if (action.NewState.Actions.ContainsKey(_NextToken.Text.ToLower()) || (_NextToken.Category.Equals(TokenCategory.Literal) && DoNotTakeIt.Equals(false)))
                            {
                                _tokenStream.Add(_NextToken);
                                _actualToken++;
                            }
                            else
                            {
                                if (!action.ActionType.Equals(ParserActionType.Reduce))
                                {
                                    Change = false;
                                }
                            }

                            _NextToken = _tokenStreamTemp[_actualToken + 1];

                            break;

                        case ParserActionType.Reduce:
                            ExecuteReduceAction(action);
                            break;
                    }//switch
                    #endregion


                }//EOF or SYNTAX_ERROR ?
                else
                {
                    _tokenStream.Add(_tokenStreamTemp[_tokenStreamCount - 1]);
                    break;
                }//else
            }//while
            return _tokenStream;
        }
        else
        {
            //return an EOF
            _tokenStream.Add(_tokenStreamTemp[_tokenStreamCount - 1]);
            return _tokenStream;
        }
    }

    private List<Token> SummarizeNonTerminals(List<List<Token>> _TokenLists, CompilerContext MyCompilerContext)
    {
        #region Data

        List<List<Token>> _TokenCluster = new List<List<Token>>();
        List<Token> _aCluster = new List<Token>();
        List<Token> _aTempCluster = null;
        string OptionDelimiter = "-";
        List<string> _aTempNonTerminalList = new List<string>();

        #endregion

        #region build up temporary token strean
        List<Token> _tokenStreamTemp = new List<Token>();
        foreach (List<Token> _aTokenList in _TokenLists)
        {
            _tokenStreamTemp.Add(_aTokenList[0]);
        }
        #endregion

        #region Clustering some Nonterminals

        foreach (Token aToken in _tokenStreamTemp)
        {
            if (!aToken.Term.Equals(typeof(StringLiteral)) && !aToken.Term.Equals(typeof(NumberLiteral)) && !aToken.GetContent().StartsWith(OptionDelimiter) && !aToken.Term.Equals(Grammar.Eof))
            {
                _aCluster.Add(aToken);
            }
            else
            {
                _aTempCluster = new List<Token>();
                _aTempCluster.AddRange(_aCluster);
                _TokenCluster.Add(_aTempCluster);
                _aCluster.Clear();
                _aCluster.Add(aToken);
            }
        }

        _aTempCluster = new List<Token>();
        _aTempCluster.AddRange(_aCluster);
        _TokenCluster.Add(_aTempCluster);

        #endregion

        #region Shrink Nonterminal-Clusters

        _context = MyCompilerContext;
        _caseSensitive = _context.Compiler.Grammar.CaseSensitive;
        _input = _tokenStreamTemp.GetEnumerator();
        Reset();
        NextToken();

        ActionRecord action = GetCurrentAction();
        this.OnActionSelected(_currentToken, action);

        //there is no action available, so lets have a look at the token-clusters
        if (action == null)
        {
            foreach (KeyValuePair<string, ActionRecord> aPair in _currentState.Actions)
            {
                if (!aPair.Key.EndsWith("\b"))
                {
                    _aTempNonTerminalList.Add(aPair.Key);
                }
            }

            //HACK: we are just looking at the first cluster, the other ambiguities are handled later on
            if (_TokenCluster[0].Count > 0 && _TokenLists.Count > 1)
            {
                foreach (String aNonTerminal in _aTempNonTerminalList)
                {
                    if (aNonTerminal.ToLower().StartsWith(_TokenCluster[0][0].GetContent().ToLower()))
                    {
                        int CountOfElements = _TokenLists.Count;
                        Token errorToken = Token.Create(Grammar.SyntaxError, _context, _TokenCluster[0][0].Location, _TokenCluster[0][0].GetContent());
                        _tokenStreamTemp.Clear();
                        _tokenStreamTemp.Add(errorToken);
                        _tokenStreamTemp.Add(_TokenLists[CountOfElements - 1][0]);
                        break;
                    }
                }
            }
        }
        

        #endregion

        return _tokenStreamTemp;
    }

    private List<Object> FilterTypeFromAction(ActionRecordTable actionRecordTable, Type type, CompilerContext MyCompilerContext)
    {

        List<Object> _currentActionObjects = new List<Object>();

        //Get the right Terminals
        TerminalList _TerminalList = new TerminalList();
        foreach (Terminal _aTerminal in MyCompilerContext.Compiler.Grammar.Terminals)
        {
            if(_aTerminal.GetType().Equals(type))
            {
                _TerminalList.Add(_aTerminal);
            }
        }

        List<string> _currentActionNames = new List<string>();
        foreach (KeyValuePair<string, ActionRecord> _aAction in actionRecordTable)
        {
            if (_aAction.Key.EndsWith("\b") && !_aAction.Key.StartsWith("EOF"))
            {
                _currentActionNames.Add(_aAction.Key);
            }//Nonterminal?
        }

        foreach (Terminal _aTerminal in _TerminalList)
        {
            if (_currentActionNames.Contains(_aTerminal.Key))
            {
                //Todo: override Contains
                Boolean TerminalAlreadyInList = false;
                foreach (Terminal _aSecondTerminal in _currentActionObjects)
                {
                    if (_aSecondTerminal.Key.Equals(_aTerminal.Key))
                    {
                        TerminalAlreadyInList = true;
                        break;
                    }
                }

                if (!TerminalAlreadyInList)
                {
                    _currentActionObjects.Add(_aTerminal);
                }
            }
        }
       

        return _currentActionObjects;
    }// Nexttoken == Stringliteral ? 




    private List<ParserReturn> GetPossibleTerminals(CompilerContext MyCompilerContext, List<Token> FilteredTokenStream, Boolean _GetAllTerminals)
    {
        #region data
                                _context            = MyCompilerContext;
                                _caseSensitive      = _context.Compiler.Grammar.CaseSensitive;
                                _input              = FilteredTokenStream.GetEnumerator();
        ActionRecord            action;
        List<String>            PossibleTerminals   = new List<string>();
        List<String>            RedundantTerminals  = new List<string>();
        String                  aActionKey          = "";
        ActionRecord            aActionValue;
        String                  aActionType         = "";
        List<ParserReturn>      ReturnValues        = new List<ParserReturn>();
        #endregion

        #region calibration
        //resets the statemachine
        Reset();

        //point to the first token
        NextToken();
        #endregion

        #region iterate tokenStream
        while (true)
        {
            #region Wohoo, we are in place
            //Todo: handle literals
            if (_currentToken.Terminal.Name.Equals("EOF"))
            {
                while (ShouldBeReduced(_currentState.Actions))
                {
                    Reduce();
                }

                foreach (KeyValuePair<string, ActionRecord> aAction in _currentState.Actions)
                {
                    aActionKey = aAction.Key;
                    aActionValue = aAction.Value;

                    if (!aActionKey.EndsWith("\b"))
                    {
                        //Try to find terminals
                        PossibleTerminals.Add(aActionKey);
                    }
                    else
                    {
                        //so... maybe it is a literal or a nonterminal that should be shown during autocompletion?
                        foreach (LRItem aActionShiftItem in aActionValue.ShiftItems)
                        {
                            NonTerminal _aNonTerminal = aActionShiftItem.Core.Current as NonTerminal;

                            if (_aNonTerminal != null)
                            {
                                if (_aNonTerminal.IsUsedForAutocompletion)
                                {
                                    PossibleTerminals.Add(_aNonTerminal.Description);

                                    if (!_GetAllTerminals)
                                    {
                                        foreach (String aTerminal in _aNonTerminal.Firsts)
                                        {
                                            RedundantTerminals.Add(aTerminal);
                                        }
                                    }

                                    break;
                                }

                            }//NonTerminal ?
                            else
                            {
                                aActionType = aActionShiftItem.Core.Current.GetType().Name;
                                if (aActionType.Contains("Literal"))
                                {
                                    PossibleTerminals.Add(aActionKey);
                                    break;
                                }
                            }//Literal !
                        }
                    }//else
                }//foreach

                break;
            }
            #endregion

            #region shifting
            //Get action
            action = GetCurrentAction();

            if (action == null)
            {
                //do sth else
            }//action==null

            if (action.HasConflict())
                action = (ActionRecord)Data.Grammar.OnActionConflict(this, _currentToken, action);
            this.OnActionSelected(_currentToken, action);

            switch (action.ActionType)
            {
                case ParserActionType.Operator:
                    if (GetActionTypeForOperation(_currentToken) == ParserActionType.Shift)
                        goto case ParserActionType.Shift;
                    else
                        goto case ParserActionType.Reduce;

                case ParserActionType.Shift:
                    ExecuteShiftAction(action);
                    break;

                case ParserActionType.Reduce:
                    ExecuteReduceAction(action);
                    break;
            }//switch

            #endregion
        }//while
        #endregion

        //Delete redundant Terminals (that have a description)
        foreach (String aRedundantElement in RedundantTerminals)
        {
            if (PossibleTerminals.Contains(aRedundantElement))
            {
                PossibleTerminals.Remove(aRedundantElement);
            }
        }

        #region create real completion info

        foreach (String aPossibleTerminal in PossibleTerminals)
        {
            String description = "";
            String name;
            Boolean isUsedForAutocompletion;
            Type typeOfLiteral = null;
            Boolean foundSth = false;

            if (aPossibleTerminal.EndsWith("\b"))
            {
                int endIndex = aPossibleTerminal.Length;
                name = aPossibleTerminal.Substring(0, endIndex - 1);
                isUsedForAutocompletion = true;
            }
            else
            {
                name = aPossibleTerminal;
                isUsedForAutocompletion = false;
            }

            int terminalIndex = 0;
            foreach (Terminal aTerminal in _context.Compiler.Grammar.Terminals)
            {
                if (aTerminal.Name.Equals(name))
                {
                    foundSth = true;
                    break;
                }
                else
                {
                    terminalIndex++;
                }
            }

            if (foundSth)
            {
                description = _context.Compiler.Grammar.Terminals[terminalIndex].Description;
                typeOfLiteral = _context.Compiler.Grammar.Terminals[terminalIndex].GetType();
            }

            ParserReturn aParserReturn = new ParserReturn(name, description, typeOfLiteral, isUsedForAutocompletion, true);

            ReturnValues.Add(aParserReturn);
        }

        #endregion


        return ReturnValues;
    }

    private bool ShouldBeReduced(ActionRecordTable actionRecordTable)
    {

        foreach (KeyValuePair<string, ActionRecord> aAction in actionRecordTable)
        {
            if (aAction.Value.ActionType.Equals(ParserActionType.Reduce))
            {
                if (aAction.Value.Key.StartsWith("EOF"))
                {
                    return false;
                }

                return true;
            }
        }

        return false;
    }

    private void Reduce()
    {
        String _aActionKey = "";
        ActionRecord _aActionValue;
        ParserActionType _aParserActionType;

        foreach (KeyValuePair<string, ActionRecord> aAction in _currentState.Actions)
        {
            _aActionKey = aAction.Key;
            _aActionValue = aAction.Value;

            _aParserActionType = _aActionValue.ActionType;

            if (_aParserActionType.Equals(ParserActionType.Reduce) && !aAction.Value.Key.StartsWith("EOF"))
            {
                ExecuteReduceAction(_aActionValue);
                break;
            }
        }
    }

    private List<Token> GetPreTokens(List<Token> _Tokens)
    {
        List<Token> FilteredTokenList = new List<Token>();

        foreach (Token _aToken in _Tokens)
        {
            if (!_aToken.IsError() && !_aToken.ValueString.Equals("EOF"))
            {
                FilteredTokenList.Add(_aToken);
            }
        }
        return FilteredTokenList;
    }

    private string GetLastWord(List<Token> tokenStream, string _InputString)
    {
        String LastWord = (string)_InputString.Clone();

        foreach (Token _aToken in tokenStream)
        {
            if (!_aToken.IsError() && !_aToken.ValueString.Equals("EOF"))
            {
                LastWord = LastWord.TrimStart().ToLower().Remove(0, _aToken.Length);
                //LastWord = LastWord.ToLower().Replace(_aToken.GetContent().ToLower(), "");
                LastWord = LastWord.Trim();
            }

        }

        return LastWord.Trim();
    }

    #endregion

    #region Error reporting and recovery
    private void ReportParseError() {
      if (_currentToken.Terminal == Grammar.Eof) {
        _context.ReportError(_currentToken.Location, "Unexpected end of file.");
        return;
      }
      StringSet expectedList = GetCurrentExpectedSymbols();
      string message = this.Data.Grammar.GetSyntaxErrorMessage(_context, expectedList);
      if (message == null)
        message = "Syntax error" + (expectedList.Count == 0 ? "." : ", expected: " + TextUtils.Cleanup(expectedList.ToString(" ")));
      if (_context.OptionIsSet(CompilerOptions.GrammarDebugging))
        message += " (parser state: " + _currentState.Name + ")";
      _context.Errors.Add(new SyntaxError(_currentToken.Location, message));
    }

    #region Comment
    //TODO: This needs more work. Currently it reports all individual symbols most of the time, in a message like
    //  "Syntax error, expected: + - < > = ..."; the better method is to group operator symbols under one alias "operator". 
    // The reason is that code picks expected key list at current(!) state only, 
    // slightly tweaking it for non-terminals, without exploring Reduce roots
    // It is quite difficult to discover grouping non-terminals like "operator" in current structure. 
    // One possible solution would be to introduce "ExtendedLookaheads" in ParserState which would include 
    // all NonTerminals that might follow the current position. This list would be calculated at start up, 
    // in addition to normal lookaheads. 
    #endregion
    private StringSet GetCurrentExpectedSymbols() {
      BnfTermList inputElements = new BnfTermList();
      StringSet inputKeys = new StringSet();
      inputKeys.AddRange(_currentState.Actions.Keys);
      //First check all NonTerminals
      foreach (NonTerminal nt in _context.Compiler.Grammar.NonTerminals) {
        if (!inputKeys.Contains(nt.Key)) continue;
        //nt is one of our available inputs; check if it has an alias. If not, don't add it to element list;
        // because we have already all its "Firsts" keys in the list. 
        // If yes, add nt to element list and remove
        // all its "fists" symbols from the list. These removed symbols will be represented by single nt alias. 
        if (string.IsNullOrEmpty(nt.DisplayName))
          inputKeys.Remove(nt.Key);
        else {
          inputElements.Add(nt);
          foreach(string first in nt.Firsts) 
            inputKeys.Remove(first);
        }
      }
      //Now terminals
      foreach (Terminal term in  _context.Compiler.Grammar.Terminals) {
        if (inputKeys.Contains(term.Key))
          inputElements.Add(term);
      }
      StringSet result = new StringSet();
      foreach(ABnfTerm term in inputElements)
        result.Add(string.IsNullOrEmpty(term.DisplayName)? term.Name : term.DisplayName);
      return result;
    }

    //TODO: need to rewrite, looks ugly
    private bool Recover() {
      //for recovery the current token must be error token, we rely on it
      if (!_currentToken.IsError())
        _currentToken = _context.CreateErrorToken(_currentToken.Location, _currentToken.Text);
      //Check the current state and states in stack for error shift action - this would be recovery state.
      ActionRecord action = GetCurrentAction();
      if (action == null || action.ActionType == ParserActionType.Reduce) {
        while(Stack.Count > 0) {
          _currentState = Stack.Top.State;
          Stack.Pop(1);
          action = GetCurrentAction();
          if (action != null && action.ActionType != ParserActionType.Reduce) 
            break; //we found shift action for error token
        }//while
      }//if
      if (action == null || action.ActionType == ParserActionType.Reduce) 
        return false; //could not find shift action, cannot recover
      //We found recovery state, and action contains ActionRecord for "error shift". Lets shift it.  
      ExecuteShiftAction(action);//push the error token
      // Now shift all tokens from input that can be shifted. 
      // These are the ones that are found in error production after the error. We ignore all other tokens
      // We stop when we find a state with reduce-only action.
      while (_currentToken.Terminal != Grammar.Eof) {
        //with current token, see if we can shift it. 
        action = GetCurrentAction();
        if (action == null) {
          NextToken(); //skip this token and continue reading input
          continue; 
        }
        if (action.ActionType == ParserActionType.Reduce || action.ActionType == ParserActionType.Operator) {
          //we can reduce - let's reduce and return success - we recovered.
          ExecuteReduceAction(action);
          return true;
        }
        //it is shift action, let's shift
        ExecuteShiftAction(action);
      }//while
      return false; // 
    }
    #endregion

    protected void OnActionSelected(Token input, ActionRecord action) {
      Data.Grammar.OnActionSelected(this, _currentToken, action);
      if (ActionSelected != null) {
        ParserActionEventArgs args = new ParserActionEventArgs(this.CurrentState, input, action);
        ActionSelected(this, args);
      }
    }

    #region Misc private methods
    private ActionRecord GetCurrentAction() {
      ActionRecord action = null;
      if (_currentToken.MatchByValue) {
        string key = CurrentToken.Text;
        if (!_caseSensitive)
          key = key.ToLower();
        if (_currentState.Actions.TryGetValue(key, out action))
          return action;
      }
      if (_currentToken.MatchByType && _currentState.Actions.TryGetValue(_currentToken.Terminal.Key, out action))
        return action;
      return null; //action not found
    }
    private ParserActionType GetActionTypeForOperation(Token current) {
      for (int i = Stack.Count - 2; i >= 0; i--) {
        AstNode prevNode = Stack[i].Node;
        if (prevNode == null || prevNode.Precedence == AstNode.NoPrecedence) continue;
        ParserActionType result;
        //if previous operator has the same precedence then use associativity
        if (prevNode.Precedence == current.Precedence) 
          result = current.Terminal.Associativity == Associativity.Left ? ParserActionType.Reduce : ParserActionType.Shift;
        else 
          result = prevNode.Precedence > current.Precedence ? ParserActionType.Reduce : ParserActionType.Shift;
        return result;
      }
      //If no operators found on the stack, do simple shift
      return ParserActionType.Shift;
    }
    private void ExecuteShiftAction(ActionRecord action) {
      Stack.Push(_currentToken, _currentState);
      _currentState = action.NewState;
      NextToken();
    }
    private void ExecuteReduceAction(ActionRecord action) {
      ParserState oldState = _currentState;
      int popCnt = action.PopCount;

      //Get new node's child nodes - these are nodes being popped from the stack 
      AstNodeList childNodes = new AstNodeList();
      for (int i = 0; i < action.PopCount; i++) {
        AstNode child = Stack[Stack.Count - popCnt + i].Node;
        if (child.Term.IsSet(TermOptions.IsPunctuation)) continue;
        //Transient nodes - don't add them but add their childrent directly to grandparent
        if (child.Term.IsSet(TermOptions.IsTransient)) {
          foreach (AstNode grandChild in child.ChildNodes)
            childNodes.Add(grandChild);
          continue; 
        }
        //Add normal child
        childNodes.Add(child);
      }

      //recover state, location and pop the stack
      SourceSpan newNodeSpan;
      if (popCnt == 0) {
        newNodeSpan = new SourceSpan(_currentToken.Location, 0);
      } else {
        SourceLocation firstPopLoc = Stack[Stack.Count - popCnt].Node.Location;
        int lastPopEndPos = Stack[Stack.Count - 1].Node.Span.EndPos;
        newNodeSpan = new SourceSpan(firstPopLoc, lastPopEndPos - firstPopLoc.Position);
        _currentState = Stack[Stack.Count - popCnt].State;
        Stack.Pop(popCnt);
      }
      //Create new node
      AstNode node = CreateNode(action, newNodeSpan, childNodes);
      action.NonTerminal.OnNodeCreated(node);

      // Push node/current state into the stack 
      Stack.Push(node, _currentState);
      //switch to new state
      ActionRecord gotoAction;
      if (_currentState.Actions.TryGetValue(action.NonTerminal.Key, out gotoAction)) {
        _currentState = gotoAction.NewState;
      } else 
        //should never happen
        throw new CompilerException( string.Format("Cannot find transition for input {0}; state: {1}, popped state: {2}", 
              action.NonTerminal, oldState, _currentState));
    }//method

    private AstNode CreateNode(ActionRecord reduceAction, SourceSpan sourceSpan, AstNodeList childNodes) {
      NonTerminal nt = reduceAction.NonTerminal;
      AstNode result;
      NodeArgs nodeArgs = new NodeArgs(_context, nt, sourceSpan, childNodes);

      if (nt.NodeCreator != null) {
        result = nt.NodeCreator(nodeArgs);
        if (result != null)  return result;
      }

      Type defaultNodeType = _context.Compiler.Grammar.DefaultNodeType;
      Type ntNodeType = nt.NodeType ?? defaultNodeType ?? typeof(AstNode);

      // Check if NonTerminal is a list
      // List nodes are produced by .Plus() or .Star() methods of BnfElement
      // In this case, we have a left-recursive list formation production:   
      //     ntList -> ntList + delim? + ntElem
      //  We check if we have already created the list node for ntList (in the first child); 
      //  if yes, we use this child as a result directly, without creating new list node. 
      //  The other incoming child - the last one - is a new list member; 
      // we simply add it to child list of the result ntList node. Optional "delim" node is simply thrown away.
      bool isList = nt.IsSet(TermOptions.IsList);
      if (isList && childNodes.Count > 1 && childNodes[0].Term == nt) {
        result = childNodes[0];
        AstNode newChild = childNodes[childNodes.Count - 1];
        newChild.Parent = result; 
        result.ChildNodes.Add(newChild);
        return result;
      }
      //Check for StarList produced by MakeStarRule; in this case the production is:  ntList -> Empty | Elem+
      // where Elem+ is non-empty list of elements. The child list we are actually interested in is one-level lower
      if (nt.IsSet(TermOptions.IsStarList) && childNodes.Count == 1) {
        childNodes = childNodes[0].ChildNodes;
      }
      // Check for "node-bubbling" case. For identity productions like 
      //   A -> B
      // the child node B is usually a subclass of node A, 
      // so child node B can be used directly in place of the A. So we simply return child node as a result. 
      // TODO: probably need a grammar option to enable/disable this behavior explicitly
      bool canBubble = (Data.Grammar.FlagIsSet(LanguageFlags.BubbleNodes)) && 
        !isList && !nt.IsSet(TermOptions.IsPunctuation) && childNodes.Count == 1 && (childNodes[0].Term is NonTerminal);
      if (canBubble) {
        NonTerminal childNT = childNodes[0].Term as NonTerminal;
        Type childNodeType = childNT.NodeType ?? defaultNodeType ?? typeof(AstNode);
        if (childNodeType == ntNodeType || childNodeType.IsSubclassOf(ntNodeType))
          return childNodes[0];
      }
      // Try using Grammar's CreateNode method
      result = Data.Grammar.CreateNode(_context, reduceAction, sourceSpan, childNodes);
      if (result != null) 
        return result; 

      //Finally create node directly. For perf reasons we try using "new" for AstNode type (faster), and
      // activator for all custom types (slower)
      if (ntNodeType == typeof(AstNode))
        return new AstNode(nodeArgs);

     // if (ntNodeType.GetConstructor(new Type[] {typeof(AstNodeList)}) != null) 
       // return (AstNode)Activator.CreateInstance(ntNodeType, childNodes);
      if (ntNodeType.GetConstructor(new Type[] {typeof(NodeArgs)}) != null) 
        return (AstNode) Activator.CreateInstance(ntNodeType, nodeArgs);
      //The following should never happen - we check that constructor exists when we validate grammar.
      string msg = string.Format(
@"AST Node class {0} does not have a constructor for automatic node creation. 
Provide a constructor with a single NodeArgs parameter, or use NodeCreator delegate property in NonTerminal.", ntNodeType);
      throw new GrammarErrorException(msg);
    }
    #endregion

    #region IParser.GetStateList()
    public string GetStateList() {
      return TextUtils.StateListToText(Data.States);
    }

    #endregion

  }//class

  public class ParserActionEventArgs : EventArgs {
    public ParserActionEventArgs(ParserState state, Token input, ActionRecord action) {
      State = state;
      Input = input;
      Action = action;
    }

    public readonly ParserState State;
    public readonly Token Input;
    public ActionRecord Action;

    public override string ToString() {
      return State + "/" + Input + ": " + Action;
    }
  }//class


}//namespace
