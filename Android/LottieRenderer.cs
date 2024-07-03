﻿namespace Zebble
{
    using System.ComponentModel;
    using System.Threading.Tasks;
    using PlatformView = Android.Views.View;
    using SkiaSharp.Views.Android;
    using System;
    using Android.Runtime;

    [EditorBrowsable(EditorBrowsableState.Never)]
    class LottieRenderer : INativeRenderer
    {
        LottiePlayer Player;
        LottieView View;
        bool IsDisposed;

        public Task<PlatformView> Render(Renderer renderer)
        {
            View = (LottieView)renderer.View;
            Player = new LottiePlayer(View.Data, OnFinished);

            View.OnPlay.Handle(Player.Play);
            View.OnPause.Handle(Player.Pause);
            View.OnResume.Handle(Player.Resume);
            View.OnStop.Handle(Player.Stop);

            Player.Play();

            return Task.FromResult<PlatformView>(Player);
        }

        async void OnFinished()
        {
            if (IsDisposed) return;
            if (View.Loop)
            {
                while (View?.IsVisibleOnScreen() != true)
                {
                    if (View?.IsDisposed ?? true) return;
                    await Task.Delay(100);
                }

                Player.Play();
            }
        }

        public void Dispose()
        {
            IsDisposed = true;
            Player?.Dispose();
            Player = null;
			
			GC.SuppressFinalize(this);
        }

        class LottiePlayer : SKCanvasView
        {
            bool IsDisposed;
            LottieAnimationController Controller;

            public LottiePlayer(string data, Action onFinished) : base(UIRuntime.CurrentActivity)
                => Controller = new(data, OnInvalidate, onFinished);

            [Preserve]
            public LottiePlayer(IntPtr ptr, JniHandleOwnership handle) : base(ptr, handle) { }

            void OnInvalidate()
            {
                if (IsDisposed) return;
                Invalidate();
            }

            public void Play() => Controller?.Play();

            public void Pause() => Controller?.Pause();

            public void Resume() => Controller?.Resume();

            public void Stop() => Controller?.Stop();

            protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
            {
                base.OnPaintSurface(e);
                Controller?.Render(e.Surface.Canvas, e.Info.Rect);
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                IsDisposed = true;
                Controller?.Dispose();
                Controller = null;
            }
        }
    }
}