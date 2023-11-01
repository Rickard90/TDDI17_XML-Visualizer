﻿using System;
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
	public Component(Type type)
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
	public string Name 			{get => this.name; set => this.name = value;}
    public Point Position		{get => this.position; set => this.position = value;}
    public Rectangle Rectangle => new(this.position.X, this.position.Y, this.width, this.height);
    public Component Parent		{get => this.parent; set => this.parent = value;}
    public List<Component> Children => this.children;
	private int TextMaxWidth {get{
		return this.width - (3*Component.lineThickness);
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
		int counter = 0;
		foreach(var connection in connections)
		{	
			counter++;
			//Draws the arrow-body
			sb.Draw(Window.whitePixelTexture, new Rectangle(pos.X + 5*spacing, pos.Y + spacing/2 + (counter*spacing/2), spacing/2, spacing/8), Color.Black);
			//This draws an arrowhead, OBS: the rotation is by radians and Vector2.Zero denotes the point around which you rotate
			sb.Draw(TopologyHead.arrowhead, new Rectangle(pos.X + 5*spacing + spacing/4, pos.Y + spacing/4 + spacing/16 + (counter*spacing/2), 3*spacing/4, spacing/2), Color.White);
		}
		
		this.width = this.height;
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

		Vector2 size = font.MeasureString(displayName);

		if (size.X < innerWidth)
		{
			//Console.WriteLine($"			 name is short enough already: size = {size.X}, innerWidth = {innerWidth}");
			return displayName;
		}
		else
		{
			displayName += "...";
			size = font.MeasureString(displayName);

			do
			{
				float excess = (size.X - innerWidth) / font.FontSize;
				int reduceBy = Math.Max(1, (int)excess) + "...".Length;
				displayName = displayName[..^reduceBy] + "...";
				size = font.MeasureString(displayName);
			}
			while (size.X < innerWidth);

			return displayName;

		}

	}

    public override string ToString()
    {
        return $"({this.Name}:{this.type})";
    }

	//Fields:		
	protected 		 	string				name		= "";
	protected 		   	int 				width		= 125;
	protected 		   	int 				height		= 100;
	protected			Point				position	= new(0,0);
    protected 			Component 			parent 		= null;
	protected 		 	List<Component> 	children	= new();
	public	 			Dictionary<Component, int> 	connections	= new();
	public 				enum 				Type{Top, Computer, Partition, Application, Thread, Port}
	public	  readonly 	Type 				type 		= Type.Top;

	
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
	public string interf 	= ""; 
	public string role		= "";
}