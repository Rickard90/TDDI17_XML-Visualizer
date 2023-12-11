//using System.Globalization;

using System.Collections.ObjectModel;

static class ComponentFinder
{
    private static Dictionary<string, Component> componentDict = new();
    public static Component componentToGoTo = null;

    public static void Construct(TopologyHead top)
    {
        Component head = top.GetHead();
        RecursiveAdd(head);
    }

    public static void RecursiveAdd(Component currentComponent)
    {
        if (currentComponent.type == Component.Type.Thread) return;
        foreach (Component c in currentComponent.Children) {
            Insert(FullPathName(c), c);
            RecursiveAdd(c);
        }
    }

    public static void Insert(string name, Component component)
    {
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
            return null;
        }
    }

    public static string FullPathName(Component c)
    {
        List<string> strings = new();
        Component curr = c;
        do {
            strings.Add(curr.Name);
            curr = curr.Parent;
        } while (curr.Parent != null);
        strings.Reverse();
        string result = "/" + string.Join("/", strings);
        return result;
    }

    public static string GoToComponentWithName(string name)
    {
        componentToGoTo = Get(name);
        return "";
    }

    // For debugging purposes
    public static void Print()
    {
        Log.Print("[ComponentFinder] ALL COMPONENTS (EXCEPT PORTS)");
        foreach (KeyValuePair<string, Component> entry in ComponentFinder.componentDict)
        {
            Log.Print("Name: {0} Type: {1}" + entry.Key.ToString() + entry.Value.type.ToString());
        }
        Log.Print("END");
    }
}