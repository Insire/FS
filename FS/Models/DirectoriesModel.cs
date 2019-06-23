using System.Collections.Generic;

namespace FS
{
    public class DirectoriesModel
    {
        public List<PatternModel> Includes { get; set; }
        public List<PatternModel> Excludes { get; set; }

        public string TargetDirectory { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
    }
}
