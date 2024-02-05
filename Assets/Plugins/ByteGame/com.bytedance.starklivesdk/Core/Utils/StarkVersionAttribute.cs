using System;

namespace StarkLive
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class StarkVersionAttribute : Attribute
    {
        public string MinSCAndroidVersion { get; set; }
        public string MinCloudAndroidVersion { get; set; }

        public int MinAndroidOSVersion { get; set; }

        public string WebGLMethod { get; set; }
        
        public bool IsSupportWebGL { get; set; }
    }
}