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

    [Test]
    public void two_days_in_same_month_with_31k()
    {
        GivenAllBudgets(new List<Budget>()
        {
            new Budget()
            {
                YearMonth = "202210",
                Amount = 31000
            }
        });

        var totalAmount = WhenQuery(new DateTime(2022, 10, 15), new DateTime(2022, 10, 16));

        totalAmount.Should().Be(2000);
    }

    [Test]
    public void full_month_with_31k()
    {
        GivenAllBudgets(new List<Budget>()
        {
            new Budget()
            {
                YearMonth = "202210",
                Amount = 31000
            }
        });

        var totalAmount = WhenQuery(new DateTime(2022, 10, 1), new DateTime(2022, 10, 31));

        totalAmount.Should().Be(31000);
    }

    [Test]
    public void cross_month_with_31k_in_oct_3k_in_nov()
    {
        GivenAllBudgets(new List<Budget>()
        {
            new Budget()
            {
                YearMonth = "202210",
                Amount = 31000
            },
            new Budget()
            {
                YearMonth = "202211",
                Amount = 3000
            },
        });

        var totalAmount = WhenQuery(new DateTime(2022, 10, 31), new DateTime(2022, 11, 2));

        totalAmount.Should().Be(1200);
    }

    [Test]
    public void cross_month_with_31k_in_oct_3k_in_nov_31k_in_dec()
    {
        GivenAllBudgets(new List<Budget>()
        {
            new Budget()
            {
                YearMonth = "202210",
                Amount = 31000
            },
            new Budget()
            {
                YearMonth = "202211",
                Amount = 3000
            },
            new Budget()
            {
                YearMonth = "202212",
                Amount = 31000
            },
        });

        var totalAmount = WhenQuery(new DateTime(2022, 10, 30), new DateTime(2022, 12, 5));

        totalAmount.Should().Be(2000 + 3000 + 5000);
    }

    [Test]
    public void cross_month_with_31k_in_oct_3k_in_nov_31k_in_dec_during_oct_5_dec_10()
    {
        GivenAllBudgets(new List<Budget>()
        {
            new Budget()
            {
                YearMonth = "202210",
                Amount = 31000
            },
            new Budget()
            {
                YearMonth = "202211",
                Amount = 3000
            },
            new Budget()
            {
                YearMonth = "202212",
                Amount = 31000
            },
        });

        var totalAmount = WhenQuery(new DateTime(2022, 10, 5), new DateTime(2022, 12, 10));

        totalAmount.Should().Be(27000 + 3000 + 10000);
    }

    [Test]
    public void cross_month_with_3k_in_sep_31k_in_oct_3k_in_nov_31k_in_dec_during_sep_30_dec_2()
    {
        GivenAllBudgets(new List<Budget>()
        {
            new Budget
            {
                YearMonth = "202209",
                Amount = 3000
            },
            new Budget()
            {
                YearMonth = "202210",
                Amount = 31000
            },
            new Budget()
            {
                YearMonth = "202211",
                Amount = 3000
            },
            new Budget()
            {
                YearMonth = "202212",
                Amount = 31000
            },
        });

        var totalAmount = WhenQuery(new DateTime(2022, 9, 30), new DateTime(2022, 12, 2));

        totalAmount.Should().Be(100 + 31000 + 3000 + 2000);
    }

    [Test]
    public void invalid_start_end_date()
    {
        GivenAllBudgets(new List<Budget>());

        var totalAmount = WhenQuery(new DateTime(2022, 10, 16), new DateTime(2022, 10, 15));

        totalAmount.Should().Be(0);
    }

    [Test]
    public void when_no_budget()
    {
        GivenAllBudgets(new List<Budget>());

        var totalAmount = WhenQuery(new DateTime(2022, 10, 16), new DateTime(2022, 10, 16));

        totalAmount.Should().Be(0); 
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