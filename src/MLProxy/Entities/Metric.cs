using System;

namespace MLProxy.Entities
{
    public class Metric
    {
        public string Ip { get; }

        public string Path { get; }

        public DateTime RequestDate { get; }

        public Metric(string ip, string path, DateTime requestDate)
        {
            Ip = ip;
            Path = path;
            RequestDate = requestDate;
        }
    }
}
