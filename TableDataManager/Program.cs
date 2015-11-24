using DataTableStorage.Model;
namespace TableDataManagerSample
{
    using TableDataManagerSample.Model;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;

   

    public class Program
    {       
        internal const string TableName = "SensorData";

        public static CloudTable GetTable()
        {
            CloudStorageAccount storageAccount = CreateStorageAccountFromConnectionString(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            return  tableClient.GetTableReference(TableName);
        }
        public static void Main(string[] args)
        {
           //SensorEntity s = new SensorEntity("ROOM",DateTime.Parse("24. 11. 2015 10:55:31"));
           //s.teperature = 20;
           //s.light = 30;
           // insert(s);       
           //   RetrieveEntityUsingPointQueryAsync(s.PartitionKey, s.RowKey);
           //   PartitionScanAsync(GetTable(), s.PartitionKey);          
        }

        /// <summary>
        /// Create a table for the sample application to process messages in. 
        /// </summary>
        /// <returns>A CloudTable object</returns>
        private static async Task<CloudTable> CreateTableAsync()
        {
            // Retrieve storage account information from connection string.
            CloudStorageAccount storageAccount = CreateStorageAccountFromConnectionString(CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create a table client for interacting with the table service
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            Console.WriteLine("1. Create a Table for the demo");

            // Create a table client for interacting with the table service 
            CloudTable table = tableClient.GetTableReference(TableName);
            try
            {
                if (await table.CreateIfNotExistsAsync())
                {
                    Console.WriteLine("Created Table named: {0}", TableName);
                }
                else
                {
                    Console.WriteLine("Table {0} already exists", TableName);
                }
            }
            catch (StorageException)
            {
                Console.WriteLine("If you are running with the default configuration please make sure you have started the storage emulator. Press the Windows key and type Azure Storage to select and run it from the list of applications - then restart the sample.");
                Console.ReadLine();
                throw;
            }

            return table;
        }

    
        
        private static CloudStorageAccount CreateStorageAccountFromConnectionString(string storageConnectionString)
        {
            CloudStorageAccount storageAccount;
            try
            {
                storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the application.");
                throw;
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.");
                Console.ReadLine();
                throw;
            }

            return storageAccount;
        }

        /// <summary>
        /// The Table Service supports two main types of insert operations. 
        ///  1. Insert - insert a new entity. If an entity already exists with the same PK + RK an exception will be thrown.
        ///  2. Replace - replace an existing entity. Replace an existing entity with a new entity. 
        ///  3. Insert or Replace - insert the entity if the entity does not exist, or if the entity exists, replace the existing one.
        ///  4. Insert or Merge - insert the entity if the entity does not exist or, if the entity exists, merges the provided entity properties with the already existing ones.
        /// </summary>
        /// <param name="table">The sample table name</param>
        /// <param name="entity">The entity to insert or merge</param>
        /// <returns></returns>
        private static async Task<SensorEntity> InsertOrMergeEntityAsync(SensorEntity sensorData)
        {
            CloudStorageAccount storageAccount = CreateStorageAccountFromConnectionString(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(TableName);
            if (sensorData == null)
            {
                throw new ArgumentNullException("sensorData");
            }

            // Create the InsertOrReplace  TableOperation
            TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(sensorData);

            // Execute the operation.
            TableResult result = await table.ExecuteAsync(insertOrMergeOperation);
            SensorEntity insertedSensorData = result.Result as SensorEntity;
            return insertedSensorData;
        }

        private static void insert(SensorEntity s)
        {
            CloudStorageAccount storageAccount = CreateStorageAccountFromConnectionString(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(TableName);

            
                TableOperation operation = TableOperation.Insert(s);
                table.Execute(operation);             
        }
        /// <summary>
        /// Demonstrate the most efficient storage query - the point query - where both partition key and row key are specified. 
        /// </summary>
        /// <param name="table">Sample table name</param>
        /// <param name="partitionKey">Partition key - ie - last name</param>
        /// <param name="rowKey">Row key - ie - first name</param>
        private static async Task<SensorEntity> RetrieveEntityUsingPointQueryAsync(string partitionKey, string rowKey)
        {
            CloudStorageAccount storageAccount = CreateStorageAccountFromConnectionString(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(TableName);
            TableOperation retrieveOperation = TableOperation.Retrieve<SensorEntity>(partitionKey, rowKey);
            TableResult result = await table.ExecuteAsync(retrieveOperation);
            SensorEntity sensor = result.Result as SensorEntity;
            if (sensor != null)
            {
                Console.WriteLine("\t{0}\t{1}\t{2}\t{3}", sensor.PartitionKey , sensor.RowKey, sensor.teperature, sensor.light);
            }

            return sensor;
        }

        /// <summary>
        /// Delete an entity
        /// </summary>
        /// <param name="table">Sample table name</param>
        /// <param name="deleteEntity">Entity to delete</param>
        private static async Task DeleteEntityAsync(CloudTable table, CustomerEntity deleteEntity)
        {
            if (deleteEntity == null)
            {
                throw new ArgumentNullException("deleteEntity");
            }

            TableOperation deleteOperation = TableOperation.Delete(deleteEntity);
            await table.ExecuteAsync(deleteOperation);
        }

        /// <summary>
        /// Demonstrate inserting of a large batch of entities. Some considerations for batch operations:
        ///  1. You can perform updates, deletes, and inserts in the same single batch operation.
        ///  2. A single batch operation can include up to 100 entities.
        ///  3. All entities in a single batch operation must have the same partition key.
        ///  4. While it is possible to perform a query as a batch operation, it must be the only operation in the batch.
        ///  5. Batch size must be <= 4MB
        /// </summary>
        /// <param name="table">Sample table name</param>
        private static async Task BatchInsertOfCustomerEntitiesAsync(CloudTable table)
        {
            // Create the batch operation. 
            TableBatchOperation batchOperation = new TableBatchOperation();

            // The following code  generates test data for use during the query samples.  
            for (int i = 0; i < 100; i++)
            {
                batchOperation.InsertOrMerge(new CustomerEntity("Smith", string.Format("{0}", i.ToString("D4")))
                {
                    Email = string.Format("{0}@contoso.com", i.ToString("D4")),
                    PhoneNumber = string.Format("425-555-{0}", i.ToString("D4"))
                });
            }

            // Execute the batch operation.
            IList<TableResult> results = await table.ExecuteBatchAsync(batchOperation);

            foreach (var res in results)
            {
                var customerInserted = res.Result as CustomerEntity;
                Console.WriteLine("Inserted entity with\t Etag = {0} and PartitionKey = {1}, RowKey = {2}", customerInserted.ETag, customerInserted.PartitionKey, customerInserted.RowKey);
            }

        }

        /// <summary>
        /// Demonstrate a partition range query whereby we are searching within a partition for a set of entities that are within a 
        /// specific range. The async API's require the user to implement paging themselves using continuation tokens. 
        /// </summary>
        /// <param name="table">Sample table name</param>
        /// <param name="partitionKey">The partition within which to search</param>
        /// <param name="startRowKey">The lowest bound of the row key range within which to search</param>
        /// <param name="endRowKey">The highest bound of the row key range within which to search</param>
        private static async Task PartitionRangeQueryAsync(CloudTable table, string partitionKey, string startRowKey, string endRowKey)
        {
            // Create the range query using the fluid API 
            TableQuery<CustomerEntity> rangeQuery = new TableQuery<CustomerEntity>().Where(
                TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey),
                        TableOperators.And,
                        TableQuery.CombineFilters(
                            TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual, startRowKey),
                            TableOperators.And,
                            TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThanOrEqual, endRowKey))));

            // Page through the results - requesting 50 results at a time from the server. 
            TableContinuationToken token = null;
            rangeQuery.TakeCount = 50;
            do
            {
                TableQuerySegment<CustomerEntity> segment = await table.ExecuteQuerySegmentedAsync(rangeQuery, token);
                token = segment.ContinuationToken;
                foreach (CustomerEntity entity in segment)
                {
                    Console.WriteLine("Customer: {0},{1}\t{2}\t{3}", entity.PartitionKey, entity.RowKey, entity.Email, entity.PhoneNumber);
                }
            }
            while (token != null);
        }

        /// <summary>
        /// Demonstrate a partition scan whereby we are searching for all the entities within a partition. Note this is not as efficient 
        /// as a range scan - but definitely more efficient than a full table scan. The async API's require the user to implement 
        /// paging themselves using continuation tokens. 
        /// </summary>
        /// <param name="table">Sample table name</param>
        /// <param name="partitionKey">The partition within which to search</param>
        private static async Task PartitionScanAsync(CloudTable table, string partitionKey)
        {
            TableQuery<SensorEntity> partitionScanQuery = new TableQuery<SensorEntity>().Where
                (TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));

            TableContinuationToken token = null;
            // Page through the results
            do
            {
                TableQuerySegment<SensorEntity> segment = await table.ExecuteQuerySegmentedAsync(partitionScanQuery, token);
                token = segment.ContinuationToken;
                foreach (SensorEntity entity in segment)
                {
                    Console.WriteLine("Customer: {0},{1}\t{2}\t{3}", entity.PartitionKey, entity.RowKey, entity.teperature, entity.light);
                }
            }
            while (token != null);
        }

        /// <summary>
        /// Delete a table
        /// </summary>
        /// <param name="table">Sample table name</param>
        private static async Task DeleteTableAsync(CloudTable table)
        {
            await table.DeleteIfExistsAsync();
        }
    }
}
