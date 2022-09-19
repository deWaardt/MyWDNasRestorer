using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace WD_File_Recovery
{
    internal class DBController
    {
        public string _dbPath;
        string cs;
        SQLiteConnection conn = null;
        SQLiteDataReader rdr = null;

        public DBController(string dbPath)
        {
            _dbPath = dbPath;
            cs = $"Data Source = {_dbPath} ;Version=3;";
        }
        public List<Item> GetAll()
        {
            List<Item> toReturn = new List<Item>();
            List<string> strings = new List<string>();

            try
            {
                conn = new SQLiteConnection(cs);
                conn.Open();


                string stm = "SELECT * FROM Files";
                SQLiteCommand cmd = new SQLiteCommand(stm, conn);
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    Item toAdd = new Item();
                    toAdd.id = rdr.GetTextReader(0).ReadToEnd();
                    toAdd.parentID = rdr.GetTextReader(1).ReadToEnd();
                    toAdd.contentID = rdr.GetTextReader(2).ReadToEnd() ; ;
                    toAdd.filename = rdr.GetTextReader(4).ReadToEnd();
                    toAdd.type = rdr.GetTextReader(10).ReadToEnd();
                    toReturn.Add(toAdd);
                }

            }
            catch (Exception ex)
            {
                throw ex;
                //return null;
            }
            finally
            {
                if (rdr != null) { rdr.Close(); }
                if (conn != null) { conn.Close(); }
            }

            return toReturn;
        }

        public Item GetByContentID(string contentID)
        {
            List<string> strings = new List<string>();
            Item toAdd = new Item();
            try
            {
                conn = new SQLiteConnection(cs);
                conn.Open();


                string stm = $"SELECT * FROM Files WHERE contentID = '{contentID}'";
                SQLiteCommand cmd = new SQLiteCommand(stm, conn);
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    
                    toAdd.id = rdr.GetTextReader(0).ReadToEnd();
                    toAdd.parentID = rdr.GetTextReader(1).ReadToEnd();
                    toAdd.contentID = rdr.GetTextReader(2).ReadToEnd(); ;
                    toAdd.filename = rdr.GetTextReader(4).ReadToEnd();
                    toAdd.type = rdr.GetTextReader(10).ReadToEnd();

                }

            }
            catch (Exception ex)
            {
                throw ex;
                //return null;
            }
            finally
            {
                if (rdr != null) { rdr.Close(); }
                if (conn != null) { conn.Close(); }
            }

            return toAdd;
        }
    }
}
