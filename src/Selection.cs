using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;



public static class Selection
{
    private static MouseState    previousMouseState;
    private static KeyboardState previousKeyboardState;

    private static MouseState    currentMouseState;
    private static KeyboardState currentKeyboardState;

    private static Point mouseCursorPosition;
    private static bool leftMouseJustReleased = false;
    public  static bool ComponentGoRight = false;

    public static void Update()
    {
        UpdateMouseInfo();
        UpdateKeyInfo();
    }

    private static void UpdateMouseInfo()
    {
        previousMouseState = currentMouseState;
        currentMouseState = Mouse.GetState();
        mouseCursorPosition.X = currentMouseState.X;
        mouseCursorPosition.Y = currentMouseState.Y;

        if (previousMouseState.LeftButton == ButtonState.Pressed &&
            currentMouseState.LeftButton  == ButtonState.Released)
        {
            leftMouseJustReleased = true;
        }
        else
        {
            leftMouseJustReleased = false;
        }
    }

    private static void UpdateKeyInfo()
    {
        previousKeyboardState = currentKeyboardState;
        currentKeyboardState = Keyboard.GetState();

        if (previousKeyboardState.IsKeyDown(Keys.D) &&
            currentKeyboardState.IsKeyUp(Keys.D))
        {
            ComponentGoRight = true;
        }
        else
        {
            ComponentGoRight = false;
        }
    }

    public static bool LeftMouseJustReleased()
    {
        return leftMouseJustReleased;
    }

    public static Point MouseCursorPosition()
    {
        return mouseCursorPosition;
    }

    public static bool CursorIsInside(Rectangle rect)
    {
        return (mouseCursorPosition.X >= rect.X && mouseCursorPosition.X <= (rect.X + rect.Width) &&
				mouseCursorPosition.Y >= rect.Y && mouseCursorPosition.Y <= (rect.Y + rect.Height));
    }

    public static Component CursorIsInsideAnyComponent(List<Component> components)
    {
        foreach(Component c in components)
            if (Selection.CursorIsInside(Canvas.Camera.ModifiedDrawArea(c.Rectangle)))
                return c;
        return null;
    }

    public static LinkButton CursorIsInsideAnyLinkButton(List<Component> components)
    {
        foreach(Component c in components)
            foreach(LinkButton b in c.linkButtons)
                if (Selection.CursorIsInside(Canvas.Camera.ModifiedDrawArea(b.rectangle)))
                //if (Selection.CursorIsInside(b.rectangle))
                    return b;
        return null;
    }

}