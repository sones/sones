//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using sones.Lib.CLI
//using System.Collections.Generic;
//using sones.GraphDB;
//using sones.Graph.Storage;
//using sones.Graph.Storage.GraphFS.Session;
//using System;
//using System.Diagnostics;
//using System.IO;
//
//using sones.Graph.Storage.GraphFS;
//
//using sones.Lib;
//using sones.Graph.Storage.GraphFS.Datastructures;
//using sones.Lib.DataStructures;
//using sones;
//using sones.GraphFS.Session;

//namespace GraphDatabaseTests
//{
    
    
//    /// <summary>
//    ///This is a test class for DBCLI_CREATETest and is intended
//    ///to contain all DBCLI_CREATETest Unit Tests
//    ///</summary>
//    [TestClass()]
//    public class DBCLI_Tests
//    {


//        private IGraphDBSession _IGraphDBSession;
//        private IGraphFS _IGraphFS;
//        private IGraphFSSession _IGraphFS2Session;
//        private Object _IGraphFS2SessionObj;
//        private Object _IGraphDBSessionObj;
//        private String myGUID;
//        private String _CurrentPath;

//        private TestContext testContextInstance;

//        private GraphCLI _GraphCLI;
//        private MemoryStream _MemoryStream;

//        /// <summary>
//        ///Gets or sets the test context which provides
//        ///information about and functionality for the current test run.
//        ///</summary>
//        public TestContext TestContext
//        {
//            get
//            {
//                return testContextInstance;
//            }
//            set
//            {
//                testContextInstance = value;
//            }
//        }

//        #region Additional test attributes
//        // 
//        //You can use the following additional attributes as you write your tests:
//        //
//        //Use ClassInitialize to run code before running the first test in the class
//        //[ClassInitialize()]
//        //public static void MyClassInitialize(TestContext testContext)
//        //{
//        //}
//        //
//        //Use ClassCleanup to run code after all tests in a class have run
//        //[ClassCleanup()]
//        //public static void MyClassCleanup()
//        //{
//        //}
//        //
//        //Use TestInitialize to run code before running each test
//        [TestInitialize()]
//        public void MyTestInitialize()
//        {

//            var _Stopwatch1 = new Stopwatch();
//            _Stopwatch1.Start();

//            var _NotificationSettings = new NotificationSettings();
//            _NotificationSettings.StartBrigde = false;
//            _NotificationSettings.StartDispatcher = false;

//            _IGraphFS = new GraphFS2();
//            _IGraphFS2Session = new GraphFSSession(_IGraphFS, "root");

//            myGUID = System.Guid.NewGuid().ToString();
//            _CurrentPath = "/database1";

//            _IGraphFS2Session.MakeFileSystem("file://Graphdbtest-" + myGUID + ".fs", "GraphTestFS1", 40000000, true, null);
//            _IGraphFS2Session.MountFileSystem("file://Graphdbtest-" + myGUID + ".fs", new ObjectLocation(FSPathConstants.PathDelimiter), AccessModeTypes.rw);
//            _IGraphFS2Session.CreateDirectoryObject(new ObjectLocation(_CurrentPath));

//            GraphDB internalDB = new GraphDB(new UUID(), new ObjectLocation(_CurrentPath), _IGraphFS2Session, true);
//            _IGraphDBSession = new GraphDBSession(internalDB, _IGraphFS2Session.SessionToken.SessionInfo.Username);
//            Assert.IsNotNull(_IGraphDBSession);

//            _Stopwatch1.Stop();
//            Debug.WriteLine("Init: " + _Stopwatch1.ElapsedMilliseconds);

//            _IGraphDBSessionObj = _IGraphDBSession;
//            _IGraphFS2SessionObj = _IGraphFS2Session;

//            _MemoryStream = new MemoryStream();

