namespace FontForge;
public interface ISerializable
{
    void Deserialize(ParserInStream stream);
    void Serialize(ParserOutStream stream);
}