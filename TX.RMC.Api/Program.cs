using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using TX.RMC.Api.Services;
using TX.RMC.Api.Utils;
using TX.RMC.BusinessLogic;
using Microsoft.OpenApi.Models;
using TX.RMC.DataService.MongoDB;
using Microsoft.Extensions.Options;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Newtonsoft;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using TX.RMC.Api.Swagger.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Identity service
builder.Services.AddTransient<IdentityService>();

// configure jwt authentication
var JwtSecretkey = Encoding.UTF8.GetBytes(ApiSecurityHelper.OauthKey);
var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(JwtSecretkey),
    ValidIssuer = ApiSecurityHelper.Issuer,
    ValidAudience = ApiSecurityHelper.Audience
};
builder.Services.AddSingleton(tokenValidationParameters);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
#if DEBUG
    x.RequireHttpsMetadata = false;
#endif

    x.SaveToken = true;
    x.TokenValidationParameters = tokenValidationParameters;
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Services.AddAuthorizationBuilder()
    .SetDefaultPolicy(new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .Build());

builder.Services.AddControllers().AddNewtonsoftJson(o =>
{
    o.SerializerSettings.Converters.Add(new StringEnumConverter
    {
        NamingStrategy = new CamelCaseNamingStrategy()
    });
});

builder.Services.AddSwaggerGenNewtonsoftSupport();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(config =>
{
    config.SwaggerDoc("v1", new OpenApiInfo { Title = "TX RMC Api", Version = "v1" });
    config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                    Enter 'Bearer' [space] and then your token in the text input below.
                    \r\n\r\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });

    config.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });

    // using System.Reflection;
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    config.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    config.SchemaFilter<EnumTypesSchemaFilter>(xmlFilename);
});

var connectionString = builder.Configuration.GetConnectionString("MongoDBConnection");
var dbName = builder.Configuration["MongoDBDatabase"];
if (!string.IsNullOrEmpty(connectionString) && !string.IsNullOrEmpty(dbName))
{
    builder.Services.AddMongoDbServices(connectionString, dbName);
}

builder.Services.AddBusinessLogicServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => TypedResults.LocalRedirect("~/swagger"));

app.Run();
