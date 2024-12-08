using DnDBackEnd.DTOs;
using DnDBackEnd.Models;
using DnDBackEnd.Services;
using Microsoft.AspNetCore.Mvc;

namespace DnDBackEnd.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class CharactersController : ControllerBase
	{
		private readonly ICharacterService _characterService;
		private readonly ILogger<CharactersController> _logger;

		public CharactersController(ICharacterService characterService, ILogger<CharactersController> logger)
		{
			_characterService = characterService;
			_logger = logger;
		}

		/// <summary>
		/// Deals damage to a character
		/// </summary>
		/// <param name="request">DealDamageRequest</param>
		/// <returns>JSON representation of Character</returns>
		[HttpPost("deal-damage")]
		public async Task<ActionResult<string>> DealDamage([FromBody] DealDamageRequest request)
		{
			try
			{
				if (!Enum.TryParse<DamageType>(request.DamageType, true, out var damageType))
				{
					_logger.LogError($"Invalid damage type: {request.DamageType}");
					return BadRequest($"Invalid damage type: {request.DamageType}");
				}

				var result = await _characterService.DealDamageAsync(request);
				return Ok(result);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error dealing damage");
				return BadRequest(ex.Message);
			}
		}

		/// <summary>
		/// Will heal HitPoints on given character
		/// </summary>
		/// <param name="request">HealRequest</param>
		/// <returns>JSON representation of Character</returns>
		[HttpPost("heal")]
		public async Task<ActionResult<string>> Heal([FromBody] HealRequest request)
		{
			try
			{
				var result = await _characterService.HealAsync(request);
				return Ok(result);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error healing character");
				return BadRequest(ex.Message);
			}
		}

		/// <summary>
		/// Adds temporary hit points to character
		/// </summary>
		/// <param name="request">TemporaryHPRequest</param>
		/// <returns>JSON representation of Character</returns>
		[HttpPost("add-temporary-hp")]
		public async Task<ActionResult<string>> AddTemporaryHP([FromBody] TemporaryHPRequest request)
		{
			try
			{
				var result = await _characterService.AddTemporaryHPAsync(request);
				return Ok(result);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error adding temporary HP");
				return BadRequest(ex.Message);
			}
		}
	}
}
