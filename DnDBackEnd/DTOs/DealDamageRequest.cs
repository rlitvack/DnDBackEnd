namespace DnDBackEnd.DTOs
{
	public class DealDamageRequest
	{
		public required string Name { get; set; }
		public int DamageAmount { get; set; }
		public required string DamageType { get; set; }
	}
}
