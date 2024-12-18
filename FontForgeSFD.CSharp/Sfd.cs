using System.Text;

namespace FontForge;
public class Sfd
{
    private protected Sfd(string text) => Deserialize(text);

    void Deserialize(string text)
    {
        using var parseStream = new ParserInStream(text);
        Deserialize(parseStream);
    }

    void Deserialize(ParserInStream stream)
    {

    }

    public string Serialize()
    {

    }

    public static Sfd FromText(string text) => new(text);
}

public class SfdFile : Sfd
{
    SfdFile(string filePath) : base(File.ReadAllText(filePath, Encoding.ASCII)) 
        => FilePath = filePath;

    public readonly string FilePath;

    public void Save() => File.WriteAllText(FilePath, Serialize(), Encoding.ASCII);

    public static SfdFile FromFile(string filePath) => new(filePath);
}