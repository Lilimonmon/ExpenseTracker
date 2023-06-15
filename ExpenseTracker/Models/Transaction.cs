using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        [Required(ErrorMessage = "Выберите категорию")]
        //[Range(1, int.MaxValue, ErrorMessage = "Выберите категорию")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [Required(ErrorMessage = "Поле не заполнено")]
        public int? Amount { get; set; }


        [Column(TypeName = "nvarchar(70)")]
        public string? Note { get; set; } = string.Empty;

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Выберите дату")]
        public DateTime? Date { get; set; }

        [NotMapped]
        public string? CategoryTitleWithIcon {
            get
            {
                return Category == null ? "" : Category.Icon + " " + Category.Title;
            }
        }

        [NotMapped]
        public string? FormattedAmount
        {
            get
            {
                return ((Category == null || Category.Type == "расход") ? "- " : "+ ") + Amount.ToString() + "₽";
            }
        }
    }
}