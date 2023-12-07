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

    private FontSystem fontSystem;

    private TopologyHead top; 
	private Canvas canvas;

    private BackButton backButton;
    private HighlightButton highlightButton;
    private HelpButton helpButton;
    private Textbox enterFolderTextbox;

    private string folderPath;
    private bool updateCanvas;

    public Point windowSize; 
    
    public Window(string folderPath)
    {
        this.folderPath = folderPath;
        _ = new GraphicsDeviceManager(this);
        base.Content.RootDirectory = "Content";
        base.IsMouseVisible = true;
        this.updateCanvas = true;
        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += this.OnResize;
        Window.AllowAltF4 = true;
        windowSize = Window.ClientBounds.Size;
    }

    public void OnResize(Object sender, EventArgs e)
    {
        if (top.NumberOfColums(Window.ClientBounds.Width, canvas.zoomLevel) != top.NumberOfColums(windowSize.X, canvas.zoomLevel))
            updateCanvas = true;
        this.windowSize = base.Window.ClientBounds.Size;
        Canvas.Camera.offset.X = (Window.ClientBounds.Size.X - canvas.CanvasSize.X) / 2;
        this.enterFolderTextbox.OnResize(windowSize);
        helpButton.UpdatePosition(windowSize);
    }

    protected override void LoadContent()
    {
		Log.Print("Loading Content");
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
        this.top = new TopologyHead(folderPath);


        this.highlightButton = new HighlightButton(this.top.GetCurrent().Children.First());
        this.backButton = new BackButton(new Rectangle(10, 10, 100, 50), "back");
        this.helpButton = new HelpButton(new Rectangle ( windowSize.X - 110, 10, 100, 50), "(H)elp");
        Tooltip.spriteBatch = spriteBatch;
        Tooltip.graphicsDevice = this.GraphicsDevice;    

        this.enterFolderTextbox = new Textbox(this.windowSize, this.fontSystem.GetFont(18), ComponentFinder.GoToComponentWithName, ComponentList.GetSuggestions, null);
        this.Window.TextInput += enterFolderTextbox.RegisterTextInput;

        // Fill the global component list, must be done only after reading and constructing the topology.
        ComponentList.Construct(this.top);
        ComponentList.Sort();
        //ComponentList.Print();
        ComponentFinder.Construct(this.top);
        ComponentFinder.Print();
    }

    protected override void Update(GameTime gameTime)
    {
        this.HandleSelection();
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {   
        if (updateCanvas)
        {
            int numberOfRows = (top.NumberOfChildren()-1) / top.NumberOfColums(windowSize.X, canvas.zoomLevel) + 1;
            int numberOfColums = top.NumberOfColums(windowSize.X, canvas.zoomLevel);
            UpdateCanvasSize(numberOfColums, numberOfRows);
            Canvas.Camera.ControlOffset(canvas.CanvasSize, Window.ClientBounds);
            canvas.ScrollCanvasToArea(highlightButton.GetArea(), Window.ClientBounds);
        }
        base.GraphicsDevice.Clear(Color.White);
        spriteBatch.Begin();
        this.canvas.Draw();

        this.highlightButton.Draw(spriteBatch);
        spriteBatch.Draw(whitePixelTexture, new Rectangle(0, 0, Window.ClientBounds.Size.X, Constants.ToolbarHeight), new Color(230, 230, 230, 255));
        spriteBatch.Draw(whitePixelTexture, new Rectangle(0, Constants.ToolbarHeight-3, Window.ClientBounds.Size.X, 3), Color.Gray);
        this.backButton.Draw(spriteBatch, this.fontSystem.GetFont(32));
        this.helpButton.Draw(spriteBatch, this.fontSystem.GetFont(32), windowSize.X);
        this.top.DrawPath(spriteBatch, this.fontSystem.GetFont(22));
        
        this.enterFolderTextbox.Draw();
        Tooltip.DrawCurrent();
        spriteBatch.End();

        base.Draw(gameTime);
        updateCanvas = false;
    }
	
    //  this is the render function
	private void RenderTopology(Point canvasSize)
    {
        this.top.Draw(spriteBatch, this.fontSystem, canvas.zoomLevel, canvasSize.X);
    }

    private void UpdateCanvasSize(int numberOfColums, int numberOfRows) {
        int canvasHeight;
        int canvasWidth;
        if(this.top.GetCurrent().type != Component.Type.Thread)
        {
            canvasHeight = ((numberOfRows * Constants.ComponentSize + Constants.Spacing)*canvas.zoomLevel/8) + Constants.ToolbarHeight;
            canvasWidth = (numberOfColums*(8*Constants.Spacing + Constants.ComponentSize) - 3*Constants.Spacing)*canvas.zoomLevel/12;
        }
        else //this.top.GetCurrent().type == Component.Type.Thread need rework
        {
            canvasHeight = 8*Constants.ComponentSize*canvas.zoomLevel/12 + Constants.ToolbarHeight;
            canvasWidth = 8*Constants.ComponentSize*canvas.zoomLevel/12 + Constants.ToolbarHeight;
        }
        this.canvas.ReSize(new Point(canvasWidth, canvasHeight));
        this.canvas.UpdateTexture();
    }

    private void HandleSelection()
    {
        Selection.Update();

        Component child = null;
        LinkButton linkButton = null;
        Tooltip.SetTooltip(null, Selection.MouseCursorPosition(), null);

        if (Selection.GoToLink != -1)
        {
            int offset = Selection.GoToLink - 1;
            Component highlightedComponent = this.highlightButton.Component;
            int selectedIndex = highlightedComponent.linkDrawIndex + offset;
            if (selectedIndex < highlightedComponent.connections.Count) {
                this.updateCanvas = true;
                this.top.GoToAny(highlightedComponent.linkButtons[selectedIndex].Component, this.highlightButton);
            }
        }
        else if (Selection.linkScroll != Selection.LinkScroll.Nothing)
        {
            this.updateCanvas = true;
            this.highlightButton.Component.UpdateLinkDrawIndex();
        }
        else if ((child = Selection.CursorIsInsideAnyComponent(this.top.GetCurrent().Children)) != null
            && Selection.CursorIsInside(new Rectangle (0, Constants.ToolbarHeight, Window.ClientBounds.Width, Window.ClientBounds.Height)))
        {
            if (Selection.LeftMouseJustReleased()) {
                this.updateCanvas = true;
                this.top.GoToChild(child, this.highlightButton);
            }
            else {
				Tooltip.SetTooltip(child, Canvas.Camera.ModifiedPosition(new Point(child.Rectangle.Right - Component.lineThickness, child.Rectangle.Top)), fontSystem.GetFont(14));
            }
        }
        else if(Selection.CursorIsInside(helpButton.rectangle) && Selection.LeftMouseJustReleased())
        {
            using StreamReader topologyReader = new("help.txt");
            string line = topologyReader.ReadLine();
            while (line != null)
            {
                Console.WriteLine(line);
                line = topologyReader.ReadLine();
            }
            
        }
        else if (Selection.LeftMouseJustReleased() && (linkButton = Selection.CursorIsInsideAnyLinkButton(this.top.GetCurrent().Children)) != null
            && Selection.CursorIsInside(new Rectangle (0, Constants.ToolbarHeight, Window.ClientBounds.Width, Window.ClientBounds.Height)))
        {
            this.updateCanvas = true;
            this.top.GoToAny(linkButton.Component, this.highlightButton);
            canvas.ScrollCanvasToArea(highlightButton.GetArea(), Window.ClientBounds);
        }
        else if (ComponentFinder.componentToGoTo != null)
        {
            this.updateCanvas = true;
            this.top.GoToAny(ComponentFinder.componentToGoTo, this.highlightButton);
            canvas.ScrollCanvasToArea(highlightButton.GetArea(), Window.ClientBounds);
            ComponentFinder.componentToGoTo = null;
        }
        else if (Selection.CursorIsInside(backButton.rectangle) && Selection.LeftMouseJustReleased())
        {
            this.updateCanvas = true;
            this.top.GoBack();
            this.highlightButton.Component = this.top.GetCurrent().Children.First();
            canvas.ScrollCanvasToArea(highlightButton.GetArea(), Window.ClientBounds);
        }


        if (Selection.ComponentGoRight)
        {
            this.highlightButton.GoRight(this.top.GetCurrent().Children);
            canvas.ScrollCanvasToArea(highlightButton.GetArea(), Window.ClientBounds);
        } 
        else if (Selection.ComponentGoLeft)
        {
            this.highlightButton.GoLeft(this.top.GetCurrent().Children);
            canvas.ScrollCanvasToArea(highlightButton.GetArea(), Window.ClientBounds);
        }

        
        if (Selection.ZoomChange != Selection.CanvasZoomChange.Nothing) {
            canvas.Update(Selection.ZoomChange, Window.ClientBounds);
            updateCanvas = true;
        }

        if (Selection.ScrollChange != Selection.CanvasScroll.Nothing) {
            Canvas.Camera.Update(Selection.ScrollChange, canvas.CanvasSize, Window.ClientBounds);
        }

        if (Selection.PrtSc) {    
            int numberOfColums = top.NumberOfColums(windowSize.X, canvas.zoomLevel);
            int numberOfRows = (top.NumberOfChildren()-1) / top.NumberOfColums(windowSize.X, canvas.zoomLevel) + 1;
        
            int currentZoom = canvas.zoomLevel;
            canvas.zoomLevel = Constants.screenshotZoom;

            UpdateCanvasSize(numberOfColums, numberOfRows);

            if (!Directory.Exists("screenshots"))
            {
                Directory.CreateDirectory("screenshots");
            }
            canvas.SaveAsPng("screenshots/screenshot " + DateTime.Now.ToString("yyyy-MM-dd ") + DateTime.Now.ToString("[HH;mm;ss]")+ ".png");
            Selection.PrtSc = false;

            canvas.zoomLevel = currentZoom;
            UpdateCanvasSize(numberOfColums, numberOfRows);
        }

        this.enterFolderTextbox.Update(Mouse.GetState(), Keyboard.GetState());
    }
}