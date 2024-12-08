using DnDBackEnd.Data;
using DnDBackEnd.DTOs;
using DnDBackEnd.Models;
using DnDBackEnd.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;

namespace DndBackEndTests
{
	public class CharacterServiceTests : IDisposable
	{
		private readonly ApplicationDbContext _context;
		private readonly ICharacterService _characterService;

		public CharacterServiceTests()
		{
			// Set up In-Memory Database for testing
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDatabase")
				.Options;

			_context = new ApplicationDbContext(options);

			_characterService = new CharacterService(_context, new NullLogger<CharacterService>());

			AddCharacter();
		}

		private void AddCharacter()
		{
			var character = new Character
			{
				Name = "Rob",
				HitPoints = 20,
				TempHP = 10,
				Defenses = new List<Defense>()
				{
					new Defense() { DamageType = DamageType.Force, DefenseType = DefenseType.Immunity },
					new Defense() { DamageType = DamageType.Fire, DefenseType = DefenseType.Resistance },
				}
			};

			_context.Characters.Add(character);
			_context.SaveChanges();
		}

		public void Dispose()
		{
			//Cleanup after each test to reset the database
			_context.Database.EnsureDeleted();
			_context.Dispose();
		}

		[Fact]
		public async Task DealDamage_NoResistance_NoImmunity_UseTemp()
		{
			// Arrange
			var damageRequest = new DealDamageRequest
			{
				Name = "Rob",
				DamageAmount = 1,
				DamageType = DamageType.Thunder.ToString()
			};

			// Act
			var result = await _characterService.DealDamageAsync(damageRequest);

			var characterDTO = JsonConvert.DeserializeObject<CharacterDTO>(result);

			// Assert
			// Should only use TempHP
			Assert.Equal(20, characterDTO.HitPoints); 
			Assert.Equal(9, characterDTO.TempHP);
		}

		[Fact]
		public async Task DealDamage_NoResistance_NoImmunity_UseTempAndRegular()
		{
			// Arrange
			var damageRequest = new DealDamageRequest
			{
				Name = "Rob",
				DamageAmount = 20,
				DamageType = DamageType.Thunder.ToString()
			};

			// Act
			var result = await _characterService.DealDamageAsync(damageRequest);

			var characterDTO = JsonConvert.DeserializeObject<CharacterDTO>(result);

			// Assert
			// Should remove all TempHP then Regular HP
			Assert.Equal(10, characterDTO.HitPoints);
			Assert.Equal(0, characterDTO.TempHP);
		}

		[Fact]
		public async Task DealDamage_WithResistance_NoImmunity()
		{
			// Arrange
			var damageRequest = new DealDamageRequest
			{
				Name = "Rob",
				DamageAmount = 10,
				DamageType = DamageType.Fire.ToString()
			};

			// Act
			var result = await _characterService.DealDamageAsync(damageRequest);

			var characterDTO = JsonConvert.DeserializeObject<CharacterDTO>(result);

			// Assert
			// Damage should be 50% due to resistance
			Assert.Equal(20, characterDTO.HitPoints); 
			Assert.Equal(5, characterDTO.TempHP);
		}

		[Fact]
		public async Task DealDamage_NoResistance_WithImmunity()
		{
			// Arrange
			var damageRequest = new DealDamageRequest
			{
				Name = "Rob",
				DamageAmount = 20,
				DamageType = DamageType.Force.ToString()
			};

			// Act
			var result = await _characterService.DealDamageAsync(damageRequest);

			var characterDTO = JsonConvert.DeserializeObject<CharacterDTO>(result);

			// Assert
			// No damage should be taken
			Assert.Equal(20, characterDTO.HitPoints);
			Assert.Equal(10, characterDTO.TempHP);
		}


		[Fact]
		public async Task DealDamage_KillRob()
		{
			// Arrange
			var damageRequest = new DealDamageRequest
			{
				Name = "Rob",
				DamageAmount = 30,
				DamageType = DamageType.Thunder.ToString()
			};

			// Act
			var result = await _characterService.DealDamageAsync(damageRequest);

			// Assert
			// Character would have 0 HP, so they are dead
			Assert.Equal("Your character is dead!", result);
		}


