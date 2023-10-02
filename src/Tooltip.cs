using Microsoft.Xna.Framework;

class Tooltip
{
    private const int fontSize = 11;

    public static void SetTooltip(Component component, Point mousePosition)
    {
        throw new NotImplementedException();

    }

    public static Rectangle CurrentArea()
    {
        throw new NotImplementedException();
        return Rectangle.Empty;
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