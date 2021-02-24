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
    public class ExpenseTests
    {
        [TestMethod]
        public void TestExpenseNextContributionDateOne()
        {
            //Arrange
            var recurrence = 1;
            var today = Convert.ToDateTime("2021-01-05 00:00:00");
            var nextExpenseDate = Convert.ToDateTime("2021-01-01 00:00:00");

            var mockAccountAccessor = new Mock<IAccountDataAccessor>();
            var mockBudgetAccessor = new Mock<IBudgetingDataAccessor>();
            var mockPlaidAccessor = new Mock<IPlaidDataAccessor>();
            var dashboardManager = new DashboardManager(mockAccountAccessor.Object, mockPlaidAccessor.Object, mockBudgetAccessor.Object);

            //Act
            var nextContributionDate = dashboardManager.CalculateNextExpenseContributionDate(today, nextExpenseDate, recurrence);

            //Assert
            Assert.AreEqual(Convert.ToDateTime("2021-01-08 00:00:00"), nextContributionDate);
        }

        [TestMethod]
        public void TestExpenseNextContributionDateTwo()
        {
            //Arrange
            var recurrence = 2;
            var today = Convert.ToDateTime("2021-01-05 00:00:00");
            var nextExpenseDate = Convert.ToDateTime("2021-01-01 00:00:00");

            var mockAccountAccessor = new Mock<IAccountDataAccessor>();
            var mockBudgetAccessor = new Mock<IBudgetingDataAccessor>();
            var mockPlaidAccessor = new Mock<IPlaidDataAccessor>();
            var dashboardManager = new DashboardManager(mockAccountAccessor.Object, mockPlaidAccessor.Object, mockBudgetAccessor.Object);

            //Act
            var nextContributionDate = dashboardManager.CalculateNextExpenseContributionDate(today, nextExpenseDate, recurrence);

            //Assert
            Assert.AreEqual(Convert.ToDateTime("2021-01-15 00:00:00"), nextContributionDate);
        }

        [TestMethod]
        public void TestExpenseNextContributionDateThree()
        {
            //Arrange
            var recurrence = 3;
            var today = Convert.ToDateTime("2021-01-05 00:00:00");
            var nextExpenseDate = Convert.ToDateTime("2021-01-01 00:00:00");

            var mockAccountAccessor = new Mock<IAccountDataAccessor>();
            var mockBudgetAccessor = new Mock<IBudgetingDataAccessor>();
            var mockPlaidAccessor = new Mock<IPlaidDataAccessor>();
            var dashboardManager = new DashboardManager(mockAccountAccessor.Object, mockPlaidAccessor.Object, mockBudgetAccessor.Object);

            //Act
            var nextContributionDate = dashboardManager.CalculateNextExpenseContributionDate(today, nextExpenseDate, recurrence);

            //Assert
            Assert.AreEqual(Convert.ToDateTime("2021-02-01 00:00:00"), nextContributionDate);
        }

        [TestMethod]
        public void TestExpenseNextContributionDateFour()
        {
            //Arrange
            var recurrence = 4;
            var today = Convert.ToDateTime("2021-01-05 00:00:00");
            var nextExpenseDate = Convert.ToDateTime("2021-01-01 00:00:00");

            var mockAccountAccessor = new Mock<IAccountDataAccessor>();
            var mockBudgetAccessor = new Mock<IBudgetingDataAccessor>();
            var mockPlaidAccessor = new Mock<IPlaidDataAccessor>();
            var dashboardManager = new DashboardManager(mockAccountAccessor.Object, mockPlaidAccessor.Object, mockBudgetAccessor.Object);

            //Act
            var nextContributionDate = dashboardManager.CalculateNextExpenseContributionDate(today, nextExpenseDate, recurrence);

            //Assert
            Assert.AreEqual(Convert.ToDateTime("2021-03-01 00:00:00"), nextContributionDate);
        }

        [TestMethod]
        public void TestExpenseNextContributionDateFive()
        {
            //Arrange
            var recurrence = 5;
            var today = Convert.ToDateTime("2021-01-05 00:00:00");
            var nextExpenseDate = Convert.ToDateTime("2021-01-01 00:00:00");

            var mockAccountAccessor = new Mock<IAccountDataAccessor>();
            var mockBudgetAccessor = new Mock<IBudgetingDataAccessor>();
            var mockPlaidAccessor = new Mock<IPlaidDataAccessor>();
            var dashboardManager = new DashboardManager(mockAccountAccessor.Object, mockPlaidAccessor.Object, mockBudgetAccessor.Object);

            //Act
            var nextContributionDate = dashboardManager.CalculateNextExpenseContributionDate(today, nextExpenseDate, recurrence);

            //Assert
            Assert.AreEqual(Convert.ToDateTime("2021-04-01 00:00:00"), nextContributionDate);
        }

        [TestMethod]
        public void TestExpenseNextContributionDateSix()
        {
            //Arrange
            var recurrence = 6;
            var today = Convert.ToDateTime("2021-01-05 00:00:00");
            var nextExpenseDate = Convert.ToDateTime("2021-01-01 00:00:00");

            var mockAccountAccessor = new Mock<IAccountDataAccessor>();
            var mockBudgetAccessor = new Mock<IBudgetingDataAccessor>();
            var mockPlaidAccessor = new Mock<IPlaidDataAccessor>();
            var dashboardManager = new DashboardManager(mockAccountAccessor.Object, mockPlaidAccessor.Object, mockBudgetAccessor.Object);

            //Act
            var nextContributionDate = dashboardManager.CalculateNextExpenseContributionDate(today, nextExpenseDate, recurrence);

            //Assert
            Assert.AreEqual(Convert.ToDateTime("2021-07-01 00:00:00"), nextContributionDate);
        }

        [TestMethod]
        public void TestExpenseNextContributionDateSeven()
        {
            //Arrange
            var recurrence = 7;
            var today = Convert.ToDateTime("2021-01-05 00:00:00");
            var nextExpenseDate = Convert.ToDateTime("2021-01-01 00:00:00");

            var mockAccountAccessor = new Mock<IAccountDataAccessor>();
            var mockBudgetAccessor = new Mock<IBudgetingDataAccessor>();
            var mockPlaidAccessor = new Mock<IPlaidDataAccessor>();
            var dashboardManager = new DashboardManager(mockAccountAccessor.Object, mockPlaidAccessor.Object, mockBudgetAccessor.Object);

            //Act
            var nextContributionDate = dashboardManager.CalculateNextExpenseContributionDate(today, nextExpenseDate, recurrence);

            //Assert
            Assert.AreEqual(Convert.ToDateTime("2022-01-01 00:00:00"), nextContributionDate);
        }
    }
}
