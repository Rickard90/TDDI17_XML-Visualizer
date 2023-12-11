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
	public Boolean Highlight = false;

    public LinkButton(Component Component)
        :   base(new())
    {
        this.Component = Component;
    }

    public void Draw(SpriteBatch sb, SpriteFontBase font, Point pos, int height, int width)
    {
        this.rectangle.X = pos.X;
        this.rectangle.Y = pos.Y;
        this.rectangle.Width = width;
        this.rectangle.Height = height;
		
		if(Highlight)
		{
			DrawHighlight(sb, font);
		}
		else
		{		
			DrawDefault(sb, font);
		}
		DrawArrow(sb, width);

    }
	private void DrawDefault(SpriteBatch sb, SpriteFontBase font)
	{
        Vector2 size = font.MeasureString(this.Component.Name);
        if (size.X < this.rectangle.Width)
        {
            sb.DrawString(font, this.Component.Name, new Vector2(this.rectangle.X, this.rectangle.Y), Color.Black);
            return;
        }
        string newName = this.Component.Name + "...";
        size = font.MeasureString(newName);
        float excess;
        int reduceBy;
        do
        {
            excess = (size.X - this.rectangle.Width) / font.FontSize;
            reduceBy = Math.Max(1, (int)excess) + "...".Length;
            newName = newName[..^reduceBy] + "...";
            size = font.MeasureString(newName);
        }
        while (size.X > this.rectangle.Width);

        sb.DrawString(font, newName, new Vector2(this.rectangle.X, this.rectangle.Y), Color.Black);
	}
	
    private void DrawHighlight(SpriteBatch sb, SpriteFontBase font)
    {
        int nameSize = (int)font.MeasureString(this.Component.Name).X;
        int alteredWidth =  nameSize > this.rectangle.Width ? nameSize : this.rectangle.Width;
		int borderOffset = Component.lineThickness;

        Color highlightColor = ColorConfiguration.color_1;
        Vector2 namePos = new(this.rectangle.X, this.rectangle.Y);
        Rectangle spaceToDrawOn = new(this.rectangle.X - borderOffset, this.rectangle.Y - borderOffset, alteredWidth + 2*borderOffset, this.rectangle.Height + 2*borderOffset);

        sb.Draw(Window.whitePixelTexture, spaceToDrawOn, highlightColor);
		spaceToDrawOn = this.rectangle;
		spaceToDrawOn.Width = alteredWidth;
		sb.Draw(Window.whitePixelTexture, spaceToDrawOn, Color.White);
		sb.DrawString(font, this.Component.Name, namePos, Color.Black);
		
		this.rectangle.Width = alteredWidth + borderOffset;
    }
	
	private void DrawArrow(SpriteBatch sb, int size)
	{
		Rectangle arrowRectangle = new(this.rectangle.Left + this.rectangle.Width, this.rectangle.Center.Y - (int)Math.Round((float)this.rectangle.Height/6f), size/6, this.rectangle.Height/3);
        //Draws the arrow-body
		sb.Draw(Window.whitePixelTexture, arrowRectangle , Color.Black);
        //Draws the arrowhead
		arrowRectangle.Y = this.rectangle.Y;
		arrowRectangle.Width = 2*arrowRectangle.Width;
		arrowRectangle.Height = this.rectangle.Height;
		sb.Draw(TopologyHead.arrowhead, arrowRectangle, Color.Black);
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

    public void GoUp(List<Component> components, int columns)
    {
        int currentIndex = components.IndexOf(this.Component);
        int row = currentIndex / columns;
        int column = currentIndex % columns;
        if (row == 0) {
            return;
        }
        this.Component = components[(row-1) * columns + column];
    }

    public void GoDown(List<Component> components, int columns)
    {
        int currentIndex = components.IndexOf(this.Component);
        int row = currentIndex / columns;
        int column = currentIndex % columns;
        int newIndex = (row+1) * columns + column;
        if (newIndex >= components.Count) {
            return;
        }
        this.Component = components[newIndex];
    }

    public void GoLeft(List<Component> components, int columns)
    {
        int currentIndex = components.IndexOf(this.Component);
        int column = currentIndex % columns;
        if (column > 0) {
            this.Component = components[currentIndex - 1];
        }
    }

    public void GoRight(List<Component> components, int columns)
    {   
        if (this.Component == components.Last()) {
            return;
        }
        int currentIndex = components.IndexOf(this.Component);
        int column = currentIndex % columns;            
        if (column < columns - 1) {
            this.Component = components[currentIndex + 1];
        }
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

    public void UpdatePosition(Point windowSize)
    {
        rectangle.X = windowSize.X - 110;
    }

    public void Draw(SpriteBatch sb, SpriteFontBase font, int windowSize)
    {
        Rectangle modifiedArea = this.rectangle;
        modifiedArea.X = windowSize - 110;
        sb.Draw(Window.whitePixelTexture, modifiedArea, Color.Black);
        sb.DrawString(font, this.description, new Vector2(modifiedArea.X + 10, modifiedArea.Y + 10), Color.White);
    }
}