using System;
using System.Collections.Generic;

namespace space_invider
{
    class Program
    {
        static void Main(string[] args)
        {
           
            Frame frame = new Frame(30,40);
            frame.CreateFrame();

            frame.DrawFrame();


          
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
    class FrameElement : Position
    {
        public char Frame { get; set; }

        public FrameElement(int y, int x) : base(x,y)
        { 
            this.Frame = '#';
        }
    }
    class Frame
    {
        public List<FrameElement> frameList = new List<FrameElement>();

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
                for (int j = 0; j < this.Width; j++)
                {
                    if (i == 0 || i==this.Height-1 || j==0 || j==this.Width-1)
                    {
                        frameList.Add(new FrameElement(i, j));
                    }
                    
                }
            }
        }

        public void DrawFrame()
        {
            foreach (var fl in frameList)
            {
                Console.SetCursorPosition(fl.X, fl.Y);
                Console.Write(fl.Frame);
            }
        }

    }
    class GameBoard
    {
    }

    

}
