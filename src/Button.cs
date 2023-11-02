using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


public abstract class Button
{
    public Rectangle rectangle;

    protected Button(Rectangle rectangle)
    {
        this.rectangle = rectangle;
    }

    // Idea: Different types of buttons have different actions
    //public abstract void DoAction();
}

/*---------------------------*/
/*      LinkButton           */
/*---------------------------*/

public class LinkButton : Button
{
    public Component Component;

    public LinkButton(Component Component)
        :   base(new())
    {
        this.Component = Component;
    }

    public void Draw(SpriteBatch sb, SpriteFontBase font, Point pos, int height)
    {
        this.rectangle.X = pos.X;
        this.rectangle.Y = pos.Y;
        this.rectangle.Width = 120;
        this.rectangle.Height = height;
        sb.Draw(Window.whitePixelTexture, this.rectangle, Color.Chocolate);
        sb.DrawString(font, this.Component.GetName(), new Vector2(this.rectangle.X, this.rectangle.Y), Color.Black);
    }

}

/*---------------------------*/
/*      BackButton           */
/*---------------------------*/

class BackButton : Button
{
    private readonly string description;

    public BackButton(Rectangle rectangle, string description)
        :   base(rectangle)
    {
        this.description = description;
    }

    public void Draw(SpriteBatch sb, SpriteFontBase font)
    {
        Rectangle modifiedArea = Canvas.Camera.ModifiedDrawArea(this.rectangle);
        sb.Draw(Window.whitePixelTexture, modifiedArea, Color.Black);
        sb.DrawString(font, this.description, new Vector2(modifiedArea.X + 10, modifiedArea.Y + 10), Color.White);  //  consider cacheing drawing fonts
    }
}

/*---------------------------*/
/*      HighlightButton      */
/*---------------------------*/

class HighlightButton
{
    public Component Component;

    public HighlightButton(Component Component)
    {
        this.Component = Component;
    }

    public void Draw(SpriteBatch sb)
    {
        if (this.Component == null)
            return;

        Color color = Color.Red;
        const int lineWidth = 3;

        Rectangle rectangle = Canvas.Camera.ModifiedDrawArea(this.Component.Rectangle);

        // Draw top side
        sb.Draw(Window.whitePixelTexture, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, lineWidth), color);
        // Draw left side
        sb.Draw(Window.whitePixelTexture, new Rectangle(rectangle.X, rectangle.Y, lineWidth, rectangle.Height), color);
        // Draw right side
        sb.Draw(Window.whitePixelTexture, new Rectangle(rectangle.X + rectangle.Width, rectangle.Y, lineWidth, rectangle.Height), color);
        // Draw bottom side
        sb.Draw(Window.whitePixelTexture, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height - lineWidth, rectangle.Width, lineWidth), color);
    }
}