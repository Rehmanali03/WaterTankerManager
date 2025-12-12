using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WaterTankerManager.Data;
using WaterTankerManager.Models;
using WaterTankerManager.Models.ViewModels;

using Microsoft.AspNetCore.Authorization;

namespace WaterTankerManager.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        var now = DateTime.Now;
        var startOfMonth = new DateTime(now.Year, now.Month, 1);
        var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

        var roundsThisMonth = _context.RoundEntries
            .Where(r => r.Date >= startOfMonth && r.Date <= endOfMonth && !r.IsHidden)
            .ToList();

        var expensesThisMonth = _context.ExpenseEntries
            .Where(e => e.Date >= startOfMonth && e.Date <= endOfMonth)
            .ToList();

        // Chart Data (Expenses by Category for this month)
        var expenseChartData = expensesThisMonth
            .GroupBy(e => e.ExpenseName)
            .Select(g => new { Name = g.Key, Amount = g.Sum(e => e.ExpensePrice) })
            .ToList();

        // Pivot Data (Expenses by Name vs Month for current year)
        var startOfYear = new DateTime(now.Year, 1, 1);
        var expensesThisYear = _context.ExpenseEntries
            .Where(e => e.Date >= startOfYear && e.Date <= now)
            .ToList();

        var pivotMonths = Enumerable.Range(1, 12).Select(m => new DateTime(now.Year, m, 1).ToString("MMM")).ToList();
        
        var pivotData = expensesThisYear
            .GroupBy(e => e.ExpenseName)
            .Select(g => new PivotRow
            {
                ExpenseName = g.Key,
                MonthlyAmounts = pivotMonths.ToDictionary(
                    m => m,
                    m => g.Where(e => e.Date.ToString("MMM") == m).Sum(e => e.ExpensePrice)
                ),
                Total = g.Sum(e => e.ExpensePrice)
            })
            .ToList();

        var viewModel = new DashboardViewModel
        {
            TotalRounds = roundsThisMonth.Sum(r => r.NumberOfRounds),
            TotalRevenue = roundsThisMonth.Sum(r => r.TotalAmount),
            TotalExpenses = expensesThisMonth.Sum(e => e.ExpensePrice),
            CurrentMonthName = now.ToString("MMMM yyyy"),
            
            // Chart
            ExpenseLabels = expenseChartData.Select(x => x.Name).ToList(),
            ExpenseValues = expenseChartData.Select(x => x.Amount).ToList(),

            // Pivot
            PivotMonths = pivotMonths,
            PivotData = pivotData
        };
        
        viewModel.NetProfit = viewModel.TotalRevenue - viewModel.TotalExpenses;

        // Calculate Percentages for Pie Chart
        // Logic: 
        // If Profit: Base = Revenue. Slices = Profit (Green) + Expenses (Red).
        // If Loss: Base = Expenses. Slices = Revenue (Green) + Loss (Red).
        
        if (viewModel.NetProfit >= 0)
        {
            // PROFIT SCENARIO
            decimal baseAmount = viewModel.TotalRevenue > 0 ? viewModel.TotalRevenue : 1; // Avoid divide by zero
            
            viewModel.ChartSlice1Label = "Net Profit";
            viewModel.ChartSlice1Value = Math.Round((viewModel.NetProfit / baseAmount) * 100, 1);
            
            viewModel.ChartSlice2Label = "Expenses";
            viewModel.ChartSlice2Value = Math.Round((viewModel.TotalExpenses / baseAmount) * 100, 1);
        }
        else
        {
            // LOSS SCENARIO
            // User Request: "if our net profit is in minus so show 100% expense"
            // We just show a full red circle labeled "Expenses".
            
            viewModel.ChartSlice1Label = ""; // No Green Slice
            viewModel.ChartSlice1Value = 0;
            
            viewModel.ChartSlice2Label = "Expenses";
            viewModel.ChartSlice2Value = 100;
        }

        // Date Frequency (Pivot Table: Date vs Round Count)
        var dateFrequency = roundsThisMonth
            .GroupBy(r => r.Date)
            .Select(g => new DateRoundCount
            {
                Date = g.Key,
                Count = g.Sum(r => r.NumberOfRounds)
            })
            .OrderByDescending(x => x.Count)
            .ToList();

        viewModel.DateFrequency = dateFrequency;

        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
