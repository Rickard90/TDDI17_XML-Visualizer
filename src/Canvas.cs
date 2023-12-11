using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

//  todo:
//      expose window size to allow for resizing of window

partial class Canvas
{

    public delegate void RenderTopology(Point canvasSize);
    public RenderTopology renderFunction = null;

    private Point canvasSize;
    private GraphicsDevice graphicsDevice;
    private SpriteBatch spriteBatch;
    private Texture2D texture = null;
    public int zoomLevel = Constants.defaultZoom;
    private const int minZoom = 9;
    private const int maxZoom = 25;

    public Point CanvasSize{
        get{return canvasSize;}
        set{
            this.canvasSize = value;
            this.UpdateTexture();
        }
    }

    public Canvas(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Point windowSize)
    {
        this.graphicsDevice = graphicsDevice;
        this.spriteBatch = spriteBatch;
        this.canvasSize = windowSize;
    }

    public void ReSize(Point NewSize)
    {
        canvasSize = NewSize;
    }
    public void Draw()
    {
        if (this.texture == null)
            throw new Exception("Tried to draw canvas without generating texture");
        Rectangle area = Camera.ModifiedDrawArea(new Rectangle(0,0, canvasSize.X, canvasSize.Y));           
        this.spriteBatch.Draw(this.texture, area, Color.White);
    }

    public void Update(Selection.CanvasZoomChange zoomChange, Rectangle WindowSize)
    {
        if (zoomChange == Selection.CanvasZoomChange.In) {
            zoomLevel = Math.Min(maxZoom, zoomLevel+1);
        } else if (zoomChange == Selection.CanvasZoomChange.Out) {
            zoomLevel = Math.Max(minZoom, zoomLevel-1);
        }
    }

    public void ScrollCanvasToArea(Rectangle target, Rectangle windowRect) 
    {
        if ( target.Y > - Camera.offset.Y + windowRect.Height ) {
            Camera.offset.Y = -target.Y - 4*target.Height/3 + windowRect.Height;
        } else if ( target.Y < - Camera.offset.Y ) {
            Camera.offset.Y = -target.Y + 2*target.Height/3;
        }
        if ( target.X+target.Width > - Camera.offset.X + windowRect.Width ) {
            Camera.offset.X = -target.X-target.Width - 4*target.Width/3 + windowRect.Width;
        } else if ( target.X < - Camera.offset.X ) {
            Camera.offset.X = -target.X + 2*target.Width/3;
        }
    }
    public void UpdateTexture()
    {
        if (this.renderFunction == null)
            throw new Exception("Tried to render without a render function!");


        using (RenderTarget2D renderTargetIsAOffScreenBuffer = new (graphicsDevice, this.canvasSize.X, this.canvasSize.Y, false, SurfaceFormat.Color, DepthFormat.None))
        {

            spriteBatch.Begin();
                graphicsDevice.SetRenderTarget(renderTargetIsAOffScreenBuffer);
                this.spriteBatch.Draw(Window.whitePixelTexture, new Rectangle(0,0, canvasSize.X, canvasSize.Y), Color.White);
                this.renderFunction.Invoke(this.canvasSize);
            spriteBatch.End();
            

            this.texture?.Dispose();
            using (MemoryStream stream = new())
            {
                renderTargetIsAOffScreenBuffer.SaveAsPng(stream, canvasSize.X, canvasSize.Y);
                this.texture = Texture2D.FromStream(graphicsDevice, stream);
            }

            graphicsDevice.SetRenderTarget(null);
        }

        //this.SaveAsPng("test.png");
        
    }

    public void SaveAsPng(string path)
    {
        if (!path.EndsWith(".png"))
            throw new ArgumentException("Path should end with \".png\" since the output file is .png !");

        using MemoryStream data = new MemoryStream();
        texture.SaveAsPng(data, this.texture.Width, this.texture.Height);
        File.WriteAllBytes(path, data.ToArray());

    }

}