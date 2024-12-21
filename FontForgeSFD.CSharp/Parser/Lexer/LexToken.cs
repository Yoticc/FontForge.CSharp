namespace FontForge.Parser;
public abstract class LexToken
{
    public LexToken(LexKind kind) => Kind = kind;

    public readonly LexKind Kind;

    public override string ToString() => Kind.ToString();
}

public class LexIdentifierToken() : LexToken(LexKind.Identifier)
{
    public string Text;
    public int Padding;
    public bool IsNumber;
    public bool IsFloat;
    public bool IsInteger;
    public bool IsHex;
    public bool IsBoolean;

    public override string ToString() => $"{base.ToString()} ({Text})";
}

public class LexGeneralToken(LexKind kind) : LexToken(kind);