//dotnet add package Apache.Avro
//dotnet add package Apache.Avro.File

using System;
using System.Collections.Generic;
using System.IO;
using Avro;
using Avro.File;
using Avro.Generic;

public class AvroSerializationStrategy : ISerializationStrategy
{
    private readonly Schema _schema;
    private readonly RecordSchema _inventoryItemSchema;

    private readonly string _schemaJson = @"
    {
        ""type"": ""record"",
        ""name"": ""SerializableObject"",
        ""fields"": [
            { ""name"": ""Login"", ""type"": ""string"" },
            { ""name"": ""CountInventory"", ""type"": ""int"" },
            { ""name"": ""Coin"", ""type"": ""float"" },
            { ""name"": ""Inventory"", ""type"": { ""type"": ""array"", ""items"": {
                ""type"": ""record"",
                ""name"": ""InventoryItem"",
                ""fields"": [
                    { ""name"": ""Key"", ""type"": ""string"" },
                    { ""name"": ""Value"", ""type"": ""int"" }
                ]
            }}},
            { ""name"": ""PartyLogins"", ""type"": { ""type"": ""array"", ""items"": ""string"" } }
        ]
    }";

    public AvroSerializationStrategy()
    {
        _schema = Schema.Parse(_schemaJson);
        var recordSchema = (RecordSchema)_schema;
        var inventoryField = recordSchema.Fields.Find(f => f.Name == "Inventory");
        var inventoryArraySchema = (ArraySchema)inventoryField.Schema;
        _inventoryItemSchema = (RecordSchema)inventoryArraySchema.ItemSchema;
    }

    public SerializationResult Serialize(ISerializableObject obj)
    {
        var result = new SerializationResult();
        var filePath = $"../Research-of-data-serialization-methods/files/current.avro";

        var record = new GenericRecord((RecordSchema)_schema);
        record.Add("Login", obj.Login);
        record.Add("CountInventory", obj.CountInventory);
        record.Add("Coin", obj.Coin);

        var inventoryItems = new List<GenericRecord>();
        foreach (var item in obj.Inventory)
        {
            var inventoryRecord = new GenericRecord(_inventoryItemSchema);
            inventoryRecord.Add("Key", item.Key);
            inventoryRecord.Add("Value", item.Value);
            inventoryItems.Add(inventoryRecord);
        }

        var arrayItems = new List<GenericRecord>(inventoryItems);
        record.Add("Inventory", arrayItems);

        record.Add("PartyLogins", new List<string>(obj.PartyLogins));

        using (var fileStream = File.OpenWrite(filePath))
        using (var writer = DataFileWriter<GenericRecord>.OpenWriter(new GenericDatumWriter<GenericRecord>(_schema), fileStream))
        {
            writer.Append(record);
        }

        result.SizeInBytes = new FileInfo(filePath).Length;
        result.SerializedData = Convert.ToBase64String(File.ReadAllBytes(filePath));

        return result;
    }

    public ISerializableObject Deserialize(string data, out SerializationResult result)
    {
        result = new SerializationResult();
        var filePath = $"../Research-of-data-serialization-methods/files/current.avro";
        var bytes = Convert.FromBase64String(data);
        File.WriteAllBytes(filePath, bytes);

        SerializableObject obj;

        using (var fileStream = File.OpenRead(filePath))
        using (var reader = DataFileReader<GenericRecord>.OpenReader(fileStream))
        {
            var record = reader.Next();
            obj = new SerializableObject
            {
                Login = (string)record["Login"],
                CountInventory = (int)record["CountInventory"],
                Coin = (float)record["Coin"],
                PartyLogins = new List<string>((IEnumerable<string>)record["PartyLogins"]),
                Inventory = new Dictionary<string, int>()
            };

            foreach (var item in (IEnumerable<GenericRecord>)record["Inventory"])
            {
                obj.Inventory[(string)item["Key"]] = (int)item["Value"];
            }
        }

        result.SizeInBytes = bytes.Length;
        result.SerializedData = data;

        obj.Print();
        return obj;
    }
}
