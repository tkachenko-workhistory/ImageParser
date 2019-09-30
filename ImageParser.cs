using Newtonsoft.Json;
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
            byte[] buffer = new byte[4];
            // Определяем тип файла
            stream.Read(buffer, 0, 4);
            string s = Encoding.UTF8.GetString(buffer, 0, buffer.Length).ToLower();

            ImageInfo info;

            if (s.Contains("png"))
                info = GetPngInfo(stream);
            else if (s.Contains("gif"))
                info = GetGifInfo(stream);
            else if (buffer[0] == 0x42 && buffer[1] == 0x4D)
                info = GetBmpInfo(stream);
            else
                throw new Exception("Неподдерживаемый формат");

            return JsonConvert.SerializeObject(info);
        }

        private ImageInfo GetPngInfo(Stream stream)
        {
            var info = new ImageInfo();
            info.Extension = "png";

            stream.Position = 16; // Переходим на позицию чтения Chunks
            byte[] buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            info.Width = BitConverter.ToInt32(new byte[4] { buffer[3], buffer[2], buffer[1], buffer[0] }, 0);

            buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            info.Height = BitConverter.ToInt32(new byte[4] { buffer[3], buffer[2], buffer[1], buffer[0] }, 0);

            info.Size = stream.Length;

            return info;
        }

        private ImageInfo GetGifInfo(Stream stream)
        {
            var info = new ImageInfo();
            info.Extension = "gif";

            // Ищем блок изображения
            byte[] buffer = new byte[1];
            string type = "";
            while (type != "2c")
            {
                stream.Read(buffer, 0, 1);
                StringBuilder sb = new StringBuilder(buffer.Length * 2);
                // ToDo: можно пропускать расширения, узнав их длину в байтах
                sb.AppendFormat("{0:x2}", buffer[0]);
                type = sb.ToString();
            }

            buffer = new byte[4];
            stream.Position += 4;

            stream.Read(buffer, 0, 2);
            info.Width = BitConverter.ToInt32(buffer, 0);

            stream.Read(buffer, 0, 2);
            info.Height = BitConverter.ToInt32(buffer, 0);

            info.Size = stream.Length;

            return info;
        }

        private ImageInfo GetBmpInfo(Stream stream)
        {
            var info = new ImageInfo();
            info.Extension = "bmp";

            byte[] buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            info.Size = BitConverter.ToInt32(buffer, 0);

            buffer = new byte[7];
            stream.Position = 16;
            stream.Read(buffer, 0, 4);
            for (int i = 0; i < 4; i++)
                if (buffer[i] != 0)
                {
                    info.Width = BitConverter.ToInt32(buffer, i);
                    break;
                }

            stream.Position = 20;
            stream.Read(buffer, 0, 4);
            for (int i = 0; i < 4; i++)
                if (buffer[i] != 0)
                {
                    info.Height = BitConverter.ToInt32(buffer, i);
                    break;
                }

            return info;
        }


        class ImageInfo
        {
            public string Extension { get; set; }

            public int Width { get; set; }

            public int Height { get; set; }

            public long Size { get; set; }
        }
    }
}