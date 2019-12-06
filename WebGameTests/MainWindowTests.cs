using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebGame.Tests
{
    [TestClass()]
    public class MainWindowTests
    {
        private MainWindow game = new MainWindow();
        

        [TestMethod()]
        public void MainWindowTest()
        {
            Assert.IsTrue(game);
        }
    }
}