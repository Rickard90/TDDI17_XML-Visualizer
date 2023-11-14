using System;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//TopologyHead is an object that keeps track of a full loaded topography
//A topography is stored as a tree with a topologyHead-component
//namespace XML_Visualizer;
class TopologyHead
{
	
	//This is the texture that draws the head of arrows
    private          List<Component> path;
	private readonly Component       head;
    public  static   Texture2D       arrowhead;

	public TopologyHead(string folderName)
	{
		this.head = new Component("", XmlReader.ReadComponents(folderName));
		this.path = new List<Component>{this.head};
	}

    public void Draw(SpriteBatch sb, FontSystem font, int zoomLevel)
    {
		int width = 67*zoomLevel;

		if(width < 480)
		{
			width = 480;
		}
		switch((this.GetCurrent()).type)
        {
			case Component.Type.Thread:
				DrawThread(sb, font, width);
				break;
			default:
				DrawDefault(sb, font, width);
				break;
		}
    }
    public void DrawPath(SpriteBatch sb, SpriteFontBase font)
    {
        // For printing the path as text
        String pathString = "";
        foreach(Component C in path)
        {
            pathString += C.Name;
            pathString += " > ";
        }
        sb.DrawString(font, pathString, new Vector2(105, 37), Color.Black);
    }
    public bool IsHead()
    {
        return this.head == this.path.Last();
    }
	public void GoBack()
	{
		if(path.Last() != head)
		{
			this.path.Remove(this.path.Last());
		}
	}
	public List<Component> GetPath()
	{
		return this.path;
	}
	
    public Component GetCurrent()
    {
        return this.path.Last();
    }

    public void GotoHead()
    {   
        this.path = new List<Component>{head};
    }

    public int NumberOfChildren()
    {
        return this.path.Last().Children.Count;
    }

	public void GoToChild(Component child, HighlightButton highlightButton)
	{
        if(child.GetInfo() != "") {
            Console.WriteLine("Clicked component info: " + child.Name + " Type: " + child.GetType() + "\n" + child.GetInfo());
        }
        Console.WriteLine("Component children: {0}", child.Children.Count);
        if(child.type != Component.Type.Port && child.Children.Count > 0) {
            this.path.Add(child);
            if (child.Children.Count == 0) {
                highlightButton.Component = null;
            }
            else {
                highlightButton.Component = child.Children.First();
            }
        }
	}

    public void GoToAny(Component component, HighlightButton highlightButton)
    {
        this.path.Clear();
        this.path.Add(component.Parent);
        while (this.path.Last().Parent != null) {
            this.path.Add(this.path.Last().Parent);
        }
        this.path.Reverse();
        highlightButton.Component = component;
    }



	//Private functions and fields:
	private void DrawDefault(SpriteBatch sb, FontSystem font, int width)
    {		
		//The following three variables serve to decide edge and spacing layout:
		int startX  = width/24;
		int startY  = 100;
		int spacing = width/24;

		Point pos = new(startX, startY);
		
		int count = 0;
		foreach(Component C in path.Last().Children)
		{
			C.Draw(pos, sb, font, width);
			count++;
			if(count < 2)
			{
				pos.X += C.Rectangle.Width + 7*spacing;
			}
			else
			{
				count = 0;
				pos.X = startX;
				pos.Y += C.Rectangle.Height + 2*spacing;
			}
		}
	}
	private void DrawThread(SpriteBatch sb, FontSystem font, int width)
    {
		try{
			Thread thread = (Thread)this.GetCurrent();
			Point pos = new(width/2, width/4 + 50);
			thread.Draw(pos, sb, font, width);
		}
		catch{};
	}
}
