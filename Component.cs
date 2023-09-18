using System;

public class Component
{
	public Component()
	{
	}

	public virtual void Draw(position in pos);

 private:
	string			component_name	= new();
    float			graphicsize		= 0;
    component		parent			= new();
	list{component} children		= new();
	list{component}	myConnections	= new();
		
}
