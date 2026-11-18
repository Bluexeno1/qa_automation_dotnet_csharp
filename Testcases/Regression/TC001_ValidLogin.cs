public class ValidLoginTests : TestBase
{
    [Test]
    public async Task Valid_Login()
    {
        var login = new LoginPage(Page);

        await login.Login(
            LoginTestData.ValidUsername,
            LoginTestData.ValidPassword
        );
    }
}