namespace databasePG.Config
{
    public class DatabaseConfig
    {
        public string ConnectionString { get; }

        public DatabaseConfig(string? connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("Connection string cannot be null or empty.");
            }

            ConnectionString = connectionString;
        }
    }

}
