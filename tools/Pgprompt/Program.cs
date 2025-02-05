using Sharprompt;
using System.Data;

namespace Pgprompt;

/// <summary />
public class Program
{
    /// <summary />
    public static int Main( string[] args )
    {
        Prompt.ThrowExceptionOnCancel = true;


        /*
         * 
         */
        var conn = new Npgsql.NpgsqlConnection();
        conn.ConnectionString = "Host=localhost;Port=5432;Username=test;Password=test;";


        /*
         * 
         */
        try
        {
            conn.Open();
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

                var cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;

                using ( var r = cmd.ExecuteReader() )
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
            conn.Close();
        }

        return 0;
    }
}