using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

public class BinarySerializationStrategy : ISerializationStrategy
{
    public SerializationResult Serialize(ISerializableObject obj)
    {
        var result = new SerializationResult();
        string filePath = $"../Research-of-data-serialization-methods/files/current.bin";

        // Delete the file if it already exists
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        using (var memoryStream = new MemoryStream())
        using (var writer = new BinaryWriter(memoryStream))
        {
            writer.Write(obj.Login);
            writer.Write(obj.CountInventory);
            writer.Write(obj.Coin);

            writer.Write(obj.Inventory.Count);
            foreach (var item in obj.Inventory)
            {
                writer.Write(item.Key);
                writer.Write(item.Value);
            }

            writer.Write(obj.PartyLogins.Count);
            foreach (var login in obj.PartyLogins)
            {
                writer.Write(login);
            }

            var binaryData = memoryStream.ToArray();
            result.SizeInBytes = binaryData.Length;
            result.SerializedData = Convert.ToBase64String(binaryData);

            File.WriteAllBytes(filePath, binaryData);
            File.SetCreationTime(filePath, DateTime.Now);
        }

        return result;
    }

    public ISerializableObject Deserialize(string data, out SerializationResult result)
    {
        result = new SerializationResult();
        string filePath = $"../Research-of-data-serialization-methods/files/current.bin";
        var binaryData = File.ReadAllBytes(filePath);
        result.SizeInBytes = binaryData.Length;
        result.SerializedData = Convert.ToBase64String(binaryData);

        using (var memoryStream = new MemoryStream(binaryData))
        using (var reader = new BinaryReader(memoryStream))
        {
            var obj = new SerializableObject
            {
                Login = reader.ReadString(),
                CountInventory = reader.ReadInt32(),
                Coin = reader.ReadSingle()
            };

            int inventoryCount = reader.ReadInt32();
            for (int i = 0; i < inventoryCount; i++)
            {
                string key = reader.ReadString();
                int value = reader.ReadInt32();
                obj.Inventory.Add(key, value);
            }

            int partyLoginsCount = reader.ReadInt32();
            for (int i = 0; i < partyLoginsCount; i++)
            {
                obj.PartyLogins.Add(reader.ReadString());
            }

            obj.Print();
            return obj;
        }
    }
}
