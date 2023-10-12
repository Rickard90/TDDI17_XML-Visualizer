using System.Globalization;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


abstract class Button
{
    protected Rectangle rectangle;

    protected Button(Rectangle rectangle)
    {
        this.rectangle = rectangle;
    }

    public Rectangle GetRectangle()
	{
		return this.rectangle;
	}

    public Rectangle SetRectangle(Rectangle rectangle)
	{
		return this.rectangle = rectangle;
	}

    // Idea: Different types of buttons have different actions
    //public abstract void DoAction();
}

class HighlightButton
{
    public Component component;

    public HighlightButton(Component component)
    {
        this.component = component;
    }

    public void Draw(SpriteBatch sb)
    {
        if (this.component == null)
        {
            return;
        }

        Color color = Color.Red;
        int lineWidth = 3;

        Rectangle rectangle = Canvas.Camera.ModifiedDrawArea(this.component.GetRectangle());

        // Draw top side
        sb.Draw(Window.whitePixelTexture, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, lineWidth), color);
        // Draw left side
        sb.Draw(Window.whitePixelTexture, new Rectangle(rectangle.X, rectangle.Y, lineWidth, rectangle.Height), color);
        // Draw right side
        sb.Draw(Window.whitePixelTexture, new Rectangle(rectangle.X + rectangle.Width, rectangle.Y, lineWidth, rectangle.Height), color);
        // Draw bottom side
        sb.Draw(Window.whitePixelTexture, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height, rectangle.Width, lineWidth), color);
    }
}

/*
class ComponentButton : Button
{
    // Reference to a component
    private Component component;

    public ComponentButton(Rectangle rectangle, Component component)
    :   base(rectangle)
    {
        this.component = component;
    }
}
*/

class BackButton : Button
{
    private string description;

    public BackButton(Rectangle rectangle, string description)
        :   base(rectangle)
    {
        this.description = description;
    }

    public void Draw(SpriteBatch sb, SpriteFontBase font)
    {
        Rectangle modifiedArea = Canvas.Camera.ModifiedDrawArea(this.rectangle);
        sb.Draw(Window.whitePixelTexture, modifiedArea, Color.Black);
        sb.DrawString(font, this.description, new Vector2(modifiedArea.X + 10, modifiedArea.Y + 10), Color.White);
    }
}