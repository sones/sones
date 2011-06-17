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
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

namespace sones.Library.Commons
{
	/// <summary>
	/// Provides a disposing mechanism for freeing managed resources that can be used when implementing the IDisposable interface.
	/// </summary>
	public class DisposalService
	{
		/// <summary>
		/// Stores the a collection of actions that are to be executed on calling the Dispose method.
		/// </summary>
		private List<Action> _manResDisposals;

		/// <summary>
		/// Stores the object name that is to be used for throwing an ObjectDisposedException.
		/// </summary>
		private String _objectName;

		#region Construction

		/// <summary>
		/// Creates a new instance of the DisposalService class.
		/// </summary>
		/// 
		/// <param>The object name that is to be used by the EnsureNotDisposed method when throwing an ObjectDisposedException.</param>
		/// 
		/// <exception cref="System.ArgumentNullException">
		///		objectName is NULL.
		/// </exception>
		public DisposalService(String objectName)
		{
			if(objectName == null)
				throw new ArgumentNullException("objectName");

			_objectName = objectName;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets a value indicating whether the object has been disposed.
		/// </summary>
		/// 
		/// <value>
		/// true, if the object is in a disposed state; otherwise, false.
		/// </value>
		public bool IsDisposed
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets a value indicating whether the the disposingAction is currently in progress.
		/// </summary>
		/// 
		/// <value>
		/// true, if the disposingAction is currently in progress; otherwise, false.
		/// </value>
		public bool IsDisposing
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets a collection of Actions that are to be invoked on calling Dispose(true).
		/// </summary>
		private IList<Action> ManagedResourceDisposals
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			get
			{
				if(_manResDisposals == null)
				{
					_manResDisposals = new List<Action>();
				}

				return _manResDisposals;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Adds the specified disposing action to this service.
		/// </summary>
		/// 
		/// <param name="disposalAction">The disposal action to be invoked on calling Dispose(true) on this instance.</param>
		/// 
		/// <exception cref="System.ArgumentNullException">
		///		disposalAction is NULL.
		/// </exception>
		public void AddManagedResourceDisposal(Action disposalAction)
		{
			if(disposalAction == null)
				throw new ArgumentNullException("disposingAction");

			this.ManagedResourceDisposals.Insert(0, disposalAction);
		}

		/// <summary>
		/// Invokes the disposal actions that has been added to this service. 
		/// </summary>
		public void Dispose()
		{
			if(IsDisposed || IsDisposing)
				return;

			IsDisposing = true;

			try
			{
				// free managed resources
				foreach(var dispose in ManagedResourceDisposals)
					dispose();
			}
			finally
			{
				IsDisposed = true;
				IsDisposing = false;
			}
		}

		/// <summary>
		/// Ensures that the object is not in a disposed state and throws an ObjectDisposedException in case it has been disposed of.
		/// </summary>
		/// 
		/// <exception cref="System.ObjectDisposedException">
		///		The object is in a disposed state.
		/// </exception>
		public void EnsureNotDisposed()
		{
			if(IsDisposed)
				throw new ObjectDisposedException(_objectName);
		}

		#endregion
	}
}
