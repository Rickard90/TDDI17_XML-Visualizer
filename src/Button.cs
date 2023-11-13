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

    public void Draw(SpriteBatch sb, SpriteFontBase font, Point pos, int height, int width)
    {
        this.rectangle.X = pos.X + width;
        this.rectangle.Y = pos.Y;
        this.rectangle.Width = width/3;
        this.rectangle.Height = height;

        //Draws the arrow-body
        //sb.Draw(Window.whitePixelTexture, new Rectangle(this.rectangle.X, this.rectangle.Y, width/4, height/2), Color.Black);
        //This draws an arrowhead, OBS: the rotation is by radians and Vector2.Zero denotes the point around which you rotate
        //sb.Draw(TopologyHead.arrowhead, new Rectangle(this.rectangle.X, this.rectangle.Y, width/3, height), Color.White);
        sb.Draw(TopologyHead.arrowhead, this.rectangle, Color.White);

        Vector2 size = font.MeasureString(this.Component.Name);
        if (size.X < width)
        {
            sb.DrawString(font, this.Component.Name, new Vector2(pos.X, pos.Y), Color.Black);
            return;
        }

        string newName = this.Component.Name + "...";
        size = font.MeasureString(newName);
        float excess;
        int reduceBy;
        do
        {
            excess = (size.X - width) / font.FontSize;
            reduceBy = Math.Max(1, (int)excess) + "...".Length;
            newName = newName[..^reduceBy] + "...";
            size = font.MeasureString(newName);
        }
        while (size.X > width);

        sb.DrawString(font, newName, new Vector2(pos.X, pos.Y), Color.Black);
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
        //Rectangle modifiedArea = Canvas.Camera.ModifiedDrawArea(this.rectangle);
        Rectangle modifiedArea = this.rectangle;
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

    public Rectangle GetArea() 
    {
        return Component.Rectangle;;
    }
    public void Draw(SpriteBatch sb)
    {
        if (this.Component == null)
            return;

        Color color = Color.Red;
        Rectangle rectangle = Canvas.Camera.ModifiedDrawArea(this.Component.Rectangle);
        int lineThickness = Component.LineThickness;

        // Draw top side
        sb.Draw(Window.whitePixelTexture, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, lineThickness), color);
        // Draw left side
        sb.Draw(Window.whitePixelTexture, new Rectangle(rectangle.X, rectangle.Y, lineThickness, rectangle.Height), color);
        // Draw right side
        sb.Draw(Window.whitePixelTexture, new Rectangle(rectangle.X + rectangle.Width - lineThickness, rectangle.Y, lineThickness, rectangle.Height), color);
        // Draw bottom side
        sb.Draw(Window.whitePixelTexture, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height - lineThickness, rectangle.Width, lineThickness), color);
    }

    public void GoRight(List<Component> components)
    {
        if (this.Component == components.Last())
            this.Component = components.First();
        else
            this.Component = components[components.IndexOf(this.Component) + 1];
    }

    public void GoLeft(List<Component> components)
    {
        if (this.Component == components.First())
            this.Component = components.Last();
        else
            this.Component = components[components.IndexOf(this.Component) - 1];
    }
}