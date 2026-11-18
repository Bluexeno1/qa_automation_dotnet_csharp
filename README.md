<!-- Portal .Net

C:\Users\tdapayo\Documents

1. Download Portable .NET

You want .NET SDK portable (zip) — not installer.

Go to:

https://dotnet.microsoft.com/download/dotnet

Download:

.NET SDK (zip)
Example: dotnet-sdk-8.0.x-win-x64.zip

Extract to:

D:\portable-dotnet\

You should have:

D:\portable-dotnet\dotnet.exe

Test:

D:\portable-dotnet\dotnet --version




===================================================================

Check:

dotnet --version
2. Create project folder
mkdir PlaywrightTests
cd PlaywrightTests
3. Create a test project

Use xUnit (recommended):

dotnet new xunit

Add Playwright dependency:

dotnet add package Microsoft.Playwright

Install browsers:

dotnet tool install --global Microsoft.Playwright.CLI
playwright install
4. Project structure

After setup:

PlaywrightTests/
 ├── UnitTest1.cs
 ├── PlaywrightTests.csproj
 └── ...

We will replace UnitTest1.cs. -->

=====================================================================================================================
ERROR: bin\Debug\net8.0\playwright.ps1 install 'pwsh' is not recognized as an internal or external command, operable program or batch file.

That error means PowerShell 7 (pwsh) is not installed or not in PATH.
Playwright's install command is trying to run:
    pwsh bin\Debug\net8.0\playwright.ps1 install

but Windows can't find pwsh.exe.

Option 1: Install PowerShell 7 (recommended)
    Download and install PowerShell 7
    After installation, open a new terminal and check:
    pwsh --versioN
Then run:
    pwsh bin\Debug\net8.0\playwright.ps1 install

Option 2: Use Windows PowerShell
    Check if you have Windows PowerShell:
        powershell -version
    If that works, try:
        powershell -ExecutionPolicy Bypass -File bin\Debug\net8.0\playwright.ps1 install

Option 3: Install browsers through Playwright directly
    Depending on your Playwright version, this often works:
        dotnet tool restore
        dotnet build
        playwright install
    or
        dotnet build
        .\bin\Debug\net8.0\playwright.ps1 install
=====================================================================================================================
ERROR: playwright install 'playwright' is not recognized as an internal or external command, operable program or batch file.

That's expected. playwright is not a standalone command in a C# Playwright project.

1. Did you install the Playwright package?
    In your project folder, run:
        dotnet list package
    Do you see:
        Microsoft.Playwright

2. Did you build the project?
    Run:
        dotnet build
    This should generate a Playwright installation script in a path similar to:
        bin\Debug\net8.0\playwright.ps1
    or
        bin\Debug\netX.X\playwright.ps1

3. Check if the script exists
    Run:
        dir /s playwright.ps1
    If you find it, note the path.

