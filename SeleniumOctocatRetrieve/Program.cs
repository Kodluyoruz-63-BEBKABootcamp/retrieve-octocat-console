using System;
using System.Diagnostics;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace SeleniumOctocatRetrieve
{
    class Program
    {
        #region Cropped Image Information
        const int LOCATION_X = 450; //multiple caret using
        const int LOCATION_Y = 130;
        const int WIDTH = 825;
        const int HEIGHT = 900;
        #endregion

        static void Main(string[] args)
        {
            #region Configuration
            const int SAMPLE_COUNT = 20;
            const bool FULLSCREEN = false;
            #endregion

            #region Main
            Stopwatch watch = new Stopwatch();
            watch.Start();
            Console.WriteLine("Project Started");

            var driver = InitializeAndNavigate();

            for (int i = 0; i < SAMPLE_COUNT; i++)
            {
                Randomize(driver);
                FixOctocat(driver);

                Guid guid = Guid.NewGuid();
                var screenShot = driver.GetScreenshot();

                if (FULLSCREEN)
                {
                    SaveFullScreenShot(guid, screenShot);
                }
                SaveOctocat(guid, screenShot);
            }

            driver.Quit();
            watch.Stop();
            Console.WriteLine(string.Format("Elapsed time for {0} images : {1} seconds",SAMPLE_COUNT,watch.Elapsed.TotalSeconds.ToString()));
            #endregion
        }

        #region Methods

        private static void CreateFolderIfNotExists()
        {
            var path = Directory.GetCurrentDirectory() + "//scr";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private static void SaveOctocat(Guid guid, Screenshot screenshot)
        {
            CreateFolderIfNotExists();

            var image = Image.Load<Rgba32>(screenshot.AsByteArray);
            var rectangle = new Rectangle(LOCATION_X, LOCATION_Y, WIDTH, HEIGHT); //magic number
            image.Mutate(x => x.Crop(rectangle));
            image.SaveAsPng(string.Format("scr//octocat-cropped-{0}.png", guid));
        }

        private static void SaveFullScreenShot(Guid guid, Screenshot screenshot)
        {
            CreateFolderIfNotExists();

            screenshot.SaveAsFile(string.Format("scr//octocat-full-{0}.png", guid), ScreenshotImageFormat.Png);
        }

        private static void FixOctocat(ChromeDriver driver)
        {
            IWebElement element = driver.FindElementById("octocat");
            Console.WriteLine("Octocat found.");

            Actions actions = new Actions(driver);
            actions.MoveToElement(element).Build().Perform();
        }

        private static void Randomize(ChromeDriver driver)
        {
            driver.FindElementById("radomize").Click();
            Console.WriteLine("Randomize clicked");
        }

        private static ChromeDriver InitializeAndNavigate()
        {
            var options = new ChromeOptions();
            options.AddArgument("headless");
            ChromeDriver driver = new ChromeDriver(options);
            driver.Url = "https://myoctocat.com/build-your-octocat/";
            Console.WriteLine("Chrome Driver initialized.");

            driver.Navigate();
            Console.WriteLine("Navigating to site.");

            return driver;
        }
        #endregion
    }
}
