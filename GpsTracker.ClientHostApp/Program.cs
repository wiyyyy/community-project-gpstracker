using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;

await Host.CreateDefaultBuilder()
        .UseOrleansClient(client =>
        {
            client.Configure<ClusterOptions>(options =>
            {
                options.ClusterId = "zort";
                options.ServiceId = "ef8e8eb3-1dc1-4e00-bb3e-8892565b5e36";
            }).UseRedisClustering(opt =>
            {
                opt.ConnectionString = "localhost:6379";
                opt.Database = 0;
            });
        }).ConfigureServices(services =>
        {
            services.AddSingleton<IHostedService, DataGenerator>();
        }).RunConsoleAsync();


