namespace Models
{
    public class Patient
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; }=string.Empty;
        public string Address { get; set; }=string.Empty;
        public required string[] InsuranceCoverage { get; set; }
        public int therapyVisitsRemaining { get; set; }
        public int totalTherapyVisits { get; set; }
    }
}
