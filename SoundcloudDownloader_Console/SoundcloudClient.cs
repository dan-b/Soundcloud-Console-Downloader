using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace Soundcloud
{
    class SoundcloudClient
    {
        static String clientID = "b45b1aa10f1ac2941910a7f0d10f8e28";

        public struct TrackID
        {
            public int index;
            public String id;
            public String title;

            public TrackID(int i, String j, String k)
            {
                index = i;
                id = j;
                title = k;
            }
        }

        static void Main(string[] args)
        {
            bool menu = true;
            while (menu)
            {
                Console.Write("URL > ");
                String url = Console.ReadLine();
                if (url.Contains("/sets/"))
                {
                    Console.WriteLine("Downloading playlist.");
                    downloadSet(url);
                }
                else
                {
                    downloadSong(url);
                }
            }
        }

        /// <summary>
        /// Downloads and returns the trackID of given song
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static TrackID ResolveTrackID(String url, int startindex)
        {
            WebClient w = new WebClient();
            //String str = w.DownloadString("http://api.soundcloud.com/resolve.json?url=" + url + "&client_id=" + clientID);
            String str =
                "{\"kind\":\"track\",\"id\":15993727,\"created_at\":\"2011/05/26 20:00:30 +0000\",\"user_id\":2176409,\"duration\":145165,\"commentable\":true,\"state\":\"finished\",\"original_content_size\":25585056,\"last_modified\":\"2014/10/25 19:47:30 +0000\",\"sharing\":\"public\",\"tag_list\":\"\",\"permalink\":\"finally-remix\",\"streamable\":true,\"embeddable_by\":\"all\",\"downloadable\":false,\"purchase_url\":\"http://www.beatport.com/track/finally-mord-fustang-remix/2749900\",\"label_id\":null,\"purchase_title\":null,\"genre\":\"\",\"title\":\"Froidz - Finally (Mord Fustang Remix)\",\"description\":\"\",\"label_name\":\"Yawa Recordings\",\"release\":\"\",\"track_type\":\"remix\",\"key_signature\":\"\",\"isrc\":\"\",\"video_url\":null,\"bpm\":null,\"release_year\":2011,\"release_month\":8,\"release_day\":30,\"original_format\":\"wav\",\"license\":\"all-rights-reserved\",\"uri\":\"https://api.soundcloud.com/tracks/15993727\",\"user\":{\"id\":2176409,\"kind\":\"user\",\"permalink\":\"mordfustang\",\"username\":\"Mord Fustang\",\"last_modified\":\"2014/10/13 13:54:42 +0000\",\"uri\":\"https://api.soundcloud.com/users/2176409\",\"permalink_url\":\"http://soundcloud.com/mordfustang\",\"avatar_url\":\"https://i1.sndcdn.com/avatars-000079911702-qfflx7-large.jpg\"},\"permalink_url\":\"http://soundcloud.com/mordfustang/finally-remix\",\"artwork_url\":\"https://i1.sndcdn.com/artworks-000009997346-4ts29q-large.jpg\",\"waveform_url\":\"https://w1.sndcdn.com/qhRMZDL3bHd3_m.png\",\"stream_url\":\"https://api.soundcloud.com/tracks/15993727/stream\",\"playback_count\":281965,\"download_count\":0,\"favoritings_count\":3214,\"comment_count\":283,\"likes_count\":3214,\"reposts_count\":308,\"attachments_uri\":\"https://api.soundcloud.com/tracks/15993727/attachments\",\"policy\":\"ALLOW\"}";
            try
            {
                dynamic track = Newtonsoft.Json.JsonConvert.DeserializeObject(str);
                var id = track.id;
            }
            catch (Exception exception)
            {
                var message = exception.Message;
            }
            //int index = str.IndexOf("\"id\":", startindex) + 5;
            int index = str.IndexOf("\"track\",\"id\":", startindex) + 13;
            int titleindex = str.IndexOf("\"title\":", index) + 9;
            TrackID trackID = new TrackID(index, str.Substring(index, str.IndexOf(",", index) - index), str.Substring(titleindex, str.IndexOf("\",", titleindex) - titleindex));
            return trackID;
        }

        /// <summary>
        /// Downloads and returns all trackIDs of all songs in a given playlist
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static List<TrackID> resolveTrackIDs(String url)
        {
            List<TrackID> ret = new List<TrackID>();
            // https://api.soundcloud.com/resolve.json?url=https://soundcloud.com/instance01/sets/extremly-chilled&client_id=b45b1aa10f1ac2941910a7f0d10f8e28
            // https://api.soundcloud.com/playlists/42506087.json?client_id=b45b1aa10f1ac2941910a7f0d10f8e28
            int currentindex = 0;
            for (int i = 0; i < getTrackCount(url); i++)
            {
                TrackID t = ResolveTrackID(url, currentindex);
                currentindex = t.index + 1;
                ret.Add(t);
            }
            return ret;
        }

        public static int getTrackCount(String url)
        {
            WebClient w = new WebClient();
            String str = w.DownloadString("http://api.soundcloud.com/resolve.json?url=" + url + "&client_id=" + clientID);
            int index = str.IndexOf("\"track_count\":") + 14;
            return Convert.ToInt32(str.Substring(index, str.IndexOf(",", index) - index)); 
        }

        /// <summary>
        /// Downloads and returns the download link for given song
        /// </summary>
        /// <param name="trackID"></param>
        /// <returns></returns>
        public static String resolveDownloadURL(String trackID)
        {
            WebClient w = new WebClient();
            String str = w.DownloadString("https://api.soundcloud.com/tracks/" + trackID + "/streams?client_id=" + clientID);
            int index = str.IndexOf(":\"http") + 2;
            return str.Substring(index, str.IndexOf("\"", index) - index);
        }

        /// <summary>
        /// Downloads a single song
        /// </summary>
        /// <param name="url"></param>
        public static void downloadSong(String url)
        {
            TrackID trackID = ResolveTrackID(url, 0);
            String downloadurl = resolveDownloadURL(trackID.id).Replace("\\u0026", "&");
            WebClient w = new WebClient();
            String filename = trackID.title + ".mp3";
            if (filename.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                foreach (char c in new string(Path.GetInvalidFileNameChars()))
                {
                    filename = filename.Replace(c.ToString(), "");
                }
            }
            Console.WriteLine("Downloading " + filename + ".");
            w.DownloadFile(downloadurl, filename);
            Console.WriteLine("Finished downloading " + filename + ".");
        }

        /// <summary>
        /// Downloads a single Set
        /// </summary>
        /// <param name="url"></param>
        public static void downloadSet(String url)
        {
            List<TrackID> trackIDs = new List<TrackID>(resolveTrackIDs(url));
            foreach (TrackID trackID in trackIDs)
            {
                String downloadurl = Regex.Unescape(resolveDownloadURL(trackID.id)); //.Replace("\\u0026", "&");
                WebClient w = new WebClient();
                String filename = Regex.Unescape(trackID.title) + ".mp3";
                if (filename.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                {
                    foreach (char c in new string(Path.GetInvalidFileNameChars()))
                    {
                        filename = filename.Replace(c.ToString(), "");
                    }
                }
                Console.WriteLine("Downloading " + filename + ".");
                w.DownloadFile(downloadurl, filename);
                Console.WriteLine("Finished downloading " + filename + ".");
            }
            Console.WriteLine("Finished downloading the playlist.");
        }
    }
}
