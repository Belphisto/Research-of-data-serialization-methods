using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using ProtoBuf;
using System.IO;
using Avro;
using Avro.Generic;
using Avro.IO;
using Avro.Specific;

class Program
{
    static void Main(string[] args)
    {
        Avro();
    }
    static void SampleTest()
    {
        ISerializableObject obj = new SerializableObject
        {
            Login = "John Doe",
            CountInventory = 3000,
            Coin = 1000000.5f,
            Inventory = new Dictionary<string, int> 
            { 
                { "Item1", 900 },
                { "Item2", 800 },
                { "Item3", 700 },
                { "Item4", 600 },
                { "Item5", 500 },
                { "Item6", 400 },
                { "Item7", 300 },
                { "Item8", 200 },
                { "Item9", 100 },
                { "Item10", 50 }
            },
            PartyLogins = new List<string> 
            { 
                "User1", "User2", "User3", "User4", "User5", 
                "User6", "User7", "User8", "User9", "User10",
                "User11", "User12", "User13", "User14", "User15",
                "User16", "User17", "User18", "User19", "User20"
            }
        };

        // JSON тест
        var jsonTester = new SerializationTester(new JsonSerializationStrategy(), "JSON");
        var jsonResults = new List<SerializationResult>
        {
            jsonTester.TestSerialization(obj)
        };

        // нативная бинарная сериализация тест
        var binTester = new SerializationTester(new BinarySerializationStrategy(), "BIN");
        var binResults = new List<SerializationResult>
        {
            binTester.TestSerialization(obj)
        };

        // xml сериализация тест
        var xmlTester = new SerializationTester(new XmlSerializationStrategy(), "xml");
        var xmlResults = new List<SerializationResult>
        {
            xmlTester.TestSerialization(obj)
        };


        // Сохранение результатов в Excel
        jsonTester.SaveResultsToExcel(jsonResults, "serialization_results.xlsx");
        binTester.SaveResultsToExcel(binResults, "serialization_results.xlsx");
        xmlTester.SaveResultsToExcel(xmlResults, "serialization_results.xlsx");


        Console.WriteLine("Результаты сохранены в файл 'serialization_results.xlsx'");
    }

    public static void Proto()
{
    var obj = new SerializableObjectProto
    {
        Login = "John Doe",
        CountInventory = 3000,
        Coin = 1000000.5f,
        Inventory = new Dictionary<string, int>
        {
            { "Item1", 900 },
            { "Item2", 800 },
            { "Item3", 700 },
            { "Item4", 600 },
            { "Item5", 500 },
            { "Item6", 400 },
            { "Item7", 300 },
            { "Item8", 200 },
            { "Item9", 100 },
            { "Item10", 50 }
        },
        PartyLogins = new List<string>
        {
            "User1", "User2", "User3", "User4", "User5",
            "User6", "User7", "User8", "User9", "User10",
            "User11", "User12", "User13", "User14", "User15",
            "User16", "User17", "User18", "User19", "User20"
        }
    };

    // Measure serialization time
    Stopwatch stopwatch = new Stopwatch();
    stopwatch.Start();

    byte[] serializedData;
    using (var ms = new MemoryStream())
    {
        Serializer.Serialize(ms, obj);
        serializedData = ms.ToArray();
    }

    stopwatch.Stop();
    Console.WriteLine($"Serialization Time: {stopwatch.Elapsed.TotalMilliseconds} ms");

    // Output size of serialized data
    Console.WriteLine($"Serialized Data Size: {serializedData.Length} bytes");

    // Measure deserialization time
    stopwatch.Reset();
    stopwatch.Start();

    SerializableObjectProto deserializedObj;
    using (var ms = new MemoryStream(serializedData))
    {
        deserializedObj = Serializer.Deserialize<SerializableObjectProto>(ms);
    }

    stopwatch.Stop();
    Console.WriteLine($"Deserialization Time: {stopwatch.Elapsed.TotalMilliseconds} ms");}

   public static void Avro()
{
    var obj = new SerializableObject
    {
        Login = "John Doe",
        CountInventory = 3000,
        Coin = 1000000.5f,
        Inventory = new Dictionary<string, int>
        {
            { "Item1", 900 },
            { "Item2", 800 },
            { "Item3", 700 },
            { "Item4", 600 },
            { "Item5", 500 },
            { "Item6", 400 },
            { "Item7", 300 },
            { "Item8", 200 },
            { "Item9", 100 },
            { "Item10", 50 }
        },
        PartyLogins = new List<string>
        {
            "User1", "User2", "User3", "User4", "User5",
            "User6", "User7", "User8", "User9", "User10",
            "User11", "User12", "User13", "User14", "User15",
            "User16", "User17", "User18", "User19", "User20"
        }
    };

    var schema = (RecordSchema)Schema.Parse(@"{
        'type': 'record',
        'name': 'SerializableObject',
        'fields': [
            {'name': 'Login', 'type': 'string'},
            {'name': 'CountInventory', 'type': 'int'},
            {'name': 'Coin', 'type': 'float'},
            {'name': 'Inventory', 'type': {
                'type': 'map',
                'values': 'int'
            }},
            {'name': 'PartyLogins', 'type': {
                'type': 'array',
                'items': 'string'
            }}
        ]
    }");

    var datumWriter = new GenericDatumWriter<GenericRecord>(schema);
    var datumReader = new GenericDatumReader<GenericRecord>(schema, schema);

    using (var memoryStream = new MemoryStream())
    {
        var writer = new BinaryEncoder(memoryStream);
        var record = new GenericRecord(schema);
        record.Add("Login", obj.Login);
        record.Add("CountInventory", obj.CountInventory);
        record.Add("Coin", obj.Coin);
        record.Add("Inventory", obj.Inventory);
        record.Add("PartyLogins", obj.PartyLogins.ToArray()); // Преобразование в массив

        var stopwatch = Stopwatch.StartNew();

        // Serialize
        datumWriter.Write(record, writer);
        stopwatch.Stop();
        var serializationTime = stopwatch.ElapsedMilliseconds;

        var serializedBytes = memoryStream.ToArray();
        var byteSize = serializedBytes.Length;

        Console.WriteLine($"Serialized Data Size: {byteSize} bytes");

        memoryStream.Seek(0, SeekOrigin.Begin);

        // Deserialize
        stopwatch.Restart();
        var reader = new BinaryDecoder(memoryStream);
        var deserializedRecord = datumReader.Read(null, reader);
        stopwatch.Stop();
        var deserializationTime = stopwatch.ElapsedMilliseconds;

        Console.WriteLine($"Byte Size: {byteSize}");
        Console.WriteLine($"Serialization Time: {serializationTime} ms");
        Console.WriteLine($"Deserialization Time: {deserializationTime} ms");
    }
}}
