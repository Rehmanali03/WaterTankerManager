using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WaterTankerManager.Models;

public class RoundEntry
{
    [Key]
    public int Id { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime Date { get; set; }

    [Required]
    [Display(Name = "Number of Rounds")]
    public int NumberOfRounds { get; set; }

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    [Display(Name = "Total Amount")]
    public decimal TotalAmount { get; set; }

    public bool IsHidden { get; set; }
}
