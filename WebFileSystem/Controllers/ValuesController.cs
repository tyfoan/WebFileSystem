using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebFileSystem.Controllers
{
    public class DirToView
    {
        public string FullPath { get; set; }
        public string LastSegment { get; set; }
    }

    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<DirToView> Get()
        {
            var drives = DriveInfo.GetDrives().Select(x => x.Name).ToList();
            var directory = new List<DirToView>();

            foreach (var drive in drives)
            {
                directory.Add(new DirToView() { LastSegment = drive, FullPath = drive });
            }
            return directory;
        }


        // GET api/values/5
        public IEnumerable<DirToView> Get(string path)
        {

            var directories = Directory.GetDirectories(path);
            var list = new List<DirToView>();

            foreach (var item in directories)
            {

                list.Add(new DirToView()
                {
                    FullPath = item,
                    LastSegment = path.Substring(path.LastIndexOf('/') + 1)
                });
            }

            return list;
        } 
    }
}
