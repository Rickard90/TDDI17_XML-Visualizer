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
public class Component
{
	protected static readonly int lineThickness = 3;

	//Constructors:
	public Component()
	{

	}
    public Component(string name)
    {
        this.name = name;
    }
    public Component(string name,
					 List<Component> children)
	{
		this.name	= name;
		this.SetChildren(children);
	}
    //Public Functions:
    public string GetName() => this.name;
    public Point GetPosition() => this.position;
    public Rectangle GetRectangle() => new(this.position.X, this.position.Y, this.width, this.height);
    public Component GetParent() => this.parent;
    public List<Component> GetChildren() => this.children;

    public void SetPosition(Point pos) => this.position = pos;
	public void SetName(string name) => this.name = name;
    public void SetParent(Component newParent) 	=> this.parent = newParent;
    public void AddChild(Component newChild) 	=> this.children.Add(newChild);
	
	//Virtual Functions:   
	public virtual void SetChildren(List<Component> newChildren)
	{
		connections.Clear();
 		foreach(Component child in newChildren) {
			this.AddChild(child);
			child.SetParent(this);
			UpdateStats(child);
		}
	}
	public virtual string GetInfo()
	{
		Console.WriteLine("|" + this.GetName());
		foreach(var test in connections){
			Console.WriteLine("---->" + test.Key.GetName() + "Connection Weight: " + test.Value);
		}
		return ("RamSize = " + ramSize + "\nInitStack = " + initStack + "\nExecution Time = " + execTime + "\nExecution Stack = " + execStack + "\nFrequency = " + frequency);
	}
	public virtual void Draw(Point pos, SpriteBatch sb, SpriteFontBase font, int size)
	{
		this.width  = size/6;
		this.height = this.width;
		int spacing = size/24; //Each component is measured in a number of blocks of this size
		int border = Component.lineThickness;
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
		sb.DrawString(font, this.name, new Vector2(pos.X + 2*border , pos.Y + 2*border), Color.Black);
		
		
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
			if (this.type != "Port")
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
	
	
	//Properties:
	public virtual string type {get => "Component";}

	//Fields:		
	protected 		 	string				name	= "";
	protected 		   	int 				width			= 125;
	protected 		   	int 				height			= 100;
	protected			Point				position		= new(0,0);
    protected 			Component 			parent 			= null;
	protected 		 	List<Component> 	children		= new();
	public	 			Dictionary<Component, int> 	connections		= new();
	
	//Info:
	public int ramSize 	 = 0;
	public int initStack = 0;
	public int execTime  = 0;
	public int execStack = 0;
	public int frequency = 0;
}
//Sub-Components:

/*_______C_O_M_P_U_T_E_R________*/
public class Computer : Component
{
	public Computer(string name) : base(name)
	{

	}
	public Computer(string name, List<Component> children) : base(name, children)
	{
		
	}
	public override string type {get => "Computer";}
}

/*______P_A_R_T_I_T_I_O_N________*/
public class Partition : Component
{
	public Partition(string name) : base(name)
	{

	}
	public Partition(string name, List<Component> children) : base(name, children)
	{
		
	}
	public override string type {get => "Partition";}
}

/*______A_P_P_L_I_C_A_T_I_O_N______*/
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
	public override string type {get => "Application";}
}

/*_________T_H_R_E_A_D__________*/
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
		this.execTime  = execTime;
		this.execStack = execStack;
	}
	public Thread(string name,
				  int execTime, int execStack) : base(name)
	{
		this.execTime   = execTime;
		this.execStack  = execStack;
	}

	//Functions:
	public override void SetChildren(List<Component> newChildren)
	{
 		foreach(Port c in newChildren) {
			this.AddChild(c);
			c.SetParent(this);
			Console.WriteLine(c.GetName());

		}
	}
    public void SetFrequency(int frequency) => this.frequency = frequency;
	
    public override string GetInfo()
	{
		Console.WriteLine("|" + this.GetName());
		foreach(var test in connections){
			Console.WriteLine("---->" + test.Key.GetName() + "Connection Weight: " + test.Value);
		}
		return ("Frequency = " + frequency + ", Execution Time = " + execTime + ", Execution Stack = " + execStack);
	}

	public override string type {get => "Thread";}
}

/*__________P_O_R_T___________*/
public class Port : Component
{
	public Port(string name, 
				string interf, string role) : base(name)
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
	public override string type {get => "Port";}		
	public string interf 	= ""; 
	public string role		= "";
}