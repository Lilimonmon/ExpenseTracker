using ExpenseTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ExpenseTracker.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ActionResult> Index()
        {
            //Week
            DateTime StartDate = DateTime.Today.AddDays(-6);
            DateTime EndDate = DateTime.Today;

            List<Transaction> SelectedTransactions = await _context.Transactions
                .Include(x => x.Category)
                .Where(y => y.Date >= StartDate && y.Date <= EndDate)
                .ToListAsync();

            //сумма доход
            int totalIncome = (int)SelectedTransactions
                .Where(x => x.Category.Type == "доход")
                .Sum(x => x.Amount);
            ViewBag.TotalIncome = totalIncome.ToString() + "₽";

            //сумма расход
            int totalExpense = (int)SelectedTransactions
                .Where(x => x.Category.Type == "расход")
                .Sum(x => x.Amount);
            ViewBag.TotalExpense = totalExpense.ToString() + "₽";

            //Баланс
            int Balance = totalIncome - totalExpense;
            ViewBag.Balance = Balance.ToString() + "₽";

            //Круговая диаграмма
            ViewBag.DoughnutChartData = SelectedTransactions
                .Where(x => x.Category.Type == "расход")
                .GroupBy(x => x.Category.CategoryId)
                .Select(x => new
                {
                    categoryTitleWithIcon = x.First().Category.Icon + " " + x.First().Category.Title,
                    amount = x.Sum(x => x.Amount),
                    //formattedAmount = x.Sum(x => x.Amount).ToString()
                })
                .OrderByDescending(x => x.amount)
                .ToList();

            //график расходов и доходов
            //доход
            List<SplineChart> IncomeSummary = SelectedTransactions
                .Where(x => x.Category.Type == "доход")
                .GroupBy(x => x.Date)
                .Select(x => new SplineChart()
                {
                    day = x.First().Date?.ToString("dd-MMM"),
                    income = x.Sum(x => x.Amount)
                })
                .ToList();

            //расход
            List<SplineChart> ExpenseSummary = SelectedTransactions
                .Where(x => x.Category.Type == "расход")
                .GroupBy(x => x.Date)
                .Select(x => new SplineChart()
                {
                    day = x.First().Date?.ToString("dd-MMM"),
                    expense = x.Sum(x => x.Amount)
                })
                .ToList();

            //объединение расход и доход
            var splineCharts = Enumerable.Range(0, 7)
                .Select(x => new SplineChart { day = StartDate.AddDays(x).ToString("dd-MMM"), expense = 0, income = 0 })
                .ToList();

            splineCharts.ForEach(x =>
            {
                x.income += IncomeSummary.FirstOrDefault(y => y.day == x.day)?.income ?? 0;
                x.expense += ExpenseSummary.FirstOrDefault(y => y.day == x.day)?.expense ?? 0;
            });

            ViewBag.SplineChart = splineCharts;


            //История транзакций
            ViewBag.HistoryTransactions = await _context.Transactions
                .Include(x => x.Category)
                .OrderByDescending(x => x.Date)
                .Take(5)
                .ToListAsync();

            return View();
        }
    }

    public class SplineChart
    {
        public string day;
        public int? income;
        public int? expense;
    }
}