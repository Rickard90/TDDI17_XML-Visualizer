using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


class Button
{
    private Rectangle rectangle;
    private String description;

    public Button(Rectangle rectangle, String description)
    {
        this.rectangle = rectangle;
        this.description = description;
    }

    public Rectangle GetRectangle()
	{
		return this.rectangle;
	}

    public void Draw(SpriteBatch sb, SpriteFont font)
    {
        sb.Draw(Window.whitePixelTexture, this.rectangle, Color.Black);
        sb.DrawString(font, this.description, new Vector2(this.rectangle.X + 10, this.rectangle.Y + 10), Color.White);
    }
}