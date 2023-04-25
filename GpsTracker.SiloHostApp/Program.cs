using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;

try
{
    foreach (var item in args)
    {
        Console.WriteLine(item);
    }
    using IHost host = await StartSiloAsync(args);
    
    Console.WriteLine("\n\n Press Enter to terminate...\n\n");
    Console.ReadLine();

    await host.StopAsync();

    return 0;
}
catch (Exception ex)
{
    Console.WriteLine(ex);
    return 1;
}

static async Task<IHost> StartSiloAsync(string[] args)
{
    var builder = Host.CreateDefaultBuilder(args)
        .UseOrleans((ctx, silo )=>
        {
            foreach (var item in ctx.Configuration.AsEnumerable())
            {
                Console.WriteLine(string.Join(" ", item.Key,item.Value));
            }
            int instanceId = ctx.Configuration.GetValue<int>("InstanceId");
            Console.WriteLine(instanceId);
            silo.Configure<ClusterOptions>(x =>
             {
                 x.ClusterId = "zort";
                 x.ServiceId = "ef8e8eb3-1dc1-4e00-bb3e-8892565b5e36";
             })
            .ConfigureEndpoints(siloPort: 11111+instanceId, gatewayPort: 30000+instanceId)
            .UseRedisClustering(opt =>
            {
                opt.ConnectionString = "localhost:6379";
                opt.Database = 0;
            })
            .AddRedisGrainStorage("gpsDataStore", optionsBuilder => optionsBuilder.Configure(options =>
            {
                options.ConnectionString = "localhost:6379";
                options.DatabaseNumber = 1;
            }));
        });

    var host = builder.Build();
    await host.StartAsync();

    return host;
}