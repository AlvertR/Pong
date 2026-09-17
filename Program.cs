using Pong;
using Raylib_cs;
using System.Numerics;

public class Program
{

    public Program() { }

    public static void Main(string[] args)
    {
        const int heightWindow = 450, widthWindow = 800;

        Raylib.InitWindow(widthWindow, heightWindow, "Pong");
        Raylib.InitAudioDevice();
        string basePath = AppDomain.CurrentDomain.BaseDirectory;
        
        string fulPath = Path.Combine(basePath, "Resource", "pong-image.png");
        Image icon = Raylib.LoadImage(fulPath);
        Raylib.ImageFormat(ref icon, PixelFormat.UncompressedR8G8B8A8);
        Raylib.SetWindowIcon(icon);
        Raylib.UnloadImage(icon);

        string soundPath = Path.Combine(basePath, "Resource", "tennis-ball-hit.mp3");
        Sound hitBallSound = Raylib.LoadSound(soundPath);

        Raylib.SetTargetFPS(60);

        Racket leftRacket = new Racket(100, 15, 600, 40, 10);
        Racket rightRacket = new Racket(100, 15, 600, (widthWindow - 15 - leftRacket.Position.X), 10);
        Ball ball = new Ball(30, 400, 225, 160, 200, 140, 226);

        float deltaTime = 0;
        
        int leftPlayerScore = 0;
        int rightPlayerScore = 0;
        
        float runningSeconds = 60;
        int displaySeconds = (int)runningSeconds;
        GameStatus gameState = GameStatus.Start;

        while (!Raylib.WindowShouldClose())
        {
            deltaTime = Raylib.GetFrameTime();
            HandleInput();
            Update();
            Draw();
        }

        void HandleInput()
        {
            switch (gameState)
            {
                case GameStatus.Start:
                    if (Raylib.IsKeyPressed(KeyboardKey.I))
                        gameState = GameStatus.Playing;
                    break;
                case GameStatus.Playing:
                    if (Raylib.IsKeyDown(KeyboardKey.Up) && rightRacket.Position.Y > 0)
                        rightRacket.SetPositionY(rightRacket.Position.Y - (rightRacket.Speed * deltaTime));
                    if (Raylib.IsKeyDown(KeyboardKey.Down) && rightRacket.Position.Y < heightWindow - rightRacket.Height - 1)
                        rightRacket.SetPositionY(rightRacket.Position.Y + (rightRacket.Speed * deltaTime));

                    if (Raylib.IsKeyDown(KeyboardKey.W) && leftRacket.Position.Y > 0)
                        leftRacket.SetPositionY(leftRacket.Position.Y - (leftRacket.Speed * deltaTime));
                    if (Raylib.IsKeyDown(KeyboardKey.S) && leftRacket.Position.Y < heightWindow - leftRacket.Height - 1)
                        leftRacket.SetPositionY(leftRacket.Position.Y + (leftRacket.Speed * deltaTime));
                    if (Raylib.IsKeyPressed(KeyboardKey.P))
                    {
                        if (ball.Velocity.X > 0)
                            ball.IsRightDirection = true;
                        else
                            ball.IsRightDirection = false;
                        gameState = GameStatus.Paused;
                    }
                    break;
                case GameStatus.Paused:
                    if (Raylib.IsKeyPressed(KeyboardKey.C))
                    {
                        ball.ResetBall();
                        gameState = GameStatus.Playing;
                    }
                    if (Raylib.IsKeyPressed(KeyboardKey.T))
                        gameState = GameStatus.End;
                    break;
                case GameStatus.PointEnd:
                    if (Raylib.IsKeyPressed(KeyboardKey.C))
                    {
                        ball.ResetBall();
                        gameState = GameStatus.Playing;
                    }
                    break;
                case GameStatus.End:
                default:
                    break;
            }
        }

        void MoveBall()
        {
            ball.Position += ball.Velocity * deltaTime;

            if (ball.Position.Y <= 1 + ball.Radius || ball.Position.Y >= heightWindow - ball.Radius - 1)
            {
                Raylib.PlaySound(hitBallSound);
                ball.SetVelocitY(ball.Velocity.Y * -1);
            }

            if (ball.Position.X - ball.Radius <= leftRacket.Position.X + leftRacket.Width + 1)
            {
                if (ball.Position.Y + ball.Radius >= leftRacket.Position.Y + 1 && ball.Position.Y - ball.Radius <= leftRacket.Position.Y + leftRacket.Height)
                {
                    Raylib.PlaySound(hitBallSound);
                    ball.ChangeAngle(leftRacket.Position, leftRacket.Height);
                    ball.SetPositionX(leftRacket.Position.X + leftRacket.Width + ball.Radius);
                }
                else if (ball.Position.X - ball.Radius <= 1)
                {
                    rightPlayerScore++;
                    gameState = GameStatus.PointEnd;
                    ball.IsRightDirection = true;
                }
            }

            if (ball.Position.X + ball.Radius >= rightRacket.Position.X + 1)
            {
                if (ball.Position.Y + ball.Radius >= rightRacket.Position.Y + 1 && ball.Position.Y - ball.Radius <= rightRacket.Position.Y + rightRacket.Height)
                {
                    Raylib.PlaySound(hitBallSound);
                    ball.ChangeAngle(rightRacket.Position, rightRacket.Height);
                    ball.SetPositionX(rightRacket.Position.X - ball.Radius);
                }
                else if (ball.Position.X + ball.Radius >= widthWindow - 1)
                {
                    leftPlayerScore++;
                    gameState = GameStatus.PointEnd;
                    ball.IsRightDirection = false;
                }
            }
        }

        void Update()
        {
            switch (gameState)
            {
                case GameStatus.Playing:
                    MoveBall();

                    runningSeconds -= deltaTime;
                    displaySeconds = (int)runningSeconds;
                    if (runningSeconds <= 0 || leftPlayerScore >= 10 || rightPlayerScore >= 10)
                        gameState = GameStatus.End;
                    break;
                default:
                    break;
            }
        }

        void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);
            switch (gameState)
            {
                case GameStatus.Start:
                    Raylib.DrawText("Presiona I para inciar", 10, 10, 42, Color.White);
                    break;
                case GameStatus.Playing:
                    Raylib.DrawCircleV(ball.Position, ball.Radius, Color.White);
                    Raylib.DrawRectangleV(leftRacket.Position, new Vector2(leftRacket.Width, leftRacket.Height), Color.White);
                    Raylib.DrawRectangleV(rightRacket.Position, new Vector2(rightRacket.Width, rightRacket.Height), Color.White);
                    Raylib.DrawLineDashed(new Vector2(widthWindow / 2, 0), new Vector2(widthWindow / 2, heightWindow), 20, 10, Color.White);
                    Raylib.DrawText(displaySeconds.ToString(), (widthWindow / 2) - 27, 0, 54, Color.White);
                    Raylib.DrawText("Jugador 1: " + leftPlayerScore.ToString(), 40, 5, 32, Color.White);
                    Raylib.DrawText("Jugador 2: " + rightPlayerScore.ToString(), 590, 5, 32, Color.White);
                    break;
                case GameStatus.Paused:
                    Raylib.DrawText("Presiona C para continuar", 10, 10, 42, Color.White);
                    Raylib.DrawText("Presiona T para Terminar", 10, 70, 42, Color.White);
                    break;
                case GameStatus.PointEnd:
                    if (rightPlayerScore > leftPlayerScore)
                    {
                        Raylib.DrawText("Punto para jugador 2", 10, 10, 42, Color.White);
                        Raylib.DrawText("Jugador 1: " + leftPlayerScore, 10, 70, 42, Color.White);
                        Raylib.DrawText("Jugador 2: " + rightPlayerScore, 10, 140, 42, Color.White);
                    }
                    else
                    {
                        Raylib.DrawText("Punto para jugador 1", 10, 10, 42, Color.White);
                        Raylib.DrawText("Jugador 1: " + leftPlayerScore, 10, 70, 42, Color.White);
                        Raylib.DrawText("Jugador 2: " + rightPlayerScore, 10, 140, 42, Color.White);
                    }
                    Raylib.DrawText("Presiona C para continuar", 10, 210, 42, Color.White);
                    break;
                case GameStatus.End:
                    Raylib.DrawText("Fin del juego", 10, 10, 42, Color.White);
                    if (rightPlayerScore == leftPlayerScore)
                        Raylib.DrawText("Empate con " + rightPlayerScore.ToString() + " puntos", 10, 70, 36, Color.White);
                    else if (rightPlayerScore > leftPlayerScore)
                        Raylib.DrawText("Gana el jugador 2 con " + rightPlayerScore.ToString() + " puntos", 10, 70, 36, Color.White);
                    else
                        Raylib.DrawText("Gana el jugador 1 con " + leftPlayerScore.ToString() + " puntos", 10, 70, 36, Color.White);
                    break;
                default:
                    break;
            }
            Raylib.EndDrawing();
        }
        Raylib.UnloadSound(hitBallSound);
        Raylib.CloseAudioDevice();
        Raylib.CloseWindow();
    }
}