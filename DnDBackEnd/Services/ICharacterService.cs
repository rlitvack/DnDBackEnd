using DnDBackEnd.DTOs;
using DnDBackEnd.Models;

namespace DnDBackEnd.Services
{
	public interface ICharacterService
	{
		Task<Character> GetCharacterAsync(string Name);
		Task<string> DealDamageAsync(DealDamageRequest request);
		Task<string> HealAsync(HealRequest request);
		Task<string> AddTemporaryHPAsync(TemporaryHPRequest request);
	}
}
