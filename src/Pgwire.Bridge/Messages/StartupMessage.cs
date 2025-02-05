namespace Pgwire.Bridge.Messages;

/// <summary />
public class StartupMessage : IFrontend
{
    /// <summary />
    public static bool Is( byte[] buffer )
    {
        return buffer.Length > 8
            && buffer[ 4 ] == 0x00
            && buffer[ 5 ] == 0x03
            && buffer[ 6 ] == 00
            && buffer[ 7 ] == 00;
    }


    /// <summary />
    public static Dictionary<string, string> Parse( byte[] buffer )
    {
        var keyValues = new Dictionary<string, string>();

        int i = 8;
        while ( i < buffer.Length )
        {
            string key = "";

            while ( buffer[ i ] != 0x00 )
            {
                key += (char) buffer[ i ];
                i++;
            }

            i++;
            string value = "";

            while ( buffer[ i ] != 0x00 )
            {
                value += (char) buffer[ i ];
                i++;
            }
            i++;

            if ( key != "" )
                keyValues.Add( key, value );

            if ( buffer[ i + 1 ] == 0x00 )
                break;
        }

        return keyValues;
    }
}