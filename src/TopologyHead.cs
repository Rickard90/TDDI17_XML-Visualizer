using System;

//Top is an object that keeps track of a full loaded topography
//A topography is stored as a tree with a top component
namespace XML_Visualizer;
public class TopologyHead
{

	public TopologyHead(string folderName)
	{
        this.head 				= new Component();
		this.currentComponent 	= head;

        //Insert filereader here:
		XmlReader fileRead = new XmlReader();
        this.head.SetChildren(fileRead.ReadXml());

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
	private Component head;
}