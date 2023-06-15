using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage ="Поле не заполнено")]
        public string Title { get; set; }

        [Column(TypeName = "nvarchar(10)")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Icon { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(10)")]
        public string Type { get; set; }

        [NotMapped]
        public string? TitleWithIcon
        {
            get
            {
                return this.Icon + " " + this.Title;
            }
        }
    }
}