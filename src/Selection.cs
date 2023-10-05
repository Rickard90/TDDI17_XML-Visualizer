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

    private static bool leftMouseJustReleased = false;
    public  static bool componentGoRight = false;

    public static void Update()
    {
        UpdateMouseInfo();
        UpdateKeyInfo();
    }

    private static void UpdateMouseInfo()
    {
        previousMouseState = currentMouseState;
        currentMouseState = Mouse.GetState();

        if (previousMouseState.LeftButton == ButtonState.Pressed &&
            currentMouseState.LeftButton  == ButtonState.Released)
        {
            leftMouseJustReleased = true;
        }
        else
        {
            leftMouseJustReleased = false;
        }

        /*
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
        */
    }

    private static void UpdateKeyInfo()
    {
        previousKeyboardState = currentKeyboardState;
        currentKeyboardState = Keyboard.GetState();

        if (previousKeyboardState.IsKeyDown(Keys.D) &&
            currentKeyboardState.IsKeyUp(Keys.D))
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
        return new Point(currentMouseState.X, currentMouseState.Y);
    }

    public static bool CursorIsInside(Rectangle rect)
    {
        return (currentMouseState.X >= rect.X && currentMouseState.X <= (rect.X + rect.Width) &&
				currentMouseState.Y >= rect.Y && currentMouseState.Y <= (rect.Y + rect.Height));
    }

}