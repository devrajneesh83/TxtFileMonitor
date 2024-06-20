using System.Timers;

public class FileMonitor
{
    private static string targetFilePath = string.Empty;
    private static string previousContent = string.Empty;
    private static FileSystemWatcher watcher = default!;
    private static System.Timers.Timer timer = default!;

    public static void InitializeFileMonitor()
    {
        Console.WriteLine("File Change Monitor");
        Console.WriteLine("-------------------");

        bool isValidFile = false;

        while (!isValidFile)
        {
            Console.Write("Enter the full path of the target text file (.txt only, default D: drive) :  ");
            string userInput = Console.ReadLine()!.Trim();

            if (string.IsNullOrEmpty(Path.GetPathRoot(userInput)))
            {
                targetFilePath = Path.Combine("D:\\", userInput); 
            }
            else
            {
                targetFilePath = userInput;
            }

            if (!File.Exists(targetFilePath))
            {
                try
                {
                    // create an empty file
                    File.WriteAllText(targetFilePath, "");
                    if (Path.GetExtension(targetFilePath).ToLower() != ".txt")
                    {
                        isValidFile = false;
                    }
                    else
                    {
                        isValidFile = true;
                        Console.WriteLine($"Created new file: '{targetFilePath}'");
                    }
                        
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating file '{targetFilePath}': {ex.Message}");
                }
            }
            else
            {
                if (Path.GetExtension(targetFilePath).ToLower() == ".txt")
                {
                    isValidFile = true; 
                }
                else
                {
                    Console.WriteLine($"Error: The file '{targetFilePath}' is not a .txt file.");
                }
            }
            if (!isValidFile)
            {
                Console.WriteLine("Please enter a valid .txt file path.");
            }
        }

        // Initialize previous content of the file
        try
        {
            previousContent = File.ReadAllText(targetFilePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading file '{targetFilePath}': {ex.Message}");
            return;
        }

        // File watcher setup
        watcher = new FileSystemWatcher();
        watcher.Path = Path.GetDirectoryName(targetFilePath)!;
        watcher.Filter = Path.GetFileName(targetFilePath);
        watcher.NotifyFilter = NotifyFilters.LastWrite;
        watcher.Changed += OnFileChanged;
        watcher.EnableRaisingEvents = true;

        // Timer setup to check every 15 seconds
        timer = new System.Timers.Timer(15000); // 15 seconds in milliseconds
        timer.Elapsed += OnTimerElapsed!;
        timer.AutoReset = true;
        timer.Enabled = true;

        Console.WriteLine($"Monitoring changes in '{targetFilePath}'. Press Enter to exit.");
        Console.ReadLine();

        // Clean up
        watcher.Dispose();
        timer.Dispose();
    }

    #region Private methods
    private static void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        string currentContent;
        try
        {
            currentContent = File.ReadAllText(targetFilePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading file '{targetFilePath}': {ex.Message}");
            return;
        }

        if (currentContent != previousContent)
        {
            Console.WriteLine($"File '{targetFilePath}' is changed:");
            Console.WriteLine($"\nPrevious content: {previousContent}");
            Console.WriteLine($"\nCurrent content: {currentContent}");
            Console.WriteLine("\n---------------------------------------");
            previousContent = currentContent;
        }
    }

    private static void OnTimerElapsed(object sender, ElapsedEventArgs e)
    {
        // This method is used to keep the application running
    }
    #endregion
}

