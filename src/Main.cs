using System.IO.Enumeration;

class MainClass {
    static void Main(string[] args){
        Log.LogCreate();


        string fileName;
        if(args.Length == 1){
            if (args[0] == "-h" || args[0] == "--help")
            {
                if (File.Exists("help.txt"))
                    Console.WriteLine(File.ReadAllText("help.txt"));
                else
                    Console.WriteLine("help file does not exist");
                return;
            }
            else
                fileName = args[0];
     
        }
        else if (args.Length > 1)
        {
            Console.WriteLine($"Incorrect number of arguments given ({args.Length}):");
            Console.WriteLine($"Usage: {System.AppDomain.CurrentDomain.FriendlyName} [topology data folder path]");
            Console.WriteLine("Getting help: -h or --help");
            return;
        }
        else
        {
            Console.WriteLine("You did not enter a file. \nUsing default file: Fake Data Format");
            fileName = "Fake Data Format";
        }

        TopologyHead topologyHead = new TopologyHead(fileName);
        if (topologyHead.GetCurrent().IsEmpty())
            Console.WriteLine($"Could not load topology with file path \"{fileName}\""); 
        else  
        {
            using var window = new Window(topologyHead);
            window.Run();
        }    

    }
}