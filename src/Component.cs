using System;
//using System.Drawing;
using System.Linq.Expressions;
using System.Net.Mime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


//All types of components inherit constructor and fields from the component-type
public abstract class Component
{
	public Component()
	{
	}
	public Component(string name,
					 List<Component> children)
	{
		this.componentName	= name;
		this.children		= children;
	}

	public float getSize()
	{
		return graphicSize;
	}
	public string getName()
	{
		return componentName;
	}

	public abstract void Draw(Point pos, GraphicsDevice graph, SpriteBatch sb, SpriteFont font);

 
	protected 		 	string				componentName	= "";
	protected 		   	int					graphicSize		= 300;
	protected 			Component			parent;
	protected readonly 	List<Component> 	children		= new();
	protected readonly 	List<Component>		myConnections	= new();
		
}

//Top is an object that keeps track of a full loaded topography
//A topography is stored as a tree with a top component
public class Top
{

	public Top(Component head)
	{
		this.currentComponent 	= head;
		this.head 				= head;
	}

	public void Goto(Component newComponent)
	{
		currentComponent = newComponent;
	}

	private 		 Component currentComponent;
	private readonly Component head;
}

public class Computer : Component
{
	public Computer(string name)
	{
		this.componentName = name;
	}

	public override void Draw(Point pos, GraphicsDevice graph, SpriteBatch sb, SpriteFont font)
	{	
		int lineThickness = 3;
		int innerSize = graphicSize - 2*lineThickness;
		Texture2D texture = new(graph, 1, 1);
       	texture.SetData(new Color[] { Color.White });
		sb.Draw(texture, new Rectangle(pos.X, pos.Y, graphicSize, graphicSize), Color.Black);
		sb.Draw(texture, new Rectangle(pos.X + 	lineThickness, pos.Y + 	lineThickness, innerSize, innerSize), Color.White);
		sb.DrawString(font, this.componentName, new Vector2(pos.X + lineThickness*2 , pos.Y + lineThickness*2), Color.Black);
	}
}

public class Partition : Component
{
	public Partition(){}

	public override void Draw(Point pos, GraphicsDevice graph, SpriteBatch sb, SpriteFont font)
	{
		int lineThickness = 3;
		int innerSize = graphicSize - 2*lineThickness;
		Texture2D texture = new(graph, 1, 1);
       	texture.SetData(new Color[] { Color.White });
		sb.Draw(texture, new Rectangle(pos.X, pos.Y, graphicSize, graphicSize), Color.Black);
		sb.Draw(texture, new Rectangle(pos.X + 	lineThickness, pos.Y + 	lineThickness, innerSize, innerSize), Color.White);
		sb.DrawString(font, this.componentName, new Vector2(pos.X + lineThickness*2 , pos.Y + lineThickness*2), Color.Black);
	}
}

public class Application : Component
{
	public Application(){}

	public override void Draw(Point pos, GraphicsDevice graph, SpriteBatch sb, SpriteFont font)
	{
		int lineThickness = 3;
		int innerSize = graphicSize - 2*lineThickness;
		Texture2D texture = new(graph, 1, 1);
       	texture.SetData(new Color[] { Color.White });
		sb.Draw(texture, new Rectangle(pos.X, pos.Y, graphicSize, graphicSize), Color.Black);
		sb.Draw(texture, new Rectangle(pos.X + 	lineThickness, pos.Y + 	lineThickness, innerSize, innerSize), Color.White);
		sb.DrawString(font, this.componentName, new Vector2(pos.X + lineThickness*2 , pos.Y + lineThickness*2), Color.Black);
	}
}

public class Thread : Component
{
	public Thread(){}

	public override void Draw(Point pos, GraphicsDevice graph, SpriteBatch sb, SpriteFont font)
	{
		int lineThickness = 3;
		int innerSize = graphicSize - 2*lineThickness;
		Texture2D texture = new(graph, 1, 1);
       	texture.SetData(new Color[] { Color.White });
		sb.Draw(texture, new Rectangle(pos.X, pos.Y, graphicSize, graphicSize), Color.Black);
		sb.Draw(texture, new Rectangle(pos.X + 	lineThickness, pos.Y + 	lineThickness, innerSize, innerSize), Color.White);
		sb.DrawString(font, this.componentName, new Vector2(pos.X + lineThickness*2 , pos.Y + lineThickness*2), Color.Black);
	}
}