using StaffWebApi.Repository.Abstract;
using StaffWebApi.Repository.Dapper;
using System.Reflection;

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

}));


var connectionString = builder.Configuration.GetConnectionString("Default");

builder.Services.AddScoped<IPositionRepository, PositionRepositoryDapper>(provider => new PositionRepositoryDapper(connectionString));
builder.Services.AddScoped<IPersonRepository, PersonRepositoryDapper>(provider => new PersonRepositoryDapper(connectionString));
builder.Services.AddScoped<IRoleRepository, RoleRepositoryDapper>(provider => new RoleRepositoryDapper(connectionString));
builder.Services.AddScoped<IUserRepository, UserRepositoryDapper>(provider => new UserRepositoryDapper(connectionString));


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
