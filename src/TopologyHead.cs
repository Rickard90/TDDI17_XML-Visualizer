using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//TopologyHead is an object that keeps track of a full loaded topography
//A topography is stored as a tree with a topologyHead-component

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

    public void Draw(SpriteBatch sb, FontSystem font, int zoomLevel, int width)
    {
		
		switch(this.GetCurrent().type)
        {
			case Component.Type.Thread:
				DrawThread(sb, font, zoomLevel);
				break;
			default:
				DrawDefault(sb, font, zoomLevel, width);
				break;
		}
    }
    public void DrawPath(SpriteBatch sb, int pos_x, SpriteFontBase font)
    {
        // For printing the path as text
        String pathString = "";
        foreach(Component C in path)
        {
            pathString += C.Name;
			if (C.Name != "" && this.GetCurrent().GetType() != C.GetType())
            	pathString += " > ";
        }
        sb.DrawString(font, pathString, new Vector2(pos_x+10, 10), Color.Black);
		if (GetCurrent().Children[0].type != Component.Type.Component ) //kanske ändra för "Ports" till något i still med "threadview"?
			sb.DrawString(font, GetCurrent().Children[0].type + "s:", new Vector2(pos_x+10, 37), Color.Black);
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
    public Component GetHead()
    {
        return this.head;
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

	public int NumberOfColums(int width, int zoomLevel)
	{
		return Math.Max(1, (width+2*Constants.spacing*zoomLevel/Constants.defaultZoom) / ((Constants.componentSize + 7*Constants.spacing)*zoomLevel/Constants.defaultZoom));
	}

	public void GoToChild(Component child, HighlightButton highlightButton)
	{
		if(child.type != Component.Type.Port && child.Children.Count > 0 && child != this.GetCurrent()) {
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
        if (component.type == Component.Type.Thread) {
            this.path.Add(component);
        }
		this.path.Add(component.Parent);
		while (this.path.Last().Parent != null) 
		{
			this.path.Add(this.path.Last().Parent);
		}
		this.path.Reverse();
		highlightButton.Component = component;
	}



	//Private functions and fields:
	private void DrawDefault(SpriteBatch sb, FontSystem font, int zoomLevel, int width)
    {		
		//The following three variables serve to decide edge and spacing layout:
		int spacing = Constants.spacing*zoomLevel/Constants.defaultZoom;
		int startX  = -2*spacing;//change to some negative value so that it is equal deadspace left and right
		int startY  = Constants.toolbarHeight + spacing*zoomLevel/Constants.defaultZoom;
		int adjComponentSize = Constants.componentSize*zoomLevel/Constants.defaultZoom;//zoom adjusted

		Point pos = new(startX, startY);
		
		int count = 0;
		foreach(Component C in path.Last().Children)
		{
			pos.X += 3*spacing;
			C.Draw(pos, sb, font, zoomLevel);
			count++;
			if(count < NumberOfColums(width, zoomLevel))
			{
				pos.X += adjComponentSize + 5*spacing;
			}
			else
			{
				count = 0;
				pos.X = startX;
				pos.Y += adjComponentSize + 2*spacing;
			}
		}
	}
	private void DrawThread(SpriteBatch sb, FontSystem font, int zoomLevel)
    {
		int width = 800*zoomLevel/Constants.defaultZoom;//for now
		try{
			Thread thread = (Thread)this.GetCurrent();
			Point pos = new(width/2, 2*width/5 + (int)(1.5f*Constants.toolbarHeight));
			thread.Draw(pos, sb, font, width);
		}
		catch{};
	}
}
