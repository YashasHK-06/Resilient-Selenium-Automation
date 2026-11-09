
using System;
using OpenQA.Selenium;

public class SmartLocator
{
    private readonly IWebDriver driver;
    private IWebElement searchScope;

    public SmartLocator(IWebDriver driver)
    {
        this.driver = driver;
    }

    public IWebElement FindControl(
        string fieldName,
        string expectedTag,
        string expectedType = "")
    {
        var elements = searchScope == null
            ? driver.FindElements(By.CssSelector("input, select, textarea"))
            : searchScope.FindElements(By.CssSelector("input, select, textarea"));

        IWebElement bestElement = null;
        int bestScore = 0;
        bool ambiguous = false;

        foreach (var element in elements)
        {
            if (!element.Displayed || !element.Enabled)
                continue;

            string tag = element.TagName.ToLowerInvariant();
            string type = (element.GetAttribute("type") ?? "").ToLowerInvariant();

            if (tag != expectedTag.ToLowerInvariant())
                continue;

            if (!string.IsNullOrEmpty(expectedType) &&
                type != expectedType.ToLowerInvariant())
                continue;

            string labels = GetLabels(element);
            string aria = GetAttribute(element, "aria-label");
            string placeholder = GetAttribute(element, "placeholder");
            string context = GetContext(element);

            int score = 0;

            if (ExactMatch(labels, fieldName))
                score += 100;

            if (ExactMatch(aria, fieldName))
                score += 90;

            if (ExactMatch(placeholder, fieldName))
                score += 70;

            if (Matches(context, fieldName))
                score += 30;

            if (Matches(GetAttribute(element, "name"), fieldName))
                score += 10;

            if (Matches(GetAttribute(element, "id"), fieldName))
                score += 10;

            Console.WriteLine(
                $"{fieldName} candidate: {tag}, Score: {score}"
            );

            if (score > bestScore)
            {
                bestScore = score;
                bestElement = element;
                ambiguous = false;
            }
            else if (score == bestScore && score > 0)
            {
                ambiguous = true;
            }
        }

        if (bestElement == null || bestScore < 30)
        {
            throw new NoSuchElementException(
                $"Could not reliably identify {fieldName}."
            );
        }

        if (ambiguous)
        {
            throw new InvalidOperationException(
                $"Multiple elements match {fieldName} with the same score."
            );
        }

        if (searchScope == null)
        {
            var js = (IJavaScriptExecutor)driver;

            searchScope = js.ExecuteScript(
                "return arguments[0].closest('form');",
                bestElement
            ) as IWebElement;
        }

        Console.WriteLine(
            $"Selected {fieldName}, confidence score: {bestScore}"
        );

        return bestElement;
    }

    public IWebElement FindRadioOption(
        string groupName,
        string optionName)
    {
        var radios = searchScope == null
            ? driver.FindElements(By.CssSelector("input[type='radio']"))
            : searchScope.FindElements(By.CssSelector("input[type='radio']"));

        IWebElement bestElement = null;
        int bestScore = 0;
        bool ambiguous = false;

        foreach (var radio in radios)
        {
            if (!radio.Displayed || !radio.Enabled)
                continue;

            string value = GetAttribute(radio, "value");
            string optionLabel = GetRadioLabel(radio);
            string context = GetContext(radio);

            int score = 0;

            if (ExactMatch(optionLabel, optionName))
                score += 100;

            if (ExactMatch(value, optionName))
                score += 80;

            if (Matches(context, groupName))
                score += 30;

            Console.WriteLine(
                $"{groupName}/{optionName} candidate: {value}, Score: {score}"
            );

            if (score > bestScore)
            {
                bestScore = score;
                bestElement = radio;
                ambiguous = false;
            }
            else if (score == bestScore && score > 0)
            {
                ambiguous = true;
            }
        }

        if (bestElement == null || bestScore < 30)
        {
            throw new NoSuchElementException(
                $"Could not identify {optionName} in {groupName}."
            );
        }

        if (ambiguous)
        {
            throw new InvalidOperationException(
                $"Multiple radio options match {optionName}."
            );
        }

        return bestElement;
    }

    private string GetLabels(IWebElement element)
    {
        var js = (IJavaScriptExecutor)driver;

        return js.ExecuteScript(
            "return Array.from(arguments[0].labels || [])" +
            ".map(x => x.innerText || x.textContent).join(' ');",
            element
        )?.ToString() ?? "";
    }

    private string GetRadioLabel(IWebElement radio)
    {
        var js = (IJavaScriptExecutor)driver;

        return js.ExecuteScript(
            "const id = arguments[0].id;" +
            "return Array.from(document.querySelectorAll('[for]'))" +
            ".filter(x => id && x.getAttribute('for') === id)" +
            ".map(x => x.innerText || x.textContent).join(' ');",
            radio
        )?.ToString() ?? "";
    }

    private string GetContext(IWebElement element)
    {
        var js = (IJavaScriptExecutor)driver;

        return js.ExecuteScript(
            "let e = arguments[0];" +
            "let texts = [];" +
            "for(let i = 0; i < 3 && e; i++, e = e.parentElement) {" +
            "texts.push(e.innerText || e.textContent || '');" +
            "}" +
            "return texts.join(' ');",
            element
        )?.ToString() ?? "";
    }

    private string GetAttribute(
        IWebElement element,
        string attribute)
    {
        return element.GetAttribute(attribute) ?? "";
    }

    private bool Matches(string actual, string expected)
    {
        return !string.IsNullOrWhiteSpace(actual) &&
               actual.Contains(
                   expected,
                   StringComparison.OrdinalIgnoreCase
               );
    }

    private bool ExactMatch(string actual, string expected)
    {
        return string.Equals(
            actual.Trim(),
            expected.Trim(),
            StringComparison.OrdinalIgnoreCase
        );
    }
}
