namespace Pgwire.Bridge.Messages;

/// <summary />
public class Query : IFrontend
{
    /// <summary />
    public static bool Is( byte[] buffer )
    {
        return buffer[ 0 ] == (byte) 'Q';
    }


    /// <summary />
    public static string Parse( BufferReader reader )
    {
        reader.ReadChar();
        var length = reader.ReadInt32();
        
        return reader.ReadString( length );
    }
}