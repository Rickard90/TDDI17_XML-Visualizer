using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/*__________P_O_R_T___________*/
class Port : Component
{
	public override Type type {get => Type.Port;}
	public 	float ConnectionOffset{get => this.connectionOffset; set => this.connectionOffset = value;}
	private float connectionOffset = 0.5f;
	public string interf 	= ""; 
	public string role		= "";

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

		foreach (Port connectedTo in this.connections.Keys) {
			if (this.role != connectedTo.role)
			{
				Component thisParent = this;
				Component connectedParent = connectedTo;

				while (thisParent.type != Type.Application) {
					thisParent = thisParent.Parent;
				}
				while (connectedParent.type != Type.Application) {
					connectedParent = connectedParent.Parent;
				}
				if (connectedParent != thisParent)
				{
					((Application)thisParent).externalConnections++;
				}

				while (thisParent.type != Type.Computer) {
					thisParent = thisParent.Parent;
				}
				while (connectedParent.type != Type.Computer) {
					connectedParent = connectedParent.Parent;
				}
				if (connectedParent != thisParent)
				{
					if (role == "Sender") {
						((Computer)thisParent).connectionsExternalSend++;
					} else if (role == "Receiver"){
						((Computer)thisParent).connectionsExternalReceive++;
					}
				} else { //Internal
					((Computer)thisParent).connectionsInternal++;
				}
			}
		}
	}
	public override string GetInfo()
	{
		return role;
	}
	public override void Draw(Point pos, SpriteBatch sb, FontSystem fontSystem, int spacing)
	{
		int border = Component.lineThickness; //Just for reading clarity's sake
		this.width  = spacing/2;
		this.height = spacing/2;				
		
		//Updates component's position
		this.position.X = pos.X - width /2;
		this.position.Y = pos.Y - height/2;

		//Draws square:
		sb.Draw(Window.whitePixelTexture, new Rectangle(this.position.X, this.position.Y, this.width, this.height), Color.Black); //black outline
		sb.Draw(Window.whitePixelTexture, new Rectangle(this.position.X + border, this.position.Y + border, this.width - 2*border, this.height - 2*border), Color.White);
	}
}