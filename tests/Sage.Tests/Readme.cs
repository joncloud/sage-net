using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Sage.Tests
{
    public class Readme
    {
        public string InstallationVersion { get; }

        public Readme(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));

            InstallationVersion = "";

            var lines = File.ReadLines(path);
            foreach (var line in lines)
            {
                if (line.StartsWith("dotnet tool"))
                {
                    var match = Regex.Match(line, "dotnet tool install --global sage --version (.+)(-\\*){0,1}");
                    if (match.Success)
                    {
                        InstallationVersion = match.Groups[1].Value;
                    }
                }
            }
        }
    }
}
