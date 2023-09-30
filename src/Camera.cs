using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Size = Microsoft.Xna.Framework.Point;

partial class Canvas
{
    public static class Camera
    {
        public static Point offset = Point.Zero;
        public static float zoomLevel = 1.0f; //default zoom level
        private static float minZoom = 0.1f;
        private static float maxZoom = 10.0f;
        private static float zoomSpeed = 0.0003f;
        private static int previousScrollValue = 0;


        public static Rectangle ModifiedDrawArea(Rectangle area)
        {
            int xOffset = (int)((area.X + offset.X) * zoomLevel);
            int yOffset = (int)((area.Y + offset.Y) * zoomLevel);
            int width = (int)(area.Width * zoomLevel);
            int height = (int)(area.Height * zoomLevel);

            return new Rectangle(xOffset, yOffset, width, height);
        }

        // public static bool IsVisible(Rectangle area)
        // {

        //     Rectangle checkRec = new Rectangle(offset.X, offset.Y, cameraWindowSize.X, cameraWindowSize.Y);
        //     // Check if every corner of 'area' is inside the camera view
        //     bool topLeftInside = checkRec.Contains(area.Left, area.Top);
        //     bool topRightInside = checkRec.Contains(area.Right, area.Top);
        //     bool bottomLeftInside = checkRec.Contains(area.Left, area.Bottom);
        //     bool bottomRightInside = checkRec.Contains(area.Right, area.Bottom);

        //     return topLeftInside && topRightInside && bottomLeftInside && bottomRightInside;
        // }

        public static void UpdateByMouse(MouseState mouseState, Size bufferSize)
        {
            int margin = 20; // define the margin in pixels from the edge of the screen
            int leftMargin = margin;
            int rightMargin = bufferSize.X - margin;
            int topMargin = margin;
            int bottomMargin = bufferSize.Y - margin;

            // Calculate the scroll delta based on the change in scroll wheel value
            int scrollDelta = mouseState.ScrollWheelValue - previousScrollValue;
            previousScrollValue = mouseState.ScrollWheelValue;

            // Calculate the new zoom level and limit it within specified bounds
            float newZoomLevel = zoomLevel + scrollDelta * zoomSpeed;
            newZoomLevel = MathHelper.Clamp(newZoomLevel, minZoom, maxZoom);

            // Calculate the change in zoom level
            float zoomFactor = newZoomLevel / zoomLevel;

            // Calculate the mouse position in world coordinates
            Point mouseWorldPosition = new Point(
                (int)((mouseState.Position.X - offset.X) / zoomLevel),
                (int)((mouseState.Position.Y - offset.Y) / zoomLevel)
            );

            // Calculate the new camera offset to keep the mouse position fixed while zooming
            offset.X -= (int)((mouseWorldPosition.X * zoomFactor) - mouseWorldPosition.X);
            offset.Y -= (int)((mouseWorldPosition.Y * zoomFactor) - mouseWorldPosition.Y);

            // Update the zoom level
            zoomLevel = newZoomLevel;
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