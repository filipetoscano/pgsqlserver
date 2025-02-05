namespace Pgwire.Bridge.Messages;

/// <summary />
public class ReadyForQuery : IBackend
{
    /// <summary />
    static readonly byte[] Payload = new byte[] {
        (byte) 'Z',
        0x00, 0x00, 0x00, 0x05,
        (byte) 'I' };


    /// <summary />
    public static void Write( Stream stream )
    {
        stream.Write( Payload );
    }
}