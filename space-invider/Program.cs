using System;
using System.Collections.Generic;
using System.Threading;

namespace space_invider
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            GameBoard gB = new GameBoard();
            gB.Play();

        }

    }


    class Position
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Position(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }

    class Frame
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public Frame(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        public void CreateFrame()
        {
            for (int i = 0; i < this.Height; i++)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.SetCursorPosition(0, i);
                Console.Write("*");
                Console.SetCursorPosition(this.Width-1, i);
                Console.Write("*");
            }
            for (int j = 0; j < this.Width; j++)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.SetCursorPosition(j, 0);
                Console.Write("*");
                Console.SetCursorPosition(j, this.Height-1);
                Console.Write("*");
            }

        }

    }
    class Ship:Position
    {
        public char SpaceShip { get; set; }

        public Ship(int x, int y) : base(x,y)
        {
            this.SpaceShip = '^';
        }

        public void DrawShip()
        {
            Console.SetCursorPosition(this.X, this.Y);
            Console.Write(this.SpaceShip);
        }

    }

    class Enemy: Position
    {
        public char EnemyCharacter { get; set; }
        public Enemy(int x, int y) : base(x,y)
        {
            this.EnemyCharacter = '$';
        }

        public void MoveEnemy(int height)
        {
            if(this.Y != height -1)
            {
                Console.SetCursorPosition(this.X, this.Y);
                Console.Write(" ");
                this.Y++;

            }

        }

        public void DrawEnemy() 
        {
            Console.SetCursorPosition(this.X, this.Y);
            Console.Write(this.EnemyCharacter);
        }
    }

    class Laser: Position
    {
        public char FireLasor { get; set; }

        public Laser(int x, int y):base(x,y)
        {
            this.FireLasor = '|';
        }

        public void MoveLasor(GameBoard gb)
        {
            // Mozliwosc przyspieszenia strzalu, po obsluzeniu warunku
            while (this.Y != 1) 
            {
                Console.SetCursorPosition(this.X, this.Y);
                Console.Write(this.FireLasor);
                Thread.Sleep(10);
                gb.Draw();
                Console.SetCursorPosition(this.X, this.Y);
                Console.Write(" ");
                this.Y -= 1;


            } 
        }
    }

    class GameBoard
    {
        
        public Ship ship { get; set; }
        public Frame frame { get; set; }
        public Enemy enemy { get; set; }

        public Laser laser { get; set; }

        public int Score { get; set; }

        public GameBoard()
        {
            this.ship = new Ship(13, 18);
            this.frame = new Frame(30, 20);
            Score = 0;
        }

        public void Play() 
        {
            frame.CreateFrame();

            GenerateEnemy();
            while (true)
            {
                
                Draw();
                

                ListenClicks();
                

            }

        }

        public void GenerateEnemy()
        {
            Random rnd = new Random();

            Enemy en = new Enemy(rnd.Next(1, frame.Width-2), 1);


            this.enemy = en;
        }


        public void ListenClicks()
        {
            ConsoleKeyInfo keyPressed = Console.ReadKey(true);

            if ((keyPressed.Key == ConsoleKey.W && ship.Y != 1) || (keyPressed.Key == ConsoleKey.S && ship.Y != frame.Height - 2))
            {
                Console.SetCursorPosition(ship.X, ship.Y);
                Console.Write(" ");
                ship.Y += (keyPressed.Key == ConsoleKey.S) ? 1 : -1;
            }
            if ((keyPressed.Key == ConsoleKey.A && ship.X != 1) || (keyPressed.Key == ConsoleKey.D && ship.X != frame.Width - 2))
            {
                Console.SetCursorPosition(ship.X, ship.Y);
                Console.Write(" ");
                ship.X += (keyPressed.Key == ConsoleKey.D) ? 1 : -1;
            }
            if (keyPressed.Key == ConsoleKey.Spacebar)
            {
                this.laser = new Laser(ship.X, ship.Y-1);
                laser.MoveLasor(this);
            }
            if(enemy!=null)
                this.enemy.MoveEnemy(this.frame.Height);

            
        }

        public void Draw()
        {

            Console.SetCursorPosition(0, frame.Height );
            Console.WriteLine("(" + ship.X + "," + ship.Y + ")");
            Console.WriteLine("Score: {0}", Score);

            for (int y = 1; y < frame.Height; y++)
            {
                for (int x = 1; x < frame.Width; x++)
                {
                    if (enemy != null && ship != null && enemy.X == ship.X && enemy.Y == ship.Y || enemy.Y >= frame.Height - 2)  // losing game
                    {
                        enemy = null;
                        ship = null;
                        Console.SetCursorPosition(0, frame.Height + 2);
                        Console.Write("Game Over!");

                    }
                    if ((laser!=null) &&(enemy !=null)&& (enemy.X==laser.X) && (enemy.Y==laser.Y)) // killing enemy
                    {
                        enemy = null;
                        laser = null;
                        Score++;
                       
                    }
                    if((enemy!=null) && (x == enemy.X) && (y == enemy.Y))
                    {
                        enemy.DrawEnemy();
                    }
                    if (ship!=null && x == ship.X && y == ship.Y)
                    {
                        ship.DrawShip();
                    }
                    if (enemy == null)
                    {
                       GenerateEnemy();
                    }



                }
            }
        }
    }

    

}
