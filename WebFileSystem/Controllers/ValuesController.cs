using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Web.Http;

namespace WebFileSystem.Controllers
{
    public class DirToView
    {
        public string FullPath { get; set; }
        public string FileName { get; set; }
    }



    public class ValuesController : ApiController
    {
        protected static class CountOfFiles
        {
            public static long less10Mb;
            public static long less50Mb;
            public static long more100Mb;
        }

        public static string ExtractFilename(string path)
        {
            // If path ends with a "\", it's a path only so return String.Empty.
            if (path.Trim().EndsWith(@"\"))
                return path;

            //if (Regex.Matches(path, @"\").Count <= 2)
            //    return path;

            // Determine where last backslash is.
            int position = path.LastIndexOf('\\');
            // If there is no backslash, assume that this is a filename.
            if (position == -1)
            {
                // Determine whether file exists in the current directory.
                if (File.Exists(Environment.CurrentDirectory + Path.DirectorySeparatorChar + path))
                    return path;
                else
                    return String.Empty;
            }
            else
            {
                // Return filename without file path.
                return path.Substring(position + 1);
            }
        }


        public static bool CanRead(string path)
        {
            var readAllow = false;
            var readDeny = false;
            DirectorySecurity accessControlList;
            try
            {
                accessControlList = Directory.GetAccessControl(path);
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
            if (accessControlList == null)
                return false;
            var accessRules = accessControlList.GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));
            if (accessRules == null)
                return false;

            foreach (FileSystemAccessRule rule in accessRules)
            {
                if ((FileSystemRights.Read & rule.FileSystemRights) != FileSystemRights.Read) continue;

                if (rule.AccessControlType == AccessControlType.Allow)
                    readAllow = true;
                else if (rule.AccessControlType == AccessControlType.Deny)
                    readDeny = true;
            }

            return readAllow && !readDeny;
        }

        public static long ConvertBytesToMegabytes(long bytes)
        {
            return (long)((bytes / 1024f) / 1024f);
        }

        //public static string CountFiles(string path)
        //{
        //    foreach (var dir in Directory.GetDirectories(path))
        //    {
        //        FileAttributes attr = File.GetAttributes(path);

        //        if (attr.HasFlag(FileAttributes.Directory))
        //        {
        //            return CountFiles(dir);
        //        }
        //        else
        //        {
        //            FileInfo info;

        //            foreach (var file in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories))
        //            {
        //                info = new FileInfo(file);
        //                info.le
        //            }
        //        }
        //    }
        //    return "0";
        //}



        public static long GetDirectorySize(string path)
        {
            long totalFileSize = 0;
            string[] files;

            try
            {
                files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            }
            catch (UnauthorizedAccessException)
            {
                return 0;
            }


            foreach (string file in files)
            {
                // FileInfo to get length of each file.
                FileInfo info = new FileInfo(file);
                totalFileSize = totalFileSize + info.Length;
            }
            return ConvertBytesToMegabytes(totalFileSize);
        }


        // GET api/values/5
        public object Get(string path)
        {
            var dirs = new List<DirToView>();

            List<string> directories;
            List<string> files;

            long less10Mb = 0, less50Mb = 0, more100Mb = 0;


            if (string.IsNullOrWhiteSpace(path))
            {
                directories = DriveInfo.GetDrives().Select(x => x.Name).ToList();
                files = null;
            }
            else
            {
                directories = Directory.GetDirectories(path).Where(f => (new FileInfo(f).Attributes & FileAttributes.Hidden & FileAttributes.System) == 0).ToList();
                files = Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly).Where(f => (new FileInfo(f).Attributes & FileAttributes.Hidden & FileAttributes.System) == 0).ToList();
                directories.AddRange(files);
            }

            foreach (var directory in directories)
            {
                if (!CanRead(directory))
                    continue;

                
                try
                {
                    //FileAttributes attr;
                    //attr = File.GetAttributes(directory);
                    //if (!attr.HasFlag(FileAttributes.Directory))
                    //{
                    var items = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories).Where(f => (new FileInfo(f).Attributes & FileAttributes.Hidden & FileAttributes.System) == 0);
                    //}
                    FileInfo info;
                    foreach (var file in items)
                    {
                        info = new FileInfo(file);
                        if (ConvertBytesToMegabytes(info.Length) < 10)
                        {
                            less10Mb += 1;
                            continue;
                        }


                        if (ConvertBytesToMegabytes(info.Length) >= 10 && ConvertBytesToMegabytes(info.Length) <= 50)
                        {
                            less50Mb += 1;
                            continue;
                        }

                        if (ConvertBytesToMegabytes(info.Length) >= 100)
                            more100Mb += 1;
                    }
                }
                catch (Exception)
                {

                }

               


                dirs.Add(new DirToView()
                {
                    FullPath = directory,
                    FileName = ExtractFilename(directory)
                });
            }
            return new { dirs, less10Mb, less50Mb, more100Mb };
        }
    }
}
