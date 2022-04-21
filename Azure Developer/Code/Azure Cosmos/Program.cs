using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

public class Program
{
    // Replace <documentEndpoint> with the information created earlier
    private static readonly string EndpointUri = "document endpoint";

    // Set variable to the Primary Key from earlier.
    private static readonly string PrimaryKey = "primary key value";

    // The Cosmos client instance
    private CosmosClient cosmosClient;

    // The database we will create
    private Database database;

    // The container we will create.
    private Container container;

    // The names of the database and container we will create
    private string databaseId = "az204Database";
    private string containerId = "az204Container";

    public static async Task Main(string[] args)
    {
        try
        {
            Console.WriteLine("Beginning operations...\n");
            Program p = new Program();
            await p.CosmosAsync();

        }
        catch (CosmosException de)
        {
            Exception baseException = de.GetBaseException();
            Console.WriteLine("{0} error occurred: {1}", de.StatusCode, de);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: {0}", e);
        }
        finally
        {
            Console.WriteLine("End of program, press any key to exit.");
            Console.ReadKey();
        }
    }

    public async Task CosmosAsync()
    {
        // Create a new instance of the Cosmos Client
        this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
        await this.CreateDatabaseAsync();
        await this.CreateContainerAsync();
        await this.InsertItem();
        await this.ReadItem();
        await this.DeleteItem();
    }

    private async Task CreateDatabaseAsync()
    {
        // Create a new database using the cosmosClient
        this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
        Console.WriteLine("Created database: {0}\n", this.database.Id);
    }

    private async Task CreateContainerAsync()
    {
        // Create a new container
        this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/LastName");
        Console.WriteLine("Created Container: {0}\n", this.container.Id);
    }

    private async Task InsertItem()
    {
        // Insert an item into the container
        ToDoActivity test = new ToDoActivity()
        {
            id = "42",
            status = "InProgress",
            LastName = "LastName"
        };

        ItemResponse<ToDoActivity> item = await this.container.CreateItemAsync<ToDoActivity>(test, new PartitionKey(test.LastName));
    }

    // TODO: This doesn't work...
    private async Task ReadItem()
    {
        // Read an item from the container
        ToDoActivity response = await this.container.ReadItemAsync<ToDoActivity>("42", new PartitionKey("LastName"));
        Console.WriteLine("Item found: ID: {0}, Status: {1}", response.id, response.status);
    }

    // TODO: Finish this
    private async Task QueryItem()
    {
        // Query an item from the container
        QueryDefinition query = new QueryDefinition("select * from c where c.LastName = @LastName").WithParameter("@LastName", "Test!");

        FeedIterator<ToDoActivity> resultSet = this.container.GetItemQueryIterator<ToDoActivity>(query, requestOptions: new QueryRequestOptions()
        {
            PartitionKey = new PartitionKey("LastName"),
            MaxItemCount = 1
        });
    }

    // TODO: This doesn't work...
    private async Task DeleteItem()
    {
        ItemResponse<ToDoActivity> item = await this.container.DeleteItemAsync<ToDoActivity>("42", new PartitionKey("LastName"));
    }
}

public class ToDoActivity
{
    public string id { get; set; }
    public string status { get; set; }
    public string LastName { get; set; }
}