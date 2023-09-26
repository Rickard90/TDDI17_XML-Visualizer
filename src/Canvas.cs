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

    public LevelOfDetail levelOfDetail;
    public delegate void RenderTopology(LevelOfDetail levelOfDetail);
    public RenderTopology renderFunction = null;

    private Rectangle copyArea; //  the current area of the canvas that should be drawn, needs to match the ratio of the windowSize
    private Point windowSize;
    private GraphicsDevice graphicsDevice;
    private SpriteBatch spriteBatch;
    private Texture2D[] textures = new Texture2D[0];
    private Point textureSize;
    private Color clearColor;

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
        if (this.textures.Length == 0)
            throw new Exception("Tried to draw canvas without generating textures!");
        if (this.levelOfDetail > LevelOfDetail.Max)
            throw new Exception($"Tried to render invalid level of detail : {this.levelOfDetail}");

        this.spriteBatch.Draw(this.textures[(int)this.levelOfDetail], new Rectangle(0, 0, windowSize.X, windowSize.Y), copyArea, Color.White);

    }

    //  the size of the canvas depends on factors outside of this class, thus most be given
    public void GenerateTextures(Point textureSize, Color clearColor)
    {
        this.clearColor = clearColor;
        this.textureSize = textureSize;

        if (this.renderFunction == null)
            throw new Exception("Canvas has not been given a render function!");

        this.textures = new Texture2D[((int)LevelOfDetail.Max + 1)];

        using ( RenderTarget2D renderTarget = new RenderTarget2D(graphicsDevice, textureSize.X, textureSize.Y, false, SurfaceFormat.Color, DepthFormat.None))
        {
            graphicsDevice.SetRenderTarget(renderTarget);
            for (LevelOfDetail detail = LevelOfDetail.Min; detail <= LevelOfDetail.Max; detail++) 
            {
                GenerateSingleTexture(textureSize, detail, renderTarget);
            }
            graphicsDevice.SetRenderTarget(null);   //  give back the rendering target
        }
    }

    public void UpdateTexture(LevelOfDetail detail)
    {
        using ( RenderTarget2D renderTarget = new RenderTarget2D(graphicsDevice, textureSize.X, textureSize.Y, false, SurfaceFormat.Color, DepthFormat.None))
        {
            GenerateSingleTexture(textureSize, detail, renderTarget);
        }        
    }

    private void GenerateSingleTexture(Point textureSize, LevelOfDetail detail, RenderTarget2D renderTarget)
    {
        //  v render the things here v
        this.spriteBatch.Begin();
        this.spriteBatch.Draw(Window.whitePixelTexture, new Rectangle(0,0,textureSize.X, textureSize.Y), clearColor);
        this.renderFunction.Invoke(detail);
        this.spriteBatch.End();
        //  ^ render the things here ^ 

        //  transfer the data from the render target to the texture
        using MemoryStream stream = new MemoryStream();
        renderTarget.SaveAsPng(stream, textureSize.X, textureSize.Y);
        textures[(int)detail] = Texture2D.FromStream(graphicsDevice, stream);
    }

}