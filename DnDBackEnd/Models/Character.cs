using Newtonsoft.Json;

namespace DnDBackEnd.Models
{
	public class Character
	{
		public int Id { get; set; }

		[JsonProperty("name")]
		public required string Name { get; set; }

		[JsonProperty("hitPoints")]
		public int HitPoints { get; set; }

		public int TempHP { get; set; }

		[JsonProperty("defenses")]
		public List<Defense> Defenses { get; set; } = new List<Defense>();
	}
}

