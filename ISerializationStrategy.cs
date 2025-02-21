// Интерфейс для стратегии сериализации
public interface ISerializationStrategy
{
    SerializationResult Serialize(ISerializableObject obj);
    ISerializableObject Deserialize(string data, out SerializationResult result);
}