using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialLibs.Animations
{
    public interface IAnimationKeyFrameBase : INotifyPropertyChanged
    {
        double Progress { get; set; }
        string Value { get; set; }
    }
}
