//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4959
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

[assembly: global::System.Reflection.AssemblyVersionAttribute("1.0.0.0")]
[assembly: global::System.Reflection.AssemblyProductAttribute("MyHexaPodService")]
[assembly: global::System.Reflection.AssemblyTitleAttribute("MyHexaPodService")]
[assembly: global::Microsoft.Dss.Core.Attributes.ServiceDeclarationAttribute(global::Microsoft.Dss.Core.Attributes.DssServiceDeclaration.Transform, SourceAssemblyKey="MyHexaPodService.Y2011.M05, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6111" +
    "f4f52f855ee1")]
[assembly: global::System.Security.SecurityTransparentAttribute()]
[assembly: global::System.Security.AllowPartiallyTrustedCallersAttribute()]

namespace Dss.Transforms.TransformMyHexaPodService {
    
    
    public class Transforms : global::Microsoft.Dss.Core.Transforms.TransformBase {
        
        static Transforms() {
            Register();
        }
        
        public static void Register() {
            global::Microsoft.Dss.Core.Transforms.TransformBase.AddProxyTransform(typeof(global::MyHexaPodService.Proxy.MyHexaPodServiceState), new global::Microsoft.Dss.Core.Attributes.Transform(MyHexaPodService_Proxy_MyHexaPodServiceState_TO_MyHexaPodService_MyHexaPodServiceState));
            global::Microsoft.Dss.Core.Transforms.TransformBase.AddSourceTransform(typeof(global::MyHexaPodService.MyHexaPodServiceState), new global::Microsoft.Dss.Core.Attributes.Transform(MyHexaPodService_MyHexaPodServiceState_TO_MyHexaPodService_Proxy_MyHexaPodServiceState));
        }
        
        private static global::MyHexaPodService.Proxy.MyHexaPodServiceState _cachedInstance0 = new global::MyHexaPodService.Proxy.MyHexaPodServiceState();
        
        private static global::MyHexaPodService.MyHexaPodServiceState _cachedInstance = new global::MyHexaPodService.MyHexaPodServiceState();
        
        public static object MyHexaPodService_Proxy_MyHexaPodServiceState_TO_MyHexaPodService_MyHexaPodServiceState(object transformFrom) {
            return _cachedInstance;
        }
        
        public static object MyHexaPodService_MyHexaPodServiceState_TO_MyHexaPodService_Proxy_MyHexaPodServiceState(object transformFrom) {
            return _cachedInstance0;
        }
    }
}
