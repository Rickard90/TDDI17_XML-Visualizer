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

    private TopologyHead top = new TopologyHead("Fake Data Format");
	private Canvas canvas;

    private BackButton backButton = new BackButton(new Rectangle(10, 40, 100, 50), "back");
    private HighlightButton highlightButton;
    
    public Window()
    {
        this.graphics = new GraphicsDeviceManager(this);
        base.Content.RootDirectory = "Content";
        base.IsMouseVisible = true;

        this.highlightButton = new HighlightButton(this.top.GetCurrent().GetChildren().First());
        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += this.OnResize;
        Window.AllowAltF4 = true;
    }

    public void OnResize(Object sender, EventArgs e)
    {
        Console.WriteLine($"Window bounds = {base.Window.ClientBounds}");
        this.canvas.WindowSize = base.Window.ClientBounds.Size;
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
		
		this.canvas = new Canvas(base.GraphicsDevice, spriteBatch, Window.ClientBounds.Size);
        this.canvas.renderFunction = this.RenderTopology;
        ComponentFinder.top = this.top;
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        Selection.Update();

        if (Selection.LeftMouseJustReleased())
        {
            Console.WriteLine("LEFT MOUSE JUST RELEASED");
            
            Point cursorPosition = Selection.MouseCursorPosition();

            Console.WriteLine("cursor.x = {0}, cursor.y = {1}", cursorPosition.X, cursorPosition.Y);

            Component currComponent = this.top.GetCurrent();
            Console.WriteLine("Current component: {0}", currComponent.GetName());

            if(Selection.CursorIsInside(this.backButton.GetRectangle()))
            {
                Console.WriteLine("BACK-BUTTON SELECTED");
                this.top.GoBack();
                this.highlightButton.component = this.top.GetCurrent().GetChildren().First();
            }
            else
            {
                foreach (Component child in currComponent.GetChildren())
                {
                    //Console.WriteLine("child pos.x = {0}, child pos.y = {1}, child width = {2}, child height = {3}", rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
                    if(Selection.CursorIsInside(Canvas.Camera.ModifiedDrawArea(child.GetRectangle())))
                    {
                        if (child.GetInfo() != "")
                        {
                            Console.WriteLine("Clicked component info: " + child.GetName() + " Type: " + child.GetType() + "\n" + child.GetInfo());
                        }
                        if(child.GetChildren().Count() > 0)
                        {
                            this.top.Goto(child);
                            this.highlightButton.component = this.top.GetCurrent().GetChildren().First();
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
        }

        if (Selection.componentGoRight)
        {
            List<Component> children = this.top.GetCurrent().GetChildren();
            if (this.highlightButton.component == children.Last())
            {
                this.highlightButton.component = children.First();
            }
            else
            {
                this.highlightButton.component = children[children.IndexOf(this.highlightButton.component) + 1];
            }
        }

        canvas.Update(Mouse.GetState(), Keyboard.GetState());

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        base.GraphicsDevice.Clear(Color.White);

        this.canvas.UpdateTexture();  //  triggers an update every frame, FIX THIS, should only update when something actually change

        base.GraphicsDevice.Clear(Color.Gray);
        this.spriteBatch.Begin();
        this.canvas.Draw();
        //this.top.Draw(this.spriteBatch, this.font);
        this.backButton.Draw(this.spriteBatch, this.font);

        this.highlightButton.Draw(this.spriteBatch);

        this.spriteBatch.End();
        
        base.Draw(gameTime);
    }
	
    //  this is the render function
	private void RenderTopology(Point canvasSize)
    {
        this.top.Draw(this.spriteBatch, this.font);
    }
}