using Raylib_cs;
using System.Numerics;

namespace Pong
{
    public class Racket
    {
        public Racket() { }

        public Racket(int height, int width, float speed, float posX, float posY) { 
            Height = height;
            Width = width;
            Speed = speed;
            Position = new Vector2(posX, posY);
        }
        public int Height { get; set; }
        public int Width { get; set; }
        public float Speed { get; set; }
        public Vector2 Position { get; set; }

        public void SetPositionX(float position)
        {
            this.Position = new Vector2(position, this.Position.Y);
        }

        public void SetPositionY(float position)
        {
            this.Position = new Vector2(this.Position.X, position);
        }

        public void MoveUp(KeyboardKey upKey, float deltaTime)
        {
            if (Raylib.IsKeyDown(upKey) && this.Position.Y > 0)
                this.SetPositionY(this.Position.Y - (this.Speed * deltaTime));
        }

        public void MoveDown(KeyboardKey downKey, float deltaTime, int heightWindow)
        {
            if (Raylib.IsKeyDown(downKey) && this.Position.Y < heightWindow - this.Height - 1)
                this.SetPositionY(this.Position.Y + (this.Speed * deltaTime));
        }
    }
}