4. Since you don't have pwsh
    Try using Windows PowerShell:
        powershell -ExecutionPolicy Bypass -File .\bin\Debug\net8.0\playwright.ps1 install
        (adjust the path if your target framework isn't net8.0)
    If that still doesn't work
        Paste the output of these commands:
            dotnet --version
            dotnet list package
            dir

=====================================================================================================================
PWERSHELL ERROR: dotnet test --filter Program dotnet: The term 'dotnet' is not recognized as a name of a cmdlet, function, script file, or executable program. Check the spelling of the name, or if a path was included, verify that the path is correct and try again.

You're now inside PowerShell, and PowerShell can't find dotnet.exe.
This isn't a Playwright problem. It's because you're using a portable .NET SDK and haven't added it to PATH in this PowerShell session.

Find your portable SDK
For example, suppose it is here:
    C:\Users\account\Documents\portable-dotnet\
        dotnet.exe
    Add it to PATH in PowerShell

Run:
    $env:PATH="C:\Users\account\Documents\portable-dotnet;$env:PATH"

Then test:
    dotnet --version

=====================================================================================================================
Difference between dotnet test and dotnet run

dotnet test

Runs unit tests (NUnit, xUnit, MSTest)
Looks for test methods like [Test] or [Fact]
Does not execute Main()

dotnet run

Runs your console application
Executes:
    static void Main(string[] args)
    {
        Console.WriteLine("Hello World!");
    }

=====================================================================================================================
Running scripts for QA automation

When dotnet test shows only:
    Restore complete
    Build succeeded
and nothing like:
    Test Run Successful
    Passed: 1
it means zero tests were found.

🔥 Why this is happening
In your code:

    using NUnit.Framework;

    public class LoginTests
    {
        [Test]
        public void Login_ShouldWork()
        {
            Console.WriteLine("Running login test");
        }
    }

You are missing test project configuration OR test SDKs.

✅ Fix checklist (do these in order)
✔ 1. Make sure this is a TEST project
    Open your .csproj file.

It MUST contain:

    <Project Sdk="Microsoft.NET.Sdk">

    <PropertyGroup>
        <TargetFramework>net8.0</TargetFramework>
        <IsTestProject>true</IsTestProject>
    </PropertyGroup>

    <ItemGroup>
        <PackageReference Include="NUnit" Version="3.14.0" />
        <PackageReference Include="NUnit3TestAdapter" Version="4.5.0" />
        <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
    </ItemGroup>

    </Project>

✔ 2. Install missing packages (IMPORTANT)
    Run:
        dotnet add package NUnit
        dotnet add package NUnit3TestAdapter
        dotnet add package Microsoft.NET.Test.Sdk

✔ 3. Rebuild
    dotnet clean
    dotnet build
✔ 4. Run tests WITHOUT filter first
    dotnet test

You should see:

Passed: 1
✔ 5. Then filtering will work
    dotnet test --filter Login_ShouldWork

🧠 Most common cause in your case
    You are almost certainly missing:

        Microsoft.NET.Test.Sdk

👉 Without it, NUnit tests are NOT discovered.

💡 Quick mental model
Component	                    Purpose
--------------------------------------------------
[Test]	                        marks test
NUnit	                        test framework
NUnit adapter	                connects NUnit to dotnet
Microsoft.NET.Test.Sdk	        enables test discovery

=====================================================================================================================
Which project to use for QA Automation?

For QA automation (especially Playwright in C#) you should use:

✅ Test Project (NOT Console App)
👉 You use:
    [Test] (NUnit) or [Fact] (xUnit)
    dotnet test
    No Main()

🧠 Why test projects are used in QA
QA automation needs:
    multiple test cases
    setup/teardown (open/close browser)
    running only failed tests
    filtering tests
    CI/CD (GitHub Actions, Jenkins, etc.)

All of that is built into test frameworks, not Main().

🔥 Correct QA structure (Playwright + NUnit)
    AutomationProject/
    ├─ Tests/
    │   ├─ LoginTests.cs
    │   ├─ CartTests.cs
    ├─ Pages/
    │   ├─ LoginPage.cs
    ├─ Utilities/

✅ Example test (this is REAL QA style)
    using NUnit.Framework;
    using Microsoft.Playwright;
    using System.Threading.Tasks;

public class LoginTests
{
    [Test]
    public async Task Login_ShouldWork()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new() { Headless = false });

        var page = await browser.NewPageAsync();
        await page.GotoAsync("https://google.com");

        Console.WriteLine("Browser opened successfully");
    }
}

Run it:
    dotnet test

❌ What QA automation does NOT use
❌ Main()
❌ dotnet run
❌ Console apps
❌ single-file execution

🧠 Simple rule
Purpose	                                    Use
Learning basic C#	                        Console App (Main)
QA Automation / Playwright	                Test Project ([Test])
CI/CD pipelines	                            Test Project

=====================================================================================================================
ERRROR: pwsh bin/Debug/netX/playwright.ps1 install

That error is happening because you used a placeholder path:

    bin/Debug/netX/playwright.ps1

