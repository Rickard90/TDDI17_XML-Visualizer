using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//All types of components inherit constructor and fields from the component-type
public class Component
{
	//Fields:
	public virtual Component.Type type {get => Type.Component;}
	
	public string Name		{get => this.name; set => this.name = value;}
    public Point Position	{get => this.position; set => this.position = value;}
    public Component Parent	{get => this.parent; set => this.parent = value;}
    public Rectangle Rectangle => new(this.position.X, this.position.Y, this.width, this.height);
	public List<Component> Children => this.children;
    private int TextMaxWidth => (this.width - (4 * Component.lineThickness));	
	public 				enum 				Type{Component, Computer, Partition, Application, Thread, Port};
	protected 		 	string				name		         = "";
	protected 		   	int 				width		         = Constants.componentSize;
	protected 		   	int 				height		         = Constants.componentSize;
	protected			Point				position	         = new(0,0);
    protected 			Component 			parent 	        	 = null;
	protected 		 	List<Component> 	children	         = new();
	public	 			Dictionary<Component, int> 	connections	 = new();
	public              List<LinkButton>    linkButtons          = new();
    public              int                 linkDrawIndex        = 0;
    public static readonly int              numberOfVisibleLinks = 5;
    public static readonly int              lineThickness        = 3;
	protected enum Direction{Up, Right, Down, Left, None};

	//Info:
	public int ramSize 	 = 0;
	public int initStack = 0;
	public int execTime  = 0;
	public int execStack = 0;
	public int frequency = 0;

	//Constructors:
	public Component()
	{
		
	}
	public Component(string name, List<Component> children)
		: this(name)
	{
		this.SetChildren(children);
	}
    public Component(string name)
    {
        this.name = name;
    }
	public Component(Component otherComponent)
		: this(otherComponent.Name)
	{
		this.position 	 = new Point(otherComponent.Position.X, otherComponent.Position.Y);
		this.parent		 = otherComponent.Parent;
		this.children 	 = otherComponent.children;
		this.width		 = otherComponent.width;	
		this.height 	 = otherComponent.height;
		this.connections = otherComponent.connections;
		this.linkButtons = otherComponent.linkButtons;
		this.linkDrawIndex = otherComponent.linkDrawIndex;
		
		this.ramSize	= otherComponent.ramSize;
		this.initStack	= otherComponent.initStack;
		this.execTime 	= otherComponent.execTime;
		this.execStack 	= otherComponent.execStack;
		this.frequency 	= otherComponent.frequency;
	}

    public void AddChild(Component newChild) => this.children.Add(newChild);
	
