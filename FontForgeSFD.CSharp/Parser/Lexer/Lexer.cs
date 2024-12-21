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
    public static LexTokenList ParseTokens(string text)
        => ParseTokens(Encoding.ASCII.GetBytes(text));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LexTokenList ParseTokens(byte[] textBytes)
    {
        var contentBytes = textBytes;
        var contentGCHandle = GCHandle.Alloc(contentBytes, GCHandleType.Pinned);
        var contentPointer = (byte*)contentGCHandle.AddrOfPinnedObject();
        var tokens = ParseTokens(contentPointer, contentPointer + contentBytes.Length);

        var tokenList = new LexTokenList()
        {
            ContentBytes = contentBytes,
            ContentGCHandle = contentGCHandle,
            ContentPointer = contentPointer,
            Tokens = tokens
        };

        return tokenList;
    }

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

            var token = BuildIdentifierToken(startTokenBody, length);
            result.Add(token);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void PushGeneralToken(byte signle) => result.Add(lexKindDictiory[signle]);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        LexIdentifierToken BuildIdentifierToken(byte* text, int length)
        {
            const byte CHAR_ZERO = 0x30;
            const byte CHAR_X = 0x78;
            const byte CHAR_DASH = 0x2D;

            var pos = 0;
            var paddings = 0;

            while (length > 0 && *text == ' ')
            {
                paddings++;
                text++;
                length--;
            }

            var token = new LexIdentifierToken()
            {
                Text = text,
                TextLength = length,
                Padding = paddings
            };

            var textSpan = new ReadOnlySpan<byte>(text, length);

            if (pos < length)
                if (text[pos] == CHAR_ZERO && pos + 1 < length && text[pos + 1] == CHAR_X)
                {
                    if (long.TryParse(textSpan, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out long result))
                    {
                        token.IsNumber = true;
                        token.IsHex = true;
                    }
                }
                else if (text[pos] - CHAR_ZERO is <= 9 and >= 0)
                {
                    if (decimal.TryParse(textSpan, CultureInfo.InvariantCulture, out decimal result))
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