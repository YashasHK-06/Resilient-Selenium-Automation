
using System;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

class Program
{
    static void Main(string[] args)
    {
        string url = args.Length > 0
            ? args[0]
            : "https://app.cloudqa.io/home/AutomationPracticeForm";

        using IWebDriver driver = new ChromeDriver();

        try
        {
            Console.WriteLine("Opening webpage...");
            driver.Navigate().GoToUrl(url);

            WebDriverWait wait = new WebDriverWait(
                driver,
                TimeSpan.FromSeconds(15)
            );

            wait.Until(d =>
                ((IJavaScriptExecutor)d)
                    .ExecuteScript("return document.readyState")
                    ?.ToString() == "complete"
            );

            Console.WriteLine("Page loaded successfully!");

            SmartLocator locator = new SmartLocator(driver);

            Console.WriteLine("\nFinding First Name...");
            IWebElement firstName = locator.FindControl(
                "First Name",
                "input",
                "text"
            );

            firstName.Clear();
            firstName.SendKeys("Yashas");

            Console.WriteLine("First Name entered.");

            Console.WriteLine("\nFinding Gender...");
            IWebElement maleOption = locator.FindRadioOption(
                "Gender",
                "Male"
            );

            maleOption.Click();

            Console.WriteLine("Gender selected.");

            Console.WriteLine("\nFinding State...");
            IWebElement stateDropdown = locator.FindControl(
                "State",
                "select"
            );

            SelectElement select = new SelectElement(stateDropdown);

            select.SelectByText("India");

            Console.WriteLine("India selected.");

            Console.WriteLine("\n========== VERIFICATION ==========");

            Console.WriteLine(
                "First Name: " +
                (firstName.GetAttribute("value") == "Yashas" ? "PASS" : "FAIL")
            );

            Console.WriteLine(
                "Gender: " +
                (maleOption.Selected ? "PASS" : "FAIL")
            );

            Console.WriteLine(
                "State: " +
                (select.SelectedOption.Text.Trim().Equals(
                    "India",
                    StringComparison.OrdinalIgnoreCase
                ) ? "PASS" : "FAIL")
            );

            Console.WriteLine("\nPress ENTER to close the browser.");
            Console.ReadLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine("\nAutomation failed: " + ex.Message);
            Console.WriteLine("Press ENTER to close the browser.");
            Console.ReadLine();
        }
        finally
        {
            driver.Quit();
        }
    }
}
