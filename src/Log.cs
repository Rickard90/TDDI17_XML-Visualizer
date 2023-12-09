public static class Log
{
    
    public static void LogCreate() 
    {
        if (Constants.debug == Constants.Debug.File) 
        {
            WriteLog("", false);
        }
    }

    public static void Print(string text) 
    {
        if (Constants.debug == Constants.Debug.File) 
        {
            WriteLog(text, true);
        } else if (Constants.debug == Constants.Debug.Console) 
        {
            Console.WriteLine(text);
        }
    }

    private static void WriteLog(string text, bool append) {
        using (StreamWriter logFile = new StreamWriter("log.txt", append))
        {
            logFile.WriteLine(text);
        }
    }

}