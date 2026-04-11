using Microsoft.EntityFrameworkCore;
using ProjectManagement.BL.Implementations;
using ProjectManagement.BL.Interfaces;
using ProjectManagement.Models;
using ProjectManagement.Repositories.Implementations;
using ProjectManagement.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// database connection
builder.Services.AddDbContext<ProjectManagementContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITask, ClsTask>();
builder.Services.AddScoped<ITaskHistoryRepository, TaskHistoryRepository>();
builder.Services.AddScoped<ITaskHistory, ClsTaskHistory>();

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
