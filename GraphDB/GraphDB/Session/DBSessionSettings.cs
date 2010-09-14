/*
 * DBSessionSettings
 * (c) sones GmbH, 2009 - 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using sones.GraphDB.Settings;
using sones.GraphDB.TypeManagement;
using sones.Lib.ErrorHandling;
using sones.GraphFS.Session;
using sones.GraphDB.TypeManagement;

#endregion

namespace sones.GraphDB.Session
{

    public class DBSessionSettings : SessionSettings
    {

        #region Properties

        //new
        //setting
        Dictionary<String, ADBSettingsBase> _SessionSettingsDB = new Dictionary<string, ADBSettingsBase>();

        //setting
        Dictionary<String, ADBSettingsBase> _SessionSettingsSession = new Dictionary<string, ADBSettingsBase>();

        //type, setting
        Dictionary<TypeUUID, Dictionary<String, ADBSettingsBase>> _SessionSettingsType = new Dictionary<TypeUUID, Dictionary<string, ADBSettingsBase>>();

        //type, attribute, setting
        Dictionary<TypeUUID, Dictionary<AttributeUUID, Dictionary<String, ADBSettingsBase>>> _SessionSettingsTypeAttribute = new Dictionary<TypeUUID, Dictionary<AttributeUUID, Dictionary<string, ADBSettingsBase>>>();

        #endregion

        #region Constructors

        public DBSessionSettings()
        {
        }

        public DBSessionSettings(SessionSettings anotherSettings)
            :base(anotherSettings.GetAllSettings())
        {

        }

        public DBSessionSettings(DBSessionSettings anotherSettings)
        {
            //general settings
            foreach (var aSetting in anotherSettings._SessionSettings)
            {
                _SessionSettings.Add(aSetting.Key, aSetting.Value.Clone());
            }

            //db settings
            foreach (var aSetting in anotherSettings._SessionSettingsDB)
            {
                _SessionSettingsDB.Add(aSetting.Key, (ADBSettingsBase)aSetting.Value.Clone());
            }

            //session settings
            foreach (var aSetting in anotherSettings._SessionSettingsSession)
            {
                if (_SessionSettingsDB.ContainsKey(aSetting.Key))
                {
                    _SessionSettingsDB[aSetting.Key] = (ADBSettingsBase)aSetting.Value.Clone();
                }
                else
                {
                    _SessionSettingsDB.Add(aSetting.Key, (ADBSettingsBase)aSetting.Value.Clone());
                }
            }

            //type settings
            foreach (var aType in anotherSettings._SessionSettingsType)
            {
                _SessionSettingsType.Add(aType.Key, new Dictionary<string, ADBSettingsBase>());

                foreach (var aSetting in aType.Value)
                {
                    _SessionSettingsType[aType.Key].Add(aSetting.Key, (ADBSettingsBase)aSetting.Value.Clone());
                }
            }

            //attribute settings
            foreach (var aType in anotherSettings._SessionSettingsTypeAttribute)
            {
                _SessionSettingsTypeAttribute.Add(aType.Key, new Dictionary<AttributeUUID, Dictionary<string, ADBSettingsBase>>());

                foreach (var aAttribute in aType.Value)
                {
                    _SessionSettingsTypeAttribute[aType.Key].Add(aAttribute.Key, new Dictionary<string, ADBSettingsBase>());

                    foreach (var aSetting in aAttribute.Value)
                    {
                        _SessionSettingsTypeAttribute[aType.Key][aAttribute.Key].Add(aSetting.Key, (ADBSettingsBase)aSetting.Value.Clone());
                    }
                }
            }

        }

        #endregion

        #region settings handling

        /// <summary>
        /// Returns a setting that was set for an attribute. If there isn't a direct hit the type, session and db are checked.
        /// </summary>
        /// <param name="settingName">The name of the setting</param>
        /// <param name="typeUUID">The name of the type</param>
        /// <param name="attributeUUID">The attribute name</param>
        /// <returns>A setting.</returns>
        public ADBSettingsBase GetAttributeSetting(string settingName, TypeUUID typeUUID, AttributeUUID attributeUUID)
        {
            if (_SessionSettingsTypeAttribute.ContainsKey(typeUUID))
            {
                if (_SessionSettingsTypeAttribute[typeUUID].ContainsKey(attributeUUID))
                {
                    if (_SessionSettingsTypeAttribute[typeUUID][attributeUUID].ContainsKey(settingName))
                    {
                        return _SessionSettingsTypeAttribute[typeUUID][attributeUUID][settingName];
                    }
                    else
                    {
                        return GetTypeSetting(settingName, typeUUID);
                    }
                }
                else
                {
                    return GetTypeSetting(settingName, typeUUID);
                }
            }
            else
            {
                return GetTypeSetting(settingName, typeUUID);
            }
        }

        /// <summary>
        /// Returns a setting that was set for a type. If there isn't a direct hit the session and db are checked.
        /// </summary>
        /// <param name="settingName">The name of the setting</param>
        /// <param name="typeUUID">The name of the type</param>
        /// <returns>A setting</returns>
        public ADBSettingsBase GetTypeSetting(string settingName, TypeUUID typeUUID)
        {
            if (_SessionSettingsType.ContainsKey(typeUUID))
            {
                if (_SessionSettingsType[typeUUID].ContainsKey(settingName))
                {
                    return _SessionSettingsType[typeUUID][settingName];
                }
                else
                {
                    return GetSessionSetting(settingName);
                }
            }
            else
            {
                return GetSessionSetting(settingName);
            }
        }


        /// <summary>
        /// Returns a setting that was set for this session. If there isn't a direct hit the db settings are checked.
        /// </summary>
        /// <param name="settingName">The name of the setting</param>
        /// <returns>A setting</returns>
        public ADBSettingsBase GetSessionSetting(string settingName, bool recursive = true)
        {
            if (_SessionSettingsSession.ContainsKey(settingName))
            {
                return _SessionSettingsSession[settingName];
            }
            else
            {
                if (recursive)
                {
                    return GetDBSetting(settingName);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Returns a setting that was set for the database within this session.
        /// </summary>
        /// <param name="settingName">The name of the setting</param>
        /// <returns>A setting</returns>
        public ADBSettingsBase GetDBSetting(string settingName)
        {
            if (_SessionSettingsDB.ContainsKey(settingName))
            {
                return _SessionSettingsDB[settingName];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Sets the given setting for the database within this session
        /// </summary>
        /// <param name="setting">The setting to be set</param>
        /// <returns>True for success</returns>
        public Exceptional SetDBSetting(ADBSettingsBase setting)
        {
            lock (_SessionSettingsDB)
            {
                if (_SessionSettingsDB.ContainsKey(setting.Name))
                {
                    _SessionSettingsDB[setting.Name] = setting;
                }
                else
                {
                    _SessionSettingsDB.Add(setting.Name, setting);
                }    
            }

            return Exceptional.OK;
        }

        /// <summary>
        /// Sets the given setting for this session
        /// </summary>
        /// <param name="setting">The setting to be set</param>
        /// <returns>True for success</returns>
        public Exceptional SetSessionSetting(ADBSettingsBase setting)
        {
            lock (_SessionSettingsSession)
            {
                if (_SessionSettingsSession.ContainsKey(setting.Name))
                {
                    _SessionSettingsSession[setting.Name] = setting;
                }
                else
                {
                    _SessionSettingsSession.Add(setting.Name, setting);
                }
            }
            return Exceptional.OK;
        }

        /// <summary>
        /// Sets the given setting for a given type within this session
        /// </summary>
        /// <param name="setting">The setting to be set</param>
        /// <param name="typeUUID">The name of the type</param>
        /// <returns>True for success</returns>
        public Exceptional SetTypeSetting(ADBSettingsBase setting, TypeUUID typeUUID)
        {
            lock (_SessionSettingsType)
            {
                if (_SessionSettingsType.ContainsKey(typeUUID))
                {
                    if (_SessionSettingsType[typeUUID].ContainsKey(setting.Name))
                    {
                        _SessionSettingsType[typeUUID][setting.Name] = setting;
                    }
                    else
                    {
                        _SessionSettingsType[typeUUID].Add(setting.Name, setting);
                    }
                }
                else
                {
                    _SessionSettingsType.Add(typeUUID, new Dictionary<string, ADBSettingsBase>());
                    _SessionSettingsType[typeUUID].Add(setting.Name, setting);
                }    
            }

            return Exceptional.OK;
        }

        /// <summary>
        /// Sets the given setting for a given type/typeattribute within this session
        /// </summary>
        /// <param name="setting">The setting to be set</param>
        /// <param name="typeUUID">The name of the type</param>
        /// <param name="attributeName">The name of the attribute</param>
        /// <returns>True for success</returns>
        public Exceptional SetAttributeSetting(ADBSettingsBase setting, TypeUUID typeUUID, AttributeUUID attributeUUID)
        {
            lock (_SessionSettingsTypeAttribute)
            {
                if (_SessionSettingsTypeAttribute.ContainsKey(typeUUID))
                {
                    if (_SessionSettingsTypeAttribute[typeUUID].ContainsKey(attributeUUID))
                    {
                        if (_SessionSettingsTypeAttribute[typeUUID][attributeUUID].ContainsKey(setting.Name))
                        {
                            _SessionSettingsTypeAttribute[typeUUID][attributeUUID][setting.Name] = setting;
                        }
                        else
                        {
                            _SessionSettingsTypeAttribute[typeUUID][attributeUUID].Add(setting.Name, setting);
                        }
                    }
                    else
                    {
                        _SessionSettingsTypeAttribute[typeUUID].Add(attributeUUID, new Dictionary<string, ADBSettingsBase>());
                        _SessionSettingsTypeAttribute[typeUUID][attributeUUID].Add(setting.Name, setting);
                    }
                }
                else
                {
                    _SessionSettingsTypeAttribute.Add(typeUUID, new Dictionary<AttributeUUID, Dictionary<string, ADBSettingsBase>>());
                    _SessionSettingsTypeAttribute[typeUUID].Add(attributeUUID, new Dictionary<string, ADBSettingsBase>());
                    _SessionSettingsTypeAttribute[typeUUID][attributeUUID].Add(setting.Name, setting);
                }
            }

            return Exceptional.OK;
        }

        /// <summary>
        /// Removes a database setting from this session.
        /// </summary>
        /// <param name="settingName">The name of the setting that should be removed</param>
        /// <returns>True if there was something removed. Otherwise false.</returns>
        public bool RemoveDBSetting(string settingName)
        {
            lock (_SessionSettingsDB)
            {
                return _SessionSettingsDB.Remove(settingName);
            }
        }

        /// <summary>
        /// Removes a type setting from this session
        /// </summary>
        /// <param name="settingName">The name of the setting that should be removed</param>
        /// <param name="typeUUID">The name of the type</param>
        /// <returns>True if there was something removed. Otherwise false.</returns>
        public bool RemoveTypeSetting(string settingName, TypeUUID typeUUID)
        {
            lock (_SessionSettingsType)
            {
                if (_SessionSettingsType.ContainsKey(typeUUID))
                {
                    return _SessionSettingsType[typeUUID].Remove(settingName);
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Removes an attribute setting from this session
        /// </summary>
        /// <param name="settingName">The name of the setting that should be removed</param>
        /// <param name="typeUUID">The name of the type</param>
        /// <param name="attributeUUID">The name of the attribute</param>
        /// <returns>True if there was something removed. Otherwise false.</returns>
        public bool RemoveAttributeSetting(string settingName, TypeUUID typeUUID, AttributeUUID attributeUUID)
        {
            lock (_SessionSettingsTypeAttribute)
            {
                if (_SessionSettingsTypeAttribute.ContainsKey(typeUUID))
                {
                    if (_SessionSettingsTypeAttribute[typeUUID].ContainsKey(attributeUUID))
                    {
                        return _SessionSettingsTypeAttribute[typeUUID][attributeUUID].Remove(settingName);
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Removes a session setting.
        /// </summary>
        /// <param name="settingName">The name of the setting that should be removed</param>
        /// <returns>True if there was something removed. Otherwise false.</returns>
        public bool RemoveSessionSettingReloaded(string settingName)
        {
            lock (_SessionSettingsSession)
            {
                return _SessionSettingsSession.Remove(settingName);
            }
        }

        #endregion
    
    }

}
