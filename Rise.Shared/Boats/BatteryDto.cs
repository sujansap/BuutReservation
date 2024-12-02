namespace Rise.Shared.Boats
{
    /// <summary>
    /// Info for a battery to update them
    /// </summary>
    public record BatteryDto
    {
        public required string Type { get; set; }
        public required int MentorId { get; set; }
    }
}


