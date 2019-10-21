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
                Console.SetCursorPosition(0, i);
                Console.Write("*");
                Console.SetCursorPosition(this.Width-1, i);
                Console.Write("*");
            }
            for (int j = 0; j < this.Width; j++)
            {
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
            if(this.Y != height -2)
            {
                this.Y++;

            }

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
                this.Y -= 1;
                Console.SetCursorPosition(this.X, this.Y);
                Console.Write(this.FireLasor);              
                Thread.Sleep(10);
                gb.Draw();

            }
        }
    }

    class GameBoard
    {
        
        public Ship ship { get; set; }
        public Frame frame { get; set; }

        public Enemy enemy { get; set; }

        public GameBoard()
        {
            this.ship = new Ship(13, 18);
            this.frame = new Frame(30, 20);
        }

        public void Play() 
        {
            GenerateEnemy();
            while (true)
            {
                Draw();

                ListenClicks();


            }

        }

        public void GenerateEnemy()
        {
            Enemy en = new Enemy(1, 1);

            this.enemy = en;
        }


        public void ListenClicks()
        {
            ConsoleKeyInfo keyPressed = Console.ReadKey(true);

            if ((keyPressed.Key == ConsoleKey.W && ship.Y != 1) || (keyPressed.Key == ConsoleKey.S && ship.Y != frame.Height - 2))
            {
                ship.Y += (keyPressed.Key == ConsoleKey.S) ? 1 : -1;
            }
            if ((keyPressed.Key == ConsoleKey.A && ship.X != 1) || (keyPressed.Key == ConsoleKey.D && ship.X != frame.Width - 2))
            {
                ship.X += (keyPressed.Key == ConsoleKey.D) ? 1 : -1;
            }
            if (keyPressed.Key == ConsoleKey.Spacebar)
            {

                Laser laser = new Laser(ship.X, ship.Y);
                laser.MoveLasor(this);
            }

            this.enemy.MoveEnemy(this.frame.Height);
        }

        public void Draw()
        {
            Console.Clear();
            frame.CreateFrame();

            Console.WriteLine("(" + ship.X + "," + ship.Y + ")");

            for (int y = 1; y < frame.Height; y++)
            {
                for (int x = 1; x < frame.Width; x++)
                {
                    if(x == enemy.X && y == enemy.Y)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(enemy.EnemyCharacter);
                    }
                    else if (x == ship.X && y == ship.Y)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(ship.SpaceShip);
                    }



                }
            }
        }
    }

    

}
