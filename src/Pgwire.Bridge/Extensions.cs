using System.Text;

namespace Pgwire.Bridge;

internal static class Extensions
{
    /// <summary />
    internal static void WriteChar( this Stream stream, char value )
    {
        stream.WriteByte( (byte) value );
    }


    /// <summary />
    internal static void WriteInt32( this Stream stream, int value )
    {
        var bytes = BitConverter.GetBytes( value );

        if ( BitConverter.IsLittleEndian == true )
            Array.Reverse( bytes );

        stream.Write( bytes );
    }


    /// <summary />
    internal static void WriteString( this Stream stream, string value )
    {
        var bytes = Encoding.ASCII.GetBytes( value );

        stream.Write( bytes );
        stream.WriteByte( Messages.B.Null );
    }
}