using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace NeXt.BulkRenamer.Utility
{
    internal class VisibleRangeBehavior : Behavior<ItemsControl>
    {
        public static readonly DependencyProperty VisibleRangeProperty = DependencyProperty.RegisterAttached(
            "VisibleRange",
            typeof(VisibleRange),
            typeof(VisibleRangeBehavior),
            new FrameworkPropertyMetadata
            {
                BindsTwoWayByDefault = true
            }
        );
        
        public static void SetVisibleRange(DependencyObject obj, object value)
        {

            if (obj == null) throw new ArgumentNullException(nameof(obj));
            obj.SetValue(VisibleRangeProperty, value);
        }

        public static object GetVisibleRange(DependencyObject obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            return obj.GetValue(VisibleRangeProperty);
        }
        
        public VisibleRange VisibleRange
        {
            get => (VisibleRange)GetValue(VisibleRangeProperty);
            set => SetValue(VisibleRangeProperty, value);
        }
        
        protected override void OnDetaching()
        {
            AssociatedObject.RemoveHandler(ScrollViewer.ScrollChangedEvent, handler);
            handler = null;
            base.OnDetaching();
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            handler = ScrollChanged;
            AssociatedObject.AddHandler(ScrollViewer.ScrollChangedEvent, handler);
        }

        private ScrollChangedEventHandler handler;
        
        private void ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (AssociatedObject.Items.Count <= 0) return;

            var itemHeight = e.ExtentHeight/ AssociatedObject.Items.Count;
            if (itemHeight < 2d) return;

            var index = Math.Max((int)Math.Floor(e.VerticalOffset / itemHeight), 0);
            var count = Math.Min((int)Math.Ceiling(e.ViewportHeight / itemHeight), AssociatedObject.Items.Count - index);
            
            VisibleRange = new VisibleRange(index, count);
        }
    }
}