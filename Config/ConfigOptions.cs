using Microsoft.Win32;

namespace MSProj_Analog.Config
{
    public static class ConfigOptions
    {
        public const string Path = "C:\\Users\\User\\Desktop\\";
        public const string ConnectionString = "Server=W11PC\\SQLEXPRESS;Database=MsProjAnalogDBTest;Trusted_Connection=True;TrustServerCertificate=True;";
        public static class Messages
        {
            public const string InvalidData = "Please enter a valid data";
            public const string InvalidResourceType = "Invalid resource type";
            public const string Error = "Error";
        }
        public static SaveFileDialog Export(string title, string filter, string extension, string filename, string initDirectory = ConfigOptions.Path)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Title = title,
                Filter = filter, // Ограничение выбора
                DefaultExt = extension,
                FileName = filename, // Стандартное имя файла
                InitialDirectory = initDirectory // Начальная папка для сохранения
            };
            return saveFileDialog;
        }

        public static OpenFileDialog Import(string title, string filter, string extension, string initDirectory = ConfigOptions.Path)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = title,
                Filter = filter, // Ограничение выбора
                DefaultExt = extension,
                InitialDirectory = initDirectory // Начальная папка для сохранения
            };
            return openFileDialog;
        }

    }
}
