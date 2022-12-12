namespace Zebble
{
    using System;
    using System.Threading.Tasks;
    using Windows.UI.Xaml;
    using Microsoft.Toolkit.Uwp.UI.Lottie;
    using Zebble.Device;
    using System.IO;
    using Microsoft.UI.Xaml.Controls;
    using System.Diagnostics;
    using Windows.Storage;
    using System.Text;

    public class LottieRenderer : INativeRenderer
    {
        AnimatedVisualPlayer Player;
        LottieView View;
        LottieVisualSource Source;
        public async Task<FrameworkElement> Render(Renderer renderer)
        {
            Player = new AnimatedVisualPlayer();
            View = (LottieView)renderer.View;
            try
            {
                Source = LottieVisualSource.CreateFromString($@"ms-appx:///Resources/{View.AnimationJsonFile}");
                Player.Source = Source;
            }
            catch (Exception ex)
            {
                await Alert.Toast("Failed: " + ex.Message);
            }

            View.OnPlay.Handle(async () => await Player.PlayAsync(View.From, View.To, View.Loop));
            View.OnPause.Handle(() => Player.Pause());
            View.OnResume.Handle(() => Player.Resume());
            View.OnPropertyChanged.Handle(async () =>
            {
                Player.PlaybackRate = View.PlayBackRate;
                await Player.PlayAsync(View.From, View.To, View.Loop);
            });
            //Player.AutoPlay = false;
            return await Task.FromResult(Player);
        }

        public void Dispose()
        {
            Player = null;
            View = null;
        }
    }
}
