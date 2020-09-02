using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Demo
{
    public class Chrome : TrainingAutomation
    {

        /// <summary>
        /// verify if header elements can be found in all locales
        /// </summary>
        [Test]
        [TestCaseSource(typeof(TrainingAutomation), "BrowserToRunWith")]
        public void HeaderTestRussian(string browserName)
        {
            Initialize(browserName);
            SelectResourceFile("English");
            SelectLanguage("English");
            VerifyHeaderSection();

            SelectResourceFile("russian");
            SelectLanguage("Русский");
            VerifyHeaderSection();

            SelectResourceFile("ukrainine");
            SelectLanguage("Українська");
            VerifyHeaderSection();
        }
    }
}