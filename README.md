# Image Parser

## Формулировка

Тебе предстоит реализовать утилиту для получения информации об изображении по структуре файла. Изображения будут трёх форматов:
* png
* bmp
* gif

На вход будет дан Stream файла с изображением, на выходе нужно вывести информацию об изображении в формате JSON:

```
{
    "Height": 0,    // - Высота изображения в пикселях
    "Width": 0,     // - Ширина изображения в пикселях
    "Format": "",   // - Формат изображения
    "Size": 0       // - Размер файла с изображением в байтах
}
```

Формат gif поддерживает хранение нескольких изображений, тебе нужно вывести информацию только о первом изображении.

[Скачай проект ImageParser](https://ulearn.me/Exercise/StudentZip?courseId=Campus1920&slideId=7d886a4b-c5da-4c84-b1d6-4f276b8a092e) и реализуй метод `string GetImageInfo(Stream stream)` в классе `ImageParser`.

Для работы с JSON можно использовать библиотеку Newtonsoft.Json. Учти, что использовать любые библиотеки работы с изображениями запрещено.

**Пример использования**

```CSharp
var parser = new ImageParser();
string imageInfoJson;
using (var file = new FileStream("image.png", FileMode.Open, FileAccess.Read))
{
    imageInfoJson = parser.GetImageInfo(file);
}
Console.WriteLine(imageInfoJson);
```

**Пример выходных данных**

```json
{
    "Height": 292,
    "Width": 640,
    "Format": "Png",
    "Size": 229983

}
```
