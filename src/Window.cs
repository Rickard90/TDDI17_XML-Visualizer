using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

//namespace XML_Visualizer;

public class Window : Game
{
    public static Texture2D whitePixelTexture;

    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    private FontSystem fontSystem;

    private TopologyHead top; 
	private Canvas canvas;

    private BackButton backButton;
    private HighlightButton highlightButton;
    private string path;
    private bool updateCanvas = true;
    
    public Window(string path)
    {
        this.path = path;
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

        this.fontSystem = new();
        this.fontSystem.AddFont(File.ReadAllBytes("resource/font/arial.ttf"));

        whitePixelTexture = new Texture2D(base.GraphicsDevice, 1, 1);
        whitePixelTexture.SetData( new Color[] { Color.White });

		TopologyHead.arrowhead = Content.Load<Texture2D>("Arrowhead");

        this.canvas = new Canvas(base.GraphicsDevice, spriteBatch, Window.ClientBounds.Size)
        {
            renderFunction = this.RenderTopology
        };

        this.top = new TopologyHead(path);
		ComponentFinder.top = this.top;

        this.highlightButton = new HighlightButton(this.top.GetCurrent().Children.First());
        this.backButton = new BackButton(new Rectangle(10, 40, 100, 50), "back");

        Tooltip.spriteBatch = this.spriteBatch;
        Tooltip.graphicsDevice = this.GraphicsDevice;
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) {
            Exit();
        }

        Selection.Update();

        Component child = null;
        LinkButton linkButton = null;
        Tooltip.SetTooltip(null, Selection.MouseCursorPosition(), null);

        if ((child = Selection.CursorIsInsideAnyComponent(this.top.GetCurrent().Children)) != null)
        {
            this.updateCanvas = true;
            if (Selection.LeftMouseJustReleased()) {
                if(child.GetInfo() != "") {
                    Console.WriteLine("Clicked component info: " + child.Name + " Type: " + child.GetType() + "\n" + child.GetInfo());
                }
                Console.WriteLine("Component children: {0}", child.Children.Count);
                if(child.type != Component.Type.Thread && child.Children.Count() > 0) {
                    this.top.Goto(child);
                    if (child.Children.Count == 0) {
                        this.highlightButton.Component = null;
                    }
                    else {
                        this.highlightButton.Component = this.top.GetCurrent().Children.First();
                    }
                }
            }
            else {
                Tooltip.SetTooltip(child, Selection.MouseCursorPosition(), fontSystem.GetFont(12));
            }
        }
        else if (Selection.LeftMouseJustReleased() && (linkButton = Selection.CursorIsInsideAnyLinkButton(this.top.GetCurrent().Children)) != null)
        {
            this.updateCanvas = true;
            List<Component> topPath = this.top.GetPath();
            topPath.Clear();
            topPath.Add(linkButton.Component.Parent);
            while (topPath.Last().Parent != null) {
                topPath.Add(topPath.Last().Parent);
            }
            topPath.Reverse();
            this.highlightButton.Component = linkButton.Component;
        }
        else if (Selection.CursorIsInside(Canvas.Camera.ModifiedDrawArea(this.backButton.rectangle)) && Selection.LeftMouseJustReleased())
        {
            this.updateCanvas = true;
            Console.WriteLine("BACK-BUTTON SELECTED");
            this.top.GoBack();
            this.highlightButton.Component = this.top.GetCurrent().Children.First();
        }

        if (Selection.ComponentGoRight)
        {
            updateCanvas = true;
            List<Component> children = this.top.GetCurrent().Children;
            if (this.highlightButton.Component == children.Last()) {
                this.highlightButton.Component = children.First();
            }
            else {
                this.highlightButton.Component = children[children.IndexOf(this.highlightButton.Component) + 1];
            }
        }

        canvas.Update(Mouse.GetState(), Keyboard.GetState());
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        
        if (updateCanvas) {
            this.canvas.UpdateTexture();  // only updated if needed
        }
        updateCanvas = false;
        base.GraphicsDevice.Clear(Color.Black);
        this.spriteBatch.Begin();
        this.canvas.Draw();
        //this.top.Draw(this.spriteBatch, this.font);

        Tooltip.DrawCurrent();
        
        //this.RenderTopology();
        this.spriteBatch.End();
        
        base.Draw(gameTime);
    }
	
    //  this is the render function
	private void RenderTopology(Point canvasSize)
    {
        int fontSize = canvasSize.X/60;
        fontSize = fontSize<8?8:fontSize;
        this.top.Draw(this.fontSystem, this.spriteBatch, this.fontSystem.GetFont(fontSize), canvasSize.X, canvasSize.Y);
        this.highlightButton.Draw(this.spriteBatch);
        if(!top.IsHead()) {
            this.backButton.Draw(this.spriteBatch, this.fontSystem.GetFont(32));
        }
    }
}