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
 * GraphFSLookuptable
 * Achim Friedland, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Text;

using sones.Lib.DataStructures;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Objects;

#endregion

namespace sones.GraphFS
{

    public class MountpointLookup
    {


        #region Data

        private readonly Dictionary<ObjectLocation, String>     _SymlinkLookuptable;
        private readonly Dictionary<ObjectLocation, IGraphFS>   _ChildFSLookuptable;

        #endregion

        #region Constructor

        public MountpointLookup()
        {
            _SymlinkLookuptable = new Dictionary<ObjectLocation, String>();
            _ChildFSLookuptable = new Dictionary<ObjectLocation, IGraphFS>();
        }

        #endregion


        #region Set(myObjectLocation, mySymlinkTarget)

        public void Set(ObjectLocation myObjectLocation, String mySymlinkTarget)
        {

            if (_SymlinkLookuptable.ContainsKey(myObjectLocation))
                _SymlinkLookuptable[myObjectLocation] = mySymlinkTarget;

            else
                _SymlinkLookuptable.Add(myObjectLocation, mySymlinkTarget);

        }

        #endregion

        #region Set(myObjectLocation, myChildFS)

        public void Set(ObjectLocation myObjectLocation, IGraphFS myChildFS)
        {

            if (_ChildFSLookuptable.ContainsKey(myObjectLocation))
                _ChildFSLookuptable[myObjectLocation] = myChildFS;

            else
                _ChildFSLookuptable.Add(myObjectLocation, myChildFS);

        }

        #endregion


        #region IsSymlink(myObjectLocation)

        public Boolean IsSymlink(ObjectLocation myObjectLocation)
        {
            return _SymlinkLookuptable.ContainsKey(myObjectLocation);
        }

        #endregion

        #region IsMountpoint(myObjectLocation)

        public Boolean IsMountpoint(ObjectLocation myObjectLocation)
        {
            return _ChildFSLookuptable.ContainsKey(myObjectLocation);
        }

        #endregion


        #region GetSymlink(myObjectLocation)

        public String GetSymlink(ObjectLocation myObjectLocation)
        {

            String _SymlinkTarget = null;

            if (_SymlinkLookuptable.TryGetValue(myObjectLocation, out _SymlinkTarget))
                return _SymlinkTarget;

            return null;

        }

        #endregion

        #region GetChildFS(myObjectLocation)

        public IGraphFS GetChildFS(ObjectLocation myObjectLocation)
        {

            IGraphFS _ChildFS = null;

            if (_ChildFSLookuptable.TryGetValue(myObjectLocation, out _ChildFS))
                return _ChildFS;

            return null;

        }

        #endregion


        #region Symlinks

        public IEnumerable<String> Symlinks
        {
            get
            {
                return _SymlinkLookuptable.Values;
            }
        }

        #endregion

        #region ChildFSs

        public IEnumerable<IGraphFS> ChildFSs
        {
            get
            {
                return _ChildFSLookuptable.Values;
            }
        }

        #endregion

        #region MountedFSs

        public IEnumerable<KeyValuePair<ObjectLocation, IGraphFS>> MountedFSs
        {
            get
            {
                return _ChildFSLookuptable;
            }
        }

        #endregion

        #region Mountpoints

        public IEnumerable<ObjectLocation> Mountpoints
        {
            get
            {
                return _ChildFSLookuptable.Keys;
            }
        }

        #endregion


        #region Remove(myObjectLocation)

        public void Remove(ObjectLocation myObjectLocation)
        {
            //_ObjectLocatorLookuptable.Remove(myObjectLocation);
            // Remove AGraphObjects!
            _SymlinkLookuptable.Remove(myObjectLocation);
            _ChildFSLookuptable.Remove(myObjectLocation);
        }

        #endregion


        #region Clear()

        public void Clear()
        {
            _SymlinkLookuptable.Clear();
            _ChildFSLookuptable.Clear();
        }

        #endregion

    }

}
