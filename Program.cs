using System;
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

            ElementAnalyzer analyzer =
                new ElementAnalyzer(driver);

            analyzer.AnalyzePage();

            Console.WriteLine("\nPress ENTER to close browser...");
            Console.ReadLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        finally
        {
            driver.Quit();
        }
    }
}
