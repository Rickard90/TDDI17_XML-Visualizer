using System.Globalization;
using Microsoft.Xna.Framework;

class Tooltip
{
    private const int fontSize = 11;


    private static Component currentComponent = null;
    public static Rectangle CurrentArea
    {
        get;
        private set;
    } = Rectangle.Empty;

    public static void SetTooltip(Component component, Point mousePosition)
    {
        currentComponent = component;
        if (component == null)
            CurrentArea = Rectangle.Empty;
        else
            CurrentArea = new Rectangle(mousePosition, new Point(100,100));
        
        Console.WriteLine($"Tooltip area : {CurrentArea}");
    }



    public readonly string[] text;
    public Rectangle DrawArea{get { return new Rectangle(position, size);}}
    private readonly Point size;
    private Point position;

    public Tooltip(Component component)
    {
        throw new NotImplementedException();

        string line_0 = $"{component.GetParent()}/{component.GetName()}";
        string line_1 = $"{component.GetInfo()}";

        this.text = new string[2]
        {
            line_0,
            line_1
        };

        //
        //  fix text texture here
        //
    }

    public void Draw()
    {
        throw new NotImplementedException();

    }

    

}