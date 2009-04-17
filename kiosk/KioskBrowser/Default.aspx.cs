using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace KioskBrowser
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        public void Button1_Click(object sender, EventArgs e)
        {
            passTextBox.Text += "1";
        }

        public void Button2_Click(object sender, EventArgs e)
        {
            passTextBox.Text += "2";
        }

        public void Button3_Click(object sender, EventArgs e)
        {
            passTextBox.Text += "3";
        }

        public void Button4_Click(object sender, EventArgs e)
        {
            passTextBox.Text += "4";
        }

        public void Button5_Click(object sender, EventArgs e)
        {
            passTextBox.Text += "5";
        }

        public void Button6_Click(object sender, EventArgs e)
        {
            passTextBox.Text += "6";
        }

        public void Button7_Click(object sender, EventArgs e)
        {
            passTextBox.Text += "7";
        }

        public void Button8_Click(object sender, EventArgs e)
        {
            passTextBox.Text += "8";
        }

        public void Button9_Click(object sender, EventArgs e)
        {
            passTextBox.Text += "9";
        }

        public void Button0_Click(object sender, EventArgs e)
        {
            passTextBox.Text += "0";
        }

        public void backButton_Click(object sender, EventArgs e)
        {
            try
            {
                passTextBox.Text = passTextBox.Text.Remove(passTextBox.Text.Length - 1);
            }
            catch (Exception exception)
            {

            }
        }

        public void okButton_Click(object sender, EventArgs e)
        {
            
        }
    }
}
