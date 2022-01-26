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
    public partial class ViewFraudsterPic : MetroForm
    {
        public ViewFraudsterPic(string fraudID)
        {
            InitializeComponent();


            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CyCAP.Properties.Settings.CyCAPDBConnectionString"].ConnectionString);

            try
            {

                SqlCommand cmd;
                
                string query = "SELECT * from Fraudster WHERE Id = '" + fraudID + "'";
                cmd = new SqlCommand(query, con);
                
                SqlDataAdapter dp = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet("MyImages");

                byte[] MyData = new byte[0];

                dp.Fill(ds, "MyImages");
                DataRow myRow;
                myRow = ds.Tables["MyImages"].Rows[0];

                MyData = (byte[])myRow["Picture"];

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

        private void ViewFraudsterPic_Load(object sender, EventArgs e)
        {

        }
    }
}
