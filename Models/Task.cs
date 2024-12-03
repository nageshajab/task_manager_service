using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models
{
    /// <summary>
    /// This class will also be used to store 'Rents' and set reminders
    /// Use ItemType to set whether it is specially for rent or not
    /// </summary>
    public class Task
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime DueDate { get; set; }

        public Priority Priority { get; set; }

        public Status Status { get; set; }

        public int UserId { get; set; }

        [NotMapped]
        public bool CanRepeat { get; set; }

        [NotMapped]
        public RepeatType RepeatType { get; set; }

        [NotMapped]
        public DateTime EndDate { get; set; }
    }

    public enum RepeatType
    {
        None = 0,
        Daily = 1,
        Weekly = 2,
        Monthly = 3,
        Yearly = 4
    }

    public enum Priority
    {
        High=0,
        Medium=1,
        Low=2
    }

    public enum Status
    {
        None = -1,
        Pending =0,
        InProgress=1,
        Completed=2
    }
}
