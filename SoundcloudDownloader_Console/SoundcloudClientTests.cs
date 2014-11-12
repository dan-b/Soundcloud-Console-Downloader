using System;
using NUnit.Framework;
using Newtonsoft.Json;

namespace Soundcloud
{
    class SoundcloudClientTests
    {
        [Test]
        [TestCase(@"https://soundcloud.com/mordfustang/finally-remix", 0)]
        public void ResolveTrackID(String url, int startindex)
        {
            var TrackID = SoundcloudClient.ResolveTrackID(url, startindex);
        }
        [Test]
        [TestCase(@"is:this/name|safe\?")]
        public void MakeFileNameSafe(string unsafeFileName)
        {
            var result = SoundcloudClient.MakeFileNameSafe(unsafeFileName);
        }
        [Test]
        [TestCase(@"https://soundcloud.com/mordfustang/finally-remix")]
        public void DownloadSong(String url)
        {
            SoundcloudClient.DownloadSong(url);
        }

    }
}
