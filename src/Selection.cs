using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;



public static class Selection
{
    private static bool leftMouseBeingPressed = false;
    private static bool leftMouseJustReleased = false;
    private static Point mouseCursorPosition = new Point();

    public static bool componentGoRight = false;

    public static void Update()
    {
        UpdateMouseInfo();
        UpdateKeyInfo();
    }

    private static void UpdateMouseInfo()
    {
        MouseState mouseState = Mouse.GetState();

        mouseCursorPosition = new Point( mouseState.X, mouseState.Y );

        bool pressed  = (mouseState.LeftButton == ButtonState.Pressed);
        bool released = (mouseState.LeftButton == ButtonState.Released);

        if (released && leftMouseBeingPressed) {
            leftMouseJustReleased = true;
        } else {
            leftMouseJustReleased = false;
            //leftMouseBeingPressed = !released;
        }

        leftMouseBeingPressed = pressed;
    }

    private static void UpdateKeyInfo()
    {
        if (Keyboard.GetState().IsKeyDown(Keys.D))
        {
            componentGoRight = true;
        }
        else
        {
            componentGoRight = false;
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

}