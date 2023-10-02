using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

//  todo:
//      expose window size to allow for resizing of window

partial class Canvas
{

    private LevelOfDetail levelOfDetail;
    public LevelOfDetail LevelOfDetail
    {   get {return this.levelOfDetail;}
        set {
            this.levelOfDetail = value; 
            this.UpdateTexture();
        }
    }
    public delegate void RenderTopology(LevelOfDetail levelOfDetail);
    public RenderTopology renderFunction = null;

    private Point windowSize;
    private GraphicsDevice graphicsDevice;
    private SpriteBatch spriteBatch;
    private Texture2D texture = null;

    public Canvas(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Point windowSize, LevelOfDetail levelOfDetail = LevelOfDetail.Max)
    {
        this.graphicsDevice = graphicsDevice;
        this.spriteBatch = spriteBatch;
        this.windowSize = windowSize;
        this.levelOfDetail = levelOfDetail;
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
        Camera.UpdateByMouse(mouseState, this.windowSize);

    }

    public void UpdateTexture()
    {
        if (this.levelOfDetail > LevelOfDetail.Max)
            throw new Exception($"Tried to render invalid level of detail : {this.levelOfDetail}");
        if (this.renderFunction == null)
            throw new Exception("Tried to render without a render function!");


        RenderTarget2D renderTargetIsAOffScreenBuffer = new RenderTarget2D(graphicsDevice, this.windowSize.X, this.windowSize.Y, false, SurfaceFormat.Color, DepthFormat.None);

        spriteBatch.Begin();
            graphicsDevice.SetRenderTarget(renderTargetIsAOffScreenBuffer);
            this.spriteBatch.Draw(Window.whitePixelTexture, new Rectangle(0,0, windowSize.X, windowSize.Y), Color.White);
            this.renderFunction.Invoke(LevelOfDetail);
        spriteBatch.End();

        using (MemoryStream stream = new())
        {
            renderTargetIsAOffScreenBuffer.SaveAsPng(stream, windowSize.X, windowSize.Y);
            this.texture = Texture2D.FromStream(graphicsDevice, stream);
        }

        graphicsDevice.SetRenderTarget(null);
        
    }

}