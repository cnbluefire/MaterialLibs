using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace MaterialLibs.Factorys
{
    public abstract class EventPipeBase<TArgs> : IDisposable
    {
        public EventPipeBase(object obj, string EventName, bool handledEventsToo = false)
        {
            Source = obj;
            this.EventName = EventName;
            this.handledEventsToo = handledEventsToo;
            EventInfo = Source.GetType().GetRuntimeEvent(EventName);

            if (EventInfo == null)
            {
                throw new ArgumentNullException($"找不到{EventName}");
            }

            var onEventInfo = typeof(EventPipeBase<TArgs>).GetTypeInfo().GetDeclaredMethod("OnEvent");
            _onEventHandler = onEventInfo.CreateDelegate(EventInfo.EventHandlerType, this);
            isWindowsRuntimeEvent = IsWindowsRuntimeEvent(EventInfo);

            if (handledEventsToo)
            {
                if (Source is UIElement ele)
                {
                    var property = typeof(UIElement).GetRuntimeProperty(EventName + "Event");

                    ele.AddHandler((RoutedEvent)property.GetValue(null), _onEventHandler, handledEventsToo);
                }
            }
            else
            {
                if (isWindowsRuntimeEvent)
                {
                    this.addEventHandlerMethod = add => (EventRegistrationToken)EventInfo.AddMethod.Invoke(this.Source, new object[] { add });
                    this.removeEventHandlerMethod = token => EventInfo.RemoveMethod.Invoke(this.Source, new object[] { token });

                    WindowsRuntimeMarshal.AddEventHandler(this.addEventHandlerMethod, this.removeEventHandlerMethod, this._onEventHandler);
                }
                else
                {
                    EventInfo.AddEventHandler(obj, _onEventHandler);
                }
            }
        }

        protected abstract void OnEventAttachedCore(EventAttachedArgs args);

        protected void OnEventAttached(TArgs args)
        {
            var eventAttachedArgs = new EventAttachedArgs();
            OnEventAttachedCore(eventAttachedArgs);
            if (!eventAttachedArgs.Canceled)
            {
                EventAttached?.Invoke(Source, args);
            }
        }

        protected void OnEvent(object sender, object args)
        {
            OnEventAttached((TArgs)args);
        }

        public object Source { get; set; }
        public string EventName { get; set; }
        protected EventInfo EventInfo { get; set; }

        private Delegate _onEventHandler;
        private bool isWindowsRuntimeEvent;

        private Func<Delegate, EventRegistrationToken> addEventHandlerMethod;
        private Action<EventRegistrationToken> removeEventHandlerMethod;

        private bool handledEventsToo;

        public event TypedEventHandler<object, TArgs> EventAttached;

        private static bool IsWindowsRuntimeEvent(EventInfo eventInfo)
        {
            return eventInfo != null &&
                IsWindowsRuntimeType(eventInfo.EventHandlerType) &&
                IsWindowsRuntimeType(eventInfo.DeclaringType);
        }

        private static bool IsWindowsRuntimeType(Type type)
        {
            if (type != null)
            {
                return type.AssemblyQualifiedName.EndsWith("ContentType=WindowsRuntime", StringComparison.Ordinal);
            }

            return false;
        }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                    if (EventInfo != null)
                    {
                        if (handledEventsToo)
                        {
                            if (Source is UIElement ele)
                            {
                                var property = typeof(UIElement).GetRuntimeProperty(EventName + "Event");

                                ele.RemoveHandler((RoutedEvent)property.GetValue(null), _onEventHandler);
                            }
                        }
                        else
                        {
                            if (isWindowsRuntimeEvent)
                            {
                                WindowsRuntimeMarshal.RemoveEventHandler(this.removeEventHandlerMethod, this._onEventHandler);
                            }
                            else
                            {
                                EventInfo.RemoveEventHandler(Source, _onEventHandler);
                            }
                        }
                    }
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~EventPipeBase() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }

    public class EventAttachedArgs : EventArgs
    {
        public bool Canceled { get; set; }
    }
}
