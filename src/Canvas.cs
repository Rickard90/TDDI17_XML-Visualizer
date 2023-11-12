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
    public int zoomLevel = 12; //default zoom level
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

    public bool Update(MouseState mouseState, KeyboardState keyboardState, Rectangle WindowSize)
    {
        bool change = false;
        Camera.Update(mouseState, keyboardState, WindowSize, canvasSize);
        if (keyboardState.IsKeyDown(Keys.LeftControl) || keyboardState.IsKeyDown(Keys.RightControl)) {
            if (keyboardState.IsKeyDown(Keys.Add) || Selection.GetMouseScrollDelta() > 0) {
                zoomLevel = Math.Min(maxZoom, zoomLevel+1);
                change = true;
            }
            if (keyboardState.IsKeyDown(Keys.OemMinus) || Selection.GetMouseScrollDelta() < 0) {
                zoomLevel = Math.Max(minZoom, zoomLevel-1);
                change = true;
            }
        }
        return change;
    }

    public void OffetControl(Rectangle WindowSize) {
        if (WindowSize.Width < canvasSize.X) {
            Camera.offset.X = Math.Min(0, Camera.offset.X);
            Camera.offset.X = Math.Max(WindowSize.Width-canvasSize.X, Camera.offset.X);
        } else {
            Camera.offset.X = Math.Max(0, Camera.offset.X);
            Camera.offset.X = Math.Min(WindowSize.Width-canvasSize.X, Camera.offset.X);
        }
        if (WindowSize.Height < canvasSize.Y) {
            Camera.offset.Y = Math.Min(0, Camera.offset.Y);
            Camera.offset.Y = Math.Max(WindowSize.Height-canvasSize.Y, Camera.offset.Y);
        } else {
            Camera.offset.Y = Math.Max(0, Camera.offset.Y);
            Camera.offset.Y = Math.Min(WindowSize.Height-canvasSize.Y, Camera.offset.Y);
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
        
    }

}