using System.Globalization;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

class Textbox
{
    private static int selectedTextboxes = 0;
    public static bool AnySelected {get {return selectedTextboxes > 0;}}

    private const int outlinePxSize = 4;
    private const int outlineTextBufferPxSize = 2; 
    private const int windowToOutlineBuffer = 37;
    private const double widthMinimalPropotionPercentage = 0.20;

    private static readonly Color outlineColor = ColorConfiguration.color_3;
    private static readonly Color fillColor = ColorConfiguration.color_0;
    private static readonly char[] invalidFilenameCharacters = Path.GetInvalidPathChars();

    //  return the new textStr
    public delegate string WhenEntered(string ghostStr);
    public delegate string[] WhenChanged(string textStr);

    public WhenEntered whenEntered = null;      //  this event should be set to a method which may load a new topology
    protected WhenChanged whenChanged = null;

    public Rectangle Bounds     {   get { return this.DrawArea;}}
    public Rectangle DrawArea   {   get { return new Rectangle(Position, size);}}
    

    private readonly SpriteFontBase font;

    private string textStr = "";
    private string ghostStr = "";
    private string[] suggestions = new string[0];
    private int suggestion_index = 0;

    private Point windowSize;
    private Point Position {
        get {return new Point((windowSize.X - size.X) / 2, windowToOutlineBuffer);}}
    private Point size;

    public bool IsSelected { 
        get {return this.isSelected;} 
        set {if (value) selectedTextboxes++; else selectedTextboxes--; this.isSelected = value;} 
    }
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
            bool editedTextStr = false;
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

            if (e.Key == Keys.Back)
            {
                if (this.textStr != "")
                {
                    this.textStr = this.textStr.Remove(this.textStr.Length - 1);
                    editedTextStr = true;
                }
            }
            else if (validInput)
            {
                //Console.WriteLine($"Input char : {input}");
                if (this.textStr == "")
                    this.textStr = $"{input}";
                else
                    this.textStr += input;

                editedTextStr = true;
            }
            else if (e.Key == Keys.Enter)
            {
                //Console.WriteLine("Is enter key");
                if (this.whenEntered != null)
                {
                    this.textStr = this.whenEntered.Invoke(this.ghostStr);
                    editedTextStr = true;
                    this.isSelected = false;
                    selectedTextboxes--;
                }
            }
            else
            {
                //Console.WriteLine("Ignored input");
            }


            UpdateGhostStr(editedTextStr);
            
        }
    }

    private void UpdateGhostStr(bool updateSuggestions)
    {

        if (this.whenChanged != null)
        {
            if (updateSuggestions)
            {   //  we need to reset
                this.suggestions = this.whenChanged.Invoke(this.textStr);
                this.suggestion_index = 0;
            }

            if (this.suggestions.Length == 0)
            {
                this.ghostStr = "";
            }
            else
            {
                if (updateSuggestions)
                {
                    for (int i = 0; i < suggestions.Length; i++)
                        if (this.suggestions[i] == this.ghostStr)
                            this.suggestion_index = i;  
                }                 
                this.ghostStr = suggestions[suggestion_index];
            }

        }

        this.needToUpdateTexture = true;
     
    }

    public void InputChangedFunction()
    {
        this.suggestions = ComponentList.GetSuggestions(this.textStr);
    }

    bool pressed_keyup = false, pressed_keydown = false;
    private void Update(KeyboardState keyboardState)
    {

        if (this.isSelected && this.suggestions.Length > 0)
        {
            if (keyboardState.IsKeyDown(Keys.Up) && (pressed_keyup == false))
            {
                this.suggestion_index++;
                this.suggestion_index %= this.suggestions.Length;
                UpdateGhostStr(false);
            }
            else if (keyboardState.IsKeyDown(Keys.Down) && (pressed_keydown == false))
            {
                this.suggestion_index = this.suggestion_index <= 0 ? this.suggestions.Length - 1 : this.suggestion_index - 1;
                // this.suggestion_index--;
                // this.suggestion_index %= this.suggestions.Length;
                UpdateGhostStr(false);                
            }
        }

        pressed_keyup = keyboardState.IsKeyDown(Keys.Up);
        pressed_keydown = keyboardState.IsKeyDown(Keys.Down);


    }

    public void Update(MouseState mouseState, KeyboardState keyboardState)
    {
        this.Update(keyboardState);

        if (this.potentialSelection)
        {
            if (mouseState.LeftButton == ButtonState.Released)
            {
                if (this.Bounds.Contains(mouseState.Position))
                {
                    this.isSelected = true;
                    selectedTextboxes++;
                }
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
                {
                    this.isSelected = false;
                    selectedTextboxes--;
                }
            }
        }

        if (this.needToUpdateTexture)
        {
            this.size = this.CalculateSize();
            this.drawTexture.Dispose();
            this.drawTexture = this.RenderTexture(); 
            this.needToUpdateTexture = false;  
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
        Vector2 drawPosition = new Vector2(renderArea.X + outlineTextBufferPxSize + outlinePxSize, renderArea.Y + outlineTextBufferPxSize + outlinePxSize);
        string bgString = $"{this.textStr} --> {this.ghostStr}";
        Window.spriteBatch.DrawString(this.font, bgString, drawPosition, Color.Black * 0.75f);
        Window.spriteBatch.DrawString(this.font, this.textStr, drawPosition, Color.Black);

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
        else
            effectiveStr = $"{this.textStr} --> {this.ghostStr}";
        Vector2 textSize = this.font.MeasureString(effectiveStr);
        int width = ((int)textSize.X) + 2 * (outlinePxSize + outlineTextBufferPxSize);
        int height = ((int)textSize.Y) + 2 * (outlinePxSize + outlineTextBufferPxSize);

        int minimalWidth = (int)(this.windowSize.X * widthMinimalPropotionPercentage);
        if (minimalWidth > width)
            width = minimalWidth;
        return new Point(width, height);
    }



}