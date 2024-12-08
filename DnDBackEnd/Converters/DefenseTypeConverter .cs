using DnDBackEnd.Models;
using Newtonsoft.Json;

namespace DnDBackEnd.Converters
{
	//Used to convert between Enumeration values and integer equivalent
	public class DefenseTypeConverter : JsonConverter<DefenseType>
	{
		public override DefenseType ReadJson(JsonReader reader, Type objectType, DefenseType existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			var value = reader.Value.ToString();

			if (!Enum.TryParse<DefenseType>(value, true, out var defenseType))
			{
				throw new JsonException($"Invalid defense type: {value}");
			}

			return defenseType;
		}

		public override void WriteJson(JsonWriter writer, DefenseType value, JsonSerializer serializer)
		{
			writer.WriteValue(value.ToString().ToLower());
		}
	}

}
