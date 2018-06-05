using System;
using System.Linq;
using Discord;
using MySql.Data.MySqlClient;

namespace new_bot.Modules
{
    public class sqlFunctionsHandler {
        public string Host;
        public string Port;
        public string Password;
        public string Username;
        public string Database;
        public string IntegratedSecurity;

        public ulong[] staffRoleIds = new ulong[] { 451422933634777109 , 451423817923952651, 132908718588166144, 453651587500605456 };

        public static MySqlConnection connection = new MySqlConnection();

        public MySqlCommand command = connection.CreateCommand();
        public MySqlDataReader reader;
       
        public void Initialize() { connection.ConnectionString = $"Data Source={Host};Integrated Security={IntegratedSecurity};port={Port};username={Username};password={Password};database={Database};";  }

       public void testConnection()
        {
            try
            {
                connection.Open();
                Sql_logLine("Connection opened.");
                Sql_logLine($"ServerVersion: {connection.ServerVersion}");
                Sql_logLine($"State: {connection.State}");
            }
            catch (MySqlException)
            {
                throw;
            } finally {
                closeConnection(connection);
            }
        }

        public void closeConnection(MySqlConnection connection)
        {
            if (connection.State != System.Data.ConnectionState.Closed)
            {
                connection.Close();
            }
        }

        public bool isStaff(IGuildUser user)
        {
            foreach(ulong roleId in user.RoleIds)
            {
                if (staffRoleIds.Contains<ulong>(roleId))
                {
                    return true;
                }
            }
            return false;
        }

        public Int32 numberOfWarns(ulong targetUserId)
        {
            try
            {
                command.CommandText = $"SELECT COUNT(warnedUser) FROM warningbot.warningdata WHERE warnedUser = '{targetUserId}';";
                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
            catch (MySqlException i)
            {
                Console.Write(i);
                throw;
            } finally { closeConnection(connection); }
        }

        public void warnUser(ulong targetUserId, ulong sourceUserId, string reason)
        {
            try
            {
                command.CommandText = $"INSERT INTO warningbot.warningdata(warnedUser, warnedBy, reason) VALUES ({targetUserId}, {sourceUserId}, '{Uri.EscapeDataString(reason)}');";
                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (MySqlException)
            {
                throw;
            } finally { closeConnection(connection); }
        }

        public bool IsBotChannel(ulong channelId)
        {
            if (channelId == 453651564079480832)
            {
                return true;
            } else
            {
                return false;
            }
        }

        public void removeWarns(ulong targetUser)
        {
            try
            {
                command.CommandText = $"DELETE * FROM warningbot.warningdata WHERE warnedUser = '{targetUser}';";
                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (MySqlException)
            {
                throw;
            }
            finally { closeConnection(connection); }
        }

        private void discord_logLine(string input) {
            ConsoleColor currentColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo)} Discord     {input}");
            Console.ForegroundColor = currentColor;
        }

        public void IoSql_logLine(string input)
        {
            ConsoleColor currentColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo)} SQL-IO\t     {input}");
            Console.ForegroundColor = currentColor;
        }

        private void Sql_logLine(string input) {
            ConsoleColor currentColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo)} SQL\t     {input}");
            Console.ForegroundColor = currentColor;
        }

        public void log_logLine(string input)
        {
            ConsoleColor currentColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo)} LOG\t     Message object{Environment.NewLine}{input}");
            Console.ForegroundColor = currentColor;
        }
    }
    public class CPlayerData
    {
        public ulong mPlayerId;
        public string mReason;
        public DateTime mTimeStamp;
        public string mpRepPoint;
    }
}
