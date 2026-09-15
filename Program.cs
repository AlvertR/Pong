using Raylib_cs;
using System.Numerics;
using System.Timers;

enum GameStatus
{
    Start,
    Playing,
    Continue,
    End
}
public class Program
{

    public Program() { }
    private static System.Timers.Timer aTimer;
    private static int seg = 60;
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

        const int racketHeight = 100, racketWidth = 15, ballRadius = 30;
        Vector2 leftRacket = new Vector2(40, 10);
        Vector2 rightRacket = new Vector2(widthWindow - racketWidth - leftRacket.X, 10);
        Vector2 ballPosition = new Vector2(400, 225);
        int defaultDirection = 160;
        Vector2 ballVelocity = new Vector2(defaultDirection, defaultDirection);
        int maxDirection = 200;
        int minDirection = 140;
        int baseSpeed = 226;
        float deltaTime = 0;
        int leftPlayerScore = 0;
        int rightPlayerScore = 0;
        float racketSpeed = 600;
        GameStatus gameState = GameStatus.Start;
        aTimer = new System.Timers.Timer(1000);
        aTimer.Elapsed += OnTimedEvent;
        aTimer.AutoReset = true;

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
                    if (Raylib.IsKeyDown(KeyboardKey.Up) && rightRacket.Y > 0)
                        rightRacket.Y -= racketSpeed * deltaTime;
                    if (Raylib.IsKeyDown(KeyboardKey.Down) && rightRacket.Y < heightWindow - racketHeight - 1)
                        rightRacket.Y += racketSpeed * deltaTime;

                    if (Raylib.IsKeyDown(KeyboardKey.W) && leftRacket.Y > 0)
                        leftRacket.Y -= racketSpeed * deltaTime;
                    if (Raylib.IsKeyDown(KeyboardKey.S) && leftRacket.Y < heightWindow - racketHeight - 1)
                        leftRacket.Y += racketSpeed * deltaTime;
                    if (Raylib.IsKeyPressed(KeyboardKey.P))
                        gameState = GameStatus.Continue;
                    break;
                case GameStatus.Continue:
                    if (Raylib.IsKeyPressed(KeyboardKey.C))
                    {
                        ballPosition.X = 400;
                        ballPosition.Y = 225;
                        gameState = GameStatus.Playing;
                    }
                    if (Raylib.IsKeyPressed(KeyboardKey.T))
                        gameState = GameStatus.End;
                    break;
                case GameStatus.End:
                default:
                    break;
            }
        }

        void ChangeAngle(Vector2 racket)
        {
            double racketCenter = racket.Y + (racketHeight / 2);
            double impactPoint = (ballPosition.Y - racketCenter) / (racketHeight / 2);
            impactPoint = Math.Max(-1.0, Math.Min(1.0, impactPoint));

            ballVelocity.Y = (int)(impactPoint * maxDirection);
            int directionX = (ballVelocity.X > 0) ? 1 : -1;
            ballVelocity.X = (Math.Abs(ballVelocity.Y) < 3) ? directionX * defaultDirection : directionX * minDirection;
            Vector2 normalize = Vector2.Normalize(ballVelocity);
            ballVelocity = normalize * baseSpeed;
        }

        void MoveBall()
        {
            ballPosition += ballVelocity * deltaTime;

            if (ballPosition.Y <= 1 + ballRadius || ballPosition.Y >= heightWindow - ballRadius - 1)
            {
                Raylib.PlaySound(hitBallSound);
                ballVelocity.Y = ballVelocity.Y * -1;
            }

            if (ballPosition.X - ballRadius <= leftRacket.X + racketWidth + 1)
            {
                if (ballPosition.Y + ballRadius >= leftRacket.Y + 1 && ballPosition.Y - ballRadius <= leftRacket.Y + racketHeight)
                {
                    Raylib.PlaySound(hitBallSound);
                    ChangeAngle(leftRacket);
                    ballPosition.X = leftRacket.X + racketWidth + ballRadius;
                    ballVelocity.X = Math.Abs(ballVelocity.X);
                }
                else if (ballPosition.X - ballRadius <= 1)
                {
                    rightPlayerScore++;
                    ballPosition.X = 400;
                    ballPosition.Y = 225;
                }
            }

            if (ballPosition.X + ballRadius >= rightRacket.X + 1)
            {
                if (ballPosition.Y + ballRadius >= rightRacket.Y + 1 && ballPosition.Y - ballRadius <= rightRacket.Y + racketHeight)
                {
                    Raylib.PlaySound(hitBallSound);
                    ChangeAngle(rightRacket);
                    ballPosition.X = rightRacket.X - ballRadius;
                    ballVelocity.X = -Math.Abs(ballVelocity.X);
                }
                else if (ballPosition.X + ballRadius >= widthWindow - 1)
                {
                    leftPlayerScore++;
                    ballPosition.X = 400;
                    ballPosition.Y = 225;
                }
            }
        }

        void Update()
        {
            switch (gameState)
            {
                case GameStatus.Playing:
                    MoveBall();
                    aTimer.Enabled = true;
                    if (seg == 0 || leftPlayerScore >= 10 || rightPlayerScore >= 10)
                        gameState = GameStatus.End;
                    break;
                case GameStatus.Continue:
                    aTimer.Enabled = false;
                    break;
                case GameStatus.End:
                    aTimer.Enabled = false;
                    aTimer.Stop();
                    aTimer.Dispose();
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
                    Raylib.DrawCircleV(ballPosition, ballRadius, Color.White);
                    Raylib.DrawRectangleV(leftRacket, new Vector2(racketWidth, racketHeight), Color.White);
                    Raylib.DrawRectangleV(rightRacket, new Vector2(racketWidth, racketHeight), Color.White);
                    Raylib.DrawLineDashed(new Vector2(widthWindow / 2, 0), new Vector2(widthWindow / 2, heightWindow), 20, 10, Color.White);
                    Raylib.DrawText(seg.ToString(), (widthWindow / 2) - 27, 0, 54, Color.White);
                    Raylib.DrawText("Jugador 1: " + leftPlayerScore.ToString(), 40, 5, 32, Color.White);
                    Raylib.DrawText("Jugador 2: " + rightPlayerScore.ToString(), 600, 5, 32, Color.White);
                    break;
                case GameStatus.Continue:
                    Raylib.DrawText("Presiona C para continuar", 10, 10, 42, Color.White);
                    Raylib.DrawText("Presiona T para Terminar", 10, 70, 42, Color.White);
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
    private static void OnTimedEvent(Object source, ElapsedEventArgs e)
    {
        seg--;
    }
}