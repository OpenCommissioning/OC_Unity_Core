using System;
using System.Collections.Generic;

namespace OC.Data
{
    [Serializable]
    public class SceneSnapshot
    {
        public List<PayloadDescription> PayloadsDescription;
        public List<DeviceDescription> DevicesDescription;
        public List<ComponentDescription> ComponentsDescription;
    }
}
