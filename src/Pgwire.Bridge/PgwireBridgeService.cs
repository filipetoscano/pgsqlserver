using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pgwire.Bridge.Messages;
using System.Net;
using System.Net.Sockets;
namespace Pgwire.Bridge;

/// <summary />
public class PgwireBridgeService : BackgroundService, IHostedService
{
    private readonly TcpListener _server;
    private readonly ILogger<PgwireBridgeService> _logger;


    /// <summary />
    public PgwireBridgeService( IOptions<PgwireBridgeOptions> options, ILogger<PgwireBridgeService> logger )
    {
        var o = options.Value;

        _server = new TcpListener( IPAddress.Any, o.Port );
        _logger = logger;
    }


    /// <inheritdoc />
    protected override async Task ExecuteAsync( CancellationToken stoppingToken )
    {
        /*
         * 
         */
        _logger.LogInformation( "Server {Endpoint}: Start", _server.LocalEndpoint );
        _server.Start();


        /*
         *
         */
        try
        {
            while ( stoppingToken.IsCancellationRequested == false )
            {
                var client = await _server.AcceptTcpClientAsync( stoppingToken );
                _logger.LogInformation( "Client {ClientId}: Connected", client.Client.Handle );

                _ = Task.Run( () => HandleClient( client, stoppingToken ) );
            }
        }
        catch ( TaskCanceledException )
        {
            // Snuff
        }
        finally
        {
            _server.Stop();
        }
    }


    /// <summary />
    private async Task HandleClient( TcpClient client, CancellationToken stoppingToken )
    {
        /*
         * 
         */
        using ( _logger.BeginScope( new Dictionary<string, object>()
        {
            { "ClientId", client.Client.Handle }
        } ) )
        {
            var br = new BufferReader();
            var stream = client.GetStream();


            while ( stoppingToken.IsCancellationRequested == false )
            {
                var n = await stream.ReadAsync( br.Buffer, stoppingToken );

                if ( n == 0 )
                    continue;


                /*
                 * 
                 */
                if ( SSLRequest.Is( br.Buffer ) == true )
                {
                    _logger.LogDebug( "SSL Negotation: Decline with N/no" );

                    stream.Write( new byte[] { (byte) 'N' } );
                    continue;
                }


                /*
                 * 
                 */
                if ( Terminate.Is( br.Buffer ) == true )
                {
                    _logger.LogDebug( "Quit" );
                    break;
                }


                /*
                 * 
                 */
                if ( StartupMessage.Is( br.Buffer ) == true )
                {
                    var kv = StartupMessage.Parse( br.Buffer );

                    foreach ( var kvp in kv )
                        _logger.LogDebug( "Startup: {Key}={Value}", kvp.Key, kvp.Value );

                    AuthenticationOk.Write( stream );
                    BackEndKey.Write( stream, (int) client.Client.Handle, 4242 );
                    ParameterStatus.Write( stream, "server_version", "9.5" );
                    ReadyForQuery.Write( stream );

                    continue;
                }


                /*
                 * 
                 */
                if ( Query.Is( br.Buffer ) == true )
                {
                    var sql = Query.Parse( br );
                    _logger.LogDebug( "Query: {Query}", sql );

                    continue;
                }
            }

            _logger.LogInformation( "Closing" );
            client.Close();
        }
    }
}