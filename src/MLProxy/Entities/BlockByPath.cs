using System;

namespace MLProxy.Entities
{
    public class BlockByPath
    {
        public string Path { get; }

        public long Attempts { get; }

        public DateTime CreateDate { get; }

        public BlockByPath(string path, long attempts, DateTime createDate)
        {
            Path = path;
            Attempts = attempts;
            CreateDate = createDate;
        }
    }
}