	public bool IsEmpty()
	{
		return this.Children.Count == 0;
	}
	
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
		return this.BaseInfo();
	}
	protected string BaseInfo()
	{
		return 	"RamSize = " + ramSize + 
				"\nInitStack = " + initStack + 
				"\nExecution Time = " + execTime + 
				"\nExecution Stack = " + execStack;
	}
	public virtual List<Component> TooltipList()
	{
		return this.children;
	}

	public virtual void Draw(Point pos, SpriteBatch sb, FontSystem fontSystem, int zoomLevel)
	{
		//Updates component info:
		this.position = pos;
		this.width  = Constants.componentSize*zoomLevel/Constants.defaultZoom;
		this.height = this.width;
		
		SpriteFontBase font = fontSystem.GetFont(zoomLevel);
		Rectangle internalRectangle = new(pos.X + lineThickness, pos.Y + lineThickness, this.height - 2*lineThickness, this.width  - 2*lineThickness);
		Rectangle sideRectangle = new(pos.X + width, pos.Y + height/8, 3* width/4, 3* height/4);
		Rectangle internalSideRectangle = new(sideRectangle.X, sideRectangle.Y + lineThickness, sideRectangle.Width - lineThickness, sideRectangle.Height - 2* lineThickness);

		//Draws small square to the right:
		sb.Draw(Window.whitePixelTexture, sideRectangle, Color.Black); //black outline
		sb.Draw(Window.whitePixelTexture, internalSideRectangle, Color.White);
		//Draws big square:
		sb.Draw(Window.whitePixelTexture, this.Rectangle, Color.Black); //black outline
		sb.Draw(Window.whitePixelTexture, internalRectangle, Color.White);
		//Draws component name:
		string displayName = this.CalculateDisplayName(font);
		sb.DrawString(font, displayName, new Vector2(pos.X + 2*lineThickness , pos.Y + 2*lineThickness), Color.Black);
		//Handles buttons:
		this.DrawLinkbuttons(sb, fontSystem);
	}


	private void DrawLinkbuttons(SpriteBatch sb, FontSystem fontSystem)
	{
		 if (this.connections.Count == 0) return;

		int smallWidth  = this.width*3/4;
		int smallHeight = this.height*3/4; 
        int smallInnerWidth  = smallWidth  - 2*lineThickness;
        int smallInnerHeight = smallHeight - 2*lineThickness;
        // Draw linkbuttons
		Rectangle inner = new Rectangle(this.position.X + lineThickness, this.position.Y + lineThickness, smallWidth, smallHeight);
        Point smallPoint = new Point(this.position.X + this.width - lineThickness, this.position.Y + lineThickness);
        Rectangle smallOuter = new Rectangle(smallPoint.X, smallPoint.Y, smallWidth, smallHeight);
        Rectangle smallInner = new Rectangle(smallPoint.X + lineThickness, smallPoint.Y + lineThickness, smallInnerWidth, smallInnerHeight);
        int numberOfLinks = Component.numberOfVisibleLinks;
		smallPoint.X += lineThickness;
        smallPoint.Y += this.height / 8; // lineThickness;
        int linkButtonHeight = smallInner.Height / numberOfLinks;
        int linkButtonWidth  = smallWidth - lineThickness;
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

	public static void DrawArrowBody(SpriteBatch sb, Point A, Point B, int thickness, float offset = 0.5f, Color color = new())
	{	
		if (color == new Color())
		{
			color = Color.Black;
		}
		Rectangle body = new(A, new Point((int)(offset*Math.Abs(A.X - B.X)) + thickness/2, thickness));
		Direction direction = Direction.Right;
		if (offset < 0f || offset > 1f)
		{
			offset = 1f;
		}
		if((A.Y < B.Y) && (1.5*Math.Abs(A.X - B.X) < Math.Abs(A.Y - B.Y)))
		{
			direction = Direction.Down;
		}		
		else if ((A.Y > B.Y) && (2*Math.Abs(A.X - B.X) < Math.Abs(A.Y - B.Y)))
		{
			direction = Direction.Up;
		}		
		else if (A.X > B.X) 
		{
			direction = Direction.Left;
		}

		switch(direction)
		{
			case Direction.Left:
				DrawArrowBody(sb, B, A, thickness, 1f - offset, color);
				break;
			case Direction.Up:
				DrawArrowBody(sb, B, A, thickness, 1f - offset, color);
				break;
			case Direction.Right:
				body.Y -= thickness/2;
				sb.Draw(Window.whitePixelTexture, body, color);
				body.X += body.Width - thickness/2;
				body.Width = (int)((1f - offset) * Math.Abs(A.X - B.X)) + thickness/2;
				body.Y += B.Y - A.Y;
				sb.Draw(Window.whitePixelTexture, body, color);
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
				sb.Draw(Window.whitePixelTexture, body, color);
				break;
			case Direction.Down:
				body.X -= thickness/2;
				body.Width = thickness;
				body.Height = (int)(Math.Abs(A.Y - B.Y)* offset) + thickness/2;
				sb.Draw(Window.whitePixelTexture, body, color);
				body.Y += body.Height - thickness;
				body.X = B.X - thickness/2;
				body.Height = (int)(Math.Abs(A.Y - B.Y)* (1f-offset)) + thickness/2;
				sb.Draw(Window.whitePixelTexture, body, color);
				if(B.X > A.X)
				{
					body.X = A.X - thickness/2;
				}
				body.Height = thickness;
				body.Width = Math.Abs(A.X - B.X) + thickness/2; 
				sb.Draw(Window.whitePixelTexture, body, color);
				break;
		}
	}
	protected void DrawArrowHead(SpriteBatch sb, Point pos, int spacing, Component.Direction direction = Direction.None)
	{
		double rotation = 0f;
		int width =  spacing;
		int height = (int)(3/4f*spacing);
		Rectangle destination = new(pos.X, pos.Y, width, height);
		Vector2 center = new(TopologyHead.arrowhead.Width/2, TopologyHead.arrowhead.Height/2);
		switch(direction)
		{
			case Direction.Right:
			rotation = 0f;
			break;
			case Direction.Down:
			rotation = Math.PI/2;
			break;
			case Direction.Left:
			rotation = Math.PI;
			break;
			case Direction.Up:
			rotation = -Math.PI/2;
			break;
		}
		//This draws an arrowhead, OBS: the rotation is by radians and Vector2.Zero denotes the point around which you rotate
		sb.Draw(TopologyHead.arrowhead, destination, null, Color.Black, (float)rotation, center, SpriteEffects.None, 0f);	
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

	
}

//Sub-Components:
/*_______C_O_M_P_U_T_E_R________*/
class Computer : Component
{
	public override Type type {get => Type.Computer;}
	public int connectionsExternalSend = 0;
	public int connectionsExternalReceive = 0;
	public int connectionsInternal = 0;
	public int TotalConnections {get => connectionsExternalReceive + connectionsExternalSend;}

	public Computer(string name) : base(name)
	{}
	public Computer(string name, List<Component> children) : base(name, children)
	{}
	public override string GetInfo()
	{
		return 	this.BaseInfo() + 
				"\nTotal Connections = " + this.TotalConnections +
				"\nOutgoing Connections = " + connectionsExternalSend +
				"\nIncoming Connections = " + connectionsExternalReceive;
	}
	public override void Draw(Point pos, SpriteBatch sb, FontSystem fontSystem, int zoomLevel)
	{
		SpriteFontBase font = fontSystem.GetFont(zoomLevel);
		this.width  = (int)Math.Ceiling(1.2*Constants.componentSize*zoomLevel/Constants.defaultZoom);
		this.height = this.width;
		int spacing = this.width/4;
		int biggestConnectionStringLength = connectionsExternalSend > connectionsExternalReceive ? connectionsExternalSend.ToString().Length : connectionsExternalReceive.ToString().Length;
		int sideRectangleOffset = biggestConnectionStringLength * zoomLevel;
		Rectangle internalRectangle = new(pos.X + lineThickness, pos.Y + lineThickness, this.height - 2*lineThickness, this.width  - 2*lineThickness);
		Rectangle sideRectangle = new(pos.X + width, pos.Y + spacing/2, sideRectangleOffset + 2*lineThickness, 3*spacing);
		Rectangle internalSideRectangle = new(sideRectangle.X, sideRectangle.Y + lineThickness, sideRectangle.Width - lineThickness, sideRectangle.Height - 2* lineThickness);

		//Updates component's position
		this.position = pos;

		//Draws small square to the right:
		sb.Draw(Window.whitePixelTexture, sideRectangle, Color.Black); //black outline
		sb.Draw(Window.whitePixelTexture, internalSideRectangle, Color.White);
		//Draws big square:
		sb.Draw(Window.whitePixelTexture, this.Rectangle, Color.Black); //black outline
		sb.Draw(Window.whitePixelTexture, internalRectangle, Color.White);

		//Connections out Arrow
		Point arrowHead = new(sideRectangle.X + spacing/2 + biggestConnectionStringLength * zoomLevel + lineThickness, sideRectangle.Center.Y - 3*spacing/4);
		Point arrowStart = new(arrowHead.X - spacing/2, arrowHead.Y - zoomLevel/2);
		sideRectangleOffset = zoomLevel*(biggestConnectionStringLength-connectionsExternalSend.ToString().Length);
		DrawArrowHead(sb, arrowHead, spacing);
		sb.Draw(Window.whitePixelTexture, new Rectangle(arrowStart, new Point(spacing/2, zoomLevel)), Color.Black);
		sb.DrawString(fontSystem.GetFont(2*zoomLevel), connectionsExternalSend.ToString() ,new Vector2(sideRectangle.X + sideRectangleOffset, arrowHead.Y - zoomLevel), Color.Black);


		//Connections in Arrow
		arrowHead.Y += 6*spacing/4;
		arrowStart.Y += 6*spacing/4;
		arrowStart.X += spacing/2;
		sideRectangleOffset = zoomLevel*(biggestConnectionStringLength-connectionsExternalReceive.ToString().Length);
		DrawArrowHead(sb, arrowHead, spacing, Direction.Left);
		sb.Draw(Window.whitePixelTexture, new Rectangle(arrowStart, new Point(spacing/2, zoomLevel)), Color.Black);
		sb.DrawString(fontSystem.GetFont(2*zoomLevel), connectionsExternalReceive.ToString() ,new Vector2(sideRectangle.X + sideRectangleOffset, arrowHead.Y - zoomLevel), Color.Black);
		//Draws out the name
		string displayName = this.CalculateDisplayName(font);
		sb.DrawString(font, displayName, new Vector2(pos.X + 2*lineThickness , pos.Y + 2*lineThickness), Color.Black);
	}
}

/*______P_A_R_T_I_T_I_O_N________*/
class Partition : Component
{
	public override Type type {get => Type.Partition;}
	
	public Partition(string name) : base(name)
	{

	}
	public Partition(string name, List<Component> children) : base(name, children)
	{
		
	}
	protected override void UpdateStats(Component child)
	{
		this.execStack += child.execStack;
		this.execTime  += child.execTime;
		this.ramSize   += child.ramSize;
		this.initStack += child.initStack;
	}
}

/*______A_P_P_L_I_C_A_T_I_O_N______*/
class Application : Component
{
	public int externalConnections = 0;
	public override Type type {get => Type.Application;}
	
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
	protected override void UpdateStats(Component child)
	{
		this.execStack += child.execStack;
		this.execTime  += child.execTime/child.frequency; //eller kanske child.execTime*child.frequency?
		this.ramSize   += child.ramSize;
		this.initStack += child.initStack;
		this.externalConnections += ((Thread)child).connectionsIn + ((Thread)child).connectionsOut;
	}
	public override string GetInfo()
	{
		return "Execution Time = " + execTime 
				+ "\nExecution Stack = " + execStack 
				+ "\nRamsize = " + ramSize
				+ "\nExternal Connections = " + externalConnections;
	}


}
