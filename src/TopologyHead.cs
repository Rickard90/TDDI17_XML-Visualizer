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

	public void Goto(Component newComponent)
	{
		currentComponent = newComponent;
	}
    public void Head()
    {   
        currentComponent = head;
    }

	private 		 Component currentComponent;
	private Component head;
}