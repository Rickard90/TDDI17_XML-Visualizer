using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/*__________P_O_R_T___________*/
class Port : Component
{
	public Port(string name, 
				string interf, string role) : base(name, Type.Port)
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
	public string interf 	= ""; 
	public string role		= "";
}