namespace QA_Automation.Keywords.Features.Hubwise;

public class LoginFeature
{
    private readonly LoginPage _login;

    public LoginFeature(IPage page)
    {
        _login = new(page);
    }

    public async Task LoginAs(string username, string password)
    {
        await _login.Login(username, password);
    }
}