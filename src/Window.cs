using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

//namespace XML_Visualizer;

public class Window : Game
{
    public static Texture2D whitePixelTexture;

    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    private SpriteFont font;
    //private Texture2D tex;
    

    private TopologyHead top = new TopologyHead("test");
    private Canvas canvas;

    public Window()
    {
        this.graphics = new GraphicsDeviceManager(this);
        base.Content.RootDirectory = "Content";
        base.IsMouseVisible = true;

        
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();

        Window.AllowUserResizing = true;
    }

    protected override void LoadContent()
    {
        this.spriteBatch = new SpriteBatch(GraphicsDevice);
        //Component.LoadWhitePixelTexture(GraphicsDevice);

        this.font = Content.Load<SpriteFont>("Text");
        whitePixelTexture = new Texture2D(base.GraphicsDevice, 1, 1);
        whitePixelTexture.SetData( new Color[] { Color.White });

        this.canvas = new Canvas(base.GraphicsDevice, spriteBatch, Window.ClientBounds.Size, LevelOfDetail.Max);
        this.canvas.renderFunction = this.RenderTopology;
        this.canvas.GenerateTextures(new Point(5000, 5000), Color.White);
        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here

        Selection.UpdateMouseInfo();

        if (Selection.LeftMouseJustReleased())
        {
            Console.WriteLine("LEFT MOUSE JUST RELEASED");
            
            Point cursorPosition = Selection.MouseCursorPosition();

            Console.WriteLine("cursor.x = {0}, cursor.y = {1}", cursorPosition.X, cursorPosition.Y);

            Component currComponent = this.top.GetCurrent();

            foreach (Component child in currComponent.GetChildren())
            {
                Rectangle rectangle = child.GetRectangle();

                Console.WriteLine("child pos.x = {0}, child pos.y = {1}, child width = {2}, child height = {3}", rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);

                if (rectangle.X <= cursorPosition.X && cursorPosition.X <= (rectangle.X + rectangle.Width) &&
                    rectangle.Y <= cursorPosition.Y && cursorPosition.Y <= (rectangle.Y + rectangle.Height))
                {
                    this.top.Goto(child);
                    Console.WriteLine("BREAK");
                    break;
                }
            }
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        base.GraphicsDevice.Clear(Color.White);

        this.spriteBatch.Begin();
        this.canvas.Draw();
        this.spriteBatch.End();

        // TODO: Add your drawing code here

        base.Draw(gameTime);
    }

    private void RenderTopology(LevelOfDetail levelOfDetail)
    {
        this.top.Draw(this.spriteBatch, this.font);
    }
}
