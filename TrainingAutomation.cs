using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.PerformanceData;
using System.Globalization;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Reflection;
using System.Resources;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace Demo
{
    [TestFixture]
    public class TrainingAutomation
    {
        public enum Countries
        {
            Armenia,
            Belarus,
            Kazakhstan,
            Poland,
            Russia,
            Ukraine,
            Uzbekistan
        }

        private static readonly string LanguageSelected = "//a[contains(.,'{0}')]";

        protected IWebDriver driver = new ChromeDriver();

        protected ResourceManager resourceManager;

        protected CultureInfo cultureInfo;

        private static readonly By ExpandSearchTextbox =
            By.XPath("//input[contains(@class,'input-field-search')]");

        private static readonly By CollapseSearchTextbox =
            By.XPath("//div[@class='filter-toggle__arrow-icon ng-scope arrow-icon-rotate']");

        private static readonly string SelectCountryLocator =
            "//ul[@class='location__countries-list-countries']//div[contains(text(),'{0}')]";

        private static readonly string CoursesLocator = "//div[@class='location__cities']//label[contains(.,'{0}')]";

        private static readonly By TrainingListLocator = By.XPath("//a[@href='/#!/TrainingList' and @class='main-nav__item']");

        private static readonly By AboutLocator = By.XPath("//a[@href='/#!/About' and @class='main-nav__item']");

        private static readonly By BlogLocator = By.XPath("//a[@href='/#!/News' and @class='main-nav__item']");

        private static readonly By FaqLocator = By.XPath("//a[@href='/#!/FAQ' and @class='main-nav__item']");

        private static readonly string frame = "//frame[@name='{0}']";

        public static IEnumerable<string> BrowserToRunWith()
        {
            string[] browsers = { "Chrome", "Edge" };
            foreach (var b in browsers)
            {
                yield return b;
            }
        }

        public void Initialize(string browserName)
        {
            Setup(browserName);
            NavigateToUrl("https://training.by/");
            var footerBox = driver.FindElement(By.XPath("//div[@class='footer-modal' and @style='display: block;']"));
            if (footerBox.Displayed)
            {
                AcceptCookies();
            }
        }

        public void Setup(string browserName)
        {
            if (browserName.Equals("IE"))
                driver = new InternetExplorerDriver();
            else if (browserName.Equals("Chrome"))
                driver = new ChromeDriver();
            else if (browserName.Equals("Edge"))
                driver = new EdgeDriver();
            else
                driver = new FirefoxDriver();
        }

        /// <summary>
        /// Navigate to the url.
        /// </summary>
        /// <param name="url"></param>
        public void NavigateToUrl(string url)
        {
            driver.Navigate().GoToUrl(url);
            driver.Manage().Window.Maximize();
        }

        public void UploadDocument(string path , string file)
        {
            //var uploadDocumentBrowseButton = this.WaitForElementDisplayed(By.Id("file-upload"));
            //if (uploadDocumentBrowseButton.Displayed)
            //{
            //    uploadDocumentBrowseButton.Click();
            //}
            this.WaitForElementDisplayed(By.Id("file-upload")).SendKeys(path + "\\" + file);

        }

        public void ClickOnUploadButton()
        {
            this.WaitForElementDisplayed(By.Id("file-submit")).Click();
        }

        public void ClickNextWindowLink()
        {
            this.WaitForElementDisplayed(By.XPath("//a[text()='Click Here']")).Click();
        }

        public void SwitchToNewWindow()
        {
            List<String> tabs = new List<String>(driver.WindowHandles);
            driver.SwitchTo().Window(tabs[1]);
        }

        public bool SwitchingToNewWindowSuccessful()
        {
            var text = this.WaitForElementDisplayed(By.ClassName("example")).Text;
            return text.Contains("New Window");
        }
        public bool FileSuccessfullyUploaded(string message)
        {
            var text = this.WaitForElementDisplayed(By.Id("content")).Text;
            return text.Contains("File Uploaded!");
        }

        public void AcceptCookies()
        {
            this.WaitForElementDisplayed(By.XPath("//button[@class='footer-modal__action']")).Click();
        }

        public void ExpandSearchTextBox()
        {
            this.WaitForElementDisplayed(ExpandSearchTextbox).Click();
        }

        public IWebElement WaitForElementDisplayed(By elementLocator, int timeout = 40)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            try
            {
                var wait = new DefaultWait<IWebDriver>(driver);
                wait.Timeout = TimeSpan.FromSeconds(timeout);
                wait.PollingInterval = TimeSpan.FromSeconds(1);
                wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
                return wait.Until(ExpectedConditions.ElementIsVisible(elementLocator));
            }
            catch (Exception)
            {
                Console.WriteLine("Element with locator: '" + elementLocator + "' was not found."); 
                throw;
            }
        }


        public void CollapseSearchTextBox()
        {
            this.WaitForElementDisplayed(CollapseSearchTextbox).Click();
        }

        public void SelectCountry(Countries country)
        {
            this.WaitForElementDisplayed(By.XPath(String.Format(SelectCountryLocator, country))).Click();
        }

        public void ClickCourseCheckboxes(string courses)
        {
            this.WaitForElementDisplayed(By.XPath(String.Format(CoursesLocator, courses))).Click();
        }

        public bool NoTrainingAvailable()
        {
            return this.WaitForElementDisplayed(By.XPath("//span[text()='No trainings are available.']")).Displayed;
        }

        /// <summary>
        /// Click on the global icon from where language can be changed 
        /// </summary>
        public void ClickOnGlobalIcon()
        {
            this.WaitForElementDisplayed(By.ClassName("location-selector__globe")).Click();
        }

        public int GetTrainingsCount()
        {
            List<string> allElementsText = new List<string>();
            var count = driver.FindElements(By.XPath(
                "//div[@class='training-list__container training-list__desktop']//div[contains(text(),'Belarus')]"));
            foreach (var training in count)
            {
                allElementsText.Add(training.Text);
            }
            return allElementsText.Count;
        }
        /// <summary>
        /// Select language ukraine or russian or english 
        /// </summary>
        /// <param name="language"></param>
        public void SelectLanguage(string language)
        {
            ClickOnGlobalIcon();
            this.WaitForElementDisplayed(By.XPath(String.Format(LanguageSelected, language))).Click();
        }

        public void SelectResourceFile(string language)
        {
            if (language.Equals("ukrainine"))
            {
                cultureInfo = new CultureInfo("uk");
            }
            else if (language.Equals("russian"))
            {
                cultureInfo = new CultureInfo("ru");
            }
            resourceManager = new ResourceManager("Training.Resource", Assembly.GetExecutingAssembly());
        }

        /// <summary>
        ///  Header of the site contains correct text
        /// </summary>
        /// <returns>True if header text is similar in all the languages</returns>
        public void VerifyHeaderSection()
        {
            var element1 = resourceManager.GetString("TrainingList", cultureInfo).ToLower();
            var element2 = resourceManager.GetString("FAQ", cultureInfo).ToLower();
            var element3 = resourceManager.GetString("Blog", cultureInfo).ToLower();
            var element4 = resourceManager.GetString("About", cultureInfo).ToLower();

            var trainingListText = this.WaitForElementDisplayed(TrainingListLocator).Text.ToLower();
            var about = this.WaitForElementDisplayed(AboutLocator).Text.ToLower();
            var blog = this.WaitForElementDisplayed(BlogLocator).Text.ToLower();
            var faq = this.WaitForElementDisplayed(FaqLocator).Text.ToLower();

            Assert.AreEqual(trainingListText, element1);
            Assert.AreEqual(faq, element2);
            Assert.AreEqual(blog, element3);
            Assert.AreEqual(about, element4);
        }

        public void CloseTheBrowser()
        {
            driver.Close();
        }
    }
}
