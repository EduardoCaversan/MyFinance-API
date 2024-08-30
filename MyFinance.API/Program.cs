using Microsoft.AspNetCore.Mvc.Versioning.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MyFinance.API.Filters;
using MyFinance.Domain.Commands;
using MyFinance.Domain.Events;
using MyFinance.Domain.Queries;
using MyFinance.Domain.Utils;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);

Keys.SetIssuer(builder.Configuration.GetValue<string>("MyFinanceServer:Issuer"));
Keys.SetApiUrl(builder.Configuration.GetValue<string>("MyFinanceServer:ApiUrl"));

builder.Services.AddDbContext<CommandsDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Commands")));
builder.Services.AddDbContext<QueriesDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Queries")));

builder.Services.AddScoped<CommandsHandler>();
builder.Services.AddScoped<QueriesHandler>();
builder.Services.AddScoped<ListenersHandler>();
builder.Services.AddScoped<ValidateTokenFilter>();

builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.Conventions.Add(new VersionByNamespaceConvention());
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyFinance API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            }, new List<string>()
        }
    });
    c.CustomSchemaIds(type =>
    {
        if (!type.IsGenericType || type.Name != "QueryResult`1")
            return GetSwaggerTypeName(type);
        var argumentType = type.GenericTypeArguments[0];
        var typeName = GetSwaggerTypeName(argumentType);
        return $"QueryResult<{typeName}>";
    });
});

builder.Services.AddMvc().AddJsonOptions(options => { });
builder.Services.AddAuthentication();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true)
    .AllowCredentials());

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyFinance API.v1");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();

static string GetSwaggerTypeName(Type type)
{
    var pattern = @"^([0-9a-z_\-]+)\.Domain\.Queries\.([0-9a-z_\-]+).(?<namespace>[0-9a-z_\-]+)\.ViewModels\.(?<class>[0-9a-z_\-]+)$";
    var m = Regex.Match(type.FullName, pattern, RegexOptions.IgnoreCase);
    if (!m.Success)
        return type.Name;
    var ns = m.Groups["namespace"].Value;
    var cl = m.Groups["class"].Value;
    return ns + "." + cl;
}
