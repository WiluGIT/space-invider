using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;

namespace space_invider
{
    class Program
    {
        private static string userAction;

        private static bool isStayInMenu=true;

        private static bool isAnimating = true;

        private static Thread shipAnimationThread;
        static void Main(string[] args)
        {
            Console.CursorVisible = false;

            GameBoard gB;

            showMenu(out userAction);

            do
            {
                switch (userAction)
                {
                    case "-1":
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("\nUzyj strzalek!");
                        Console.ReadLine();
                        Console.ResetColor();
                        Console.Clear();
                        showMenu(out userAction);
                        break;
                    case "0":
                        isAnimating = false;
                        Console.Clear();
                        gB = new GameBoard();
                        gB.Play();
                        Console.Clear();
                        showMenu(out userAction);
                        break;
                    case "1":
                        Console.Clear();
                        Console.WriteLine("Wybrales opcje 2");
                        isAnimating = true;
                        animateShip();
                        Console.ReadLine();
                        isAnimating = false;
                        Thread.Sleep(10);
                        Console.Clear();
                        showMenu(out userAction);
                        break;
                    case "2":
                        isAnimating = false;
                        isStayInMenu = false;
                        Console.Clear();
                        break;
                }
            } while (isStayInMenu);


        }

        private static void animateShip()
        {
            Frame frame = new Frame(70,10);

            shipAnimationThread = new Thread(t =>
            {
                while (isAnimating)
                {

                    Image image = Image.FromFile(@"C:\Users\wolfr\Desktop\gif1.gif");
                    FrameDimension dimension = new FrameDimension(image.FrameDimensionsList[0]);
                    int frameCount = image.GetFrameCount(dimension);
                    StringBuilder sb;

                    int left = Console.WindowLeft, top = Console.WindowTop;

                    char[] chars = { '#', '#', '@', '%', '=', '+', '*', ':', '-', '.', ' ' };
                    
                    for (int i = 0; ; i = (i + 1) % frameCount)
                    {
                        if (isAnimating == false)
                            break;
                        sb = new StringBuilder();
                        image.SelectActiveFrame(dimension, i);
                        Console.SetCursorPosition(20, top);
                        frame.CreateAnimationFrame();

                        Console.SetCursorPosition(22, top+1);
                        Console.Write("Cel gry: Zlikwiduj wszystkich kosmitow zanim oni dopadna ciebie! \n");
                        Console.SetCursorPosition(22, top+2);
                        Console.Write("To jest twoj statek:\n");
                        Console.SetCursorPosition(22, top + 3);
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write("^");
                        Console.ResetColor();
                        Console.SetCursorPosition(22, top + 4);
                        Console.Write("To jest twoj przeciwnik:\n");          
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.SetCursorPosition(22, top + 5);
                        Console.Write("$");
                        Console.ResetColor();
                        for (int h = 0; h < image.Height; h++)
                        {
                            for (int w = 0; w < image.Width; w++)
                            {
                                Color cl = ((Bitmap)image).GetPixel(w, h);
                                int gray = (cl.R + cl.R + cl.B) / 3;
                                int index = (gray * (chars.Length - 1)) / 255;

                                sb.Append(chars[index]);
                            }
                            sb.Append('\n');
                        }
                       
                        Console.SetCursorPosition(left, top);
                        Console.Write(sb.ToString());

                        System.Threading.Thread.Sleep(100);
                    }

                }
                

            })
            {
                IsBackground = true
            };
            shipAnimationThread.Start();
           
        }

        private static void showMenu(out string userAction)
        {
            
            var menu = new Menu(new string[] { "1. Play Game!\n", "2. Show Instruction\n", "3. Exit Game\n" });

            bool done = false;
            menu.DrawMenu();
            do
            {

                ConsoleKeyInfo keyInfo  = Console.ReadKey();
                Console.Clear();
                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        menu.MoveUp();
                        break;
                    case ConsoleKey.DownArrow:
                        menu.MoveDown();
                        break;
                    case ConsoleKey.Enter:
                        done = true;
                        break;
                }
                menu.DrawMenu();
            }
            while (!done);

            userAction = menu.SelectedIndex.ToString();
            

           

        }

    }


    class Menu
    {
        public IReadOnlyList<string> Items { get; set; }
        public int SelectedIndex { get; private set; } = -1;
        public string SelctedOption => SelectedIndex != -1 ? this.Items[SelectedIndex] : null;

        public void MoveUp()
        {
            this.SelectedIndex = Math.Max(SelectedIndex - 1, 0);
        }

        public void MoveDown()
        {
            this.SelectedIndex = Math.Min(SelectedIndex + 1, Items.Count - 1);
        }

        public void DrawMenu()
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            char down = '\x2193';
            char up = '\x2191';
            Console.SetCursorPosition(0, 6);
            Console.Write("Wybierz opcje za pomoca strzalek {0} {1}", up,down);
            Console.SetCursorPosition(0, 0);
            switch (SelectedIndex)
            {
                case -1:
                    Console.Write(Items[0]);
                    Console.Write(Items[1]);
                    Console.Write(Items[2]);
                    break;
                case 0:
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write(Items[0]);
                    Console.ResetColor();
                    Console.Write(Items[1]);
                    Console.Write(Items[2]);
                    break;
                case 1:
                    Console.Write(Items[0]);
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write(Items[1]);
                    Console.ResetColor();
                    Console.Write(Items[2]);
                    break;
                case 2:
                    Console.Write(Items[0]);
                    Console.Write(Items[1]);
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write(Items[2]);
                    Console.ResetColor();
                    break;
            }

        }

        public Menu(IEnumerable<string> items)
        {
            this.Items = items.ToArray();
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

        public void CreateAnimationFrame()
        {
            for (int i = 0; i < this.Height; i++)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.SetCursorPosition(20, i);
                Console.Write("\x058E");
                Console.SetCursorPosition(this.Width + 19, i);
                Console.Write("\x058E");
            }
            for (int j = 20; j < this.Width+20; j++)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.SetCursorPosition(j, 0);
                Console.Write("\x058E");
                Console.SetCursorPosition(j, this.Height - 1);
                Console.Write("\x058E");
            }
            Console.ResetColor();
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

                    if ((keyPressed.Key == ConsoleKey.UpArrow && this.Y != 1) || (keyPressed.Key == ConsoleKey.DownArrow && this.Y != frame.Height - 2))
                    {
                        Console.SetCursorPosition(this.X, this.Y);
                        Console.Write(" ");
                        this.Y += (keyPressed.Key == ConsoleKey.DownArrow) ? 1 : -1;
                        this.DrawShip();
                    }
                    if ((keyPressed.Key == ConsoleKey.LeftArrow && this.X != 1) || (keyPressed.Key == ConsoleKey.RightArrow && this.X != frame.Width - 2))
                    {
                        Console.SetCursorPosition(this.X, this.Y);
                        Console.Write(" ");
                        this.X += (keyPressed.Key == ConsoleKey.RightArrow) ? 1 : -1;
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
                    this.isRunning = false;
                    this.isPlaying = false;
                }
                for (int i = 0; i < this.enemy.Length; i++)
                {
                    if (this.enemy[i] != null)  // losing game
                    {

                        if (ship != null && this.enemy[i].X == ship.X && this.enemy[i].Y == ship.Y || this.enemy[i].Y >= frame.Height - 2)
                        {
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
            Console.SetCursorPosition(0, frame.Height + 1);
            Console.WriteLine("Your score: {0}", this.Score);
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
