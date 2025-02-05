using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Sockets;
using System.Text;

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

                /*
                 * 
                 */
                var stream = client.GetStream();

                var msg = Encoding.UTF8.GetBytes( "echo ECHO" );
                await stream.WriteAsync( msg, stoppingToken );


                /*
                 * 
                 */
                _logger.LogInformation( "Client {ClientId}: Close", client.Client.Handle );
                client.Close();
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
}