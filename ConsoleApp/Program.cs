using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Chrome.ChromeDriverExtensions;
using OpenQA.Selenium.Chrome.Fiddler;
using RestSharp;

var driverBinary = @"D:\soft\chrome\chrome2\Chrome-bin\";

ChromeOptions options = new ChromeOptions
{
    BinaryLocation = Path.Combine(driverBinary, "chrome.exe")
};

Environment.SetEnvironmentVariable("webdriver.chrome.driver", driverBinary);
options.AddArgument("--disable-blink-features=AutomationControlled");
options.AddArguments("--disable-infobars");
List<string> ls = new List<string> { "enable-automation" };
options.AddExcludedArguments(ls);

#region 代理

// var pp = GetProxy().Split(":");
// options.AddHttpProxy(pp[0], int.Parse(pp[1]), "", "");

#endregion

#region Fillder

options.AddFiddler(new FiddlerOption
{
    OnBeforeRequestOptions = new List<FiddlerOnBeforeRequestOptions>
    {
        new FiddlerOnBeforeRequestOptions
        {
            Match = "https://www.cnblogs.com/yudongdong/ajax/GetPostStat",
            RedirectUrl = "http://localhost:5000/GetPostStat",
            Cancel = false
        }
    }
});

#endregion

var chrome = new ChromeDriver(driverBinary, options);


#region method

string GetProxy()
{
    var client = new RestClient("http://webapi.http.zhimacangku.com/getip?num=1&type=1&pro=&city=0&yys=0&port=1&time=1&ts=0&ys=0&cs=0&lb=1&sb=0&pb=4&mr=1&regions=");
    var request = new RestRequest();
    var response = client.Execute(request);
    return response.Content;
}

#endregion
