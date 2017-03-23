using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ScanBarcodeConsoleApp
{
	class Program
	{
		string baseUrl = "https://www.amazon.co.jp/s/ref=nb_sb_noss?__mk_ja_JP=%E3%82%AB%E3%82%BF%E3%82%AB%E3%83%8A&url=search-alias%3Daps&field-keywords=%barcode%&rh=i%3Aaps%2Ck%3A%barcode%";

		static void Main(string[] args)
		{
			
			var s = Environment.TickCount;
			//var chrome = new ChromeDriver();
			var chrome = new EdgeDriver();
			var barcode = "4562356928620";
			if (args.Length > 0) {
				barcode = args[0];
			}
			//chrome.Manage().Window.Position
			var item = new Program().ScanBarcode(chrome, barcode);

			if (item != null)
				Console.Out.WriteLine($"ItemName={item.Name}, Maker={item.Maker}, Prise(目安)={item.EstimatedPrice}");
			else
				Console.Out.WriteLine($"該当データが見つかりません。barcode={barcode}");
			//chrome 6625ms, 画像取り込みを入れると9500ms
			//edge  6688ms, 画像取り込みを入れると6547ms
			Debug.WriteLine($"処理時間={Environment.TickCount - s}ms");

		}

		private Item ScanBarcode(RemoteWebDriver webDriver, string barcode)
		{
			Debug.WriteLine($"BrowerName={webDriver.Capabilities.BrowserName}");
			Debug.WriteLine($"BrowerVersion={webDriver.Capabilities.Version}");

			var url = baseUrl.Replace("%barcode%", barcode);
			webDriver.Url = url;
			//webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(30));
			webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(8);

			try
			{
				var result = webDriver.FindElementById("result_0");

				//var img = webDriver.FindElementByTagName("img");
				//var imgsrc = img.GetAttribute("src");

				var imgurl = result.FindElement(By.TagName("img")).GetAttribute("src");



				//Debug.Write(result.Text);
				var r = result.Text.Split(new char[] { '\r', '\n' });

				var item = new Item();
				item.Name = r[4];
				item.Maker = r[6];
				item.EstimatedPrice = GetPrice(r[8]);
				item.Image = GetImage(imgurl, barcode);
				return item;
			}
			catch (Exception e) when (e is InvalidOperationException || e is NoSuchElementException) {
				//タグ無しはデータなし
				return null;
			}
			finally
			{
				webDriver.Quit();
			}
		}

		private string GetPrice(string v)
		{
			var reg = new Regex("[0-9]");
			var r = v.Split('+')[0];
			return reg.IsMatch(r) ? r : "参考価格なし";
		}

		private Image GetImage(string imgurl, string barcode)
		{
			//var file = Path.GetTempFileName() + ".jpg";
			var file = barcode + ".jpg";
			using (var client = new WebClient()) { 
				client.DownloadFile(imgurl, file);
			}
			var image = Image.FromFile(file);
			//File.Delete(file);
			return image;
		}
	}

	class Item
	{
		public string Barcode { get; set; }

		public string Name { get; set; }

		public Decimal Price{ get; set; }

		public string EstimatedPrice { get; set; }

		public string Maker { get; set; }

		public Image Image { get; set; }
	}

	
}
