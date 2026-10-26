using VotePreferentiel;
using VotePreferentiel.Worker;

var builder = Host.CreateApplicationBuilder(args);

var timeoutMinutes = builder.Configuration.GetValue("Votes:TimeoutMinutes", 10);

builder.Services.AddHttpClient<VoteProvider>(client => client.Timeout = TimeSpan.FromMinutes(timeoutMinutes));
builder.Services.AddSingleton<IVoteCounter, VoteCounter>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
