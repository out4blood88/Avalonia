using System;
using System.Threading;
using System.Windows.Input;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace Avalonia.Controls
{
    public class RefreshVisualizer : ContentControl
    {
        private const int DragLimit = 140;
        private const double MinScale = 0.3;
        private Visual? _visualizer;
        private Visual? _pivot;
        private TranslateTransform _visualizerTranslateTransform;
        private RotateTransform _visualizerRotateTransform;
        private ICommand? _refreshCommand;
        private ScaleTransform _visualizerScaleTransform;
        private RefreshVisualizerState _refreshVisualizerState;

        public static readonly RoutedEvent<RefreshRequestedEventArgs> RefreshRequestedEvent =
            RoutedEvent.Register<RefreshVisualizer, RefreshRequestedEventArgs>(nameof(RefreshRequested), RoutingStrategies.Bubble);

        public static readonly DirectProperty<RefreshVisualizer, RefreshVisualizerState> RefreshVisualizerStateProperty =
            AvaloniaProperty.RegisterDirect<RefreshVisualizer, RefreshVisualizerState>(nameof(RefreshVisualizerState),
                s => s.RefreshVisualizerState);

        public static readonly StyledProperty<PullDirection> PullDirectionProperty =
            AvaloniaProperty.Register<RefreshVisualizer, PullDirection>(nameof(PullDirection), PullDirection.FromTop);

        public RefreshVisualizerState RefreshVisualizerState
        {
            get
            {
                return _refreshVisualizerState;
            }
            private set
            {
                bool changed = value != _refreshVisualizerState;

                SetAndRaise(RefreshVisualizerStateProperty, ref _refreshVisualizerState, value);

                if (changed)
                {
                    switch (_refreshVisualizerState)
                    {
                        case RefreshVisualizerState.Idle:
                            _visualizer?.Classes.Remove("refreshing");
                            _visualizer?.SetValue(OpacityProperty, 0);
                            break;
                        case RefreshVisualizerState.Refreshing:
                            _visualizer?.Classes.Add("refreshing");
                            SetVisualizerTransforms(DragLimit,
                                PullDirection == PullDirection.FromTop || PullDirection == PullDirection.FromBottom,
                                PullDirection == PullDirection.FromBottom || PullDirection == PullDirection.FromRight);

                            var refreshArgs = new RefreshRequestedEventArgs(() => RefreshVisualizerState = RefreshVisualizerState.Idle, RefreshRequestedEvent);

                            refreshArgs.IncrementCount();

                            RaiseEvent(refreshArgs);

                            refreshArgs.DecrementCount();
                            break;
                    }
                }
            }
        }

        public PullDirection PullDirection
        {
            get => GetValue(PullDirectionProperty);
            set => SetValue(PullDirectionProperty, value);
        }

        public event EventHandler<RefreshRequestedEventArgs>? RefreshRequested
        {
            add => AddHandler(RefreshRequestedEvent, value);
            remove => RemoveHandler(RefreshRequestedEvent, value);
        }

        public RefreshVisualizer()
        {
            _visualizerTranslateTransform = new TranslateTransform();
            _visualizerRotateTransform = new RotateTransform();
            _visualizerScaleTransform = new ScaleTransform()
            {
                ScaleX = 0.5f,
                ScaleY = 0.5f,
            };
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            _visualizer = e.NameScope.Find<Visual>("PART_Visualizer");
            _pivot = e.NameScope.Find<Visual>("PART_VisualizerPivot");

            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(_visualizerTranslateTransform);
            transformGroup.Children.Add(_visualizerScaleTransform);

            if (_visualizer != null)
            {
                _visualizer.RenderTransform = transformGroup;
            }

            if (_pivot != null)
            {
                _pivot.RenderTransform = _visualizerRotateTransform;
            }
        }

        public void EndPull()
        {
            CheckAndSetRefreshing();
        }

        public void Pull(Vector delta)
        {
            if (_visualizer != null)
            {
                var xOffset = delta.X;
                var yOffset = delta.Y;

                bool isVertical = PullDirection == PullDirection.FromTop || PullDirection == PullDirection.FromBottom;
                bool isReverse = PullDirection == PullDirection.FromBottom || PullDirection == PullDirection.FromRight;

                var offset = isVertical ? yOffset : xOffset;

                SetVisualizerTransforms(offset, isVertical, isReverse);
                SetVisualizerState(offset);
            }
        }

        private void SetVisualizerTransforms(double offset, bool isVertical, bool isReverse)
        {
            if (_visualizer != null && offset < DragLimit)
            {
                var ratio = offset / DragLimit;
                var angle = ratio * 360;
                _visualizerRotateTransform.Angle = angle;

                if (isVertical)
                    _visualizerTranslateTransform.Y = offset * (isReverse ? -1 : 1);
                else
                    _visualizerTranslateTransform.X = offset * (isReverse ? -1 : 1);
                _visualizerScaleTransform.ScaleX = ratio < MinScale ? MinScale : ratio;
                _visualizerScaleTransform.ScaleY = ratio < MinScale ? MinScale : ratio;
                _visualizer.Opacity = Math.Min(ratio != 0 ? ratio + 0.1 : ratio, 1);
            }
        }

        private void SetVisualizerState(double offset)
        {
            if (offset < DragLimit)
            {

                RefreshVisualizerState = RefreshVisualizerState.Interacting;
            }
            else
            {
                RefreshVisualizerState = RefreshVisualizerState.Pending;
            }
        }

        private void CheckAndSetRefreshing()
        {
            if (RefreshVisualizerState == RefreshVisualizerState.Pending)
            {
                RefreshVisualizerState = RefreshVisualizerState.Refreshing;
            }
            else
            {
                RefreshVisualizerState = RefreshVisualizerState.Idle;
            }
        }
    }

    public enum RefreshVisualizerState
    {
        Idle,
        Interacting,
        Pending,
        Refreshing
    }

    public class RefreshRequestedEventArgs : RoutedEventArgs
    {
        private RefreshCompletionDeferral _refreshCompletionDeferral;

        public RefreshCompletionDeferral RefreshCompletionDeferral
        {
            get
            {
                return _refreshCompletionDeferral.Get();
            }
        }

        public RefreshRequestedEventArgs(Action deferredAction, RoutedEvent? routedEvent) : base(routedEvent)
        {
            _refreshCompletionDeferral = new RefreshCompletionDeferral(deferredAction);
        }

        public RefreshRequestedEventArgs(RefreshCompletionDeferral completionDeferral, RoutedEvent? routedEvent) : base(routedEvent)
        {
            _refreshCompletionDeferral = completionDeferral;
        }

        internal void IncrementCount()
        {
            _refreshCompletionDeferral?.Get();
        }

        internal void DecrementCount()
        {
            _refreshCompletionDeferral?.Complete();
        }
    }

    public class RefreshCompletionDeferral
    {
        private Action _deferredAction;
        private int _deferCount;

        public RefreshCompletionDeferral(Action deferredAction)
        {
            _deferredAction = deferredAction;
        }

        public void Complete()
        {
            Interlocked.Decrement(ref _deferCount);

            if (_deferCount == 0)
            {
                _deferredAction?.Invoke();
            }
        }

        public RefreshCompletionDeferral Get()
        {
            Interlocked.Increment(ref _deferCount);

            return this;
        }
    }
}
