namespace IIS_Manager.Models
{
    public class ServerInfo
    {
        public string ServerId { get; set; }
        public string ProcessorName { get; set; }
        public int ProcessorCores { get; set; }
        public int MemorySize { get; set; }

        public ServerInfo(string serverId, string processorName, int processorCores, int memorySize)
        {
            ServerId = serverId;
            ProcessorName = processorName;
            ProcessorCores = processorCores;
            MemorySize = memorySize;
        }

        public ServerInfo()
        {
        }
    }
}
