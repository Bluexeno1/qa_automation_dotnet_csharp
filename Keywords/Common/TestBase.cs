
namespace QA_Autoamtion.Keywords.Common;
public class TestBase
{
    protected IPlaywright Playwright = null!;
    protected IBrowser Browser = null!;
    protected IBrowserContext Context = null!;
    protected IPage Page = null!;

    [SetUp]
    public async Task Setup()
    {
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        Browser = await Playwright.Chromium.LaunchAsync(new() 
        {   Headless = false,
            SlowMo = 1000
        });
        Context = await Browser.NewContextAsync();
        Page = await Context.NewPageAsync();
        await Page.GotoAsync("https://portal-qa-e.hubwise.co.uk/adviser-HUBWISE/logon");
    }

    [TearDown]
    public async Task Cleanup()
    {
        await Browser.CloseAsync();
        Playwright.Dispose();
    }
}
