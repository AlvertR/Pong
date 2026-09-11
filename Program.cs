/// codigo de chatgpt, se acabron los token asta aqui :(
using Raylib_cs;

const int heigthWindow = 450, widthWindow = 800;

Raylib.InitWindow(widthWindow, heigthWindow, "Pong");
Raylib.SetTargetFPS(60);

const int racketHeight = 100, racketWidth = 30, leftRacketX = 40, rightRacketX = widthWindow - racketWidth - leftRacketX, ballRadius = 30;
int leftRacketY = 10;
int rightRacketY = 10;
int ballX = 400;
int ballY = 225;
bool ballIsDown = true;
bool ballIsRight = true;
int ballSpeed = 2;
int leftPlayerScore = 0;
int rightPlayerScore = 0;
bool playing = false;
bool start = false;
bool end = false;

while (!Raylib.WindowShouldClose())
{
    if (start)
    {
        if (playing)
        {
            CheckKey();
            MoveBall();
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);
            Raylib.DrawCircle(ballX, ballY, ballRadius, Color.White);
            Raylib.DrawRectangle(leftRacketX, leftRacketY, racketWidth, racketHeight, Color.White);
            Raylib.DrawRectangle(rightRacketX, rightRacketY, racketWidth, racketHeight, Color.White);
            Raylib.DrawText("Jugador 1: " + leftPlayerScore.ToString(), leftRacketX, 5, 24, Color.White);
            Raylib.DrawText("Jugador 2: " + rightPlayerScore.ToString(), rightRacketX, 5, 24, Color.White);
            Raylib.EndDrawing();
        }
        else if (end)
        {
            //Raylib.EndDrawing();
            EndGame();
        }
        else
        {
            ContinueGame();
        }
    }
    else
        StartGame();
}

void StartGame()
{
    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.Black);
    Raylib.DrawText("Presion I para inciar", widthWindow / 4, heigthWindow / 2, 42, Color.White);
    if (Raylib.IsKeyPressed(KeyboardKey.I))
    {
        playing = true;
        start = true;
    }
    Raylib.EndDrawing();
}

void ContinueGame()
{
    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.Black);
    Raylib.DrawText("Presion C para continuar", widthWindow / 5, heigthWindow / 2, 42, Color.White);
    Raylib.DrawText("Presion E para Terminar", widthWindow / 4, (heigthWindow / 2) + 60, 42, Color.White);
    if (Raylib.IsKeyPressed(KeyboardKey.C))
    {
        ballX = 400;
        playing = true;
    }
    if (Raylib.IsKeyPressed(KeyboardKey.E))
    {
        end = true;
        Raylib.EndDrawing();
        //EndGame();
    }
    Raylib.EndDrawing();
}

void EndGame()
{
    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.Black);
    if (rightPlayerScore == leftPlayerScore)
        Raylib.DrawText("Empate, " + rightPlayerScore.ToString() + " puntos", 0, heigthWindow / 2, 36, Color.White);
    else if (rightPlayerScore > leftPlayerScore)
        Raylib.DrawText("Gana el jugador 2 con " + rightPlayerScore.ToString() + " puntos", 0, heigthWindow / 2, 36, Color.White);
    else
        Raylib.DrawText("Gana el jugador 1 con " + leftPlayerScore.ToString() + " puntos", 0, heigthWindow / 2, 36, Color.White);
    Raylib.EndDrawing();
}

void CheckKey()
{
    if (Raylib.IsKeyDown(KeyboardKey.Up) && rightRacketY > 0)
        rightRacketY -= 10;
    if (Raylib.IsKeyDown(KeyboardKey.Down) && rightRacketY < heigthWindow - racketHeight - 1)
        rightRacketY += 10;

    if (Raylib.IsKeyDown(KeyboardKey.W) && leftRacketY > 0)
        leftRacketY -= 10;
    if (Raylib.IsKeyDown(KeyboardKey.S) && leftRacketY < heigthWindow - racketHeight - 1)
        leftRacketY += 10;
}

void MoveBall()
{
    if (ballIsDown)
        ballY += ballSpeed;
    else ballY -= ballSpeed;

    if (ballIsRight)
        ballX += ballSpeed;
    else ballX -= ballSpeed;

    if (ballY <= 1 + ballRadius || ballY >= heigthWindow - ballRadius - 1)
        ballIsDown = !ballIsDown;

    //if (ballX <= 1 + ballRadius || ballX >= widthWindow - ballRadius - 1)
    //    ballIsRight = !ballIsRight;

    if (ballX <= leftRacketX + racketWidth + 1)
    {
        if (ballY >= leftRacketY + 1 && ballY <= leftRacketY + racketHeight)
            ballIsRight = !ballIsRight;
        else if (ballX <= 1)
        {
            rightPlayerScore++;
            //ballX = 400;
            playing = false;
            Raylib.EndDrawing();
            //ContinueGame();
        }
    }

    if (ballX >= rightRacketX + 1)
    {
        if (ballY >= rightRacketY + 1 && ballY <= rightRacketY + racketHeight)
            ballIsRight = !ballIsRight;
        else if (ballX >= widthWindow - 1)
        {
            leftPlayerScore++;
            //ballX = 400;
            playing = false;
            Raylib.EndDrawing();
            //ContinueGame();
        }
    }
}

Raylib.CloseWindow();
