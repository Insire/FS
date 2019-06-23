namespace FS.Sync
{
    public sealed class Arguments
    {
        public string Source { get; set; }
        public string Destination { get; set; }
        public string Pattern { get; set; } = "*";
        public bool Nonrecursive { get; set; }
        public bool WhatIf { get; set; }

        public bool CopyLeftOnlyFiles { get; set; } = true;
        public bool UpdateChangedFiles { get; set; } = true;
        public bool DeleteRightOnlyFiles { get; set; } = true;
        public bool CopyEmptyDirectories { get; set; } = true;
        public bool DeleteRightOnlyDirectories { get; set; } = true;
        public bool DeleteSameFiles { get; set; }
        public bool DeleteChangedFiles { get; set; }
    }
}
