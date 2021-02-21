namespace MLProxy.Responses
{
    public class GetMetricsResponse
    {
        public string Path { get; }

        public long RequestsCount { get; }

        public GetMetricsResponse(string path, long requestsCount)
        {
            Path = path;
            RequestsCount = requestsCount;
        }
    }
}
