namespace FontForge.Parser;
public abstract class LexToken
{
    public LexToken(LexKind kind) => Kind = kind;

    public readonly LexKind Kind;

    public override string ToString() => Kind.ToString();
}

public unsafe class LexIdentifierToken() : LexToken(LexKind.Identifier)
{
    public byte* Text;
    public int TextLength;
    public int Padding;
    public bool IsNumber;
    public bool IsFloat;
    public bool IsInteger;
    public bool IsHex;
    public bool IsBoolean;

    public override string ToString() => $"{base.ToString()} ({new string((sbyte*)Text, 0, TextLength)})";
}

public class LexGeneralToken(LexKind kind) : LexToken(kind);