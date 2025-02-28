namespace MSProj_Analog.Config
{
    public static class ConfigOptions
    {
        public const string ConnectionString = "Server=W11PC\\SQLEXPRESS;Database=MsProjAnalogDB;Trusted_Connection=True;TrustServerCertificate=True;";
        public static class Messages
        {
            public const string InvalidData = "Please enter a valid data.";
            public const string InvalidResourceType = "Invalid resource type";
            public const string Error = "Error";
        }

    }
}
