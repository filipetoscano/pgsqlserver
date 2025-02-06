using Sharprompt;
using System.Data;

namespace Pgprompt;

/// <summary />
public class Program
{
    /// <summary />
    public async static Task<int> Main( string[] args )
    {
        Prompt.ThrowExceptionOnCancel = true;


        /*
         * 
         */
        await using var conn = new Npgsql.NpgsqlConnection();
        conn.ConnectionString = "Host=localhost;Port=5432;Username=test;Password=test;";


        /*
         * 
         */
        try
        {
            await conn.OpenAsync();
        }
        catch ( Exception ex )
        {
            Console.WriteLine( ex.ToString() );
            return 1;
        }


        /*
         * 
         */
        try
        {
            while ( true )
            {
                var sql = Prompt.Input<string>( "> " ).Trim();
                Console.WriteLine( sql );

                if ( sql == ".q" )
                    return 0;

                await using var cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;

                await using ( var r = cmd.ExecuteReader() )
                {
                    do
                    {
                        while ( r.Read() )
                        {
                            for ( int i = 0; i < r.FieldCount; i++ )
                            {
                                var name = r.GetName( i );
                                var value = r.GetValue( i );

                                Console.WriteLine( "{0} = {1}", name, value );
                            }
                        }
                    } while ( r.NextResult() );
                }

            }
        }
        catch ( PromptCanceledException )
        {
        }
        finally
        {
            await conn.CloseAsync();
        }

        return 0;
    }
}