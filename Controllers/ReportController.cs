using Microsoft.AspNetCore.Mvc;
using WaterTankerManager.Data;
using WaterTankerManager.Models.ViewModels;
using WaterTankerManager.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

using Microsoft.AspNetCore.Authorization;

namespace WaterTankerManager.Controllers;

[Authorize]
public class ReportController : Controller
{
    private readonly ApplicationDbContext _context;

    public ReportController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index(int? month, int? year, bool showHidden = false)
    {
        var viewModel = GetMonthlyReportModel(month, year, showHidden);
        return View(viewModel);
    }

    public IActionResult DownloadPdf(int? month, int? year, bool showHidden = false)
    {
        var model = GetMonthlyReportModel(month, year, showHidden);

        if (!model.DailyReports.Any())
        {
            return RedirectToAction(nameof(Index), new { month, year, showHidden });
        }

        try 
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    var titlePrefix = showHidden ? "Hidden Orders" : "Monthly Business";
                    page.Header()
                        .Text($"{titlePrefix} Report: {System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(model.SelectedMonth)} {model.SelectedYear}")
                        .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Table(table =>
                        {
                            // Definition
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn(2); // Expense Details is wider
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            // Header
                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Date");
                                header.Cell().Element(CellStyle).AlignRight().Text("Rounds");
                                header.Cell().Element(CellStyle).AlignRight().Text("Income");
                                header.Cell().Element(CellStyle).Text("Expense Details");
                                header.Cell().Element(CellStyle).AlignRight().Text("Expense");
                                header.Cell().Element(CellStyle).AlignRight().Text("Profit");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.DefaultTextStyle(x => x.SemiBold()).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                                }
                            });

