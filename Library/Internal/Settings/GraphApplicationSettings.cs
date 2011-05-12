/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using sones.Library.Settings.ErrorHandling;

namespace sones.Library.Settings
{
    /// <summary>
	/// The main Application Setting class.
	/// </summary>
    public sealed class GraphApplicationSettings
    {
        #region CurrentSetting

		/// <summary>
		/// Just a helper class to store the setting information to his corresponding name
		/// </summary>
		private sealed class CurrentSetting
		{
			public IGraphSetting              IGraphDSSetting { get; set; }
			public String                     CurrentValue { get; set; }
			public GraphSettingChangingEvent  GraphDSSettingChangingEvent { get; set; }
		}
		
		#endregion

		#region Data

        readonly Dictionary<String,CurrentSetting> _Settings;

		#endregion

		#region Ctor

        /// <summary>
        /// Creates a new GraphApplicationSettings object
        /// </summary>
        /// <param name="mySettingXmlLocation">The location of the settings xml file</param>
        public GraphApplicationSettings(String mySettingXmlLocation = null)
		{
            _Settings = new Dictionary<string, CurrentSetting>();   

            if (mySettingXmlLocation != null)
            {
                LoadXML(mySettingXmlLocation);
            }
		}

		#endregion

		#region Subscribe<T1>

		/// <summary>
		/// Subscribe to an setting. The event will be fired if the setting is changing.
		/// You need to subscribe to the defined setting <typeparamref name="T1"/> to make it known the the manager.
		/// </summary>
		/// <typeparam name="T1">The setting.</typeparam>
		/// <param name="myGraphDSSettingChangingEvent">The event handler.</param>
		/// <example>Subscribe&lt;ObjectCacheCapacitySetting&gt;(myGraphAppSettings_OnSettingChanging);</example>
		/// <returns></returns>
		public void Subscribe<T1>(GraphSettingChangingEvent myGraphDSSettingChangingEvent)
			where T1 : IGraphSetting, new()
		{
			var IGraphDSSetting = new T1();

			lock (_Settings)
			{
				if (!_Settings.ContainsKey(IGraphDSSetting.SettingName))
				{
					_Settings.Add(IGraphDSSetting.SettingName, new CurrentSetting() {
						IGraphDSSetting             = IGraphDSSetting,
						CurrentValue                = IGraphDSSetting.DefaultSettingValue,
						GraphDSSettingChangingEvent = null
					});
				}
				else if (_Settings[IGraphDSSetting.SettingName].IGraphDSSetting == null) // In case the setting was load from file
				{
					_Settings[IGraphDSSetting.SettingName].IGraphDSSetting = IGraphDSSetting;
				}

				_Settings[IGraphDSSetting.SettingName].GraphDSSettingChangingEvent += myGraphDSSettingChangingEvent;
			}
		}

		#endregion

		#region UnSubscribe<T1>

		/// <summary>
		/// UnSubscribe from a setting.
		/// </summary>
		/// <typeparam name="T1">The setting.</typeparam>
		/// <param name="myGraphDSSettingChangingEvent">The event handler.</param>
		/// <example>Subscribe&lt;ObjectCacheCapacitySetting&gt;(myGraphAppSettings_OnSettingChanging);</example>
		/// <returns></returns>
		public void UnSubscribe<T1>(GraphSettingChangingEvent myGraphDSSettingChangingEvent)
			where T1 : IGraphSetting, new()
		{
			var IGraphDSSetting = new T1();

			lock (_Settings)
			{
				if (_Settings.ContainsKey(IGraphDSSetting.SettingName) && _Settings[IGraphDSSetting.SettingName].IGraphDSSetting != null)
				{
					_Settings[IGraphDSSetting.SettingName].GraphDSSettingChangingEvent -= myGraphDSSettingChangingEvent;
				}
			}
		}

		#endregion

		#region Set<T1>

		/// <summary>
		/// Sets the setting <typeparamref name="T1"/> to the value <paramref name="mySettingValue"/>.
		/// </summary>
		/// <typeparam name="T1">The type of the setting.</typeparam>
		/// <param name="mySettingValue">The setting value. This must be convertible to the type of the setting value.</param>
		/// <example>GraphAppSettings.Set&lt;ObjectCacheCapacitySetting&gt;(1234567UL);</example>
		/// <returns>Any occured error or successfull.</returns>
		public void Set<T1>(String mySettingValue)
			where T1 : IGraphSetting, new()
		{

			var IGraphDSSetting = new T1();

			#region Verify matching value type

			if (!IGraphDSSetting.IsValidValue(mySettingValue))
			{
                throw new DataTypeMismatchException(IGraphDSSetting.SettingType, mySettingValue.GetType());
			}

			#endregion

			#region Apply setting

			ApplySetting(IGraphDSSetting, mySettingValue);

			#endregion
		}

		#endregion

		#region Get<T1>

		/// <summary>
		/// Get the value of setting <typeparamref name="T1"/>.
		/// </summary>
		/// <typeparam name="T1">The type of the setting.</typeparam>
		/// <example>GraphAppSettings.Get&lt;ObjectCacheCapacitySetting&gt;();</example>
		/// <returns>The value of the setting either a previously set value or the default value (if there is no valid value).</returns>
		public String Get<T1>()
			where T1 : IGraphSetting, new()
		{

			var IGraphDSSetting = new T1();

			lock (_Settings)
			{
				if (!_Settings.ContainsKey(IGraphDSSetting.SettingName))
				{
					return IGraphDSSetting.DefaultSettingValue;
				}

				var curValue = _Settings[IGraphDSSetting.SettingName].CurrentValue;
				if (!IGraphDSSetting.IsValidValue(curValue))
				{
					return IGraphDSSetting.DefaultSettingValue;
				}
				else
				{
					return curValue;
				}
			}

		}
		
