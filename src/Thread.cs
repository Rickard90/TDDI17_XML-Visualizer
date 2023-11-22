using System.Globalization;
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
	public new void Draw(Point pos, SpriteBatch sb, FontSystem fontSystem, int size)
	{	
		SpriteFontBase font = fontSystem.GetFont(size/67);
		int spacing = size/24; //Each component is measured in a number of blocks of this size
		this.width  = 4*spacing;
		this.height = 5*spacing;

		//Updates component's position
		this.position.X = pos.X - this.width/2;
		this.position.Y = pos.Y - this.height/2;
		
		DrawPorts(sb, fontSystem, spacing); //Needed to update all the port positions
		DrawBody(pos, sb, fontSystem, spacing, spacing);
		DrawConnections(sb, fontSystem, spacing);
		DrawPorts(sb, fontSystem, spacing);
	}
	
	public void DrawBody(Point pos, SpriteBatch sb, FontSystem fontSystem, int spacing, int textsize)
	{
		int border = Component.lineThickness; //Just for reading clarity's sake
		SpriteFontBase font = fontSystem.GetFont(24*textsize/67);
		this.width  = 4*spacing;
		this.height = 5*spacing;

		//Updates component's position
		this.position.X = pos.X - this.width/2;
		this.position.Y = pos.Y - this.height/2;
		
		int innerHeight = this.height - 2*border;
		int innerWidth  = this.width  - 2*border;

		//Draws big square:
		sb.Draw(Window.whitePixelTexture, new Rectangle(this.position.X, this.position.Y, this.width, this.height), Color.Black); //black outline
		sb.Draw(Window.whitePixelTexture, new Rectangle(this.position.X + border, this.position.Y + border, innerWidth, innerHeight), Color.White);
		
		//Draws out the name
		string displayName = this.CalculateDisplayName(font);
		sb.DrawString(font, displayName, new Vector2(this.position.X + 2*border , this.position.Y + 2*border), Color.Black);

	}

	private void DrawConnections(SpriteBatch sb, FontSystem fontSystem, int spacing)
	{
		int counter = 0;
		int border = Component.lineThickness;
		float offset = 0.5f;
		Point portPos = new();
		Point threadPos = new();
		Component port = new();
		Component otherPort = new();
		int numberOfConnections = 0;
		int connectionsOnCurrentSide = 0;
		int sideCounter = 0;
		for(int x = 0; x < this.Children.Count; x++)
		{
			numberOfConnections += this.Children.ElementAt(x).connections.Keys.Count;
		}
		//Console.WriteLine("This is written in Thread.DrawConnections for diagnostics:");
		//Console.WriteLine("  Thread's # of children: {0}, Number of connections: {1}", this.Children.Count, numberOfConnections);
		//Console.WriteLine("  Exact list of all connections and their ports:");
		for(int x = 0; x < this.Children.Count; x++)
		{
			port = this.Children.ElementAt(x);
			//Console.WriteLine("    Port {0} and its connections ( {1} st):", port.Name, port.connections.Keys.Count);
			for(int y = 0; y < port.connections.Keys.Count; y++)
			{
				counter++;
				offset = 0.5f;
				otherPort = port.connections.Keys.ElementAt(y);
				//Console.WriteLine("      Connection {0}", otherPort.Name);
				switch (counter%3)
				{
					case 1:		//Draws on the right of the thread
						sideCounter = (int)Math.Ceiling(counter/3.0);
						connectionsOnCurrentSide = (int)Math.Ceiling(numberOfConnections/3.0);
						portPos.X = port.Position.X + 7*spacing + spacing/4;
						portPos.Y = (this.Rectangle.Top - 2*this.height) + sideCounter * (5*this.height)/(connectionsOnCurrentSide + 1);
						threadPos.X = portPos.X + this.width/4 + spacing/4 - 2*border;
						threadPos.Y = portPos.Y;

						//The offset is currently way too big
						//Console.WriteLine("Distance from rightside center: {0}", (float)(Math.Abs(Math.Round((float)((connectionsOnCurrentSide))/2f) - (float)sideCounter)));
						//Console.WriteLine("This is connection number: {0}", sideCounter);
						//Console.WriteLine("Total number or ports on this side: {0}", connectionsOnCurrentSide);
						//Console.WriteLine("offset = {0}", offset);
						break;
					case 2:		//Draws on the left of the thread
						sideCounter =  (int)Math.Ceiling(counter/3.0);
						portPos.X = port.Position.X - 7*spacing + spacing/4;
						if(numberOfConnections%3 == 2)
            			{
							connectionsOnCurrentSide = (int)Math.Ceiling(numberOfConnections/3.0);
							portPos.Y =   (this.Rectangle.Top - 2*this.height) + sideCounter * (5*this.height)/(connectionsOnCurrentSide + 1);
						}		
						else
						{
							connectionsOnCurrentSide = (int)Math.Floor(numberOfConnections/3.0);
							portPos.Y =  (this.Rectangle.Top - 2*this.height) + sideCounter * (5*this.height)/(connectionsOnCurrentSide+1);
						}
						threadPos.X = portPos.X - (this.width/4 + spacing/4 - 2*border);
						threadPos.Y = portPos.Y;
						break;
					default:	//Draws on the bottom of the thread
						sideCounter = (int)Math.Floor(counter/3.0);
						connectionsOnCurrentSide = (int)Math.Floor(numberOfConnections/3.0);
						portPos.X = (this.Rectangle.Left - this.width - this.width/2) + sideCounter * (4*this.width)/(connectionsOnCurrentSide + 1);
						portPos.Y = port.Position.Y + 9*spacing + spacing/4;
						threadPos.X = portPos.X;
						threadPos.Y = portPos.Y + this.height/4 + spacing/4 - 2*border;
						break;
				}
				offset = 1f - (float)Math.Ceiling(Math.Abs((float)(connectionsOnCurrentSide+1f)/2f - sideCounter)) * (0.5f/connectionsOnCurrentSide);
				this.DrawArrowBody(sb, port.Rectangle.Center, portPos, spacing/8, offset);
				otherPort.Draw(portPos, sb, fontSystem, spacing);
				((Thread)otherPort.Parent).DrawBody(threadPos, sb, fontSystem, spacing/2, spacing);
			}
		}
	}

	private void DrawPorts(SpriteBatch sb, FontSystem fontSystem, int spacing)
	{
		int border = Component.lineThickness; //Just for reading clarity's sake
		int counter = 0;
		int numberOfPorts = this.children.Count;
        Point portPos = new();
        foreach (Component port in this.children)
		{	
			counter++;
			//Calculates positions of the ports
			switch (counter%3)
			{
				case 1:		//Draws on the right of the thread
					portPos.X = this.position.X + this.width + spacing/4 - border;
            		portPos.Y = this.position.Y + (int)Math.Ceiling(counter/3.0) * this.height/((int)Math.Ceiling(numberOfPorts/3.0)+1);
					break;
				case 2:		//Draws on the left of the thread
					portPos.X = this.position.X - spacing/4  + border;
					if(numberOfPorts%3 == 2)
            		{
						portPos.Y = this.position.Y + (int)Math.Ceiling(counter/3.0) * this.height/((int)Math.Ceiling(numberOfPorts/3.0)+1);
					}		
					else
					{
						portPos.Y = this.position.Y + (int)Math.Ceiling(counter/3.0) * this.height/((int)Math.Floor(numberOfPorts/3.0)+1);
					}
					break;
				default:	//Draws on the bottom of the thread
            		portPos.X = this.position.X + (int)Math.Floor(counter/3.0) * this.width/((int)Math.Floor(numberOfPorts/3.0)+1);
					portPos.Y = this.position.Y + this.height + spacing/4 - border;
					break;
			}
			port.Draw(portPos, sb, fontSystem, spacing);
		}
	}
}
