using System.Timers;

public static class FileMonitor
{
    private static string targetFilePath=string.Empty;
    private static string previousContent=string.Empty;
    private static FileSystemWatcher watcher=default!;
    private static System.Timers.Timer timer=default!;

    public static void InitializeFileMonitor()
    {
        Console.WriteLine("File Change Monitor");
        Console.WriteLine("-------------------");
        Console.Write("Enter the full path of the target text file : ");
        targetFilePath = Console.ReadLine()!;

        // Check if the specified file exists
        if (!File.Exists(targetFilePath))
        {
            Console.WriteLine($"The file '{targetFilePath}' does not exist. Creating a new file.");
            try
            {
                File.WriteAllText(targetFilePath, ""); // Create an empty file
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating file: {ex.Message}");
                return;
            }
        }

        // Initialize previous content of the file
        previousContent = File.ReadAllText(targetFilePath);

        // File watcher setup
        watcher = new FileSystemWatcher();
        watcher.Path = Path.GetDirectoryName(targetFilePath)!;
        watcher.Filter = Path.GetFileName(targetFilePath);
        watcher.NotifyFilter = NotifyFilters.LastWrite;
        watcher.Changed += OnFileChanged;
        watcher.EnableRaisingEvents = true;
        Console.WriteLine("File name : " + Path.GetFileName(targetFilePath));
        
        // Timer setup to check every 15 seconds
        timer = new System.Timers.Timer(5); // 15 seconds in milliseconds
        timer.Elapsed += OnTimerElapsed!;
        timer.AutoReset = true;
        timer.Enabled = true;

        Console.WriteLine($"Monitoring changes in '{targetFilePath}'. Press Enter to exit.");
        Console.ReadLine();
        watcher.Dispose();
        timer.Dispose();
    }

    #region Private methods
    //To catch on change event
    private static void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        string currentContent = File.ReadAllText(targetFilePath);
        if (currentContent != previousContent)
        {
            Console.WriteLine($"\nFile '{targetFilePath}' Has Changed");
            Console.WriteLine("\nPrevious Contents : ");
            Console.WriteLine($"{previousContent}\n\n");
            Console.WriteLine("New Contents :");
            Console.WriteLine($"{currentContent}\n");
            Console.WriteLine("---------------------------------------");

            // Update previous content to current content
            previousContent = currentContent;
        }
    }

    // This method is used to keep the application running
    private static void OnTimerElapsed(object sender, ElapsedEventArgs e)
    {
        
    }
    #endregion
}
