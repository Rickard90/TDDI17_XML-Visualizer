//using System.Globalization;

static class ComponentFinder
{

    private static Dictionary<string, Component> componentDict = new();

    // In order to access "top" within Window, we need a reference to it, this is set within Window.LoadContent
    public static TopologyHead top = null;


    public static void Insert(string name, Component component)
    {
        // No error-handling here for now... For example, what if two components have the same name (key)?
        componentDict[name] = component;
    }

    public static Component Get(string name)
    {
        if (componentDict.TryGetValue(name, out Component retrievedComponent))
        {
            return retrievedComponent;
        }
        else
        {
            Console.WriteLine($"Component with key '{name}' not found.");
            return top.GetCurrent();
        }
    }
}