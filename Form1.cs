using Microsoft.Win32;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace InstagramBot
{
    public partial class Form1 : Form
    {
        string PORT;
        string PORT2;
        string YOL;
        RegistryKey reg_key;
        [DllImport("wininet.dll")]
        public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
        public const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        public const int INTERNET_OPTION_REFRESH = 37;
        bool settingsReturn, refreshReturn;
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            Degistir();
        }
        public void Degistir()
        {
            Random random = new Random();
            int number = random.Next(20, 500);
            textBox2.Text = number.ToString();
            
        }
        public void Password()
        {
            Random random = new Random();
            int number = random.Next(2000000, 10000000);
            textBox10.Text = number.ToString();
        }
        public void Baslat(string PORT)
        {
            reg_key = null;
            reg_key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
            string proxy = textBox5.Text + ":" + PORT;
            reg_key.SetValue("ProxyEnable", 1);
            reg_key.SetValue("ProxyServer", proxy);
            settingsReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
            refreshReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
        }
        public void MailChangeNo()
        {
            Random random = new Random();
            int number = random.Next(4000, 10000);
            textBox11.Text = number.ToString();
        }
        public void MailChange()
        {
            Random random = new Random();
            int number = random.Next(1, 4);
            textBox13.Text = number.ToString();
            if (number == 1)
            {
                textBox12.Text = "@hotmail.com";
            }
            else if (number == 2)
            {
                textBox12.Text = "@gmail.com";
            }
            else
            {
                textBox12.Text = "@yahoo.com";
            }
        }
        public void Bitir()
        {
            reg_key = null;
            reg_key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
            reg_key.SetValue("ProxyEnable", 0);
            reg_key.SetValue("ProxyServer", "0.0.0.0:0000");
            settingsReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
            refreshReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);

        }
        public void DosyaYazdir()
        {
            string sSelectedFolder;
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            fbd.Description = "Custom Description"; //not mandatory
            if (fbd.ShowDialog() == DialogResult.OK)
            {

                sSelectedFolder = fbd.SelectedPath;
            }

            else
                sSelectedFolder = string.Empty;
            //  MessageBox.Show(sSelectedFolder);
            YOL = sSelectedFolder;
        }

        public static ChromeDriver driver
        {
            get;
            set;
        }
  

        private void Button2_Click(object sender, EventArgs e)//Proxy Aktif Butonu
        {
            PORT = textBox6.Text;
            PORT2 = textBox7.Text;
            Baslat(PORT);
        }
        private void Button3_Click(object sender, EventArgs e)//Proxy Pasif Butonu
        {
            Bitir();
        }

        public async void StartDriverP(string proxy)//ASENKRON BİR ŞEKİLDE ÇALIŞTIR DONMA OLMASIN..
        {                     
            Degistir();
            Password();
            MailChangeNo();
            MailChange();
            try
            {
                if (Convert.ToInt32(PORT) < Convert.ToInt32(PORT2))
                {
                    PORT = (Int32.Parse(PORT) + 1).ToString();
                    Baslat(PORT);

                }
                else
                {
                    Bitir();
                    driver.Quit();
                }
            }
            catch (Exception ex)
            {

                listBox1.Items.Add("Proxy Hatası: " + ex.Message);
            }
            
            ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;//KOMUT PENCERESİNİ GİZLE
            ChromeOptions chromeOptions = new ChromeOptions();//PROXY KULLANMAK İÇİN 
            if (!string.IsNullOrEmpty(proxy))
            {
                chromeOptions.AddArgument("--proxy-server=" + proxy);
            }
            chromeOptions.AddArgument("disable-infobars");
            chromeOptions.AddArguments("--allow-file-access");
            chromeOptions.AddArguments("--incognito");//GİZLİ SEKME...                        
            chromeOptions.AddArgument("--disable-web-security");
            chromeOptions.AddArgument("--allow-running-insecure-content");           
            driver = new ChromeDriver(chromeDriverService, chromeOptions, TimeSpan.FromMinutes(3.0));
            await Task.Delay(2000);
        }

        public static bool IsTestElementPresent(By element)
        {
            try
            {
                driver.FindElement(element);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        private void ListBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            label1.Text = "İsim Sayısı:" + listBox2.Items.Count;
        }
        private void ListBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            label2.Text = "Soyisim Sayısı: " + listBox2.Items.Count;
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
        }
        private void TextBox2_TextChanged(object sender, EventArgs e)
        {
        }

        private async void Button1_Click(object sender, EventArgs e)
        {
            await Task.Run(async () =>
            {
                try
                {
                    int i;
                    for (i = 0; i < listBox2.Items.Count; i++)
                    {

                        StartDriverP(textBox5.Text + ":" + PORT.ToString());
                        driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(59);//SAYFA YÜKLENME SÜRESİ
                        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(59);
                        driver.Manage().Window.Size = new Size(1280, 720);
                        //driver.Navigate().Refresh();

                        //await Task.Delay(2000);
                        if (await InstaKayit(i))
                        {
                            listBox1.Items.Add(i + 1 + ". Kayıt işlemi başarılı.");
                            StreamWriter SW = new StreamWriter(@YOL + "user.txt", append: true);//DosyaYazdir();                                           
                            SW.WriteLine(listBox2.Items[i].ToString().ToLower() + "_" + listBox3.Items[i].ToString().ToLower() + textBox11.Text + textBox2.Text + ":" + listBox2.Items[i].ToString().ToLower() + listBox3.Items[i].ToString().ToLower() + textBox10.Text);
                            SW.Close();
                            goto label;
                        }
                        else
                        {
                            listBox1.Items.Add(i + 1 + ". Kayıt işlemi hatalı.");
                            driver.Quit();
                            //goto label;
                        }
                    //driver.Navigate().Refresh();
                    //driver.SwitchTo().Window(driver.WindowHandles.First());
                    label:
                        driver.Quit();
                    }
                }
                catch (Exception ex)
                {
                    listBox1.Items.Add("(btnBaslat) Hata: " + ex.Message);
                }

            });
        }
        public async Task<bool> InstaKayit(int s)
        {
            try
            {
                //((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
                //await Task.Delay(1000);
                //driver.SwitchTo().Window(driver.WindowHandles.Last());
                //await Task.Delay(1000);
                driver.Navigate().GoToUrl("https://www.instagram.com/?hl=tr");
                await Task.Delay(2000);
                string isimSoyisim = listBox2.Items[s].ToString().ToLower() + " " + listBox3.Items[s].ToString().ToLower();
                string isim = listBox2.Items[s].ToString().ToLower();
                string soyisim = listBox3.Items[s].ToString().ToLower();//richTextBox2.Lines[s]
                string sifre = isim + soyisim + textBox10.Text;
                string mail = isim + "_" + soyisim + textBox11.Text + textBox2.Text + textBox12.Text + textBox4.Text;
                string kullaniciAdi = isim + "_" + soyisim + textBox11.Text + textBox2.Text;//DEVAM ET
                IWebElement email = driver.FindElement(By.Name("emailOrPhone"));
                char[] charMail = mail.ToCharArray();
                for (int i = 0; i < charMail.Length; i++)
                {
                    email.SendKeys(charMail[i].ToString());
                    await Task.Delay(150);
                }
                IWebElement name = driver.FindElement(By.Name("fullName"));
                char[] charName = isimSoyisim.ToCharArray();
                for (int i = 0; i < charName.Length; i++)
                {
                    name.SendKeys(charName[i].ToString());
                    await Task.Delay(150);
                }
                IWebElement username = driver.FindElement(By.Name("username"));
                char[] charUser = kullaniciAdi.ToCharArray();
                for (int i = 0; i < charUser.Length; i++)
                {
                    username.SendKeys(charUser[i].ToString());
                    await Task.Delay(150);
                }
                IWebElement password = driver.FindElement(By.Name("password"));
                char[] charPassword = sifre.ToCharArray();
                for (int i = 0; i < charPassword.Length; i++)
                {
                    password.SendKeys(charPassword[i].ToString());
                    await Task.Delay(150);
                }
                await Task.Delay(300);
                IWebElement btn = driver.FindElement(By.CssSelector("#react-root > section > main > article > div.rgFsT > div:nth-child(1) > div > form > div:nth-child(8) > div > button"));//Sign Up
                btn.Click();
                await Task.Delay(2000);
                IWebElement btn2 = driver.FindElement(By.CssSelector("#igCoreRadioButtonageRadioabove_18"));
                btn2.Click();
                await Task.Delay(2000);
                IWebElement IleriBtn = driver.FindElement(By.CssSelector("body > div.RnEpo.Yx5HN > div > div._0GT5G > div"));
                IleriBtn.Click();
                await Task.Delay(3000);
                IWebElement ReklamBtn = driver.FindElement(By.CssSelector("body > div.RnEpo.Yx5HN > div > div > div.mt3GC > button.aOOlW.HoLwm"));//Şimdi Değil Reklam Geç Butonu
                ReklamBtn.Click();//((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", btn2);//TIKLAMAK İÇİN...               
                await Task.Delay(2000);
                IWebElement ProfilBtn = driver.FindElement(By.CssSelector("#react-root > section > nav > div._8MQSO.Cx7Bp > div > div > div.ctQZg > div > div:nth-child(3) > a > svg"));
                ProfilBtn.Click();
                await Task.Delay(3000);//#react-root > section > main > div > header > section > div.Y2E37 > a > button                                           

                return true;
            }
            catch (Exception ex)
            {
                listBox1.Items.Add("Hata: " + ex.Message);
                return false;
            }
        }
        private void TextBox6_TextChanged(object sender, EventArgs e)
        {
        }

        private void İsimDosya_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();

            if (open.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            StreamReader streamReader = new StreamReader(open.FileName);
            string metin;
            while ((metin = streamReader.ReadLine()) != null)
            {
                listBox2.Items.Add(metin);
            }


            streamReader.Close();
        }

        private void SoyisimDosya_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();

            if (open.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            StreamReader streamReader = new StreamReader(open.FileName);
            string metin;
            while ((metin = streamReader.ReadLine()) != null)
            {
                listBox3.Items.Add(metin);
            }


            streamReader.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Button4_Click(object sender, EventArgs e)
        {
            /*FORMUN AÇILIŞINDA STRINGI KONTROL ETTIR BOSSA BUNU VOIDE AL CAGIR */
            //BU DEGERI STRINGE ATA
            DosyaYazdir();
            //PORT = ((Convert.ToInt32(PORT) + 1).ToString());
            //MessageBox.Show(PORT);
        }
    }
}
