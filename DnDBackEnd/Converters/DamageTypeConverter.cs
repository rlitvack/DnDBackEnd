using DnDBackEnd.Models;
using Newtonsoft.Json;

namespace DnDBackEnd.Converters
{
	//Used to convert between Enumeration values and integer equivalent
	public class DamageTypeConverter : JsonConverter<DamageType>
	{
		public override DamageType ReadJson(JsonReader reader, Type objectType, DamageType existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			var value = reader.Value.ToString();

			if (!Enum.TryParse<DamageType>(value, true, out var damageType))
			{
				throw new JsonException($"Invalid damage type: {value}");
			}

			return damageType;
		}

		public override void WriteJson(JsonWriter writer, DamageType value, JsonSerializer serializer)
		{
			writer.WriteValue(value.ToString().ToLower());
		}
	}

}
