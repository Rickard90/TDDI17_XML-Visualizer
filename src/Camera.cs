using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

partial class Canvas
{
    public static class Camera
    {
        public static Point offset = Point.Zero;

        public static Rectangle ModifiedDrawArea(Rectangle area)
        {
            int xOffset = (int)(area.X + offset.X);
            int yOffset = (int)(area.Y + offset.Y);
            int width = (int)area.Width;
            int height = (int)area.Height;

            return new Rectangle(xOffset, yOffset, width, height);
        }

        public static void ControlOffset(Point canvasSize, Rectangle WindowSize)
        {
            if (WindowSize.Width < canvasSize.X) {
                offset.X = Math.Min(0, offset.X);
                offset.X = Math.Max(WindowSize.Width-canvasSize.X, offset.X);
            } else {
                offset.X = Math.Max(0, offset.X);
                offset.X = Math.Min(WindowSize.Width-canvasSize.X, offset.X);
            }
            if (WindowSize.Height < canvasSize.Y) {
                offset.Y = Math.Min(0, offset.Y);
                offset.Y = Math.Max(WindowSize.Height-canvasSize.Y, offset.Y);
            } else {
                offset.Y = Math.Max(0, offset.Y);
                offset.Y = Math.Min(WindowSize.Height-canvasSize.Y, offset.Y);
            }
        }

        public static void Update(Selection.CanvasScroll ScrollChange, Point canvasSize, Rectangle WindowSize)
        {
        
            if (ScrollChange == Selection.CanvasScroll.Right)
            {
                offset.X -= 10;
            }
            if (ScrollChange == Selection.CanvasScroll.Left)
            {
                offset.X += 10;
            }
            if (ScrollChange == Selection.CanvasScroll.Down)
            {
                offset.Y -= 10;
            }
            if (ScrollChange == Selection.CanvasScroll.Up)
            {
                offset.Y += 10;
            }
            ControlOffset(canvasSize, WindowSize);
            /*if (WindowSize.Width < canvasSize.X) {
                offset.X = Math.Min(0, offset.X);
                offset.X = Math.Max(WindowSize.Width-canvasSize.X, offset.X);
            } else {
                offset.X = Math.Max(0, offset.X);
                offset.X = Math.Min(WindowSize.Width-canvasSize.X, offset.X);
            }
            if (WindowSize.Height < canvasSize.Y) {
                offset.Y = Math.Min(0, offset.Y);
                offset.Y = Math.Max(WindowSize.Height-canvasSize.Y, offset.Y);
            } else {
                offset.Y = Math.Max(0, offset.Y);
                offset.Y = Math.Min(WindowSize.Height-canvasSize.Y, offset.Y);
            }*/
            
        }
    }
}