using System;
using System.Collections.Generic;

namespace space_invider
{
    class Program
    {
        static void Main(string[] args)
        {

            Ship ship = new Ship(13, 18);
            Frame frame = new Frame(15, 20);


            /*
                        while (true)
            {
                Draw(ship,frame);

                ConsoleKeyInfo keyPressed = Console.ReadKey();

                if ((keyPressed.Key == ConsoleKey.W && ship.Y != 1) || (keyPressed.Key == ConsoleKey.S && ship.Y != frame.Height-2))
                {
                    ship.Y += (keyPressed.Key == ConsoleKey.S) ? 1 : -1;
                }
                if ((keyPressed.Key == ConsoleKey.A && ship.X != 2) || (keyPressed.Key == ConsoleKey.D && ship.X != frame.Width - 1))
                {
                    ship.Y += (keyPressed.Key == ConsoleKey.D) ? 1 : -1;
                }
                
            }

             */

            frame.CreateFrame();
            Console.Clear();






        }

        static void Draw(Ship ship,Frame frame)
        {
            Console.Clear();
            frame.CreateFrame();

            Console.WriteLine("(" + ship.X +"," + ship.Y +")");

            for (int y = 1; y < frame.Height; y++)
            {
                for(int x =1; x<frame.Width;x++)
                {
                    if (x == ship.X && y == ship.Y)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(ship.SpaceShip);
                    }
                        
                    
                }
            }


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

    class GameBoard
    {
    }

    

}
