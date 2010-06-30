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
 * PandoraFSLookup
 * Achim Friedland, 2010
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Text;

using sones.Lib.DataStructures;
using sones.GraphFS.DataStructures;

#endregion

namespace sones.TmpFS.Caches
{

    public class PandoraFSLookup
    {


        #region Properties

        #region ObjectLocator

        private ObjectLocator _ObjectLocator;

        public ObjectLocator ObjectLocator
        {

            get
            {
                return _ObjectLocator;
            }

            set
            {
                _ObjectLocator = value;
                _Symlink = null;
                _ChildFS = null;
            }

        }

        #endregion


        #region Symlink

        private ObjectLocation _Symlink;

        public ObjectLocation Symlink
        {

            get
            {
                return _Symlink;
            }

            set
            {
                _ObjectLocator = null;
                _Symlink = value;
                _ChildFS = null;
            }

        }

        #endregion

        #region isSymlink

        public Boolean isSymlink
        {
            get
            {
                return _Symlink == null ? false : true;
            }
        }

        #endregion


        #region ChildFS

        private IGraphFS _ChildFS;

        public IGraphFS ChildFS
        {

            get
            {
                return _ChildFS;
            }

            set
            {
                _ObjectLocator = null;
                _Symlink = null;
                _ChildFS = value;
            }

        }

        #endregion

        #region isChildFS

        public Boolean isChildFS
        {
            get
            {
                return _ChildFS == null ? false : true;
            }
        }

        #endregion

        #endregion


        #region Constructors

        #region PandoraFSLookup()

        public PandoraFSLookup()
        {
            _ObjectLocator  = null;
            _Symlink        = null;
            _ChildFS        = null;
        }

        #endregion

        #region PandoraFSLookup(myObjectLocator)

        public PandoraFSLookup(ObjectLocator myObjectLocator)
        {
            _ObjectLocator  = myObjectLocator;
            _Symlink        = null;
            _ChildFS        = null;
        }

        #endregion

        #region PandoraFSLookup(mySymlink)

        public PandoraFSLookup(ObjectLocation mySymlink)
        {
            _ObjectLocator  = null;
            _Symlink        = mySymlink;
            _ChildFS        = null;
        }

        #endregion

        #region PandoraFSLookup(myChildFS)

        public PandoraFSLookup(IGraphFS myChildFS)
        {
            _ObjectLocator  = null;
            _Symlink        = null;
            _ChildFS        = myChildFS;
        }

        #endregion

        #endregion


    }

}
