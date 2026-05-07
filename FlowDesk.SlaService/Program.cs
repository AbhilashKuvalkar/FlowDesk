using FlowDesk.SlaService.Services;
using FlowDesk.TicketService.Domain.Repositories;
using FlowDesk.TicketService.Infrastructure.Persistence;
using FlowDesk.TicketService.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ISlaPolicyRepository, SlaPolicyRepository>();

var app = builder.Build();

app.MapGrpcService<SlaPolicyGrpcService>();

app.Run();
