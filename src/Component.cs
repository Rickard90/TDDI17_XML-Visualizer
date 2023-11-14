﻿using System;
using System.ComponentModel;
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
	private enum Direction{Up, Right, Down, Left};
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
		return "RamSize = " + ramSize + "\nInitStack = " + initStack + "\nExecution Time = " + execTime + "\nExecution Stack = " + execStack + "\nFrequency = " + frequency;
	}
	public virtual void Draw(Point pos, SpriteBatch sb, FontSystem fontSystem, int size)
	{
		SpriteFontBase font = fontSystem.GetFont(size/67);
		int spacing = size/24;
		this.width  = size/6;
		this.height = size/6;
		int border = Component.lineThickness; //Just for reading clarity's sake
		int innerHeight = this.height - 2*border;
		int innerWidth  = this.width  - 2*border;

		//Updates component's position
		this.position = pos;

		//Draws small square to the right:
		sb.Draw(Window.whitePixelTexture, new Rectangle(pos.X + this.width, pos.Y + this.height/8, 3* this.width/4, 3* this.height/4), Color.Black); //black outline
		sb.Draw(Window.whitePixelTexture, new Rectangle(pos.X + this.width, pos.Y + spacing/2 + border, 3 * this.width/4 - border, 3 * this.width/4 - 2*border), Color.White);
		//Draws big square:
		sb.Draw(Window.whitePixelTexture, new Rectangle(pos.X, pos.Y, this.width, this.height), Color.Black); //black outline
		sb.Draw(Window.whitePixelTexture, new Rectangle(pos.X + border, pos.Y + border, innerWidth, innerHeight), Color.White);
		
		//Draws out the name
		string displayName = this.CalculateDisplayName(font);
		sb.DrawString(font, displayName, new Vector2(pos.X + 2*border , pos.Y + 2*border), Color.Black);

		this.DrawLinkbuttons(sb, fontSystem);
	}
	private void DrawLinkbuttons(SpriteBatch sb, FontSystem fontSystem)
	{
		 if (this.connections.Count == 0) return;

		int smallWidth  = this.width*3/4;
		int smallHeight = this.height*3/4; 
        int smallInnerWidth  = smallWidth  - 2*LineThickness;
        int smallInnerHeight = smallHeight - 2*LineThickness;
        // Draw linkbuttons
		Rectangle inner = new Rectangle(this.position.X + LineThickness, this.position.Y + LineThickness, smallWidth, smallHeight);
        Point smallPoint = new Point(this.position.X + this.width - LineThickness, this.position.Y + lineThickness);
        Rectangle smallOuter = new Rectangle(smallPoint.X, smallPoint.Y, smallWidth, smallHeight);
        Rectangle smallInner = new Rectangle(smallPoint.X + LineThickness, smallPoint.Y + LineThickness, smallInnerWidth, smallInnerHeight);
        int numberOfLinks = Component.numberOfVisibleLinks;
		smallPoint.X += LineThickness;
        smallPoint.Y += LineThickness + this.height/8;
        int linkButtonHeight = smallInner.Height / numberOfLinks;
        int linkButtonWidth  = smallWidth - LineThickness;
        if (this.connections.Count > numberOfLinks)
        {
            if (this.linkDrawIndex > 0) {
                sb.DrawString(fontSystem.GetFont(linkButtonHeight*2), "...", new Vector2(smallPoint.X + linkButtonWidth/3, position.Y - linkButtonHeight), Color.Black);
            }
            if (this.connections.Count - this.linkDrawIndex > numberOfLinks) {
                // Draw a downwards arrow or something
                sb.DrawString(fontSystem.GetFont(linkButtonHeight*2), "...", new Vector2(smallPoint.X + linkButtonWidth/3, position.Y + smallInner.Height), Color.Black);
            }
            int counter = 0;
            int i = linkDrawIndex;
            while (counter < numberOfLinks && i < connections.Count) {
                this.linkButtons[i].Draw(sb, fontSystem.GetFont(linkButtonHeight), smallPoint, linkButtonHeight, linkButtonWidth);
				smallPoint.Y += linkButtonHeight;
                counter += 1;
                i += 1;
            }
            
        }
		else
		{
			foreach(LinkButton B in this.linkButtons)
			{
				B.Draw(sb, fontSystem.GetFont(linkButtonHeight), smallPoint, linkButtonHeight, linkButtonWidth);
				smallPoint.Y += linkButtonHeight;
			}
		}
	}
	public void UpdateConnections() {
		foreach (Component child in children) {
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
		
		Rectangle body = new(A,new Point((Math.Abs(A.X - B.X) + thickness)/2, thickness));
		Direction direction = Direction.Right;

		if((A.Y < B.Y) && (Math.Abs(A.X - B.X) < Math.Abs(A.Y - B.Y)))
		{
			direction = Direction.Down;
		}		
		else if (A.X > B.X) 
		{
			direction = Direction.Left;
		}

		switch(direction)
		{
			case Direction.Right:
				body.Y -= thickness/2;
				sb.Draw(Window.whitePixelTexture, body, Color.Black);
			
				body.X += (B.X - A.X)/2;
				body.Y += (B.Y - A.Y);
				sb.Draw(Window.whitePixelTexture, body, Color.Black);
			
				body.Width = thickness;
				body.Height = Math.Abs(A.Y - B.Y) + thickness/2;
				if(A.Y > B.Y)
				{
					body.Y = B.Y - thickness/2;
				}
				else
				{		
					body.Y = A.Y;
				}
				body.X -= thickness/2;
				sb.Draw(Window.whitePixelTexture, body, Color.Black);
				break;
			case Direction.Left:
				DrawArrowBody(sb, B, A, thickness);
				break;
			case Direction.Down:
				body.X -= thickness/2;
				body.Width = thickness;
				body.Height = (Math.Abs(A.Y - B.Y) + thickness)/2;
				sb.Draw(Window.whitePixelTexture, body, Color.Black);
				body.Y += (B.Y - A.Y)/2;
				body.X = B.X - thickness/2;
				sb.Draw(Window.whitePixelTexture, body, Color.Black);
				if(B.X > A.X)
				{
					body.X = A.X - thickness/2;
				}
				body.Height = thickness;
				body.Width = Math.Abs(A.X - B.X) + thickness/2; 
				sb.Draw(Window.whitePixelTexture, body, Color.Black);
				break;
			default:
				break;
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

    public void UpdateLinkDrawIndex()
    {
        if (Selection.linkScroll == Selection.LinkScroll.Up &&
            this.linkDrawIndex > 0) {
            this.linkDrawIndex -= 1;
        } else if (Selection.linkScroll == Selection.LinkScroll.Down &&
                   this.connections.Count - this.linkDrawIndex > Component.numberOfVisibleLinks) {
            this.linkDrawIndex += 1;
        }
    }

	//Fields:	
	public string Name		{get => this.name; set => this.name = value;}
    public Point Position	{get => this.position; set => this.position = value;}
    public Component Parent	{get => this.parent; set => this.parent = value;}
    public Rectangle Rectangle => new(this.position.X, this.position.Y, this.width, this.height);
	public List<Component> Children => this.children;
    private int TextMaxWidth => (this.width - (4 * Component.lineThickness));	
	public 				enum 				Type{Component, Computer, Partition, Application, Thread, Port};
	public	  readonly 	Type 				type 		         = Type.Component;
	protected 		 	string				name		         = "";
	protected 		   	int 				width		         = 125;
	protected 		   	int 				height		         = 100;
	public				Point				position	         = new(0,0);
    protected 			Component 			parent 	        	 = null;
	protected 		 	List<Component> 	children	         = new();
	public	 			Dictionary<Component, int> 	connections	 = new();
	public              List<LinkButton>    linkButtons          = new();
    public              int                 linkDrawIndex        = 0;
    public static readonly int              numberOfVisibleLinks = 5;
    public static readonly int              LineThickness        = 3;
	
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
