using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;
using WCFServiceWebRole1.Model;

namespace WCFServiceWebRole1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        //The Relay client is saving straight to the database so this shouldn't be needed.
        //Leaving it in if needed for testing
        //[OperationContract]
        //bool SaveDataAsync(DataModel data);
        
        [OperationContract]
        int[] GetLastRoomData();

        [OperationContract]
        int GetLastOvenData();
        
        [OperationContractAttribute(AsyncPattern = true)]
        IAsyncResult BeginReminderAsync(int desiredTemperature, AsyncCallback callback, object asyncState);
        bool EndReminderAsync(IAsyncResult result);
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
}
