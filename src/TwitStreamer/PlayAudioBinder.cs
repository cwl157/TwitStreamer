/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace TwitStreamer
{
    public class PlayAudioBinder : Binder
    {
        public PlayingService Service
        {
            get { return this.service; }
        }
        protected PlayingService service;

        public bool IsBound { get; set; }

        public PlayAudioBinder(PlayingService service) { this.service = service; }
    }
}
*/