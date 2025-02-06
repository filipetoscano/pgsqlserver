using System.Text;

namespace Pgwire.Bridge;

/// <summary />
public struct BufferReader
{
    /// <summary />
    public BufferReader()
    {
    }


    internal byte[] Buffer = new byte[ 8192 ];
    internal int NumberBytes = 0;
    internal int Position = 0;

    internal byte[] _int16 = new byte[ 2 ];
    internal byte[] _int32 = new byte[ 4 ];
    internal byte[] _int64 = new byte[ 8 ];


    /// <summary />
    internal char PeekChar()
    {
        var v = Buffer[ Position ];

        return (char) v;
    }


    /// <summary />
    internal char ReadChar()
    {
        var v = Buffer[ Position ];

        Position++;

        return (char) v;
    }


    /// <summary />
    internal int ReadInt32()
    {
        Array.Copy( Buffer, Position, _int32, 0, 4 );

        if ( BitConverter.IsLittleEndian == true )
            Array.Reverse( _int32 );

        Position += 4;
        return BitConverter.ToInt32( _int32 );
    }


    /// <summary />
    internal long ReadInt64()
    {
        Array.Copy( Buffer, Position, _int64, 0, 8 );

        if ( BitConverter.IsLittleEndian == true )
            Array.Reverse( _int64 );

        Position += 8;
        return BitConverter.ToInt32( _int64 );
    }


    /// <summary />
    internal string ReadString( int length )
    {
        var str = Encoding.ASCII.GetString( Buffer, Position, length );

        Position += length;

        return str;
    }
}