using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Forms;

namespace CyCAP
{
    public partial class AddDetails : MetroForm
    {
        public string refNo { get; set; }

        
        public AddDetails(string refID)
        {
            refNo = refID;
            InitializeComponent();
            metroLabel3.Text += " " + refID;

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CyCAP.Properties.Settings.CyCAPDBConnectionString"].ConnectionString);

            try
            {

                SqlCommand cmd;
                SqlDataReader dr;
                string query = "SELECT * from Customer WHERE Id = '" + refID + "'";

                cmd = new SqlCommand(query, con);
                con.Open();
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    metroLabel1.Text += " " + dr["Telephone"].ToString() ;
                    
                }

                con.Close();
                SqlDataAdapter dp = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet("MyImages");

                byte[] MyData = new byte[0];

                dp.Fill(ds, "MyImages");
                DataRow myRow;
                myRow = ds.Tables["MyImages"].Rows[0];

                MyData = (byte[])myRow["ScannedID"];

                MemoryStream stream = new MemoryStream(MyData);
                //With the code below, you are in fact converting the byte array of image
                //to the real image.
                pictureBox1.Image = Image.FromStream(stream);
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

               
            }
            catch (Exception ex)
            {
                MetroMessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                con.Close();
            }
        }

        private void AddDetails_Load(object sender, EventArgs e)
        {

        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            if (metroTextBox1.Text == string.Empty && metroTextBox2.Text == string.Empty)
            {
                MetroMessageBox.Show(this, "Both First Name and Second Name are Required", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CyCAP.Properties.Settings.CyCAPDBConnectionString"].ConnectionString);

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "UPDATE Customer SET FirstName=@FirstName,SecondName=@SecondName, OtherName=@OtherName WHERE Id='"+refNo+"'";
                // Add ADO.NET parameters
                cmd.Parameters.AddWithValue("@FirstName", metroTextBox1.Text);
                cmd.Parameters.AddWithValue("@SecondName", metroTextBox2.Text);
                cmd.Parameters.AddWithValue("@OtherName", metroTextBox3.Text);
                // Execute command
                using (con)
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                 MetroMessageBox.Show(this, "Record Saved Successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                 this.Dispose();                
            }
        }
    }
}
