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
    //Public Functions:
    public string GetName() => this.componentName;
    public Point GetPosition() => this.position;
    public Rectangle GetRectangle() => new(this.position.X, this.position.Y, this.width, this.height);
    public Component GetParent() => this.parent;
    public List<Component> GetChildren() => this.children;

    public void SetPosition(Point pos) => this.position = pos;
	public void SetName(string name) => this.componentName = name;
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
		Console.WriteLine("******" + this.GetName());
		foreach(Component test in connections){
			Console.WriteLine("........" + test.GetName());
		}
		return ("RamSize = " + ramSize + "\nInitStack = " + initStack + "\nExecution Time = " + execTime + "\nExecution Stack = " + execStack + "\nFrequency = " + frequency);
	}
	public virtual void Draw(Point pos, SpriteBatch sb, SpriteFontBase font, int size)
	{
		this.width  = 5 * size/24;
		this.height = 4 * size/24;
		int lineThickness = 3;
		int smallWidth  = this.width/6;
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
		

		// if(name.Length > 6)
		// {
		// 	name = name[..6];
		// }
		//Draws out the name
		sb.DrawString(font, name, new Vector2(pos.X + lineThickness*2 , pos.Y + lineThickness*2), Color.Black);
		
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
	protected virtual void UpdateConnections(Component child) //Not used atm, probably not needed
	{
		foreach(Component connection in child.connections) {
			if(this.connections.Contains(connection.parent))	//If the connection is only internal it is not needed for higher up components
			{
				this.connections.Remove(connection.parent);
			}
			else
			{
				this.connections.Add(connection.parent);
			}
		}
	}

	public virtual void UpdateParentConnections(HashSet<Component> newConnections){
		foreach (var connection in newConnections) {
			if (!this.connections.Contains(connection)) {
				this.connections.Add(connection.GetParent());
			}
		}

		/* This would work, but we cannot construct LinkButtons based on the components position and size because the
		   components position and size is not set until the first Component.Draw()-method is called.
		   
		if (this.connections.Count > 0)
		{
			int buttonHeight = this.height / this.connections.Count;
			buttonHeight = buttonHeight > 20 ? 20 : buttonHeight;
			int count = 0;
			foreach (var connection in this.connections) {
				this.linkButtons.Add(new LinkButton(new Rectangle(this.position.X + this.width,
																		this.position.Y + count*buttonHeight,
																		120, buttonHeight),
																		connection));
				count += 1;
			}
		}
		*/

		// This is the "hacky" solution instead
		if (this.connections.Count > 0)
		{
			int buttonHeight = this.height / this.connections.Count;
			buttonHeight = buttonHeight > 20 ? 20 : buttonHeight;
			int count = 0;
			foreach (var connection in this.connections) {
				this.linkButtons.Add(new LinkButton(connection, count*buttonHeight, buttonHeight));
				count += 1;
			}
		}

		this.parent.UpdateParentConnections(connections);
	}
	
	
	//Properties:
	public virtual string type {get => "Component";}

	//Fields:		
	protected 		 	string				componentName	  = "";
	// Gjorde width och height publika för att kunna komma åt dem i LinkButton.Draw() // Mattias
	public	 		   	int 				width			  = 125;
	public	 		   	int 				height			  = 100;
	protected			Point				position		  = new(0,0);
    protected 			Component 			parent 			  = null;
	protected 		 	List<Component> 	children		  = new();
	public	 			HashSet<Component> 	connections		  = new();
	public				List<LinkButton>    linkButtons 	  = new();
	
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
	public override void UpdateParentConnections(HashSet<Component> newConnections){
		foreach(var connection in newConnections) {
			if (!this.connections.Contains(connection)) {
				this.connections.Add(connection.GetParent());
			}
		}
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
		// foreach(Port P in children.Cast<Port>())
		// {
		// 		connections.Add(P.GetName());
		// }
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
		Console.WriteLine("******" + this.GetName());
		foreach(Component test in connections){
			Console.WriteLine("........" + test.GetName());
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
			if (this != connectedTo) {
				this.connections.Add(connectedTo);
			}
		}
		this.parent.UpdateParentConnections(this.connections);
	}
	public override string type {get => "Port";}		
	public string interf 	= ""; 
	public string role		= "";
}