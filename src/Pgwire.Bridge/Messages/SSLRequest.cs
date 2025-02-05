namespace Pgwire.Bridge.Messages;

/// <summary />
public class SSLRequest : IFrontend
{
    /// <summary />
    public static bool Is( byte[] buffer )
    {
        return
            buffer[ 3 ] == 0x08
            && buffer[ 4 ] == 0x04
            && buffer[ 5 ] == 0xd2
            && buffer[ 6 ] == 0x16
            && buffer[ 7 ] == 0x2f;
    }
}