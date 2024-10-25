using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SingleResponsibilityPrinciple;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SingleResponsibilityPrinciple.Tests
{
    [TestClass()]
    public class TradeProcessorTests
    {
        [TestMethod()]
        public void TestEmptyFile()
        {
            // Arrange
            var emptyStream = new MemoryStream(); // Empty stream
            var tradeProcessor = new TradeProcessor();

            // Act
            int countBefore = CountDbRecords();
            tradeProcessor.ProcessTrades(emptyStream);

            // Assert
            int countAfter = CountDbRecords();
            Assert.AreEqual(countBefore, countAfter); // No trades added
        }

        [TestMethod()]
        public void TestTenTrades()
        {
            // Arrange
            var tradeData = string.Join("\n", Enumerable.Repeat("GBPUSD,1000,1.51", 10));
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(tradeData));
            var tradeProcessor = new TradeProcessor();

            // Act
            int countBefore = CountDbRecords();
            tradeProcessor.ProcessTrades(stream);

            // Assert
            int countAfter = CountDbRecords();
            Assert.AreEqual(countBefore + 10, countAfter); // 10 trades added
        }

        [TestMethod()]
        public void TestBadTradeTooManyValues()
        {
            // Arrange
            var badTrade = "GBPUSD,1000,1.51,ExtraValue";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(badTrade));
            var tradeProcessor = new TradeProcessor();

            // Act
            int countBefore = CountDbRecords();
            tradeProcessor.ProcessTrades(stream);

            // Assert
            int countAfter = CountDbRecords();
            Assert.AreEqual(countBefore, countAfter); // No trades added
        }

        [TestMethod()]
        public void TestBadTradeNegativeLotSize()
        {
            // Arrange
            var badTrade = "GBPUSD,-100,1.51";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(badTrade));
            var tradeProcessor = new TradeProcessor();

            // Act
            int countBefore = CountDbRecords();
            tradeProcessor.ProcessTrades(stream);

            // Assert
            int countAfter = CountDbRecords();
            Assert.AreEqual(countBefore, countAfter); // No trades added
        }

        [TestMethod()]
        [ExpectedException(typeof(FileNotFoundException))]
        public void TestFileNotFound()
        {
            // Act
            using (var stream = File.OpenRead("nonexistentfile.txt"))
            {
                var tradeProcessor = new TradeProcessor();
                tradeProcessor.ProcessTrades(stream);
            }
        }

        [TestMethod()]
        public void TestInvalidCurrencyCode()
        {
            // Arrange
            var invalidCurrencyTrade = "XXXUSD,100,1.50"; 
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(invalidCurrencyTrade));
            var tradeProcessor = new TradeProcessor();

            // Act
            int countBefore = CountDbRecords();
            tradeProcessor.ProcessTrades(stream);

            // Assert
            int countAfter = CountDbRecords();
            Assert.AreEqual(countBefore, countAfter); // Expect no trades added, but it may add 1
        }


        private int CountDbRecords()
        {
            // Update this connection string with the one you use in your main project
            string genericConnectString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\BDDDo\source\repos\cis3285-unit8-f2024-BristonDomino\Unit8_SRP_F24\DataFiles\tradedatabase.mdf;Integrated Security=True;Connect Timeout=30;";
            using (var connection = new SqlConnection(genericConnectString))
            {
                if (connection.State == ConnectionState.Closed)
                {
                    //connection.Open();
                }
                string myScalarQuery = "SELECT COUNT(*) FROM trade";
                SqlCommand myCommand = new SqlCommand(myScalarQuery, connection);
                myCommand.Connection.Open();
                int count = (int)myCommand.ExecuteScalar();
                connection.Close();
                return count;
            }
        }


        [TestMethod()]
        public void TestNormalFile()
        {
            //Arrange
            //  -- Make sure you have a file named goodtrades.txt in your test project.  
            //     You can copy it from the main project and rename it.
            var tradeStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Unit8_SRP_F24Tests.goodtrades1.txt");
            var tradeProcessor = new TradeProcessor();

            //Act
            int countBefore = CountDbRecords();
            tradeProcessor.ProcessTrades(tradeStream);
            //Assert
            int countAfter = CountDbRecords();
            Assert.AreEqual(countBefore + 1, countAfter);
        }
    

        [TestMethod()]
        public void ProcessTradesTest()
        {
           // Assert.Fail();
        }
    }
}