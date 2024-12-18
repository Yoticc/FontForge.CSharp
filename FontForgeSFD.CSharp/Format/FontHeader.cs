namespace FontForge.Format;
public class FontHeader : ISerializable
{
    public string SplineFontDB;
    public string FontName;
    public string FullName;
    public string FamilyName;
    public string DefaultBaseFilename;
    public string Weight;
    public string Copyright;
    public string Comments;
    public string UComments;
    public string FontLog;
    public string Version;
    public double ItalicAngle;
    public double UnderlinePosition;
    public double UnderlineWidth;
    public int Ascent;
    public int Descent;
    public int InvalidEm;
    public int SfntRevision;

    [UseCustomSerializer<>]
    public List<Layer> Layers;

    public void Deserialize(ParserInStream parser)
    {
        throw new NotImplementedException();
    }

    public string Serialize()
    {
        throw new NotImplementedException();
    }
}