👉 netX is NOT a real folder, so PowerShell cannot find the script.

✅ Step 1: Find your real folder

    From what you showed earlier, your real output folder is like:

        bin\Debug\net10.0\

    So your script is actually here:

        bin\Debug\net10.0\playwright.ps1

✅ Step 2: Run the correct command

    Go to your project folder first:

        cd C:\Users\account\Documents\Automation

    Then run:

        pwsh .\bin\Debug\net10.0\playwright.ps1 install
    ⚠️ IMPORTANT (your real issue)


🧠 Simple explanation
Command	                                    Meaning
pwsh	                                    PowerShell 7
powershell	                                Windows PowerShell
playwright.ps1	                            Playwright browser installer script


If you're using a portable .NET SDK (not installed system-wide), then every time you open a new terminal you need to make sure the dotnet executable is available.



Option 2: Add the portable SDK to PATH for the current terminal session

In Command Prompt:

	set PATH=C:\Tools\dotnet;%PATH%

In PowerShell:

	$env:PATH="C:\Tools\dotnet;$env:PATH"

Then you can use:

	dotnet build
	dotnet run

for the rest of that terminal session.


For a Playwright project

Typically the workflow is:

	dotnet restore
	dotnet build
	dotnet run

And if Playwright browsers haven't been installed yet:

	playwright install

or

	dotnet build
	pwsh bin/Debug/net8.0/playwright.ps1 install

depending on how the project was set up.
One thing to be aware of, using a portable SDK works, but it can be inconvenient because:
	- VS Code's C# extensions may not automatically find it.
	- Every new terminal needs PATH configured.
	- Some tools expect dotnet to be installed normally.

=====================================================================================================================
So i have a portable dotnet folder so now i have to create a qa automation folder then run dotnet commands there?


Example structure
D:\
├─ dotnet-portable\
│  ├─ dotnet.exe
│  └─ ...
└─ QA-Automation\
Step 1: Open a terminal

Either:

Add your portable SDK folder to PATH for that session, or
Use the full path to dotnet.exe.

Example:

	set PATH=D:\dotnet-portable;%PATH%

	I.E:	set PATH=C:\Users\tdapayo\Documents\portable-dotnet;%PATH%

Step 2: Create your automation project

Navigate to your QA folder:

	cd D:\QA-Automation

Create a new Playwright project:

	dotnet new console

or create a dedicated folder first:

	mkdir PlaywrightTests
	cd PlaywrightTests
	dotnet new console
Step 3: Add Playwright
	dotnet add package Microsoft.Playwright
Step 4: Build the project
	dotnet build
Step 5: Install browsers
	pwsh bin\Debug\netXXX\playwright.ps1 install
	I.E:	pwsh bin\Debug\net10.0\playwright.ps1 install

(or the equivalent path for your target framework)

Step 6: Run tests
	dotnet run

Future workflow, Once the project exists, you do not create it again.
Each time you want to work on it:
	set PATH=D:\dotnet-portable;%PATH%
	cd D:\QA-Automation\PlaywrightTests
	dotnet build
	dotnet run

or if you're using Playwright Test/NUnit:

	dotnet test

A convenient setup is:

D:\
├─ dotnet-portable\
├─ QA-Automation\
│  ├─ PlaywrightTests\
│  ├─ APIAutomation\
│  └─ Utilities\

Then all your automation projects can share the same portable .NET SDK.

==================================================================================================================
Almost. You should append the existing PATH, not replace it.

Use:

	set PATH=C:\Users\account\Documents\portable-dotnet;%PATH%

Then test:

	dotnet --version

If it works, you'll see something like:

	8.0.xxx

If it says:

	'dotnet' is not recognized...

then check whether dotnet.exe is actually in:

	C:\Users\account\Documents\portable-dotnet\

You should be able to see:

C:\Users\account\Documents\portable-dotnet\
    dotnet.exe
    host\
    sdk\
    shared\

