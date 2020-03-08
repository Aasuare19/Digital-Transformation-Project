using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.util;

namespace Project1C
{
    public class PDFController
    {
        private string[] _arrFiles;
        private string[] _arrTxtFiles;
        private int _intNumOfPapers;
        private int _intNumOfWordsInCorpus = 0;
        private List<string> _lstStopWords;
        

        public PDFController(List<string> lstStopWords)
        {
            _arrFiles = Directory.GetFiles(@"C:\ProjTestFiles\"); //CHANGE TO LOCATION OF CORPUS FOLDER CONTAINING PDF FILES
            _intNumOfPapers = _arrFiles.Count();
            _arrTxtFiles = new string[_arrFiles.Count()];
            _lstStopWords = lstStopWords;
        }

        public void setArrayTextFiles()
        {
            //convert pdf to text
            for (int i = 0; i < _arrFiles.Length; i++)
            {
                string text = parseUsingPDFBox(_arrFiles[i]);
                text = formatStringForLooping(text);
                _intNumOfWordsInCorpus += getNumberOfWords(text);

                //add to array 
                _arrTxtFiles[i] = text;

                //delete below after one run

                //string name = Path.GetFileName(_arrFiles[i]);

                //using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
                //{
                //    using (SqlCommand cmd = new SqlCommand())
                //    {
                //        string sql = "INSERT INTO Corpus(Name, Text)" +
                //        " VALUES(@name, @text)";

                //        cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;
                //        cmd.Parameters.Add("@text", SqlDbType.NVarChar).Value = text;

                //        cmd.CommandType = CommandType.Text;
                //        cmd.CommandText = sql;
                //        cmd.Connection = conn;
                //        conn.Open();
                //        int rowsAffected = cmd.ExecuteNonQuery();
                //        conn.Close();
                //    }
                //}
            }
        }

        private static string parseUsingPDFBox(string input)
        {
            PDDocument doc = null;

            try
            {
                doc = PDDocument.load(input);
                PDFTextStripper stripper = new PDFTextStripper();
                return stripper.getText(doc);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "Error";
            }
            finally
            {
                if (doc != null)
                {
                    doc.close();
                }
            }
        }

        private string formatStringForLooping(string text)
        {
            //1. replace punctuation with spaces
            text = Regex.Replace(text, @"[^\w\s]", " ");

            //2. remove stop words
            foreach (string sw in _lstStopWords)
            {
                text = Regex.Replace(text, @"\b" + sw + @"\b", "", RegexOptions.IgnoreCase);
                //text = text.Replace(sw, "");
            }

            //3. remove special characters
            text = Regex.Replace(text, "[^0-9A-Za-z ,]", "");

            //4. remove any numbers
            text = Regex.Replace(text, @"[\d-]", "");

            //5. reduce any amount of spaces to just one
            text = Regex.Replace(text, "[ ]{2,}", " ", RegexOptions.None);

            return text;
        }
        private int getNumberOfWords(string text)
        {
            List<string> words = text.Split(' ').ToList();

            //remove empties
            words.RemoveAll(w => string.IsNullOrEmpty(w));

            return words.Count;
        }
        //GETTERS
        public string[] Files()
        {
            return _arrFiles;
        }
        public string[] TxtFiles()
        {
            return _arrTxtFiles;
        }
        public int IntNumOfPapers()
        {
            return _intNumOfPapers;
        }
        public int IntNumOfWordsInCorpus()
        {
            return _intNumOfWordsInCorpus;
        }

       
    }
}