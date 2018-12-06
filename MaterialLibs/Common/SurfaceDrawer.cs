using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Composition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.Composition;
using Windows.UI.Xaml;

namespace MaterialLibs.Common
{
    public class SurfaceDrawer : IDisposable
    {
        private static object drawlocker = new object();

        public Compositor Compositor { get; }

        private CanvasDevice canvasDevice;
        private CompositionGraphicsDevice graphicsDevice;
        private Size Size => surface.Size;
        private CompositionDrawingSurface surface;
        private bool IsSurfaceCreator;
        private Action<CompositionDrawingSurface, CanvasDrawingSession> drawAction;

        public CompositionDrawingSurface Surface => surface;

        public SurfaceDrawer(Compositor Compositor, Size Size)
        {
            this.Compositor = Compositor;
            CreateDevices();
            surface = graphicsDevice.CreateDrawingSurface(Size, Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized, Windows.Graphics.DirectX.DirectXAlphaMode.Premultiplied);
            IsSurfaceCreator = true;
        }

        public SurfaceDrawer(Compositor Compositor, CompositionDrawingSurface Surface)
        {
            this.Compositor = Compositor;
            CreateDevices();
            surface = Surface;
            IsSurfaceCreator = false;
        }

        ~SurfaceDrawer()
        {
            Dispose(false);
        }

        private void CreateDevices()
        {
            canvasDevice = new CanvasDevice();
            graphicsDevice = CanvasComposition.CreateCompositionGraphicsDevice(Compositor, canvasDevice);

            canvasDevice.DeviceLost += CanvasDevice_DeviceLost;
            graphicsDevice.RenderingDeviceReplaced += GraphicsDevice_RenderingDeviceReplaced;
            DisplayInformation.DisplayContentsInvalidated += DisplayInformation_DisplayContentsInvalidated;
        }

        private void DisplayInformation_DisplayContentsInvalidated(DisplayInformation sender, object args)
        {
            TrySetCanvasDevice();
        }

        private void GraphicsDevice_RenderingDeviceReplaced(CompositionGraphicsDevice sender, RenderingDeviceReplacedEventArgs args)
        {
            Redraw();
        }

        private void CanvasDevice_DeviceLost(CanvasDevice sender, object args)
        {
            lock (drawlocker)
            {
                canvasDevice.DeviceLost -= CanvasDevice_DeviceLost;
                canvasDevice.Dispose();

                canvasDevice = new CanvasDevice();
                canvasDevice.DeviceLost += CanvasDevice_DeviceLost;
                TrySetCanvasDevice();
            }
        }

        private void TrySetCanvasDevice()
        {
            try
            {
                if (graphicsDevice != null)
                {
                    CanvasComposition.SetCanvasDevice(graphicsDevice, canvasDevice);
                }
            }
            catch (Exception e) when (canvasDevice != null && canvasDevice.IsDeviceLost(e.HResult))
            {
                canvasDevice.RaiseDeviceLost();
            }
        }

        private void OnDrawing()
        {
            if (drawAction != null)
            {
                lock (drawlocker)
                {
                    using (var session = CanvasComposition.CreateDrawingSession(surface))
                    {
                        drawAction?.Invoke(surface, session);
                    }
                }
            }
        }

        public void Resize(Size size)
        {
            CanvasComposition.Resize(surface, size);
        }

        public void Draw(Action<CompositionDrawingSurface, CanvasDrawingSession> DrawAction)
        {
            drawAction = DrawAction;
            OnDrawing();
        }

        public void Redraw()
        {
            OnDrawing();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool IsDisposing)
        {
            if (graphicsDevice != null)
            {
                graphicsDevice.Dispose();
                graphicsDevice = null;
            }
            if (canvasDevice != null)
            {
                canvasDevice.Dispose();
                canvasDevice = null;
            }
            if (IsSurfaceCreator && surface != null)
            {
                surface.Dispose();
                surface = null;
            }

            if (IsDisposing)
            {
                GC.SuppressFinalize(this);
            }
        }

    }

}
