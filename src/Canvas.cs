using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

//  todo:
//      expose window size to allow for resizing of window

partial class Canvas
{

    public delegate void RenderTopology(Point canvasSize);
    public RenderTopology renderFunction = null;

    private Point windowSize;
    private GraphicsDevice graphicsDevice;
    private SpriteBatch spriteBatch;
    private Texture2D texture = null;

    public Point WindowSize{
        get{return windowSize;}
        set{
            this.windowSize = value;
            this.UpdateTexture();
        }
    }

    public Canvas(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Point windowSize)
    {
        this.graphicsDevice = graphicsDevice;
        this.spriteBatch = spriteBatch;
        this.windowSize = windowSize;
    }

    public void ReSize(Point NewSize)
    {
        windowSize = NewSize;
    }
    public void Draw()
    {

        if (this.texture == null)
            throw new Exception("Tried to draw canvas without generating texture");
        Rectangle area = Camera.ModifiedDrawArea(new Rectangle(0,0, windowSize.X, windowSize.Y));           
        this.spriteBatch.Draw(this.texture, area, Color.White);
    }

    public void Update(MouseState mouseState, KeyboardState keyboardState)
    {
        Camera.UpdateByKeyboard(keyboardState);
        Camera.UpdateByMouse(mouseState);
        Camera.offset.X = Math.Min(0,Camera.offset.X);
        Camera.offset.Y = Math.Min(0,Camera.offset.Y);
        //Camera.offset.X = Math.Min(0,Camera.offset.X);
        //Camera.offset.Y = Math.Min(0,Camera.offset.Y);
        
    }
    public void UpdateTexture()
    {
        if (this.renderFunction == null)
            throw new Exception("Tried to render without a render function!");


        using (RenderTarget2D renderTargetIsAOffScreenBuffer = new (graphicsDevice, this.windowSize.X, this.windowSize.Y, false, SurfaceFormat.Color, DepthFormat.None))
        {

            spriteBatch.Begin();
                graphicsDevice.SetRenderTarget(renderTargetIsAOffScreenBuffer);
                this.spriteBatch.Draw(Window.whitePixelTexture, new Rectangle(0,0, windowSize.X, windowSize.Y), Color.White);
                this.renderFunction.Invoke(this.windowSize);
            spriteBatch.End();
            

            this.texture?.Dispose();
            using (MemoryStream stream = new())
            {
                renderTargetIsAOffScreenBuffer.SaveAsPng(stream, windowSize.X, windowSize.Y);
                this.texture = Texture2D.FromStream(graphicsDevice, stream);
            }

            graphicsDevice.SetRenderTarget(null);
        }
        
    }

}