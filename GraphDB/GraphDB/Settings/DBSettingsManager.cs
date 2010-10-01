/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
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
* 
*/

/* <id name="GraphDB – Settings" />
 * <copyright file="DBSettingsManager.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Dirk Bludau</developer>
 * <summary></summary>
 */

#region Usings

using System;
using System.Linq;
using System.Collections.Generic;
using sones.Lib.DataStructures.UUID;
using sones.Lib.ErrorHandling;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Errors;
using sones.GraphDB.Structures;
using sones.GraphDB.Errors;
using sones.GraphDB.TypeManagement.BasicTypes;
using sones.GraphFS.Objects;
using sones.GraphFS.Session;
using sones.Lib.DataStructures.Indices;
using sones.GraphDB.Structures.Enums;
using sones.GraphDB.Exceptions;
using sones.GraphDB.TypeManagement;

using sones.GraphDB.Managers.Structures.Setting;

using sones.GraphDB.Result;

#endregion

namespace sones.GraphDB.Settings
{
	#region SettingsParameter
	public class DBSettingsManager : IDisposable
	{

		#region data

		public Dictionary<String, ADBSettingsBase> AllSettingsByName { get; private set; }
		public Dictionary<UUID, ADBSettingsBase> AllSettingsByUUID { get; private set; }

		private IGraphFSSession                         _IGraphFSSession;
		private ObjectLocation                          _DBSettingsLocation;

		#endregion

		#region constructor

		public DBSettingsManager(Dictionary<String, ADBSettingsBase> myReflectorSettings, Dictionary<String, ADBSettingsBase> myDBSettings, IGraphFSSession myIGraphFS, ObjectLocation mySettingsLocation)
		{

			_IGraphFSSession    = myIGraphFS;
			_DBSettingsLocation = mySettingsLocation;

			AllSettingsByName = new Dictionary<string, ADBSettingsBase>();
			AllSettingsByUUID = new Dictionary<UUID, ADBSettingsBase>();

			AddSettings(myReflectorSettings);
			AddSettings(myDBSettings);
		}

		private void AddSettings(Dictionary<string, ADBSettingsBase> mySettings)
		{
			lock (this)
			{
				foreach (var aSetting in mySettings)
				{
					if((!AllSettingsByUUID.ContainsKey(aSetting.Value.ID) && (!AllSettingsByName.ContainsKey(aSetting.Key))))
					{
						AllSettingsByName.Add(aSetting.Key, aSetting.Value);
						AllSettingsByUUID.Add(aSetting.Value.ID, aSetting.Value);
					}
				}
			}
		}

		#endregion

		private Exceptional VerifyReadWriteOperationIsValid(DBContext myDBContext, String myOperation = "")
		{
			var isReadOnlySettingValue = GetSettingValue(new SettingReadonly().Name, myDBContext, TypesSettingScope.DB);
			if (isReadOnlySettingValue.Failed())
			{
				return new Exceptional(isReadOnlySettingValue);
			}

			if ((isReadOnlySettingValue.Value as DBBoolean).GetValue())
			{
				return new Exceptional(new Error_ReadOnlyViolation(myOperation));
			}

			return Exceptional.OK;
		}

		#region public methods

		#region SetPersistentDBSetting(mySetting)

		public Exceptional SetPersistentDBSetting(DBContext myDBContext, ADBSettingsBase mySetting)
		{

			#region Verify that DB is not set to readonly

			var readWriteCheck = VerifyReadWriteOperationIsValid(myDBContext, mySetting.Name);
			if (readWriteCheck.Failed())
			{
				return new Exceptional(readWriteCheck);
			}

			#endregion

			var _SetMetadatumExceptional = _IGraphFSSession.SetMetadatum<ADBSettingsBase>(_DBSettingsLocation, FSConstants.SETTINGSSTREAM, FSConstants.DefaultEdition, mySetting.Name, mySetting, IndexSetStrategy.REPLACE);

			if (_SetMetadatumExceptional != null && _SetMetadatumExceptional.Success())
			{
				return Exceptional.OK;
			}

			return _SetMetadatumExceptional.PushIError(new Error_SettingCouldNotBeSet(mySetting.Name));

		}

		#endregion

		#region GetDBSettingValue(mySettingName)

