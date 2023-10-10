using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

class Tooltip
{
    private const int outlinePxSize = 4;
    private const int outlineTextBufferPxSize = 2; 
    private const int textSectionsBufferPxSize = 2; //  is added on top of textHeightBufferPxSize
    private const int textHeightBufferPxSize = 1;
    //private const int fontPxSize = 11;
    //  how much bigger width is allowed to be compared to height
    private const double ratioTolerence = 2.5;

    private static readonly Color outlineColor = Color.Azure;
    private static readonly Color fillColor = Color.DarkGray;


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
    public Point position;
    private readonly Point size;
    private readonly SpriteFont font;
    

    public Tooltip(Component component, SpriteFont font)
    {
        this.font = font;
        //  the font is have a constant size, monogame is to blame
        //font.MeasureString("text");

        string line_0 = $"{component.GetParent()}/{component.GetName()}";
        string line_1 = $"{component.GetInfo()}";

        this.text = new string[2]
        {
            line_0,
            line_1
        };

        string[][] result = new string[text.Length][];
        for(int i = 0; i < text.Length; i++)
        {
            string entry = text[i];
            string[] lines = entry.Split('\n');
            result[i] = lines;
        }

        int height = this.CalculateHeight(result);
        int width = this.CalculateWidth(result);

        //
        //  v fix text texture here v
        //

        //
        //  ^ --------------------- ^
        //

    }

    public void Draw()
    {
    }




    


    private int CalculateHeight(string[][] content)
    {
        if (content.Length == 0)
            throw new ArgumentException("Tooltip content is empty, this should never happen!");

        int result = 0;
        result += (outlinePxSize + outlineTextBufferPxSize) * 2;    // cover both end and start
        foreach (string[] list in content)
        {
            //  semi loop calculation, list.Count allows us to skip the loop
                result += list.Length * (this.font.LineSpacing + textHeightBufferPxSize);
            //
            result += textSectionsBufferPxSize + textHeightBufferPxSize;
        }
        result -= textSectionsBufferPxSize + textHeightBufferPxSize;

        return result;

    }

    //  returns width and index respectivly of the longest line
    private int CalculateWidth(string[][] content)
    {
        int width = 0;


        foreach (string[] list in content)
        {
            foreach (string line in list)
            {
                int textWidth = (int)this.font.MeasureString(line).X;
                int newWidth = textWidth + 2 * (outlinePxSize + outlinePxSize);
                if (newWidth > width)
                    width = newWidth;
            }
        }

        return width;

    }

    

}