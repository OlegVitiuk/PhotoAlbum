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
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   A general peregliad. </summary>
    ///
    /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public partial class GeneralPeregliad : Form
    {

        //private bool variantOfPokaz;
        private string whatTheTableIs;
        private string dataSource;
        //private BindingSource bindingSource;
        private int pointer;
        private int[] massiveOfCodes;
        private string[] massiveOfDescriptions;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ///
        /// <param name="whatTheTableIs">   The what the table is. </param>
        /// <param name="dataSource">       The data source. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public GeneralPeregliad(string whatTheTableIs, string dataSource)
        {
            InitializeComponent();
            //this.variantOfPokaz = variantOfPokaz;
            this.whatTheTableIs = whatTheTableIs;
            this.dataSource = dataSource;
            this.pointer = 0;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Go to next image </summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void button3_Click(object sender, EventArgs e)
        {
            button1.Enabled = true;
            button3.Enabled = true;
            if (pointer < massiveOfCodes.Length)
            {
                next();
            }
            else
            {
                button3.Enabled = false;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Move to the next item in the collection. </summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void next()
        {
            button3.Enabled = true;
            if (pointer < massiveOfCodes.Length-1)
            {
                OleDbConnection cn = new OleDbConnection();
                cn.ConnectionString = "Provider=Microsoft.Jet.OleDB.4.0;" + dataSource;
                cn.Open();
                MemoryStream ms;
                byte[] byteArray = null;
                Image image;
                OleDbCommand cmd1 = new OleDbCommand();
                cmd1.Connection = cn;
                cmd1.CommandText = String.Format("SELECT Фото FROM [{0}] where Код={1}", whatTheTableIs, massiveOfCodes[++pointer]);
                OleDbDataReader myDataReader;
                myDataReader = cmd1.ExecuteReader();
                while (myDataReader.Read())
                {
                    byteArray = new byte[((byte[])myDataReader["Фото"]).Length];
                    byteArray = (byte[])myDataReader["Фото"];
                    ms = new MemoryStream(GetImageBytesFromOLEField(byteArray));
                    image = Bitmap.FromStream(ms);
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                    pictureBox1.Image = image;
                }
                textBox1.Text = massiveOfDescriptions[pointer];
                cn.Close();
            }
            else
            {
                button3.Enabled = false;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Load Form - picture,description </summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void GeneralPeregliad_Load(object sender, EventArgs e)
        {
            OleDbConnection cn = new OleDbConnection();
            cn.ConnectionString = "Provider=Microsoft.Jet.OleDB.4.0;" + dataSource;
            cn.Open();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = cn;
            cmd.CommandText = String.Format("SELECT Код FROM [{0}]", whatTheTableIs);
            OleDbDataAdapter dap = new OleDbDataAdapter();
            dap.SelectCommand = cmd;
            DataTable dt = new DataTable();
            dap.Fill(dt);
            int sizeOfMassive = dt.Rows.Count;
            int i = 0;
            massiveOfCodes = new int[sizeOfMassive];
            foreach (DataRow row in dt.Rows)
            {
                massiveOfCodes[i++] = Convert.ToInt32(row[0]);
            }
            MemoryStream ms;
            byte[] byteArray = null;
            Image image;
            OleDbCommand cmd1 = new OleDbCommand();
            cmd1.Connection = cn;
            cmd1.CommandText = String.Format("SELECT Фото FROM [{0}] where Код={1}", whatTheTableIs, massiveOfCodes[pointer]);
            OleDbDataReader myDataReader;
            myDataReader = cmd1.ExecuteReader();
            while (myDataReader.Read())
            {
                byteArray = new byte[((byte[])myDataReader["Фото"]).Length];
                byteArray = (byte[])myDataReader["Фото"];
                ms = new MemoryStream(GetImageBytesFromOLEField(byteArray));
                image = Bitmap.FromStream(ms);
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox1.Image = image;
            }
            OleDbCommand cmd2 = new OleDbCommand();
            cmd2.Connection = cn;
            cmd2.CommandText = String.Format("SELECT Опис FROM [{0}]", whatTheTableIs);
            OleDbDataAdapter dap1 = new OleDbDataAdapter();
            dap1.SelectCommand = cmd2;
            DataTable dt1 = new DataTable();
            dap1.Fill(dt1);
            int sizeOfMassive1 = dt1.Rows.Count;
            int i1 = 0;
            massiveOfDescriptions = new string[sizeOfMassive1];
            foreach (DataRow row in dt1.Rows)
            {
                massiveOfDescriptions[i1++] = row[0].ToString();
            }
            textBox1.Text = massiveOfDescriptions[pointer];
            cn.Close();
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
        /// <summary>   Go to the previous image </summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = true;
            button3.Enabled = true;
            if (pointer > 0)
            {
                OleDbConnection cn = new OleDbConnection();
                cn.ConnectionString = "Provider=Microsoft.Jet.OleDB.4.0;" + dataSource;
                cn.Open();
                MemoryStream ms;
                byte[] byteArray = null;
                Image image;
                OleDbCommand cmd1 = new OleDbCommand();
                cmd1.Connection = cn;
                cmd1.CommandText = String.Format("SELECT Фото FROM [{0}] where Код={1}", whatTheTableIs, massiveOfCodes[--pointer]);
                OleDbDataReader myDataReader;
                myDataReader = cmd1.ExecuteReader();
                while (myDataReader.Read())
                {
                    byteArray = new byte[((byte[])myDataReader["Фото"]).Length];
                    byteArray = (byte[])myDataReader["Фото"];
                    ms = new MemoryStream(GetImageBytesFromOLEField(byteArray));
                    image = Bitmap.FromStream(ms);
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                    pictureBox1.Image = image;
                }
                textBox1.Text = massiveOfDescriptions[pointer];
                cn.Close();
            }
            else
            {
                button1.Enabled = false;
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

    }
}
