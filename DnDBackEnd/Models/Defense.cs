using DnDBackEnd.Converters;
using Newtonsoft.Json;

namespace DnDBackEnd.Models
{
	public class Defense
	{
		public int Id { get; set; }

		[JsonProperty("type")]
		[JsonConverter(typeof(DamageTypeConverter))]
		public DamageType DamageType { get; set; }

		[JsonProperty("defense")]
		[JsonConverter(typeof(DefenseTypeConverter))]
		public DefenseType DefenseType {get; set;}
	}
}
