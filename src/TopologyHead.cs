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
    public TopologyHead()
	{
        this.head 				= new Component("Top Level");
		this.currentComponent 	= head;

		List<Component> Computers = new List<Component>{new Computer("PC1"), new Computer("PC2",new List<Component>{new Partition("Part1"), new Partition("Part2")}), new Computer("PC3"), new Computer("WC")};
        head.SetChildren(Computers);

	}

    public void Draw(SpriteBatch sb, SpriteFont font)
    {
        Point pos = new(50, 100);
        sb.DrawString(font, currentComponent.GetName(), new Vector2( 50, 50), Color.Black);
        foreach(Component C in currentComponent.GetChildren())
        {
            C.Draw(pos, sb, font);
            if( pos.X < 270)
            {
                pos.X += 110;
            }
            else
            {
                pos.X = 50;
                pos.Y += 110;
            }
        }  
    }

    public Component GetCurrent()
    {
        return this.currentComponent;
    }

    public void GotoHead()
    {   
        currentComponent = head;
    }

	public void Goto(Component newComponent)
	{
		currentComponent = newComponent;
	}


	private 		 Component currentComponent;
	private readonly Component head;
}