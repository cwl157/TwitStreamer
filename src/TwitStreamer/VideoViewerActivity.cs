using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using TwitStreamer.ViewModels;

namespace TwitStreamer
{
    [Activity(Label = "VideoViewerActivity")]
    public class VideoViewerActivity : Activity
    {
        private ShowViewModel _show;
        private EpisodeViewModel _episode;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.VideoPlayBack);

            _show = TwitApi.Instance.Shows.FirstOrDefault(s => s.Selected == true);
            if (_show == null)
            {
                // return to MainActivity
            }

            _episode = _show.Episodes.FirstOrDefault(e => e.Selected == true);
            if (_episode == null)
            {
                // return to MainActivity
            }

            
            var videoView = FindViewById<VideoView>(Resource.Id.videoViewer);
            videoView.SetVideoURI(new Android.Net.Uri(_episode.Episode.HdVideoDetails.MediaUrl));
            videoView.Start();
            videoView.Duration = 

            // Create your application here
        }
    }
}