using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using System.Management;
using System.Drawing.Imaging;
using System.Windows.Forms.DataVisualization.Charting;

namespace SinavaHazirlik.cs
{
    public class csDBIslemleri
    {
        public static SqlConnection connection;

        public static SqlConnection GetConnection()
        {
            try
            {
                if (connection == null)
                    connection = new SqlConnection(@"Data Source=;Initial Catalog=; User Id=;Password=;");
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

            }
            catch (Exception error)
            {
                System.Windows.Forms.MessageBox.Show("StackTrace" + Environment.NewLine + error.StackTrace.ToString());
                System.Windows.Forms.MessageBox.Show("Message" + Environment.NewLine + error.Message.ToString());
            }
            return connection;
        }

        public static DataTable GetDataTable(string sqlSentence)
        {
            connection = GetConnection();

            DataTable dt = new DataTable();

            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                using (SqlDataAdapter da = new SqlDataAdapter(sqlSentence, connection))
                {
                    if (da != null)
                    {
                        dt.Clear();
                        da.Fill(dt);
                    }
                    da.Dispose();
                }
                connection.Close();
            }
            catch (Exception error)
            {
                connection.Close();
                throw new Exception(error.Message);
            }
            return dt;
        }

        public static string DeleteElement(string tableName, string afterWhere, string controlColumn, string controlValue, SqlDbType[] types)
        {
            connection = GetConnection();
            string result = "yes";

            string[] columns = controlColumn.Split(new string[] { "½" }, StringSplitOptions.RemoveEmptyEntries);
            string[] values = controlValue.Split(new string[] { "½" }, StringSplitOptions.RemoveEmptyEntries);

            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                string sqlSentence = "Delete From " + tableName + " where " + afterWhere;

                using (SqlCommand cmd = new SqlCommand(sqlSentence, connection))
                {
                    for (int i = 0; i < columns.Length; i++)
                    {
                        cmd.Parameters.Add("@" + columns[i], types[i]).Value = values[i];
                    }
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }

                connection.Close();
            }
            catch (Exception error)
            {
                connection.Close();
                result = error.Message;
            }
            return result;
        }

        public static string AddOrUpdateElement(string tableName, string afterWhere, bool add, string addColumns, string addValues, SqlDbType[] addTypes, string controlColumns, string controlValues, SqlDbType[] controlTypes)
        {
            connection = GetConnection();
            string result = "yes";

            string[] addColumns = addColumns.Split(new string[] { "½" }, StringSplitOptions.RemoveEmptyEntries);
            string[] addValues = addValues.Split(new string[] { "½" }, StringSplitOptions.RemoveEmptyEntries);
            string[] controlColumns = controlColumns.Split(new string[] { "½" }, StringSplitOptions.RemoveEmptyEntries);
            string[] controlValues = controlValues.Split(new string[] { "½" }, StringSplitOptions.RemoveEmptyEntries);

            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                string sqlSentence = "";
                if (add)
                {
                    sqlSentence = "Insert Into " + tableName + "(";
                    for (int i = 0; i < addColumns.Length; i++)
                    {
                        sqlSentence += addColumns[i] + ",";
                    }
                    sqlSentence = sqlSentence.Trim(',');
                    sqlSentence += ") values(";
                    for (int i = 0; i < addColumns.Length; i++)
                    {
                        sqlSentence += "@" + addColumns[i] + ",";
                    }
                    sqlSentence = sqlSentence.Trim(',');
                    sqlSentence += ")";
                }
                else
                {
                    sqlSentence = "Update " + tableName + " Set ";
                    for (int i = 0; i < addColumns.Length; i++)
                    {
                        sqlSentence += addColumns[i] + "=@" + addColumns[i] + ",";
                    }
                    sqlSentence = sqlSentence.Trim(',');
                    sqlSentence += " where " + afterWhere;
                }

                using (SqlCommand cmd = new SqlCommand(sqlSentence, connection))
                {
                    if (!add)
                    {
                        for (int i = 0; i < controlColumns.Length; i++)
                        {
                            cmd.Parameters.Add("@" + controlColumns[i], controlTypes[i]).Value = controlValues[i];
                        }
                    }
                    for (int i = 0; i < addColumns.Length; i++)
                    {
                        cmd.Parameters.Add("@" + addColumns[i], addTypes[i]).Value = addValues[i];
                    }
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
                connection.Close();
            }
            catch (Exception error)
            {
                connection.Close();
                result = error.Message;
            }
            return result;
        }

        public static Dictionary<string, string> GetObjectInformations(string tableName, string getColumns, string afterWhere, string controlColumns, string controlValues, SqlDbType[] types)
        {
            connection = GetConnection();
            Dictionary<string, string> informations = new Dictionary<string, string>();

            string[] readColumns = getColumns.Split(new string[] { "½" }, StringSplitOptions.RemoveEmptyEntries);
            string[] controlColumns = controlColumns.Split(new string[] { "½" }, StringSplitOptions.RemoveEmptyEntries);
            string[] controlValues = controlValues.Split(new string[] { "½" }, StringSplitOptions.RemoveEmptyEntries);

            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                string sqlSentence = "Select ";
                for (int i = 0; i < readColumns.Length; i++)
                {
                    sqlSentence += readColumns[i] + ",";
                }
                sqlSentence = sqlSentence.Trim(',');
                sqlSentence += " from " + tableName + " where " + afterWhere;

                using (SqlCommand cmd = new SqlCommand(sqlSentence, connection))
                {
                    for (int i = 0; i < controlColumns.Length; i++)
                    {
                        cmd.Parameters.Add("@" + controlColumns[i], types[i]).Value = controlValues[i];
                    }
                    using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        if (dr.Read())
                        {
                            for (int i = 0; i < readColumns.Length; i++)
                            {
                                informations.Add(readColumns[i], dr[readColumns[i]].ToString());
                            }
                        }
                        dr.Close();
                    }
                    cmd.Dispose();
                }
                connection.Close();
            }
            catch (Exception error)
            {
                connection.Close();
                throw new Exception(error.Message);
            }
            return informations;
        }

        public static bool IsRegistered(string tableName, string afterWhere, string controlColumns, string controlValues, SqlDbType[] types)
        {
            connection = GetConnection();
            bool register = false;

            string[] controlColumns = controlColumns.Split(new string[] { "½" }, StringSplitOptions.RemoveEmptyEntries);
            string[] controlValues = controlValues.Split(new string[] { "½" }, StringSplitOptions.RemoveEmptyEntries);

            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                string sqlSentence = "Select * from " + tableName + " where " + afterWhere;

                using (SqlCommand cmd = new SqlCommand(sqlSentence, connection))
                {
                    for (int i = 0; i < controlColumns.Length; i++)
                    {
                        cmd.Parameters.Add("@" + controlColumns[i], types[i]).Value = controlValues[i];
                    }
                    using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        if (dr.Read())
                            register = true;

                        dr.Close();
                    }
                    cmd.Dispose();
                }
                connection.Close();
            }
            catch (Exception error)
            {
                connection.Close();
                throw new Exception(error.Message);
            }

            return register;
        }

        public static string IsUnique(string tableName, string readColumn, string afterWhere, string controlColumn, string controlValue, SqlDbType[] controlTypes)
        {
            connection = GetConnection();
            string value = "";

            string[] controlColumns = controlColumn.Split(new string[] { "½" }, StringSplitOptions.RemoveEmptyEntries);
            string[] controlValues = controlValue.Split(new string[] { "½" }, StringSplitOptions.RemoveEmptyEntries);

            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                string sqlSentence = "Select " + readColumn + " from " + tableName + " where " + afterWhere;

                using (SqlCommand cmd = new SqlCommand(sqlSentence, connection))
                {
                    for (int i = 0; i < controlColumns.Length; i++)
                    {
                        cmd.Parameters.Add("@" + controlColumns[i], controlTypes[i]).Value = controlValues[i];
                    }
                    using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        if (dr.Read())
                            value = dr[readColumn].ToString();

                        dr.Close();
                    }
                    cmd.Dispose();
                }

                connection.Close();
            }
            catch (Exception error)
            {
                connection.Close();
                throw new Exception(error.Message);
            }

            return value;
        }

        public static DataTable GetDataTableControlly(string sqlSentence, string controlColumns, string controlValues, SqlDbType[] controlTypes)
        {
            connection = GetConnection();
            DataTable dt = new DataTable();
            string[] controlColumns = controlColumns.Split(new string[] { "½" }, StringSplitOptions.RemoveEmptyEntries);
            string[] controlValues = controlValues.Split(new string[] { "½" }, StringSplitOptions.RemoveEmptyEntries);
            SqlDataAdapter da = new SqlDataAdapter();
            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();


                using (da.SelectCommand = new SqlCommand(sqlSentence, connection))
                {
                    for (int i = 0; i < controlColumns.Length; i++)
                    {
                        da.SelectCommand.Parameters.Add("@" + controlColumns[i], controlTypes[i]).Value = controlValues[i];
                    }
                    dt.Clear();
                    da.Fill(dt);
                }

                da.Dispose();
                connection.Close();
            }
            catch (Exception error)
            {
                da.Dispose();
                connection.Close();
                throw new Exception(error.Message);
            }

            return dt;
        }

        public static List<string> GetList(string tableName, string getColumn)
        {
            connection = GetConnection();
            List<string> liste = new List<string>();


            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                string sqlSentence = "Select " + getColumn + " From " + tableName + " Order By " + getColumn;

                using (SqlCommand cmd = new SqlCommand(sqlSentence, connection))
                {
                    using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        while (dr.Read())
                        {
                            liste.Add(dr[getColumn].ToString());
                        }
                        dr.Close();
                    }
                    cmd.Dispose();
                }
                connection.Close();

            }
            catch (Exception error)
            {
                connection.Close();
                throw new Exception(error.Message);
            }

            return liste;
        }

        public static string AddWithDataTable(string tableName, DataTable dt, SqlDbType[] types)
        {
            connection = GetConnection();
            string result = "yes";
            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                string[] columns = new string[dt.Columns.Count];
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    columns[i] = dt.Columns[i].ColumnName.ToString();
                }

                string sqlSentence = "Insert Into " + tableName + "(";
                for (int i = 0; i < columns.Length; i++)
                {
                    sqlSentence += columns[i] + ",";
                }
                sqlSentence = sqlSentence.Trim(',');
                sqlSentence += ") values(";

                for (int i = 0; i < columns.Length; i++)
                {
                    sqlSentence += "@" + columns[i] + ",";
                }
                sqlSentence = sqlSentence.Trim(',');
                sqlSentence += ")";

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    using (SqlCommand cmd = new SqlCommand(sqlSentence, connection))
                    {
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            cmd.Parameters.Add("@" + dt.Columns[j].ColumnName, types[j]).Value = dt.Rows[i].ItemArray[j].ToString();
                        }
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                }

                connection.Close();
            }
            catch (Exception error)
            {
                connection.Close();
                result = error.Message;
            }

            return result;
        }

        public static List<string> GetListControlly(string tableName, string afterWhere, string getColumn, string controlColumn, string controlValue, SqlDbType[] type)
        {
            connection = GetConnection();
            List<string> list = new List<string>();
            string[] controlColumn = controlColumn.Split(new string[] { "½" }, StringSplitOptions.RemoveEmptyEntries);
            string[] controlValue = controlValue.Split(new string[] { "½" }, StringSplitOptions.RemoveEmptyEntries);

            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                string sqlSentence = "Select " + getColumn + " From " + tableName + " Where " + afterWhere;

                using (SqlCommand cmd = new SqlCommand(sqlSentence, connection))
                {
                    for (int i = 0; i < controlColumn.Length; i++)
                    {
                        cmd.Parameters.Add("@" + controlColumn[i], type[i]).Value = controlValue[i];
                    }
                    using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        while (dr.Read())
                        {
                            if (getColumn.Split('.').Length == 2)
                                list.Add(dr[getColumn.Split('.')[1]].ToString());
                            else
                                list.Add(dr[getColumn].ToString());
                        }
                        dr.Close();
                    }
                    cmd.Dispose();
                }
                connection.Close();

            }
            catch (Exception error)
            {
                connection.Close();
                throw new Exception(error.Message);
            }

            return list;
        }

        public static string AddOrUpdateImage(string tableName, string afterWhere, bool add, string addColumn, Bitmap bmp, string controlColumn, string controlValue, SqlDbType[] types)
        {
            connection = GetConnection();
            string result = "yes";
            string[] columns = controlColumn.Split(new string[] { "½" }, StringSplitOptions.RemoveEmptyEntries);
            string[] values = controlValue.Split(new string[] { "½" }, StringSplitOptions.RemoveEmptyEntries);

            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();


                string sqlSentence = "";
                if (add)
                    sqlSentence = "Insert Into " + tableName + "(" + addColumn + ") values(@" + addColumn + ")";
                else
                    sqlSentence = "Update " + tableName + " Set " + addColumn + "=@" + addColumn + " Where " + afterWhere;

                using (SqlCommand cmd = new SqlCommand(sqlSentence, connection))
                {
                    if (!add)
                    {
                        for (int i = 0; i < values.Length; i++)
                        {
                            cmd.Parameters.Add("@" + columns[i], types[i]).Value = values[i];
                        }
                    }
                    
                    ImageConverter converter = new ImageConverter();
                    byte[] list = (byte[]) converter.ConvertTo(bmp, typeof(byte[]));

                    cmd.Parameters.Add("@" + addColumn, SqlDbType.VarBinary, list.Length).Value = list;

                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }

                connection.Close();
            }
            catch (Exception error)
            {
                connection.Close();
                result = error.Message;
            }
            return result;
        }

        public static string GetFirstAndLastElement(string tableName, string getColumn, string controlColumn, bool asc)
        {
            connection = GetConnection();
            string result = "";

            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                string sqlSentence = "Select " + getColumn + " From " + tableName + " Order By " + controlColumn + " " + ((asc) ? "ASC" : "DESC");

                using (SqlCommand cmd = new SqlCommand(sqlSentence, connection))
                {
                    using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        if (dr.Read())
                            result += dr[0].ToString();

                        dr.Close();
                    }
                    cmd.Dispose();
                }

                connection.Close();
            }
            catch (Exception error)
            {
                connection.Close();
                throw new Exception(error.Message);
            }

            return result;
        }

        public static Bitmap getImage(string tableName, string getColumn, string afterWhere, string controlColumn, string controlValue, SqlDbType[] type)
        {
            connection = GetConnection();
            Bitmap bmp = null;
            string[] columns = controlColumn.Split(new string[] { "½" }, StringSplitOptions.RemoveEmptyEntries);
            string[] values = controlValue.Split(new string[] { "½" }, StringSplitOptions.RemoveEmptyEntries);

            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                string sqlSentence = "Select " + getColumn + " From " + tableName + " Where " + afterWhere;

                using (SqlCommand cmd = new SqlCommand(sqlSentence, connection))
                {
                    for (int i = 0; i < columns.Length; i++)
                    {
                        cmd.Parameters.Add("@" + columns[i], type[i]).Value = values[i];
                    }
                    using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        if (dr.Read())
                        {
                            if (!DBNull.Value.Equals(dr[0]))
                            {
                                byte[] image = (byte[]) dr[0];
                                MemoryStream ms = new MemoryStream(image, 0, image.Length);
                                bmp = (Bitmap) Image.FromStream(ms, true);
                                ms.Close();
                            }
                        }
                        dr.Close();
                    }
                    cmd.Dispose();
                }
                connection.Close();
            }
            catch (Exception error)
            {
                connection.Close();
                throw new Exception(error.Message);
            }

            return bmp;
        }

        public static string CreateTable(string tableName, string sqlSentence)
        {
            connection = GetConnection();
            string result = "yes";
            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                bool station = false;

                try
                {
                    using (SqlCommand cmd = new SqlCommand("Select * From " + tableName, connection))
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleResult))
                        {
                            station = false;
                            dr.Close();
                        }
                        cmd.Dispose();
                    }
                }
                catch (Exception)
                {
                    station = true;
                }

                if (station)
                {
                    using (SqlCommand cmd = new SqlCommand(sqlSentence, connection))
                    {
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                }

                connection.Close();
            }
            catch (Exception error)
            {
                connection.Close();
                result = error.Message;
            }
            return result;
        }

        public static void SaveGridAsImage(DataGridView gv, string documentName)
        {
            FolderBrowserDialog fbdFolder = new FolderBrowserDialog();
            fbdFolder.ShowDialog();
            string documentPath = fbdFolder.SelectedPath;
            fbdFolder.Dispose();

            if (documentPath != "")
            {
                int DGVOriginalHeight = gv.Height;
                gv.Height = (gv.RowCount * gv.RowTemplate.Height) + gv.ColumnHeadersHeight;

                using (Bitmap bitmap = new Bitmap(gv.Width, gv.Height))
                {
                    gv.DrawToBitmap(bitmap, new Rectangle(Point.Empty, gv.Size));
                    bitmap.Save(Path.Combine(documentPath, documentName), ImageFormat.Png);
                }

                gv.Height = DGVOriginalHeight;

                MessageBox.Show("Image Saved Successfully");
            }
        }

        public static void SaveChartAsImage(Chart cht, string documentName)
        {
            FolderBrowserDialog fbdFolder = new FolderBrowserDialog();
            fbdFolder.ShowDialog();
            string documentPath = fbdFolder.SelectedPath;
            fbdFolder.Dispose();

            if (documentPath != "")
            {
                using (Bitmap bitmap = new Bitmap(cht.Width, cht.Height))
                {
                    cht.DrawToBitmap(bitmap, new Rectangle(Point.Empty, cht.Size));
                    bitmap.Save(Path.Combine(documentPath, documentName), ImageFormat.Png);
                }

                MessageBox.Show("Image Saved Successfully");
            }
        }
    }
}
