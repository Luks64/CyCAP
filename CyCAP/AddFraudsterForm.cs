using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Forms;

namespace CyCAP
{
    public partial class AddFraudsterForm : MetroForm
    {
        public AddFraudsterForm()
        {
            InitializeComponent();
        }

        private void metroTile1_Click(object sender, EventArgs e)
        {
            OpenFileDialog getImage = new OpenFileDialog() { Filter = "Image Files(*.jpeg;*.bmp;*.png;*.jpg)|*.jpeg;*.bmp;*.png;*.jpg" };
            getImage.ShowDialog();
            pictureBox1.ImageLocation = getImage.FileName;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                MetroMessageBox.Show(this, "Photo Is Required", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                //Convert Image to byte
                MemoryStream ms = new MemoryStream();
                pictureBox1.Image.Save(ms, ImageFormat.Jpeg);
                byte[] photo_aray = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(photo_aray, 0, photo_aray.Length);

                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CyCAP.Properties.Settings.CyCAPDBConnectionString"].ConnectionString);

                // Initialize command
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "INSERT into Fraudster(FirstName,SecondName,Picture)" +
                "VALUES (@FirstName, @SecondName, @Picture)";
                // Add ADO.NET parameters
                cmd.Parameters.AddWithValue("@FirstName", metroTextBox1.Text);
                cmd.Parameters.AddWithValue("@SecondName", metroTextBox2.Text);
                cmd.Parameters.AddWithValue("@Picture", photo_aray);
                // Execute command
                using (con)
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                //Finally
                pictureBox1.Image = null;
                metroTextBox1.Text = string.Empty;
                metroTextBox2.Text = string.Empty;
                MetroMessageBox.Show(this, "Fraudster Saved Successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Dispose();
            }
        }
    }
}
