namespace Zebble
{
    using System.ComponentModel;
    using System.Threading.Tasks;
    using UIKit;
    using Zebble.Device;
    using Airbnb.Lottie;

    [EditorBrowsable(EditorBrowsableState.Never)]
    partial class LottieRenderer : INativeRenderer
    {
        private LOTAnimationView Player;
        private LottieView View;

        public Task<UIView> Render(Renderer renderer)
        {
            if (Player == null)
            {
                View = (LottieView)renderer.View;
                var info = IO.File(View.AnimationJsonFile);

                Player = LOTAnimationView.AnimationWithFilePath(info.FullName);
                Player.Play();
            }

            // AnimationSpeed = PlayBackRate, LoopAnimation=Loop, PlayFromProgress, Stop

            View.OnPlay.Handle(() => Player.Play());
            View.OnPause.Handle(() => Player.Pause());
            View.OnResume.Handle(() => Player.PlayWithCompletion(AnimationCompletionBlock));
            View.OnPropertyChanged.Handle(() =>
            {
                Player.AnimationSpeed = View.PlayBackRate;
                Player.PlayFromProgress(View.From, View.To, AnimationCompletionBlock);
            });


            return Task.FromResult<UIView>(Player);
        }

        private void AnimationCompletionBlock(bool animationFinished)
        {
            if (animationFinished)
            {
                if (View.Loop)
                    Player.PlayWithCompletion(AnimationCompletionBlock);
                else
                    Player.Stop();
            }
        }

        public void Dispose() => Player.Dispose();
    }
}