using R2yChatSystem.IRepository;
using R2yChatSystem.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.AddGraphQL()
    .AddFiltering()
    .AddSorting()
    .AddTypes();

builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IChatRepository, ChatRepository>();

var app = builder.Build();

app.MapGraphQL();

app.RunWithGraphQLCommands(args);
