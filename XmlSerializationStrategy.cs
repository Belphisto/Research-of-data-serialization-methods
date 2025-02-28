using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

public class XmlSerializationStrategy : ISerializationStrategy
{
    public SerializationResult Serialize(ISerializableObject obj)
    {
        var result = new SerializationResult();
        string filePath = $"../Research-of-data-serialization-methods/files/current.xml";

        // Delete the file if it already exists
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        using (var stringWriter = new StringWriter())
        {
            var serializer = new XmlSerializer(typeof(SerializableObject));
            serializer.Serialize(stringWriter, obj);
            var xmlData = stringWriter.ToString();

            // Handle Inventory serialization separately
            var inventoryFilePath = filePath.Replace(".xml", "_inventory.xml");
            SerializeDictionary(((SerializableObject)obj).Inventory, inventoryFilePath);

            result.SizeInBytes = Encoding.UTF8.GetByteCount(xmlData);
            result.SerializedData = xmlData;

            File.WriteAllText(filePath, xmlData);
            File.SetCreationTime(filePath, DateTime.Now);
        }

        return result;
    }

    public ISerializableObject Deserialize(string data, out SerializationResult result)
    {
        result = new SerializationResult();
        string filePath = $"../Research-of-data-serialization-methods/files/current.xml";
        var xmlData = File.ReadAllText(filePath);

        result.SizeInBytes = Encoding.UTF8.GetByteCount(xmlData);
        result.SerializedData = xmlData;

        SerializableObject obj;

        using (var stringReader = new StringReader(xmlData))
        {
            var serializer = new XmlSerializer(typeof(SerializableObject));
            obj = (SerializableObject)serializer.Deserialize(stringReader);

            // Handle Inventory deserialization separately
            var inventoryFilePath = filePath.Replace(".xml", "_inventory.xml");
            DeserializeDictionary(obj.Inventory, inventoryFilePath);
        }

        obj.Print();
        return obj;
    }

    private void SerializeDictionary(Dictionary<string, int> dictionary, string filePath)
    {
        using (var writer = new StreamWriter(filePath))
        {
            foreach (var kvp in dictionary)
            {
                writer.WriteLine($"{kvp.Key},{kvp.Value}");
            }
        }
    }

    private void DeserializeDictionary(Dictionary<string, int> dictionary, string filePath)
    {
        dictionary.Clear();
        using (var reader = new StreamReader(filePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var parts = line.Split(',');
                if (parts.Length == 2)
                {
                    dictionary[parts[0]] = int.Parse(parts[1]);
                }
            }
        }
    }
}
