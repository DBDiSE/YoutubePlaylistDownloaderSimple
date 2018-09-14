using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom.Events;
using YoutubeExplode;
using YoutubeExplode.Models.MediaStreams;

namespace WV_Playlist_cmd
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Podaj link do playlisty: (np. PL-fnyl58_XjsiTPKxJRkJG2uBWFUKRhBD)");
            string link = Console.ReadLine();

            Program v = new Program();
            v.ClearLog();
            v.MakeVideoDir();
            v.GetVideos(link);

            Console.ReadKey();
        }

        void ClearLog()
        {
            using (var tw = new StreamWriter(Directory.GetCurrentDirectory() + @"\Video\log.txt"))
            {
                tw.WriteLine(DateTime.Now + Environment.NewLine);
            }
        }

        void LogConsole(string message)
        {
            using (var tw = new StreamWriter(Directory.GetCurrentDirectory() + @"\Video\log.txt", true))
            {
                tw.WriteLine(message);
            }
        }

        string Postep(int i, int ilosc)
        {
            //string postep = ((decimal)i / (decimal)ilosc).ToString("0.00%");

            string postep = (((decimal)i / (decimal)ilosc)*100).ToString("0.00");

            return postep;
        }

        private static string NormalizeFileSize(long fileSize)
        {
            string[] units = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
            double size = fileSize;
            var unit = 0;

            while (size >= 1024)
            {
                size /= 1024;
                ++unit;
            }

            return $"{size:0.#} {units[unit]}";
        }

        void WriteOut(string tytul, string url, int i, int ilosc, int istnieje, string size)
        {
            if (istnieje == 1)
            {
                // + Environment.NewLine + "POBIERANIE..." + " Postep: " + Postep(i, ilosc) + "%"
                Console.WriteLine(Environment.NewLine + i + " z " + ilosc + "  --- TYTUL: " + tytul + "  --- URL: " + url + " --- ROZMIAR: " + size + Environment.NewLine + "Postep: " + Postep(i, ilosc) + "%" + "\nPlik istnieje, pomijam.");
            }
            else if(istnieje == 0)
            {
                Console.WriteLine(Environment.NewLine + i + " z " + ilosc + "  --- TYTUL: " + tytul + "  --- URL: " + url + " --- ROZMIAR: " + size + Environment.NewLine + "POBIERANIE..." + "\nPostep: " + Postep(i, ilosc) + "%");
            }
            
        }

        void WriteOut(string message)
        {
            Console.WriteLine(message);
            LogConsole(message);
        }

        void MakeVideoDir()
        {
            string currentPath = Directory.GetCurrentDirectory();
            if (!Directory.Exists(Path.Combine(currentPath, "Video")))
                Directory.CreateDirectory(Path.Combine(currentPath, "Video"));
        }

        //PL9v11TvMGKeIjVqzftXkpABp3aTsQ9WTk

        async void GetVideos(string link)
        {
            var client = new YoutubeClient();

            var playlist = await client.GetPlaylistAsync(link);
            var video = playlist.Videos.ToArray();

            Console.Clear();
            WriteOut("Liczba video w playliscie: " + video.Length);

            int ilosc = video.Length;
            int i = 0;

            foreach(var v in video)
            {
                i++;

                try
                {
                    var streamInfoSet = await client.GetVideoMediaStreamInfosAsync(v.Id);
                    var streamInfo = streamInfoSet.Muxed.WithHighestVideoQuality();
                    var ext = streamInfo.Container.GetFileExtension();

                    if (!File.Exists(Directory.GetCurrentDirectory() + @"\Video\" + v.Title + "." + ext))
                    {
                        int istnieje = 0;

                        using (var progress = new ProgressBar())
                        {
                            WriteOut(v.Title, v.Id, i, ilosc, istnieje, NormalizeFileSize(streamInfo.Size));
                            await client.DownloadMediaStreamAsync(streamInfo, Directory.GetCurrentDirectory() + @"\Video\" + v.Title + "." + ext, progress);
                        }                         
                    }
                    else
                    {
                        int istnieje = 1;
                        WriteOut(v.Title, v.Id, i, ilosc, istnieje, NormalizeFileSize(streamInfo.Size));
                    }               
                }
                catch
                {
                    string message = Environment.NewLine + i + " z " + ilosc + "  --- TYTUL: " + v.Title + "  --- URL: " + v.Id + "\nBlad podczas pobierania video";
                    WriteOut(message);
                }
                
            }

            
        }
    }
}
