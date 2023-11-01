using System;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//Top is an object that keeps track of a full loaded topography
//A topography is stored as a tree with a top component
//namespace XML_Visualizer;
class TopologyHead
{
	
	//This is the texture that draws the head of arrows
	public static Texture2D arrowhead; 
    
	public TopologyHead(string folderName)
	{
        //Filereader:
		//XmlReader fileRead = new();
		//XmlReader.ComponentsAndConnections cAC = fileRead.ReadComponents(folderName);
		
		/*The following bit is only for diagnostic purposes:*/
		//int counter = 0;
		//Console.WriteLine("TopologyHead Constructing\nNumber of connections: {0}", cAC.connections.Count);
		//foreach(var connection in cAC.connections)
		//{
		//	Console.WriteLine("Connection:");
		//	Console.WriteLine("Component: {0}", connection.Key);
			
		//	foreach(var port in connection.Value)
		//	{
		//		counter++;
		//		Console.WriteLine("Port {0}: {1}", counter, port.GetName());
		//	}
        //}
		//Diagnostic parapgraph done, regular code resumes:
		
		this.head = new Component("Top", XmlReader.ReadComponents(folderName), Component.Type.Top);
		this.path = new List<Component>{this.head};
	}

    public void Draw(SpriteBatch sb, SpriteFontBase font, int width, int height)
    {
        int count = 0;
        if(width < 480)
        {
            width = 480;
        }
        //The following three variables serve to decide edge and spacing layout:
        int startX  = width/24;
        int startY  = 100;
        int spacing = width/24;

        Point pos = new(startX, startY);

        // For printing the path as text
        String pathString = "";
        foreach(Component C in path)
        {
            pathString += C.Name;
            pathString += " > ";
        }
        pathString = pathString.Remove(pathString.Length - 3);

        //sb.DrawString(font, path.Last().GetName(), new Vector2(startX/2, 0), Color.Black);
        //sb.DrawString(font, pathString, new Vector2(startX/2, 0), Color.Black);
        sb.DrawString(font, pathString, new Vector2(startX/2, 0), Color.Black);
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
    public int NumberOfChildren()
    {
        return this.path.Last().Children.Count;
    }


	private List<Component> path;
	private readonly Component head;
}

