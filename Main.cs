using System.IO.Enumeration;

class MainClass {
    static void Main(string[] args){
        string fileName;
        if(args.Length == 1){
          fileName = args[0];
        }
        else if (args.Length > 1)
        {
            Console.WriteLine($"Incorrect number of arguments given ({args.Length}):");
            Console.WriteLine($"Usage:\n    {System.AppDomain.CurrentDomain.FriendlyName} <path>");
            Console.WriteLine("Using default file: Fake Data Format");
            fileName = "Fake Data Format";
        }
        else
        {
            Console.WriteLine("You did not enter a file. \nUsing default file: Fake Data Format");
            fileName = "Fake Data Format";
        }
        using var window = new Window(fileName);
        window.Run();
    }
}