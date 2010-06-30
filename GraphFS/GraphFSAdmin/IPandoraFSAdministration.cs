/* <id Name="PandoraFS - Administration Interface" />
 * <copyright file="IPandoraFSAdministration.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Daniel Kirstenpfad</developer>
 * <summary>
 * 
 * This code is just here to have an interface definition to code along with.
 * 
 * </summary>
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace sones.GraphFS.Administration
{
    #region stubs just here to compile
    
    public class StorageEngineStatus
    {    
    }

    
    public class FSStatus
    {
    }

    
    public class DirectoryListing
    {
    }
    #endregion

    /// <summary>
    /// this is just an Interface declaration that contains needed method needed by the
    /// Administration Interface
    /// 
    /// An Administration Interface is exported over WCF and may be implemented by PandoraFS
    /// </summary>
    [ServiceContract()]
    public interface IPandoraFSAdministration
    {
        /// <summary>
        /// Initiates the shutdown of this filesystem.
        /// Fails if anything is mounted and ForceUnmountAll == false
        /// Unmounts everything when something is mounted and ForceUnmountAll == true
        /// </summary>
        /// <param name="ForceUnmountAll">force a shutdown by unmounting everything</param>
        [OperationContract()]
        void Shutdown(bool ForceUnmountAll);

        /// <summary>
        /// Lists all known Storage Engines
        /// </summary>
        /// <returns>a list of Storage Engines</returns>
        [OperationContract()]
        List<String> ListKnownStorageEngines();

        /// <summary>
        /// Lists all Storage Engines that are associated and used by this FS
        /// </summary>
        /// <returns>a list of Storage Engines</returns>
        [OperationContract()]
        List<String> ListAssociatedStorageEngines();

        /// <summary>
        /// Attaches a running Storage Engine to the Filesystem. This Storage Engine was previously installed.
        /// </summary>
        /// <param name="StorageEngineID">the ID of the Storage Engine that needs to be added</param>
        /// <returns>true if completed successfully, false if error</returns>
        [OperationContract()]
        bool AttachStorageEngine(String StorageEngineID);

        /// <summary>
        /// Detaches a Storage Engine from the Filesystem
        /// </summary>
        /// <param name="StorageEngineID">the ID of the Storage Engine that gets detached</param>
        /// <param name="forceFully">if the Storage Engine is still in use true means that it will be disconnected wether or not anything is prepared for this. If false it'll be cleared and removed with all consequences.</param>
        /// <returns>true if completed successfully, false if error</returns>
        [OperationContract()]
        bool DetachStorageEngine(String StorageEngineID, bool DetachForcefully);

        /// <summary>
        /// Mounts an existing Storage Engine into the given Mountpoint inside the currently running Filesystem
        /// </summary>
        /// <param name="StorageEndingID">the ID of the Storage Engine that gets mounted</param>
        /// <param name="Mountpoint">the mountpoint of this Storage Engine</param>
        /// <returns>true if completed successfully, false if error</returns>
        [OperationContract()]
        bool MountStorageEngine(String StorageEndingID, String Mountpoint);

        /// <summary>
        /// Returns the status of one particular Storage Engine
        /// </summary>
        /// <param name="StorageEngineID">the ID of the Storage Engine</param>
        /// <returns></returns>
        [OperationContract()]
        StorageEngineStatus GetStorageEngineStatus(String StorageEngineID);

        /// <summary>
        /// Returns the overall status of this filesystem
        /// Contains:
        ///  - Startup-DateTime
        ///  - Status (= OK, WARNING,...)
        ///  - All current System Warning messages (like disk space warnings, storage engine failures,...)
        /// </summary>
        /// <returns></returns>
        [OperationContract()]
        FSStatus GetFilesystemStatus();

        /// <summary>
        /// Returns the number of free bytes at the given location / path
        /// </summary>
        /// <param name="Location">the path/location</param>
        /// <returns></returns>
        [OperationContract()]
        long GetFreeSpaceAtLocation(String Location);

        /// <summary>
        /// Returns the listing of this directory, including the Name and number of available streams per object
        /// </summary>
        /// <param name="Location">the path/location</param>
        /// <returns>the listing containing names, data-stream-lists, sizes,...</returns>
        [OperationContract()]
        DirectoryListing GetDirectoryListing(String Location);
    }
}
