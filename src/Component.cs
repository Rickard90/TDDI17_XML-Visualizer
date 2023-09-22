using System;
//using System.Drawing;
using System.Linq.Expressions;
using System.Net.Mime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


//All types of components inherit constructor and fields from the component-type
public class Component
{

	protected static Texture2D whitePixelTexture;

	public static void LoadWhitePixelTexture(GraphicsDevice graphicsDevice)
	{
		whitePixelTexture = new(graphicsDevice, 1, 1);
       	whitePixelTexture.SetData(new Color[] { Color.White });	
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

	public Rectangle getSize() const
	{
		return new Rectangle(position.X, position.X, width, height);
	}
	public string getName() const
	{
		return componentName;
	}

	public void setChildren(List<Component> newChildren)
	{
		this.children = newChildren;
	}

	public void setParent(Component newParent)
	{
		this.parent = newParent;
	}


	public virtual void Draw(Point pos, SpriteBatch sb, SpriteFont font)
	{
		int lineThickness = 3;
		int innerHeight = this.height - 2*lineThickness;
		int innerWidth  = this.width  - 2*lineThickness; 


		//Updated component's position
		this.position = pos;

       	whitePixelTexture.SetData(new Color[] { Color.White });
		sb.Draw(whitePixelTexture, new Rectangle(pos.X, pos.Y, width, height), Color.Black);
		sb.Draw(whitePixelTexture, new Rectangle(pos.X + 	lineThickness, pos.Y + 	lineThickness, innerWidth, innerHeight), Color.White);
		sb.DrawString(font, this.componentName, new Vector2(pos.X + lineThickness*2 , pos.Y + lineThickness*2), Color.Black);
		
	}

 
	protected 		 	string				componentName	= "";
	protected 		   	int 				width			= 300;
	protected 		   	int 				height			= 300;
	protected			Point				position		= new(0,0);
    protected 			Component 			parent 			= null;
	protected 		 	List<Component> 	children		= new();
	protected 		 	List<Component>		myConnections	= new();
		
}

public class Computer : Component
{

}

public class Partition : Component
{

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

	public int frequency = 0;
	public int exeTime 	 = 0;
	public int exeStack  = 0;
}