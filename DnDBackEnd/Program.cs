using DnDBackEnd.Data;
using DnDBackEnd.DTOs;
using DnDBackEnd.Models;
using DnDBackEnd.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("CharacterDB"));
builder.Services.AddScoped<ICharacterService, CharacterService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Loading briv.json, deserializing and creating the new character
// Hardcoded as per requirements
// Future consideration - having this value stored in a config setting
var filePath = Path.Combine(Directory.GetCurrentDirectory(), "briv.json");

if (File.Exists(filePath))
{
	var characterData = File.ReadAllText(filePath);
	var characterDTO = JsonConvert.DeserializeObject<CharacterDTO>(characterData);

	using (var scope = app.Services.CreateScope())
	{
		var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

		// Check if the character is already loaded
		if (!context.Characters.Any() && characterDTO != null)
		{
			var character = new Character
			{
				Name = characterDTO.Name,
				HitPoints = characterDTO.HitPoints,
				TempHP = characterDTO.TempHP,
				Defenses = characterDTO.Defenses.Select(d => new Defense
				{
					DamageType = d.DamageType,
					DefenseType = d.DefenseType
				}).ToList()
			};

			context.Characters.Add(character);
			context.SaveChanges();
		}
	}
}
else
{
	Console.WriteLine("briv.json file not found.");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();