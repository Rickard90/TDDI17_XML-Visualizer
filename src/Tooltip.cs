using System.Globalization;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

class Tooltip
{
    private const int outlinePxSize = 4;
    private const int outlineTextBufferPxSize = 2; 

    private static readonly Color outlineColor = Color.Red;
    private static readonly Color fillColor = Color.DarkGray;

    private static readonly Dictionary<Component,Tooltip> toolTipDict = new();

    public static SpriteBatch spriteBatch;

    private static Tooltip currentTooltip = null;
    public static Rectangle CurrentArea
    {
        get;
        private set;
    } = Rectangle.Empty;

    public static void SetTooltip(Component component, Point mousePosition, SpriteFontBase font)
    {
        if (component == null)
        {
            CurrentArea = Rectangle.Empty;
            currentTooltip = null;
        }
        else
        {
            
            if (!toolTipDict.ContainsKey(component))
            {
                currentTooltip = new Tooltip(mousePosition, component, font);
                toolTipDict.Add(component,currentTooltip);
            }
            else
            {
                currentTooltip = toolTipDict[component];
                currentTooltip.position = mousePosition;
            }
            CurrentArea = currentTooltip.DrawArea;
        }
        
        //Console.WriteLine($"Tooltip area : {CurrentArea}");
    }
    public static void DrawCurrent()
    {
        if (currentTooltip != null)
            currentTooltip.Draw();
    }

    public Rectangle DrawArea{get { return new Rectangle(position, size);}}
    public Point position;
    private readonly Point size;
    private readonly SpriteFontBase font;
    private readonly string text;


    private Tooltip(Point drawPoint, Component component, SpriteFontBase font)
    {
        this.position = drawPoint;
        this.font = font;
        //  the font is have a constant size, monogame is to blame
        //font.MeasureString("text");

        string line_0 = $"{component.GetParent()}/{component.GetName()}";
        string line_1 = $"{component.GetInfo()}";

        this.text = line_0 + '\n' + line_1; 

        this.size = this.CalculateSize();

        //
        //  v fix text texture here v
        //



        //
        //  ^ --------------------- ^
        //

    }

    private void Draw()
    {
        // Draw outline
        spriteBatch.Draw(Window.whitePixelTexture, this.DrawArea, Tooltip.outlineColor);
        // Draw inner box
        Rectangle copy = this.DrawArea;
        copy.X += outlinePxSize;
        copy.Y += outlinePxSize;
        copy.Width -= outlinePxSize * 2;
        copy.Height -= outlinePxSize * 2;
        spriteBatch.Draw(Window.whitePixelTexture, copy, Tooltip.fillColor);

        // Draw text
        spriteBatch.DrawString(this.font, this.text, new Vector2(CurrentArea.X + outlineTextBufferPxSize + outlinePxSize, CurrentArea.Y + outlineTextBufferPxSize + outlinePxSize), Color.Black);
    }

    private Point CalculateSize()
    {
        Vector2 textSize = this.font.MeasureString(this.text);
        int width = ((int)textSize.X) + 2 * (outlinePxSize + outlineTextBufferPxSize);
        int height = ((int)textSize.Y) + 2 * (outlinePxSize + outlineTextBufferPxSize);
        return new Point(width, height);
    }

    

    

}