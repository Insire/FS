using System;
using System.Collections.Generic;
using System.IO;

namespace FS
{
    public class DirectoriesModel
    {
        public List<PatternModel> Includes { get; set; }
        public List<PatternModel> Excludes { get; set; }

        public string TargetDirectory { get; set; } = new DirectoryInfo(".").FullName;
        public string Name { get; set; } = string.Empty;
        public string Root { get; set; } = new DirectoryInfo(".").FullName;
        public int Id { get; set; }

        public bool CopyLeftOnlyFiles { get; set; } = true;
        public bool UpdateChangedFiles { get; set; } = true;
        public bool DeleteRightOnlyFiles { get; set; } = true;
        public bool CopyEmptyDirectories { get; set; } = true;
        public bool DeleteRightOnlyDirectories { get; set; } = true;
        public bool DeleteSameFiles { get; set; }
        public bool DeleteChangedFiles { get; set; }
        public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(30);
    }
}
