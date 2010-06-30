/*
 * GraphFS - ISessionInfo
 * (c) Achim Friedland, 2009 - 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using sones.Pandora.Lib.Settings;


#endregion

namespace sones.GraphFS.Session
{

    public class SessionToken
    {

        private ISessionInfo _SessionInfo;

        public ISessionInfo SessionInfo
        {
            get { return _SessionInfo; }
            set { _SessionInfo = value; }
        }





        public SessionToken(ISessionInfo mySessionInfo)
        {
            _SessionInfo = mySessionInfo;
            _Transactions = new LinkedList<Transaction>();
            _SessionSettings = new Dictionary<string, ISettings>();
        }


        #region Transactions

        private LinkedList<Transaction> _Transactions;

        public Transaction AddTransaction(Transaction myTransaction)
        {
            return _Transactions.AddLast(myTransaction).Value;
        }

        public void RemoveTransaction(Transaction myTransaction)
        {
            if (myTransaction != null)
                _Transactions.Remove(myTransaction);
        }

        public Transaction RemoveTransaction()
        {
            Transaction retVal = Transaction;
            if (retVal != null)
                _Transactions.Remove(retVal);

            return retVal;
        }

        public Transaction Transaction
        {
            get
            {
                if (_Transactions.Last == null)
                    return null;
                return _Transactions.Last.Value;
            }
        }

        public Int64 CurrentNestedTransactionCount
        {
            get
            {
                return _Transactions.Count;
            }
        }

        #endregion

        #region Settings

        //public Dictionary<String, ISettings> SessionSettings
        //{
        //    get
        //    {
        //        return _SessionSettings;
        //    }
        //}

        private Dictionary<String, ISettings> _SessionSettings;

        public void AddSessionSetting(string myName, ISettings mySetting)
        {
            if (!_SessionSettings.ContainsKey(myName))
                _SessionSettings.Add(myName, mySetting.Clone());
            else
                ChangeSessionSetting(myName, mySetting);
        }

        public void RemoveSessionSetting(string myName)
        {
            _SessionSettings.Remove(myName);
        }

        public void ChangeSessionSetting(string myName, ISettings mySetting)
        {
            if (_SessionSettings.ContainsKey(myName))
                _SessionSettings[myName] = (ISettings)mySetting.Clone();
            else
                AddSessionSetting(myName, mySetting);
        }

        public Dictionary<string, ISettings> GetAllSettings()
        {
            Dictionary<string, ISettings> RetVal = new Dictionary<string, ISettings>();
            foreach (var Setting in _SessionSettings)
                RetVal.Add(Setting.Key, Setting.Value.Clone());

            return RetVal;
        }

        public ISettings GetSettingValue(string myName)
        {
            Dictionary<string, ISettings> RetVal = GetAllSettings();

            if (RetVal.ContainsKey(myName))
                return RetVal[myName].Clone();

            return null;
        }

        #endregion


        public SessionInfos ToIronySessionToken()
        {

            var _IronySessionToken = new SessionInfos();

            _IronySessionToken.SessionSettings = _SessionSettings;

            //foreach (var item in GetAllSettings())
            //{
            //    _IronySessionToken.AddSessionSetting(item.Key, item.Value);
            //}

            return _IronySessionToken;

        }


    }
}
