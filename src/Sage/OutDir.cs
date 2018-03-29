using System;
using System.Collections.Generic;
using System.IO;

namespace Sage
{
    class OutDir : IDisposable
    {
        readonly string _target;
        readonly List<string> _files = new List<string>();
        readonly bool _cleanupFiles;
        public OutDir()
        {
            _target = Path.GetTempPath();
            _cleanupFiles = true;
        }

        public OutDir(string path) => _target = path;

        public string AddFile(string name)
        {
            var path = Path.Combine(_target, name);
            if (File.Exists(path)) File.Delete(path);
            _files.Add(path);
            return path;
        }

        public void Dispose()
        {
            if (!_cleanupFiles) return;

            foreach (var file in _files)
            {
                if (File.Exists(file)) File.Delete(file);
            }
        }
    }
}
