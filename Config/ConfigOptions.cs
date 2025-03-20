using Microsoft.Win32;
using System.Windows;
using System.Windows.Media;

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

        public static SaveFileDialog Export(string title, string filter, string extension, string filename, string initDirectory = Path)
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
        public static OpenFileDialog Import(string title, string filter, string extension, string initDirectory = Path)
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

        public static void MessageBoxError(string messageBoxText, string messageBoxLabel)
        {
            MessageBox.Show(messageBoxText, messageBoxLabel, MessageBoxButton.OK, MessageBoxImage.Error);
        }
        public static void MessageBoxInfo(string messageBoxText, string messageBoxLabel)
        {
            MessageBox.Show(messageBoxText, messageBoxLabel, MessageBoxButton.OK, MessageBoxImage.Information);
        }
        public static void MessageBoxExport(string messageBoxLabel = "Экспорт")
        {
            MessageBox.Show("Экспорт завершен!", messageBoxLabel, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static List<Color> chartColors = new()
        {
            Color.FromRgb(251, 228, 214),
            Color.FromRgb(38, 31, 179),
            Color.FromRgb(22, 17, 121),
            Color.FromRgb(255, 99, 71),  // Tomato
            Color.FromRgb(135, 206, 250), // LightSkyBlue
            Color.FromRgb(50, 205, 50),  // LimeGreen
            Color.FromRgb(255, 165, 0),  // Orange
            Color.FromRgb(148, 0, 211),  // DarkViolet
            Color.FromRgb(139, 69, 19),  // SaddleBrown
            Color.FromRgb(0, 255, 255),  // Aqua
            Color.FromRgb(255, 20, 147), // DeepPink
            Color.FromRgb(0, 128, 128),  // Teal
            Color.FromRgb(173, 255, 47),  // GreenYellow
            Color.FromRgb(12, 9, 80)
        };
    }
}
