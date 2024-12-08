namespace DnDBackEnd.DTOs
{
	public class HealRequest
	{
		public required string Name { get; set; }
		public int HealAmount { get; set; }
	}
}
