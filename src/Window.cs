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

    private Button buttonBack = new Button(new Rectangle(10, 40, 100, 50), "back");
    
    private TopologyHead top; 
	private Canvas canvas;

    public Window()
    {
		Console.WriteLine("Window constructing");
        this.graphics = new GraphicsDeviceManager(this);
        base.Content.RootDirectory = "Content";
        base.IsMouseVisible = true;
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
		
		this.canvas = new Canvas(base.GraphicsDevice, spriteBatch, Window.ClientBounds.Size);
        this.canvas.renderFunction = this.RenderTopology;
        
		this.top = new TopologyHead("Fake Data Format");
		ComponentFinder.top = this.top;
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        Selection.UpdateMouseInfo();

        if (Selection.LeftMouseJustReleased())
        {
            Point cursorPosition = Selection.MouseCursorPosition();
            Component currComponent = this.top.GetCurrent();

            if(Selection.CursorIsInside(this.buttonBack.GetRectangle()))
            {
                Console.WriteLine("BACK-BUTTON SELECTED");
                this.top.GoBack();
            }
            else
            {
                foreach (Component child in currComponent.GetChildren())
                {
                    if(Selection.CursorIsInside(Canvas.Camera.ModifiedDrawArea(child.GetRectangle())))
                    {
						Console.WriteLine("Clicked component: {0} of type {1}", child.GetName(), child.type);
                        if(child.GetInfo() != "")
                        {
                            Console.WriteLine("Clicked component info: \n {0}", child.GetInfo());
                        }
						Console.WriteLine("Component children: {0}", child.GetChildren().Count);
						
						if(child.GetChildren().Count()	> 0) //&& child.type != "Thread")
                        {
                            this.top.Goto(child);
						}
                        else
                        {
                            //Console.WriteLine("Lowest level already reached");
                        }
                        break;
                    }
                }
            }
        }

        canvas.Update(Mouse.GetState(), Keyboard.GetState());

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        base.GraphicsDevice.Clear(Color.White);

        this.canvas.UpdateTexture();  //  triggers an update every frame, FIX THIS, should only update when something actually change

        //base.GraphicsDevice.Clear(Color.Gray);
        this.spriteBatch.Begin();
        this.canvas.Draw();
        //this.RenderTopology();
        this.spriteBatch.End();
        
        base.Draw(gameTime);
    }
	
    //  this is the render function
	private void RenderTopology()
    {
        this.top.Draw(this.spriteBatch, this.font, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
        if(!top.IsHead())
        {
            this.buttonBack.Draw(this.spriteBatch, this.font);
        }
    }
}