namespace Zebble
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Security.Cryptography;
    using System.Threading.Tasks;
    using Android.Animation;
    using Android.App;
    using Android.OS;
    using Android.Runtime;
    using Android.Util;
    using Com.Airbnb.Lottie;
    using Java.IO;
    using Olive;
    using Org.Json;

    [Preserve]
    [EditorBrowsable(EditorBrowsableState.Never)]
    partial class LottieRenderer : INativeRenderer
    {
        private LottieAnimationView Player;
        private LottieView View;

        public void Dispose()
        {
            Player.Dispose();
            Player = null;
        }

        public async Task<Android.Views.View> Render(Renderer renderer)
        {
            try
            {
                Player = new LottieAnimationView(UIRuntime.CurrentActivity);
                View = (LottieView)renderer.View;
                // It is possible that this section would be deleted
                var assembly = UIRuntime.CurrentActivity.GetType().Assembly;
                var resource = View.AnimationJsonFile.Replace("/", ".");

                var name = assembly.GetManifestResourceNames().FirstOrDefault(x => x.EndsWith(resource));
                var str = assembly.ReadEmbeddedTextFile(name, System.Text.Encoding.UTF8).Trim();
                

                //Loop = RepeatCount, SetMinProgress = From, SetMaxProgress = To, Speed=PlayBackRate
                //Reference: https://github.com/airbnb/lottie-android/blob/master/lottie/src/main/java/com/airbnb/lottie/LottieAnimationView.java
                Player.SetAnimationFromJson(str, "jsonKey");
                View.OnPlay.Handle(() => Player.PlayAnimation());
                View.OnPause.Handle(() => Player.PauseAnimation());
                View.OnResume.Handle(() => Player.ResumeAnimation());

                View.OnPropertyChanged.Handle(() =>
                {
                    Player.Speed = View.PlayBackRate;
                    Player.SetMinProgress(View.From);
                    Player.SetMaxProgress(View.To);
                    Player.RepeatCount = View.Loop ? 1 : 0;
                    Player.PlayAnimation();
                });
                if (View.Loop)
                    Player.RepeatCount = ValueAnimator.Infinite;
                Player.PlayAnimation();
                return Player;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return null;
            }
            
        }
    }
}