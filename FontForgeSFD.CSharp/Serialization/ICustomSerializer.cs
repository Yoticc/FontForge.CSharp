namespace FontForge;
public interface ICustomSerializer<T>
{
    T Deserialize(ParserInStream stream);
    void Serialize(ParserOutStream stream, T value);
}