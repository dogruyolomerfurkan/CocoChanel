using System.Threading.Channels;
using WebApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHostedService<Processor>();

//Unbounded: Sınır memory dolana kadardır.
builder.Services.AddSingleton<Channel<ChannelRequest>>(_ => Channel.CreateUnbounded<ChannelRequest>(new UnboundedChannelOptions
{
    SingleReader = true,
    AllowSynchronousContinuations = false
}));

//Bounded Capacity arka planda kaç tane sıralı işlem tutulacağını belirler.
//BoundedChannelFullMode.Wait:Bundan dolayı anlık birkaç istekte ilk istek direkt işlenir. 2. sıraya alınır. 3. istek atıldığında istek beklemede kalır(Capacity 1 varsayıldı)
//DropNewest: En son ekleneni listeden atar
//DropOldest: İlk ekleneni listeden atar
//DropWrite: Liste dolduğu anda diğerlerini işlemden atar.
// builder.Services.AddSingleton<Channel<ChannelRequest>>(_ => Channel.CreateBounded<ChannelRequest>(new BoundedChannelOptions(2)
// {
//     FullMode = BoundedChannelFullMode.Wait,
//     SingleReader = true,
//     AllowSynchronousContinuations = false
// }));

//CreateUnboundedPrioritized Comparerda belirtilen önem sırası karşılaştırmasına göre sıradaki işlemleri önceliklendirir.
// builder.Services.AddSingleton<Channel<ChannelRequest>>(_ => Channel.CreateUnboundedPrioritized(new UnboundedPrioritizedChannelOptions<ChannelRequest>
// {
//     Comparer = Comparer<ChannelRequest>.Create((req1,req2)=> req1.Message.Length.CompareTo(req2.Message.Length)),
//     SingleReader = true,
//     AllowSynchronousContinuations = false
// }));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();