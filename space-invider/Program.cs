using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace space_invider
{
    class Program
    {
        private static string userAction;

        private static string userActionFromGame;

        private static bool isGameFinished = false;

        private static bool isStayInMenu=true;

        private static bool isStayInMenuGame = true;

        private static bool isAnimating = true;

        private static Thread shipAnimationThread;
        static void Main(string[] args)
        {
            Console.SetWindowSize(97,25);
            Console.SetCursorPosition(0, 200);

            Console.CursorVisible = false;

            GameBoard gB = new GameBoard();
            gB.ReadFromFile();

            showMenu(out userAction);

            do
            {
                switch (userAction)
                {
                    case "0":
                        isAnimating = false;
                        Console.Clear();
                        gB.Play(out isGameFinished);

                        do {
                            if (isGameFinished == true)
                            {
                                gB.showSecondMenu(out userActionFromGame, isGameFinished);
                                switch (userActionFromGame)
                                {
                                    case "0":
                                        isGameFinished = false;
                                        Console.Clear();
                                        gB.Play(out isGameFinished);
                                        break;
                                    case "1":
                                        isStayInMenuGame=false;
                                        Console.Clear();
                                        break;
                                }

                            }
                            else if (isGameFinished == false)
                            {
                                gB.showSecondMenu(out userActionFromGame, isGameFinished);
                                switch (userActionFromGame)
                                {
                                    case "0":
                                        Console.Clear();
                                        gB.Play(out isGameFinished);
                                        break;
                                    case "1":
                                        Console.Clear();
                                        isStayInMenuGame = false;
                                        break;
                                }
                            }
                        } while (isStayInMenuGame);
                        
                        

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
                        Console.SetCursorPosition(22, top + 6);
                        Console.Write("To sa cukierki za, ktore masz dodatkowe punkty:\n");
                        Console.SetCursorPosition(22, top + 7);
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.Write("o");
                        Console.ResetColor();
                        Console.SetCursorPosition(22, top + 8);
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write("Poruszaj sie strzalkami i strzelaj za pomoca entera. Powodzenia!\n");
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
            
            var menu = new Menu(new string[] { "1. Zagraj w gre!\n", "2. Pokaz instrukcje\n", "3. Wyjdz z gry\n" });

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

            Console.SetCursorPosition(0, 0);


        }

    }


    class Menu
    {
        public IReadOnlyList<string> Items { get; set; }
        public int SelectedIndex { get; private set; } = 0;
        public string SelctedOption => SelectedIndex != 0 ? this.Items[SelectedIndex] : null;

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
            char enter = '\x2190';
            string title = @" 
 ________  ________  ________  ___  __    _______  _________  _________  _______   ________         
|\   __  \|\   __  \|\   ____\|\  \|\  \ |\  ___ \|\___   ___\\___   ___\\  ___ \ |\   __  \        
\ \  \|\  \ \  \|\  \ \  \___|\ \  \/  /|\ \   __/\|___ \  \_\|___ \  \_\ \   __/|\ \  \|\  \       
 \ \   _  _\ \  \\\  \ \  \    \ \   ___  \ \  \_|/__  \ \  \     \ \  \ \ \  \_|/_\ \   _  _\      
  \ \  \\  \\ \  \\\  \ \  \____\ \  \\ \  \ \  \_|\ \  \ \  \     \ \  \ \ \  \_|\ \ \  \\  \     
   \ \__\\ _\\ \_______\ \_______\ \__\\ \__\ \_______\  \ \__\     \ \__\ \ \_______\ \__\\ _\     
    \|__|\|__|\|_______|\|_______|\|__| \|__|\|_______|   \|__|      \|__|  \|_______|\|__|\|__|    
                                                                                                    
                                                                                                    
                                                                                                    ";


            Console.SetCursorPosition(0, 0);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(title);
            Console.ResetColor();
            Console.SetCursorPosition(0, 15);
            Console.Write("Wybierz opcje za pomoca strzalek {0} {1}, a nastepnie zatwierdz enterem {2}", up,down, enter);
            Console.SetCursorPosition(0, 10);

            switch (SelectedIndex)
            {

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

        public void DrawSecondMenu()
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            char down = '\x2193';
            char up = '\x2191';
            char enter = '\x2190';
            string title = @" 
 ________  ________  ________  ___  __    _______  _________  _________  _______   ________         
|\   __  \|\   __  \|\   ____\|\  \|\  \ |\  ___ \|\___   ___\\___   ___\\  ___ \ |\   __  \        
\ \  \|\  \ \  \|\  \ \  \___|\ \  \/  /|\ \   __/\|___ \  \_\|___ \  \_\ \   __/|\ \  \|\  \       
 \ \   _  _\ \  \\\  \ \  \    \ \   ___  \ \  \_|/__  \ \  \     \ \  \ \ \  \_|/_\ \   _  _\      
  \ \  \\  \\ \  \\\  \ \  \____\ \  \\ \  \ \  \_|\ \  \ \  \     \ \  \ \ \  \_|\ \ \  \\  \     
   \ \__\\ _\\ \_______\ \_______\ \__\\ \__\ \_______\  \ \__\     \ \__\ \ \_______\ \__\\ _\     
    \|__|\|__|\|_______|\|_______|\|__| \|__|\|_______|   \|__|      \|__|  \|_______|\|__|\|__|    
                                                                                                    
                                                                                                    
                                                                                                    ";


            Console.SetCursorPosition(0, 0);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(title);
            Console.ResetColor();
            Console.SetCursorPosition(0, 15);
            Console.Write("Wybierz opcje za pomoca strzalek {0} {1}, a nastepnie zatwierdz enterem {2}", up, down, enter);
            Console.SetCursorPosition(0, 10);
            switch (SelectedIndex)
            {

                case 0:
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write(Items[0]);
                    Console.ResetColor();
                    Console.Write(Items[1]);
                    break;
                case 1:
                    Console.Write(Items[0]);
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write(Items[1]);
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
                Console.SetCursorPosition((Console.WindowWidth/2)-5, i);
                Console.Write("*");
                Console.SetCursorPosition((Console.WindowWidth / 2) - 5 +this.Width-1, i);
                Console.Write("*");
            }
            for (int j = (Console.WindowWidth / 2) - 5; j < (Console.WindowWidth / 2) - 5 + this.Width; j++)
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
                    if ((keyPressed.Key == ConsoleKey.LeftArrow && this.X != gb.xMove+1) || (keyPressed.Key == ConsoleKey.RightArrow && this.X != gb.xMove + frame.Width - 2))
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

    class Candy : Position
    {
        public char CandyLook { get; set; }

        public Candy(int x, int y): base(x,y)
        {
            this.CandyLook = 'o';
        }
        public void DrawCandy()
        {
            Console.SetCursorPosition(this.X, this.Y);
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(this.CandyLook);
        }
        public void ClearCandy()
        {
            Console.SetCursorPosition(this.X, this.Y);
            Console.ResetColor();
            Console.Write(" ");
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
        public Frame[] frame { get; set; }
        public Enemy[] enemy { get; set; }
        public Laser laser { get; set; }
        public Candy candy { get; set; }

        public int xMove { get; set; } = (Console.WindowWidth / 2) - 5;

        public bool isRunning { get; set; }

        private Thread enemyThread;
        public int Score { get; set; } = 0;

        public int GameBoardIndex { get; set; } = 0;

        public volatile bool isPlaying;

        public int BoardCounter { get; set; } = 0;
        public int enemyY { get; set; }

        public bool IndexChanged { get; set; } = false;


        public void Play(out bool isGameFinished) 
        {
            this.ship = new Ship(this.xMove +this.frame[this.GameBoardIndex].Width - 2, this.frame[this.GameBoardIndex].Height - 2);
            SpawnCandy();
            this.isPlaying = true;
            this.isRunning = true;
            this.IndexChanged = false;
            this.enemyY = 1;

            this.frame[GameBoardIndex].CreateFrame();
            GenerateEnemy();
            this.ship.DrawShip();
            this.MoveEnemy();
            this.ship.MoveShip(this, this.frame[GameBoardIndex]);

            while (this.isPlaying)
            {

                bool enemyExist = Array.Exists(this.enemy, element => element != null);


                if (!enemyExist) // all enemies are dead
                {
                    if (this.GameBoardIndex < this.frame.Length - 1)
                    {
                        this.GameBoardIndex++;
                        this.IndexChanged = true;
                    }


                    if(this.BoardCounter<=this.frame.Length)
                        this.BoardCounter++;

                    this.isRunning = false;
                    this.isPlaying = false;

                }
                if ((candy != null) && (this.ship != null) && (this.ship.X == candy.X) && (this.ship.Y == candy.Y)) // getting candy
                {
                    this.candy.ClearCandy();
                    this.candy = null;
                    Score+=20;

                }
                for (int i = 0; i < this.enemy.Length; i++)
                {


                    if (this.enemy[i] != null)  // losing game
                    {

                        if (ship != null && this.enemy[i].X == ship.X && this.enemy[i].Y == ship.Y || this.enemy[i].Y >= frame[this.GameBoardIndex].Height - 2)
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
            isGameFinished = false;

            if (this.BoardCounter>=this.frame.Length)
            {
                Thread.Sleep(1000);
                Console.ResetColor();
                Console.SetCursorPosition((Console.WindowWidth / 2) - 5, frame[GameBoardIndex].Height);
                Console.WriteLine("Gratulacje, ukonczyles wszystkie poziomy!");
                Console.WriteLine("Twoj wynik: {0}", this.Score);
                Console.WriteLine("Wcisnij enter, aby otworzyc menu");
                Console.ReadKey(true);
                isGameFinished = true;
                this.GameBoardIndex = 0;
                this.BoardCounter = 0;
                this.Score = 0;
                this.enemyY = 1;
                Console.Clear();
            }
            else
            {
                Thread.Sleep(1000);
                Console.ResetColor();
                if(this.IndexChanged)
                {
                    Console.SetCursorPosition((Console.WindowWidth / 2) - 5, frame[GameBoardIndex - 1].Height);
                }
                else 
                {
                    Console.SetCursorPosition((Console.WindowWidth / 2) - 5, frame[GameBoardIndex].Height);
                }
                Console.WriteLine("Twoj wynik: {0}", this.Score);
                Console.WriteLine("Wcisnij enter, aby otworzyc menu");
                this.candy = null;
                this.enemyY = 1;
                Console.ReadKey(true);

                Console.Clear();

            }






        }


        public void showSecondMenu(out string userActionFromGame,bool isGameFinished)
        {
            var menu1 = new Menu(new string[] { "1. Zagraj ponownie!\n",  "2. Wyjdz\n" });
            var menu2 = new Menu(new string[] { "1. Zagraj nastepna mape!\n", "2. Wyjdz\n" });

            bool done = false;
            if (isGameFinished)
            {
                menu1.DrawSecondMenu();
                do
                {

                    ConsoleKeyInfo keyInfo = Console.ReadKey();
                    Console.Clear();
                    switch (keyInfo.Key)
                    {
                        case ConsoleKey.UpArrow:
                            menu1.MoveUp();
                            break;
                        case ConsoleKey.DownArrow:
                            menu1.MoveDown();
                            break;
                        case ConsoleKey.Enter:
                            done = true;
                            break;
                    }
                    menu1.DrawSecondMenu();
                }
                while (!done);

                userActionFromGame = menu1.SelectedIndex.ToString();
            }
            else 
            {
                menu2.DrawSecondMenu();
                do
                {

                    ConsoleKeyInfo keyInfo = Console.ReadKey();
                    Console.Clear();
                    switch (keyInfo.Key)
                    {
                        case ConsoleKey.UpArrow:
                            menu2.MoveUp();
                            break;
                        case ConsoleKey.DownArrow:
                            menu2.MoveDown();
                            break;
                        case ConsoleKey.Enter:
                            done = true;
                            break;
                    }
                    menu2.DrawSecondMenu();
                }
                while (!done);

                userActionFromGame = menu2.SelectedIndex.ToString();
            }
           


        }

        public void MoveEnemy()
        {
            this.enemyThread = new Thread(t =>
            {

                for (int i = 0; i < frame[this.GameBoardIndex].Height - 1; i++)
                {

                    if (this.isRunning == false)
                    {

                        break;
                    }
                        
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

            Console.SetCursorPosition(this.xMove, this.frame[this.GameBoardIndex].Height);
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write("Wynik: {0}", this.Score);
            Console.ResetColor();
            for (int i = 0; i < this.enemy.Length; i++)
            {
                if (this.enemy[i]!= null)
                {
                    Console.SetCursorPosition(this.enemy[i].X, this.enemy[i].Y);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(this.enemy[i].EnemyCharacter);
                }
            }

            if (this.candy == null)
            {
                SpawnCandy();

            }

            Thread.Sleep(800);

            if (this.IndexChanged)
            {
                for (int j = this.xMove+1; j < this.xMove+frame[this.GameBoardIndex-1].Width - 1; j++)
                {
                    Console.SetCursorPosition(j, this.enemyY);
                    Console.Write(" ");
                }
            }
            else
            {
                for (int j = this.xMove+1; j < this.xMove + frame[this.GameBoardIndex].Width - 1; j++)
                {
                    Console.SetCursorPosition(j, this.enemyY);
                    Console.Write(" ");
                }
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
            int enemyCount = (int)Math.Floor(frame[this.GameBoardIndex].Width * 0.5);

            this.enemy = new Enemy[enemyCount];

            Random rnd = new Random();

            int frameX = rnd.Next(this.xMove+1, this.xMove + frame[this.GameBoardIndex].Width - enemyCount);

            for(int i=0;i<this.enemy.Length;i++)
            {
                this.enemy[i] = new Enemy(frameX,1);
                frameX++;
            }

        }

        public void SpawnCandy()
        {
            Random rnd = new Random();

            int frameX = rnd.Next(this.xMove+1, this.xMove+ this.frame[this.GameBoardIndex].Width-2);
            int frameY = rnd.Next(this.enemyY, this.frame[this.GameBoardIndex].Height-1);

            int spawnOrNo = 0;

            int random = rnd.Next(0, 3);

            if (this.candy == null && this.enemyY < frameY && frameY<this.frame[this.GameBoardIndex].Height-1)
            {
                if (random == spawnOrNo)
                {
                    Candy candy = new Candy(frameX, frameY);
                    this.candy = candy;
                    this.candy.DrawCandy();
                }

            }



        }

        public void ReadFromFile()
        {
            try
            {
                StreamReader file = new StreamReader(@"./input.txt");
                string line;
                string[] buffor;
                int counter = 0;
                while ((line = file.ReadLine()) != null)
                {
                    counter++;
                }

                file.DiscardBufferedData();
                file.BaseStream.Seek(0, SeekOrigin.Begin);

                this.frame = new Frame[counter];
                counter = 0;

                while ((line = file.ReadLine()) != null)
                {
                    buffor = line.Split(",");
                    this.frame[counter] = new Frame(int.Parse(buffor[0]), int.Parse(buffor[1]));


                    counter++;
                }
                file.Close();
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            

        }



    }
    

    

}
