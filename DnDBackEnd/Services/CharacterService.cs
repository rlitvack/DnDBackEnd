using DnDBackEnd.Data;
using DnDBackEnd.DTOs;
using DnDBackEnd.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DnDBackEnd.Services
{
	public class CharacterService : ICharacterService
	{
		private readonly ApplicationDbContext _context;
		private readonly ILogger<CharacterService> _logger;

		public CharacterService(ApplicationDbContext context, ILogger<CharacterService> logger)
		{
			_context = context;
			_logger = logger;
		}

		public async Task<Character> GetCharacterAsync(string name)
		{
			//Retrieve given character from context
			var character = await _context.Characters.Include(c => c.Defenses).FirstOrDefaultAsync(c => c.Name == name);

			if (character == null)
			{
				_logger.LogWarning($"Character not found with Name {name}");
				throw new KeyNotFoundException($"Character with Name {name} not found.");
			}

			return character;
		}

		public async Task<string> DealDamageAsync(DealDamageRequest request)
		{
			var character = await GetCharacterAsync(request.Name);
			var damage = request.DamageAmount;

			Enum.TryParse<DamageType>(request.DamageType, true, out var damageType);

			// Check if character has resistance or immunity
			var defense = character.Defenses.FirstOrDefault(d => d.DamageType == damageType);

			if (defense != null)
			{
				// If Immunity then do damage should be dealt
				if (defense.DefenseType == DefenseType.Immunity)
				{
					_logger.LogInformation($"Character {character.Name} is immune to {damageType}. No damage taken.");
					return JsonConvert.SerializeObject(character);
				}
				else if (defense.DefenseType == DefenseType.Resistance)
				{
					_logger.LogInformation($"Character {character.Name} has resistance to {damageType}. Damage halved.");
					// Do 50% damage
					// Future consideration - there could be different defense types with different reductions
					// For simplicity, hard-coding this to 50%
					damage = (int)(damage * .5);
				}
			}

			// Apply damage to HP, considering temporary HP
			if (character.TempHP > 0)
			{
				if (damage <= character.TempHP)
				{
					// Remove only TempHP in this scenario
					character.TempHP -= damage;
					_logger.LogInformation($"Damage fully absorbed by temporary hit points. Remaining temporary HP: {character.TempHP}");
				}
				else
				{
					// Remove all TempHP and reduce HP by leftover
					damage -= character.TempHP;
					character.TempHP = 0;
					_logger.LogInformation($"Temporary hit points depleted. Remaining damage: {damage}");
					character.HitPoints -= damage;
				}
			}
			else
			{
				character.HitPoints -= damage;
			}

			_context.Characters.Update(character);
			await _context.SaveChangesAsync();

			// Account for 0 HP scenario
			if (character.HitPoints <= 0)
				return "Your character is dead!";

			return JsonConvert.SerializeObject(character);
		}

		public async Task<string> HealAsync(HealRequest request)
		{
			var character = await GetCharacterAsync(request.Name);

			// Future consideration - should there be a Max HP for a character
			// In theory we could heal a large amount here
			character.HitPoints = character.HitPoints + request.HealAmount;

			_context.Characters.Update(character);
			await _context.SaveChangesAsync();

			return JsonConvert.SerializeObject(character);
		}

		public async Task<string> AddTemporaryHPAsync(TemporaryHPRequest request)
		{
			// Adds temporary HP to a character
			var character = await GetCharacterAsync(request.Name);

			// Only add the maximum value, not a cummulative effect
			character.TempHP = Math.Max(character.TempHP, request.TempHPAmount);

			_context.Characters.Update(character);
			await _context.SaveChangesAsync();

			return JsonConvert.SerializeObject(character);
		}
	}
}
