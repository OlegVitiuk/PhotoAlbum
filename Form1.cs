using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO;
using System.Data.Common;

namespace CourseWorkFinal
{
    public delegate void deleg(String desciption, Image image,String WhatTableToInsert);
    public delegate List<String> getTableNames();

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>  Main Form  </summary>
    ///
    /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public partial class Form1 : Form
    {
        //private string dataSource = "Data Source = E:\\1.mdb";
        private string dataSource;
        private string whatTheTableIs;
        deleg A;
        TreeNode album;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public Form1()
        {
            InitializeComponent();
            A = new deleg(this.InsertionToDataBase);
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            OleDbConnection cn = new OleDbConnection();
            cn.ConnectionString = "Provider=Microsoft.Jet.OleDB.4.0;" + dataSource;
            cn.Open();
            DataTable schemaTables = cn.GetSchema("Tables", new[] { null, null, null, "TABLE" });
            List<String> tableNames = new List<string>();
            tableNames.AddRange(from DataRow item in schemaTables.Rows select item["TABLE_NAME"].ToString());
            foreach (String tableName in tableNames)
            {
                createTree(tableName);
            }
        }

        private void toolStripMenuItem2_Click_1(object sender, EventArgs e)
        {

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Creates a tree nodes </summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ///
        /// <param name="albumName">    Name of the album. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void createTree(string albumName)
        {
            album = new TreeNode();
            treeView1.Nodes.Add(albumName);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Returns foreground color. </summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void returnForeColor()
        {
            
            foreach(TreeNode album in treeView1.Nodes)
            {
                album.ForeColor = Color.Black;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Enter items to comboBoxes </summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Tree view event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            comboBox3.Items.Clear();
            returnForeColor();
            DataGridViewColumn currentColumn;
            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {
                    currentColumn = dataGridView1.Columns[i];
                    comboBox1.Items.Add(currentColumn.HeaderText);
                    comboBox2.Items.Add(currentColumn.HeaderText);
            }
            e.Node.ForeColor = Color.Red;

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Fill datagridView code,description </summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Tree node mouse click event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            OleDbConnection cn = new OleDbConnection();
            cn.ConnectionString = "Provider=Microsoft.Jet.OleDB.4.0;" + dataSource;
            cn.Open();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = cn;
            cmd.CommandText = String.Format("SELECT Код,Опис FROM [{0}]",e.Node.Text);
            whatTheTableIs = e.Node.Text;
            OleDbDataAdapter dap = new OleDbDataAdapter();
            dap.SelectCommand = cmd;
            DataTable dt = new DataTable();
            dap.Fill(dt);
            dataGridView1.DataSource = dt;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets data source. </summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ///
        /// <returns>   The data source. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private static string getDataSource()
        {
            string dataSource = " ";
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = @"c:\";
            openFileDialog1.Filter = "DataBase files (*.mdb)|*.mdb";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            dataSource = String.Format(@"Data Source = {0}", openFileDialog1.FileName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error : Could not read file from disk:	" + ex.Message);
                }
            }
            return dataSource;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Select a photo</summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void dataGridView1_Click(object sender, EventArgs e)
        {
            MemoryStream ms;
            byte[] byteArray = null;
            Image image; 
            OleDbConnection cn = new OleDbConnection();
            cn.ConnectionString = "Provider=Microsoft.Jet.OleDB.4.0;" + dataSource;
            cn.Open();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = cn;
            cmd.CommandText = String.Format("SELECT Фото FROM [{0}] where Код={1}", whatTheTableIs,dataGridView1.SelectedRows[0].Cells[0].Value);
            OleDbDataReader myDataReader;
            myDataReader = cmd.ExecuteReader();
            while (myDataReader.Read())
            {
                byteArray = new byte[((byte[])myDataReader["Фото"]).Length];
                byteArray = (byte[])myDataReader["Фото"];
                ms = new MemoryStream(GetImageBytesFromOLEField(byteArray));
                image = Bitmap.FromStream(ms);
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox1.Image = image;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Cell photo fill</summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Data grid view cell event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            MemoryStream ms;
            byte[] byteArray = null;
            Image image; 
            OleDbConnection cn = new OleDbConnection();
            cn.ConnectionString = "Provider=Microsoft.Jet.OleDB.4.0;" + dataSource;
            cn.Open();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = cn;
            cmd.CommandText = String.Format("SELECT Фото FROM [{0}] where Код={1}", whatTheTableIs,dataGridView1.SelectedRows[0].Cells[0].Value);
            OleDbDataReader myDataReader;
            myDataReader = cmd.ExecuteReader();
            while (myDataReader.Read())
            {
                byteArray = new byte[((byte[])myDataReader["Фото"]).Length];
                byteArray = (byte[])myDataReader["Фото"];
                ms = new MemoryStream(GetImageBytesFromOLEField(byteArray));
                image = Bitmap.FromStream(ms);
                //pictureBox1.BackgroundImage = image;

            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets image bytes from OLE field. </summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ///
        /// <exception cref="Exception">    Thrown when an exception error condition occurs. </exception>
        ///
        /// <param name="oleFieldBytes">    The OLE field in bytes. </param>
        ///
        /// <returns>   An array of byte. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private byte[] GetImageBytesFromOLEField(byte[] oleFieldBytes)
        {

            const string BITMAP_ID_BLOCK = "BM";

            const string JPG_ID_BLOCK = "\u00FF\u00D8\u00FF";

            const string PNG_ID_BLOCK = "\u0089PNG\r\n\u001a\n";

            const string GIF_ID_BLOCK = "GIF8";

            const string TIFF_ID_BLOCK = "II*\u0000";



            byte[] imageBytes;



            // Get a UTF7 Encoded string version

            Encoding u8 = Encoding.UTF7;

            string strTemp = u8.GetString(oleFieldBytes);



            // Get the first 300 characters from the string

            string strVTemp = strTemp.Substring(0, 300);



            // Search for the block

            int iPos = -1;

            if (strVTemp.IndexOf(BITMAP_ID_BLOCK) != -1)

                iPos = strVTemp.IndexOf(BITMAP_ID_BLOCK);

            else if (strVTemp.IndexOf(JPG_ID_BLOCK) != -1)

                iPos = strVTemp.IndexOf(JPG_ID_BLOCK);

            else if (strVTemp.IndexOf(PNG_ID_BLOCK) != -1)

                iPos = strVTemp.IndexOf(PNG_ID_BLOCK);

            else if (strVTemp.IndexOf(GIF_ID_BLOCK) != -1)

                iPos = strVTemp.IndexOf(GIF_ID_BLOCK);

            else if (strVTemp.IndexOf(TIFF_ID_BLOCK) != -1)

                iPos = strVTemp.IndexOf(TIFF_ID_BLOCK);

            else

                throw new Exception("Unable to determine header size for the OLE Object");



            // From the position above get the new image

            if (iPos == -1)

                throw new Exception("Unable to determine header size for the OLE Object");



            //Array.Copy(

            imageBytes = new byte[oleFieldBytes.LongLength - iPos];

            MemoryStream ms = new MemoryStream();

            ms.Write(oleFieldBytes, iPos, oleFieldBytes.Length - iPos);

            imageBytes = ms.ToArray();

            ms.Close();

            ms.Dispose();

            return imageBytes;

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Form Load image </summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox2.Image = Resources.Resource1._83562121_82320235_large_tumblr_lxmeh7Za3F1qbi7l7o1_500_large;
            textBox2.BackColor = Form1.DefaultBackColor;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Create names for nods(albums of photo) </summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            try
            {
                this.dataSource = getDataSource();


                toolStripMenuItem11.Enabled = true;
                toolStripMenuItem12.Enabled = true;
                textBox2.Visible = false;
                dataGridView1.Visible = true;
                pictureBox2.Visible = false;
                toolStripMenuItem8.Enabled = true;
                toolStripMenuItem7.Enabled = true;
                treeView1.Nodes.Clear();
                comboBox1.Items.Clear();
                comboBox2.Items.Clear();
                comboBox3.Items.Clear();
                dataGridView1.DataSource = null;
                groupBox2.Enabled = true;
                OleDbConnection cn = new OleDbConnection();
                cn.ConnectionString = "Provider=Microsoft.Jet.OleDB.4.0;" + dataSource;
                cn.Open();
                DataTable schemaTables = cn.GetSchema("Tables", new[] { null, null, null, "TABLE" });
                List<String> tableNames = new List<string>();
                tableNames.AddRange(from DataRow item in schemaTables.Rows select item["TABLE_NAME"].ToString());
                foreach (String tableName in tableNames)
                {
                    createTree(tableName);
                }
                toolStripMenuItem6.Enabled = false;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(),"Невідома помилка!");
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Fill comboBox3  </summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string stringTocompare;
            //comboBox2.Items.Clear();
            comboBox3.Items.Clear();
            for (int i = 0; i < dataGridView1.RowCount - 1; i++)
            {
                stringTocompare = dataGridView1[comboBox2.SelectedIndex, i].Value.ToString();
                if (comboBox2.Items.Contains(stringTocompare))
                {
                    continue;
                }
                else
                {
                    comboBox3.Items.Add(stringTocompare);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sort dataGridView </summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            if (radioButton1.Checked == true)
            {
                dataGridView1.Sort(dataGridView1.Columns[comboBox1.SelectedIndex], ListSortDirection.Ascending);
            }
            else if (radioButton2.Checked == true)
            {
                dataGridView1.Sort(dataGridView1.Columns[comboBox1.SelectedIndex], ListSortDirection.Descending);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Filter dataGridView </summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            OleDbConnection cn = new OleDbConnection();
            cn.ConnectionString = "Provider=Microsoft.Jet.OleDB.4.0;" + dataSource;
            cn.Open();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = cn;
            cmd.CommandText = String.Format("SELECT Код,Опис FROM [{0}]", whatTheTableIs);
            OleDbDataAdapter dap = new OleDbDataAdapter();
            dap.SelectCommand = cmd;
            DataTable dt = new DataTable();
            dap.Fill(dt);
            BindingSource bindingSource = new BindingSource();
            dataGridView1.DataSource = dt;
            bindingSource.DataSource = dt;
            bindingSource.Filter = String.Format("[{0}] = '{1}'", comboBox2.Text, comboBox3.Text);
            cn.Close();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Search data </summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            OleDbConnection cn = new OleDbConnection();
            cn.ConnectionString = "Provider=Microsoft.Jet.OleDB.4.0;" + dataSource;
            cn.Open();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = cn;
            cmd.CommandText = String.Format("SELECT Код,Опис FROM [{0}] where [Код] like ? or [Опис] like ?", whatTheTableIs);
            cmd.Parameters.AddWithValue("@A", textBox1.Text + "%");
            cmd.Parameters.AddWithValue("@B", textBox1.Text + "%");

            OleDbDataAdapter dap = new OleDbDataAdapter();
            dap.SelectCommand = cmd;
            DataTable dt = new DataTable();
            dap.Fill(dt);
            dataGridView1.DataSource = dt;
            cn.Close();
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem3_Click_1(object sender, EventArgs e)
        {
            
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Insert to current table </summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            //A = new deleg(this.InsertionToDataBase);
            getTableNames A1 = new getTableNames(this.getTableNames);
            Insert insertForm = new Insert(A,A1);
            insertForm.Show();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Insertion to data base. </summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ///
        /// <param name="desciption">           The desciption. </param>
        /// <param name="image">                The image. </param>
        /// <param name="WhatTableToInsert">    The what table to insert. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public void InsertionToDataBase(String desciption,Image image,String WhatTableToInsert)
        {
                int[] arr = new int[dataGridView1.RowCount - 1];
                for (int i = 0; i < dataGridView1.RowCount - 1; i++)
                    arr[i] = Convert.ToInt32(dataGridView1[0, i].Value);
                int codeNumberToInsert = arr.Max<int>();
                codeNumberToInsert++;

                try
                {
                    OleDbConnection cn = new OleDbConnection();
                    cn.ConnectionString = "Provider=Microsoft.Jet.OleDB.4.0;" + dataSource;
                    cn.Open();
                    OleDbCommand cmd = new OleDbCommand();
                    cmd.Connection = cn;
                    cmd.CommandText = String.Format("INSERT INTO [{0}] values(?,?,?)", WhatTableToInsert);
                    //OleDbParameter oleDbParameter = new OleDbParameter("image", OleDbType.VarBinary);
                    MemoryStream memoryStream = new MemoryStream();//Параметр запроса
                    image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);                                                     //Сохраняем изображение в поток.
                    //oleDbParameter.Value = memoryStream.ToArray();                     
                    cmd.Parameters.AddWithValue("@A", codeNumberToInsert);
                    cmd.Parameters.AddWithValue("@image", memoryStream.ToArray());
                    cmd.Parameters.AddWithValue("@K", desciption);
                    cmd.ExecuteNonQuery();
                    cn.Close();
                }
            catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "Невідома помилка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                OleDbConnection cn1 = new OleDbConnection();
                cn1.ConnectionString = "Provider=Microsoft.Jet.OleDB.4.0;" + dataSource;
                cn1.Open();
                OleDbCommand cmd1 = new OleDbCommand();
                cmd1.Connection = cn1;
                cmd1.CommandText = String.Format("SELECT Код,Опис FROM [{0}]", WhatTableToInsert);
                OleDbDataAdapter dap = new OleDbDataAdapter();
                dap.SelectCommand = cmd1;
                DataTable dt = new DataTable();
                dap.Fill(dt);
                dataGridView1.DataSource = dt;
                cn1.Close();
              
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets table names. </summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ///
        /// <returns>   The table names. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public List<String> getTableNames()
        {
            OleDbConnection cn = new OleDbConnection();
            cn.ConnectionString = "Provider=Microsoft.Jet.OleDB.4.0;" + dataSource;
            cn.Open();
            DataTable schemaTables = cn.GetSchema("Tables", new[] { null, null, null, "TABLE" });
            List<String> tableNames = new List<string>();
            tableNames.AddRange(from DataRow item in schemaTables.Rows select item["TABLE_NAME"].ToString());
            return tableNames;
        }

        private void toolStripMenuItem1_Click_1(object sender, EventArgs e)
        {

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Delete a data from dataBase </summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            OleDbConnection cn = new OleDbConnection();
            cn.ConnectionString = "Provider=Microsoft.Jet.OleDB.4.0;" + dataSource;
            cn.Open();
            if (MessageBox.Show("Are you sure delete this row?",
   "Deleting", MessageBoxButtons.YesNo,
   MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = cn;
                cmd.CommandText = String.Format("DELETE FROM [{0}] WHERE Код=?",whatTheTableIs);
                cmd.Parameters.AddWithValue("@id", dataGridView1.SelectedRows[0].Cells[0].Value);
                cmd.ExecuteNonQuery();
                cn.Close();


                OleDbConnection cn1 = new OleDbConnection();
                cn1.ConnectionString = "Provider=Microsoft.Jet.OleDB.4.0;" + dataSource;
                cn1.Open();
                OleDbCommand cmd1 = new OleDbCommand();
                cmd1.Connection = cn1;
                cmd1.CommandText = String.Format("SELECT Код,Опис FROM [{0}]", whatTheTableIs);
                OleDbDataAdapter dap = new OleDbDataAdapter();
                dap.SelectCommand = cmd1;
                DataTable dt = new DataTable();
                dap.Fill(dt);
                dataGridView1.DataSource = dt;
                cn1.Close();
            }
        }

        private void treeView1_MouseMove(object sender, MouseEventArgs e)
        {
            
           
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Insert to new table or data Base </summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void toolStripMenuItem9_Click(object sender, EventArgs e)
        {
            InsertToNewObject newAlbum = new InsertToNewObject(A,dataSource);
            newAlbum.Show();
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Slide-show </summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void toolStripMenuItem11_Click(object sender, EventArgs e)
        {

            try
            { 
            PeregliadPhotos peregliad = new PeregliadPhotos(true,whatTheTableIs,dataSource);
            peregliad.Show();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   General View </summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void toolStripMenuItem12_Click(object sender, EventArgs e)
        {
            GeneralPeregliad gp = new GeneralPeregliad(whatTheTableIs,dataSource);
            gp.Show();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Information about developer </summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void toolStripMenuItem13_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Виконав Вітюк Олег", "Інформація про розробника", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void toolStripMenuItem14_Click(object sender, EventArgs e)
        {
           //helpProvider1.HelpNamespace = Resources.Resource2.InstructionOfUser.ToString();
            //Help.ShowHelp(helpProvider1,);
        }
    }
}
