using System.Net.Security;
using System.Security.Authentication;
using Cassandra;

var options = new SSLOptions(
    sslProtocol: SslProtocols.Tls12,
    checkCertificateRevocation: true,
    remoteCertValidationCallback: (_, _, _, policyErrors) => policyErrors == SslPolicyErrors.None);

using var cluster = Cluster.Builder()
    .WithCredentials(
        username: "localhost",
        password: "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="
    )
    .WithPort(
        port: 10350
    )
    .AddContactPoint(
        address: "localhost"
    )
    .WithSSL(
        sslOptions: options
    )
    .Build();

using var session = cluster.Connect();

var createKeyspace = await session.PrepareAsync("CREATE KEYSPACE IF NOT EXISTS cosmicworks WITH replication = {'class':'basicclass', 'replication_factor': 1};");
await session.ExecuteAsync(createKeyspace.Bind());

var createTable = await session.PrepareAsync("CREATE TABLE IF NOT EXISTS cosmicworks.products (id text PRIMARY KEY, name text)");
await session.ExecuteAsync(createTable.Bind());

var item = new
{
    id = "68719518371",
    name = "Kiama classic surfboard"
};

var createItem = await session.PrepareAsync("INSERT INTO cosmicworks.products (id, name) VALUES (?, ?)");

var createItemStatement = createItem.Bind(item.id, item.name);

await session.ExecuteAsync(createItemStatement);
