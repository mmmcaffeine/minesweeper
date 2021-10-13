﻿using System;
using System.IO;
using Dgt.Minesweeper.Engine;

namespace ConsoleUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var input = File.ReadAllLines(@".\Minefield.txt");
            
            var generator = new MinefieldGenerator();
            var minefield = generator.GenerateMinefield(input);
            
            var renderer = new MinefieldRenderer();
            var output = renderer.Render(minefield);

            foreach (var line in output)
            {
                Console.WriteLine(line);
            }
            
            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }
    }
}