		[Fact]
		public async Task HealCharacter()
		{
			// Arrange
			var healRequest = new HealRequest
			{
				Name = "Rob",
				HealAmount = 10
			};

			// Act
			var result = await _characterService.HealAsync(healRequest);

			var characterDTO = JsonConvert.DeserializeObject<CharacterDTO>(result);

			// Assert
			// HP should be healed
			Assert.Equal(30, characterDTO.HitPoints);
			Assert.Equal(10, characterDTO.TempHP);
		}

		[Fact]
		public async Task HealCharacter_DealDamage()
		{
			// Arrange
			var healRequest = new HealRequest
			{
				Name = "Rob",
				HealAmount = 50
			};

			var damageRequest = new DealDamageRequest
			{
				Name = "Rob",
				DamageAmount = 60,
				DamageType = DamageType.Cold.ToString()
			};

			// Act
			var result = await _characterService.HealAsync(healRequest);
			var characterDTO = JsonConvert.DeserializeObject<CharacterDTO>(result);

			// Assert
			// HP should be healed
			Assert.Equal(70, characterDTO.HitPoints);
			Assert.Equal(10, characterDTO.TempHP);

			// Act
			result = await _characterService.DealDamageAsync(damageRequest);
			characterDTO = JsonConvert.DeserializeObject<CharacterDTO>(result);

			// Assert
			// Should remove all TempHP then Regular HP
			Assert.Equal(20, characterDTO.HitPoints);
			Assert.Equal(0, characterDTO.TempHP);
		}

		[Fact]
		public async Task AddTempHP_NewHigherValue()
		{
			// Arrange
			var tempHPRequest = new TemporaryHPRequest
			{
				Name = "Rob",
				TempHPAmount = 15
			};

			// Act
			var result = await _characterService.AddTemporaryHPAsync(tempHPRequest);

			var characterDTO = JsonConvert.DeserializeObject<CharacterDTO>(result);

			// Assert
			// TempHP should take higher value
			Assert.Equal(20, characterDTO.HitPoints);
			Assert.Equal(15, characterDTO.TempHP);
		}

		[Fact]
		public async Task AddTempHP_NewLowerValue()
		{
			// Arrange
			var tempHPRequest = new TemporaryHPRequest
			{
				Name = "Rob",
				TempHPAmount = 5
			};

			// Act
			var result = await _characterService.AddTemporaryHPAsync(tempHPRequest);

			var characterDTO = JsonConvert.DeserializeObject<CharacterDTO>(result);

			// Assert
			// TempHP should remain as original
			Assert.Equal(20, characterDTO.HitPoints);
			Assert.Equal(10, characterDTO.TempHP);
		}

		[Fact]
		public async Task AddTempHP_DealDamage()
		{
			// Arrange
			var tempHPRequest = new TemporaryHPRequest
			{
				Name = "Rob",
				TempHPAmount = 20
			};

			var damageRequest = new DealDamageRequest
			{
				Name = "Rob",
				DamageAmount = 35,
				DamageType = DamageType.Cold.ToString()
			};

			// Act
			var result = await _characterService.AddTemporaryHPAsync(tempHPRequest);

			var characterDTO = JsonConvert.DeserializeObject<CharacterDTO>(result);

			// Assert
			// TempHP should be increased
			Assert.Equal(20, characterDTO.HitPoints);
			Assert.Equal(20, characterDTO.TempHP);

			// Act
			result = await _characterService.DealDamageAsync(damageRequest);
			characterDTO = JsonConvert.DeserializeObject<CharacterDTO>(result);

			// Assert
			// Should remove all TempHP then Regular HP
			Assert.Equal(5, characterDTO.HitPoints);
			Assert.Equal(0, characterDTO.TempHP);
		}
	}
}