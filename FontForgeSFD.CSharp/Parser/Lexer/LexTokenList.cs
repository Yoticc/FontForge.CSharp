using System.Runtime.InteropServices;

namespace FontForge.Parser;
public unsafe class LexTokenList
{
    public byte[] ContentBytes;
    public GCHandle ContentGCHandle;
    public byte* ContentPointer;
    public List<LexToken> Tokens;
}