
using System.ComponentModel.DataAnnotations;
using SimpleBookCatalog.Domain.Enum;

namespace SimpleBookCatalog.Domain.Entities
{

    public class Books
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "please provide a title")]
        [StringLength(100)]
        public string? Title { get; set; }
        [Required(ErrorMessage = "please provide the author's name")]
        [StringLength(100)]
        public string? Author { get; set; }
        public DateTime? PublicationDate { get; set; }

        [EnumDataType(typeof(Category), ErrorMessage = "Please select a category")]
        public Category Category { get; set; }

    }
}
