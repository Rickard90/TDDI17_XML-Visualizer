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

    // Idea: Different types of buttons have different actions
    //public abstract void DoAction();
}

class HighlightButton : Button
{
    private Component component;

    public HighlightButton(Component component)
    :   base(component.GetRectangle())
    {
        this.component = component;
    }

    public void Draw(SpriteBatch sb)
    {
        Color color = Color.Red;
        int lineWidth = 3;

        Rectangle copy = new Rectangle(base.rectangle.X, base.rectangle.Y, base.rectangle.Width, base.rectangle.Height);
        base.rectangle = Canvas.Camera.ModifiedDrawArea(base.rectangle);

        // Draw top side
        sb.Draw(Window.whitePixelTexture, new Rectangle(base.rectangle.X, base.rectangle.Y, base.rectangle.Width, lineWidth), color);
        // Draw left side
        sb.Draw(Window.whitePixelTexture, new Rectangle(base.rectangle.X, base.rectangle.Y, lineWidth, base.rectangle.Height), color);
        // Draw right side
        sb.Draw(Window.whitePixelTexture, new Rectangle(base.rectangle.X + base.rectangle.Width, base.rectangle.Y, lineWidth, base.rectangle.Height), color);
        // Draw bottom side
        sb.Draw(Window.whitePixelTexture, new Rectangle(base.rectangle.X, base.rectangle.Y + base.rectangle.Height, base.rectangle.Width, lineWidth), color);

        base.rectangle = new Rectangle(copy.X, copy.Y, copy.Width, copy.Height);
    }

    public void SetComponent(Component component)
    {
        this.component = component;
    }

    public Component GetComponent()
    {
        return this.component;
    }
}

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

class BackButton : Button
{
    private string description;

    public BackButton(Rectangle rectangle, string description)
        :   base(rectangle)
    {
        this.description = description;
    }

    public void Draw(SpriteBatch sb, SpriteFont font)
    {
        sb.Draw(Window.whitePixelTexture, this.rectangle, Color.Black);
        sb.DrawString(font, this.description, new Vector2(this.rectangle.X + 10, this.rectangle.Y + 10), Color.White);
    }
}