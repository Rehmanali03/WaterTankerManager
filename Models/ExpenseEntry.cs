using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WaterTankerManager.Models;

public class ExpenseEntry
{
    [Key]
    public int Id { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime Date { get; set; }

    [Required]
    [Display(Name = "Expense Name")]
    public string ExpenseName { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    [Display(Name = "Expense Price")]
    public decimal ExpensePrice { get; set; }

    [Display(Name = "Off Day")]
    public bool IsOffDay { get; set; }
}
