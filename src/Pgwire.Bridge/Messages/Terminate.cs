namespace Pgwire.Bridge.Messages;

/// <summary />
public class Terminate : IFrontend
{
    /// <summary />
    public static bool Is( byte[] buffer )
    {
        return buffer[ 0 ] == (byte) 'X';
    }
}