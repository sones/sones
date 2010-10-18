using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Xml.Linq;

using sones.Lib.Settings;
using sones.Lib.Settings.Errors;
using sones.Lib.ErrorHandling;

namespace sones.Lib.Settings
{

	/// <summary>
	/// The main Application Setting class.
	/// </summary>
	public class GraphAppSettings
	{

		#region CurrentSetting

		/// <summary>
		/// Just a helper class to store the setting information to his corresponding name
		/// </summary>
		private class CurrentSetting
		{
			public IGraphSetting              IGraphDSSetting { get; set; }
			public String                       CurrentValue { get; set; }
			public GraphSettingChangingEvent  GraphDSSettingChangingEvent { get; set; }
		}
		
		#endregion

		#region Data

		Dictionary<String,CurrentSetting> _Settings;

		#endregion

		#region Ctor

		public GraphAppSettings()
		{
			_Settings = new Dictionary<string, CurrentSetting>();
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
		public Exceptional Subscribe<T1>(GraphSettingChangingEvent myGraphDSSettingChangingEvent)
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

			return new Exceptional();
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
		public Exceptional Set<T1>(String mySettingValue)
			where T1 : IGraphSetting, new()
		{

			var IGraphDSSetting = new T1();

			#region Verify matching value type

			if (!IGraphDSSetting.IsValidValue(mySettingValue))
			{
				return new Exceptional(new GraphSettingError_DataTypeMismatch(IGraphDSSetting.SettingType, mySettingValue.GetType()));
			}

			#endregion

			#region Apply setting

			ApplySetting(IGraphDSSetting, mySettingValue);

			#endregion

			return new Exceptional();

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

		private Exceptional ApplySetting(IGraphSetting IGraphDSSetting, String mySettingValue)
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
					var result = curSet.GraphDSSettingChangingEvent(new GraphSettingChangingEventArgs(curSet.IGraphDSSetting, mySettingValue));

					if (result.Success())
					{
						curSet.CurrentValue = mySettingValue;
					}
					else
					{
						return result;
					}
				}
			}

			#endregion

			return new Exceptional();

		}

		#endregion

		#region Load/Save XML
 
		public Exceptional LoadXML(String mySettingFileLocation)
		{

			if (!System.IO.File.Exists(mySettingFileLocation))
			{
				return new Exceptional();
			}

			try
			{

				var xmlDocument = XDocument.Load(mySettingFileLocation);

				#region GraphAppSettings element

				if (xmlDocument.Element("GraphAppSettings") == null)
				{
					return new Exceptional(new GraphSettingError_InvalidXMLFormat("missing \"GraphAppSettings\" element"));
				}

				var GraphAppSettingsElement = xmlDocument.Element("GraphAppSettings");

				#endregion

				#region version

				if (GraphAppSettingsElement.Attribute("version") == null)
				{
					return new Exceptional(new GraphSettingError_InvalidXMLFormat("missing \"version\" attribute"));
				}

				var versionAttribute = GraphAppSettingsElement.Attribute("version").Value;
				double version = 1.0;
				if (!Double.TryParse(versionAttribute, System.Globalization.NumberStyles.Float, new System.Globalization.CultureInfo("en-US"), out version))
				{
					return new Exceptional(new GraphSettingError_InvalidXMLFormat("invalid version \"" + versionAttribute + "\" "));
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
				return new Exceptional(new GraphSettingError_CouldNotLoadSettingsFile(ex.ToString()));
			}

			return new Exceptional();

		}

		public Exceptional SaveXML(String mySettingFileLocation)
		{

			var xmlDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"));
			var sones       = new XElement("GraphAppSettings", new XAttribute("version", "1.0"));

			foreach (var settingKV in _Settings)
			{

				var setting = settingKV.Value;
				sones.Add(new XElement("setting", new XAttribute("name", setting.IGraphDSSetting.SettingName), new XAttribute("value", setting.CurrentValue), new XAttribute("type", setting.IGraphDSSetting.SettingType.AssemblyQualifiedName)));

			}

			xmlDocument.Add(sones);
			xmlDocument.Save(mySettingFileLocation);

			return new Exceptional();

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

    }


}
