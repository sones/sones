/* <id name=”GraphLib – Autocompletion ReturnValue” />
 * <copyright file=”ParserReturn.cs”
 *            company=”sones GmbH”>
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This class is ment to hold some 
 * information concerning further autocompletion.</summary>
 */

using System;


namespace sones.Lib.Frameworks.CLIrony.Compiler.Lalr {
    
    /// <summary>
    /// This class is ment to hold some 
    /// information concerning further autocompletion.
    /// </summary>
    public class ParserReturn
    {
        #region Data

        private String      _name;
        private String      _description;
        private Boolean     _isUsedForAutoCompletion;
        private Boolean     _isComplete;
        private Type        _typeOfLiteral;

        #endregion

        #region Constructor

        public ParserReturn(String name, String description, Type typeOfLiteral, Boolean isUsedForAutoCompletion, Boolean isComplete)
        {
            #region set properties

            _name                       = name;
            _description                = description;
            _isUsedForAutoCompletion    = isUsedForAutoCompletion;
            _isComplete                 = isComplete;
            _typeOfLiteral              = typeOfLiteral;

            #endregion
        }

        #endregion

        #region Getter

        public String   name                        { get { return _name; } }
        public String   description                 { get { return _description; } }
        public Type     typeOfLiteral               { get { return _typeOfLiteral; } }
        public Boolean  isUsedForAutoCompletion     { get { return _isUsedForAutoCompletion; } }
        public Boolean  isComplete                  { get { return _isComplete; } set { _isComplete = value; } }

        #endregion


        #region Equals Overrides
        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            ParserReturn p = obj as ParserReturn;
            if ((System.Object)p == null)
            {
                return false;
            }

            return (this.description == p.description) && (this.isComplete == p.isComplete) && (this.isUsedForAutoCompletion == p.isUsedForAutoCompletion)
                && (this.name == p.name) && (this.typeOfLiteral == p.typeOfLiteral);
        }

        public bool Equals(ParserReturn p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            return (this.description == p.description) && (this.isComplete == p.isComplete) && (this.isUsedForAutoCompletion == p.isUsedForAutoCompletion)
                && (this.name == p.name) && (this.typeOfLiteral == p.typeOfLiteral);
        }

        public static bool operator ==(ParserReturn a, ParserReturn b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Equals(b);
        }

        public static bool operator !=(ParserReturn a, ParserReturn b)
        {
            return !(a == b);
        }

        #endregion

    }


}//namespace
