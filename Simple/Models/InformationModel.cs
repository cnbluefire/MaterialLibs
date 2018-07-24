using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace Simple.Models
{
    public class InformationModel
    {
        public string Version =>  string.Format("{0}.{1}.{2}.{3}",
                                    Package.Current.Id.Version.Major,
                                    Package.Current.Id.Version.Minor,
                                    Package.Current.Id.Version.Build,
                                    Package.Current.Id.Version.Revision);
        public string LibVersion => "0.1.14.0";
        public string github => "https://github.com/cnbluefire/MaterialLibs";
        public string blog => "https://www.cnblogs.com/blue-fire/";
        public string weibo => "https://weibo.com/u/2255001067";
        public string privacypolicy => "http://hotscreen.ultrabluefire.cn/privacy-policy.html";
    }
}
