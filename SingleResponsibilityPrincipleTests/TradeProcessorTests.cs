using Microsoft.VisualStudio.TestTools.UnitTesting;
using SingleResponsibilityPrinciple;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SingleResponsibilityPrinciple.Tests
{
    [TestClass()]
    public class TradeProcessorTests
    {
        private int CountDbRecords()
        {
            using (var connection = new System.Data.SqlClient.SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""C:\Users\jsenger1\source\repos\cis-3285-asg-8-Jsenger1\tradedatabase.mdf"";Integrated Security=True;Connect Timeout=30  ;"))
            //using (var connection = new System.Data.SqlClient.SqlConnection("Data Source=(local);Initial Catalog=TradeDatabase;Integrated Security=True;"))
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                string myScalarQuery = "SELECT COUNT(*) FROM trade";
                SqlCommand myCommand = new SqlCommand(myScalarQuery, connection);
                //myCommand.Connection.Open();
                int count = (int)myCommand.ExecuteScalar();
                connection.Close();
                return count;
            }
        }
        [TestMethod]
        // As a user I want a program that adds valid trades to a database so that the user can see what trades are being made. 
        public void TestNormalFile()
        {
            //Arrange
            var tradeStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SingleResponsibilityPrincipleTests.goodtrades.txt");

            var tradeProcessor = new TradeProcessor();


            //Act
            int countBefore = CountDbRecords();
            tradeProcessor.ProcessTrades(tradeStream);

            //Assert
            int countAfter = CountDbRecords();
            Assert.AreEqual(countBefore+4, countAfter);
        }
        [TestMethod]
        // As a user I want a program that doesn't add bad trades to a databse so that all my trades can be legal.
        public void TestBadFile()
        {
            //Arrange
            var tradeStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SingleResponsibilityPrincipleTests.badtrades.txt");

            var tradeProcessor = new TradeProcessor();


            //Act
            int countBefore = CountDbRecords();
            tradeProcessor.ProcessTrades(tradeStream);

            //Assert
            int countAfter = CountDbRecords();
            Assert.AreEqual(countBefore, countAfter);
        }
        [TestMethod]
        // As a user I want to have a check on trade amount so that I cannot buy a negative amount.
        public void TestTradeNegative()
        {
            //Arrange
            var tradeStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SingleResponsibilityPrincipleTests.NegativeTradeAmount.txt");

            var tradeProcessor = new TradeProcessor();

            //Act
            int countBefore = CountDbRecords();
            tradeProcessor.ProcessTrades(tradeStream);

            //Assert
            int countAfter = CountDbRecords();
            Assert.AreEqual(countBefore, countAfter);

        }
        [TestMethod]
        // As a user I want to have a check on trade price so that I cannot break the system by entering in a very high number.
        public void TestLegalPrice()
        {
            //Arrange
            var tradeStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SingleResponsibilityPrincipleTests.LegalPrice.txt");

            var tradeProcessor = new TradeProcessor();


            //Act
            int countBefore = CountDbRecords();
            tradeProcessor.ProcessTrades(tradeStream);

            //Assert
            int countAfter = CountDbRecords();
            Assert.AreEqual(countBefore, countAfter);
        }
    }
}