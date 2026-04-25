using FlowDesk.TicketService.Behaviours;
using FlowDesk.TicketService.Domain.Common;
using FlowDesk.TicketService.Domain.Enums;
using FlowDesk.TicketService.Domain.Repositories;
using FlowDesk.TicketService.Features.Tickets.Commands.AssignTicket;
using FlowDesk.TicketService.Features.Tickets.Commands.CreateAgent;
using FlowDesk.TicketService.Features.Tickets.Commands.CreateTicket;
using FlowDesk.TicketService.Features.Tickets.Queries.GetTicketById;
using FlowDesk.TicketService.Features.Tickets.Queries.GetTicketsByStatus;
using FlowDesk.TicketService.Infrastructure.Caching;
using FlowDesk.TicketService.Infrastructure.Persistence;
using FlowDesk.TicketService.Infrastructure.Persistence.Repositories;
using FlowDesk.TicketService.Middlewares;
using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddTransient(
    typeof(IPipelineBehavior<,>),
    typeof(ValidationBehaviour<,>)
);

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = $"{nameof(FlowDesk)}:";
});

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMQ"), h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ConfigureEndpoints(ctx);
    });
});

builder.Services.AddScoped<IAgentRepository, AgentRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());
builder.Services.AddScoped<ICacheService, RedisCacheService>();
builder.Services.AddScoped<ISlaPolicyRepository, SlaPolicyRepository>();

var app = builder.Build();

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/tickets", async (CreateTicketCommand command, ISender sender) =>
{
    var id = await sender.Send(command);
    return Results.Created($"/tickets/{id}", new { id });
});

app.MapGet("/tickets/{id:guid}", async (Guid id, Guid tenantId, ISender sender) =>
{
    var result = await sender.Send(new GetTicketByIdQuery(id, tenantId));
    return result is null ? Results.NotFound() : Results.Ok(result);
});

app.MapPost("/tickets/assign", async (AssignTicketCommand command, ISender sender) =>
{
    await sender.Send(command);
    return Results.Ok();
});

app.MapGet("/tickets/by-status", async (
    [FromQuery] TicketStatus status,
    [FromQuery] Guid tenantId,
    ISender sender) =>
{
    var result = await sender.Send(new GetTicketsByStatusQuery(status, tenantId));
    return Results.Ok(result);
});

app.MapPost("/agents", async (CreateAgentCommand command, ISender sender) =>
{
    var id = await sender.Send(command);
    return Results.Created($"/agents/{id}", new { id });
});

app.Run();
