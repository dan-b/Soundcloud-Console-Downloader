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
    }
}
