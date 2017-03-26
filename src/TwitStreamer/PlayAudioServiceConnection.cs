/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Content;

namespace TwitStreamer
{
    public class PlayAudioServiceConnection : Java.Lang.Object, IServiceConnection
    {
        PlayAudioBinder binder;
        public PlayAudioServiceConnection(PlayAudioBinder binder)
        {
            if (binder != null)
            {
                this.binder = binder;
            }
        }

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            PlayAudioBinder serviceBinder = service as
            PlayAudioBinder;

            if (serviceBinder != null)
            {
                this.binder = serviceBinder;
                this.binder.IsBound = true;

                // raise the service bound event
                this.ServiceConnected(this, new
                ServiceConnectedEventArgs()
                { Binder = service });

                // begin updating the location in the Service
                int duration = serviceBinder.Service.Player.Duration;
                int currentPosition = serviceBinder.Service.Player.CurrentPosition;
                Console.WriteLine("In ServiceConnection: currentPosition = " + currentPosition);
                Console.WriteLine("In ServiceConnection: duration = " + duration);
            }
        }

        public void OnServiceDisconnected(ComponentName name) { this.binder.IsBound = false; }
    }

    public class App
    {
        protected App()
        {
            PlayAudioServiceConnection = new PlayAudioServiceConnection(null);
            PlayAudioServiceConnection.ServiceConnected += (object sender, ServiceConnectedEventArgs e) =>
            {
                this.LocationServiceConnected(this, e);
            };
        }
    }
}
*/