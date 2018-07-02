using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace MaterialLibs.Controls
{
    public class BigbangPanel : Panel
    {
        public BigbangPanel()
        {
            var OffsetAnimation = compositor.CreateVector3KeyFrameAnimation();
            OffsetAnimation.InsertExpressionKeyFrame(1f, "this.FinalValue");
            OffsetAnimation.Duration = TimeSpan.FromSeconds(0.33d);
            OffsetAnimation.StopBehavior = AnimationStopBehavior.SetToFinalValue;
            OffsetAnimation.Target = "Offset";

            _ContainerImplicitAnimations = compositor.CreateImplicitAnimationCollection();
            _ContainerImplicitAnimations["Offset"] = OffsetAnimation;

            ElementCompositionPreview.GetElementVisual(this).ImplicitAnimations = _ContainerImplicitAnimations;
        }

        private Compositor compositor => Window.Current.Compositor;
        private ImplicitAnimationCollection _ContainerImplicitAnimations;
        private int _StartSelectIndex = -1;
        private int _EndSelectIndex = -1;
        private double _ContentHeaderHeight = 0;
        private double _ContentHeaderRectTop = 0;
        private double _ContentHeaderRectBottom = 0;
        private double _ContentFooterHeight = 0;
        private double _ContentFooterRectTop = 0;
        private double _ContentFooterRectBottom = 0;
        private Dictionary<UIElement, Rect> _ChildrenRects;

        public int StartSelectIndex
        {
            get => _StartSelectIndex;
            set
            {
                _StartSelectIndex = value;
                InvalidateMeasure();
                InvalidateArrange();
            }
        }
        public int EndSelectIndex
        {
            get => _EndSelectIndex;
            set
            {
                _EndSelectIndex = value;
                InvalidateMeasure();
                InvalidateArrange();
            }
        }
        public double ContentHeaderHeight
        {
            get => _ContentHeaderHeight;
            set
            {
                if (_ContentHeaderHeight != value)
                {
                    _ContentHeaderHeight = value;
                    InvalidateMeasure();
                    OnCommandRectChanged(new CommandBarChangedArgs(CommandBarMode.Header, _ContentHeaderRectTop, _ContentHeaderRectBottom, ContentHeaderHeight));
                }
            }
        }
        public double ContentHeaderRectTop
        {
            get => _ContentHeaderRectTop;
            set
            {
                if (_ContentHeaderRectTop != value)
                {
                    _ContentHeaderRectTop = value;
                    OnCommandRectChanged(new CommandBarChangedArgs(CommandBarMode.Header, _ContentHeaderRectTop, _ContentHeaderRectBottom, ContentHeaderHeight));
                }
            }
        }
        public double ContentHeaderRectBottom
        {
            get => _ContentHeaderRectBottom;
            set
            {
                if (_ContentHeaderRectBottom != value)
                {
                    _ContentHeaderRectBottom = value;
                    OnCommandRectChanged(new CommandBarChangedArgs(CommandBarMode.Header, _ContentHeaderRectTop, _ContentHeaderRectBottom, ContentHeaderHeight));
                }
            }
        }

        public double ContentFooterHeight
        {
            get => _ContentFooterHeight;
            set
            {
                if (_ContentFooterHeight != value)
                {
                    _ContentFooterHeight = value;
                    InvalidateMeasure();
                    OnCommandRectChanged(new CommandBarChangedArgs(CommandBarMode.Footer, _ContentFooterRectTop, _ContentFooterRectBottom, _ContentFooterHeight));
                }
            }
        }
        public double ContentFooterRectTop
        {
            get => _ContentFooterRectTop;
            set
            {
                if (_ContentFooterRectTop != value)
                {
                    _ContentFooterRectTop = value;
                    OnCommandRectChanged(new CommandBarChangedArgs(CommandBarMode.Footer, _ContentFooterRectTop, _ContentFooterRectBottom, _ContentFooterHeight));
                }
            }
        }
        public double ContentFooterRectBottom
        {
            get => _ContentFooterRectBottom;
            set
            {
                if (_ContentFooterRectBottom != value)
                {
                    _ContentFooterRectBottom = value;
                    OnCommandRectChanged(new CommandBarChangedArgs(CommandBarMode.Footer, _ContentFooterRectTop, _ContentFooterRectBottom, _ContentFooterHeight));
                }
            }
        }

        public Dictionary<UIElement, Rect> ChildrenRects
        {
            get
            {
                if (_ChildrenRects == null) _ChildrenRects = new Dictionary<UIElement, Rect>();
                return _ChildrenRects;
            }
            set => _ChildrenRects = value;
        }

        public event TypedEventHandler<object, CommandBarChangedArgs> CommandRectChanged;

        private void OnCommandRectChanged(CommandBarChangedArgs args)
        {
            CommandRectChanged?.Invoke(this, args);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (var child in Children)
            {
                child.Measure(availableSize);
            }

            double width = 0d, height = 0d;
            double col_width = 0d, row_height = 0d;
            int end_row_count = -1;

            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i].DesiredSize.Width + col_width > availableSize.Width)
                {
                    end_row_count = i;
                    height += row_height;
                    width = Math.Max(width, col_width);
                    col_width = 0;
                    row_height = 0;
                }
                if (i == StartSelectIndex)
                {
                    end_row_count = i;
                    height += ContentHeaderHeight;
                }
                if (i == EndSelectIndex)
                {
                    end_row_count = i;
                    height += ContentFooterHeight;
                }
                col_width += Children[i].DesiredSize.Width;
                row_height = Math.Max(row_height, Children[i].DesiredSize.Height);
            }
            if (end_row_count != -1)
            {
                col_width = 0;
                row_height = 0;
                for (int i = end_row_count; i < Children.Count; i++)
                {
                    row_height = Math.Max(row_height, Children[i].DesiredSize.Height);
                    col_width += Children[i].DesiredSize.Width;
                }
                height += row_height;
                width = Math.Max(width, col_width);
            }

            return new Size(width, height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            ChildrenRects.Clear();
            double x = 0d, y = 0d;
            double items_height = 0d;
            int end_count = -1;
            int row_start_index = 0;
            bool is_end_selected_row = false;
            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i].DesiredSize.Width + x > finalSize.Width)
                {
                    x = 0;
                    y += items_height;

                    if (is_end_selected_row)
                    {
                        ContentFooterRectTop = y;
                        y += _ContentFooterHeight;
                        ContentFooterRectBottom = y;
                        is_end_selected_row = false;
                    }

                    items_height = 0;
                    end_count = i;
                    row_start_index = i;
                }

                if (StartSelectIndex != EndSelectIndex)
                {
                    if (EndSelectIndex == -1) _EndSelectIndex = StartSelectIndex;
                    if (StartSelectIndex == -1) _StartSelectIndex = EndSelectIndex;
                }

                if (StartSelectIndex == i)
                {
                    x = 0;
                    ContentHeaderRectTop = y;
                    y += ContentHeaderHeight;
                    ContentHeaderRectBottom = y;
                    for (int j = row_start_index; j < i; j++)
                    {
                        var tmp_rect = new Rect(x, y, Children[j].DesiredSize.Width, Children[j].DesiredSize.Height);
                        Children[j].Arrange(tmp_rect);
                        ChildrenRects[Children[j]] = tmp_rect;
                        x += Children[j].DesiredSize.Width;
                        items_height = Math.Max(items_height, Children[j].DesiredSize.Height);
                    }
                }
                if (EndSelectIndex == i)
                {
                    is_end_selected_row = true;
                }

                var rect = new Rect(x, y, Children[i].DesiredSize.Width, Children[i].DesiredSize.Height);
                Children[i].Arrange(rect);
                ChildrenRects[Children[i]] = rect;
                x += Children[i].DesiredSize.Width;
                items_height = Math.Max(items_height, Children[i].DesiredSize.Height);
            }

            x = 0;
            y += items_height;
            if (is_end_selected_row)
            {
                ContentFooterRectTop = y;
                y += _ContentFooterHeight;
                ContentFooterRectBottom = y;
                is_end_selected_row = false;
            }

            return finalSize;
        }
    }

    public class CommandBarChangedArgs
    {
        public CommandBarChangedArgs(CommandBarMode mode, double top, double bottom, double height)
        {
            Mode = mode;
            Top = top;
            Bottom = bottom;
            Height = height;
        }

        public double Top { get; set; }
        public double Bottom { get; set; }
        public double Height { get; set; }
        public CommandBarMode Mode { get; set; }
    }

    public enum CommandBarMode
    {
        Header, Footer
    }
}
