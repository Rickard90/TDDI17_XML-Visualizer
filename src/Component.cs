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
	//Constructors:
	public Component()
	{

	}
    public Component(string name)
    {
        this.componentName = name;
    }
    public Component(string name,
					 List<Component> children)
	{
		this.componentName	= name;
		this.SetChildren(children);
	}
    //Functions:
    public string GetName() => this.componentName;
    public Point GetPosition() => this.position;
    public Rectangle GetRectangle() => new(this.position.X, this.position.Y, this.width, this.height);
    public Component GetParent() => this.parent;
    public List<Component> GetChildren() => this.children;

    public void SetPosition(Point pos) => this.position = pos;
	public void SetName(string name) => this.componentName = name;
    public void SetParent(Component newParent) 	=> this.parent = newParent;
    public void AddChild(Component newChild) 	=> this.children.Add(newChild);
	public virtual String GetComponentType()
	{
		return type;
	}
    public virtual void SetChildren(List<Component> newChildren)
	{
 		foreach(Component c in newChildren) {
			this.AddChild(c);
			c.SetParent(this);
		}
	}
	
	//Virtual Functions:
	public virtual string GetInfo() => "";
	public virtual void Draw(Point pos, SpriteBatch sb, SpriteFont font)
	{
		int lineThickness = 3;
		int smallWidth  = this.width /6;
		int smallHeight = this.height/10; 
		int innerHeight = this.height - 2*lineThickness;
		int innerWidth  = 5 * smallWidth  - 2*lineThickness;
		string name = this.componentName; 

		//Updates component's position
		this.position = pos;

		//Draws small square to the right:
		sb.Draw(Window.whitePixelTexture, new Rectangle(pos.X + 5*smallWidth, pos.Y + smallHeight, smallWidth, 8 * smallHeight), Color.Black); //black outline
		sb.Draw(Window.whitePixelTexture, new Rectangle(pos.X + 5*smallWidth, pos.Y + smallHeight + lineThickness, smallWidth - lineThickness, 8 * smallHeight - 2 * lineThickness), Color.White);
		//Draws big square:
		sb.Draw(Window.whitePixelTexture, new Rectangle(pos.X, pos.Y, 5 * smallWidth, this.height), Color.Black); //black outline
		sb.Draw(Window.whitePixelTexture, new Rectangle(pos.X + lineThickness, pos.Y + lineThickness, innerWidth, innerHeight), Color.White);
		

		if(name.Length > 6)
		{
			name = name[..6];
		}
		//Draws out the name
		sb.DrawString(font, name, new Vector2(pos.X + lineThickness*2 , pos.Y + lineThickness*2), Color.Black);
	}

	//Fields:
	public 	   readonly	string				type			= "Component";		
	protected 		 	string				componentName	= "";
	protected 		   	int 				width			= 125;
	protected 		   	int 				height			= 100;
	protected			Point				position		= new(0,0);
    protected 			Component 			parent 			= null;
	protected 		 	List<Component> 	children		= new();
	protected 		 	List<Component>		myConnections	= new();
		
		
}

//Sub-Components:
public class Computer : Component
{
	public Computer(string name) : base(name)
	{

	}
	public Computer(string name, List<Component> children) : base(name, children)
	{
		
	}
	public void SetChildren(List<Partition> newChildren)
	{
 		foreach(Partition c in newChildren) {
			this.AddChild(c);
			c.SetParent(this);
			this.execStack += c.execStack;
			this.execTime += c.execTime;
			this.ramSize += c.ramSize;
			this.initStack += c.initStack;
			this.frequency += c.frequency;
		}
	}
	public override string GetInfo()
	{
		return ("RamSize = " + ramSize + "\nInitStack = " + initStack + "\nExecution Time = " + execTime + "\nExecution Stack = " + execStack + "\nFrequency = " + frequency);
	}

	public new readonly	string type = "Computer";
	public int ramSize 	  = 0;
	public int initStack  = 0;
	public int execTime   = 0;
	public int execStack  = 0;
	public int frequency  = 0;
}

public class Partition : Component
{
	public Partition(string name) : base(name)
	{

	}
	public Partition(string name, List<Component> children) : base(name, children)
	{
		
	}
	public void SetChildren(List<Application> newChildren)
	{
 		foreach(Application c in newChildren) {
			this.AddChild(c);
			c.SetParent(this);
			this.execStack += c.execStack;
			this.execTime += c.execTime;
			this.ramSize += c.ramSize;
			this.initStack += c.initStack;
			this.frequency += c.frequency;
		}
	}
	public override string GetInfo()
	{
		return ("RamSize = " + ramSize + "\nInitStack = " + initStack + "\nExecution Time = " + execTime + "\nExecution Stack = " + execStack + "\nFrequency = " + frequency);
	}
	public new readonly	string type = "Partition";
	public int ramSize 	 = 0;
	public int initStack = 0;
	public int execTime   = 0;
	public int execStack  = 0;
	public int frequency  = 0;
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
	public override string GetInfo()
	{
		return ("RamSize = " + ramSize + "\nInitStack = " + initStack + "\nExecution Time = " + execTime + "\nExecution Stack = " + execStack + "\nFrequency = " + frequency);
	}

	public void SetChildren(List<Thread> newChildren)
	{
 		foreach(Thread c in newChildren) {
			this.AddChild(c);
			c.SetParent(this);
			this.execStack += c.execStack;
			this.execTime += c.execTime;
			this.frequency += c.frequency;
		}
	}

	public new readonly	string type = "Application";
	public int ramSize 	 = 0;
	public int initStack = 0;
	public int execTime   = 0;
	public int execStack  = 0;
	public int frequency  = 0;
}

public class Thread : Component
{
	//Constructors:
	public Thread(string name, List<Component> children,
				  int frequency, int execTime, int execStack) : base(name, children)
	{
		this.frequency = frequency;
		this.execTime   = execTime;
		this.execStack  = execStack;
	}
	public Thread(string name,
				  int frequency, int execTime, int execStack) : base(name)
	{
		this.frequency = frequency;
		this.execTime   = execTime;
		this.execStack  = execStack;
	}
	public Thread(string name,
				  int execTime, int execStack) : base(name)
	{
		this.execTime   = execTime;
		this.execStack  = execStack;
	}

	//Functions:
	public void SetChildren(List<Port> newChildren)
	{
 		foreach(Port c in newChildren) {
			this.AddChild(c);
			c.SetParent(this);
		}
	}
	public override	String GetComponentType()
	{
		return type;
	}
    public void SetFrequency(int frequency) => this.frequency = frequency;
    public override string GetInfo()
	{
		return ("Frequency = " + frequency + ", Execution Time = " + execTime + ", Execution Stack = " + execStack);
	}

	public new readonly	string type = "Thread";
	public int frequency  = 0;
	public int execTime   = 0;
	public int execStack  = 0;
}

public class Port : Component
{
	public Port(string name, 
				string interf, string role) : base(name)
	{
			this.interf = interf;
			this.role = role;
	}

	public new readonly	string type = "Port";		
	public string interf 	= ""; 
	public string role		= "";
}