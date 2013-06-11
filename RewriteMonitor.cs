using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvRewriteMonitor
{
 
    /// <summary>
    /// TODO: Watch a directory for changes and fire off ShowNameOperator
    /// </summary>
    public class RewriteMonitor
    {
        public Action<string> Logger;
        private FileSystemWatcher _watcher = null;

        public RewriteMonitor(string path)
        {
            if(!Directory.Exists(path))
                throw new DirectoryNotFoundException(path);

            _watcher = new FileSystemWatcher(path)
                {
                    IncludeSubdirectories = true,
                    Filter = "*.*",
                    NotifyFilter =
                        NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName |
                        NotifyFilters.DirectoryName
                };

            _watcher.Changed += _watcher_Changed;
            _watcher.Created += _watcher_Created;
            _watcher.Deleted += _watcher_Deleted;
        }

        void _watcher_Created(object sender, FileSystemEventArgs e)
        {
            if (Logger != null)
                Logger("Created " + e.Name + " " + e.FullPath + " | " + e.ChangeType.ToString());
        }

        void _watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            if (Logger != null)
                Logger("Deleted " + e.FullPath + " | " + e.ChangeType.ToString());
        }

        void _watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (Logger != null)
                Logger("Changed " + e.FullPath + " | " + e.ChangeType.ToString());
        }

        public void Start()
        {
            _watcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            _watcher.EnableRaisingEvents = false;
        }
    }
}
