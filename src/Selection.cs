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
    private static bool leftMouseJustReleased   = false;
    public  static bool ComponentGoRight        = false;
    public  static bool ComponentGoLeft         = false;

    public  static int  GoToLink                = -1;
    
    public         enum LinkScroll { Nothing, Up, Down }
    public static       LinkScroll linkScroll = LinkScroll.Nothing;
    public        enum CanvasZoomChange  { Nothing, In, Out }
    public static       CanvasZoomChange ZoomChange = CanvasZoomChange.Nothing;
    public        enum CanvasScroll  { Nothing, Up, Down, Left, Right }
    public static       CanvasScroll ScrollChange = CanvasScroll.Nothing;
    

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

        if (previousKeyboardState.IsKeyDown(Keys.D) && currentKeyboardState.IsKeyUp(Keys.D)) {
            ComponentGoRight = true;
        }
        else {
            ComponentGoRight = false;
        }
        if (previousKeyboardState.IsKeyDown(Keys.A) && currentKeyboardState.IsKeyUp(Keys.A)) {
            ComponentGoLeft = true;
        }
        else {
            ComponentGoLeft = false;
        }

        if (currentKeyboardState.IsKeyDown(Keys.LeftControl) || currentKeyboardState.IsKeyDown(Keys.RightControl)) {
            if (previousKeyboardState.IsKeyDown(Keys.Add) && currentKeyboardState.IsKeyUp(Keys.Add) 
                 || GetMouseScrollDelta() > 0) {
                ZoomChange = CanvasZoomChange.In;
            } else if (previousKeyboardState.IsKeyDown(Keys.OemMinus) && currentKeyboardState.IsKeyUp(Keys.OemMinus) 
                 || Selection.GetMouseScrollDelta() < 0) {
                ZoomChange = CanvasZoomChange.Out;
            } else {
                ZoomChange = CanvasZoomChange.Nothing;
            }
        } else {
            ZoomChange = CanvasZoomChange.Nothing;
            ScrollChange = CanvasScroll.Nothing;
            if (currentKeyboardState.IsKeyDown(Keys.Right))
            {
                ScrollChange = CanvasScroll.Right;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Left))
            {
                ScrollChange = CanvasScroll.Left;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Down) || Selection.GetMouseScrollDelta() < 0)
            {
                ScrollChange = CanvasScroll.Down;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Up) || Selection.GetMouseScrollDelta() > 0)
            {
                ScrollChange = CanvasScroll.Up;
            }
            /* 
            if (currentKeyboardState.IsKeyDown(Keys.H))
            {
                // go to usermanual view
            }
            */
        }

        GoToLink = -1;
        linkScroll = LinkScroll.Nothing;
        if (currentKeyboardState.IsKeyDown(Keys.LeftControl)) {
            int currKey = (int)Keys.D1;
            for (int i = 1; i <= Component.numberOfVisibleLinks; ++i) {
                if (previousKeyboardState.IsKeyDown((Keys)currKey) && currentKeyboardState.IsKeyUp((Keys)currKey)) {
                    GoToLink = i;
                    return;
                }
                currKey += 1;
            }
            if (previousKeyboardState.IsKeyDown(Keys.Up) && currentKeyboardState.IsKeyUp(Keys.Up)) {
                linkScroll = LinkScroll.Up;
            }
            else if (previousKeyboardState.IsKeyDown(Keys.Down) && currentKeyboardState.IsKeyUp(Keys.Down)) {
                linkScroll = LinkScroll.Down;
            }
        }
    }
    public static int GetMouseScrollDelta()
    {
        return currentMouseState.ScrollWheelValue - previousMouseState.ScrollWheelValue;
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
                    return b;
        return null;
    }

}