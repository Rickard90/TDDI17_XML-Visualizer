




using FontStashSharp;

static class ComponentList
{
    public static List<Component> list = new();

    public static void Construct(TopologyHead top)
    {
        Component head = top.GetHead();
        RecursiveAdd(head);
    }

    public static void RecursiveAdd(Component currentComponent)
    {
        if (currentComponent.type == Component.Type.Thread) return;
        foreach (Component c in currentComponent.Children) {
            ComponentList.list.Add(c);
            RecursiveAdd(c);
        }
    }

    public static void Sort()
    {
        ComponentList.list.Sort((c1, c2) => c1.Name.CompareTo(c2.Name));
    }

    public static string[] GetSuggestions(string input)
    {
        if (input.Length == 0)
            return Array.Empty<string>();
        input = input.ToLower();
        int index = ComponentList.BinarySearch(input.First());
        if (index == -1)
            return Array.Empty<string>();
        List<string> suggestions = new();
        List<Component> list = ComponentList.list;  // For readability
        while (index < list.Count && char.ToLower(list[index].Name.First()) == input.First()) {
            if (list[index].Name.ToLower().StartsWith(input))
                suggestions.Add(ComponentFinder.FullPathName(list[index]));
            ++index;
        }
        return suggestions.ToArray();
    }

    public static int BinarySearch(char c)
    {
        List<Component> list = ComponentList.list;  // For readability
        int begin = 0;
        int end = list.Count - 1;
        int mid = 1337;
        while (begin <= end)
        {
            mid = (begin + end) / 2;
            if (c < char.ToLower(list[mid].Name.First()))
                end = mid-1;
            else if (c > char.ToLower(list[mid].Name.First()))
                begin = mid+1;
            else
                break;
        }
        if (c != char.ToLower(list[mid].Name.First()))
            return -1;
        while (mid > 0 && char.ToLower(list[mid-1].Name.First()) == c)
            --mid;
        return mid;
    }

    public static string WhenEnteredFunction(string fullPathName)
    {
        
        return "";
    }

    // For debugging purposes
    public static void Print()
    {
        Log.Print("ALL COMPONENTS (EXCEPT PORTS)");
        foreach (Component c in ComponentList.list)
            Log.Print("Name: {0} Type: {1}, FullPathName: {2}" + c.Name.ToString() + c.type.ToString() + ComponentFinder.FullPathName(c));
        Log.Print("END");
    }
}