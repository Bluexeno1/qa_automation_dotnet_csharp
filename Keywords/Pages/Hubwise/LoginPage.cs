
namespace QA_Automation.Keywords.Pages.Hubwise;
public class LoginPage : BasePage
{
    public LoginPage(IPage page) : base(page) {}

    public async Task Login(string username, string password)
    {
        await Page.FillAsync(LoginPageLocators.UsernameInput, username);
        await Page.FillAsync(LoginPageLocators.PasswordInput, password);
        await Page.ClickAsync(LoginPageLocators.LoginButton);
    }
}