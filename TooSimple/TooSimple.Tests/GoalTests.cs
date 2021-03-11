using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using TooSimple.Data;
using TooSimple.DataAccessors;
using TooSimple.Extensions;
using TooSimple.Managers;
using TooSimple.Models.DataModels;

namespace TooSimple.Tests
{
    [TestClass]
    public class GoalTests
    {
        [TestMethod]
        public void TestNewGoalContributionsScheduleOne()
        {
            //Arrange
            var testSchedule = 1;
            var todayDate = Convert.ToDateTime("2021-02-01 08:00:00");
            var goal = new GoalDM
            {
                AmountContributed = 100,
                AmountSpent = 50,
                AutoRefill = false,
                GoalAmount = 1000,
                UserAccountId = "123",
                CreationDate = Convert.ToDateTime("2021-01-01 08:00:00"),
                DesiredCompletionDate = Convert.ToDateTime("2021-03-01 08:00:00"),
                ExpenseFlag = false,
                Paused = false,
                GoalName = "Test Goal Schedule 1",
                GoalId = "123",
                FundingScheduleId = "1234"
            };

            var fundingSchedule = new FundingScheduleDM
            {
                UserAccountId = "123",
                FirstContributionDate = Convert.ToDateTime("2020-01-01 08:00:00"),
                Frequency = testSchedule,
                FundingScheduleId = "1234",
                FundingScheduleName = "Test Schedule 1"
            };

            var mockAccountAccessor = new Mock<IAccountDataAccessor>();
            var mockBudgetAccessor = new Mock<IBudgetingDataAccessor>();
            var mockPlaidAccessor = new Mock<IPlaidDataAccessor>();
            var dashboardManager = new DashboardManager(mockAccountAccessor.Object, mockPlaidAccessor.Object, mockBudgetAccessor.Object);

            //Act
            var nextContribution = dashboardManager.CalculateNextContribution(goal, fundingSchedule, todayDate);

            //Assert
            Assert.AreEqual(225.00M, nextContribution.NextContributionAmount);
            Assert.AreEqual(Convert.ToDateTime("2021-02-03 00:00:00"), nextContribution.NextContributionDate);
        }

        [TestMethod]
        public void TestGoalContributionsScheduleOne()
        {
            //Arrange
            var testSchedule = 1;
            var todayDate = Convert.ToDateTime("2021-02-01 08:00:00");
            var goal = new GoalDM
            {
                AmountContributed = 100,
                AmountSpent = 50,
                AutoRefill = false,
                GoalAmount = 1000,
                UserAccountId = "123",
                CreationDate = Convert.ToDateTime("2021-01-01 08:00:00"),
                DesiredCompletionDate = Convert.ToDateTime("2021-03-01 08:00:00"),
                ExpenseFlag = false,
                Paused = false,
                GoalName = "Test Goal Schedule 1",
                GoalId = "123",
                FundingScheduleId = "1234",
                NextContributionDate = Convert.ToDateTime("2021-01-27 13:53:23")
            };

            var fundingSchedule = new FundingScheduleDM
            {
                UserAccountId = "123",
                FirstContributionDate = Convert.ToDateTime("2020-01-01 08:00:00"),
                Frequency = testSchedule,
                FundingScheduleId = "1234",
                FundingScheduleName = "Test Schedule 1"
            };

            var mockAccountAccessor = new Mock<IAccountDataAccessor>();
            var mockBudgetAccessor = new Mock<IBudgetingDataAccessor>();
            var mockPlaidAccessor = new Mock<IPlaidDataAccessor>();
            var dashboardManager = new DashboardManager(mockAccountAccessor.Object, mockPlaidAccessor.Object, mockBudgetAccessor.Object);

            //Act
            var nextContribution = dashboardManager.CalculateNextContribution(goal, fundingSchedule, todayDate);

            //Assert
            Assert.AreEqual(225.00M, nextContribution.NextContributionAmount);
            Assert.AreEqual(Convert.ToDateTime("2021-02-03 00:00:00"), nextContribution.NextContributionDate);
        }



        //[TestMethod]
        //public void TestGoalContributionsScheduleOne()
        //{
        //    //Arrange
        //    var testSchedule = 1;
        //    var initialContribution = Convert.ToDateTime("2021-01-01 08:00:00");
        //    var completionDate = initialContribution.AddDays(60);

        //    var mockAccountAccessor = new Mock<IAccountDataAccessor>();
        //    var mockBudgetAccessor = new Mock<IBudgetingDataAccessor>();
        //    var mockPlaidAccessor = new Mock<IPlaidDataAccessor>();
        //    var dashboardManager = new DashboardManager(mockAccountAccessor.Object, mockPlaidAccessor.Object, mockBudgetAccessor.Object);

