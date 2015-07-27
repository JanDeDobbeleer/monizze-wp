using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Monizze.Controls
{
    [TemplateVisualState(GroupName = CommonGroupStateName, Name = ShowStateName)]
    [TemplateVisualState(GroupName = CommonGroupStateName, Name = HideStateName)]
    public class AutoHideBar : ContentControl
    {
        internal const string CommonGroupStateName = "CommonStates";
        internal const string ShowStateName = "Show";
        internal const string HideStateName = "Hide";

        #region ScrollControl (DependencyProperty)

        /// <summary>
        /// A description of the property.
        /// </summary>
        public FrameworkElement ScrollControl
        {
            get { return (FrameworkElement)GetValue(ScrollControlProperty); }
            set { SetValue(ScrollControlProperty, value); }
        }
        public static readonly DependencyProperty ScrollControlProperty =
            DependencyProperty.Register("ScrollControl", typeof(FrameworkElement), typeof(AutoHideBar),
            new PropertyMetadata(null, OnScrollControlChanged));

        private static void OnScrollControlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AutoHideBar)d).OnScrollControlChanged(e);
        }

        protected virtual void OnScrollControlChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_scroller != null)
            {
                DetachScroller(_scroller);
            }

            if (e.NewValue == null)
                return;
            var el = e.NewValue as UIElement;
            FindAndAttachScrollViewer(el);
        }

        #endregion

        #region DependencyProperty

        /// <summary>
        /// Shows the navigation bar when on top of the list
        /// </summary>
        public bool ShowOnTop
        {
            get { return (bool)GetValue(ShowOnTopProperty); }
            set { SetValue(ShowOnTopProperty, value); }
        }
        public static readonly DependencyProperty ShowOnTopProperty =
            DependencyProperty.Register("ShowOnTop", typeof(bool), typeof(AutoHideBar),
              new PropertyMetadata(true));

        public static readonly DependencyProperty BalanceProperty = DependencyProperty.Register(
            "Balance", typeof (string), typeof (AutoHideBar), new PropertyMetadata("--.--"));

        public string Balance
        {
            get { return (string) GetValue(BalanceProperty); }
            set { SetValue(BalanceProperty, value); }
        }

        public static readonly DependencyProperty MinimumOffsetScrollingDownProperty = DependencyProperty.Register(
            "MinimumOffsetScrollingDown", typeof (double), typeof (AutoHideBar), new PropertyMetadata(140));

        public double MinimumOffsetScrollingDown
        {
            get { return (double) GetValue(MinimumOffsetScrollingDownProperty); }
            set { SetValue(MinimumOffsetScrollingDownProperty, value); }
        }
        #endregion

        private const double TopListOffsetDisplay = 10;
        private const double MinimumOffsetScrollingUp = 50;

        private UIElement _scroller;
        private DoubleAnimation _hideAnimation;

        private double _firstOffsetValue;

        public AutoHideBar()
        {
            DefaultStyleKey = typeof(AutoHideBar);
        }
        
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var hideState = (GetTemplateChild(HideStateName) as VisualState);
            if (hideState != null)
                _hideAnimation = hideState.Storyboard.Children[0] as DoubleAnimation;
        }

        private void FindAndAttachScrollViewer(UIElement start)
        {
            _scroller = FindScroller(start);
            AttachScroller(_scroller);
        }

        private UIElement FindScroller(UIElement start)
        {
            UIElement target = null;

            if (IsScroller(start))
            {
                target = start;
            }
            else
            {
                var childCount = VisualTreeHelper.GetChildrenCount(start);

                for (var i = 0; i < childCount; i++)
                {
                    var el = VisualTreeHelper.GetChild(start, i) as UIElement;

                    target = IsScroller(start) ? el : FindScroller(el);

                    if (target != null)
                        break;
                }
            }

            return target;
        }

        private bool IsScroller(UIElement el)
        {
            return ((el is ScrollBar && ((ScrollBar)el).Orientation == Orientation.Vertical));
        }

        private void AttachScroller(UIElement scroller)
        {
            var bar = scroller as ScrollBar;
            if (bar != null)
            {
                bar.ValueChanged += scrollbar_ValueChanged;
            }
        }

        private void DetachScroller(UIElement scroller)
        {
            var bar = scroller as ScrollBar;
            if (bar != null)
            {
                bar.ValueChanged -= scrollbar_ValueChanged;
            }
        }

        void scrollbar_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            UpdateVisualState(e.NewValue);
        }
        
        private void UpdateVisualState(double value)
        {
            //Debug.WriteLine("Scrolling : " + value);

            if (ShowOnTop && value <= TopListOffsetDisplay)
            {
                _firstOffsetValue = value;
                Show();
            }
            else if (_firstOffsetValue - value < -MinimumOffsetScrollingDown) // scrolling down
            {
                _firstOffsetValue = value;
                Hide();
            }
            else // scrolling up
            {
                if (!(_firstOffsetValue - value > MinimumOffsetScrollingUp))
                    return;
                _firstOffsetValue = value;
                Show();
            }
        }

        private void Show()
        {
            VisualStateManager.GoToState(this, ShowStateName, true);
        }

        private void Hide()
        {
            if (_hideAnimation != null)
                _hideAnimation.To = -ActualHeight;

            VisualStateManager.GoToState(this, HideStateName, true);
        }
    }
}
