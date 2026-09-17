using System.Numerics;

namespace Pong
{
    public class Ball
    {
        public Ball() { }
        public Ball(int radius, float posX, float posY, int baseMag, int maxMag, int minMag, int speed) { 
            Radius = radius;
            DefaultMagnitudeVel = baseMag;
            MaxMagnitudeVel = maxMag;
            MinMagnitudeVel = minMag;
            BaseSpeed = speed;
            BaseXPos = posX;
            BaseYPos = posY;
            Position = new Vector2(posX, posY);
            Velocity = new Vector2(baseMag, baseMag);
        }
        public int Radius { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public bool IsRightDirection { get; set; } = true;
        public int DefaultMagnitudeVel { get; set; }
        public int MaxMagnitudeVel { get; set; }
        public int MinMagnitudeVel { get; set; }
        public int BaseSpeed { get; set; }
        public float BaseXPos { get; set; }
        public float BaseYPos { get; set; }

        public void ResetBall()
        {
            this.Position = new Vector2(BaseXPos, BaseYPos);
            if (this.IsRightDirection)
                this.Velocity = new Vector2(this.DefaultMagnitudeVel, this.DefaultMagnitudeVel);
            else
                this.Velocity = new Vector2(this.DefaultMagnitudeVel * -1, this.DefaultMagnitudeVel);
        }

        public void ChangeAngle (Vector2 racket, int racketHeight)
        {
            double racketCenter = racket.Y + (racketHeight / 2);
            double impactPoint = (this.Position.Y - racketCenter) / (racketHeight / 2);
            impactPoint = Math.Max(-1.0, Math.Min(1.0, impactPoint));

            var velocityY = (int)(impactPoint * MaxMagnitudeVel);
            int directionX = (Velocity.X > 0) ? 1 : -1;
            float magnitudeX = (Math.Abs(Velocity.Y) < 3) ? directionX * DefaultMagnitudeVel : directionX * MinMagnitudeVel;
            var velocityX = directionX > 0 ? Math.Abs(magnitudeX) * -1 : Math.Abs(magnitudeX);
            Vector2 normalize = Vector2.Normalize(new Vector2(velocityX, velocityY));
            this.Velocity = normalize * BaseSpeed;
        }

        public void SetVelocitY(float velocity)
        {
            this.Velocity = new Vector2 (this.Velocity.X, velocity);
        }

        public void SetVelocityX(float velocity)
        {
            this.Velocity = new Vector2(velocity, this.Velocity.Y);
        }

        public void SetPositionY(float position)
        {
            this.Position = new Vector2(this.Position.X, position);
        }

        public void SetPositionX(float position)
        {
            this.Position = new Vector2(position, this.Position.Y);
        }
    }
}
