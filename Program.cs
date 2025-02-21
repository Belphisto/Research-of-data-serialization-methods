using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
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

        // Сохранение результатов в Excel
        jsonTester.SaveResultsToExcel(jsonResults, "serialization_results.xlsx");

        Console.WriteLine("Результаты сохранены в файл 'serialization_results.xlsx'");
    }
}
