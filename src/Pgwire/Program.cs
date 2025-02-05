using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pgwire.Bridge;
using Serilog;

namespace Pgwire;

/// <summary />
public class Program
{
    /// <summary />
    public static int Main( string[] args )
    {
        /*
         * 
         */
        HostApplicationBuilder builder = Host.CreateApplicationBuilder( args );

        builder.Services.AddSerilog( x =>
        {
            x.MinimumLevel.Debug();
            x.WriteTo.Console();

            x.Enrich.FromLogContext();
        } );
        builder.Services.AddHostedService<PgwireBridgeService>();

        builder.Services.Configure<PgwireBridgeOptions>( o =>
        {

        } );


        /*
         * 
         */
        IHost host = builder.Build();
        host.Run();

        return 0;
    }
}