using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

class Window : Game
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
	
	private LinkButton hoveredLinkButton = null;

    //private string folderPath;
    private bool updateCanvas;

    public Point windowSize; 
    
    public Window(TopologyHead topologyHead)
    {
        this.top = topologyHead;
        //this.folderPath = folderPath;
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
        spriteBatch = new SpriteBatch(GraphicsDevice);
        graphicsDevice = this.GraphicsDevice;
        Tooltip.spriteBatch = spriteBatch;
        Tooltip.graphicsDevice = this.GraphicsDevice;

        this.fontSystem = new();
        this.fontSystem.AddFont(File.ReadAllBytes("resource/font/arial.ttf"));

        whitePixelTexture = new Texture2D(base.GraphicsDevice, 1, 1);
        whitePixelTexture.SetData( new Color[] { Color.White });

		TopologyHead.arrowhead = Content.Load<Texture2D>("Arrowhead");

        this.canvas = new Canvas(base.GraphicsDevice, spriteBatch, Window.ClientBounds.Size)
        {
            renderFunction = this.RenderTopology
        };

        this.highlightButton = new HighlightButton(this.top.GetCurrent().Children.First());
        this.backButton = new BackButton(new Rectangle(10, 10, 100, 50), "back");

        this.helpButton = new HelpButton(new Rectangle ( windowSize.X - 110, 10, 100, 50), "(H)elp", this.fontSystem.GetFont(5+canvas.zoomLevel), this.windowSize);

        this.enterFolderTextbox = new Textbox(this.windowSize, this.fontSystem.GetFont(18), ComponentFinder.GoToComponentWithName, ComponentList.GetSuggestions, null);
        this.Window.TextInput += enterFolderTextbox.RegisterTextInput;

        // Fill the global component list, must be done only after reading and constructing the topology.
        ComponentList.Construct(this.top);
        ComponentList.Sort();
        ComponentFinder.Construct(this.top);
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
        spriteBatch.Draw(whitePixelTexture, new Rectangle(0, 0, Window.ClientBounds.Size.X, Constants.toolbarHeight), new Color(230, 230, 230, 255));
        spriteBatch.Draw(whitePixelTexture, new Rectangle(0, Constants.toolbarHeight-3, Window.ClientBounds.Size.X, 3), Color.Gray);
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
            canvasHeight = ((numberOfRows * Constants.componentSize + Constants.spacing)*canvas.zoomLevel/8) + Constants.toolbarHeight;
            canvasWidth = (numberOfColums*(8*Constants.spacing + Constants.componentSize) - 3*Constants.spacing)*canvas.zoomLevel/Constants.defaultZoom;
        }
        else //this.top.GetCurrent().type == Component.Type.Thread need rework
        {
            canvasHeight = 8*Constants.componentSize*canvas.zoomLevel/Constants.defaultZoom + Constants.toolbarHeight;
            canvasWidth = 8*Constants.componentSize*canvas.zoomLevel/Constants.defaultZoom + Constants.toolbarHeight;
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

        // Check if we should go to a connected/linked component
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
        // Else check if we should go to arbitrary component (this is triggered by the textbox)
        else if (ComponentFinder.componentToGoTo != null)
        {
            this.updateCanvas = true;
            this.top.GoToAny(ComponentFinder.componentToGoTo, this.highlightButton);
            canvas.ScrollCanvasToArea(highlightButton.GetArea(), Window.ClientBounds);
            ComponentFinder.componentToGoTo = null;
        }
        // Else check if we should scroll through our connections/links
        else if (Selection.linkScroll != Selection.LinkScroll.Nothing)
        {
            this.updateCanvas = true;
            this.highlightButton.Component.UpdateLinkDrawIndex();
        }
        //Else if the mouse is clicked and we are inside the window
        else if(Selection.LeftMouseJustReleased() 
				&& Selection.CursorIsInside(new Rectangle (0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height)))
        {
            //If a component was clicked
            if ((child = Selection.CursorIsInsideAnyComponent(this.top.GetCurrent().TooltipList())) != null)
			{
                this.updateCanvas = true;
                if (this.top.GetCurrent().type == Component.Type.Thread) {
                    this.top.GoToAny(child, this.highlightButton);
                } else {
                    this.top.GoToChild(child, this.highlightButton);
                }
            }
            //Else check if we clicked on a linkbutton
            else if ((linkButton = Selection.CursorIsInsideAnyLinkButton(this.top.GetCurrent().Children)) != null)
			{
                this.updateCanvas = true;
                this.top.GoToAny(linkButton.Component, this.highlightButton);
                canvas.ScrollCanvasToArea(highlightButton.GetArea(), Window.ClientBounds);
            }
            //Else check if it clicked the help button
            else if(Selection.CursorIsInside(helpButton.rectangle))
            {
                this.helpButton.isPressed = !this.helpButton.isPressed; 
            }
             // Else check if we clicked on the back button
            else if (Selection.CursorIsInside(backButton.rectangle))
            {
                this.updateCanvas = true;
                this.top.GoBack();
                this.highlightButton.Component = this.top.GetCurrent().Children.First();
                canvas.ScrollCanvasToArea(highlightButton.GetArea(), Window.ClientBounds);
            }
        }
        // Else check if any tooltips/highlights should be shown
        else 
        {
            //Check if the mouse is hovering a component
            if ((child = Selection.CursorIsInsideAnyComponent(this.top.GetCurrent().TooltipList())) != null )
            {
				Tooltip.SetTooltip(child, Canvas.Camera.ModifiedPosition(new Point(child.Rectangle.Right - Component.lineThickness, child.Rectangle.Top)), fontSystem.GetFont(14));
            }
            //Else check if we are hovering a linkbutton
            else if ((linkButton = Selection.CursorIsInsideAnyLinkButton(this.top.GetCurrent().Children)) != null)
			{
				if (hoveredLinkButton != linkButton)
				{
					if (hoveredLinkButton != null)
					{
						hoveredLinkButton.Highlight = false;
					}
					this.updateCanvas = true;
					hoveredLinkButton = linkButton;
					linkButton.Highlight = true;
				}
            }
			//Check if we just stopped hovering a linkbutton
            if (hoveredLinkButton != null && linkButton == null)
            {
				this.updateCanvas = true;
                hoveredLinkButton.Highlight = false;
				hoveredLinkButton = null;
            }
        }

        if (Selection.ComponentEnter && ComponentFinder.componentToGoTo == null)
        {
            this.updateCanvas = true;
            this.top.GoToChild(this.highlightButton.Component, this.highlightButton);
            canvas.ScrollCanvasToArea(highlightButton.GetArea(), Window.ClientBounds);
        }
        else if (Selection.ComponentGoBack)
        {
            this.updateCanvas = true;
            this.top.GoBack();
            this.highlightButton.Component = this.top.GetCurrent().Children.First();
            canvas.ScrollCanvasToArea(highlightButton.GetArea(), Window.ClientBounds);
        }
        else if (Selection.ComponentGoUp)
        {
            this.highlightButton.GoUp(this.top.GetCurrent().Children, this.top.NumberOfColums(windowSize.X, canvas.zoomLevel));
            canvas.ScrollCanvasToArea(highlightButton.GetArea(), Window.ClientBounds);
        }
        else if (Selection.ComponentGoDown)
        {
            this.highlightButton.GoDown(this.top.GetCurrent().Children, this.top.NumberOfColums(windowSize.X, canvas.zoomLevel));
            canvas.ScrollCanvasToArea(highlightButton.GetArea(), Window.ClientBounds);
        }
        else if (Selection.ComponentGoLeft)
        {
            this.highlightButton.GoLeft(this.top.GetCurrent().Children, this.top.NumberOfColums(windowSize.X, canvas.zoomLevel));
            canvas.ScrollCanvasToArea(highlightButton.GetArea(), Window.ClientBounds);
        }
        else if (Selection.ComponentGoRight)
        {
            this.highlightButton.GoRight(this.top.GetCurrent().Children, this.top.NumberOfColums(windowSize.X, canvas.zoomLevel));
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
            canvas.SaveAsPng("screenshots/screenshot_" + DateTime.Now.ToString("yyyy-MM-dd_") + DateTime.Now.ToString("HH-mm-ss")+ ".png");
            Selection.PrtSc = false;

            canvas.zoomLevel = currentZoom;
            UpdateCanvasSize(numberOfColums, numberOfRows);
        }

        this.enterFolderTextbox.Update(Mouse.GetState(), Keyboard.GetState());
    }
}