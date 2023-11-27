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
		int numberOfConnections = this.children.Count;
		int connectionsOnCurrentSide = 0;
		int first_third  = (int)Math.Ceiling(numberOfConnections/3f);
		int second_third = (int)Math.Ceiling(2*numberOfConnections/3f);
		int connectionsRight = 0;
		int connectionsLeft = 0;
		int connectionsBottom = 0;
		int sideCounter = 0;
		float offset = 0.5f;
		Point portPos = new();
		Point threadPos = new();
		Component otherPort = new();

		for(int x = 0; x < first_third; x++)
		{
			connectionsRight += this.children.ElementAt(x).connections.Keys.Count;
		}
		for(int x = first_third; x < second_third; x++)
		{
			connectionsLeft += this.children.ElementAt(x).connections.Keys.Count;
		}
		for(int x = second_third; x < this.Children.Count; x++)
		{
			connectionsBottom += this.children.ElementAt(x).connections.Keys.Count;
		}
		Console.WriteLine("Writing inside of DrawConnections() and your for_loops are borked");
		Console.WriteLine("Number of Ports: {0}, Rightside connections: {1}", numberOfConnections, connectionsRight);
		Console.WriteLine("If_1 if counter <= {0}, and If_2 when counter<={1}", (int)Math.Ceiling(numberOfConnections/3f), (int)Math.Ceiling(2*numberOfConnections/3f));
        
		
		foreach(Component port in this.children)
		{
			counter++;
			for(int y = 0; y < port.connections.Keys.Count; y++)
			{
				otherPort = port.connections.Keys.ElementAt(y);
				if(counter <= (int)Math.Ceiling(numberOfConnections/3f))		//Draws on the right of the thread
				{
					sideCounter = counter + y;
					connectionsOnCurrentSide = connectionsRight;
					portPos.X = port.Position.X + 7*spacing + spacing/4;
					portPos.Y = (this.Rectangle.Top - 2*this.height) + sideCounter * (5*this.height)/(connectionsOnCurrentSide + 1);
					threadPos.X = portPos.X + this.width/4 + spacing/4 - border;
					threadPos.Y = portPos.Y;
				}
				else if(counter <= (int)Math.Ceiling(2*numberOfConnections/3f)) //Draws on the left of the thread
				{
					sideCounter =  counter - first_third + y;
					connectionsOnCurrentSide = connectionsLeft;
					portPos.X = port.Position.X - 7*spacing + spacing/4;
					portPos.Y =  (this.Rectangle.Top - 2*this.height) + sideCounter * (5*this.height)/(connectionsOnCurrentSide+1);
					threadPos.X = portPos.X - (this.width/4 + spacing/4 - border);
					threadPos.Y = portPos.Y;
				}
				else //Draws on the bottom of the thread
				{
						sideCounter = counter - second_third + y;
						connectionsOnCurrentSide = connectionsBottom;
						portPos.X = (this.Rectangle.Left - this.width - this.width/2) + sideCounter * (4*this.width)/(connectionsOnCurrentSide + 1);
						portPos.Y = port.Position.Y + 9*spacing + spacing/4;
						threadPos.X = portPos.X;
						threadPos.Y = portPos.Y + this.height/4 + spacing/4 - border;
				}
				offset = 1f - (float)Math.Ceiling(Math.Abs((float)(connectionsOnCurrentSide+1f)/2f - sideCounter)) * (0.5f/connectionsOnCurrentSide);
				((Port)port).ConnectionOffset = offset;
				Component.DrawArrowBody(sb, port.Rectangle.Center, portPos, spacing/8, offset);
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
		int first_third  = (int)Math.Ceiling(numberOfPorts/3f);
		int second_third = (int)Math.Ceiling(2*numberOfPorts/3f);
        Point portPos = new();
		
		foreach (Component port in this.children)
		{
			counter++;
			//Calculates positions of the ports
			if(counter <= first_third)	//Draws on the right of the thread
			{
				portPos.X = this.position.X + this.width + spacing/4 - border;
            	portPos.Y = this.position.Y + counter * this.height/((int)Math.Ceiling(numberOfPorts/3.0)+1);
			}
			else if(counter <= second_third) //Draws on the left of the thread
			{
				portPos.X = this.position.X - spacing/4  + border;
				if(numberOfPorts%3 == 2)
            	{
					portPos.Y = this.position.Y + (counter - first_third) * this.height/((int)Math.Ceiling(numberOfPorts/3.0)+1);
				}		
				else
				{
					portPos.Y = this.position.Y + (counter - first_third) * this.height/((int)Math.Floor(numberOfPorts/3.0)+1);
				}
			}
			else	//Draws on the bottom of the thread
            {
				portPos.X = this.position.X + (counter - second_third) * this.width/((int)Math.Floor(numberOfPorts/3.0)+1);
				portPos.Y = this.position.Y + this.height + spacing/4 - border;
			}
			port.Draw(portPos, sb, fontSystem, spacing);
		}
	}
}
