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
using TwitApi.Models;

namespace TwitStreamer.ViewModels
{
    public class EpisodeViewModel
    {
        public EpisodeResponseModel Episode { get; set; }

        public bool Selected { get; set; }
    }
}