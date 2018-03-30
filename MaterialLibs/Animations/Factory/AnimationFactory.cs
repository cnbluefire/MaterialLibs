using MaterialLibs.Animations.EasingFunction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;

namespace MaterialLibs.Animations
{
    public class AnimationFactory : IAnimationFactoryBase
    {
        public Compositor Compositor => Window.Current.Compositor;
        private KeyFrameAnimation _CompositionAnimation;
        private TimeSpan _Duration;
        private TimeSpan _DelayTime;
        private string _Target;
        public ICompositionAnimationBase ContentAnimation
        {
            get => _CompositionAnimation;
        }

        public AnimationFactory Create(AnimationMode mode, EasingFunctionBase EasingFunction, bool rebuild = false)
        {
            if (rebuild || _CompositionAnimation == null)
            {
                switch (mode)
                {
                    case AnimationMode.Scalar:
                        {
                            _CompositionAnimation = Compositor.CreateScalarKeyFrameAnimation();
                        }
                        break;
                    case AnimationMode.Vector2:
                        {
                            _CompositionAnimation = Compositor.CreateVector2KeyFrameAnimation();
                        }
                        break;
                    case AnimationMode.Vector3:
                        {
                            _CompositionAnimation = Compositor.CreateVector3KeyFrameAnimation();
                        }
                        break;
                    case AnimationMode.Vector4:
                        {
                            _CompositionAnimation = Compositor.CreateVector4KeyFrameAnimation();
                        }
                        break;
                    case AnimationMode.Quaternion:
                        {
                            _CompositionAnimation = Compositor.CreateQuaternionKeyFrameAnimation();
                        }
                        break;
                    case AnimationMode.Color:
                        {
                            _CompositionAnimation = Compositor.CreateColorKeyFrameAnimation();
                        }
                        break;
                }
                if (_Duration != null)
                    _CompositionAnimation.Duration = _Duration;
                if (_DelayTime != null)
                    _CompositionAnimation.DelayTime = _DelayTime;
                if (!string.IsNullOrWhiteSpace(_Target))
                    _CompositionAnimation.Target = _Target;
            }
            return this;
        }

        public AnimationFactory Duration(TimeSpan time)
        {
            if (time != null)
            {
                _Duration = time;
                if (_CompositionAnimation != null) _CompositionAnimation.Duration = time;
            }
            return this;
        }

        public AnimationFactory DelayTime(TimeSpan time)
        {
            if (time != null)
            {
                _DelayTime = time;
                if (_CompositionAnimation != null) _CompositionAnimation.DelayTime = time;
            }
            return this;
        }

        public AnimationFactory Target(string target)
        {
            if (!string.IsNullOrWhiteSpace(target))
            {
                _Target = target;
                if (_CompositionAnimation != null) _CompositionAnimation.Target = target;
            }
            return this;
        }
    }

    public enum AnimationMode
    {
        Scalar, Vector2, Vector3, Vector4, Quaternion, Color
    }

    public enum AnimationEasingMode
    {
        Linear, Step, CubicBezier
    }
}
