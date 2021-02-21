using System;

namespace MLProxy.Entities
{
    public class BlockByIpAndPath
    {
        public string Ip { get; }

        public string Path { get; }

        public long Attempts { get; }

        public DateTime CreateDate { get; }

        public BlockByIpAndPath(string ip, string path, long attempts, DateTime createDate)
        {
            Ip = ip;
            Path = path;
            Attempts = attempts;
            CreateDate = createDate;
        }
    }
}
