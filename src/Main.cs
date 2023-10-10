using System.IO.Enumeration;

class main {
    static void Main(string[] args){
        string fileName;
        if(args.Length >= 1){
          fileName = args[0];
        }
        else
        {
            Console.WriteLine("You did not enter a file. using file Fake Data Format");
            fileName = "Fake Data Format";
        }
        using var window = new Window(fileName);
        window.Run();
    }
}