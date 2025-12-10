using R2yChatSystem.IRepository;
using R2yChatSystem.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.AddGraphQL()
    .AddFiltering()
    .AddSorting()
    .AddTypes();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();

var app = builder.Build();

app.MapGraphQL();

app.RunWithGraphQLCommands(args);
