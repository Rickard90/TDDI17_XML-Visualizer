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
        //Draws the arrow-body
		sb.Draw(Window.whitePixelTexture, new Rectangle(this.rectangle.Left, this.rectangle.Center.Y - (int)Math.Round((float)this.rectangle.Height/6f), this.rectangle.Width/2, this.rectangle.Height/3), Color.Black);
		
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

        Color color = ColorConfiguration.color_3;
        Rectangle rectangle = Canvas.Camera.ModifiedDrawArea(this.Component.Rectangle);
        int lineThickness = Component.lineThickness;

        //Special case if highlighting a port:
        if(this.Component.type == Component.Type.Port)
        {
            Rectangle otherRectangle = new();
            foreach(var otherPort in this.Component.connections.Keys)
            {
                otherRectangle = Canvas.Camera.ModifiedDrawArea(otherPort.Rectangle);
                Component.DrawArrowBody(sb, rectangle.Center, otherRectangle.Center, 2*lineThickness, ((Port)otherPort).ConnectionOffset, color);
                sb.Draw(Window.whitePixelTexture, otherRectangle, color);
                sb.Draw(Window.whitePixelTexture, new Rectangle(otherRectangle.X + lineThickness, otherRectangle.Y + lineThickness,  otherRectangle.Width- 2*lineThickness, otherRectangle.Height - 2*lineThickness), Color.White);
            }     
            sb.Draw(Window.whitePixelTexture, rectangle, Color.White);
        }

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

    /*---------------------------*/
    /*      HelpButton           */
    /*---------------------------*/

public class HelpButton : Button
{
    private readonly string description;

    public HelpButton(Rectangle rectangle, string description)
        :   base(rectangle)
    {
        this.description = description;
    }
    public void Draw(SpriteBatch sb, SpriteFontBase font, int windowSize)
    {
        Rectangle modifiedArea = this.rectangle;
        modifiedArea.X = windowSize - 110;
        sb.Draw(Window.whitePixelTexture, modifiedArea, Color.Black);
        sb.DrawString(font, this.description, new Vector2(modifiedArea.X + 10, modifiedArea.Y + 10), Color.White);
    }
}    