using System;
using System.Threading;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Avalonia.Controls
{
    public class RefreshView : ContentControl
    {
        private RefreshVisualizer? _visualizer;
        private ScrollViewer? _scrollViewer;

        public static readonly RoutedEvent<RefreshRequestedEventArgs> RefreshRequestedEvent =
            RoutedEvent.Register<RefreshView, RefreshRequestedEventArgs>(nameof(RefreshRequested), RoutingStrategies.Bubble);

        public static readonly StyledProperty<PullDirection> PullDirectionProperty =
            AvaloniaProperty.Register<RefreshVisualizer, PullDirection>(nameof(PullDirection), PullDirection.FromTop);

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

        public RefreshView()
        {
            AddHandler(Gestures.PullGestureEvent, OnPull);
            AddHandler(Gestures.PullGestureEndedEvent, OnPullEnded);
        }

        private void OnPullEnded(object? sender, PullGestureEndedEventArgs e)
        {
            _visualizer?.EndPull();
        }

        private void OnPull(object? sender, PullGestureEventArgs e)
        {
            bool isAtEdge = false;

            if (_scrollViewer != null)
            {
                switch (PullDirection)
                {
                    case PullDirection.FromTop:
                        isAtEdge = _scrollViewer.Offset.Y != 0;
                        break;
                    case PullDirection.FromBottom:
                        isAtEdge = _scrollViewer.Offset.Y != _scrollViewer.Extent.Height - _scrollViewer.Viewport.Height;
                        break;
                    case PullDirection.FromLeft:
                        isAtEdge = _scrollViewer.Offset.X != 0;
                        break;
                    case PullDirection.FromRight:
                        isAtEdge = _scrollViewer.Offset.Y != _scrollViewer.Bounds.Width - _scrollViewer.Viewport.Width;
                        break;
                }
            }

            if (!isAtEdge)
            {
                _visualizer?.Pull(e.Delta);
            }
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            if (_visualizer != null)
            {
                _visualizer.RefreshRequested -= Visualizer_RefreshRequested;
            }

            _visualizer = e.NameScope.Find<RefreshVisualizer>("PART_Visualizer");
            _scrollViewer = e.NameScope.Find<ScrollViewer>("PART_RefreshScrollViewer");

            if(_visualizer != null )
            {
                _visualizer.RefreshRequested += Visualizer_RefreshRequested;
            }
        }

        private void Visualizer_RefreshRequested(object? sender, RefreshRequestedEventArgs e)
        {
            var ev = new RefreshRequestedEventArgs(e.RefreshCompletionDeferral, RefreshRequestedEvent);
            RaiseEvent(ev);
            ev.DecrementCount();
        }
    }
}
