using System.Runtime.InteropServices;
using System.Text;

namespace FontForge;
public unsafe class ParserInStream : IDisposable
{
    const byte BackslashR = (byte)'\r';
    const byte BackslashN = (byte)'\n';
    const byte Colon = (byte)':';

    public ParserInStream(string text)
    {
        var dataBytes = Encoding.ASCII.GetBytes(text);
        dataHandle = GCHandle.Alloc(dataBytes, GCHandleType.Pinned);
        data = (byte*)dataHandle.AddrOfPinnedObject();
        length = text.Length;
    }

    GCHandle dataHandle;
    byte* data;
    int currentPosition;
    int length;
    bool isEnded;

    int CurrentLineLength
    {
        get
        {
            var length = 0;
            while (data[length] is not BackslashR and not BackslashN)
                length++;
            return length;
        }
    }

    public bool HasNextLine => !isEnded;

    string SubString(int length) => new string((sbyte*)data, currentPosition, length);

    public string ReadLine()
    {
        var result = SubString(CurrentLineLength);
        
        NextLine();

        return result;
    }

    public bool ReadBoolField() => ReadIntField() != 0;

    public int ReadIntField() => int.Parse(ReadField().value);

    public string ReadFieldName()
    {
        var index = 0;
        while (data[currentPosition + index] != Colon)
            index++;

        var name = SubString(index);

        currentPosition += 2;

        return name;
    }

    public (string name, string value) ReadField()
    {
        var line = ReadLine();
        var splitIndex = line.IndexOf(':');

        var name = line.Substring(0, splitIndex);
        var value = line.Substring(splitIndex + 1);

        return (name, value);
    }

    void NextLine()
    {
        var lineLength = CurrentLineLength;
        if (currentPosition + lineLength >= length)
        {
            isEnded = true;
            currentPosition = length;
            return;
        }
        lineLength++; // \r

        if (data[currentPosition + lineLength + 1] == BackslashN)
            lineLength++; // \n

        currentPosition += lineLength;
    }

    public void BackLine()
    {
        currentPosition--; // \r or \n
        if (data[currentPosition] == BackslashR)
            currentPosition--;
        currentPosition--; // \n

        while (data[currentPosition] is not BackslashR and not BackslashN)
            currentPosition--;
    }

    public void Dispose() => dataHandle.Free();
}

public unsafe class ParserOutStream
{
    public ParserOutStream()
    {
        Builder = new StringBuilder(2 << 16);
    }

    public StringBuilder Builder;

    public void WriteLine(string line)
    {
        Builder.Append(line);
        Builder.Append('\n');
    }

    public void WriteField(string fieldName, params object[] parts)
    {

    }

    public override string ToString() => Builder.ToString();
}