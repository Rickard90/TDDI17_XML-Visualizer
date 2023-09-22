using System;

//Top is an object that keeps track of a full loaded topography
//A topography is stored as a tree with a top component
public class TopologyHead
{

	public TopologyHead(string folderName)
	{
        this.head 				= new Component();
		this.currentComponent 	= head;

        //Insert filereader here:
		//List<Component> Computers= fileread(foldername);
        //Component.setChildren(Computers)

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
	private readonly Component head;
}