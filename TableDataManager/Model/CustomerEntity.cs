
namespace TableDataManagerSample.Model
{
    using Microsoft.WindowsAzure.Storage.Table;

    
    public class CustomerEntity : TableEntity
    {
        // Your entity type must expose a parameter-less constructor
        public CustomerEntity() { }

        // Define the PK and RK
        public CustomerEntity(string lastName, string firstName)
        {
            this.PartitionKey = lastName;
            this.RowKey = firstName;
        }

        //For any property that should be stored in the table service, the property must be a public property of a supported type that exposes both get and set.        
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
