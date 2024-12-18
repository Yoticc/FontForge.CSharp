namespace FontForge.Format;
public class Layer : ISerializable
{
    public int LayerNumber;
    public bool QuadraticFlag;
    public string Name;
    public bool zackgroundFlag;

    public void Deserialize(ParserInStream stream)
    {

    }

    public void Serialize(ParserOutStream stream)
    {
        throw new NotImplementedException();
    }

    public class ListLayerSerializer : ICustomSerializer<List<Layer>>
    {
        public List<Layer> Deserialize(ParserInStream stream)
        {
            var count = stream.ReadFieldIntValue();
        }

        public string Serialize(List<Layer> value)
        {
            throw new NotImplementedException();
        }
    }
}