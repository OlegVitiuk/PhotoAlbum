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
    /// <summary>   A peregliad photos. </summary>
    ///
    /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public partial class PeregliadPhotos : Form
    {
        private bool variantOfPokaz;
        private string whatTheTableIs;
        private string dataSource;
        //private BindingSource bindingSource;
        private int pointer;
        private int[] massiveOfCodes;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ///
        /// <param name="variantOfPokaz">   true to variant of pokaz. </param>
        /// <param name="whatTheTableIs">   The what the table is. </param>
        /// <param name="dataSource">       The data source. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public PeregliadPhotos(bool variantOfPokaz,string whatTheTableIs,string dataSource)
        {
            this.variantOfPokaz = variantOfPokaz;
            this.whatTheTableIs = whatTheTableIs;
            this.dataSource = dataSource;
            this.pointer = 0;
            InitializeComponent();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Start load photos and desciption </summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void PeregliadPhotos_Load(object sender, EventArgs e)
        {
            OleDbConnection cn = new OleDbConnection();
            cn.ConnectionString = "Provider=Microsoft.Jet.OleDB.4.0;" + dataSource;
            cn.Open();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = cn;
            cmd.CommandText = String.Format("SELECT Код FROM [{0}]",whatTheTableIs);
            OleDbDataAdapter dap = new OleDbDataAdapter();
            dap.SelectCommand = cmd;
            DataTable dt = new DataTable();
            dap.Fill(dt);
            int sizeOfMassive = dt.Rows.Count;
            int i = 0;
            massiveOfCodes = new int [sizeOfMassive];
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
            cn.Close();
            timer1.Enabled=true;
            timer1.Interval=3000;
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
        /// <summary>   Get photo  </summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void button1_Click(object sender, EventArgs e)
        {
            button3.Enabled = true;
            button1.Enabled = true;
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
                cn.Close();
            }
            else
            {
                button1.Enabled = false;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Go to next image </summary>
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
            if(pointer<massiveOfCodes.Length)
            nextImage();
            else
            {
                button3.Enabled = false;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Next image. </summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void nextImage()
        {
            if (pointer < massiveOfCodes.Length - 1)
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
                cn.Close();
            }
            else
            {
                button3.Enabled = false;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Time to show one picture  </summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void timer1_Tick(object sender, EventArgs e)
        {
            nextImage();
        }

        private void panel1_MouseHover(object sender, EventArgs e)
        {
            //panel1.Visible = true;
        }

        private void panel1_MouseLeave(object sender, EventArgs e)
        {
            
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Stop image changing </summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Start image changing </summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void button4_Click(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            //transparentPanel1.BackColor = Color.Turquoise;
        }

        private void panel1_MouseEnter(object sender, EventArgs e)
        {
            //transparentPanel1.BackColor = Color.Turquoise;
            //trackBar1.Visible = true;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            //timer1.Interval = trackBar1.Value * 1000;
        }

        private void transparentPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void transparentPanel1_MouseMove(object sender, MouseEventArgs e)
        {
            
        }

        private void transparentPanel1_MouseEnter(object sender, EventArgs e)
        {
            
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  Make more faster </summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void button6_Click(object sender, EventArgs e)
        {
            if (timer1.Interval > 1000)
            {
                timer1.Interval -= 1000;
            }
            else
            {
                MessageBox.Show("Зменшити швидкість перемикання слайдів не можливо!", "Мінімальна швидкість перемикання"
                    , MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Make more slower</summary>
        ///
        /// <remarks>   Oleh Vitiuk, 04.06.2016. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void button5_Click(object sender, EventArgs e)
        {
            timer1.Interval += 1000;
        }
        

    }
}
