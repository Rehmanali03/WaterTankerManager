using System.ComponentModel.DataAnnotations;

namespace WaterTankerManager.Models.ViewModels;

public class MonthlyReportViewModel
{
    public int SelectedMonth { get; set; }
    public int SelectedYear { get; set; }
    
    public List<DailyReportItem> DailyReports { get; set; } = new();

    [Display(Name = "Total Rounds")]
    public int GrandTotalRounds { get; set; }

    [Display(Name = "Total Income")]
    public decimal GrandTotalIncome { get; set; }

    [Display(Name = "Total Expense")]
    public decimal GrandTotalExpense { get; set; }

    [Display(Name = "Total Profit")]
    public decimal GrandTotalProfit { get; set; }

    public bool ShowHidden { get; set; }

    [Display(Name = "Profit Per Person")]
    public decimal ProfitPerPerson { get; set; }
}

public class DailyReportItem
{
    [DataType(DataType.Date)]
    public DateTime Date { get; set; }

    public int TotalRounds { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public string ExpenseNames { get; set; } = string.Empty;
    public decimal DailyProfit { get; set; }
}
