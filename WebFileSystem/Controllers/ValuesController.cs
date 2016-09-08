using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
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

        public static string ExtractFilename(string filepath)
        {
            // If path ends with a "\", it's a path only so return String.Empty.
            if (filepath.Trim().EndsWith(@"\"))
                return filepath;

            //if (Regex.Matches(filepath, @"\").Count <= 2)
            //    return filepath;

            // Determine where last backslash is.
            int position = filepath.LastIndexOf('\\');
            // If there is no backslash, assume that this is a filename.
            if (position == -1)
            {
                // Determine whether file exists in the current directory.
                if (File.Exists(Environment.CurrentDirectory + Path.DirectorySeparatorChar + filepath))
                    return filepath;
                else
                    return String.Empty;
            }
            else
            {
                // Return filename without file path.
                return filepath.Substring(position + 1);
            }
        }


        // GET api/values/5
        public object Get(string path)
        {
            var dirs = new List<DirToView>();

            IList<string> directories;
            IList<string> files;

            if (string.IsNullOrWhiteSpace(path))
            {
                directories = DriveInfo.GetDrives().Select(x => x.Name).ToList();
                files = null;
            }
            else
            {
                directories = Directory.GetDirectories(path).Where(x=> !f.;
                files = Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly).ToList();
            }

            foreach (var directory in directories)
            {
                dirs.Add(new DirToView()
                {
                    FullPath = directory,
                    FileName = ExtractFilename(directory)
                });
            }
            //http://stackoverflow.com/questions/1288975/how-to-test-if-directory-is-hidden-in-c
            //http://stackoverflow.com/questions/4133544/hidden-folders-c-sharp
            return new { dirs, files };
        }
    }
}
