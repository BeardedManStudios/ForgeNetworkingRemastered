using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleJSONEditor;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleJSONEditor.Tests
{
    [TestClass()]
    public class JSONNodeTests
    {
        [TestMethod()]
        public void ParseInvariantCultureTest()
        {
            var storedCulture = Thread.CurrentThread.CurrentCulture;
            JSONNode jsonNode;
            float expected = 0.15f;
            float actual;
            try
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                var jsonString = String.Format("{{ floatTest: {0} }}", expected.ToString(CultureInfo.InvariantCulture));
                jsonNode = SimpleJSONEditor.JSON.Parse(jsonString);
                actual = jsonNode.Children.FirstOrDefault().AsFloat;
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = storedCulture;
            }
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ParseCommaDecimalSeparatorCultureTest()
        {
            var storedCulture = Thread.CurrentThread.CurrentCulture;
            JSONNode jsonNode;
            float expected = 0.15f;
            float actual;
            try
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("fi-FI");
                var jsonString = String.Format("{{ floatTest: {0} }}", expected.ToString(CultureInfo.InvariantCulture));
                jsonNode = SimpleJSONEditor.JSON.Parse(jsonString);
                actual = jsonNode.Children.FirstOrDefault().AsFloat;
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = storedCulture;
            }
            //var floatValue = jsonNode.AsFloat;
            Assert.AreEqual(expected, actual);
        }

    }
}