		public Exceptional<ADBSettingsBase> GetPersistentDBSetting(String mySettingName)
		{
			var _GetMetadatumExceptional = _IGraphFSSession.GetMetadatum<ADBSettingsBase>(_DBSettingsLocation, FSConstants.SETTINGSSTREAM, FSConstants.DefaultEdition, mySettingName);

			if (_GetMetadatumExceptional == null || _GetMetadatumExceptional.Failed() || _GetMetadatumExceptional.Value == null || _GetMetadatumExceptional.Value.Count() == 0)
			{
				return new Exceptional<ADBSettingsBase>();
			}
			else
			{
				return new Exceptional<ADBSettingsBase>((ADBSettingsBase)_GetMetadatumExceptional.Value.First().Clone());
			}
		}

		#endregion

		#region RemoveDBSetting(mySettingName)

		public Exceptional<bool> RemovePersistentDBSetting(DBContext myDBContext, String mySettingName)
		{

			#region Verify that DB is not set to readonly

			var readWriteCheck = VerifyReadWriteOperationIsValid(myDBContext, mySettingName);
			if (readWriteCheck.Failed())
			{
				return new Exceptional<bool>(readWriteCheck);
			}

			#endregion

			var _RemoveMetadatumExceptional = _IGraphFSSession.RemoveMetadata<ADBSettingsBase>(_DBSettingsLocation, FSConstants.SETTINGSSTREAM, FSConstants.DefaultEdition, mySettingName);

			if (_RemoveMetadatumExceptional == null || _RemoveMetadatumExceptional.Failed())
			{
				return _RemoveMetadatumExceptional.Convert<bool>().PushIErrorT(new Error_SettingDoesNotExist(mySettingName));
			}

			//_DBSettings.Remove(mySettingName);

			return new Exceptional<bool>(true);
		}

		#endregion

		#region GetSetting methods

		/// <summary>
		/// Returns the value of a setting
		/// </summary>
		/// <param name="settingName">The name of the setting</param>
		/// <param name="dbContext">The current database context</param>
		/// <param name="typesOfSettingScope">The actual scope</param>
		/// <param name="type">A database type</param>
		/// <param name="attribute">A type attribute</param>
		/// <param name="includingDefaults">Include the default values or take only the settings that has been set</param>
		/// <returns>The value of the setting</returns>
		public Exceptional<ADBBaseObject> GetSettingValue(String settingName, DBContext dbContext, TypesSettingScope typesOfSettingScope, GraphDBType type = null, TypeAttribute attribute = null, bool includingDefaults = true)
		{
			if (AllSettingsByName.ContainsKey(settingName))
			{
				return GetSettingValue(AllSettingsByName[settingName].ID, dbContext, typesOfSettingScope, type, attribute, includingDefaults);
			}
			else
			{
				return new Exceptional<ADBBaseObject>();
			}
		}

		/// <summary>
		/// Returns the value of a setting
		/// </summary>
		/// <param name="settingUUID">The id of the setting</param>
		/// <param name="dbContext">The current database context</param>
		/// <param name="typesOfSettingScope">The actual scope</param>
		/// <param name="type">A database type</param>
		/// <param name="attribute">A type attribute</param>
		/// <param name="includingDefaults">Include the default values or take only the settings that has been set</param>
		/// <returns>The value of the setting</returns>
		public Exceptional<ADBBaseObject> GetSettingValue(UUID settingUUID, DBContext dbContext, TypesSettingScope typesOfSettingScope, GraphDBType type = null, TypeAttribute attribute = null, bool includingDefaults = true)
		{
			if (AllSettingsByUUID.ContainsKey(settingUUID))
			{
				var interestingSetting = AllSettingsByUUID[settingUUID].Get(dbContext, typesOfSettingScope, type, attribute);

				if (!interestingSetting.Success())
				{
					return new Exceptional<ADBBaseObject>(interestingSetting);
				}

				if (interestingSetting.Value == null)
				{
					if (includingDefaults)
					{
						//get default value of setting
						return new Exceptional<ADBBaseObject>(AllSettingsByUUID[settingUUID].Default.Clone());
					}
					else
					{
						return new Exceptional<ADBBaseObject>();
					}
				}
				else
				{
					return new Exceptional<ADBBaseObject>(interestingSetting.Value.Value.Clone());
				}
			}
			else
			{
				return new Exceptional<ADBBaseObject>();
			}
		}

