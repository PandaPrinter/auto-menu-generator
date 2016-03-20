using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project1___Menu_Generator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Country.Items.Add("US");
            Country.Items.Add("GB");
            Category.Items.Add("All Day");
            Category.Items.Add("Evening Only");
            Category.Items.Add("Diner");
            Output.Items.Add("HTML");
            Output.Items.Add("XML");
            Output.Items.Add("Text");
            


        }

        private void Country_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            MenuFactory obj = null;
            Form1.ErrorLog.Text = null;
            if (Country.SelectedItem == "US")
            {
                if (Category.SelectedItem == "All Day")
                {
                    if (Output.SelectedItem == "HTML")
                    {
                        obj = new USAllDayHTML();
                    }
                    else if (Output.SelectedItem == "XML")
                    {
                        obj = new USAllDayXML();
                    }
                    else
                    {
                        obj = new USAllDayText();
                    }
                }
                else if (Category.SelectedItem == "Evening Only")
                {
                    if (Output.SelectedItem == "HTML")
                    {
                        obj = new USEveningHTML();
                    }
                    else if (Output.SelectedItem == "XML")
                    {
                        obj = new USEveningXML();
                    }
                    else
                    {
                        obj = new USEveningText();
                    }
                }
                else
                {
                    if (Output.SelectedItem == "HTML")
                    {
                        obj = new USDinerHTML();
                    }
                    else if (Output.SelectedItem == "XML")
                    {
                        obj = new USDinerXML();
                    }
                    else
                    {
                        obj = new USDinerText();
                    }
                }
            }
            else
            {
                if (Category.SelectedItem == "All Day")
                    {
                        if (Output.SelectedItem == "HTML")
                        {
                            obj = new GBAllDayHTML();
                        }
                        else if (Output.SelectedItem == "XML")
                        {
                            obj = new GBAllDayXML();
                        }
                        else
                        {
                            obj = new GBAllDayText();
                        }
                    }
                    else if (Category.SelectedItem == "Evening Only")
                    {
                        if (Output.SelectedItem == "HTML")
                        {
                            obj = new GBEveningHTML();
                        }
                        else if (Output.SelectedItem == "XML")
                        {
                            obj = new GBEveningXML();
                        }
                        else
                        {
                            obj = new GBEveningText();
                        }
                    }
                    else
                    {
                        if (Output.SelectedItem == "HTML")
                        {
                            obj = new GBDinerHTML();
                        }
                        else if (Output.SelectedItem == "XML")
                        {
                            obj = new GBDinerXML();
                        }
                        else
                        {
                            obj = new GBDinerText();
                        }
                    }
                
            }


            reader r = obj.CreateReader();
            List<FoodItem> ItemList = new List<FoodItem>();
            string path = null;
            if (Country.SelectedItem == "US")
            {
                path = Application.StartupPath + "\\FoodItemData.json";
            }
            else
            {
                path = Application.StartupPath + "\\FoodItemData.xml";

            }
            ItemList = r.read(path);
            if (ItemList.Count > 0)
            {
                Generator g = obj.CreateGenerator();
                string[] menu = g.GenerateMenu(ItemList);
                writer w = obj.CreateWriter();
                w.write(menu);
                Form1.ErrorLog.Text = "File Created";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
