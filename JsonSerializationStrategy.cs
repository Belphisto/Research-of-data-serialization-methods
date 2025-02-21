using Newtonsoft.Json; //dotnet add package Newtonsoft.Json
using System.Text;

public class JsonSerializationStrategy : ISerializationStrategy
{
    public SerializationResult Serialize(ISerializableObject obj)
    {
        var result = new SerializationResult();
        var jsonData = JsonConvert.SerializeObject(obj);
        result.SizeInBytes = Encoding.UTF8.GetByteCount(jsonData);
        result.SerializedData = jsonData;
        return result;
    }

    public ISerializableObject Deserialize(string data, out SerializationResult result)
    {
        result = new SerializationResult();
        var obj = JsonConvert.DeserializeObject<SerializableObject>(data);
        result.SizeInBytes = Encoding.UTF8.GetByteCount(data);
        result.SerializedData = data;
        return obj;
    }
}