//            _GraphCLI = new GraphCLI(_IGraphDBSession, _IGraphFS2Session, _CurrentPath, _MemoryStream, CLI_Output.Short, typeof(ABasicDBCLICommands), typeof(AAdvancedDBCLICommands));
//        }
        
//        ///Use TestCleanup to run code after each test has run
//        [TestCleanup()]
//        public void MyTestCleanup()
//        {

//            var _Stopwatch2 = new Stopwatch();
//            _Stopwatch2.Start();


//            if (_IGraphDBSession != null)
//                _IGraphDBSession.Shutdown();
//            if (_IGraphFS2Session != null)
//                _IGraphFS2Session.UnmountAllFileSystems();

//            if (File.Exists("Graphdbtest-" + myGUID + ".fs"))

//                // In some cases, if MountFileSystem failes the Storage still holds an
//                // open connection on the file... In this case, we can't delete the
//                // file - fix the MountFileSystem
//                try
//                {
//                    File.Delete("Graphdbtest-" + myGUID + ".fs");
//                }
//                catch { }

//            _Stopwatch2.Stop();
//            Debug.WriteLine("CleanUp: " + _Stopwatch2.ElapsedMilliseconds);
//        }
        
//        #endregion

//        private String executeCmd(String myCommand, CLI_Output myCLI_Output)
//        {
//            _MemoryStream = new MemoryStream();
//            _GraphCLI = new GraphCLI(_IGraphDBSession, _IGraphFS2Session, _CurrentPath, _MemoryStream, myCLI_Output, typeof(ABasicDBCLICommands), typeof(AAdvancedDBCLICommands));

//            _GraphCLI.ReadAndExecuteCommand(myCommand);

//            _MemoryStream.Seek(0, SeekOrigin.Begin);
//            return new StreamReader(_MemoryStream).ReadToEnd();
//        }

//        private String executeCmdBogus(String myCommand, CLI_Output myCLI_Output)
//        {

//            _GraphCLI = new GraphCLI(null, _IGraphFS2Session, _CurrentPath, _MemoryStream, myCLI_Output, typeof(ABasicDBCLICommands), typeof(AAdvancedDBCLICommands));

//            _GraphCLI.ReadAndExecuteCommand(myCommand);

//            _MemoryStream.Seek(0, SeekOrigin.Begin);
//            return new StreamReader(_MemoryStream).ReadToEnd();
//        }

//        #region

//        [TestMethod()]
//        public void CLI_Ctors()
//        {
//            _GraphCLI = new GraphCLI();
//            _GraphCLI.ReadAndExecuteCommand("ll");

//            _GraphCLI = new GraphCLI(typeof(AllCLICommands));
//            _GraphCLI.ReadAndExecuteCommand("ll");

//            _GraphCLI = new GraphCLI(_IGraphFS2Session, _CurrentPath);
//            _GraphCLI.ReadAndExecuteCommand("ll");

//            _GraphCLI = new GraphCLI(_IGraphDBSession, _IGraphFS2Session, _CurrentPath);
//            _GraphCLI.ReadAndExecuteCommand("ll");

//            _GraphCLI = new GraphCLI(_IGraphFS2Session, _CurrentPath, typeof(AllCLICommands));
//            _GraphCLI.ReadAndExecuteCommand("ll");

//            _GraphCLI = new GraphCLI(_IGraphDBSession, _IGraphFS2Session, _CurrentPath, typeof(AllCLICommands));
//            _GraphCLI.ReadAndExecuteCommand("ll");

//            _GraphCLI = new GraphCLI(_IGraphDBSession, _IGraphFS2Session, _CurrentPath, _MemoryStream, CLI_Output.Standard, typeof(AllCLICommands));
//            _GraphCLI.ReadAndExecuteCommand("ll");
//        }

//        #endregion

//        #region DBCLI_DBQUERY

//        /// <summary>
//        /// DBCLI_CREATE
//        /// </summary>
//        [TestMethod()]
//        public void DBCLI_DBQUERY()
//        {

//            #region Preperation

