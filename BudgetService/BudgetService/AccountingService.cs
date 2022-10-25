namespace BudgetService;

public class AccountingService
{
    private readonly IBudgetRepo _budgetRepo;

    public AccountingService(IBudgetRepo budgetRepo)
    {
        _budgetRepo = budgetRepo;
    }

    public decimal Query(DateTime start, DateTime end)
    {
        if (start > end)
        {
            return 0;
        }
        
        var budgets = _budgetRepo.GetAll();

        if (IsSameYearMonth(start, end))
        {
            var budget = GetBudget(start,budgets);
            var queryDays = (end - start).Days + 1;
            return queryDays * (budget.Amount / DateTime.DaysInMonth(start.Year, start.Month));
        }

        var startBudget = GetBudget(start, budgets);
        var amountOfStart = (DateTime.DaysInMonth(start.Year, start.Month) - start.Day + 1)*GetDaysAmount(start, startBudget.Amount);
        
        var endDateBudget = GetBudget(end, budgets);
        var amountOfEnd = end.Day*(GetDaysAmount(end, endDateBudget.Amount));

        var current = start.AddMonths(1);
        var totalMiddleAmount = 0;
        while (current < new DateTime(end.Year,end.Month,1))
        {
            var currentBudget = budgets.FirstOrDefault(x => x.YearMonth == current.ToString(
                "yyyyMM")) ?? new Budget();

            totalMiddleAmount += currentBudget.Amount;
            current = current.AddMonths(1);
        } 
            
        return amountOfStart + totalMiddleAmount +amountOfEnd;
    }

    private static decimal GetDaysAmount(DateTime dateTime, int amount)
    {
        return amount/DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
    }

    private static Budget? GetBudget(DateTime dateTime, List<Budget> budgets)
    {
        var budget = budgets.FirstOrDefault(x => x.YearMonth == dateTime.ToString("yyyyMM"));
        
        if (budget == null)
        {
            return new Budget();
        }
        return budget;
    }

    private static bool IsSameYearMonth(DateTime start, DateTime end)
    {
        return start.ToString("yyyyMM") == end.ToString("yyyyMM");
    }
}