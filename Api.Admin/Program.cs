var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Opsilo Admin API V1");
    });
}

app.MapGet("/hello", () =>
{
    return Results.Ok(new { Message = "Hello from Opsio Admin API on .NET 10!" });
});



app.UseHttpsRedirection();


app.Run();