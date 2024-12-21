using FontForge.Parser;
using System.Text;

namespace FontForge;
public class Sfd
{
    private protected Sfd(string text) => Deserialize(text);

    void Deserialize(string text)
    {
        var lexTokens = Lexer.ParseTokens(text);

        _ = 3;
    }

    public string Serialize()
    {
        return null;
    }

    public static Sfd ParseText(string text) => new(text);
}

public class SfdFile : Sfd
{
    SfdFile(string filePath) : base(File.ReadAllText(filePath, Encoding.ASCII)) 
        => FilePath = filePath;

    public readonly string FilePath;

    public void Save() => File.WriteAllText(FilePath, Serialize(), Encoding.ASCII);

    public static SfdFile ParseFile(string filePath) => new(filePath);
}