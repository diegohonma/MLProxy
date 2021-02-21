using System;

namespace MLProxy.Entities
{
    public class BlockByIp
    {
        public string Ip { get; }

        public long Attempts { get; }

        public DateTime CreateDate { get; }

        public BlockByIp(string ip, long attempts, DateTime createDate)
        {
            Ip = ip;
            Attempts = attempts;
            CreateDate = createDate;
        }
    }
}
