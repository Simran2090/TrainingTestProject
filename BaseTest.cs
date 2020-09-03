using System;
using System.Collections.Generic;
using System.Threading;
using log4net;
using NUnit.Framework;
using OpenQA.Selenium.Remote;


namespace Demo
{
    [TestFixture]
    [Parallelizable(ParallelScope.Fixtures)]
    public class BaseTest : TrainingAutomation
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

        /// <summary>
        /// Search's for trainings 
        /// </summary>
        /// <param name="browserName"></param>
        [Test]
        public void SearchTrainings(string browserName)
        {
            Initialize(browserName);
            SelectLanguage("English");
            ExpandSearchTextBox();
            SelectCountry(Countries.Belarus);
            ClickCourseCheckboxes("Minsk");
            CollapseSearchTextBox();
            Assert.IsTrue(GetTrainingsCount() == 12);
        }

        [Test]
        public void VerifyNoTrainingsMessage(string browserName)
        {
            Initialize(browserName);
            SelectLanguage("English");
            ExpandSearchTextBox();
            SelectCountry(Countries.Armenia);
            ClickCourseCheckboxes("Yerevan");
            CollapseSearchTextBox();
            Assert.IsTrue(NoTrainingAvailable());
        }

        [Test]
        public void VerifyFileUploaded()
        {
            NavigateToUrl("https://the-internet.herokuapp.com/upload");
            UploadDocument(@"D:\", "TestingUpload.rtf");
            ClickOnUploadButton();
            Assert.IsTrue(FileSuccessfullyUploaded("File Uploaded!"),"File not uploaded");
        }

        [Test]
        public void HandlingWindows()
        {
            NavigateToUrl("https://the-internet.herokuapp.com/windows");
            ClickNextWindowLink();
            SwitchToNewWindow();
            Assert.IsTrue(SwitchingToNewWindowSuccessful());
        }

        [TearDown]
        public void CleanUp()
        {
            CloseTheBrowser();
        }
    }
}
//2.41 version download for reporting issue 