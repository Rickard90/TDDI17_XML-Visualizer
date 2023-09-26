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
        this.head 				= new Component("Top Level");
		this.currentComponent 	= head;

        //Insert filereader here:

		XmlReader fileRead = new XmlReader();
        this.head.SetChildren(fileRead.ReadXml("Fake Data Format"));
	}

    public void Draw(SpriteBatch sb, SpriteFont font)
    {
        int count = 0;
        //The following three variables serve to decide edge and spacing layout:
        int startX = 50;
        int startY = 100;
        int spacing = 125;

        Point pos = new(startX, startY);
 
        sb.DrawString(font, currentComponent.GetName(), new Vector2(0, 0), Color.Black);

        foreach(Component C in currentComponent.GetChildren())
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
	private Component head;
}