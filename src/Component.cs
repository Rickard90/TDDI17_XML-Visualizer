using System;
//using System.Drawing;
using System.Linq.Expressions;
using System.Net.Mime;
using System.Reflection.Metadata.Ecma335;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

//All types of components inherit constructor and fields from the component-type
public class Component
{


	public Component()
	{

	}

	public Component(string name)
	{
		this.componentName	= name;
	}

	public Component(string name,
					 List<Component> children)
	{
		this.componentName	= name;
		this.children		= children;
	}

	public string GetName()
	{
		return this.componentName;
	}

	public Point getPosition()
	{
		return this.position;
	}

	public Rectangle GetRectangle()
	{
		return new Rectangle(position.X, position.X, width, height);
	}
	public List<Component> GetChildren()
	{
		return this.children;
	}

	public void SetPosition(Point pos)
	{
		this.position = pos;
	}

	public string SetName()
	{
		return this.componentName;
	}

	public void SetChildren(List<Component> newChildren)
	{
		foreach(Component c in newChildren) {
			this.AddChild(c);
		}
		//this.children = newChildren;
	}

	public void AddChild(Component newChild)
	{
		this.children.Add(newChild);
	}

	public void SetParent(Component newParent)
	{
		this.parent = newParent;
	}

	public string getName()
	{
		return componentName;
	}


	public virtual void Draw(Point pos, SpriteBatch sb, SpriteFont font)
	{
		int lineThickness = 3;
		int smallWidth  = this.width /6;
		int smallHeight = this.height/10; 
		int innerHeight = this.height - 2*lineThickness;
		int innerWidth  = 5 * smallWidth  - 2*lineThickness; 

		//Updates component's position
		this.position = pos;

		//Draws small square to the right:
		sb.Draw(Window.whitePixelTexture, new Rectangle(pos.X + 5*smallWidth, pos.Y + smallHeight, smallWidth, 8 * smallHeight), Color.Black); //black outline
		sb.Draw(Window.whitePixelTexture, new Rectangle(pos.X + 5*smallWidth, pos.Y + smallHeight + lineThickness, smallWidth - lineThickness, 8 * smallHeight - 2 * lineThickness), Color.White);
		//Draws big square:
		sb.Draw(Window.whitePixelTexture, new Rectangle(pos.X, pos.Y, 5 * smallWidth, this.height), Color.Black); //black outline
		sb.Draw(Window.whitePixelTexture, new Rectangle(pos.X + lineThickness, pos.Y + lineThickness, innerWidth, innerHeight), Color.White);
		sb.DrawString(font, this.componentName, new Vector2(pos.X + lineThickness*2 , pos.Y + lineThickness*2), Color.Black);
		
	}

 
	protected 		 	string				componentName	= "";
	protected 		   	int 				width			= 125;
	protected 		   	int 				height			= 100;
	protected			Point				position		= new(0,0);
    protected 			Component 			parent 			= null;
	protected 		 	List<Component> 	children		= new();
	protected 		 	List<Component>		myConnections	= new();
		
}

public class Computer : Component
{
	public Computer(string name) : base(name)
	{

	}

	public Computer(string name, List<Component> children) : base(name, children)
	{
		
	}
}

public class Partition : Component
{
	public Partition(string name) : base(name)
	{

	}

	public Partition(string name, List<Component> children) : base(name, children)
	{
		
	}
}

public class Application : Component
{
	public Application(string name, List<Component> children,
					   int ramSize, int initStack) : base(name, children)
	{
		this.ramSize   = ramSize;
		this.initStack = initStack;
	}
	public Application(string name,
					   int ramSize, int initStack) : base(name)
	{
		this.ramSize   = ramSize;
		this.initStack = initStack;
	}
	
	public int ramSize 	 = 0;
	public int initStack = 0;
}

public class Thread : Component
{
	public Thread(string name, List<Component> children,
				  int frequency, int exeTime, int exeStack) : base(name, children)
	{
		this.frequency = frequency;
		this.exeTime   = exeTime;
		this.exeStack  = exeStack;
	}
	
	public Thread(string name,
				  int frequency, int exeTime, int exeStack) : base(name)
	{
		this.frequency = frequency;
		this.exeTime   = exeTime;
		this.exeStack  = exeStack;
	}

	public Thread(string name,
				  int exeTime, int exeStack) : base(name)
	{
		this.exeTime   = exeTime;
		this.exeStack  = exeStack;
	}
	
	public void SetFrequency(int frequency)
	{
		this.frequency = frequency;
	}

	public int frequency = 0;
	public int exeTime 	 = 0;
	public int exeStack  = 0;
}

public class Port : Component
{
	public Port(string name, 
				string interf, string role) : base(name)
	{
			this.interf = interf;
			this.role = role;
	}

	public string interf 	= ""; 
	public string role	= "";
}