        //    //Act
        //    var numberOfContributions = dashboardManager.CalculateContributionsToComplete(completionDate, initialContribution, testSchedule);

        //    //Assert
        //    Assert.AreEqual(9, numberOfContributions);
        //}

        //[TestMethod]
        //public void TestGoalContributionsScheduleTwo()
        //{
        //    //Arrange
        //    var testSchedule = 2;
        //    var initialContribution = Convert.ToDateTime("2021-01-01 08:00:00");
        //    var completionDate = initialContribution.AddDays(60);

        //    var mockAccountAccessor = new Mock<IAccountDataAccessor>();
        //    var mockBudgetAccessor = new Mock<IBudgetingDataAccessor>();
        //    var mockPlaidAccessor = new Mock<IPlaidDataAccessor>();
        //    var dashboardManager = new DashboardManager(mockAccountAccessor.Object, mockPlaidAccessor.Object, mockBudgetAccessor.Object);

        //    //Act
        //    var numberOfContributions = dashboardManager.CalculateContributionsToComplete(completionDate, initialContribution, testSchedule);

        //    //Assert
        //    Assert.AreEqual(5, numberOfContributions);
        //}

        //[TestMethod]
        //public void TestGoalContributionsScheduleThree()
        //{
        //    //Arrange
        //    var testSchedule = 3;
        //    var initialContribution = Convert.ToDateTime("2021-01-01 08:00:00");
        //    var completionDate = initialContribution.AddDays(120);

        //    var mockAccountAccessor = new Mock<IAccountDataAccessor>();
        //    var mockBudgetAccessor = new Mock<IBudgetingDataAccessor>();
        //    var mockPlaidAccessor = new Mock<IPlaidDataAccessor>();
        //    var dashboardManager = new DashboardManager(mockAccountAccessor.Object, mockPlaidAccessor.Object, mockBudgetAccessor.Object);

        //    //Act
        //    var numberOfContributions = dashboardManager.CalculateContributionsToComplete(completionDate, initialContribution, testSchedule);

        //    //Assert
        //    Assert.AreEqual(4, numberOfContributions);
        //}

        //[TestMethod]
        //public void TestGoalContributionsScheduleFour()
        //{
        //    //Arrange
        //    var testSchedule = 4;
        //    var initialContribution = Convert.ToDateTime("2021-01-01 08:00:00");
        //    var completionDate = initialContribution.AddDays(120);

        //    var mockAccountAccessor = new Mock<IAccountDataAccessor>();
        //    var mockBudgetAccessor = new Mock<IBudgetingDataAccessor>();
        //    var mockPlaidAccessor = new Mock<IPlaidDataAccessor>();
        //    var dashboardManager = new DashboardManager(mockAccountAccessor.Object, mockPlaidAccessor.Object, mockBudgetAccessor.Object);

        //    //Act
        //    var numberOfContributions = dashboardManager.CalculateContributionsToComplete(completionDate, initialContribution, testSchedule);

        //    //Assert
        //    Assert.AreEqual(2, numberOfContributions);
        //}

        //[TestMethod]
        //public void TestGoalNextContributionDateOne()
        //{
        //    //Arrange
        //    var testSchedule = 1;
        //    var lastFunded = Convert.ToDateTime("2021-01-01 08:00:00");
        //    var scheduleDate = lastFunded.AddDays(-63);

        //    var mockAccountAccessor = new Mock<IAccountDataAccessor>();
        //    var mockBudgetAccessor = new Mock<IBudgetingDataAccessor>();
        //    var mockPlaidAccessor = new Mock<IPlaidDataAccessor>();
        //    var dashboardManager = new DashboardManager(mockAccountAccessor.Object, mockPlaidAccessor.Object, mockBudgetAccessor.Object);

        //    //Act
        //    var nextContribution = dashboardManager.CalculateNextGoalContributionDate(lastFunded, scheduleDate, testSchedule);

        //    //Assert
        //    Assert.AreEqual(Convert.ToDateTime("2021-01-08, 00:00:00"), nextContribution);
        //}

        //[TestMethod]
        //public void TestGoalNextContributionDateTwo()
        //{
        //    //Arrange
        //    var testSchedule = 2;
        //    var lastFunded = Convert.ToDateTime("2021-01-01 08:00:00");
        //    var scheduleDate = lastFunded.AddDays(-70);

