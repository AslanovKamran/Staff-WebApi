using System.Reflection;
using StaffWebApi.Repository.Abstract;
using StaffWebApi.Repository.Dapper;

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


var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapControllers();

app.Run();