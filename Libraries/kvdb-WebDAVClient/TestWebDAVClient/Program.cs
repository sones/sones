using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Security.Cryptography;
using net.kvdb.webdav;

namespace TestWebDAVClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Debug.Listeners.Add(new ConsoleTraceListener());
            Boolean result;

            // A private mod_dav WebDAV server running on Apache (All tests pass)
            // No authentication required
            WebDAVClient c = new WebDAVClient();
            c.Server = "http://dev.kvdb.net";
            c.BasePath = "/openshare";
            result = RunWebDAVTests(c);

            // A private mod_dav WebDAV server running on Apache (All tests pass)
            // Basic authentication required
            c = new WebDAVClient();
            c.User = "admin";
            c.Pass = "webdavclientadmin";
            c.Server = "http://dev.kvdb.net";
            c.BasePath = "/basicshare";
            result = result && RunWebDAVTests(c);

            // A private mod_dav WebDAV server running on Apache (All tests pass)
            // Digest authentication required
            c = new WebDAVClient();
            c.User = "admin";
            c.Pass = "webdavclientadmin";
            c.Domain = "webdav";
            c.Server = "http://dev.kvdb.net";
            c.BasePath = "/digestshare";
            result = result && RunWebDAVTests(c);
            
            // A public WebDAV server for testing purposes. (All tests pass)
            // http://www.maxum.com/Rumpus/TestRumpus.html
            c = new WebDAVClient();
            c.Server = "http://www.testrumpus.com/";
            c.User = "moe";
            c.Pass = "moe";
            result = result && RunWebDAVTests(c);

            /*
            // A public WebDAV server for testing purposes. (Doesn't work with this client yet)
            // Running lighttpd 1.4.22
            c = new WebDAVClient();
            c.Server = "http://webdav.schlitt.info/";
            result = result && RunWebDAVTests(c);
             */

            if (!result)
            {
                Debug.WriteLine("Tests didn't pass");
            }
            else
            {
                Debug.WriteLine("All passed");
            }
            Console.ReadLine();
        }

        static AutoResetEvent autoResetEvent;

        static Boolean RunWebDAVTests(WebDAVClient c)
        {
            autoResetEvent = new AutoResetEvent(false);

            // Generate unique string to test with.
            string basepath = Path.GetRandomFileName() + '/';
            string tempFilePath = Path.GetTempFileName();
            string uploadTestFilePath = @"c:\windows\notepad.exe";
            //string uploadTestFilePath = @"c:\windows\explorer.exe";
            // string uploadTestFilePath = @"c:\windows\setuplog.txt";
            string uploadTestFileName = Path.GetFileName(uploadTestFilePath);

            c.CreateDirComplete += new CreateDirCompleteDel(c_CreateDirComplete);
            c.CreateDir(basepath);
            autoResetEvent.WaitOne();
            Debug.WriteLine("CreateDir passed");

            c.ListComplete += new ListCompleteDel(c_ListComplete);
            c.List(basepath);
            autoResetEvent.WaitOne();
            if (_files.Count != 0) { return false; }
            Debug.WriteLine("List passed");

            c.UploadComplete += new UploadCompleteDel(c_UploadComplete);
            c.Upload(uploadTestFilePath, basepath + uploadTestFileName);
            autoResetEvent.WaitOne();
            Debug.WriteLine("Upload 1/2 passed");
            c.List(basepath);
            autoResetEvent.WaitOne();
            if (_files.Count != 1) { return false; }
            Debug.WriteLine("Upload 2/2 passed");

            autoResetEvent = new AutoResetEvent(false);
            c.DownloadComplete += new DownloadCompleteDel(c_DownloadComplete);
            c.Download(basepath + uploadTestFileName, tempFilePath);
            autoResetEvent.WaitOne();
            Debug.WriteLine("Download 1/2 passed");
            HashAlgorithm h = HashAlgorithm.Create("SHA1");
            byte[] localhash;
            byte[] remotehash;
            using (FileStream fs = new FileStream(uploadTestFilePath, FileMode.Open))
            {
                localhash = h.ComputeHash(fs);
            }
            using (FileStream fs = new FileStream(tempFilePath, FileMode.Open))
            {
                remotehash = h.ComputeHash(fs);
            }
            for (int i = 0; i < localhash.Length; i++)
            {
                if (localhash[i] != remotehash[i]) { return false; }
            }
            Debug.WriteLine("Download 2/2 passed");

            c.DeleteComplete += new DeleteCompleteDel(c_DeleteComplete);
            c.Delete(basepath + uploadTestFileName);
            autoResetEvent.WaitOne();
            Debug.WriteLine("Delete 1/2 passed");

            c.List(basepath);
            autoResetEvent.WaitOne();
            if (_files.Count != 0) { return false; }
            Debug.WriteLine("Delete 2/2 passed");

            return true;
        }

        static void c_DeleteComplete(int statusCode)
        {
            Debug.Assert(statusCode == 204);
            autoResetEvent.Set();
        }

        static void c_UploadComplete(int statusCode, object state)
        {
            Debug.Assert(statusCode == 201);
            autoResetEvent.Set();
        }

        static void c_CreateDirComplete(int statusCode)
        {
            Debug.Assert(statusCode == 200 || statusCode == 201);
            autoResetEvent.Set();
        }

        static List<string> _files;
        static void c_ListComplete(List<string> files, int statusCode)
        {
            Debug.Assert(statusCode == 207);
            _files = files;
            autoResetEvent.Set();
        }

        static void c_DownloadComplete(int code)
        {
            Debug.Assert(code == 200);
            autoResetEvent.Set();
        }
    }
}
