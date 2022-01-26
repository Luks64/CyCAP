using MetroFramework;
using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace CyCAP
{
    public partial class Form1 : MetroForm
    {
        Boolean bHaveMouse;
        Point ptOriginal = new Point();
        Point ptLast = new Point();
        Rectangle rectCropArea;
        Image srcImage;

        public Form1()
        {
            InitializeComponent();
            bHaveMouse = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'cyCAPDBDataSet1.Fraudster' table. You can move, or remove it, as needed.
            this.fraudsterTableAdapter.Fill(this.cyCAPDBDataSet1.Fraudster);
            // TODO: This line of code loads data into the 'cyCAPDBDataSet1.Customer' table. You can move, or remove it, as needed.
            this.customerTableAdapter.FillByWithoutName(this.cyCAPDBDataSet1.Customer);
            // TODO: This line of code loads data into the 'cyCAPDBDataSet.Customer' table. You can move, or remove it, as needed.
            this.customerTableAdapter.FillByWithName(this.cyCAPDBDataSet.Customer);

            metroGrid1.DataSource = this.cyCAPDBDataSet1.Customer;
            metroGrid2.DataSource = this.cyCAPDBDataSet.Customer;

        }

        private void fillByToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.customerTableAdapter.FillBy(this.cyCAPDBDataSet.Customer);
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

        }

        private void metroGrid2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //DataTable dtSource = (DataTable)dataGridView2.DataSource;
            //MessageBox.Show(dtSource.TableName, "");
            //MetroFramework.Controls.MetroGrid myGrid = new MetroFramework.Controls.MetroGrid();

            //Respond to button
            if (metroGrid2.CurrentCell.ColumnIndex.Equals(5) && e.RowIndex != -1)
            {
                string refID = metroGrid2.Rows[e.RowIndex].Cells[0].Value.ToString();
                ViewScan myScan = new ViewScan(refID);

                if (metroGrid2.CurrentCell != null)
                    myScan.ShowDialog();
            }

            //Response if row click excluding button column
            if (metroGrid2.CurrentCell.ColumnIndex != 5 && e.RowIndex != -1)
            {

            }
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            pictureBox1.Refresh();
            OpenFileDialog getImage = new OpenFileDialog() { Filter = "Image Files(*.jpeg;*.bmp;*.png;*.jpg)|*.jpeg;*.bmp;*.png;*.jpg" };
            getImage.ShowDialog();
            pictureBox1.ImageLocation = getImage.FileName;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            srcImage = pictureBox1.Image;
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null || metroTextBox1.Text == string.Empty)
            {
                MetroMessageBox.Show(this, "The Telephone Number and the ID Scan are required", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
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
                cmd.CommandText = "INSERT into Customer(Telephone,ScannedID)" +
                "VALUES (@Telephone, @ScannedID)";
                // Add ADO.NET parameters
                cmd.Parameters.AddWithValue("@Telephone", metroTextBox1.Text);
                cmd.Parameters.AddWithValue("@ScannedID", photo_aray);
                // Execute command
                using (con)
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                //Finally
                pictureBox1.Image = null;
                metroTextBox1.Text = string.Empty;
                MetroMessageBox.Show(this, "ID and Telephone Number Saved Successfully \n Navigate to \"Attach Details Tab\" to Add Name ", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }

            // TODO: This line of code loads data into the 'cyCAPDBDataSet1.Customer' table. You can move, or remove it, as needed.
            this.customerTableAdapter.FillByWithoutName(this.cyCAPDBDataSet1.Customer);
            // TODO: This line of code loads data into the 'cyCAPDBDataSet.Customer' table. You can move, or remove it, as needed.
            this.customerTableAdapter.FillByWithName(this.cyCAPDBDataSet.Customer);
            metroGrid1.DataSource = this.cyCAPDBDataSet1.Customer;
            metroGrid2.DataSource = this.cyCAPDBDataSet.Customer;
        }

        private void metroButton3_Click(object sender, EventArgs e)
        {
            //to bring out Fraudster Dialog
            AddFraudsterForm addFraud = new AddFraudsterForm();
            addFraud.ShowDialog();

            //
            //Reload The Fraudster Grid 
            //
            // TODO: This line of code loads data into the 'cyCAPDBDataSet1.Fraudster' table. You can move, or remove it, as needed.
            this.fraudsterTableAdapter.Fill(this.cyCAPDBDataSet1.Fraudster);
            metroGrid3.DataSource = this.cyCAPDBDataSet1.Fraudster;
        }

        private void fillByWithoutNameToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.customerTableAdapter.FillByWithoutName(this.cyCAPDBDataSet.Customer);
            }
            catch (System.Exception ex)
            {
                MetroMessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void metroGrid1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //Respond to Add Details button
            if (metroGrid1.CurrentCell.ColumnIndex.Equals(2) && e.RowIndex != -1)
            {
                string refID = metroGrid1.Rows[e.RowIndex].Cells[0].Value.ToString();
                AddDetails moreDetails = new AddDetails(refID);

                if (metroGrid1.CurrentCell != null)
                    moreDetails.ShowDialog();

                // TODO: This line of code loads data into the 'cyCAPDBDataSet1.Customer' table. You can move, or remove it, as needed.
                this.customerTableAdapter.FillByWithoutName(this.cyCAPDBDataSet1.Customer);
                // TODO: This line of code loads data into the 'cyCAPDBDataSet.Customer' table. You can move, or remove it, as needed.
                this.customerTableAdapter.FillByWithName(this.cyCAPDBDataSet.Customer);
                metroGrid1.DataSource = this.cyCAPDBDataSet1.Customer;
                metroGrid2.DataSource = this.cyCAPDBDataSet.Customer;
            }


        }

        private void metroGrid3_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //Respond to View Fraudster Pic Button
            if (metroGrid3.CurrentCell.ColumnIndex.Equals(3) && e.RowIndex != -1)
            {
                string refID = metroGrid3.Rows[e.RowIndex].Cells[0].Value.ToString();
                ViewFraudsterPic showPic = new ViewFraudsterPic(refID);

                if (metroGrid3.CurrentCell != null)
                    showPic.ShowDialog();
            }
        }

        private void metroButton4_Click(object sender, EventArgs e)
        {
            srcImage = pictureBox1.Image;

            pictureBox1.Refresh();
            if (pictureBox1.Image != null)
            {

                //Prepare a new Bitmap on which the cropped image will be drawn (From Source)
                Bitmap sourceBitmap = new Bitmap(pictureBox1.Image, pictureBox1.Width, pictureBox1.Height);
                Graphics g = pictureBox1.CreateGraphics();


                ////(....to destination 
                ////Draw the image on the Graphics object with the new dimesions
                //g.DrawImage(sourceBitmap, new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height),
                //    rectCropArea, GraphicsUnit.Pixel);

                //new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height);
                if (rectCropArea.Height != 0 || rectCropArea.Width != 0)
                {
                    // Crop the image:
                    Bitmap cropBmp = sourceBitmap.Clone(rectCropArea, sourceBitmap.PixelFormat);
                    pictureBox1.Image = cropBmp;

                    //Good practice to dispose the System.Drawing objects when not in use.
                    sourceBitmap.Dispose();

                    metroButton4.Enabled = false;
                    metroButton5.Enabled = true;
                }
            }
            else
            {
                MetroMessageBox.Show(this, "Photo Is Required", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Graphics g = pictureBox1.CreateGraphics();
                g.Clear(Color.FromArgb(224,224,224));
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            // Make a note that we "have the mouse".
            bHaveMouse = true;

            // Store the "starting point" for this rubber-band rectangle.
            ptOriginal.X = e.X;
            ptOriginal.Y = e.Y;

            // Special value lets us know that no previous
            // rectangle needs to be erased.

            // Display coordinates
            lbCordinates.Text = "Coordinates  :  " + e.X.ToString() + ", " + e.Y.ToString();

            ptLast.X = -1;
            ptLast.Y = -1;

            rectCropArea = new Rectangle(new Point(e.X, e.Y), new Size());
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            // Set internal flag to know we no longer "have the mouse".
            bHaveMouse = false;

            // If we have drawn previously, draw again in that spot
            // to remove the lines.
            if (ptLast.X != -1)
            {
                Point ptCurrent = new Point(e.X, e.Y);

                // Display coordinates
                lbCordinates.Text = "Coordinates  :  " + ptOriginal.X.ToString() + ", " +
                    ptOriginal.Y.ToString() + " And " + e.X.ToString() + ", " + e.Y.ToString();

            }

            // Set flags to know that there is no "previous" line to reverse.
            ptLast.X = -1;
            ptLast.Y = -1;
            ptOriginal.X = -1;
            ptOriginal.Y = -1;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            Point ptCurrent = new Point(e.X, e.Y);

            // If we "have the mouse", then we draw our lines.
            if (bHaveMouse)
            {
                // If we have drawn previously, draw again in
                // that spot to remove the lines.
                if (ptLast.X != -1)
                {
                    // Display Coordinates
                    lbCordinates.Text = "Coordinates  :  " + ptOriginal.X.ToString() + ", " +
                        ptOriginal.Y.ToString() + " And " + e.X.ToString() + ", " + e.Y.ToString();
                }

                // Update last point.
                ptLast = ptCurrent;

                // Draw new lines.

                // e.X - rectCropArea.X;
                // normal
                if (e.X > ptOriginal.X && e.Y > ptOriginal.Y)
                {
                    rectCropArea.Width = e.X - ptOriginal.X;

                    // e.Y - rectCropArea.Height;
                    rectCropArea.Height = e.Y - ptOriginal.Y;
                }
                else if (e.X < ptOriginal.X && e.Y > ptOriginal.Y)
                {
                    rectCropArea.Width = ptOriginal.X - e.X;
                    rectCropArea.Height = e.Y - ptOriginal.Y;
                    rectCropArea.X = e.X;
                    rectCropArea.Y = ptOriginal.Y;
                }
                else if (e.X > ptOriginal.X && e.Y < ptOriginal.Y)
                {
                    rectCropArea.Width = e.X - ptOriginal.X;
                    rectCropArea.Height = ptOriginal.Y - e.Y;

                    rectCropArea.X = ptOriginal.X;
                    rectCropArea.Y = e.Y;
                }
                else
                {
                    rectCropArea.Width = ptOriginal.X - e.X;

                    // e.Y - rectCropArea.Height;
                    rectCropArea.Height = ptOriginal.Y - e.Y;
                    rectCropArea.X = e.X;
                    rectCropArea.Y = e.Y;
                }
                pictureBox1.Refresh();
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Pen drawLine = new Pen(Color.Black);
            drawLine.DashStyle = DashStyle.Dash;
            e.Graphics.DrawRectangle(drawLine, rectCropArea);
        }

        private void metroButton5_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = srcImage;
            metroButton4.Enabled = true;
            metroButton5.Enabled = false;
        }

        //Searching using textbox
        private void metroTextBox2_TextChanged(object sender, EventArgs e)
        {
            if (metroTextBox2.Text != string.Empty)
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CyCAP.Properties.Settings.CyCAPDBConnectionString"].ConnectionString);
                SqlDataReader dr;

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT Id, FirstName, SecondName, OtherName, ScannedID, Telephone" +
                " FROM Customer " +
                "WHERE  (FirstName LIKE '%' + @Value1 + '%') OR (SecondName LIKE '%' + @Value2 + '%')";
                // Add ADO.NET parameters
                cmd.Parameters.AddWithValue("@Value1", metroTextBox2.Text);
                cmd.Parameters.AddWithValue("@Value2", metroTextBox2.Text);
                // Execute command
                using (con)
                {
                    con.Open();
                    dr = cmd.ExecuteReader();
                    DataTable dt = new DataTable();

                    dt.Load(dr);
                    metroGrid2.DataSource = dt;
                    con.Close();

                    //return dt;
                }
            }
            else
            {
                metroGrid2.DataSource = this.cyCAPDBDataSet.Customer;
            }

            
            
                         
        }
    }
}
