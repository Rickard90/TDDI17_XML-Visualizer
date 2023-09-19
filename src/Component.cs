using System;
using System.Linq.Expressions;
using Microsoft.Xna.Framework;


//All types of components inherit constructor and fields from the component-type
public abstract class Component
{
	public Component()
	{
	}
	public Component(string name,
					 List<Component> children)
	{
		this.componentName	= name;
		this.children		= children;
	}


	public abstract void Draw(Point pos);

 
	private	readonly string				componentName	= "";
	private 		 float				graphicSize		= 0;
	private  		 Component			parent			= new();
	private	readonly List<Component> 	children		= new();
	private	readonly List<Component>	myConnections	= new();
		
}

//Top is an object that keeps track of a full loaded topography
//A topography is stored as a tree with a top component
public class Top
{

	public Top(Component head)
	{
		this.currentComponent 	= head;
		this.head 				= head;
	}

	public void Goto(Component newComponent)
	{
		currentComponent = newComponent;
	}

	private 		 Component currentComponent	= new();
	private readonly Component head 			= new(); 
}
