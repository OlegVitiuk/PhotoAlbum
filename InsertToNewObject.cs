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
using ADOX;
using System.Data.OleDb;
using System.Data.Common;

namespace CourseWorkFinal
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   An insert to new object. </summary>
    ///
    /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public partial class InsertToNewObject : Form
    {
        public deleg B;
        private string dataS;
        Image myimage;
        public bool openFile;
        public InsertToNewObject(deleg A,string dataSource)
        {
            InitializeComponent();
            B = A;
            this.dataS = dataSource;
            openFile = false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Makes visible name of dataBase </summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if(radioButton1.Checked ==true)
            {
                textBox2.Visible = true;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Creates a new dataBase and table </summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void button4_Click(object sender, EventArgs e)
        {
            if(openFile==true)
            {
                if (radioButton1.Checked == true)
                {
                    FolderBrowserDialog FBD = new FolderBrowserDialog();
                    if (FBD.ShowDialog() == DialogResult.OK)
                    {
                        Catalog catalog = new Catalog();
                        int codeNumberToInsert = 1;
                        string dataSource = FBD.SelectedPath + String.Format("\\{0}.mdb", textBox2.Text);
                        catalog.Create("provider=microsoft.jet.oledb.4.0; data source=" + dataSource);
                        OleDbConnection cn = new OleDbConnection();
                        cn.ConnectionString = "Provider=Microsoft.Jet.OleDB.4.0;Data Source=" + dataSource;
                        cn.Open();

                        OleDbCommand cmdCreateTable1 = new OleDbCommand(String.Format("CREATE TABLE [{0}] (Код INT PRIMARY KEY, Фото IMAGE, Опис TEXT);", textBox3.Text));
                        cmdCreateTable1.Connection = cn;
                        //посылаем запрос
                        try
                        {
                            cmdCreateTable1.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                            MessageBox.Show("Таблица  НЕ создана");
                            return;
                        }

                        try
                        {
                            OleDbConnection cn2 = new OleDbConnection();
                            cn2.ConnectionString = "Provider=Microsoft.Jet.OleDB.4.0;Data Source=" + dataSource;
                            cn2.Open();
                            OleDbCommand cmd = new OleDbCommand();
                            cmd.Connection = cn2;
                            cmd.CommandText = String.Format("INSERT INTO [{0}] values(?,?,?)", textBox3.Text);
                            //OleDbParameter oleDbParameter = new OleDbParameter("image", OleDbType.VarBinary);
                            MemoryStream memoryStream = new MemoryStream();//Параметр запроса
                            myimage.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);                                                     //Сохраняем изображение в поток.                   
                            cmd.Parameters.AddWithValue("@A", codeNumberToInsert);
                            cmd.Parameters.AddWithValue("@image", memoryStream.ToArray());
                            cmd.Parameters.AddWithValue("@K", textBox1.Text);
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Операція виконана успішно!", "Результат виконання операції", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Close();
                            cn.Close();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Невідома помилка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    int codeNumberToInsert = 1;
                    OleDbConnection cn = new OleDbConnection();
                    MessageBox.Show(dataS);
                    cn.ConnectionString = "Provider=Microsoft.Jet.OleDB.4.0;" + dataS;
                    cn.Open();

                    OleDbCommand cmdCreateTable1 = new OleDbCommand(String.Format("CREATE TABLE [{0}] (Код INT PRIMARY KEY, Фото IMAGE, Опис TEXT);", textBox3.Text));
                    cmdCreateTable1.Connection = cn;
                    //посылаем запрос
                    try
                    {
                        cmdCreateTable1.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                        MessageBox.Show("Таблица  НЕ создана");
                        return;
                    }

                    try
                    {
                        OleDbConnection cn2 = new OleDbConnection();
                        cn2.ConnectionString = "Provider=Microsoft.Jet.OleDB.4.0;" + dataS;
                        cn2.Open();
                        OleDbCommand cmd = new OleDbCommand();
                        cmd.Connection = cn2;
                        cmd.CommandText = String.Format("INSERT INTO [{0}] values(?,?,?)", textBox3.Text);
                        //OleDbParameter oleDbParameter = new OleDbParameter("image", OleDbType.VarBinary);
                        MemoryStream memoryStream = new MemoryStream();//Параметр запроса
                        myimage.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);                                                     //Сохраняем изображение в поток.                   
                        cmd.Parameters.AddWithValue("@A", codeNumberToInsert);
                        cmd.Parameters.AddWithValue("@image", memoryStream.ToArray());
                        cmd.Parameters.AddWithValue("@K", textBox1.Text);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Операція виконана успішно!", "Результат виконання операції", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                        cn.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Невідома помилка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
               MessageBox.Show("Завантажте картинку!", "Не введені всі данні", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Open image files </summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void button1_Click(object sender, EventArgs e)
        {
            openFile = true;
            button4.Enabled = true;
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
        /// <summary>   Load image from the internet using Google Chrome </summary>
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
            catch (Exception ex)
            {
                MessageBox.Show("Error : Could not read file from disk:	" + ex.Message);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Load image from the internet using Firefox </summary>
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
    }
}
