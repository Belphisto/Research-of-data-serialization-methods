using System;
using System.Collections.Generic;
using System.Diagnostics;
using ProtoBuf;
using System.IO;

[ProtoContract]
public class SerializableObjectProto
{
    [ProtoMember(1)]
    public string Login { get; set; }

    [ProtoMember(2)]
    public int CountInventory { get; set; }

    [ProtoMember(3)]
    public float Coin { get; set; }

    [ProtoMember(4, Name = "Inventory")]
    [ProtoMap]
    public Dictionary<string, int> Inventory { get; set; }

    [ProtoMember(5)]
    public List<string> PartyLogins { get; set; }

    public SerializableObjectProto()
    {
        Inventory = new Dictionary<string, int>();
        PartyLogins = new List<string>();
    }
}

