public class SerializationResult
{
    public long SizeInBytes { get; set; }
    public TimeSpan SerializationTime { get; set; }
    public TimeSpan DeserializationTime { get; set; }
    public string SerializedData { get; set; }
    public string SerializationFormat { get; set; } // Хранение формата
}