using System;
using System.ComponentModel;
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
    public Component(string name, Type type)
		: this(type)
    {
        this.name = name;
    }
    public Component(string name, List<Component> children, Type type)
		: this(name, type)
	{
		this.SetChildren(children);
	}
    //Public Functions/Properties:
	public string Name		{get => this.name; set => this.name = value;}
    public Point Position	{get => this.position; set => this.position = value;}
    public Component Parent	{get => this.parent; set => this.parent = value;}
    public Rectangle Rectangle => new(this.position.X, this.position.Y, this.width, this.height);
	public List<Component> Children => this.children;
    private int TextMaxWidth => (this.width - (4 * Component.lineThickness));

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
		int spacing = size/24;
		this.width  = size/6;
		this.height = size/6;
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
			//this.DrawArrowBody(sb, new Point(pos.X + 5*spacing, pos.Y + spacing/2 + (counter*spacing/2)), new Point(pos.X + 5*spacing, pos.Y + spacing/2 + (counter*spacing/2)), spacing/8);
			sb.Draw(Window.whitePixelTexture, new Rectangle(pos.X + 5*spacing, pos.Y + spacing/2 + (counter*spacing/2), spacing/2, spacing/8), Color.Black);
			//This draws an arrowhead
			sb.Draw(TopologyHead.arrowhead, new Rectangle(pos.X + 5*spacing + spacing/4, pos.Y + spacing/4 + spacing/16 + (counter*spacing/2), 3*spacing/4, spacing/2), Color.White);
			//this.DrawArrowHead(sb, new Point(pos.X + 5*spacing + spacing/4, pos.Y + spacing/4 + spacing/16 + (counter*spacing/2)), spacing, 0.0f);
		}
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
	//Protected functions:
	protected virtual void UpdateStats(Component child)
	{
		this.execStack += child.execStack;
		this.execTime  += child.execTime;
		this.ramSize   += child.ramSize;
		this.initStack += child.initStack;
		this.frequency += child.frequency;
	}
	protected void DrawArrowBody(SpriteBatch sb, Point A, Point B, int thickness)
	{
		if(A.X == B.X)
		{
			sb.Draw(Window.whitePixelTexture, new Rectangle(A.X, A.Y, thickness,  Math.Abs(A.Y - B.Y)), Color.Black);
		}
		if(A.Y == B.Y)
		{
			sb.Draw(Window.whitePixelTexture, new Rectangle(A.X, A.Y,  Math.Abs(A.X - B.X), thickness), Color.Black);
		}
	}
	protected void DrawArrowHead(SpriteBatch sb, Point pos, int spacing, float rotation)
	{
		int width = 3/4 * spacing;
		int height = spacing/2;
		Rectangle destination = new(pos.X - width/2, pos.Y - height/2, width, height);
		Rectangle source = new(0,0, TopologyHead.arrowhead.Width, TopologyHead.arrowhead.Height);
		Vector2 center = new(width/2, height/2);
		//This draws an arrowhead, OBS: the rotation is by radians and Vector2.Zero denotes the point around which you rotate
		sb.Draw(TopologyHead.arrowhead, destination, null, Color.White, rotation, center, SpriteEffects.None, 1f);
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
	public				Point				position	= new(0,0);
    public 				Component 			parent 		= null;
	public	 		 	List<Component> 	children	= new();
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
