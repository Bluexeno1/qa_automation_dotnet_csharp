public class ValidLoginTests : TestBase
{
    [Test]
    public async Task Invalid_Login()
    {
        var login = new LoginPage(Page);

        await login.Login(
            LoginTestData.InvalidUsername,
            LoginTestData.InvalidPassword
        );
    }
}