If dotnet.exe is inside a subfolder, use that folder instead.

For example, if the file is here:

	C:\Users\account\Documents\portable-dotnet\dotnet\
    dotnet.exe

then use:

	CMD:        set PATH=C:\Users\account\Documents\portable-dotnet\dotnet;%PATH%
    pwsh:       $env:PATH="C:\Users\tdapayo\Documents\portable-dotnet;$env:PATH"
After that:

	dotnet --version
	cd D:\QA-Automation\PlaywrightTests
	dotnet build
	dotnet run
	set PATH=D:\dotnet-portable;%PATH%

==================================================================================================================
QUESTION: dotnet new console dotnetc new nunitc dotnet new mstest so for qa automation, which one?

For QA automation with Playwright in C#, you should use:

✅ Best choice: dotnet new nunit
🏆 Why NUnit is the best for QA automation

NUnit is widely used because it supports:
    ✔ Test attributes ([Test], [SetUp], [TearDown])
    ✔ Easy test filtering (dotnet test --filter)
    ✔ Parallel test execution
    ✔ Great Playwright integration
    ✔ Simple and clean structure

🥇 Recommended setup (your case)
    dotnet new nunit -n AutomationTests
    cd AutomationTests
    dotnet add package Microsoft.Playwright
    dotnet add package NUnit3TestAdapter
    dotnet add package Microsoft.NET.Test.Sdk
    🚀 Why NOT the others?

❌ dotnet new console
    Uses Main()
    Not designed for test execution
    No test runner
    Not scalable for QA

👉 Only good for learning basic C#

⚠️ dotnet new mstest
    Works fine
    But less popular in Playwright community
    Slightly more verbose

🥇 dotnet new nunit (BEST)
    Most common in QA automation
    Clean syntax

AutomationTests/
 ├─ Tests/
 │   ├─ LoginTests.cs
 ├─ Pages/
 ├─ Helpers/

==================================================================================================================
Difference between NUnit and XUnit

Both NUnit and xUnit are test frameworks for C#, and both work with Playwright. The difference is mostly in style, design philosophy, and features, not capability.

🥊 NUnit vs xUnit (simple comparison)
    🟦 NUnit (more traditional, feature-rich)
        ✔ Pros
            Very mature (older, widely used in QA)
            Lots of built-in features:
            [SetUp], [TearDown]
            [TestCase] (parameterized tests)
            Easy for beginners
            Very popular in enterprise QA teams
        ❌ Cons
            Slightly more “heavy”
            More attributes = more boilerplate

🧪 Example NUnit
[TestFixture]
public class LoginTests
{
    [SetUp]
    public void Setup()
    {
        Console.WriteLine("Before each test");
    }

    [Test]
    public void Login_ShouldWork()
    {
        Console.WriteLine("Test running");
    }

    [TearDown]
    public void Cleanup()
    {
        Console.WriteLine("After each test");
    }
}

🟣 xUnit (modern, clean, minimal)
    ✔ Pros
        Cleaner, simpler design
        Used heavily in modern .NET projects
        No need for [TestFixture]
        Better parallel execution by default
        Encourages better architecture
    ❌ Cons
        No [SetUp] / [TearDown] (uses constructor + IDisposable instead)
        Slight learning curve if coming from NUnit

🧪 Example xUnit
using Xunit;

public class LoginTests : IDisposable
{
    public LoginTests()
    {
        Console.WriteLine("Setup (constructor)");
    }

    [Fact]
    public void Login_ShouldWork()
    {
        Console.WriteLine("Test running");
    }

    public void Dispose()
    {
        Console.WriteLine("Cleanup");
    }
}

⚔️ Key differences (important)
Feature	                                    NUnit	                                        xUnit
Setup/Teardown	                            [SetUp] / [TearDown]	                        Constructor / Dispose
Test attribute	                            [Test]	                                        [Fact]
Parameterized tests	                        [TestCase]	                                    [Theory]
Complexity	                                Slightly more	                                Simpler
Industry usage	                            Very common in QA	                            More modern dev teams
Playwright support	                        Excellent	                                    Excellent

