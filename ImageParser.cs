using System;
using System.IO;

namespace ImageParser
{
    // https://habr.com/ru/post/130472/
    public class ImageParser : IImageParser
    {
        public string GetImageInfo(Stream stream)
        {
            byte[] buffer = new byte[3];
            stream.Position = 1; // Первый байт пропускаем ибо там non-ASCII

            // Читаем расширение
            stream.Read(buffer, 0, 3); 
            string s = System.Text.Encoding.UTF8.GetString(buffer, 0, buffer.Length);

            stream.Position = 16; // Переходим на позицию чтения Chunks
            buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            var width = BitConverter.ToInt32(new byte[4] { buffer[3], buffer[2], buffer[1], buffer[0] }, 0);

            buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            var height = BitConverter.ToInt32(new byte[4] { buffer[3], buffer[2], buffer[1], buffer[0] }, 0);

            long length = stream.Length;

            //byte[] bytes = BitConverter.GetBytes(640);


            return s;

        }
    }
}