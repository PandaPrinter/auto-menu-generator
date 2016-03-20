using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Project1___Menu_Generator
{
    
    public abstract class writer
    {
        public abstract void write(string[] menu);
    }

    public class HTMLWriter : writer
    {
        public override void write(string[] menu)
        {
            StringBuilder writer = new StringBuilder();
            string[] listitem;
            writer.Append("<HTML><HEAD><TITLE>Menu</TITLE></HEAD><BODY><CENTER>Menu</CENTER>");
                                  
            foreach(string menuitem in menu)
            {
                string[] category = menuitem.Split('|');
                writer.Append("<H1>");
                writer.Append(category[0] + "</H1>");
                writer.Append("<UL>");
                if (category[1] == "")
                {
                    writer.Append("Oops, no dishes for " + category[0] + " today");
                }
                for(int j =1; j < category.Length-1;j++)
                {
                    if (category[j] != null)
                    {
                        listitem = category[j].Split('`');
                        writer.Append("<LI>" + listitem[0] + "<BR><I>" + listitem[1] + "</I><BR>" + listitem[2] + "</LI>");

                    }
                }
                writer.Append("</UL>");

            }
            writer.Append("</BODY>");
            string final = writer.ToString();
            try
            {
                using (FileStream fs = File.Create(Application.StartupPath + "\\menu.html"))
                {
                    Byte[] info = new UTF8Encoding(true).GetBytes(final);
                    fs.Write(info, 0, info.Length);
                }
            }
            catch(Exception)
            {
                Form1.ErrorLog.Text = "File couldn't be created";
            }
           }
    }

    public class TextWriter : writer
    {
        public override void write(string[] menu)
        {
            StringBuilder writer = new StringBuilder();
            writer.AppendLine("Menu\n\n");

            foreach (string menuitem in menu)
            {
                string[] category = menuitem.Split('|');
                writer.AppendLine(category[0].ToUpper() + "\n");
                if (category[1] == "")
                {
                    writer.Append("Oops, no dishes for " + category[0] + " today");
                }
                for (int j = 1; j < category.Length - 1; j++)
                {
                    if (category[j] != null)
                    {
                        string[] listitem = category[j].Split('`');
                        writer.Append(listitem[0] + "        " + listitem[2] + "\n" + listitem[1] + "\n\n");
                    }
                }
                writer.AppendLine("\n\n");

            }
            string final = writer.ToString();
            try
            {
                using (FileStream fs = File.Create(Application.StartupPath + "\\menu.txt"))
                {
                    Byte[] info = new UTF8Encoding(true).GetBytes(final);
                    fs.Write(info, 0, info.Length);
                }
            }
            catch (Exception)
            {
                Form1.ErrorLog.Text = "File couldn't be created";
            }
        }
    }
    public class XMLwriter : writer
    {
        public override void write(string[] menu)
        {
            try
            {
                using (XmlWriter xwriter = XmlWriter.Create(Application.StartupPath + "\\menu.xml"))
                {
                    xwriter.WriteStartDocument();
                    xwriter.WriteStartElement("MenuItems");

                    foreach (string menuitem in menu)
                    {
                        string[] category = menuitem.Split('|');
                        xwriter.WriteStartElement("MenuCategory");
                        xwriter.WriteAttributeString("category", category[0]);

                        for (int j = 1; j < category.Length - 1; j++)
                        {
                            if (category[j] != null)
                            {
                                string[] listitem = category[j].Split('`');
                                xwriter.WriteStartElement("MenuItem");
                                xwriter.WriteElementString("Name", listitem[0]);
                                xwriter.WriteStartElement("Price");
                                string[] currency = listitem[2].Split(' ');
                                xwriter.WriteElementString("CurrencyCode", currency[0]);
                                xwriter.WriteElementString("Amount", currency[1]);
                                xwriter.WriteEndElement();
                                xwriter.WriteElementString("Description", listitem[1]);
                                xwriter.WriteEndElement();
                            }
                        }
                        xwriter.WriteEndElement();
                    }
                    xwriter.WriteEndElement();
                    xwriter.WriteEndDocument();

                }
            }
            catch (Exception)
            {
                Form1.ErrorLog.Text = "File couldn't be created";
            }
        }
    }

    public class FoodItem
    {
        [JsonProperty("id")]
        private int id=0;
        [JsonProperty("name")]
        private string name;
        public string getname()
        {
            return name;
        }
        [JsonProperty("price")]

        private string price;
        public string getprice()
        {
            return price;
        }
        
        [JsonProperty("description")]
        private string description;
        public string getdescription()
        {
            return description;
        }        
        
        [JsonProperty("category")]
        private string category;
        public string getcategory()
        {
            return category;
        }
        [JsonProperty("-country")]
        private string country;
        public string getcountry()
        {
            return country;
        }

        public FoodItem(string name, string price, string description, string category, string country)                   
        {
            this.name = name;
            this.country = country;
            this.price = price;
            this.description = description;
            this.category = category;
        }
    }
    
    public abstract class reader
    {
        public abstract List<FoodItem> read(string filepath);
    }

    public class xmlreaderGB : reader
    {
        
        public override List<FoodItem> read(string filepath)
        {
            List<FoodItem> ItemList = new List<FoodItem>();
            string name, amt, description, category, country;
            
            XmlDocument fooditems = new XmlDocument();
            try
            {
                fooditems.Load(filepath);
            }
            catch (IOException)            
                {
                    Form1.ErrorLog.Text = "Input File not found or cant be loaded";
                    return ItemList;
                }
            XmlNodeList list = fooditems.SelectNodes("/FoodItemData/FoodItem");
            foreach (XmlNode food in list)
            {
                if (food.Attributes["country"].Value == "GB")
                {
                    country = "GB";
                    name = food["name"].InnerText;
                    category = food["category"].InnerText;
                    description = food["description"].InnerText;
                    amt = food["price"].InnerText;
                    FoodItem item = new FoodItem(name, amt, description, category, country);
                    
                    ItemList.Add(item);
                }
            }
            return ItemList;
        }
    }
    public class xmlreaderUS : reader
    {

        public override List<FoodItem> read(string filepath)
        {
            List<FoodItem> ItemList = new List<FoodItem>();
            string name, amt, description, category, country;

            XmlDocument fooditems = new XmlDocument();
            try
            {
                fooditems.Load(filepath);
            }
            catch (IOException)
            {
                Form1.ErrorLog.Text = "File not found or cant be loaded";
                return ItemList;
            } 
            XmlNodeList list = fooditems.SelectNodes("/FoodItemData/FoodItem");
            foreach (XmlNode food in list)
            {
                if (food.Attributes["country"].Value == "US")
                {
                    country = "US";
                    name = food["name"].InnerText;
                    category = food["category"].InnerText;
                    description = food["description"].InnerText;
                    amt = food["price"].InnerText;
                    FoodItem item = new FoodItem(name, amt, description, category, country);
                    ItemList.Add(item);
                }
            }
            return ItemList;
        }
    }
    public class JSONReaderUS : reader
    {
        public override List<FoodItem> read(string filepath)
        {
            string name, amt, description, category, country;
            StreamReader sr = null;
            List<FoodItem> ItemList = new List<FoodItem>();
            try
            {
                sr = File.OpenText(filepath);
            }
            catch (IOException)
            {
                Form1.ErrorLog.Text = "File not found or cant be loaded";
                return ItemList;
            }
            string json = sr.ReadToEnd();



            JObject root = JObject.Parse(json);

            JArray items = (JArray)root["FoodItemData"];
            json = items.ToString();
            List<FoodItem> temp = (List<FoodItem>)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(List<FoodItem>));

            
            foreach(FoodItem ob in temp)
            {
                if(ob.getcountry() == "US")
                {
                    country = "US";
                    name = ob.getname();
                    category = ob.getcategory();
                    description = ob.getdescription();
                    amt = ob.getprice();
                    FoodItem item = new FoodItem(name, amt, description, category, country);
                    ItemList.Add(item);
                }
            }
            return ItemList;
        }
    }

    public class JSONReaderGB : reader
    {
        public override List<FoodItem> read(string filepath)
        {
            string name, amt, description, category, country;
            List<FoodItem> ItemList = new List<FoodItem>();
            StreamReader sr = null;
            try
            {
                sr = File.OpenText(filepath);
            }
            catch (IOException)
            {
                Form1.ErrorLog.Text = "File not found or cant be loaded";
                return ItemList;
            } 
            string json = sr.ReadToEnd();



            JObject root = JObject.Parse(json);

            JArray items = (JArray)root["FoodItemData"];
            json = items.ToString();
            List<FoodItem> temp = (List<FoodItem>)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(List<FoodItem>));


            foreach (FoodItem ob in temp)
            {
                if (ob.getcountry() == "GB")
                {
                    country = "GB";
                    name = ob.getname();
                    category = ob.getcategory();
                    description = ob.getdescription();
                    amt = ob.getprice();
                    FoodItem item = new FoodItem(name, amt, description, category, country);
                    ItemList.Add(item);
                }
            }
            return ItemList;
        }
    }
    public abstract class Generator
    {
        public abstract string[] GenerateMenu(List<FoodItem> ItemList);
    }

    public class AllDayGenerator : Generator
    {
        public override string[] GenerateMenu(List<FoodItem> ItemList)
        {
            StringBuilder Breakfast = new StringBuilder();
            Breakfast.Append("Breakfast|");
            StringBuilder Lunch = new StringBuilder(); ;
            Lunch.Append("Lunch|");
            StringBuilder Snack = new StringBuilder();
            Snack.Append("Snack|");
            StringBuilder SideItems = new StringBuilder();
            SideItems.Append("Side Items|");
            StringBuilder Appetizer = new StringBuilder();
            Appetizer.Append("Appetizer|");
            StringBuilder Dinner = new StringBuilder();
            Dinner.Append("Dinner|");
            StringBuilder Dessert = new StringBuilder();
            Dessert.Append("Dessert|");

            string category, temp;

            foreach(FoodItem item in ItemList)
            {
                category = item.getcategory();
                string cur = null;
                
                if(category == "Breakfast")
                {
                    if (item.getcountry() == "US") cur = "USD";
                    else if (item.getcountry() == "GB") cur = "GBP";
                    temp = item.getname() + "`" + item.getdescription() + "`" + cur + " " + item.getprice() + "|";
                    Breakfast.Append(temp);
                }
                else if(category == "Lunch")
                {
                    if (item.getcountry() == "US") cur = "USD";
                    else if (item.getcountry() == "GB") cur = "GBP";
                    temp = item.getname() + "`" + item.getdescription() + "`" + cur + " " + item.getprice() + "|";
                    Lunch.Append(temp);
                }
                else if(category == "Snack")
                {
                    if (item.getcountry() == "US") cur = "USD";
                    else if (item.getcountry() == "GB") cur = "GBP";
                    temp = item.getname() + "`" + item.getdescription() + "`" + cur + " " + item.getprice() + "|";
                    Snack.Append(temp);
                }
                else if (category == "Side Dish")
                {
                    if (item.getcountry() == "US") cur = "USD";
                    else if (item.getcountry() == "GB") cur = "GBP";
                    temp = item.getname() + "`" + item.getdescription() + "`" + cur + " " + item.getprice() + "|";
                    SideItems.Append(temp);
                }
                else if (category == "Appetizer")
                {
                    if (item.getcountry() == "US") cur = "USD";
                    else if (item.getcountry() == "GB") cur = "GBP";
                    temp = item.getname() + "`" + item.getdescription() + "`" + cur + " " + item.getprice() + "|";
                    Appetizer.Append(temp);
                }
                else if (category == "Dinner")
                {
                    if (item.getcountry() == "US") cur = "USD";
                    else if (item.getcountry() == "GB") cur = "GBP";
                    temp = item.getname() + "`" + item.getdescription() + "`" + cur + " " + item.getprice() + "|";
                    Dinner.Append(temp);
                }
                else if (category == "Dessert")
                {
                    if (item.getcountry() == "US") cur = "USD";
                    else if (item.getcountry() == "GB") cur = "GBP";
                    temp = item.getname() + "`" + item.getdescription() + "`" + cur + " " + item.getprice() + "|";
                    Dessert.Append(temp);
                }
            }

            string[] menu = new string[7];
            menu[0] = Breakfast.ToString();
            menu[1] = Lunch.ToString();
            menu[2] = Snack.ToString();
            menu[3] = SideItems.ToString();
            menu[4] = Appetizer.ToString();
            menu[5] = Dinner.ToString();
            menu[6] = Dessert.ToString();
            return menu;
        }
    }

    public class DinerGenerator : Generator
    {
        public override string[] GenerateMenu(List<FoodItem> ItemList)
        {
            StringBuilder Breakfast = new StringBuilder();
            Breakfast.Append("Breakfast|");
            
            StringBuilder Lunch = new StringBuilder(); ;
            Lunch.Append("Lunch|");
            
            StringBuilder Snack = new StringBuilder();
            Snack.Append("Snack|");
            
            StringBuilder Appetizer = new StringBuilder();
            Appetizer.Append("Appetizer|");
            
            StringBuilder Dessert = new StringBuilder();
            Dessert.Append("Dessert|");

            string category, temp;

            foreach (FoodItem item in ItemList)
            {
                category = item.getcategory();
                string cur = null;

                if (category == "Breakfast")
                {
                    if (item.getcountry() == "US") cur = "USD";
                    else if (item.getcountry() == "GB") cur = "GBP";
                    temp = item.getname() + "`" + item.getdescription() + "`" + cur + " " + item.getprice() + "|";
                    Breakfast.Append(temp);
                }
                else if (category == "Lunch")
                {
                    if (item.getcountry() == "US") cur = "USD";
                    else if (item.getcountry() == "GB") cur = "GBP";
                    temp = item.getname() + "`" + item.getdescription() + "`" + cur + " " + item.getprice() + "|";
                    Lunch.Append(temp);
                }
                else if (category == "Snack")
                {
                    if (item.getcountry() == "US") cur = "USD";
                    else if (item.getcountry() == "GB") cur = "GBP";
                    temp = item.getname() + "`" + item.getdescription() + "`" + cur + " " + item.getprice() + "|";
                    Snack.Append(temp);
                }
                
                else if (category == "Appetizer")
                {
                    if (item.getcountry() == "US") cur = "USD";
                    else if (item.getcountry() == "GB") cur = "GBP";
                    temp = item.getname() + "`" + item.getdescription() + "`" + cur + " " + item.getprice() + "|";
                    Appetizer.Append(temp);
                }
                
                else if (category == "Dessert")
                {
                    if (item.getcountry() == "US") cur = "USD";
                    else if (item.getcountry() == "GB") cur = "GBP";
                    temp = item.getname() + "`" + item.getdescription() + "`" + cur + " " + item.getprice() + "|";
                    Dessert.Append(temp);
                }
            }

            string[] menu = new string[5];
            menu[0] = Breakfast.ToString();
            menu[1] = Lunch.ToString();
            menu[2] = Snack.ToString();
            menu[3] = Appetizer.ToString();
            menu[4] = Dessert.ToString();
            return menu;
        }
    }

    public class EveningGenerator : Generator
    {
        public override string[] GenerateMenu(List<FoodItem> ItemList)
        {
            
            StringBuilder SideItems = new StringBuilder();
            SideItems.Append("Side Items|");
            StringBuilder Appetizer = new StringBuilder();
            Appetizer.Append("Appetizer|");
            StringBuilder Dinner = new StringBuilder();
            Dinner.Append("Dinner|");
            StringBuilder Dessert = new StringBuilder();
            Dessert.Append("Dessert|");

            string category, temp;

            foreach (FoodItem item in ItemList)
            {
                category = item.getcategory();
                string cur = null;
                if (category == "Side Dish")
                {
                    if (item.getcountry() == "US") cur = "USD";
                    else if (item.getcountry() == "GB") cur = "GBP";
                    temp = item.getname() + "`" + item.getdescription() + "`" + cur + " " + item.getprice() + "|";
                    SideItems.Append(temp);
                }
                else if (category == "Appetizer")
                {
                    if (item.getcountry() == "US") cur = "USD";
                    else if (item.getcountry() == "GB") cur = "GBP";
                    temp = item.getname() + "`" + item.getdescription() + "`" + cur + " " + item.getprice() + "|";
                    Appetizer.Append(temp);
                }
                else if (category == "Dinner")
                {
                    if (item.getcountry() == "US") cur = "USD";
                    else if (item.getcountry() == "GB") cur = "GBP";
                    temp = item.getname() + "`" + item.getdescription() + "`" + cur + " " + item.getprice() + "|";
                    Dinner.Append(temp);
                }
                else if (category == "Dessert")
                {
                    if (item.getcountry() == "US") cur = "USD";
                    else if (item.getcountry() == "GB") cur = "GBP";
                    temp = item.getname() + "`" + item.getdescription() + "`" + cur + " " + item.getprice() + "|";
                    Dessert.Append(temp);
                }
            }

            string[] menu = new string[4];
            menu[0] = SideItems.ToString();
            menu[1] = Appetizer.ToString();
            menu[2] = Dinner.ToString();
            menu[3] = Dessert.ToString();
            return menu;
        }
    }

    public abstract class MenuFactory
    {
        public abstract reader CreateReader();
        public abstract Generator CreateGenerator();
        public abstract writer CreateWriter();
    }

    public class USAllDayHTML : MenuFactory
    {
        public override reader CreateReader()
        {
            return new JSONReaderUS();
        }
        public override Generator CreateGenerator()
        {
            return new AllDayGenerator();
        }
        public override writer CreateWriter()
        {
            return new HTMLWriter();
        }
    }

    public class USAllDayText : MenuFactory
    {
        public override reader CreateReader()
        {
            return new JSONReaderUS();
        }
        public override Generator CreateGenerator()
        {
            return new AllDayGenerator();
        }
        public override writer CreateWriter()
        {
            return new TextWriter();
        }
    }

    public class USEveningText : MenuFactory
    {
        public override reader CreateReader()
        {
            return new JSONReaderUS();
        }
        public override Generator CreateGenerator()
        {
            return new EveningGenerator();
        }
        public override writer CreateWriter()
        {
            return new TextWriter();
        }
    }
    public class USDinerText : MenuFactory
    {
        public override reader CreateReader()
        {
            return new JSONReaderUS();
        }
        public override Generator CreateGenerator()
        {
            return new DinerGenerator();
        }
        public override writer CreateWriter()
        {
            return new TextWriter();
        }
    }
    public class GBAllDayText : MenuFactory
    {
        public override reader CreateReader()
        {
            return new xmlreaderGB();
        }
        public override Generator CreateGenerator()
        {
            return new AllDayGenerator();
        }
        public override writer CreateWriter()
        {
            return new TextWriter();
        }
    }

    public class GBEveningText : MenuFactory
    {
        public override reader CreateReader()
        {
            return new xmlreaderGB();
        }
        public override Generator CreateGenerator()
        {
            return new EveningGenerator();
        }
        public override writer CreateWriter()
        {
            return new TextWriter();
        }
    }
    public class GBDinerText : MenuFactory
    {
        public override reader CreateReader()
        {
            return new xmlreaderGB();
        }
        public override Generator CreateGenerator()
        {
            return new DinerGenerator();
        }
        public override writer CreateWriter()
        {
            return new TextWriter();
        }
    }

    public class USEveningHTML : MenuFactory
    {
        public override reader CreateReader()
        {
            return new JSONReaderUS();
        }
        public override Generator CreateGenerator()
        {
            return new EveningGenerator();
        }
        public override writer CreateWriter()
        {
            return new HTMLWriter();
        }
    }
    public class USDinerHTML : MenuFactory
    {
        public override reader CreateReader()
        {
            return new JSONReaderUS();
        }
        public override Generator CreateGenerator()
        {
            return new DinerGenerator();
        }
        public override writer CreateWriter()
        {
            return new HTMLWriter();
        }
    }

    public class USAllDayXML : MenuFactory
    {
        public override reader CreateReader()
        {
            return new JSONReaderUS();
        }
        public override Generator CreateGenerator()
        {
            return new AllDayGenerator();
        }
        public override writer CreateWriter()
        {
            return new XMLwriter();
        }
    }

    public class USEveningXML : MenuFactory
    {
        public override reader CreateReader()
        {
            return new JSONReaderUS();
        }
        public override Generator CreateGenerator()
        {
            return new EveningGenerator();
        }
        public override writer CreateWriter()
        {
            return new XMLwriter();
        }
    }

    public class USDinerXML : MenuFactory
    {
        public override reader CreateReader()
        {
            return new JSONReaderUS();
        }
        public override Generator CreateGenerator()
        {
            return new DinerGenerator();
        }
        public override writer CreateWriter()
        {
            return new XMLwriter();
        }
    }

    public class GBAllDayHTML : MenuFactory
    {
        public override reader CreateReader()
        {
            return new xmlreaderGB();
        }
        public override Generator CreateGenerator()
        {
            return new AllDayGenerator();
        }
        public override writer CreateWriter()
        {
            return new HTMLWriter();
        }
    }

    public class GBEveningHTML : MenuFactory
    {
        public override reader CreateReader()
        {
            return new xmlreaderGB();
        }
        public override Generator CreateGenerator()
        {
            return new EveningGenerator();
        }
        public override writer CreateWriter()
        {
            return new HTMLWriter();
        }
    }
    public class GBDinerHTML : MenuFactory
    {
        public override reader CreateReader()
        {
            return new xmlreaderGB();
        }
        public override Generator CreateGenerator()
        {
            return new DinerGenerator();
        }
        public override writer CreateWriter()
        {
            return new HTMLWriter();
        }
    }

    public class GBAllDayXML : MenuFactory
    {
        public override reader CreateReader()
        {
            return new xmlreaderGB();
        }
        public override Generator CreateGenerator()
        {
            return new AllDayGenerator();
        }
        public override writer CreateWriter()
        {
            return new XMLwriter();
        }
    }

    public class GBEveningXML : MenuFactory
    {
        public override reader CreateReader()
        {
            return new xmlreaderGB();
        }
        public override Generator CreateGenerator()
        {
            return new EveningGenerator();
        }
        public override writer CreateWriter()
        {
            return new XMLwriter();
        }
    }

    public class GBDinerXML : MenuFactory
    {
        public override reader CreateReader()
        {
            return new xmlreaderGB();
        }
        public override Generator CreateGenerator()
        {
            return new DinerGenerator();
        }
        public override writer CreateWriter()
        {
            return new XMLwriter();
        }
    }
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            

        }
    }
}
