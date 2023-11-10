using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


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
		return (this.width - (4*Component.LineThickness));
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
	public virtual void Draw(Point pos, SpriteBatch sb, FontSystem fontSystem, int zoomLevel)
	{
		//Console.WriteLine("Zoomlevel is " + zoomLevel);
        int LT = Component.LineThickness;
		this.width  = 4 * 67*zoomLevel/24;
		this.height = 4 * 67*zoomLevel/24;
		int innerWidth  = this.width  - 2*LT;
		int innerHeight = this.height - 2*LT;
		int smallWidth  = this.width*3/4;
		int smallHeight = this.height*3/4; 
        int smallInnerWidth  = smallWidth  - 2*LT;
        int smallInnerHeight = smallHeight - 2*LT;
		string name = this.name; 

		//Updates component's position
		this.position = pos;

        Rectangle inner = new Rectangle(pos.X + LT, pos.Y + LT, innerWidth, innerHeight);
        Point smallPoint = new Point(pos.X + this.width - LT, pos.Y + (this.height - smallHeight)/2);
        Rectangle smallOuter = new Rectangle(smallPoint.X, smallPoint.Y, smallWidth, smallHeight);
        Rectangle smallInner = new Rectangle(smallPoint.X + LT, smallPoint.Y + LT, smallInnerWidth, smallInnerHeight);

		//Draws big square:
		sb.Draw(Window.whitePixelTexture, this.Rectangle, Color.Black); //black outline
		sb.Draw(Window.whitePixelTexture, inner, Color.White);
		//Draws small square to the right:
		sb.Draw(Window.whitePixelTexture, smallOuter, Color.Black); //black outline
		sb.Draw(Window.whitePixelTexture, smallInner, Color.White);

		string displayName = this.CalculateDisplayName(fontSystem.GetFont(zoomLevel));
		sb.DrawString(fontSystem.GetFont(zoomLevel), displayName, new Vector2(pos.X + 2*LT , pos.Y + 2*LT), Color.Black);

        
		// Draw linkbuttons
		if (this.linkButtons.Count > 0)
		{
            smallPoint.X += LT;
            smallPoint.Y += LT;
            int linkButtonHeight = smallHeight / 5; // <--- There are 5 buttons per component
            int linkButtonWidth  = smallWidth - LT;
			foreach(LinkButton B in this.linkButtons)
			{
				//B.Draw(sb, fontSystem.GetFont(linkButtonHeight - 4), pos, linkButtonHeight, linkButtonWidth);
				B.Draw(sb, fontSystem.GetFont(linkButtonHeight), smallPoint, linkButtonHeight, linkButtonWidth);
				pos.Y += linkButtonHeight;
			}
		}
        

		//this.width = this.height;		
        /*
		sb.Draw(Window.whitePixelTexture, new Rectangle(pos.X + 5*smallWidth, pos.Y + smallHeight, smallWidth, 8 * smallHeight), Color.Black); //black outline
		sb.Draw(Window.whitePixelTexture, new Rectangle(pos.X + 5*smallWidth, pos.Y + smallHeight + lineThickness, smallWidth - lineThickness, 8 * smallHeight - 2 * lineThickness), Color.White);
		//Draws big square:
		sb.Draw(Window.whitePixelTexture, new Rectangle(pos.X, pos.Y, 5 * smallWidth, this.height), Color.Black); //black outline
		sb.Draw(Window.whitePixelTexture, new Rectangle(pos.X + lineThickness, pos.Y + lineThickness, innerWidth, innerHeight), Color.White);
		
		//Draws out the name
		sb.DrawString(font, name, new Vector2(pos.X + lineThickness*2 , pos.Y + lineThickness*2), Color.Black);
        */
		
		//Draws connection-arrows
        /*
		int counter = 0;
		foreach(var connection in connections)
		{	
			counter++;
			//Draws the arrow-body
			sb.Draw(Window.whitePixelTexture, new Rectangle(pos.X + this.width - 2*lineThickness, pos.Y + counter*smallHeight + smallWidth/6 + lineThickness , 2*smallWidth/3, lineThickness), Color.Black); //black outline
			//This draws an arrowhead, OBS: the rotation is by radians and Vector2.Zero denotes the point around which you rotate
			sb.Draw(TopologyHead.arrowhead, new Rectangle(pos.X + this.width + smallWidth/6, pos.Y + counter*smallHeight, 2*smallWidth/3+ lineThickness, smallHeight + lineThickness ), Color.White);
		}
		*/
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

	public void UpdateConnections()
    {
		foreach (Component child in children)
        {
			if (this.type != Type.Port)
				child.UpdateConnections();
			foreach (var childConnection in child.connections)
            {
				if (connections.ContainsKey(childConnection.Key.parent)) {
					connections[childConnection.Key.parent] += childConnection.Value;
				} else {
					connections[childConnection.Key.parent] = childConnection.Value;
				}
			}
		}

        // Create linkbuttons
        foreach (var KV in connections)
        {
            this.linkButtons.Add(new LinkButton(KV.Key));
        }
	}

    public override string ToString()
    {
        return $"({this.Name}:{this.type})";
    }

	//Fields:		
	public 				enum 				Type{Top, Computer, Partition, Application, Thread, Port} //Should probably be named component rather than Top?
	public	  readonly 	Type 				type 		        = Type.Top;
	protected 		 	string				name		        = "";
	protected 		   	int 				width		        = 125;
	protected 		   	int 				height		        = 100;
	protected			Point				position	        = new(0,0);
    protected 			Component 			parent 	        	= null;
	protected 		 	List<Component> 	children	        = new();
	public	 			Dictionary<Component, int> 	connections	= new();
	public              List<LinkButton>    linkButtons         = new();
    public static readonly int              LineThickness       = 3;
	
	//Info:
	public int ramSize 	 = 0;
	public int initStack = 0;
	public int execTime  = 0;
	public int execStack = 0;
	public int frequency = 0;

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
			//Console.WriteLine($"			 name is short enough already: size = {size.X}, innerWidth = {innerWidth}");
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