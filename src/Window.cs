using System.Dynamic;
using System.Runtime.CompilerServices;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class Window : Game
{
    public static Texture2D whitePixelTexture;
    public static SpriteBatch spriteBatch;
    public static GraphicsDevice graphicsDevice;

    private GraphicsDeviceManager graphics;
    private FontSystem fontSystem;

    private TopologyHead top; 
	private Canvas canvas;

    private BackButton backButton;
    private HighlightButton highlightButton;
    private Textbox enterFolderTextbox;

    private string path;
    private bool updateCanvas = true;

    public Point WindowSize {get; private set;} = new Point(800,400);
    
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
        this.WindowSize = base.Window.ClientBounds.Size;
        Canvas.Camera.offset.X = (Window.ClientBounds.Size.X - canvas.CanvasSize.X) / 2;
        this.enterFolderTextbox.OnResize(WindowSize);
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
        spriteBatch = new SpriteBatch(GraphicsDevice);
        graphicsDevice = this.GraphicsDevice;

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

        Tooltip.spriteBatch = spriteBatch;
        Tooltip.graphicsDevice = this.GraphicsDevice;    

        this.enterFolderTextbox = new Textbox(this.WindowSize, this.fontSystem.GetFont(18));
        this.Window.TextInput += enterFolderTextbox.RegisterTextInput;
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

        if (Selection.GoToLink != -1)
        {
            int offset = Selection.GoToLink - 1;
            Component currComponent = this.highlightButton.Component;
            if (currComponent.linkDrawIndex + offset < currComponent.connections.Count) {
                this.updateCanvas = true;
                List<Component> topPath = this.top.GetPath();
                topPath.Clear();
                //topPath.Add(linkButton.Component.Parent);
                topPath.Add(currComponent.linkButtons[currComponent.linkDrawIndex + offset].Component.Parent);
                while (topPath.Last().Parent != null) {
                    topPath.Add(topPath.Last().Parent);
                }
                topPath.Reverse();
                //this.highlightButton.Component = linkButton.Component;
                this.highlightButton.Component = currComponent.linkButtons[currComponent.linkDrawIndex + offset].Component;
            }
        }
        else if (Selection.linkScroll != Selection.LinkScroll.Nothing)
        {
            Component currComponent = this.highlightButton.Component;
            if (Selection.linkScroll == Selection.LinkScroll.Up) {
                if (currComponent.linkDrawIndex > 0) {
                    currComponent.linkDrawIndex -= 1;
                }
            } else if (Selection.linkScroll == Selection.LinkScroll.Down) {
                if (currComponent.connections.Count - currComponent.linkDrawIndex > Component.numberOfVisibleLinks) {
                    currComponent.linkDrawIndex += 1;
                }
            }
        }
        else if ((child = Selection.CursorIsInsideAnyComponent(this.top.GetCurrent().Children)) != null)
        {
            if (Selection.LeftMouseJustReleased()) {
                this.updateCanvas = true;
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
            // TODO (Mattias): Updating highlightButton shouldn't have to update canvas.
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
        canvas.OffetControl(Window.ClientBounds);
        if (Keyboard.GetState().IsKeyDown(Keys.I) || Keyboard.GetState().IsKeyDown(Keys.O)) {
            updateCanvas = true;
        }

        this.enterFolderTextbox.Update(Mouse.GetState());
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {   
        if (updateCanvas) {
            this.canvas.ReSize(new Point(67*canvas.zoomLevel , (((this.top.NumberOfChildren()-1) / 2 + 1) * 17*canvas.zoomLevel) + 95));
            this.canvas.UpdateTexture();  // only updated if needed
        }
        updateCanvas = false;
        base.GraphicsDevice.Clear(Color.Black);
        spriteBatch.Begin();
        this.canvas.Draw();

        //this.backButton.Draw(spriteBatch, this.fontSystem.GetFont(32));
        //this.highlightButton.Draw(spriteBatch);
        this.enterFolderTextbox.Draw();

        Tooltip.DrawCurrent();

        spriteBatch.End();

        base.Draw(gameTime);
    }
	
    //  this is the render function
	private void RenderTopology(Point canvasSize)
    {
        Console.WriteLine("Number of children " + this.top.NumberOfChildren());
        Console.WriteLine("Number X" + canvasSize.X);
        Console.WriteLine("--------------------");

        this.top.Draw(spriteBatch, this.fontSystem, canvas.zoomLevel);
        this.highlightButton.Draw(spriteBatch);
        if(!top.IsHead()) 
        {
            this.backButton.Draw(spriteBatch, this.fontSystem.GetFont(32));
        }
    }
}