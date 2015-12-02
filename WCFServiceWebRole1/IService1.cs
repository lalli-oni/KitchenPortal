using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using WCFServiceWebRole1.Model;

namespace WCFServiceWebRole1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {

        [OperationContract]
        string GetData(int value);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);

        [OperationContract]
        bool SaveDataAsync(DataModel data);

        [OperationContract]

        bool SetReminder(int temperature);

        // TODO: Add your service operations here
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        } 
    }

    //[DataContract]
    //public class SensorEntity : TableEntity
    //{

    //    public SensorEntity(string type, DateTime timeOfData)
    //    {
    //        this.PartitionKey = type;
    //        this.RowKey = timeOfData.ToString();
    //    }

    //    [DataMember]
    //    public int teperature { get; set; }
    //    [DataMember]
    //    public int light { get; set; }
    //}
}
