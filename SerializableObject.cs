public class SerializableObject : ISerializableObject
{
    public string Login { get; set; }
    public int CountInventory { get; set; }
    public float Coin { get; set; }
    public Dictionary<string, int> Inventory { get; set; }
    public List<string> PartyLogins { get; set; }

    public SerializableObject()
    {
        Inventory = new Dictionary<string, int>();
        PartyLogins = new List<string>();
    }
}

