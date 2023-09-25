using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//Top is an object that keeps track of a full loaded topography
//A topography is stored as a tree with a top component
public class TopologyHead
{
	public TopologyHead(string folderName)
	{
        this.head 				= new Component("Top Level");
		this.currentComponent 	= head;

        //Insert filereader here:
		//List<Component> Computers = fileread(foldername);
        //head.setChildren(Computers)

	}

    //Temporary test-constructor
    public TopologyHead()
	{
        this.head 				= new Component("Top Level");
		this.currentComponent 	= head;

		List<Component> Computers = new List<Component>{new Computer("PC1"), new Computer("PC2",new List<Component>{new Partition("Part1"), new Partition("Part2")}), new Computer("PC3"), new Computer("WC")};
        head.SetChildren(Computers);

	}

    public void Draw(SpriteBatch sb, SpriteFont font)
    {
        int count = 0;
        //The following three variables serve to decide edge and spacing layout:
        int startX = 50;
        int startY = 100;
        int spacing = 50;

        Point pos = new(startX, startY);
 
        sb.DrawString(font, currentComponent.GetName(), new Vector2(0, 0), Color.Black);

        foreach(Component C in currentComponent.GetChildren())
        {
            C.Draw(pos, sb, font);
            count++;
            if(count < 3)
            {
                pos.X += C.GetSize().Width + spacing;
            }
            else
            {
                count = 0;
                pos.X = startX;
                pos.Y += C.GetSize().Height + spacing;
            }
        }
    }

    public Component GetCurrent()
    {
        return this.currentComponent;
    }

    public void GotoHead()
    {   
        this.currentComponent = head;
    }

	public void Goto(Component newComponent)
	{
		this.currentComponent = newComponent;
	}


	private 		 Component currentComponent;
	private readonly Component head;
}