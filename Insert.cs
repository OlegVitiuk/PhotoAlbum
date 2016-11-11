using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace CourseWorkFinal
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   An insert form  </summary>
    ///
    /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public partial class Insert : Form
    {
        public deleg B;
        public getTableNames C;
        Image myimage;
        public bool openFile;
        public Insert(deleg A, getTableNames A1)
        {
            InitializeComponent();
            B = A;
            C = A1;
            openFile = false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Choose image from folder </summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void button1_Click(object sender, EventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = @"c:\";
            openFileDialog1.Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                            Image myimage = new Bitmap(openFileDialog1.FileName);
                            pictureBox1.Image = myimage;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error : Could not read file from disk:	" + ex.Message);
                }
            }

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Open image from folder </summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void button1_Click_1(object sender, EventArgs e)
        {
            //button4.Enabled = true;
            openFile = true;
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = @"c:\";
            openFileDialog1.Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                            myimage = new Bitmap(openFileDialog1.FileName);
                            pictureBox1.Image = myimage;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error : Could not read file from disk:	" + ex.Message);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Search image in internet using chrome </summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("Chrome.exe");
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error : Could not read file from disk:	" + ex.Message);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Search image in internet using firefox </summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("Firefox.exe");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : Could not read file from disk:	" + ex.Message);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Add data to dataBase </summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void button4_Click(object sender, EventArgs e)
        {
            if (openFile==true)
            {
                B(textBox1.Text, myimage, comboBox1.SelectedItem.ToString());
                MessageBox.Show("Дані успішно додані", "Додавання данних", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Завантажте картинку!", "Не введені всі данні", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Fill names of tables to insert </summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Insert_Load(object sender, EventArgs e)
        {
            List<String> tableNames = C();
            foreach (string item in tableNames)
            {
                comboBox1.Items.Add(item);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Enables button4</summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button4.Enabled = true;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
