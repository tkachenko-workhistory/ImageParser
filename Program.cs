﻿using System;
using System.IO;

namespace ImageParser
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var parser = new ImageParser();
            string imageInfoJson;

            using (var file = new FileStream("image_little.bmp", FileMode.Open, FileAccess.Read))
            {
                imageInfoJson = parser.GetImageInfo(file);
            }

            Console.WriteLine(imageInfoJson);
        }
    }
}