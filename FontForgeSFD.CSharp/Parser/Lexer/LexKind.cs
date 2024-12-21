namespace FontForge.Parser;
public enum LexKind : byte
{
    Identifier,

    /* General tokens */
    Space,
    Colon,
    Quote,
    Apostrophe,
    LeftRoundBracket,
    RightRoundBracket,
    LeftSquareBracket,
    RightSquareBracket,
    LeftCurlyBracket,
    RightCurlyBracket,
    LeftAngleBracket,
    RightAngleBracket,
    LineFeed,
    CarriageReturn
}