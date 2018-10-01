using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Composition;

namespace MaterialLibs.Models
{
    public class DelayExpressionAnimationSource<T> where T : DelayExpressionAnimationSource<T>, new()
    {
        private TimeSpan _periodDuration = TimeSpan.FromSeconds(0.01d);
        private float _attenuationPecent = 0.5f;
        private string _expression;
        private string _target;

        public Compositor Compositor { get; protected set; }

        public KeyFrameAnimation Animation { get; protected set; }

        /// <summary>
        /// 每周期持续时间，默认为0.01秒
        /// </summary>
        public TimeSpan PeriodDuration
        {
            get => _periodDuration;
            set
            {
                _periodDuration = value;
                if (Animation != null)
                {
                    Animation.Duration = PeriodDuration;
                }
            }
        }

        /// <summary>
        /// 每周期衰减，取值从0到1，当为1时无延迟效果，默认为0.5
        /// </summary>
        public float AttenuationPecent
        {
            get => _attenuationPecent;
            set
            {
                _attenuationPecent = value;
                if (Animation != null)
                {
                    Animation.SetScalarParameter("progress", AttenuationPecent);
                }
            }
        }

        public string Expression
        {
            get => _expression;
            set
            {
                _expression = value;
                if (Animation != null)
                {
                    Animation.InsertExpressionKeyFrame(0f, "This.CurrentValue");
                    Animation.InsertExpressionKeyFrame(1f, $"Lerp({Expression}, This.CurrentValue, progress)");
                }
            }
        }

        public string Target
        {
            get => _target;
            set
            {
                _target = value;
                if (Animation != null)
                {
                    Animation.Target = _target;
                }
            }
        }

        protected virtual void CreateAnimation() { }

        public void ClearAllParameters()
        {
            if (Animation != null)
            {
                Animation.ClearAllParameters();
            }
        }
        public void ClearParameter(string key)
        {
            if (Animation != null)
            {
                Animation.ClearParameter(key);
            }
        }
        public void SetColorParameter(string key, Color value)
        {
            if (Animation != null)
            {
                Animation.SetColorParameter(key, value);
            }
        }
        public void SetMatrix3x2Parameter(string key, Matrix3x2 value)
        {
            if (Animation != null)
            {
                Animation.SetMatrix3x2Parameter(key, value);
            }
        }
        public void SetMatrix4x4Parameter(string key, Matrix4x4 value)
        {
            if (Animation != null)
            {
                Animation.SetMatrix4x4Parameter(key, value);
            }
        }
        public void SetQuaternionParameter(string key, Quaternion value)
        {
            if (Animation != null)
            {
                Animation.SetQuaternionParameter(key, value);
            }
        }
        public void SetReferenceParameter(string key, CompositionObject compositionObject)
        {
            if (Animation != null)
            {
                Animation.SetReferenceParameter(key, compositionObject);
            }
        }
        public void SetScalarParameter(string key, float value)
        {
            if (Animation != null)
            {
                Animation.SetScalarParameter(key, value);
            }
        }
        public void SetVector2Parameter(string key, Vector2 value)
        {
            if (Animation != null)
            {
                Animation.SetVector2Parameter(key, value);
            }
        }
        public void SetVector3Parameter(string key, Vector3 value)
        {
            if (Animation != null)
            {
                Animation.SetVector3Parameter(key, value);
            }
        }
        public void SetVector4Parameter(string key, Vector4 value)
        {
            if (Animation != null)
            {
                Animation.SetVector4Parameter(key, value);
            }
        }
        public void SetBooleanParameter(string key, bool value)
        {
            if (Animation != null)
            {
                Animation.SetBooleanParameter(key, value);
            }
        }


        public static T Create(Compositor compositor)
        {
            var t = new T();
            t.Compositor = compositor;
            t.CreateAnimation();
            t.Animation.Duration = t.PeriodDuration;
            t.Animation.SetScalarParameter("progress", t.AttenuationPecent);
            return t;
        }
    }

    public class DelayScalarExpressionAnimationSource : DelayExpressionAnimationSource<DelayScalarExpressionAnimationSource>
    {
        protected override void CreateAnimation()
        {
            Animation = Compositor.CreateScalarKeyFrameAnimation();
            Animation.IterationBehavior = AnimationIterationBehavior.Forever;
        }
    }

    public class DelayVector2ExpressionAnimationSource : DelayExpressionAnimationSource<DelayVector2ExpressionAnimationSource>
    {
        protected override void CreateAnimation()
        {
            Animation = Compositor.CreateVector2KeyFrameAnimation();
            Animation.IterationBehavior = AnimationIterationBehavior.Forever;
        }
    }

    public class DelayVector3ExpressionAnimationSource : DelayExpressionAnimationSource<DelayVector3ExpressionAnimationSource>
    {
        protected override void CreateAnimation()
        {
            Animation = Compositor.CreateVector3KeyFrameAnimation();
            Animation.IterationBehavior = AnimationIterationBehavior.Forever;
        }
    }

    public class DelayVector4ExpressionAnimationSource : DelayExpressionAnimationSource<DelayVector4ExpressionAnimationSource>
    {
        protected override void CreateAnimation()
        {
            Animation = Compositor.CreateVector4KeyFrameAnimation();
            Animation.IterationBehavior = AnimationIterationBehavior.Forever;
        }
    }

    public class DelayQuaternionExpressionAnimationSource : DelayExpressionAnimationSource<DelayQuaternionExpressionAnimationSource>
    {
        protected override void CreateAnimation()
        {
            Animation = Compositor.CreateQuaternionKeyFrameAnimation();
            Animation.IterationBehavior = AnimationIterationBehavior.Forever;
        }
    }

    public class DelayColorExpressionAnimationSource : DelayExpressionAnimationSource<DelayColorExpressionAnimationSource>
    {
        protected override void CreateAnimation()
        {
            Animation = Compositor.CreateColorKeyFrameAnimation();
            Animation.IterationBehavior = AnimationIterationBehavior.Forever;
        }
    }
}
