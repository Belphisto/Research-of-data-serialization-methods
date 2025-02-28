using static System.Console;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

[Serializable]
public class SerializableObject : ISerializableObject
{
    public string Login { get; set; }
    public int CountInventory { get; set; }
    public float Coin { get; set; }
    [XmlIgnore]
    public Dictionary<string, int> Inventory { get; set; }

    [XmlArray("PartyLogins")]
    [XmlArrayItem("Login")]
    public List<string> PartyLogins { get; set; }

    public SerializableObject()
    {
        Inventory = new Dictionary<string, int>();
        PartyLogins = new List<string>();
    }

    public void Print()
    {
        WriteLine($"Login: {Login}");
        WriteLine($"CountInventory: {CountInventory}");
        WriteLine($"Coin: {Coin}");
        
        WriteLine("Inventory:");
        foreach (var item in Inventory)
        {
            Write($"  {item.Key}: {item.Value}");
        }
        WriteLine();
        WriteLine("PartyLogins:");
        foreach (var login in PartyLogins)
        {
            Write($"  {login}");
        }
        WriteLine();
    }
}

