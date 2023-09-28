using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//Top is an object that keeps track of a full loaded topography
//A topography is stored as a tree with a top component
//namespace XML_Visualizer;
public class TopologyHead
{
	public TopologyHead(string folderName)
	{
        this.head = new Component("Top Level");
		this.path = new List<Component>{this.head};

        //Filereader:
		XmlReader fileRead = new XmlReader();
        this.path.Last().SetChildren(fileRead.ReadXml("Fake Data Format"));
	}

    public void Draw(SpriteBatch sb, SpriteFont font)
    {
        int count = 0;
        //The following three variables serve to decide edge and spacing layout:
        int startX = 50;
        int startY = 100;
        int spacing = 125;

        Point pos = new(startX, startY);

        // For printing the path as text
        String pathString = new String("");
        foreach(Component C in path)
        {
            pathString += C.GetName();
            pathString += " > ";
        }
        pathString = pathString.Remove(pathString.Length - 3);

        //sb.DrawString(font, path.Last().GetName(), new Vector2(startX/2, 0), Color.Black);
        sb.DrawString(font, pathString, new Vector2(startX/2, 0), Color.Black);

        foreach(Component C in path.Last().GetChildren())
        {
            C.Draw(pos, sb, font);
            count++;
            if(count < 3)
            {
                pos.X += C.GetRectangle().Width + spacing;
            }
            else
            {
                count = 0;
                pos.X = startX;
                pos.Y += C.GetRectangle().Height + spacing/6;
            }
        }
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
        return this.path.Last();;
    }

    public void GotoHead()
    {   
        this.path = new List<Component>{head};
    }

	public void Goto(Component newComponent)
	{
		this.path.Add(newComponent);
	}


	private List<Component> path = new();
	private Component head = new();
}