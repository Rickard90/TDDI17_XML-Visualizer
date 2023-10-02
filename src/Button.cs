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