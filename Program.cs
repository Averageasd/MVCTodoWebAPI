using Microsoft.EntityFrameworkCore;
using TodoProjectSample.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddDbContext<TodoContext>(op => op.UseInMemoryDatabase("TodoList"));
var app = builder.Build();

// serve js, html and css files
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