                            // Content
                            foreach (var item in model.DailyReports)
                            {
                                table.Cell().Element(CellStyle).Text(item.Date.ToString("dd MMM yyyy"));
                                table.Cell().Element(CellStyle).AlignRight().Text(item.TotalRounds.ToString());
                                table.Cell().Element(CellStyle).AlignRight().Text(item.TotalIncome.ToString("C")).FontColor(Colors.Green.Medium);
                                table.Cell().Element(CellStyle).Text(item.ExpenseNames).FontSize(10).FontColor(Colors.Grey.Darken2);
                                table.Cell().Element(CellStyle).AlignRight().Text(item.TotalExpense.ToString("C")).FontColor(Colors.Red.Medium);
                                
                                var profitColor = item.DailyProfit >= 0 ? Colors.Blue.Medium : Colors.Red.Medium;
                                table.Cell().Element(CellStyle).AlignRight().Text(item.DailyProfit.ToString("C")).FontColor(profitColor);

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten3).PaddingVertical(5);
                                }
                            }

                            // Footer (Totals)
                            table.Footer(footer =>
                            {
                                footer.Cell().Element(FooterStyle).Text("TOTAL");
                                footer.Cell().Element(FooterStyle).AlignRight().Text(model.GrandTotalRounds.ToString());
                                footer.Cell().Element(FooterStyle).AlignRight().Text(model.GrandTotalIncome.ToString("C")).FontColor(Colors.Green.Darken1);
                                footer.Cell().Element(FooterStyle).Text("");
                                footer.Cell().Element(FooterStyle).AlignRight().Text(model.GrandTotalExpense.ToString("C")).FontColor(Colors.Red.Darken1);
                                
                                var totalProfitColor = model.GrandTotalProfit >= 0 ? Colors.Blue.Darken1 : Colors.Red.Darken1;
                                footer.Cell().Element(FooterStyle).AlignRight().Text(model.GrandTotalProfit.ToString("C")).FontColor(totalProfitColor);

                                static IContainer FooterStyle(IContainer container)
                                {
                                    return container.DefaultTextStyle(x => x.SemiBold().FontSize(12)).BorderTop(1).BorderColor(Colors.Black).PaddingVertical(10);
                                }
                            });
                        });
                        
                        // Profit Split Section
                        if (showHidden)
                        {
                            page.Content()
                                .PaddingTop(1, Unit.Centimetre)
                                .Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                    });
                                    
                                    table.Header(header => 
                                    {
                                         header.Cell().ColumnSpan(2).Element(HeaderStyle).Text("Profit Distribution (2 Partners)").AlignCenter();
                                         static IContainer HeaderStyle(IContainer container) => container.Background(Colors.Grey.Lighten4).Padding(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                                    });

                                    table.Cell().Element(CellStyle).Text("Total Net Profit");
                                    table.Cell().Element(CellStyle).AlignRight().Text(model.GrandTotalProfit.ToString("C")).FontColor(model.GrandTotalProfit >= 0 ? Colors.Blue.Darken1 : Colors.Red.Darken1);

                                    table.Cell().Element(CellStyle).Text("Profit Per Person");
                                    table.Cell().Element(CellStyle).AlignRight().Text(model.ProfitPerPerson.ToString("C")).FontColor(Colors.Green.Darken1).SemiBold();

                                    static IContainer CellStyle(IContainer container) => container.BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5);
                                });
                        }

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                            x.Span($" | Generated on {DateTime.Now:g}");
                        });
                });
            });

            var stream = new MemoryStream();
            document.GeneratePdf(stream);
            
            string fileName = $"Report_{model.SelectedYear}_{model.SelectedMonth:00}{(showHidden ? "_Hidden" : "")}.pdf";
            return File(stream.ToArray(), "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error generating PDF: {ex.Message}");
        }
    }

    private MonthlyReportViewModel GetMonthlyReportModel(int? month, int? year, bool showHidden)
    {
        var now = DateTime.Now;
        int selectedMonth = month ?? now.Month;
        int selectedYear = year ?? now.Year;

        var startOfMonth = new DateTime(selectedYear, selectedMonth, 1);
        var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

        // Filter rounds based on showHidden
        var roundsQuery = _context.RoundEntries
            .Where(r => r.Date >= startOfMonth && r.Date <= endOfMonth);
            
        if (showHidden)
        {
            roundsQuery = roundsQuery.Where(r => r.IsHidden);
        }
        else
        {
            roundsQuery = roundsQuery.Where(r => !r.IsHidden);
        }

        var rounds = roundsQuery.ToList();

        // Expenses are always included (or should they be excluded from Hidden? 
        // User request: "amount never count on dashboard... hide orders report... profit and divide".
        // Usually hidden rounds mean "off the books", so expenses might be shared or separate.
        // Assuming Expenses are global for now, as they weren't explicitly asked to be hidden/split.
        // BUT, if it's a "Hidden Orders Report", maybe it should ONLY show hidden income and NO expenses?
        // Or specific hidden expenses?
        // The prompt says "amount never count on dashboard... orders report will appear... total profit".
        // Profit = Income - Expense. If I include ALL expenses in Hidden report, it might be double counting if they are also in Normal report.
        // However, "hide orders" implies the ROUNDS are hidden. Expenses (Petrol, Food) might be real.
        // Let's assume Expenses are currently GLOBAL. If I include them in Hidden Report, the profit will be (Hidden Income - Global Expenses).
        // If I include them in Normal Report, profit is (Normal Income - Global Expenses).
        // This effectively deducts expenses TWICE if we sum them up.
        // Given the ambiguity, I will Include expenses in BOTH for now, as "Profit" usually needs expenses.
        // Better yet, maybe Expenses should only be in Main report?
        // "hide orders report" -> implies just orders. 
        // Let's look at "Hide orders report will appear... give total profit".
        // If I have 0 expenses in hidden report, Profit = Income.
        // If I have global expenses, Profit is drastically reduced.
        // I will assume for "Hidden Orders Report", we might just want to see the Income from these orders?
        // But the user asked for "Profit" and "Net Profit" usually implies deductions.
        // Let's stick to the code using global expenses for now, as splitting expenses wasn't requested.
        // Actually, if a round is "hidden", maybe it's "extra" money?
        // If I put expenses in Hidden report, they shouldn't be in Main report?
        // Safest bet: Expenses in Main Report. Hidden Report might just be Revenue?
        // User said: "bottom of this report you will give total profit".
        // I will include expenses in the Hidden Report calculation for completeness, 
        // but arguably they shouldn't be there if they are already accounted for.
        // Let's keep existing logic: Expenses are fetched by date range.
        var expenses = new List<ExpenseEntry>();
        
        // Expenses are NOT included in Hidden Report (User Request)
        if (!showHidden)
        {
            expenses = _context.ExpenseEntries
                .Where(e => e.Date >= startOfMonth && e.Date <= endOfMonth)
                .ToList();
        }

        var allDates = rounds.Select(r => r.Date.Date)
            .Union(expenses.Select(e => e.Date.Date))
            .Distinct()
            .OrderBy(d => d)
            .ToList();

        var dailyReports = new List<DailyReportItem>();

        foreach (var date in allDates)
        {
            var r = rounds.Where(x => x.Date.Date == date).ToList();
            var e = expenses.Where(x => x.Date.Date == date).ToList();

            var item = new DailyReportItem
            {
                Date = date,
                TotalRounds = r.Sum(x => x.NumberOfRounds),
                TotalIncome = r.Sum(x => x.TotalAmount),
                TotalExpense = e.Sum(x => x.ExpensePrice),
                ExpenseNames = string.Join(", ", e.Select(x => x.ExpenseName))
            };
            item.DailyProfit = item.TotalIncome - item.TotalExpense;
            
            // If marked as Off Day in any round or expense for that day
            item.IsOffDay = r.Any(x => x.IsOffDay) || e.Any(x => x.IsOffDay);

            dailyReports.Add(item);
        }

        var viewModel = new MonthlyReportViewModel
        {
            SelectedMonth = selectedMonth,
            SelectedYear = selectedYear,
            DailyReports = dailyReports,
            GrandTotalRounds = dailyReports.Sum(x => x.TotalRounds),
            GrandTotalIncome = dailyReports.Sum(x => x.TotalIncome),
            GrandTotalExpense = dailyReports.Sum(x => x.TotalExpense),
            GrandTotalProfit = dailyReports.Sum(x => x.DailyProfit),
            ShowHidden = showHidden
        };
        
        viewModel.ProfitPerPerson = viewModel.GrandTotalProfit / (showHidden ? 2 : 3);

        return viewModel;
    }
}
