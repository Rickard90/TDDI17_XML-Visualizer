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

    private TopologyHead top; 
	private Canvas canvas;

    private BackButton backButton;
    private HighlightButton highlightButton;
    
    public Window()
    {
		Console.WriteLine("Window constructing");
        this.graphics = new GraphicsDeviceManager(this);
        base.Content.RootDirectory = "Content";
        base.IsMouseVisible = true;
        
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
		Console.WriteLine("Initializing");
        base.Initialize();

        Window.AllowUserResizing = true;
    }

    protected override void LoadContent()
    {
		Console.WriteLine("Loading Content");
        this.spriteBatch = new SpriteBatch(GraphicsDevice);

        this.font = Content.Load<SpriteFont>("Text");
        whitePixelTexture = new Texture2D(base.GraphicsDevice, 1, 1);
        whitePixelTexture.SetData( new Color[] { Color.White });

        this.canvas = new Canvas(base.GraphicsDevice, spriteBatch, Window.ClientBounds.Size)
        {
            renderFunction = this.RenderTopology
        };

        this.top = new TopologyHead("Fake Data Format");
		ComponentFinder.top = this.top;

        this.highlightButton = new HighlightButton(this.top.GetCurrent().GetChildren().First());
        this.backButton = new BackButton(new Rectangle(10, 40, 100, 50), "back");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        Selection.Update();
        if (Selection.LeftMouseJustReleased() && Selection.CursorIsInside(this.backButton.GetRectangle()))
        {
            Console.WriteLine("BACK-BUTTON SELECTED");
            this.top.GoBack();
            this.highlightButton.component = this.top.GetCurrent().GetChildren().First();
        }
        else if (Selection.componentGoRight)
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
        else
        {
            Component currComponent = this.top.GetCurrent();
            bool drawTooltip = false;
            foreach (Component child in currComponent.GetChildren())
            {
                if (Selection.CursorIsInside(Canvas.Camera.ModifiedDrawArea(child.GetRectangle())))
                {
                    if (Selection.LeftMouseJustReleased())
                    {

                        Console.WriteLine("Clicked component: {0} of type {1}", child.GetName(), child.type);
                        if(child.GetInfo() != "")
                        {
                            Console.WriteLine("Clicked component info: " + child.GetName() + " Type: " + child.GetType() + "\n" + child.GetInfo());
                        }
                        //Console.WriteLine("Component children: {0}", child.GetChildren().Count);
                        if(child.GetChildren().Count() > 0) //&& child.type != "Thread")
                        {
                            this.top.Goto(child);
                            this.highlightButton.component = this.top.GetCurrent().GetChildren().First();
                            Console.WriteLine("BREAK");
                        }
                        else
                        {
                            //Console.WriteLine("Lowest level already reached");
                        }
                        break;
                    }
                    else
                    {
                        // Rita tooltip
                        Tooltip.SetTooltip(child, Selection.MouseCursorPosition());
                        drawTooltip = true;
                        break;
                    }
                }
            }
            if (!drawTooltip)
            {
                Tooltip.SetTooltip(null, Selection.MouseCursorPosition());
            }
        }

        canvas.Update(Mouse.GetState(), Keyboard.GetState());

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        //base.GraphicsDevice.Clear(Color.White);
        this.canvas.UpdateTexture();  //  triggers an update every frame, FIX THIS, should only update when something actually change
        this.spriteBatch.Begin();
        this.canvas.Draw();
        //this.top.Draw(this.spriteBatch, this.font);
        this.backButton.Draw(this.spriteBatch, this.font);
        this.highlightButton.Draw(this.spriteBatch);

        //this.RenderTopology();
        this.spriteBatch.End();
        
        base.Draw(gameTime);
    }
	
    //  this is the render function
	private void RenderTopology(Point canvasSize)
    {
        this.top.Draw(this.spriteBatch, this.font, canvasSize.X, canvasSize.Y);
        if(!top.IsHead())
        {
            //this.backButton.Draw(this.spriteBatch, this.font);
        }
    }
}