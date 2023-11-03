using System;
using System.ComponentModel;

//using System.Drawing;
using System.Linq.Expressions;
using System.Net.Mime;
using System.Reflection.Metadata.Ecma335;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

//All types of components inherit constructor and fields from the component-type
class Component
{
	protected static readonly int lineThickness = 3;

	//Constructors:
	public Component()
	{
		
	}
	public Component(string name, List<Component> children)
		: this(name, Type.Component)
	{
		this.SetChildren(children);
	}
	protected Component(Type type)
	{
		this.type = type;
	}
    protected Component(string name, Type type)
		: this(type)
    {
        this.name = name;
    }
    protected Component(string name, List<Component> children, Type type)
		: this(name, type)
	{
		this.SetChildren(children);
	}
    //Public Functions/Properties:
	public string Name 			{get => this.name; set => this.name = value;}
    public Point Position		{get => this.position; set => this.position = value;}
    public Rectangle Rectangle => new(this.position.X, this.position.Y, this.width, this.height);
    public Component Parent		{get => this.parent; set => this.parent = value;}
    public List<Component> Children => this.children;
	private int TextMaxWidth {get{
		return (this.width - (4*Component.lineThickness));
	}}

    public void AddChild(Component newChild) 	=> this.children.Add(newChild);
	
	//Virtual Functions:   
	public virtual void SetChildren(List<Component> newChildren)
	{
		connections.Clear();
 		foreach(Component child in newChildren) { 
			this.AddChild(child);
			child.Parent = this;
			UpdateStats(child);
		}
	}
	public virtual string GetInfo()
	{
		Console.WriteLine("|" + this.Name);
		foreach(var test in connections){
			Console.WriteLine("---->" + test.Key.Name + "Connection Weight: " + test.Value);
		}
		return ("RamSize = " + ramSize + "\nInitStack = " + initStack + "\nExecution Time = " + execTime + "\nExecution Stack = " + execStack + "\nFrequency = " + frequency);
	}
	public virtual void Draw(Point pos, SpriteBatch sb, SpriteFontBase font, int size)
	{
		this.width  = size/6;
		this.height = this.width;
		int spacing = size/24; //Each component is measured in a number of blocks of this size
		int border = Component.lineThickness; //Just for reading clarity's sake
		int innerHeight = this.height - 2*border;
		int innerWidth  = this.width  - 2*border;

		//Updates component's position
		this.position = pos;

		//Draws small square to the right:
		sb.Draw(Window.whitePixelTexture, new Rectangle(pos.X + this.width, pos.Y + spacing/2, spacing, 3*spacing), Color.Black); //black outline
		sb.Draw(Window.whitePixelTexture, new Rectangle(pos.X + this.width, pos.Y + spacing/2 + border, spacing - border, 3*spacing - 2*border), Color.White);
		//Draws big square:
		sb.Draw(Window.whitePixelTexture, new Rectangle(pos.X, pos.Y, this.width, this.height), Color.Black); //black outline
		sb.Draw(Window.whitePixelTexture, new Rectangle(pos.X + border, pos.Y + border, innerWidth, innerHeight), Color.White);
		
		//Draws out the name
		string displayName = this.CalculateDisplayName(font);
		sb.DrawString(font, displayName, new Vector2(pos.X + 2*border , pos.Y + 2*border), Color.Black);
		
		//Draws connection-arrows
		int counter = 0;	//Change this into a for-loop thank you
		foreach(var connection in connections)
		{	
			counter++;
			//Draws the arrow-body
			sb.Draw(Window.whitePixelTexture, new Rectangle(pos.X + 5*spacing, pos.Y + spacing/2 + (counter*spacing/2), spacing/2, spacing/8), Color.Black);
			//This draws an arrowhead, OBS: the rotation is by radians and Vector2.Zero denotes the point around which you rotate
			sb.Draw(TopologyHead.arrowhead, new Rectangle(pos.X + 5*spacing + spacing/4, pos.Y + spacing/4 + spacing/16 + (counter*spacing/2), 3*spacing/4, spacing/2), Color.White);
		}
	}
	//Protected functions:
	protected virtual void UpdateStats(Component child)
	{
		this.execStack += child.execStack;
		this.execTime  += child.execTime;
		this.ramSize   += child.ramSize;
		this.initStack += child.initStack;
		this.frequency += child.frequency;
	}

