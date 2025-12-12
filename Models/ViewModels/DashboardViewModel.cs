using System.ComponentModel.DataAnnotations;

namespace WaterTankerManager.Models.ViewModels;

public class DashboardViewModel
{
    // KPIs (Current Month)
    public decimal TotalRevenue { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal NetProfit { get; set; }
    public int TotalRounds { get; set; }

    public string CurrentMonthName { get; set; } = string.Empty;

    // Charts
    public List<string> ExpenseLabels { get; set; } = new();
    public List<decimal> ExpenseValues { get; set; } = new();

    // Pie Chart Data (Profit/Loss logic)
    public decimal ChartSlice1Value { get; set; } // Green (Profit or Revenue)
    public string ChartSlice1Label { get; set; } = string.Empty;
    
    public decimal ChartSlice2Value { get; set; } // Red (Expense or Loss)
    public string ChartSlice2Label { get; set; } = string.Empty;

    // Pivot Table (Rows: ExpenseName, Cols: Month, Val: Amount)
    public List<PivotRow> PivotData { get; set; } = new();
    public List<string> PivotMonths { get; set; } = new();

    // Pivot Table: Date vs Rounds Count (Frequency)
    public List<DateRoundCount> DateFrequency { get; set; } = new();
}

public class DateRoundCount
{
    public DateTime Date { get; set; }
    public int Count { get; set; }
}

public class PivotRow
{
    public string ExpenseName { get; set; } = string.Empty;
    public Dictionary<string, decimal> MonthlyAmounts { get; set; } = new();
    public decimal Total { get; set; }
}
