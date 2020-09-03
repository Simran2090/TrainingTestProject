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
using log4net;
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
        public ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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

        protected IWebDriver driver;

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

        /// <summary>
        /// Specifying on which browsers we need to run the tests 
        /// </summary>
        public static IEnumerable<string> BrowserToRunWith()
        {
            string[] browsers = { "Chrome", "Edge" };
            foreach (var b in browsers)
            {
                yield return b;
            }
        }

        /// <summary>
        /// selects the browser , navigates to the url and accept the cookies 
        /// </summary>
        /// <param name="browserName"></param>
        public void Initialize(string browserName)
        {
            Log.Info("Set's up the browser ,Navigating to url and closing the accept cookies popup ");
            Setup(browserName);
            NavigateToUrl("https://training.by/");
            var footerBox = driver.FindElement(By.XPath("//div[@class='footer-modal' and @style='display: block;']"));
            if (footerBox.Displayed)
            {
                AcceptCookies();
            }
        }

        /// <summary>
        /// selects the browser for the run
        /// </summary>
        /// <param name="browserName"></param>
        public void Setup(string browserName)
        {
            Log.Info("Setting up the browser for the run");
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
            Log.Info("navigates to the url ");
            driver.Navigate().GoToUrl(url);
            driver.Manage().Window.Maximize();
        }

        /// <summary>
        /// Uploads the particular document to the sitee
        /// </summary>
        /// <param name="path"></param>
        /// <param name="file"></param>
        public void UploadDocument(string path , string file)
        {
            Log.Info("uploads the document to the site");
            //var uploadDocumentBrowseButton = this.WaitForElementDisplayed(By.Id("file-upload"));
            //if (uploadDocumentBrowseButton.Displayed)
            //{
            //    uploadDocumentBrowseButton.Click();
            //}
            try
            {
                this.WaitForElementDisplayed(By.Id("file-upload")).SendKeys(path + "\\" + file);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        /// <summary>
        /// clicking on the upload button 
        /// </summary>
        public void ClickOnUploadButton()
        {
            this.WaitForElementDisplayed(By.Id("file-submit")).Click();
        }

        /// <summary>
        /// Clicks on the new window link so that we can switch to next window
        /// </summary>
        public void ClickNextWindowLink()
        {
            this.WaitForElementDisplayed(By.XPath("//a[text()='Click Here']")).Click();
        }

        /// <summary>
        /// Switches to the new window 
        /// </summary>
        public void SwitchToNewWindow()
        {
            Log.Info("switching to next window");
            List<String> tabs = new List<String>(driver.WindowHandles);
            driver.SwitchTo().Window(tabs[1]);
        }

        /// <summary>
        /// verifies Switching to the new window and  the content 
        /// </summary>
        /// <returns>return true if switching happened sucessfully,false otherwise</returns>
        public bool SwitchingToNewWindowSuccessful()
        {
            try
            {
                var text = this.WaitForElementDisplayed(By.ClassName("example")).Text;
                return text.Contains("New Window");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// verifies if the file is uploaded or not 
        /// </summary>
        /// <param name="message"></param>
        /// <returns>true if uploaded and false if not </returns>
        public bool FileSuccessfullyUploaded(string message)
        {
            try
            {
                var text = this.WaitForElementDisplayed(By.Id("content")).Text;
                return text.Contains("File Uploaded!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// Accepts the cookies on the site and closes that popup 
        /// </summary>
        public void AcceptCookies()
        {
            try
            {
                this.WaitForElementDisplayed(By.XPath("//button[@class='footer-modal__action']")).Click();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// Expands the search text box where we need to select the country and cities for finding trainings
        /// </summary>
        public void ExpandSearchTextBox()
        {
            this.WaitForElementDisplayed(ExpandSearchTextbox).Click();
        }

        /// <summary>
        /// Waits for the element to display so that action can be performed on it 
        /// </summary>
        /// <param name="elementLocator"></param>
        /// <param name="timeout"></param>
        /// <returns>waits until the expected condition is followed</returns>
        public IWebElement WaitForElementDisplayed(By elementLocator, int timeout = 60)
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

        /// <summary>
        /// Closes the search text box so we can see the trainings underneath
        /// </summary>
        public void CollapseSearchTextBox()
        {
            this.WaitForElementDisplayed(CollapseSearchTextbox).Click();
        }

        /// <summary>
        /// Selects the country for which we need to find the courses
        /// </summary>
        /// <param name="country"></param>
        public void SelectCountry(Countries country)
        {
            this.WaitForElementDisplayed(By.XPath(String.Format(SelectCountryLocator, country))).Click();
        }

        /// <summary>
        /// Selecting the particular course 
        /// </summary>
        /// <param name="courses"></param>
        public void ClickCourseCheckboxes(string courses)
        {
            this.WaitForElementDisplayed(By.XPath(String.Format(CoursesLocator, courses))).Click();
        }

        /// <summary>
        /// verifies if no trainings are available message displayed
        /// </summary>
        /// <returns>true if the message displays otherwise false </returns>
        public bool NoTrainingAvailable()
        {
            try
            {
                return this.WaitForElementDisplayed(By.XPath("//span[text()='No trainings are available.']")).Displayed;
            }
            catch (Exception e)
            {
                Log.Info(e);
                throw;
            }
        }

        /// <summary>
        /// Click on the global icon from where language can be changed 
        /// </summary>
        public void ClickOnGlobalIcon()
        {
            this.WaitForElementDisplayed(By.ClassName("location-selector__globe")).Click();
        }

        /// <summary>
        /// Getting the count of the training which we find under belarus 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Selecting the particular resource file for each language
        /// </summary>
        /// <param name="language"></param>
        public void SelectResourceFile(string language)
        {
            Log.Info("Setting the resource file for different languages");
            try
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
            catch (Exception e)
            {
                Log.Info(e);
                throw;
            }
            
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
