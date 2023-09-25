using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;



public static class Selection
{
    private static bool leftMouseBeingPressed = false;
    private static bool leftMouseJustReleased = false;
    private static Point mouseCursorPosition = new Point();

    public static void UpdateMouseInfo()
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

    public static bool LeftMouseJustReleased()
    {
        return leftMouseJustReleased;
    }

    public static Point MouseCursorPosition()
    {
        return mouseCursorPosition;
    }

}