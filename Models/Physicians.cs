namespace Models
{
    public class Physicians
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Specialty { get; set; }=string.Empty;
        public int Distance { get; set; }
        public string City { get; set; }=string.Empty;
        public string Address { get; set; }=string.Empty;

    }
}