        //    var mockAccountAccessor = new Mock<IAccountDataAccessor>();
        //    var mockBudgetAccessor = new Mock<IBudgetingDataAccessor>();
        //    var mockPlaidAccessor = new Mock<IPlaidDataAccessor>();
        //    var dashboardManager = new DashboardManager(mockAccountAccessor.Object, mockPlaidAccessor.Object, mockBudgetAccessor.Object);

        //    //Act
        //    var nextContribution = dashboardManager.CalculateNextGoalContributionDate(lastFunded, scheduleDate, testSchedule);

        //    //Assert
        //    Assert.AreEqual(Convert.ToDateTime("2021-01-15, 00:00:00"), nextContribution);
        //}

        //[TestMethod]
        //public void TestGoalNextContributionDateThree()
        //{
        //    //Arrange
        //    var testSchedule = 3;
        //    var lastFunded = Convert.ToDateTime("2021-01-01 08:00:00");
        //    var scheduleDate = lastFunded.AddDays(-92);

        //    var mockAccountAccessor = new Mock<IAccountDataAccessor>();
        //    var mockBudgetAccessor = new Mock<IBudgetingDataAccessor>();
        //    var mockPlaidAccessor = new Mock<IPlaidDataAccessor>();
        //    var dashboardManager = new DashboardManager(mockAccountAccessor.Object, mockPlaidAccessor.Object, mockBudgetAccessor.Object);

        //    //Act
        //    var nextContribution = dashboardManager.CalculateNextGoalContributionDate(lastFunded, scheduleDate, testSchedule);

        //    //Assert
        //    Assert.AreEqual(Convert.ToDateTime("2021-02-01, 00:00:00"), nextContribution);
        //}

        //[TestMethod]
        //public void TestGoalNextContributionDateFour()
        //{
        //    //Arrange
        //    var testSchedule = 4;
        //    var lastFunded = Convert.ToDateTime("2021-01-01 08:00:00");
        //    var scheduleDate = lastFunded.AddDays(-122);

        //    var mockAccountAccessor = new Mock<IAccountDataAccessor>();
        //    var mockBudgetAccessor = new Mock<IBudgetingDataAccessor>();
        //    var mockPlaidAccessor = new Mock<IPlaidDataAccessor>();
        //    var dashboardManager = new DashboardManager(mockAccountAccessor.Object, mockPlaidAccessor.Object, mockBudgetAccessor.Object);

        //    //Act
        //    var nextContribution = dashboardManager.CalculateNextGoalContributionDate(lastFunded, scheduleDate, testSchedule);

        //    //Assert
        //    Assert.AreEqual(Convert.ToDateTime("2021-03-01, 00:00:00"), nextContribution);
        //}


        //[TestMethod]
        //public async void TestMethod1()
        //{
        //    //Arrange
        //    var mockDbContext = new Mock<ApplicationDbContext>();
        //    var mockBudgetAccessor = new Mock<IBudgetingDataAccessor>(mockDbContext.Object);
        //    var mockDashboardManager = new Mock<DashboardManager>(mockBudgetAccessor.Object);
        //    var mockDate = Convert.ToDateTime("2020-01-01 08:00:00");


        //    mockBudgetAccessor.Setup(x => x.GetGoalListDMAsync(It.IsAny<string>())).ReturnsAsync(new GoalListDM
        //    {
        //        Goals = new List<GoalDM>
        //        {
        //            new GoalDM
        //            {
        //                GoalAmount = 100.00M,
        //                CurrentBalance = 0.00M,
        //                UserAccountId = "test123",
        //                CreationDate = mockDate,
        //                DesiredCompletionDate = mockDate.AddDays(30),
        //                ExpenseFlag = false,
        //                Paused = false,
        //                GoalId = "testGoal",
        //                GoalName = "Test Goal",
        //                FundingScheduleId = "testSchedule",
        //            }
        //        }
        //    });

        //    mockBudgetAccessor.Setup(x => x.GetFundingScheduleDMAsync(It.IsAny<string>())).ReturnsAsync(new FundingScheduleDM
        //    {
        //        UserAccountId = "test123",
        //        FirstContributionDate = mockDate.AddDays(7),
        //        Frequency = 2,
        //        FundingScheduleName = "testSchedule",
        //        FundingScheduleId = "testSchedule"
        //    });

        //    mockBudgetAccessor.Setup(x => x.GetFundingHistoryListDMAsync(It.IsAny<string>())).ReturnsAsync(new FundingHistoryListDM
        //    {
        //        FundingHistories = new List<FundingHistoryDM>
        //        {

        //        }
        //    });

        //    //Act
        //    var test = mockDashboardManager.Object.CalculateContributionsToComplete(mockDate.AddDays(30), mockDate, 2);
        //    //Assert
        //    Assert.AreEqual(2, test);
        //}
    }
}
