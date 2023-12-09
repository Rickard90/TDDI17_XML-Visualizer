using FontStashSharp;

public static class Constants
{
    public static readonly int ToolbarHeight = 70;
    public static readonly int ComponentSize = 800/6;//base componentsize
    public static readonly int Spacing = ComponentSize / 4;//spacing between components
    public static readonly int screenshotZoom = 40;
    public enum Debug {None, Console, File};
    public static readonly Debug debug = Debug.File;
}
