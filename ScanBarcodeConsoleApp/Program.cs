using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanBarcodeConsoleApp
{
	class Program
	{
		string baseUrl = "https://www.amazon.co.jp/s/ref=nb_sb_noss?__mk_ja_JP=%E3%82%AB%E3%82%BF%E3%82%AB%E3%83%8A&url=search-alias%3Daps&field-keywords=%barcode%&rh=i%3Aaps%2Ck%3A%barcode%";

		static void Main(string[] args)
		{

			var chrome = new ChromeDriver();

			var barcode = "4562356928620";
			var item = new Program().ScanBarcode(chrome, barcode);

			Debug.WriteLine($"ItemName={item.Name}, Maker={item.Maker}, Prise(目安)={item.EstimatedPrice}");
		}

		private Item ScanBarcode(RemoteWebDriver webDriver, string barcode)
		{
			Debug.WriteLine($"BrowerName={webDriver.Capabilities.BrowserName}");
			Debug.WriteLine($"BrowerVersion={webDriver.Capabilities.Version}");

			var url = baseUrl.Replace("%barcode%", barcode);
			webDriver.Url = url;
			//webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(30));
			webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);

			var result = webDriver.FindElementById("result_0");

			//Debug.Write(result.Text);
			var r = result.Text.Split(new char[]{'\r','\n'});

			var item = new Item();
			item.Name = r[0];
			item.Maker = r[2];
			item.EstimatedPrice = r[4];
			webDriver.Quit();
			return item;
		}



	}

	class Item
	{
		public string Barcode { get; set; }

		public string Name { get; set; }

		public Decimal Price{ get; set; }

		public string EstimatedPrice { get; set; }

		public string Maker { get; set; }

		public Bitmap Image { get; set; }
	}

	
}
