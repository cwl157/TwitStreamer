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

namespace TwitStreamer.Models
{
    public class ShowsResponseModel
    {
        public int Count { get; set; }

        public List<ShowResponseModel> Shows { get; set; }
    }
}