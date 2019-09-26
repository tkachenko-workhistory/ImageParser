using System;
using System.IO;
using System.Text;

namespace ImageParser
{
    // https://habr.com/ru/post/130472/
    public class ImageParser : IImageParser
    {
        public string GetImageInfo(Stream stream)
        {
            // PNG
            /* byte[] buffer = new byte[3];
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

             long length = stream.Length;*/

            // BMP
            byte[] buffer = new byte[2];
            stream.Read(buffer, 0, 2);
            // 4D42/424D (little-endian / big-endian).
            StringBuilder sb = new StringBuilder(buffer.Length * 2);
            foreach (byte b in buffer)
                sb.AppendFormat("{0:x2}", b);
            var type = sb.ToString();

            buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            var length = BitConverter.ToInt32(buffer, 0);

            buffer = new byte[7];
            stream.Position = 16;
            stream.Read(buffer, 0, 4);
            var width = 0;
            for (int i = 0; i < 4; i++)
                if (buffer[i] != 0)
                {
                    width = BitConverter.ToInt32(buffer, i);
                    break;
                }

            stream.Position = 20;
            stream.Read(buffer, 0, 4);
            var height = 0;
            for (int i = 0; i < 4; i++)
                if (buffer[i] != 0)
                {
                    height = BitConverter.ToInt32(buffer, i);
                    break;
                }

            return "";

        }
    }
}