		#endregion

		#region ApplySetting

		private void ApplySetting(IGraphSetting IGraphDSSetting, String mySettingValue)
		{

			#region Apply setting

			lock (_Settings)
			{
				if (!_Settings.ContainsKey(IGraphDSSetting.SettingName))
				{
					_Settings.Add(IGraphDSSetting.SettingName, new CurrentSetting() { IGraphDSSetting = IGraphDSSetting, CurrentValue = mySettingValue, GraphDSSettingChangingEvent = null });
				}
                else if (_Settings[IGraphDSSetting.SettingName].GraphDSSettingChangingEvent != null)
                {
                    var curSet = _Settings[IGraphDSSetting.SettingName];
                    curSet.GraphDSSettingChangingEvent(new SettingChangingEventArgs(curSet.IGraphDSSetting, mySettingValue));

                    curSet.CurrentValue = mySettingValue;
                }
                else
                {
                    _Settings[IGraphDSSetting.SettingName].CurrentValue = mySettingValue;
                }
			}

			#endregion
		}

		#endregion

		#region Load/Save XML
 
		public void LoadXML(String mySettingFileLocation)
		{

			if (!System.IO.File.Exists(mySettingFileLocation))
			{
                return;
			}

			try
			{

				var xmlDocument = XDocument.Load(mySettingFileLocation);

				#region GraphAppSettings element

				if (xmlDocument.Element("GraphAppSettings") == null)
				{
                    throw new InvalidXMLFormatException("missing \"GraphAppSettings\" element");
				}

				var GraphAppSettingsElement = xmlDocument.Element("GraphAppSettings");

				#endregion

				#region version

				if (GraphAppSettingsElement.Attribute("version") == null)
				{
                    throw new InvalidXMLFormatException("missing \"version\" attribute");
				}

				var versionAttribute = GraphAppSettingsElement.Attribute("version").Value;
				double version = 1.0;
				if (!Double.TryParse(versionAttribute, System.Globalization.NumberStyles.Float, new System.Globalization.CultureInfo("en-US"), out version))
				{
                    throw new InvalidXMLFormatException("invalid version \"" + versionAttribute + "\" ");
				}

				#endregion

				foreach (var settingElement in GraphAppSettingsElement.Elements("setting"))
				{

					#region Setting name

					if (settingElement.Attribute("name") == null)
					{
						continue;
					}
					var settingName = settingElement.Attribute("name").Value;

					#endregion

					#region Setting value

					if (settingElement.Attribute("value") == null)
					{
						continue;
					}
					var settingValue = settingElement.Attribute("value").Value;

					#endregion

					#region setting type

					//if (settingElement.Attribute("type") == null)
					//{
					//    continue;
					//}

					//var settingtype = Type.GetType(settingElement.Attribute("type").Value);

					#endregion

					#region Set value and apply setting

					if (!_Settings.ContainsKey(settingName))
					{
						_Settings.Add(settingName, new CurrentSetting());
					}
					_Settings[settingName].CurrentValue = settingValue;

					if (_Settings[settingName].GraphDSSettingChangingEvent != null)
					{
						System.Diagnostics.Debug.Assert(_Settings[settingName].IGraphDSSetting != null);

						ApplySetting(_Settings[settingName].IGraphDSSetting, settingValue);
					}

					#endregion

				}

			}
			catch (Exception ex)
			{
                throw new CouldNotLoadSettingFileException(ex.ToString());
			}
		}

		public void SaveXML(String mySettingFileLocation)
		{

			var xmlDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"));
			var sones       = new XElement("GraphAppSettings", new XAttribute("version", "1.0"));

			foreach (var settingKV in _Settings)
			{
                var setting = settingKV.Value;
                if (setting.IGraphDSSetting != null)
                {
                    sones.Add(new XElement("setting", new XAttribute("name", setting.IGraphDSSetting.SettingName), new XAttribute("value", setting.CurrentValue), new XAttribute("type", setting.IGraphDSSetting.SettingType.AssemblyQualifiedName)));
                }
			}

			xmlDocument.Add(sones);
			xmlDocument.Save(mySettingFileLocation);
		}

		#endregion

        #region Reset

        /// <summary>
        /// Removes all subscribers
        /// </summary>
        public void UnsubscribeAll()
        {
            lock (_Settings)
            {
                foreach (var set in _Settings)
                {
                    set.Value.GraphDSSettingChangingEvent = null;
                }
            }
        }
        
        #endregion

        /// <summary>
        /// Copies all settings from <paramref name="myGraphAppSettings"/> to the current settings. Any subscribers will be called.
        /// </summary>
        /// <param name="myGraphAppSettings"></param>
        public void Apply(GraphApplicationSettings myGraphAppSettings)
        {
            foreach (var setting in myGraphAppSettings._Settings)
            {
                ApplySetting(setting.Value.IGraphDSSetting, setting.Value.CurrentValue);
            }
        }
    }
}
