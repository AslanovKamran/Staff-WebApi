using Microsoft.AspNetCore.Authentication.JwtBearer;
using StaffWebApi.Repository.Abstract;
using Microsoft.IdentityModel.Tokens;
using StaffWebApi.Repository.Dapper;
using Microsoft.OpenApi.Models;
using StaffWebApi.Tokens;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen((options =>
{
	#region Documentaion Section
	var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
	var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
	options.IncludeXmlComments(xmlPath);
	#endregion

	#region Jwt Bearer Section
	var securityScheme = new OpenApiSecurityScheme
	{
		Name = "Jwt Authentication",
		Description = "Type in a valid JWT Bearer",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.Http,
		Scheme = "Bearer",
		BearerFormat = "Jwt",
		Reference = new OpenApiReference
		{
			Id = JwtBearerDefaults.AuthenticationScheme,
			Type = ReferenceType.SecurityScheme
		}
	};
	options.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
	options.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{securityScheme,Array.Empty<string>() }
				});

	#endregion
}));

#region Jwt Options
var jwtOptions = builder.Configuration.GetSection("JwtOptions");
var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions["Key"]!));

const double ACCESS_TOKEN_LIFE_TIME_HRS = 24;
const double REFRESH_TOKEN_LIFE_TIME_DAYS = 30;

builder.Services.Configure<JwtOptions>(options =>
{
	options.Issuer = jwtOptions["Issuer"];
	options.Audience = jwtOptions["Audience"];
	options.AccessValidFor = TimeSpan.FromHours(ACCESS_TOKEN_LIFE_TIME_HRS);
	options.RefreshValidFor = TimeSpan.FromDays(REFRESH_TOKEN_LIFE_TIME_DAYS);
	options.SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
});
#endregion

#region Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateIssuerSigningKey = true,
		ValidateLifetime = true,

		ValidIssuer = builder.Configuration["JwtOptions:Issuer"],
		ValidAudience = builder.Configuration["JwtOptions:Audience"],
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtOptions:Key"])),

		ClockSkew = TimeSpan.Zero // Removes default additional time to the tokens
	};
});
#endregion

var connectionString = builder.Configuration.GetConnectionString("Default");

builder.Services.AddScoped<IPositionRepository, PositionRepositoryDapper>(provider => new PositionRepositoryDapper(connectionString));
builder.Services.AddScoped<IPersonRepository, PersonRepositoryDapper>(provider => new PersonRepositoryDapper(connectionString));
builder.Services.AddScoped<IRoleRepository, RoleRepositoryDapper>(provider => new RoleRepositoryDapper(connectionString));
builder.Services.AddScoped<IUserRepository, UserRepositoryDapper>(provider => new UserRepositoryDapper(connectionString));


builder.Services.AddSingleton<ITokenGenerator, TokenGenerator>();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();

app.UseCors(options =>
{
	options.AllowAnyMethod();
	options.AllowAnyHeader();
	options.AllowAnyOrigin();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapControllers();

app.Run();
