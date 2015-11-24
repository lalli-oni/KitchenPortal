using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace DataTableStorage.Model
{
    class SensorEntity : TableEntity
    {
        public SensorEntity() { }

        public SensorEntity(string type, DateTime timeOfData)
        {
            this.PartitionKey = type;
            this.RowKey = timeOfData.ToString();
        }
        
        public int teperature { get; set; }
        public int light { get; set; }
    }
}
