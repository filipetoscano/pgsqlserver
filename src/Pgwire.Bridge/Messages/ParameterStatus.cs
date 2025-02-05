namespace Pgwire.Bridge.Messages;

/// <summary />
/// <see href="https://www.postgresql.org/docs/current/protocol-message-formats.html#PROTOCOL-MESSAGE-FORMATS-PARAMETERSTATUS" />
public class ParameterStatus : IBackend
{
    /// <summary />
    public static void Write( Stream stream, string key, string value )
    {
        var l = 4 + key.Length + 1 + value.Length + 1;

        stream.WriteChar( 'S' );
        stream.WriteInt32( l );

        stream.WriteString( key );
        stream.WriteString( value );
    }
}