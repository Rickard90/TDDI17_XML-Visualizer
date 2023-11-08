using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
			DrawBody(pos, sb, font, size);
			DrawPorts(sb, font, size);
			DrawConnections(sb, font, size);
	}
	
	private void DrawBody(Point pos, SpriteBatch sb, SpriteFontBase font, int size)
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
		
		//Draws big square:
		sb.Draw(Window.whitePixelTexture, new Rectangle(this.position.X, this.position.Y, this.width, this.height), Color.Black); //black outline
		sb.Draw(Window.whitePixelTexture, new Rectangle(this.position.X + border, this.position.Y + border, innerWidth, innerHeight), Color.White);
		
		//Draws out the name
		string displayName = this.CalculateDisplayName(font);
		sb.DrawString(font, displayName, new Vector2(this.position.X + 2*border , this.position.Y + 2*border), Color.Black);

	}

	private void DrawConnections(SpriteBatch sb, SpriteFontBase font, int size)
	{
		int counter = 0;
		int border = Component.lineThickness;
		int spacing = size/24;
		Point pos = new();
		Component port = new();
		Component otherPort = new();
		Dictionary<Component, int> portConnections = new();
		for(int x = 0; x < this.Children.Count; x++)
		{
			port = this.Children.ElementAt(x);
			for(int y = 0; y < port.connections.Keys.Count; y++)
			{
				counter++;
				otherPort = port.connections.Keys.ElementAt(y);
				switch (counter%3)
				{
					case 1:		//Draws on the right of the thread
						pos.X = size - this.width - spacing;
						pos.Y = 3*spacing + (counter-1) * 6*spacing;
						otherPort.Draw(pos, sb, font, size);
						break;
					case 2:		//Draws on the left of the thread
						pos.X = spacing;
						pos.Y = 3*spacing + (counter-1) * 6*spacing;
						otherPort.Draw(pos, sb, font, spacing);
						break;
					default:	//Draws on the bottom of the thread
						pos.X = port.position.X;
						pos.Y = port.position.Y + this.height + spacing;
						otherPort.Draw(pos, sb, font, spacing);
						break;
				}
			}
		}
	}

	public void DrawPorts(SpriteBatch sb, SpriteFontBase font, int size)
	{
		int spacing = size/24;
		int border = Component.lineThickness; //Just for reading clarity's sake
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
					port.Draw(portPos, sb, font, spacing);
					break;
				case 2:		//Draws on the left of the thread
					portPos.X = this.position.X + 2*border - spacing/2;
					if(numberOfPorts%3 == 2)
            		{
						portPos.Y = this.position.Y + (int)Math.Ceiling(counter/3.0) * this.height/((int)Math.Ceiling(numberOfPorts/3.0)+1) - spacing/4;
					}		
					else
					{
						portPos.Y = this.position.Y + (int)Math.Ceiling(counter/3.0) * this.height/((int)Math.Floor(numberOfPorts/3.0)+1) - spacing/4;
					}
					port.Draw(portPos, sb, font, spacing);
					break;
				default:	//Draws on the bottom of the thread
            		portPos.X = this.position.X + (int)Math.Floor(counter/3.0) * this.width/((int)Math.Floor(numberOfPorts/3.0)+1) - spacing/4;
					portPos.Y = this.position.Y + this.height - border;
					port.Draw(portPos, sb, font, spacing);
					break;
			}
		}
	}
}
