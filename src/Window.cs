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
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        Selection.UpdateMouseInfo();

        if (Selection.LeftMouseJustReleased())
        {
            Console.WriteLine("LEFT MOUSE JUST RELEASED");
            
            Point cursorPosition = Selection.MouseCursorPosition();

            Console.WriteLine("cursor.x = {0}, cursor.y = {1}", cursorPosition.X, cursorPosition.Y);

            Component currComponent = this.top.GetCurrent();
            Console.WriteLine("Current component: {0}", currComponent.GetName());
            foreach (Component child in currComponent.GetChildren())
            {
                Rectangle rectangle = child.GetRectangle();

                Console.WriteLine("child pos.x = {0}, child pos.y = {1}, child width = {2}, child height = {3}", rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);

                if(cursorPosition.X >= rectangle.X && cursorPosition.X <= (rectangle.X + rectangle.Width )  
				&& cursorPosition.Y >= rectangle.Y && cursorPosition.Y <= (rectangle.Y + rectangle.Height))
				{
					if(child.GetInfo() != "")
					{
						Console.WriteLine("Clicked component info: \n {0}", child.GetInfo());
					}
					if(child.GetChildren().Count()	> 0)
					{
						this.top.Goto(child);
						Console.WriteLine("BREAK");
					}
					else
					{
						Console.WriteLine("Lowest level already reached");
					}
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
        this.top.Draw(this.spriteBatch, this.font);
        this.spriteBatch.End();


        base.Draw(gameTime);
    }
	
	private void RenderTopology(LevelOfDetail levelOfDetail)
    {
        this.top.Draw(this.spriteBatch, this.font);
    }
}