		/// <summary>
		/// Sets a setting
		/// </summary>
		/// <param name="settingName">The name of the setting</param>
		/// <param name="aDBBaseObject">The value of the setting</param>
		/// <param name="context">The current database context</param>
		/// <param name="_ActScope">The actual scope</param>
		/// <param name="type">A database type</param>
		/// <param name="attribute">A type attribute</param>
		public Exceptional SetSetting(string settingName, ADBBaseObject aDBBaseObject, DBContext context, TypesSettingScope _ActScope, GraphDBType type = null, TypeAttribute attribute = null)
		{
			if (AllSettingsByName.ContainsKey(settingName))
			{
				var currentSetting = (ADBSettingsBase)AllSettingsByName[settingName].Clone();

				currentSetting.Value = aDBBaseObject;
				return currentSetting.Set(context, _ActScope, type, attribute);
			}
			else
			{
				return new Exceptional(new Error_SettingDoesNotExist(settingName));
			}

		}

		/// <summary>
		/// Checks if a certain setting name is listed
		/// </summary>
		/// <param name="settingName">The name of the setting</param>
		/// <returns></returns>
		public bool HasSetting(string settingName)
		{
			return AllSettingsByName.ContainsKey(settingName.ToUpper());
		}

		/// <summary>
		/// Returns a single setting
		/// </summary>
		/// <param name="settingName">The name of the setting</param>
		/// <param name="context">The current context</param>
		/// <param name="scope">The scope of the request</param>
		/// <param name="type">A database type</param>
		/// <param name="attribute">A type attribute</param>
		/// <param name="includingDefaults">Include the default values or take only the settings that has been set</param>
		/// <returns>A setting</returns>
		public Exceptional<ADBSettingsBase> GetSetting(string settingName, DBContext context, TypesSettingScope scope, GraphDBType type = null, TypeAttribute attribute = null, bool includingDefaults = true)
		{
			if (AllSettingsByName.ContainsKey(settingName))
			{
				var interestingSetting = AllSettingsByName[settingName].Get(context, scope, type, attribute);

				if (!interestingSetting.Success())
				{
					return new Exceptional<ADBSettingsBase>(interestingSetting);
				}

				if (interestingSetting.Value == null)
				{
					if (includingDefaults)
					{
						return new Exceptional<ADBSettingsBase>((ADBSettingsBase)AllSettingsByName[settingName].Clone());
					}
					else
					{
						return new Exceptional<ADBSettingsBase>();
					}
				}
				else
				{
					return new Exceptional<ADBSettingsBase>((ADBSettingsBase)interestingSetting.Value.Clone());
				}
			}
			else
			{
				return new Exceptional<ADBSettingsBase>(new Error_SettingDoesNotExist(settingName));
			}
		}

		public Exceptional<ADBSettingsBase> GetSetting(string settingName)
		{
			if (AllSettingsByName.ContainsKey(settingName))
			{
				return new Exceptional<ADBSettingsBase>(AllSettingsByName[settingName]);
			}
			else
			{
				return new Exceptional<ADBSettingsBase>(new Error_SettingDoesNotExist(settingName));
			}
		}

		/// <summary>
		/// Returns a single setting
		/// </summary>
		/// <param name="settingUUID">The uuid of the setting</param>
		/// <param name="context">The current context</param>
		/// <param name="scope">The scope of the request</param>
		/// <param name="type">A database type</param>
		/// <param name="attribute">A type attribute</param>
		/// <param name="includingDefaults">Include the default values or take only the settings that has been set</param>
		/// <returns>A setting</returns>
		public Exceptional<ADBSettingsBase> GetSetting(UUID settingUUID, DBContext context, TypesSettingScope scope, GraphDBType type = null, TypeAttribute attribute = null, bool includingDefaults = true)
		{
			if (AllSettingsByUUID.ContainsKey(settingUUID))
			{
				var interestingSetting = AllSettingsByUUID[settingUUID].Get(context, scope, type, attribute);

				if (!interestingSetting.Success())
				{
					return new Exceptional<ADBSettingsBase>(interestingSetting);
				}

				if (interestingSetting.Value == null)
				{
					if (includingDefaults)
					{
						return new Exceptional<ADBSettingsBase>((ADBSettingsBase)AllSettingsByUUID[settingUUID].Clone());
					}
					else
					{
						return new Exceptional<ADBSettingsBase>();
					}
				}
				else
				{
					return new Exceptional<ADBSettingsBase>((ADBSettingsBase)interestingSetting.Value.Clone());
				}
			}
			else
			{
				return new Exceptional<ADBSettingsBase>(new Error_SettingDoesNotExist(settingUUID.ToString()));
			}
		}