//            _IGraphDBSession.Query("CREATE TYPE User ATTRIBUTES (String Name)");
//            _IGraphDBSession.Query("INSERT INTO User VALUES (Name = 'MeinName')");

//            #endregion

//            String expected = "Name = MeinName";

//            #region Short

//            var ret = executeCmd("dbquery 'FROM User u SELECT *'", CLI_Output.Short);
//            Assert.IsTrue(ret.Contains(expected), "The result does not contains the expected value '{0}' found '{1}'", expected, ret);

//            #endregion

//            #region Standard

//            ret = executeCmd("dbquery 'FROM User u SELECT *'", CLI_Output.Standard);
//            Assert.IsTrue(ret.Contains(expected), "The result does not contains the expected value '{0}' found '{1}'", expected, ret);

//            #endregion

//        }

//        /// <summary>
//        /// DBCLI_CREATE
//        /// </summary>
//        [TestMethod()]
//        public void DBCLI_DBQUERY_Bogus()
//        {
//            String expected = "Name = MeinName";

//            var ret = executeCmdBogus("dbquery 'FROM User u SELECT *'", CLI_Output.Short);
            
//            Assert.IsFalse(ret.Contains(expected), "The result does not contains the expected value '{0}' found '{1}'", expected, ret);

//        }

//        #endregion

//        #region DBCLI_DBTYPEINFO

//        /// <summary>
//        /// DBCLI_DBTYPEINFO
//        /// </summary>
//        [TestMethod()]
//        public void DBCLI_DBTYPEINFO()
//        {

//            #region Preperation

//            _IGraphDBSession.Query("CREATE TYPE User ATTRIBUTES (String Name, LIST<Integer> Numbers, LIST<User> Friends) BACKWARDEDGES (User.Friends MyFriends) ");

//            #endregion

//            String expected = "Name";

//            #region Short

//            var ret = executeCmd("dbtypeinfo 'User'", CLI_Output.Short);
//            Assert.IsTrue(ret.Contains(expected), "The result does not contains the expected value '{0}' found '{1}'", expected, ret);

//            #endregion

//            #region Standard

//            ret = executeCmd("dbtypeinfo 'User'", CLI_Output.Standard);
//            Assert.IsTrue(ret.Contains(expected), "The result does not contains the expected value '{0}' found '{1}'", expected, ret);

//            #endregion

//        }

//        /// <summary>
//        /// DBCLI_DBTYPEINFO
//        /// </summary>
//        [TestMethod()]
//        public void DBCLI_DBTYPEINFO_Bogus()
//        {
//            String expected = "Name   String";

//            var ret = executeCmdBogus("dbtypeinfo 'User'", CLI_Output.Short);

//            Assert.IsFalse(ret.Contains(expected), "The result does not contains the expected value '{0}' found '{1}'", expected, ret);

//        }

//        #endregion

//        #region StringLiteralGraphType


//        [TestMethod()]
//        public void DBCLI_StringLiteralGraphType()
//        {

//            #region Preperation

//            _IGraphDBSession.Query("CREATE TYPE User ATTRIBUTES (String Name, LIST<Integer> Numbers, LIST<User> Friends) BACKWARDEDGES (User.Friends MyFriends) ");

//            #endregion
            
//            string currentType = "Us";

//            var stringLiteralGraphType = new StringLiteralGraphType();
//            var result = stringLiteralGraphType.Complete(ref _IGraphFS2SessionObj, ref _IGraphDBSessionObj, ref _CurrentPath, currentType);

//            Assert.IsTrue(result.Contains("User"), "Did not found type \"User\"");
//        }

//        #endregion

//        #region DBCLI_OM

//        /// <summary>
//        /// DBCLI_OM
//        /// </summary>
//        [TestMethod()]
//        public void DBCLI_OM()
//        {

//            String expected = "";

//            #region Short

