using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

class Textbox
{
    private const int outlinePxSize = 4;
    private const int outlineTextBufferPxSize = 2; 
    private const int windowToOutlineBuffer = 37;
    private const double widthMinimalPropotionPercentage = 0.20;

    private static readonly Color outlineColor = ColorConfiguration.color_3;
    private static readonly Color fillColor = ColorConfiguration.color_0;
    private static readonly char[] invalidFilenameCharacters = Path.GetInvalidPathChars();

    //  return the new textStr
    public delegate string WhenEntered(string textStr);
    public delegate string[] WhenChanged(string text);

    public WhenEntered whenEntered = null;      //  this event should be set to a method which may load a new topology
    protected WhenChanged whenChanged = null;

    public Rectangle Bounds {get {return this.DrawArea;}}
    public Rectangle DrawArea{get { return new Rectangle(Position, size);}}
    

    private readonly SpriteFontBase font;

    private string textStr;
    private string ghostStr = "";
    private string[] suggestions = new string[0];
    private int suggestion_index = 0;

    private Point windowSize;
    private Point Position {
        get {return new Point((windowSize.X - size.X) / 2, windowToOutlineBuffer);}}
    private Point size;

    private bool isSelected = false;
    private bool potentialSelection = false;
    private bool needToUpdateTexture = false;

    private Texture2D drawTexture;



    public Textbox(Point windowSize, SpriteFontBase font, WhenEntered enteredResponse = null, WhenChanged changedResponse = null, string startString = null)
    {
        this.font = font;
        this.whenEntered = enteredResponse;
        this.whenChanged = changedResponse;
        if (startString != null)
            this.textStr = startString;
        else
            this.textStr = "";

        this.windowSize = windowSize;
        this.size = this.CalculateSize();

        //
        //  v fix text texture here v
        //
        this.drawTexture = this.RenderTexture();
        //
        //  ^ --------------------- ^
        //

    }



    public void RegisterTextInput(object sender, TextInputEventArgs e)
    {

        if (this.isSelected)
        {

            bool validInput = true;
            char input = e.Character;
            foreach (char invalidChar in invalidFilenameCharacters)
            {
                if (input == invalidChar)
                {
                    validInput = false;
                    break;
                }

            }

            if (validInput)
            {
                //Console.WriteLine($"Input char : {input}");
                if (this.textStr == "")
                    this.textStr = $"{input}";
                else
                    this.textStr += input;

                this.needToUpdateTexture = true;
            }
            else if (e.Key == Keys.Tab)
            {
                this.textStr = this.ghostStr;           
            }
            else if (e.Key == Keys.Enter)
            {
                //Console.WriteLine("Is enter key");
                if (this.whenEntered != null)
                    this.textStr = this.whenEntered.Invoke(this.textStr);

            }
            else if (e.Key == Keys.Back)
            {
                //Console.WriteLine("Is backspace");
                if (this.textStr != "-")
                {
                    this.textStr = this.textStr.Remove(this.textStr.Length - 1);
                    if (this.textStr == "")
                        this.textStr = "-";
                    this.needToUpdateTexture = true;
                    
                }

            }
            else
            {
                //Console.WriteLine("Ignored input");
            }

            if (this.needToUpdateTexture)
            {
                if (this.whenChanged != null)
                {
                    this.suggestions = this.whenChanged.Invoke(this.textStr);
                    this.suggestion_index = 0;
                    if (this.suggestions.Length == 0)
                        this.ghostStr = "";
                    else
                    {
                        for (int i = 0; i < suggestions.Length; i++)
                            if (this.suggestions[i] == this.ghostStr)
                                this.suggestion_index = i;                   
                        this.ghostStr = suggestions[suggestion_index];
                    }
                }
            }

        }

    }

    public void Update(MouseState mouseState)
    {
        if (this.potentialSelection)
        {
            if (mouseState.LeftButton == ButtonState.Released)
            {
                if (this.Bounds.Contains(mouseState.Position))
                    this.isSelected = true;
                this.potentialSelection = false;
            }
        }
        else
        {
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (this.Bounds.Contains(mouseState.Position))
                    this.potentialSelection = true;
                else
                    this.isSelected = false;
            }
        }

        if (this.needToUpdateTexture)
        {
            this.size = this.CalculateSize();
            this.drawTexture.Dispose();
            this.drawTexture = this.RenderTexture();   
        }        

    }

    public void OnResize(Point windowSize)
    {
        this.windowSize = windowSize;
        this.needToUpdateTexture = true;
    }

    private Texture2D RenderTexture()
    {
        Texture2D result;

        using (RenderTarget2D renderTargetIsAOffScreenBuffer = new (Window.graphicsDevice, this.size.X, this.size.Y, false, SurfaceFormat.Color, DepthFormat.None))
        {

            Window.spriteBatch.Begin();
                Window.graphicsDevice.SetRenderTarget(renderTargetIsAOffScreenBuffer);
                Window.spriteBatch.Draw(Window.whitePixelTexture, new Rectangle(0,0, size.X, size.Y), Color.White);
                //  render v
                    this.Render();
                //  render ^
            Window.spriteBatch.End();
            
            this.drawTexture?.Dispose();
            using (MemoryStream stream = new())
            {
                renderTargetIsAOffScreenBuffer.SaveAsPng(stream, size.X, size.Y);
                result = Texture2D.FromStream(Window.graphicsDevice, stream);
            }

            Window.graphicsDevice.SetRenderTarget(null);
        }  

        return result;      
    }
    //  should only be called in the RenderTexture function
    protected virtual void Render()
    {
        Rectangle renderArea = new Rectangle(Point.Zero, size);

        // Draw outline
        Window.spriteBatch.Draw(Window.whitePixelTexture, renderArea, outlineColor);
        // Draw inner box
        Rectangle copy = renderArea;
        copy.X += outlinePxSize;
        copy.Y += outlinePxSize;
        copy.Width -= outlinePxSize * 2;
        copy.Height -= outlinePxSize * 2;
        Window.spriteBatch.Draw(Window.whitePixelTexture, copy, fillColor);

        // Draw text
        Window.spriteBatch.DrawString(this.font, this.ghostStr, new Vector2(renderArea.X + outlineTextBufferPxSize + outlinePxSize, renderArea.Y + outlineTextBufferPxSize + outlinePxSize), Color.Black * 0.75f);
        Window.spriteBatch.DrawString(this.font, this.textStr, new Vector2(renderArea.X + outlineTextBufferPxSize + outlinePxSize, renderArea.Y + outlineTextBufferPxSize + outlinePxSize), Color.Black);

    }

    public void Draw()
    {
        Window.spriteBatch.Draw(this.drawTexture, this.DrawArea, Color.White);
    }

    private Point CalculateSize()
    {
        string effectiveStr = this.textStr;
        if (effectiveStr == "")
            effectiveStr = "-";
        Vector2 textSize = this.font.MeasureString(effectiveStr);
        int width = ((int)textSize.X) + 2 * (outlinePxSize + outlineTextBufferPxSize);
        int height = ((int)textSize.Y) + 2 * (outlinePxSize + outlineTextBufferPxSize);

        int minimalWidth = (int)(this.windowSize.X * widthMinimalPropotionPercentage);
        if (minimalWidth > width)
            width = minimalWidth;
        return new Point(width, height);
    }



}