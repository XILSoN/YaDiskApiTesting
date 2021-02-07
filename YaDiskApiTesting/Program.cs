namespace YaDiskApiTesting
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using YaDiskApiTesting.Interfaces;
    using YaDiskApiTesting.Services;

    class Program
    {
        private static readonly YandexDiskFileUploader FileUploader;

        static Program()
        {
            FileUploader = new YandexDiskFileUploader();
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            ValidateArguments(args);

            AsyncWait(FileUploader.ExecuteAsync(args[0], args[1]));

            Console.WriteLine("All files successfully uploaded!");

            Console.ReadLine();
        }

        private static void ValidateArguments(string[] args)
        {
            if (!args.Any())
                throw new ArgumentException("Не указаны параметры загрузки файлов, а именно:" +
                                            "\n Путь до загружаемых файлов;" +
                                            "\n Целевая директория Yandex Disk");

            if (args.Length == 1)
                throw new ArgumentException("Не указана целевая дирекорития Yandex Disk");

            if (string.IsNullOrEmpty(args[0]))
                throw new ArgumentException("Не указан путь до загружаемых файлов");
        }

        private static void AsyncWait(Task task)
        {
            task.Wait();
        }
    }
}
