using System.ComponentModel.DataAnnotations;
using PracticeProject.Enums;
namespace PracticeProject.Entities
{
    public class Books
    {

        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Title { get; set; }
        [Required]
        [StringLength(100)]
        public string? Author { get; set; }
        public DateTime? PublicationDate { get; set; }
        public Category Category { get; set; }

    }
}
