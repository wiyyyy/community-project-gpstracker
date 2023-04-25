using GpsTracker.Presentation.Hubs;
using Orleans.Configuration;

var builder = WebApplication.CreateBuilder(args);


builder.Host.UseOrleansClient(client =>
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
});

builder.Services.AddSignalR();
builder.Services.AddHostedService<HubLocationPublisher>();
// Channel grainden verileri alıp hubcontext ile broadcast yapacak

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseDefaultFiles();
app.UseRouting();
app.UseAuthorization();
app.MapHub<LocationHub>("/locationHub");

await app.RunAsync();
