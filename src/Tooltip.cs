using System.Globalization;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

class Tooltip
{
    private const int outlinePxSize = 4;
    private const int outlineTextBufferPxSize = 2; 

    private static readonly Color outlineColor = ColorConfiguration.color_3;
    private static readonly Color fillColor = Color.DarkGray;

    private static readonly Dictionary<Component,Tooltip> toolTipDict = new();

    public static SpriteBatch spriteBatch;
    public static GraphicsDevice graphicsDevice;

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
                toolTipDict.Add(component, currentTooltip);
            }
            else
            {
                currentTooltip = toolTipDict[component];
                currentTooltip.position = mousePosition;
            }
            CurrentArea = currentTooltip.DrawArea;
        }
    }

    // private static void RerenderTooltipTextures()
    // {
    //     foreach (KeyValuePair<Component, Tooltip> item in toolTipDict)
    //     {
    //         Tooltip tooltip = item.Value;
    //         tooltip.size = tooltip.CalculateSize();
    //         tooltip.drawTexture = tooltip.RenderTexture();
    //     }
    // }

    public static void DrawCurrent()
    {
        currentTooltip?.Draw();
    }

    public Rectangle DrawArea{get { return new Rectangle(position, size);}}
    public Point position;
    private readonly Point size;
    private readonly SpriteFontBase font;
    private readonly string text;

    private readonly Texture2D drawTexture;

    public Tooltip(Point drawPoint, string text, SpriteFontBase font)
    {
        this.position = drawPoint;
        this.font = font;


        this.text = text; 
        this.size = this.CalculateSize();

        //
        //  v fix text texture here v
        //
        this.drawTexture = this.RenderTexture();
        //
        //  ^ --------------------- ^
        //
    }

    private Tooltip(Point drawPoint, Component component, SpriteFontBase font)
        : this(drawPoint, $"{component.Parent}/{component.Name}\n{component.GetInfo()}", font)
    {}

    private Texture2D RenderTexture()
    {
        Texture2D result;

        using (RenderTarget2D renderTargetIsAOffScreenBuffer = new (graphicsDevice, this.size.X, this.size.Y, false, SurfaceFormat.Color, DepthFormat.None))
        {

            spriteBatch.Begin();
                graphicsDevice.SetRenderTarget(renderTargetIsAOffScreenBuffer);
                spriteBatch.Draw(Window.whitePixelTexture, new Rectangle(0,0, size.X, size.Y), Color.White);
                //  render v
                    this.Render();
                //  render ^
            spriteBatch.End();
            
            this.drawTexture?.Dispose();
            using (MemoryStream stream = new())
            {
                renderTargetIsAOffScreenBuffer.SaveAsPng(stream, size.X, size.Y);
                result = Texture2D.FromStream(graphicsDevice, stream);
            }

            graphicsDevice.SetRenderTarget(null);
        }  

        return result;      
    }

    private void Render()
    {
        Rectangle renderArea = new Rectangle(Point.Zero, size);

        // Draw outline
        spriteBatch.Draw(Window.whitePixelTexture, renderArea, Tooltip.outlineColor);
        // Draw inner box
        Rectangle copy = renderArea;
        copy.X += outlinePxSize;
        copy.Y += outlinePxSize;
        copy.Width -= outlinePxSize * 2;
        copy.Height -= outlinePxSize * 2;
        spriteBatch.Draw(Window.whitePixelTexture, copy, Tooltip.fillColor);

        // Draw text
        spriteBatch.DrawString(this.font, this.text, new Vector2(renderArea.X + outlineTextBufferPxSize + outlinePxSize, renderArea.Y + outlineTextBufferPxSize + outlinePxSize), Color.Black);
    }

    private void Draw()
    {
        spriteBatch.Draw(this.drawTexture, this.DrawArea, Color.White);
    }

    private Point CalculateSize()
    {
        Vector2 textSize = this.font.MeasureString(this.text);
        int width = ((int)textSize.X) + 2 * (outlinePxSize + outlineTextBufferPxSize);
        int height = ((int)textSize.Y) + 2 * (outlinePxSize + outlineTextBufferPxSize);
        return new Point(width, height);
    }

}