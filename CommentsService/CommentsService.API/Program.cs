using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CommentsService.Application.Extensions;
using CommentsService.Infrastructure.Extensions;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddControllers();

builder.Services.AddGrpcClients();
builder.Services.AddGrpc();
builder.Services.AddGrpc().AddJsonTranscoding();
builder.Services.AddGrpcReflection();

builder.Logging.AddSeq(builder.Configuration.GetSection("Logging:Seq"));

var app = builder.Build();

app.ApplyMigrations();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
        options.RoutePrefix = "swagger";
    });

    app.MapGrpcReflectionService();
}

app.UseHttpsRedirection();
app.MapGrpcService<CommentsService.API.Grpc.CommentsGrpcService>();
app.MapControllers();

app.Run();
