# OpenQA.Selenium.Chrome.Fiddler
Extensions for ChromeOptions (Selenium Driver)支持配置拦截或转发指定请求(正则)

# nuget
OpenQA.Selenium.Chrome.Fiddler

# how to use

```csharp

options.AddFiddler(new FiddlerOption
{
    OnBeforeRequestOptions = new List<FiddlerOnBeforeRequestOptions>
    {
        // 配置转发
        new FiddlerOnBeforeRequestOptions
        {
            Match = "https://www.cnblogs.com/yudongdong/ajax/GetPostStat",//正则
            RedirectUrl = "http://localhost:5000/GetPostStat",//如果匹配成功则将requestBody转发到这个url中去
            Cancel = false//如果配置了cancel=true那么转发将无效，true的意思是直接拦截这次的请求,不去发送了
        },
        // 配置拦截
        new FiddlerOnBeforeRequestOptions
        {
            Match = "https://www.cnblogs.com/yudongdong/ajax/blogStats",
            Cancel = true//true的意思是直接拦截这次的请求,不去发送了
        },
    }
});


```
