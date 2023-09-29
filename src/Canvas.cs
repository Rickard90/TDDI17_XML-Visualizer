using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//  todo:
//      expose window size to allow for resizing of window

class Canvas
{

    public Point CopyPoint
    {
        get {return this.copyArea.Location;}
        set {this.copyArea.Location = value;}
    }
    public double Zoom      //  please, use reasonable values, otherwise it might look weird
    {
        get {return windowSize.X / this.copyArea.Width;}
        set {
            this.copyArea.Width = (int)(this.windowSize.X / value);
            this.copyArea.Height = (int)(this.windowSize.Y / value);
        }
    }

    private LevelOfDetail levelOfDetail;
    public LevelOfDetail LevelOfDetail
    {
        get {return this.levelOfDetail;}
        set {this.levelOfDetail = value; this.UpdateTexture();}
    }
    public delegate void RenderTopology(LevelOfDetail levelOfDetail);
    public RenderTopology renderFunction = null;

    private Rectangle copyArea; //  the current area of the canvas that should be drawn, needs to match the ratio of the windowSize
    private Point windowSize;
    private GraphicsDevice graphicsDevice;
    private SpriteBatch spriteBatch;
    private Texture2D texture = null;
    private Point textureSize;
    private Color clearColor = new();

    public Canvas(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Point windowSize, LevelOfDetail levelOfDetail = LevelOfDetail.Max)
    {
        this.graphicsDevice = graphicsDevice;
        this.spriteBatch = spriteBatch;
        this.windowSize = windowSize;
        this.levelOfDetail = levelOfDetail;

        this.copyArea = new Rectangle(Point.Zero, windowSize);
    }

    public void Draw()
    {
        if (this.texture == null)
            throw new Exception("Tried to draw canvas without generating texture");
        this.spriteBatch.Draw(this.texture, new Rectangle(0, 0, windowSize.X, windowSize.Y), copyArea, Color.White);

    }

    public void UpdateTexture()
    {
        if (this.levelOfDetail > LevelOfDetail.Max)
            throw new Exception($"Tried to render invalid level of detail : {this.levelOfDetail}");
        if (this.renderFunction == null)
            throw new Exception("Tried to render without a render function!");

        this.textureSize = this.windowSize;
        this.texture = new Texture2D(graphicsDevice, textureSize.X, textureSize.Y);
        using ( RenderTarget2D renderTarget = new RenderTarget2D(graphicsDevice, textureSize.X, textureSize.Y, false, SurfaceFormat.Color, DepthFormat.None))
        {
            GenerateSingleTexture(this.levelOfDetail, renderTarget);
        }        
    }

    private void GenerateSingleTexture(LevelOfDetail detail, RenderTarget2D renderTarget)
    {
        //  v render the things here v
        this.spriteBatch.Begin();
        this.spriteBatch.Draw(Window.whitePixelTexture, new Rectangle(0,0, textureSize.X, textureSize.Y), clearColor);
        this.renderFunction.Invoke(detail);
        this.spriteBatch.End();
        //  ^ render the things here ^ 

        //  transfer the data from the render target to the texture
        using MemoryStream stream = new MemoryStream();
        renderTarget.SaveAsPng(stream, textureSize.X, textureSize.Y);
        this.texture = Texture2D.FromStream(graphicsDevice, stream);
    }

}