==================================================================================================================
ERROR: Global using error

Add a GlobalUsings.cs like this:
    global using System;
    global using System.Threading.Tasks;
    global using Microsoft.Playwright;

Create a file at your project root:
    GlobalUsings.cs

Put this inside:
    global using System;
    global using System.Threading.Tasks;
    global using Microsoft.Playwright;

🧠 What this does
    It automatically applies these using statements to ALL files in your project, so you don’t need to repeat them everywhere.

⚠️ Important notes
✔ Works only if:
    You are using .NET 6 / 7 / 8 / 9+
    Your project is SDK-style (<Project Sdk="Microsoft.NET.Sdk">)
❌ It will NOT fix everything if:
    Your .csproj is wrong
    You’re missing NuGet packages
    You still have broken async code elsewhere
🚀 Recommended GlobalUsings for QA + Playwright
    You can expand it like this (better for automation projects):
        global using System;
        global using System.Threading.Tasks;
        global using Microsoft.Playwright;
        global using NUnit.Framework;

💡 Why this is useful
        Without it, you would need this in every file:

            using System;
            using System.Threading.Tasks;
            using Microsoft.Playwright;
            using NUnit.Framework;

With GlobalUsings.cs → cleaner project, less errors.

==================================================================================================================
KNOWLEDGE: Playwright Commands

1. Are Playwright “commands” C# or Playwright?
They are Playwright API methods exposed in C#.
So:
    Playwright is the automation engine
    C# is just the language wrapper
    Example:
        await page.ClickAsync("#login");

This is:
    ✔ C# method call
    ✔ Playwright command under the hood

So you’re writing Playwright commands using C# syntax.

⚙️ 2. Core Playwright “commands” you MUST know
    Think of these as the real automation toolkit.

🌍 Navigation
    await page.GotoAsync("https://example.com");
    await page.ReloadAsync();
    await page.GoBackAsync();
    await page.GoForwardAsync();
🖱️ Clicking & interacting
    await page.ClickAsync("button");
    await page.DblClickAsync("button");
    await page.HoverAsync("div");
    await page.CheckAsync("#checkbox");
    await page.UncheckAsync("#checkbox");
⌨️ Typing / input
    await page.FillAsync("#username", "admin");
    await page.TypeAsync("#username", "admin"); // slower, character-by-character
    await page.PressAsync("#input", "Enter");
🔍 Locators (VERY important)
Modern Playwright style:
    var loginButton = page.Locator("#login");
    await loginButton.ClickAsync();
    await loginButton.FillAsync("text");
⏳ Waiting (critical for stability)
    await page.WaitForTimeoutAsync(2000); // fixed wait (avoid when possible)
    await page.WaitForSelectorAsync("#login");
    await page.Locator("#login").WaitForAsync();
    await page.WaitForURLAsync("**/dashboard");
