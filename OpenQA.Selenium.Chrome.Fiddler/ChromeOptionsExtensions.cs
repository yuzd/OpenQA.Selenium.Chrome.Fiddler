using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace OpenQA.Selenium.Chrome.Fiddler
{
    /// <summary>
    /// Extensions for ChromeOptions (Selenium Driver)
    /// 支持配置拦截指定请求(正则)
    ///		donothing
    ///		拦截后上传(http)
    /// </summary>
    public static class ChromeOptionsExtensions
    {
        private const string background_js = @"


var before_configs = {before_configs};

    chrome.webRequest.onBeforeRequest.addListener(
            function(details) {
                for (let i = 0; i<before_configs.length; i++) {
                    var config = before_configs[i];
                    if (!details.url.match(config.Match)){
                        continue;
                    }

                    if (config.Cancel)
                    {
                        return { cancel: true};
                    }
                    return { redirectUrl: config.RedirectUrl};
                }
            },
            { urls: ['<all_urls>']},
            ['blocking', 'extraHeaders', 'requestBody']
        );
";

        private const string manifest_json = @"
{
    ""version"": ""1.0.0"",
    ""manifest_version"": 2,
    ""name"": ""Chrome Fiddler"",
    ""permissions"": [
        ""proxy"",
        ""tabs"",
        ""unlimitedStorage"",
        ""storage"",
        ""<all_urls>"",
        ""webRequest"",
        ""webRequestBlocking""
    ],
    ""background"": {
        ""scripts"": [""background.js""]
	},
    ""minimum_chrome_version"":""22.0.0""
}";

        /// <summary>
        /// Add Fiddler extention
        /// </summary>
        /// <param name="options">Chrome options</param>
        /// <param name="fiddlerOption">Proxy host</param>
        public static void AddFiddler(this ChromeOptions options, FiddlerOption fiddlerOption)
        {
            var backgroundProxyJs = ReplaceTemplates(background_js, fiddlerOption);

            if (!Directory.Exists("Plugins"))
                Directory.CreateDirectory("Plugins");

            var guid = Guid.NewGuid().ToString();

            var manifestPath = $"Plugins/manifest_{guid}.json";
            var backgroundPath = $"Plugins/background_{guid}.js";
            var archiveFilePath = $"Plugins/proxy_auth_plugin_{guid}.zip";

            File.WriteAllText(manifestPath, manifest_json);
            File.WriteAllText(backgroundPath, backgroundProxyJs);

            using (var zip = ZipFile.Open(archiveFilePath, ZipArchiveMode.Create))
            {
                zip.CreateEntryFromFile(manifestPath, "manifest.json");
                zip.CreateEntryFromFile(backgroundPath, "background.js");
            }

            File.Delete(manifestPath);
            File.Delete(backgroundPath);

            options.AddExtension(archiveFilePath);
        }

        private static string ReplaceTemplates(string str, FiddlerOption fiddlerOption)
        {
            if (fiddlerOption.OnBeforeRequestOptions != null)
            {
                var beforeConfigs = Newtonsoft.Json.JsonConvert.SerializeObject(fiddlerOption.OnBeforeRequestOptions);
                str = str.Replace("{before_configs}", beforeConfigs);
            }
            return str;
        }


    }

    public sealed class FiddlerOption
    {
        public List<FiddlerOnBeforeRequestOptions>? OnBeforeRequestOptions { get; set; }
    }

    public sealed class FiddlerOnBeforeRequestOptions
    {
        public string Match { get; set; } = "";
        public bool Cancel { get; set; }
        public string RedirectUrl { get; set; } = "";
    }
}