//            var ret = executeCmd("om load proton", CLI_Output.Short);
//            Assert.IsTrue(ret.Contains(expected), "The result does not contains the expected value '{0}' found '{1}'", expected, ret);

//            #endregion

//            #region Standard

//            ret = executeCmd("om load proton", CLI_Output.Standard);
//            Assert.IsTrue(ret.Contains(expected), "The result does not contains the expected value '{0}' found '{1}'", expected, ret);

//            #endregion

//            expected = "Currently not implemented";

//            #region Short

//            ret = executeCmd("om store 'proton'", CLI_Output.Short);
//            Assert.IsTrue(ret.Contains(expected), "The result does not contains the expected value '{0}' found '{1}'", expected, ret);

//            #endregion

//            #region Standard

//            ret = executeCmd("om store 'proton'", CLI_Output.Standard);
//            Assert.IsTrue(ret.Contains(expected), "The result does not contains the expected value '{0}' found '{1}'", expected, ret);

//            #endregion
       
//            expected = "Currently not implemented";

//            #region Short

//            ret = executeCmd("om load 'others'", CLI_Output.Short);
//            Assert.IsTrue(ret.Contains(expected), "The result does not contains the expected value '{0}' found '{1}'", expected, ret);

//            #endregion

//            #region Standard

//            ret = executeCmd("om load 'others'", CLI_Output.Standard);
//            Assert.IsTrue(ret.Contains(expected), "The result does not contains the expected value '{0}' found '{1}'", expected, ret);

//            #endregion
        
//        }

//        /// <summary>
//        /// DBCLI_OM
//        /// </summary>
//        [TestMethod()]
//        public void DBCLI_OM_Bogus()
//        {
//            String expected = "No OM database instance started";

//            var ret = executeCmdBogus("om load proton", CLI_Output.Short);

//            Assert.IsTrue(ret.Contains(expected), "The result does not contains the expected value '{0}' found '{1}'", expected, ret);

//        }

//        #endregion

//        #region DBCLI_EXECDBSCRIPT

//        /// <summary>
//        /// DBCLI_EXECDBSCRIPT
//        /// </summary>
//        [TestMethod()]
//        public void DBCLI_EXECDBSCRIPT()
//        {

//            #region Preperation

//            String filename = Guid.NewGuid().ToString();
//            var fileStream = File.CreateText(filename + "1");
//            fileStream.WriteLine("CREATE TYPE User ATTRIBUTES (String Name, LIST<Integer> Numbers, LIST<User> Friends) BACKWARDEDGES (User.Friends MyFriends) ");
//            fileStream.WriteLine("INSERT INTO User VALUES (Name = 'MeinName')");
//            fileStream.Close();

//            fileStream = File.CreateText(filename+"2");
//            fileStream.WriteLine("INSERT INTO User VALUES (Name = 'MeinName2')");
//            fileStream.Close();

//            #endregion

//            String expected = "Successfully executed";

//            #region Short

//            var ret = executeCmd("execdbscript '" + (filename + "1") + "'", CLI_Output.Short);
//            Assert.IsTrue(ret.Contains(expected), "The result does not contains the expected value '{0}' found '{1}'", expected, ret);

//            #endregion

//            #region Standard

//            ret = executeCmd("execdbscript '" + (filename + "2") + "'", CLI_Output.Standard);
//            Assert.IsTrue(ret.Contains(expected), "The result does not contains the expected value '{0}' found '{1}'", expected, ret);

//            #endregion

//        }

//        /// <summary>
//        /// DBCLI_EXECDBSCRIPT
//        /// </summary>
//        [TestMethod()]
//        public void DBCLI_EXECDBSCRIPT_Bogus()
//        {
//            String expected = "Name   String";

//            var ret = executeCmdBogus("execdbscript 'filename'", CLI_Output.Short);

//            Assert.IsFalse(ret.Contains(expected), "The result does not contains the expected value '{0}' found '{1}'", expected, ret);

//        }

//        #endregion

//    }
//}
