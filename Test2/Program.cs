using System;
using System.Net;
using System.Threading.Tasks;
using Rug.Osc;
using Windows.Media;
using Windows.Media.Control;
using Windows.Phone.Notification.Management;
using Windows.Storage;
using System.Threading;


namespace Osc
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            OscSender sender = new OscSender(IPAddress.Parse("127.0.0.1"), 9001, 9000);



            sender.Connect();
            Console.WriteLine("Connected");

            // Establish Variables
            GlobalSystemMediaTransportControlsSessionManager sessionManager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
            Console.WriteLine(sessionManager.GetSessions().Count + " Media Sessions Found");
            GlobalSystemMediaTransportControlsSession session;
            GlobalSystemMediaTransportControlsSessionMediaProperties propertiess;
            GlobalSystemMediaTransportControlsSessionPlaybackInfo playbackinfo;
            string artist;
            string title = null;
            bool updated = false;
            CancellationTokenSource cts = new CancellationTokenSource();

            bool Working;

            while (true)
            {

                // Error Catcher, Making sure everything called is Valid
                Working = true;
                try
                {
                    sessionManager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();

                    session = sessionManager.GetCurrentSession();

                    if (sessionManager.GetSessions().Count > 0 && session != null)
                    {
                        propertiess = await session.TryGetMediaPropertiesAsync();
                        playbackinfo = session.GetPlaybackInfo();
                    }
                    else
                    {
                        Working = false;
                        Console.WriteLine("Im apart of the KKK and I really hate the colored individuals");
                        sender.Send(new OscMessage("/chatbox/input", "Im apart of the KKK and I really hate the colored individuals", true));
                    }
                }
                catch
                {
                    Working = false;
                    Console.WriteLine("I hate black People");
                }


                // Sending Info
                if (Working)
                {
                    session = sessionManager.GetCurrentSession();
                    playbackinfo = session.GetPlaybackInfo();

                    propertiess = await session.TryGetMediaPropertiesAsync();

                    if (playbackinfo.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing && !updated && propertiess.Title != title && propertiess.Title.Length > 0)
                    {

                        if (propertiess.Artist.Length == 0)
                        {
                            artist = "Unknown Artist";
                        }
                        else
                        {
                            artist = propertiess.Artist;
                        }
                        title = propertiess.Title;

                        Console.WriteLine(artist + " - " + title);
                        sender.Send(new OscMessage("/chatbox/input", $":3\n {artist} - {title}", true, true));
                        updated = true;

                        // Sending a delay to remove Chatbox
                        cts = new CancellationTokenSource();
                        Delay(cts.Token, sender);



                    }
                    else
                    {
                        updated = false;


                        if (playbackinfo.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Paused && title != null || session == null)
                        {
                            artist = null;
                            title = null;
                            Console.WriteLine("Paused");
                            sender.Send(new OscMessage("/chatbox/input", "", true, true));
                            cts.Cancel();

                        }
                    }

                }

                
                await Task.Delay(1000);
            }

        }
        static async Task Delay(CancellationToken cts, OscSender Sender)
        {
            await Task.Delay(5000, cts);

            Sender.Send(new OscMessage("/chatbox/input", "", true, true));
        }
    }

}