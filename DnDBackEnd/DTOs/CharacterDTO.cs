using DnDBackEnd.Models;

namespace DnDBackEnd.DTOs
{
	public class CharacterDTO
	{
		public required string Name { get; set; }
		public int HitPoints { get; set; }
		public int TempHP { get; set; }
		public List<Defense> Defenses { get; set; } = [];
	}
}
