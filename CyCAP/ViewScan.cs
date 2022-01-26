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
    public partial class ViewScan : MetroForm
    {
        
        public ViewScan(string refID)
        {
            InitializeComponent();
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
                    metroLabel1.Text += " " + dr["FirstName"].ToString() + " " + dr["SecondName"].ToString() + " " + dr["OtherName"].ToString();
                    metroLabel2.Text += " " + dr["Telephone"].ToString();
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
            catch(Exception ex)
            {
                MetroMessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
               con.Close();
            }
        }


        private void myPrintDocument2_PrintPage(System.Object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {

            Bitmap myBitmap1 = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            pictureBox1.DrawToBitmap(myBitmap1, new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height));

            e.Graphics.DrawImage(myBitmap1, 50, 50, 600, 400);

            myBitmap1.Dispose();

        }

        private void btnPrintPicture_Click(object sender, EventArgs e)
        {

            System.Drawing.Printing.PrintDocument myPrintDocument1 = new System.Drawing.Printing.PrintDocument();

            PrintDialog myPrinDialog1 = new PrintDialog();

            myPrintDocument1.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(myPrintDocument2_PrintPage);

            myPrinDialog1.Document = myPrintDocument1;



            if (myPrinDialog1.ShowDialog() == DialogResult.OK)
            {
                myPrintDocument1.Print();
            }

        }

        private void metroButton3_Click(object sender, EventArgs e)
        {
            pictureBox1.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            pictureBox1.Refresh();
        }
    }


}
