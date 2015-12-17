using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace RelayLayer.Model
{
    [DataContract]
    class SensorEntity
    {
        public SensorEntity() { }
        
        [DataMember]
        public int teperature { get; set; }
        [DataMember]
        public int light { get; set; }
    }
}
