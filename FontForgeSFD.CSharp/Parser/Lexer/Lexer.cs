using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace FontForge.Parser;
public unsafe static class Lexer
{
    static Lexer()
    {
        var generalTokensChars = " :\"'()[]{}<>\r\n"u8.ToArray();
        GeneralTokensChars = (byte*)GCHandle.Alloc(generalTokensChars, GCHandleType.Pinned).AddrOfPinnedObject();
        GeneralTokensChars_end = GeneralTokensChars + generalTokensChars.Length;

        var singleToLexKindDictiory = new LexGeneralToken[byte.MaxValue];
        var kinds = Enum.GetValues<LexKind>();
        for (var i = 0; i < generalTokensChars.Length; i++)
            singleToLexKindDictiory[generalTokensChars[i]] = new(kinds[(byte)LexKind.Space + i]);
        SingleToLexKindDictiory = singleToLexKindDictiory;
    }
    
    static readonly byte* GeneralTokensChars, GeneralTokensChars_end;
    static readonly LexGeneralToken[] SingleToLexKindDictiory;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static List<LexToken> ParseTokens(string text)
        => ParseTokens(Encoding.ASCII.GetBytes(text));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static List<LexToken> ParseTokens(byte[] textBytes)
    {
        fixed (byte* pointer = textBytes)
            return ParseTokens(pointer, textBytes.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static List<LexToken> ParseTokens(byte* text, int length)
        => ParseTokens(text, text + length);

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static List<LexToken> ParseTokens(byte* text, byte* text_end)
    {
        var generalTokensChars_end = GeneralTokensChars_end;
        var lexKindDictiory = SingleToLexKindDictiory;

        var result = new List<LexToken>(2 << 16);

        var startTokenBody = text;
        var endTokenBody = text;
        byte single;

        while (text < text_end)
        {
            single = *text;

            var generalTokensChars = GeneralTokensChars;
            while (generalTokensChars < generalTokensChars_end)
            {
                if (*generalTokensChars++ == single)
                {
                    PushIdentifierToken();
                    PushGeneralToken(single);
                    startTokenBody = text;
                    continue;
                }
            }

            endTokenBody++;
            text++;
        }
        PushIdentifierToken();

        return result;

        void PushIdentifierToken()
        {
            var length = (int)(endTokenBody - startTokenBody);
            if (length == 0)
                return;

            var identifier = new string((sbyte*)startTokenBody, 0, length);
            var token = BuildIdentifierToken(identifier);
            result.Add(token);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void PushGeneralToken(byte signle) => result.Add(lexKindDictiory[signle]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        LexIdentifierToken BuildIdentifierToken(string text)
        {
            var pos = 0;
            var paddings = 0;

            while (pos < text.Length && text[pos++] == ' ')
                paddings++;

            if (paddings != 0)
                text = text.Substring(paddings);

            var token = new LexIdentifierToken()
            {
                Text = text,
                Padding = paddings
            };

            if (pos < text.Length)
                if (text[pos] == '0' && pos + 1 < text.Length && text[pos + 1] == 'x')
                {
                    if (long.TryParse(text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out long result))
                    {
                        token.IsNumber = true;
                        token.IsHex = true;
                    }
                }
                else if (char.IsDigit(text[pos]) || (text[pos] == '-' && pos + 1 < text.Length && char.IsDigit(text[pos + 1])))
                {
                    if (decimal.TryParse(text, CultureInfo.InvariantCulture, out decimal result))
                    {
                        var isFloat = result % 1 != 0;

                        token.IsNumber = true;

                        if (isFloat) 
                            token.IsFloat = true;
                        else
                        {
                            if (result is 0 or 1)
                                token.IsBoolean = true;
                            else token.IsInteger = true;
                        }
                    }
                }
            
            return token;            
        }
    }
}