		public IEnumerable<ADBSettingsBase> GetAllSettings()
		{
			foreach (var setting in AllSettingsByName)
			{
				yield return setting.Value;
			}
		}

		/// <summary>
		/// Returns all settings
		/// </summary>
		/// <param name="_DBContext">The current context</param>
		/// <param name="typesSettingScope">The scope of the request</param>
		/// <param name="type">A database type</param>
		/// <param name="attribute">A type attribute</param>
		/// <param name="includingDefaults">Include the default values or take only the settings that has been set</param>
		/// <returns>Settings as a dictionary name - setting</returns>
		public Dictionary<string, ADBSettingsBase> GetAllSettings(DBContext _DBContext, TypesSettingScope typesSettingScope, GraphDBType type = null, TypeAttribute attribute = null, bool includingDefaults = true)
		{
			Dictionary<String, ADBSettingsBase> result = new Dictionary<string, ADBSettingsBase>();

			foreach (var aSetting in AllSettingsByName)
			{
				var aExtractedSetting = GetSetting(aSetting.Key, _DBContext, typesSettingScope, type, attribute, includingDefaults);
				if (!aExtractedSetting.Success())
				{
					throw new GraphDBException(new Error_NotImplemented(new System.Diagnostics.StackTrace()));
				}

				if (aExtractedSetting.Value != null)
				{
					/*
					var currentDBSetting = aExtractedSetting.Value.Get(_DBContext, TypesSettingScope.DB);
					if (currentDBSetting.Failed())
					{
						throw new GraphDBException(currentDBSetting.Errors);
					}
					if (currentDBSetting.Value != null)
					{
						result.Add(aSetting.Key, aExtractedSetting.Value);
					}
					*/
					result.Add(aSetting.Key, aExtractedSetting.Value);
				}

			}

			return result;
		}

		/// <summary>
		/// Removes a single setting
		/// </summary>
		/// <param name="context">The current context</param>
		/// <param name="settingName">The name of the setting</param>
		/// <param name="scope">The scope of the request</param>
		/// <param name="type">A database type</param>
		/// <param name="attribute">A type attribute</param>
		/// <returns></returns>
		public Exceptional<bool> RemoveSetting(DBContext context, string settingName, TypesSettingScope scope, GraphDBType type = null, TypeAttribute attribute = null)
		{
			if (AllSettingsByName.ContainsKey(settingName))
			{
				return AllSettingsByName[settingName].Remove(context, scope, type, attribute);
			}
			else
			{
				throw new GraphDBException(new Error_SettingDoesNotExist(settingName));
			}
		}

		#endregion

		#region ExecuteSettingOperation

		/// <summary>
		/// this is whaer the aektschn haeppens
		/// </summary>
		/// <param name="myDBContext"></param>
		/// <returns></returns>
		public QueryResult ExecuteSettingOperation(DBContext myDBContext, ASettingDefinition mySettingDefinition, TypesOfSettingOperation myTypeOfSettingOperation, Dictionary<string, string> mySettings)
		{

			QueryResult result = new QueryResult();

			switch (myTypeOfSettingOperation)
			{
				case TypesOfSettingOperation.GET:

					#region GET

					var extractDataResult = mySettingDefinition.ExtractData(mySettings, myDBContext);
					if (extractDataResult.Failed())
					{
						return new QueryResult(extractDataResult);
					}
					return new QueryResult(extractDataResult.Value);
				
					#endregion

				case TypesOfSettingOperation.SET:

					#region SET

					var setDataResult = mySettingDefinition.SetData(mySettings, myDBContext);
					if (setDataResult.Failed())
					{
						return new QueryResult(setDataResult);
					}
					return new QueryResult(setDataResult.Value);

					#endregion

				case TypesOfSettingOperation.REMOVE:

					#region REMOVE

					var removeDataResult = mySettingDefinition.RemoveData(mySettings, myDBContext);
					if (removeDataResult.Failed())
					{
						return new QueryResult(removeDataResult);
					}
					return new QueryResult(removeDataResult.Value);

					#endregion

				default:
					
					return new QueryResult(new Error_NotImplemented(new System.Diagnostics.StackTrace(true)));

			}

		}

		#endregion

		#endregion

		#region IDisposable

		public void Dispose()
		{
			AllSettingsByName.Clear();
			AllSettingsByUUID.Clear();
		}

		#endregion

	}
	#endregion
}
