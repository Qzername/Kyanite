namespace WhiteboardServer.Models
{
    public struct DataItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string[] Value { get; set; }   
    }
}
