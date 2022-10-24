using FluentAssertions;
using NSubstitute;

namespace BudgetService;

[TestFixture]
public class AccountingServiceTests
{
    private AccountingService _accountingService;
    private IBudgetRepo? _budgetRepo;

    [SetUp]
    public void SetUp()
    {
        _budgetRepo = Substitute.For<IBudgetRepo>();
        _accountingService = new AccountingService(_budgetRepo);
    }

    [Test]
    public void one_day()
    {
        GivenAllBudgets(new List<Budget>()
        {
            new Budget()
            {
                YearMonth = "202210",
                Amount = 31000
            }
        });

        var totalAmount = WhenQuery(new DateTime(2022, 10, 15), new DateTime(2022, 10, 15));

        totalAmount.Should().Be(1000);
    }

    private decimal WhenQuery(DateTime startDate, DateTime endDate)
    {
        var totalAmount = _accountingService.Query(startDate, endDate);
        return totalAmount;
    }

    private void GivenAllBudgets(List<Budget> budgets)
    {
        _budgetRepo.GetAll().Returns(budgets);
    }
}