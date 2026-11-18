namespace QA_Automation.Resources.Locators.Hubwise;

public static class HomePageLocators
{
    public static class Home {
    public const string UsernameInput = "//input[@id='appun']";
    public const string PasswordInput = "//input[@id='apppw']";
    public const string LoginButton = "//*[@id='l-login']";
    }
    public static class InvestorsMenu
    {
        public const string MenuButton = "#react-burger-menu-btn";
        public const string AllItemsOption = "#inventory_sidebar_link";
        public const string LogoutOption = "#logout_sidebar_link";
        public const string ResetAppStateOption = "#reset_sidebar_link";
    }
}


//*[@id="a-menu"]//div[@tooltip="Main Menu"]
//*[@id="main-menu"]//li[@data-link="views/accountHolders/HoldersDash/HoldersDash"]
//*[@id="main-menu"]//li[@data-link="views/accountHolders/Holder/HolderEdit/HolderEdit?action=webAccess&autoOpen=true"]
//*[@id="main-menu"]//li[@data-link="views/accountHolders/Holder/HolderCreate/HolderCreate"]
//*[@id="main-menu"]//li[@data-link="views/accountHolders/HoldersAccounts/HoldersAccounts"]




//*[@id="a-menu"]//div[@tooltip="Admin Menu"]
//Investor Account Dashboard
//(//div[@id="pickerView"]//input[@class="st-filter-input-element st-filter-selected-value"])[1]
/// (//div[@id="pickerView"]//div[@class="st-filter-button"])[1]
/// //div[@class="st-filter-selected-field chosen"]
/// //div[@data-field-id="person.searchText"]
/// //div[@data-field-id="UPPER:account.accountName"]
/// //div[@data-field-id="account.accountId"]
/// //div[@data-field-id="person.postCode"]
/// //div[@data-field-id="account.productId"]
/// (//div[@tooltip="View Investor Overview"])[1]
/// (//div[@tooltip="View Account Details"])[1]
/// Wait until Overview dashboard is present //div[@class="v-column-heading"]
/// 
/// 
/// 
/// 
//*[@id="a-menu"]//div[@tooltip="Logout Menu"]