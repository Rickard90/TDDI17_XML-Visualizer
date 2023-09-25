using System.Drawing;
using Microsoft.Xna.Framework.Input;



public static class Selection
{
    private static bool leftMouseBeingPressed = false;
    private static bool leftMouseJustReleased = false;
    private static Point mouseCursorPosition = new Point();

    public static void CheckMouse()
    {
        MouseState mouseState = Mouse.GetState();

        mouseCursorPosition = new Point( mouseState.X, mouseState.Y );

        bool released = (mouseState.LeftButton == ButtonState.Released);

        if (released && leftMouseBeingPressed) {
            leftMouseJustReleased = true;
        } else {
            leftMouseJustReleased = false;
            leftMouseBeingPressed = !released;
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

}