public interface ISerializableObject
{
    string Login { get; set; }
    int CountInventory { get; set; }
    float Coin { get; set; }
    Dictionary<string, int> Inventory { get; set; }
    List<string> PartyLogins { get; set; }
}