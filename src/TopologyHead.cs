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
	public static Texture2D arrowhead; 
    
	public TopologyHead(string folderName)
	{
		this.head = new Component("", XmlReader.ReadComponents(folderName));
		this.path = new List<Component>{this.head};
	}

    public void Draw(SpriteBatch sb, SpriteFontBase font, int zoomLevel)
    {
		int width = 67*zoomLevel;

		if(width < 480)
		{
			width = 480;
		}
		
		// For printing the path as text
		String pathString = "";
		foreach(Component C in path)
		{
			pathString += C.Name;
			pathString += " > ";
		}
		pathString = pathString.Remove(pathString.Length - 3);
		sb.DrawString(font, pathString, new Vector2(width/48, 0), Color.Black);
	
		
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

	public void Goto(Component newComponent)
	{
		this.path.Add(newComponent);
	}
	
	//Private functions and fields:
	private void DrawDefault(SpriteBatch sb, SpriteFontBase font, int width)
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
			if(count < 3)
			{
				pos.X += C.Rectangle.Width + 4*spacing;
			}
			else
			{
				count = 0;
				pos.X = startX;
				pos.Y += C.Rectangle.Height + 2*spacing;
			}
		}
	}
	private void DrawThread(SpriteBatch sb, SpriteFontBase font, int width)
    {
		try{
			Thread thread = (Thread)this.GetCurrent();
			Point pos = new(width/2, width/2);
			thread.Draw(pos, sb, font, width);
		}
		catch{};
	}
	
	private List<Component> path = new();
	private readonly Component head = new();
}
