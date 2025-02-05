namespace Pgwire.Bridge.Messages;

/// <summary />
public class AuthenticationOk : IBackend
{
    /// <summary />
    static readonly byte[] Payload = new byte[] {
        (byte) 'R',
        0x00, 0x00, 0x00, 0x08,
        0x00, 0x00, 0x00, 0x00 };


    /// <summary />
    public static void Write( Stream stream )
    {
        stream.Write( Payload );
    }
}