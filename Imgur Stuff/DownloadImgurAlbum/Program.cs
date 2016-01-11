using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
// ReSharper disable InconsistentNaming

namespace DownloadImgurAlbum
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.Clear();
                Console.WriteLine("Usage: DownloadImgurAlbum <album id> <directory>\r\nExample: DownloadImgurAlbum.exe j3gP4 \"d:\\imgur\"");
                return;
            }
            try
            {
                var getUrl = $"http://imgur.com/ajaxalbums/getimages/{args[0]}/hit.json?all=true";
                var dir = Path.Combine(args[1], args[0]);
                Directory.CreateDirectory(dir);
                using (var c = new WebClient())
                {
                    var data = c.DownloadString(getUrl);
                    var images = JsonConvert.DeserializeObject<AlbumResponse>(data);

                    Console.WriteLine($"Found {images.data.count} images.  Downloading...");
                    var index = 1;
                    foreach (var image in images.data.images)
                    {
                        Console.SetCursorPosition(0, 1);
                        Console.Write(new string(' ', Console.WindowWidth));
                        Console.SetCursorPosition(0, 1);
                        Console.Write($"{index} / {images.data.count}: {image.hash} - {image.title}");
                        c.DownloadFile($"http://i.imgur.com/{image.hash}{image.ext}", $"{dir}\\{image.hash}{image.ext}");
                        index++;
                    }
                }
                Console.WriteLine("\r\nComplete.  Press any key to exit.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"\r\nError...  Make sure you called the application with the correct command line.  DownloadImgurAlbum <albumid> <directory>\r\nExample: DownloadImgurAlbum.exe j3gP4 \"d:\\imgur\"\r\n{e.Message}");
            }
        }

        public class Image
        {
            public string hash { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public int width { get; set; }
            public int height { get; set; }
            public int size { get; set; }
            public string ext { get; set; }
            public bool animated { get; set; }
            public bool prefer_video { get; set; }
            public bool looping { get; set; }
            public string datetime { get; set; }
        }

        public class Data
        {
            public int count { get; set; }
            public List<Image> images { get; set; }
        }

        public class AlbumResponse
        {
            public Data data { get; set; }
            public bool success { get; set; }
            public int status { get; set; }
        }
    }
}
