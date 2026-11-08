using System;
using OpenQA.Selenium;

public class ElementAnalyzer
{
    private readonly IWebDriver driver;

    public ElementAnalyzer(IWebDriver driver)
    {
        this.driver = driver;
    }

    public void AnalyzePage()
    {
        var elements = driver.FindElements(
            By.CssSelector("input, select, textarea")
        );

        Console.WriteLine("\n========== PAGE ANALYSIS ==========");
        Console.WriteLine("Total elements found: " + elements.Count);

        int count = 1;

        foreach (var element in elements)
        {
            string tag = element.TagName;
            string type = element.GetAttribute("type") ?? "";
            string id = element.GetAttribute("id") ?? "";
            string name = element.GetAttribute("name") ?? "";
            string placeholder =
                element.GetAttribute("placeholder") ?? "";

            var js = (IJavaScriptExecutor)driver;

            string labels = js.ExecuteScript(
                "return Array.from(arguments[0].labels || [])" +
                ".map(label => label.innerText || label.textContent)" +
                ".join(' | ');",
                element
            )?.ToString() ?? "";

            string ariaLabel =
                element.GetAttribute("aria-label") ?? "";

            Console.WriteLine($"\nElement {count++}");
            Console.WriteLine("Tag         : " + tag);
            Console.WriteLine("Type        : " + type);
            Console.WriteLine("ID          : " + id);
            Console.WriteLine("Name        : " + name);
            Console.WriteLine("Placeholder : " + placeholder);
            Console.WriteLine("Label       : " + labels);
            Console.WriteLine("ARIA Label  : " + ariaLabel);
        }

        Console.WriteLine("\n========== ANALYSIS COMPLETE ==========");
    }
}
