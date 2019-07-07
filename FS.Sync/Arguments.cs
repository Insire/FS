namespace FS.Sync
{
    public sealed class Arguments
    {
        public bool CopyLeftOnlyFiles { get; set; } = true;
        public bool UpdateChangedFiles { get; set; } = true;
        public bool DeleteRightOnlyFiles { get; set; } = true;
        public bool DeleteSameFiles { get; set; }
        public bool DeleteChangedFiles { get; set; }
        public bool RespectLastAccessDateTime { get; set; }
    }
}
