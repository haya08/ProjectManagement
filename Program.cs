using Microsoft.EntityFrameworkCore;
using ProjectManagement.BL;
using ProjectManagement.BL.Implementations;
using ProjectManagement.BL.Interfaces;
using ProjectManagement.Models;
using ProjectManagement.Repositories.Implementations;
using ProjectManagement.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();   // ⭐ مهم

// database connection
builder.Services.AddDbContext<ProjectManagementContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(
            builder.Configuration.GetConnectionString("DefaultConnection")
        )
    )
);

builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITask, ClsTask>();

builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IProjectBL, ProjectBL>();

var app = builder.Build();

// Configure pipeline

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();       // ⭐ مهم
    app.UseSwaggerUI();     // ⭐ مهم
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();