	public void UpdateConnections() {
		foreach (Component child in children) {
			if (this.type != Type.Port)
				child.UpdateConnections();
			foreach (var childConnection in child.connections) {
				if (connections.ContainsKey(childConnection.Key.parent)) {
					connections[childConnection.Key.parent] += childConnection.Value;
				} else {
					connections[childConnection.Key.parent] = childConnection.Value;
				}
			}
			
		}
	}

	public string CalculateDisplayName(SpriteFontBase font)
	{
		return this.CalculateDisplayName(font, TextMaxWidth);
	}
	public string CalculateDisplayName(SpriteFontBase font, int innerWidth)
	{
		string displayName = this.name;
		float excess = new();
		int reduceBy = new();

		Vector2 size = font.MeasureString(displayName);

		if (size.X < innerWidth)
		{
			//Console.WriteLine($"name is short enough already: size = {size.X}, innerWidth = {innerWidth}");
			return displayName;
		}
		else
		{
			displayName += "...";
			size = font.MeasureString(displayName);

			do
			{
				excess = (size.X - innerWidth) / font.FontSize;
				reduceBy = Math.Max(1, (int)excess) + "...".Length;
				displayName = displayName[..^reduceBy] + "...";
				size = font.MeasureString(displayName);
			}
			while (size.X > innerWidth);

			return displayName;

		}

	}

    public override string ToString()
    {
        return $"({this.Name}:{this.type})";
    }

	//Fields:		
	public 				enum 				Type{Component, Computer, Partition, Application, Thread, Port} //Should probably be named component rather than Top?
	public	  readonly 	Type 				type 		= Type.Component;
	protected 		 	string				name		= "";
	protected 		   	int 				width		= 125;
	protected 		   	int 				height		= 100;
	protected			Point				position	= new(0,0);
    protected 			Component 			parent 		= null;
	protected 		 	List<Component> 	children	= new();
	public	 			Dictionary<Component, int> 	connections	= new();
	

	
	//Info:
	public int ramSize 	 = 0;
	public int initStack = 0;
	public int execTime  = 0;
	public int execStack = 0;
	public int frequency = 0;
}
//Sub-Components:

/*_______C_O_M_P_U_T_E_R________*/
class Computer : Component
{
	public Computer(string name) : base(name, Type.Computer)
	{}
	public Computer(string name, List<Component> children) : base(name, children, Type.Computer)
	{}
}

/*______P_A_R_T_I_T_I_O_N________*/
class Partition : Component
{
	public Partition(string name) : base(name, Type.Partition)
	{

	}
	public Partition(string name, List<Component> children) : base(name, children, Type.Partition)
	{
		
	}
}

/*______A_P_P_L_I_C_A_T_I_O_N______*/
class Application : Component
{
	public Application(string name, List<Component> children,
					   int ramSize, int initStack) : base(name, children, Type.Application)
	{
		this.ramSize   = ramSize;
		this.initStack = initStack;
	}
	public Application(string name,
					   int ramSize, int initStack) : base(name, Type.Application)
	{
		this.ramSize   = ramSize;
		this.initStack = initStack;
	}
}

/*_________T_H_R_E_A_D__________*/
class Thread : Component
{
	//Constructors:
	public Thread(string name, List<Component> children,
				  int frequency, int execTime, int execStack) : base(name, children, Type.Thread)
	{
		this.frequency = frequency;
		this.execTime   = execTime;
		this.execStack  = execStack;
	}
	public Thread(string name,
				  int frequency, int execTime, int execStack) : base(name, Type.Thread)
	{
		this.frequency = frequency;
		this.execTime  = execTime;
		this.execStack = execStack;
	}
	public Thread(string name,
				  int execTime, int execStack) : base(name, Type.Thread)
	{
		this.execTime   = execTime;
		this.execStack  = execStack;
	}

	//Functions:
	public override void SetChildren(List<Component> newChildren)
	{
 		foreach(Port c in newChildren) {
			this.AddChild(c);
			c.Parent = this;
			Console.WriteLine(c.Name);

		}
	}
    public void SetFrequency(int frequency) => this.frequency = frequency;
	
