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

        public static void Update(MouseState mouseState, KeyboardState keyboardState, Rectangle WindowSize, Point canvasSize)
        {
            if (!keyboardState.IsKeyDown(Keys.LeftControl) && !keyboardState.IsKeyDown(Keys.RightControl)) {
                if (keyboardState.IsKeyDown(Keys.Right))
                {
                    offset.X -= 10;
                }
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    offset.X += 10;
                }
                if (keyboardState.IsKeyDown(Keys.Down) || Selection.GetMouseScrollDelta() < 0)
                {
                    offset.Y -= 10;
                }
                if (keyboardState.IsKeyDown(Keys.Up) || Selection.GetMouseScrollDelta() > 0)
                {
                    offset.Y += 10;
                }
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
        }
    }
}