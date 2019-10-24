using System;
using System.Collections.Generic;
using System.Threading;

namespace space_invider
{
    class Program
    {
        private static string userAction;

        private static bool isStayInMenu=true;
        static void Main(string[] args)
        {
            Console.CursorVisible = false;

            GameBoard gB;

            showMenu(out userAction);

            do
            {
                switch (userAction)
                {
                    case "1":
                        Console.Clear();
                        gB = new GameBoard();
                        gB.Play();
                        Console.Clear();
                        showMenu(out userAction);
                        break;
                    case "2":
                        Console.Clear();
                        Console.WriteLine("Wybrales opcje 2");
                        Console.ReadLine();
                        Console.Clear();
                        showMenu(out userAction);
                        break;
                    case "3":
                        isStayInMenu = false;
                        Console.Clear();
                        break;
                    default:
                        Console.WriteLine("Wybrales zla opcje, wybierz ponownie");
                        Console.ReadLine();
                        Console.Clear();
                        showMenu(out userAction);
                        break;
                }
            } while (isStayInMenu);


        }


        private static void showMenu(out string userAction)
        {
            string menu = "1. Play Game!\n2. 2. Show Instruction\n3. Exit Game";
            Console.WriteLine(menu);
            userAction = Console.ReadLine().ToLower();

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
        private Thread thread;
        public bool isRunning { get; set; }

        public Ship(int x, int y) : base(x,y)
        {
            this.SpaceShip = '^';
        }

        public void DrawShip()
        {
            Console.SetCursorPosition(this.X, this.Y);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(this.SpaceShip);
        }

        public void MoveShip(GameBoard gb,Frame frame)
        {
            thread = new Thread(t =>
            {
                while(gb.isPlaying)
                {
                    gb.buttonClicked = false;
                    ConsoleKeyInfo keyPressed = Console.ReadKey(true);

                    if ((keyPressed.Key == ConsoleKey.W && this.Y != 1) || (keyPressed.Key == ConsoleKey.S && this.Y != frame.Height - 2))
                    {
                        Console.SetCursorPosition(this.X, this.Y);
                        Console.Write(" ");
                        this.Y += (keyPressed.Key == ConsoleKey.S) ? 1 : -1;
                        this.DrawShip();
                    }
                    if ((keyPressed.Key == ConsoleKey.A && this.X != 1) || (keyPressed.Key == ConsoleKey.D && this.X != frame.Width - 2))
                    {
                        Console.SetCursorPosition(this.X, this.Y);
                        Console.Write(" ");
                        this.X += (keyPressed.Key == ConsoleKey.D) ? 1 : -1;
                        this.DrawShip();
                    }
                    if (this != null && keyPressed.Key == ConsoleKey.Spacebar)
                    {
                        gb.laser = new Laser(this.X, this.Y - 1);
                        gb.laser.MoveLasor(gb);
                    }
                    gb.buttonClicked = true;
                }

            })
            {
                IsBackground = true
            };
            thread.Start();
        }

    }

    class Enemy: Position
    {
        public char EnemyCharacter { get; set; }
        private Thread thread;
        public bool isRunning { get; set; }
        public Enemy(int x, int y) : base(x,y)
        {
            this.EnemyCharacter = '$';
            this.isRunning = true;
        }

        public void MoveEnemy(int height,GameBoard gb)
        {
            thread = new Thread(t =>
            {

                for(int i=0;i<height-1;i++)
                {

                    Console.SetCursorPosition(this.X, this.Y);
                    Console.ForegroundColor = ConsoleColor.Red;

                    Console.Write(this.EnemyCharacter);
                    Thread.Sleep(400);
                    Console.SetCursorPosition(this.X, this.Y);
                    Console.Write(" ");
                    this.Y++; ;
                    if (this.isRunning == false)
                        break;
                }

            })
            {
                IsBackground = true
            };
            thread.Start();
         

        }

        public void DrawEnemy() 
        {
            Console.SetCursorPosition(this.X, this.Y);
            Console.ForegroundColor = ConsoleColor.Red;
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
            while (this.Y != 0) 
            {
                Console.SetCursorPosition(this.X, this.Y);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(this.FireLasor);
                Thread.Sleep(30);
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

        public bool isPlaying { get; set; }

        public bool buttonClicked { get; set; }

        public GameBoard()
        {
            this.ship = new Ship(13, 18);
            this.frame = new Frame(30, 20);
            this.Score = 0;
            this.isPlaying = true;
            this.buttonClicked = false;

        }

        public void Play() 
        {

            frame.CreateFrame();
            GenerateEnemy();
            this.ship.DrawShip();
            this.enemy.MoveEnemy(frame.Height, this);
            this.ship.MoveShip(this, this.frame);
            while (this.isPlaying) 
            {
                if (enemy == null)
                {
                    GenerateEnemy();
                    this.enemy.MoveEnemy(frame.Height, this);
                }
                if (enemy != null && ship != null && enemy.X == ship.X && enemy.Y == ship.Y || enemy.Y >= frame.Height - 2)  // losing game
                {
                    this.enemy.isRunning = false;
                    this.isPlaying = false;
                    enemy = null;
                    ship = null;
                    Console.SetCursorPosition(0, frame.Height + 2);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Game Over!");
                    Console.WriteLine("Wcisnij dowolny przycisk, aby przejsc do menu glownego");                  
                    Console.ReadKey();
                    


                }
                if ((laser != null) && (enemy != null) && (enemy.X == laser.X) && (enemy.Y == laser.Y)) // killing enemy
                {
                    enemy.isRunning = false;
                    enemy = null;
                    laser = null;
                    Score++;

                }


            }
            Console.Clear();



        }


        public void GenerateEnemy()
        {
            Random rnd = new Random();

            Enemy en = new Enemy(rnd.Next(1, frame.Width-2), 1);


            this.enemy = en;
        }



    }
    

    

}