    public override string GetInfo()
	{
		Console.WriteLine("|" + this.Name);
		foreach(var test in connections){
			Console.WriteLine("---->" + test.Key.Name + "Connection Weight: " + test.Value);
		}
		return ("Frequency = " + frequency + ", Execution Time = " + execTime + ", Execution Stack = " + execStack);
	}
	
	//OBS: Does not overload Component.Draw(), Used if you explicitly cast a component into a thread
	public new void Draw(Point pos, SpriteBatch sb, SpriteFontBase font, int size)
	{
		int spacing = size/24; //Each component is measured in a number of blocks of this size
		this.width  = 4*spacing;
		this.height = 5*spacing;
		int border = Component.lineThickness; //Just for reading clarity's sake
		int innerHeight = this.height - 2*border;
		int innerWidth  = this.width  - 2*border;

		//Updates component's position
		this.position.X = pos.X - this.width/2;
		this.position.Y = pos.Y - this.height/2;

		int counter = 0;
		int numberOfPorts = this.children.Count;
        Point portPos = new();
        foreach (Component port in this.children)
		{	
			counter++;
			//Draws the ports
			switch (counter%3)
			{
				case 1:		//Draws on the right of the thread
					portPos.X = this.position.X + this.width - border;
            		portPos.Y = this.position.Y + (int)Math.Ceiling(counter/3.0) * this.height/((int)Math.Ceiling(numberOfPorts/3.0)+1) - spacing/4;
					port.Draw(portPos, sb, font, size);
					break;
				case 2:		//Draws on the left of the thread
					portPos.X = this.position.X + 2*border - spacing/2;
					//Some sort of error here, gotta write out things in console to diagnose
					if(numberOfPorts%3 == 2)
            		{
						portPos.Y = this.position.Y + (int)Math.Ceiling(counter/3.0) * this.height/((int)Math.Ceiling(numberOfPorts/3.0)+1) - spacing/4;
					}		
					else
					{
						portPos.Y = this.position.Y + counter/3 * this.height/((int)Math.Floor(numberOfPorts/3.0)+1) - spacing/4;
					}
					port.Draw(portPos, sb, font, size);
					break;
				default:	//Draws on the bottom of the thread
            		portPos.X = this.position.X + (int)Math.Floor(counter/3.0) * this.width/((int)Math.Floor(numberOfPorts/3.0)+1) - spacing/4;
					portPos.Y = this.position.Y + this.height - border;
					port.Draw(portPos, sb, font, size);
					break;
			}
		}

		//Draws big square:
		sb.Draw(Window.whitePixelTexture, new Rectangle(this.position.X, this.position.Y, this.width, this.height), Color.Black); //black outline
		sb.Draw(Window.whitePixelTexture, new Rectangle(this.position.X + border, this.position.Y + border, innerWidth, innerHeight), Color.White);
		
		//Draws out the name
		string displayName = this.CalculateDisplayName(font);
		sb.DrawString(font, displayName, new Vector2(this.position.X + 2*border , this.position.Y + 2*border), Color.Black);
		
		
	}
}

/*__________P_O_R_T___________*/
class Port : Component
{
	public Port(string name, 
				string interf, string role) : base(name, Type.Thread)
	{
			this.interf = interf;
			this.role = role;
	}
	public void AddConnections(List<Port> connections)
	{
		foreach (Component connectedTo in connections) {
			if (this != connectedTo && !this.connections.ContainsKey(connectedTo)) {
				this.connections.Add(connectedTo, 1);
			}
		}
	}
	public override void Draw(Point pos, SpriteBatch sb, SpriteFontBase font, int size)
	{
		int border = Component.lineThickness; //Just for reading clarity's sake
		int spacing = size/24; //Each component is measured in a number of blocks of this size
		this.width  = spacing/2;
		this.height = spacing/2;				
		

		//Updates component's position
		this.position.X = pos.X - border;
		this.position.Y = pos.Y;

		//Draws square:
		sb.Draw(Window.whitePixelTexture, new Rectangle(this.position.X, this.position.Y, this.width, this.height), Color.Black); //black outline
		sb.Draw(Window.whitePixelTexture, new Rectangle(this.position.X + border, this.position.Y + border, this.width - 2*border, this.height - 2*border), Color.White);
	}
	public string interf 	= ""; 
	public string role		= "";
}