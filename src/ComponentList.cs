




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

    public static List<Component> GetChoices(string input)
    {
        int index = ComponentList.BinarySearch(input.First());
        if (index == -1)
            return null;
        List<Component> choices = new();
        List<Component> list = ComponentList.list;  // For readability
        while (index < list.Count && list[index].Name.First() == input.First()) {
            if (list[index].Name.StartsWith(input))
                choices.Add(list[index]);
            ++index;
        }
        return choices;
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
            if (c < list[mid].Name.First())
                end = mid-1;
            else if (c > list[mid].Name.First())
                begin = mid+1;
            else
                break;
        }
        if (c != list[mid].Name.First())
            return -1;
        while (mid > 0 && list[mid-1].Name.First() == c)
            --mid;
        return mid;
    }

    // For debugging purposes
    public static void Print()
    {
        Console.WriteLine("ALL COMPONENTS (EXCEPT PORTS)");
        foreach (Component c in ComponentList.list)
            Console.WriteLine("Name: {0} Type: {1}", c.Name, c.type);
        Console.WriteLine("END");
    }
}