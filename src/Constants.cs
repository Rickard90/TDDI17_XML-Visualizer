using FontStashSharp;

public static class Constants
{
    public static readonly int toolbarHeight = 70;
    public static readonly int componentSize = 800/6;//base componentsize
    public static readonly int spacing = componentSize / 4;//spacing between components
    public static readonly int screenshotZoom = 40;
    public static readonly int defaultZoom = 12;
    public enum Debug {None, Console, File};
    public static readonly Debug debug = Debug.File;
}
