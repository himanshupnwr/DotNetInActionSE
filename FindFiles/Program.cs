using CommandLine;

namespace FindFiles
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            var results = Parser.Default.ParseArguments<FileOptions>(args).WithParsed<FileOptions>(options =>
            {
                if (options.Filename != null)
                {
                    SearchFile(new FileInfo(options.Filename),options.Text!);
                }
                else
                {
                    string? filename;
                    while (!string.IsNullOrWhiteSpace(filename = Console.ReadLine()))
                    {
                        SearchFile(new FileInfo(filename),
                        options.Text!);
                    }
                }
            });
        }

        // folder’s nonexistence is something that’s easy to check by testing the DirectoryInfo.Exists property
        static void RecursiveFind(DirectoryInfo folder, string filter, bool recurse)
        {
            if (!folder.Exists)
            {
                Console.WriteLine($"{folder.FullName} does not exist");
                return;
            }

            Console.WriteLine(folder.FullName);
            try
            {
                foreach (var file in folder.EnumerateFiles(filter))
                {
                    Console.WriteLine($"\t{file.Name}");
                }
            }
            catch (System.Security.SecurityException)
            {
                return;
            }

            if (recurse)
            {
                foreach (var subFolder in folder.GetDirectories())
                {
                    RecursiveFind(subFolder, filter, recurse);
                }
            }
        }

        static void SearchFile(FileInfo file, string text)
        {
            if (!file.Exists)
            {
                Console.Error.WriteLine( $"{file.FullName} does not exist");
                return;
            }

            try
            {
                using var reader = file.OpenText();
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains(text,StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine(line);
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.Error.WriteLine($"Unauthorized: {file.FullName}");
            }
            catch (IOException)
            {
                Console.Error.WriteLine($"IO error: {file.FullName}");
            }
        }
    }
}
