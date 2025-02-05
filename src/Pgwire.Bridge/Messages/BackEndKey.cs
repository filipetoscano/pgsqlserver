namespace Pgwire.Bridge.Messages;

/// <summary />
public class BackEndKey : IBackend
{
    /// <summary />
    public static void Write( Stream stream, int processId, int secretKey )
    {
        stream.WriteChar( 'K' );
        stream.WriteInt32( 12 );

        stream.WriteInt32( processId );
        stream.WriteInt32( secretKey );
    }
}