📦 Assertions (Playwright Test style, optional in C#)
    If using NUnit, you typically use:
        Assert.That(await page.TitleAsync(), Does.Contain("Google"));
        Or:
        await Expect(page.Locator("#login")).ToBeVisibleAsync();
📸 Debug tools (VERY useful)
    await page.ScreenshotAsync(new()
    {
        Path = "screenshot.png",
        FullPage = true
    });
🧭 Frames / popups
    var frame = page.FrameLocator("iframe");
    await frame.Locator("#btn").ClickAsync();

    Popup:
    var popup = await page.RunAndWaitForPopupAsync(async () =>
    {
        await page.ClickAsync("#open");
    });
🧪 Context (VERY important concept)
    var context = await browser.NewContextAsync();
    var page = await context.NewPageAsync();

    Context = isolated session (like incognito tab group)

📡 Network / API interception
    await page.RouteAsync("**/*", route =>
    {
        route.ContinueAsync();
    });
🧾 Getting data
    var text = await page.InnerTextAsync("#title");
    var value = await page.GetAttributeAsync("#input", "value");
    var url = page.Url;
    var title = await page.TitleAsync();

==================================================================================================================
KNOWLEDGE: Playwright Commands

🌍 Navigation (moving around websites)
    GotoAsync
        await page.GotoAsync("https://google.com");

    ➡ Opens a website (like typing URL + Enter)

🖱️ Interaction (user actions)
    ClickAsync
        await page.ClickAsync("#login");

    ➡ Clicks a button or element (like a mouse click)

    FillAsync
        await page.FillAsync("#username", "admin");

    ➡ Types into an input box (like typing on keyboard)

⏳ Waiting (important for timing)
    WaitForSelectorAsync
        await page.WaitForSelectorAsync("#dashboard");

    ➡ Waits until an element appears on the page

    WaitForURLAsync
        await page.WaitForURLAsync("**/dashboard");

    ➡ Waits until the page changes to a URL

🐞 Debug tools (for testing)
    ScreenshotAsync
        await page.ScreenshotAsync(new() { Path = "test.png" });

    ➡ Takes a picture of the browser screen

    PauseAsync
        await page.PauseAsync();

    ➡ Freezes the test and opens debugging tool

🧱 Structure (browser setup)
    NewContextAsync
        var context = await browser.NewContextAsync();

    ➡ Creates a fresh “session” (like incognito mode)

    NewPageAsync
        var page = await context.NewPageAsync();

    ➡ Opens a new browser tab


==================================================================================================================
KNOWLEDGE: Dynamic locators

📁 Folder structure
Locators/
    LoginLocators.cs
Tests/
Pages/ (optional or skip)

📄 LoginLocators.cs (simple string-based locators)
    namespace Locators;

    public static class LoginLocators
    {
        public const string LoginField = "#login";
        public const string Username = "#username";
        public const string Password = "#password";
        public const string SubmitButton = "#submit";
    }

🧪 Use directly in tests
    using Locators;

    [Test]
    public async Task LoginTest()
    {
        await page.GotoAsync("https://example.com");

        await page.FillAsync(LoginLocators.Username, "admin");
        await page.FillAsync(LoginLocators.Password, "123");
        await page.ClickAsync(LoginLocators.SubmitButton);
    }

⚡ Why this is good
    ✔ Very simple
    ✔ No classes or boilerplate
    ✔ Easy to maintain
    ✔ Central place for selectors
    ✔ Beginner-friendly
    ✔ Fast to write tests

🧠 Even cleaner upgrade (slightly better)
You can group by page:

    public static class LoginPageLocators
    {
        public const string Username = "#username";
        public const string Password = "#password";
        public const string LoginButton = "#login";
    }

Instead of putting all locators in one flat list like this:
    public static class Locators
    {
        public const string Username = "#username";
        public const string CartButton = "#cart";
        public const string Search = "#search";
    }

You organize them by which page they belong to.

📄 Example (your login page only)
    public static class LoginPageLocators
    {
        public const string Username = "#username";
        public const string Password = "#password";
        public const string LoginButton = "#login";
    }

👉 Meaning:
These locators ONLY belong to the Login Page.

🧠 Why this matters
Think of a website like this:
Page	                                    Elements
Login Page	                                username, password, login button
Home Page	                                search bar, menu, cart
Product Page	                            add to cart, price, images

✅ Good (grouped by page)
public static class LoginPageLocators
    {
        public const string Username = "#username";
        public const string Password = "#password";
        public const string LoginButton = "#login";
    }

    public static class HomePageLocators
    {
        public const string SearchBar = "#search";
        public const string CartButton = "#cart";
    }
    
🧪 How you use it
    await page.FillAsync(LoginPageLocators.Username, "admin");
    await page.FillAsync(LoginPageLocators.Password, "123");
    await page.ClickAsync(LoginPageLocators.LoginButton);