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

namespace SingleResponsibilityPrinciple.Tests
{
    [TestClass()]
    public class TradeProcessorTests
    {
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