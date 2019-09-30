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
                info = GetBmpInfo(stream, true);
            else if (buffer[1] == 0x42 && buffer[0] == 0x4D)
                info = GetBmpInfo(stream, true);
            else
                throw new Exception("Неподдерживаемый формат");

            return JsonConvert.SerializeObject(info);
        }

        private ImageInfo GetPngInfo(Stream stream)
        {
            var info = new ImageInfo();
            info.Format = "png";

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
            info.Format = "gif";

            // Ищем блок изображения
            byte[] buffer = new byte[1];
            string type = "";
            while (type != "2c")
            {
                stream.Read(buffer, 0, 1);
                StringBuilder sb = new StringBuilder(buffer.Length * 2);
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

        private ImageInfo GetBmpInfo(Stream stream, bool bigEndian)
        {
            var info = new ImageInfo();
            info.Format = "bmp";

            // Определить версию структуры
            byte[] buffer = new byte[4];
            stream.Position = 14;
            stream.Read(buffer, 0, 4);
            int version = BitConverter.ToInt32(buffer, 0);

            info.Size = stream.Length;

            if (!bigEndian)
                throw new Exception("littleEndian " + info.Height);

            if (version == 40 || version == 108 || version == 124)
            // Соответствует 3, 4 и 5 версии
            {
                buffer = new byte[4];
                stream.Position = 18;
                stream.Read(buffer, 0, 4);
                info.Width = BitConverter.ToInt32(buffer, 0);

                buffer = new byte[4];
                stream.Read(buffer, 0, 4);
                info.Height = BitConverter.ToInt32(buffer, 0);
            }
            else if (version == 12)
            // CORE
            {
                throw new Exception("core " + info.Height);
            }
            return info;
        }


        class ImageInfo
        {
            public string Format { get; set; }

            public int Width { get; set; }

            public int Height { get; set; }

            public long Size { get; set; }
        }
    }
}