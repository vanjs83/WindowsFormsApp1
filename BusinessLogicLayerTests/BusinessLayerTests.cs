using Microsoft.VisualStudio.TestTools.UnitTesting;
using BusinessLogicLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Tests
{
    [TestClass()]
    public class BusinessLayerTests
    {
     

        [TestMethod()]
        public void ConvertHrkTest()
        {
            var result= BusinessLayer.ConvertHrk("10","10");
            Assert.AreEqual(typeof(string), result.GetType());
            Assert.AreEqual("100,00" , result);
        }


        [TestMethod]
        [ExpectedException(typeof(FormatException),
    "A sumaValue of null was inappropriately allowed.")]
        public void ConvertHrkExpectedExceptionTest()
        {
           BusinessLayer.ConvertHrk("10", "10.00");
        }


    }
}