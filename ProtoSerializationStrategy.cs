using Google.Protobuf;
using System.IO;

public class ProtoSerializationStrategy : ISerializationStrategy
{
    public SerializationResult Serialize(ISerializableObject obj)
    {
        var result = new SerializationResult();

        // Convert SerializableObject to the generated Proto SerializableObject
        var protoObj = new SerializableObjectProto
        {
            Login = obj.Login,
            CountInventory = obj.CountInventory,
            Coin = obj.Coin,
        };

        foreach (var item in obj.Inventory)
        {
            protoObj.Inventory.Add(new InventoryItemProto
            {
                Key = item.Key,
                Value = item.Value,
            });
        }

        protoObj.PartyLogins.AddRange(obj.PartyLogins);

        using (var memoryStream = new MemoryStream())
        {
            protoObj.WriteTo(memoryStream);
            var data = memoryStream.ToArray();
            result.SizeInBytes = data.Length;
            result.SerializedData = Convert.ToBase64String(data);

            File.WriteAllBytes("../Research-of-data-serialization-methods/files/current.bin", data);
            File.SetCreationTime("../Research-of-data-serialization-methods/files/current.bin", DateTime.Now);
        }

        return result;
    }

    public ISerializableObject Deserialize(string data, out SerializationResult result)
    {
        result = new SerializationResult();
        var bytes = File.ReadAllBytes("../Research-of-data-serialization-methods/files/current.bin");

        result.SizeInBytes = bytes.Length;
        result.SerializedData = Convert.ToBase64String(bytes);

        SerializableObjectProto protoObj;
        using (var memoryStream = new MemoryStream(bytes))
        {
            protoObj = SerializableObjectProto.Parser.ParseFrom(memoryStream);
        }

        var obj = new SerializableObject
        {
            Login = protoObj.Login,
            CountInventory = protoObj.CountInventory,
            Coin = protoObj.Coin,
            PartyLogins = protoObj.PartyLogins.ToList(),
            Inventory = protoObj.Inventory.ToDictionary(i => i.Key, i => i.Value)
        };

        obj.Print();
        return obj;
    }
}
