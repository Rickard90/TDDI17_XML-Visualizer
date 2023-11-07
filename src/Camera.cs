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

        public static void UpdateByKeyboard(KeyboardState keyboardState)
        {
            if (keyboardState.IsKeyDown(Keys.Right))
            {
                offset.X -= 10;
            }
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                offset.X += 10;
            }
            if (keyboardState.IsKeyDown(Keys.Down))
            {
                offset.Y -= 10;
            }
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                offset.Y += 10;
            }
        }
    }
}