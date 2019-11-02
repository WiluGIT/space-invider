using System;
using System.Collections.Generic;
using System.Linq;
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

        public Enemy(int x, int y) : base(x,y)
        {
            this.EnemyCharacter = '$';

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
        public  Enemy[] enemy { get; set; }
        public Laser laser { get; set; }


        public bool isRunning { get; set; }

        private Thread enemyThread;
        public int Score { get; set; }

        public volatile bool isPlaying;

        private readonly object gameLock = new object();


        public int enemyY { get; set; }

        public GameBoard()
        {
            this.ship = new Ship(13, 18);
            this.frame = new Frame(30, 20);
            this.Score = 0;
            this.isPlaying = true;

            this.isRunning = true;
            this.enemyY = 1;
        }

        public void Play() 
        {

            frame.CreateFrame();
            GenerateEnemy();
            this.ship.DrawShip();
            this.MoveEnemy();
            this.ship.MoveShip(this, this.frame);
            while (this.isPlaying) 
            {
                bool enemyExist= Array.Exists(this.enemy, element => element != null);

                if (!enemyExist) // all enemies are dead
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    this.isRunning = false;
                    this.isPlaying = false;
                }
                for (int i = 0; i < this.enemy.Length; i++)
                {
                    if (this.enemy[i] != null)  // losing game
                    {

                        if (ship != null && this.enemy[i].X == ship.X && this.enemy[i].Y == ship.Y || this.enemy[i].Y >= frame.Height - 2)
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            this.isRunning = false;
                            this.isPlaying = false;
                            this.enemy[i] = null;
                            ship = null;


                        }



                    }
                    if ((laser != null) && (this.enemy[i] != null) && (this.enemy[i].X == laser.X) && (this.enemy[i].Y == laser.Y)) // killing enemy
                    {
                        this.enemy[i] = null;
                        laser = null;
                        Score++;

                    }

                }



            }
            Thread.Sleep(1000);
            Console.ResetColor();
            Console.SetCursorPosition(0, frame.Height + 2);
            Console.WriteLine("Game Over!");
            Console.WriteLine("Wcisnij dowolny przycisk, aby przejsc do menu glownego");
            Console.ReadKey(true);
            
            Console.Clear();



        }


        public void MoveEnemy()
        {
            this.enemyThread = new Thread(t =>
            {

                for (int i = 0; i < frame.Height - 1; i++)
                {
                    if (this.isRunning == false)
                        break;
                    DrawEnemy();
                    this.enemyY++;

                }

            })
            {
                IsBackground = true
            };
            this.enemyThread.Start();


        }

        public void DrawEnemy()
        {
            /*
             
                         Console.SetCursorPosition(this.enemy[j].X, this.enemy[j].Y);
            Console.ForegroundColor = ConsoleColor.Red;

            Console.Write(this.enemy[j].EnemyCharacter);
            Thread.Sleep(400);
            Console.SetCursorPosition(this.enemy[j].X, this.enemy[j].Y);
            Console.Write(" ");
            this.enemy[j].Y++; ;
            if (this.isRunning == false)
                break;

             */

            for (int i = 0; i < this.enemy.Length; i++)
            {
                if (this.enemy[i]!= null)
                {
                    Console.SetCursorPosition(this.enemy[i].X, this.enemy[i].Y);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(this.enemy[i].EnemyCharacter);
                }
            }
            Thread.Sleep(800);
            for (int j = 1; j < frame.Width-1; j++)
            {
                Console.SetCursorPosition(j, this.enemyY);
                Console.Write(" ");
            }

            for (int i = 0; i < this.enemy.Length; i++)
            {
                if (this.enemy[i] != null)
                {
                    this.enemy[i].Y++;
                }

            }

        }

        public void GenerateEnemy()
        {
            int enemyCount = (int)Math.Floor(frame.Width * 0.5);

            this.enemy = new Enemy[enemyCount];

            Random rnd = new Random();

            int frameX = rnd.Next(1, frame.Width - enemyCount);

            for(int i=0;i<this.enemy.Length;i++)
            {
                this.enemy[i] = new Enemy(frameX,1);
                frameX++;
            }

        }



    }
    

    

}
