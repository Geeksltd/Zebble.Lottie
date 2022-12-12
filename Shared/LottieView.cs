namespace Zebble
{
#if ANDROID
    [Android.Runtime.Preserve]
#endif
    public partial class LottieView : View, IRenderedBy<LottieRenderer>
    {
        internal AsyncEvent OnPlay = new AsyncEvent();
        internal AsyncEvent OnPause = new AsyncEvent();
        internal AsyncEvent OnResume = new AsyncEvent();
        internal AsyncEvent OnPropertyChanged = new AsyncEvent();



        private string animationJsonFile;
        public string AnimationJsonFile { get => animationJsonFile; set => SetAnimationJsonFile(value); }
        private async void SetAnimationJsonFile(string value)
        {
#if ANDROID
            animationJsonFile="Android.Resources." + value;
#else
            animationJsonFile = value;
#endif
            await Render();
        }

        private bool loop = true;
        public bool Loop { get => loop; set => SetLoop(value); }
        private async void SetLoop(bool value)
        {
            if (value == loop) return;
            loop = value;
            await OnPropertyChanged.RaiseOn(Thread.UI);
        }

        private float from = 0;
        public float From { get => from; set => SetFrom(value); }
        private async void SetFrom(float value)
        {
            if (value == from) return;
            if (value > 1.0) value = 1;
            if (value < 0.0) value = 0;
            if (value > to) value = to;
            from = value;
            await OnPropertyChanged.RaiseOn(Thread.UI);
        }

        private float to = 1;
        public float To { get => to; set => SetTo(value); }
        private async void SetTo(float value)
        {
            if (value == to) return;
            if (value > 1.0) value = 1;
            if (value < 0.0) value = 0;
            if (value < from) value = from;
            to = value;
            await OnPropertyChanged.RaiseOn(Thread.UI);
        }

        private float playBackRate = 1;
        public float PlayBackRate { get => playBackRate; set => SetPlayBackRate(value); }
        private async void SetPlayBackRate(float value)
        {
            if (value == playBackRate) return;
            playBackRate = value;
            await OnPropertyChanged.RaiseOn(Thread.UI);
        }
        public void Pause() => OnPause.RaiseOn(Thread.UI);
        public void Play() => OnPlay.RaiseOn(Thread.UI);
        public void Resume() => OnResume.RaiseOn(Thread.UI);


        public LottieView() 
            {
            
            }

    }
}
