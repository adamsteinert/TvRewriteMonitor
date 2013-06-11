using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TvRewriteMonitor
{
    /// <summary>
    /// Takes files in the top level directory and addes them sequentially to the Specials folder.
    /// TODO: If there is an existing episode name, use it and crate the directory as needed.
    /// </summary>
    public class ShowNameOperator
    {
        private const string SpecialsDirectoryName = "Specials";
        private readonly Regex _episodeNameMatch = new Regex(@"[s|S](?<Season>\d\d)[e|E](?<Episode>\d\d)", RegexOptions.Compiled);
        private DirectoryInfo _rootDirectory;
        private string _showName;
        private readonly Action<string> _logger;


        public ShowNameOperator(string path, Action<string> logger)
        {
            _logger = logger;

            if(!Directory.Exists(path))
                throw new DirectoryNotFoundException(path);
            
            Initialize(path);
        }

        private void Initialize(string path)
        {
            _rootDirectory = new DirectoryInfo(path);
            _showName = _rootDirectory.Name;

            if (!_rootDirectory.EnumerateDirectories(SpecialsDirectoryName).Any())
            {
                Directory.CreateDirectory(GetSpecialsDirectoryName(path));
            }
        }

        /// <summary>
        /// Move all files in the root of the directory to the specials folder
        /// </summary>
        public void MoveAllFiles()
        {
            foreach (var file in _rootDirectory.EnumerateFiles())
            {
                MoveFileToNextInSequence(file);
            }
        }

        /// <summary>
        /// Move a file in the root with an arbitrary name to next in the Specials sequence
        /// </summary>
        /// <param name="file"></param>
        public void MoveFileToNextInSequence(FileInfo file)
        {
            // Check for episdoe name and add an initial space if one is found.
            var episodeName = ExtractEpisodeName(file);
            if (!string.IsNullOrEmpty(episodeName))
                episodeName = " " + episodeName;

            string newFileName = string.Format("{0}-{1}{2}{3}", GetNextShowIndicator(), _showName, episodeName, file.Extension);
            var newPath = Path.Combine(GetSpecialsDirectoryName(file.DirectoryName), newFileName);
            File.Move(file.FullName, newPath);

            if(_logger != null)
                _logger(string.Format("Renamed {0} to {1}", file.FullName, newPath.ToString(CultureInfo.InvariantCulture)));
        }

        private string ExtractEpisodeName(FileInfo file)
        {
            // Remove show name, hyphens and extension
            string preservedFileName = file.Name.Replace(_showName, "");
            preservedFileName = preservedFileName.Replace(file.Extension, "");
            preservedFileName = preservedFileName.Replace("-", "");
            
            // If we find an episode number pull it out.
            var epMatch = _episodeNameMatch.Match(file.Name);
            if (epMatch.Success)
            {
                preservedFileName = preservedFileName.Replace(epMatch.Groups[0].Value, "");
            }

            return preservedFileName.Trim();
        }

        private string GetSpecialsDirectoryName(string path)
        {
            return Path.Combine(path, SpecialsDirectoryName);
        }

        /// <summary>
        /// Parse the Specials directory and get the episode/season string in the form sNNeNN
        /// </summary>
        /// <returns></returns>
        private string GetNextShowIndicator()
        {
            var keyDir = Directory.CreateDirectory(GetSpecialsDirectoryName(_rootDirectory.FullName));
            int lastSeason = 0;
            int lastEp = 0;

            foreach (var dir in keyDir.EnumerateFiles())
            {
                var match = _episodeNameMatch.Match(dir.Name);
                if (match.Success)
                {
                    var season = int.Parse(match.Groups[1].Value);
                    var ep = int.Parse(match.Groups[2].Value);

                    if (season > lastSeason)
                    {
                        lastEp = ep;
                        lastSeason = season;
                    }
                    else if (ep > lastEp)
                    {
                        lastEp = ep;
                    }
                }
            }
            return string.Format("s{0:00}e{1:00}", lastSeason, ++lastEp);
        }
    }
}
