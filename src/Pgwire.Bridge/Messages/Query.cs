using System.Text;

namespace Pgwire.Bridge.Messages;

/// <summary />
public class Query : IFrontend
{
    /// <summary />
    public static bool Is( byte[] buffer )
    {
        return buffer.Length > 5 
            && buffer[ 0 ] == (byte) 'Q';
    }


    /// <summary />
    public static string Parse( byte[] buffer )
    {
        var str = Encoding.ASCII.GetString( buffer, 5, buffer.Length - 5 );

        return str;
    }
}