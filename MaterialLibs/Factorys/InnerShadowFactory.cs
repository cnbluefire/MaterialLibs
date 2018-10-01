using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Composition;

namespace MaterialLibs.Factorys
{
    public class InnerShadowFactory
    {
        private InnerShadowFactory(Compositor Compositor)
        {
            this.Compositor = Compositor;
        }

        private DropShadow _Shadow;
        private SpriteVisual _ShadowHost;
        private Size _Size;
        private CanvasGeometry _MaskGeometry;

        private ICompositionSurface _surface;
        private CompositionSurfaceBrush _brush;

        public Compositor Compositor { get; }

        public Visual ShadowHost => _ShadowHost;

        public Size Size
        {
            get => _Size;
            set
            {
                _Size = value;
                UpdateSize();
            }
        }

        public CanvasGeometry MaskGeometry
        {
            get => _MaskGeometry;
            set
            {
                _MaskGeometry = value;
                UpdateMask();
            }
        }


        private void UpdateSize()
        {
            var realsize = new Size(Size.Width * 3, Size.Height * 3);

        }

        private void UpdateMask()
        {

        }

        private void UpdateShadow()
        {

        }
    }
}
