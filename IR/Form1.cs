﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace IR
{
    public partial class Form1 : Form
    {
        private List<String> toVisit;
        private List<String> visited;//urls
        private List<String> content;//htmlcontent
        public Form1()
        {
            InitializeComponent();
            toVisit = new List<string>();
            visited = new List<string>();
            content = new List<string>();
            toVisit.Add("http://www.google.com");
            /*
            string connectionString = "Data source=orcl; User Id=scott; Password=tiger;";
            OracleConnection conn;
            */
        }

        private void crawl_Click(object sender, EventArgs e)
        {
            int i = 0;
            while (visited.Count != 3000)//طبعا محدش يرن 3000 الاب  هيوقف  :D 
            {
                if (toVisit.Count != 0)
                {
                    content.Add(HTTPRequest(toVisit[i]));
                    visited.Add(toVisit[i]);
                    listView1.Items.Add(new ListViewItem(toVisit[i]));
                    listView2.Items.Add(new ListViewItem(content[i]));
                    toVisit.RemoveAt(i);
                    searchForLinks(content[i]);

                    i++;
                }
            }
        }

        public string HTTPRequest(String URL)
        {
            
            WebRequest myWebRequest;
            WebResponse myWebResponse;

            // Create a new 'WebRequest' object to the mentioned URL.
            myWebRequest = WebRequest.Create(URL);

            // The response object of 'WebRequest' is assigned to a WebResponse' variable.
            myWebResponse = myWebRequest.GetResponse();

            Stream streamResponse = myWebResponse.GetResponseStream();
            StreamReader sReader = new StreamReader(streamResponse);
            string rString = sReader.ReadToEnd();

            streamResponse.Close();
            sReader.Close();
            myWebResponse.Close();
            return rString;

        }
        public void searchForLinks(String content)
        {
            var urlDictionary = new Dictionary<string, string>();

            Match match = Regex.Match(content, "(?i)<a .*?href=\"([^\"]+)\"[^>]*>(.*?)</a>");
            while (match.Success)
            {
                
                string urlKey = match.Groups[1].Value;

                
                string urlValue = Regex.Replace(match.Groups[2].Value, "(?i)<.*?>", string.Empty);

                urlDictionary[urlKey] = urlValue;
                match = match.NextMatch();
            }

            foreach (var item in urlDictionary)
            {
                string href = item.Key;
                string text = item.Value;

                if (!string.IsNullOrEmpty(href))
                {
                    string url = href.Replace("%3f", "?")
                        .Replace("%3d", "=")
                        .Replace("%2f", "/")
                        .Replace("&amp;", "&");

                    if (string.IsNullOrEmpty(url) || url.StartsWith("#")
                        || url.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase)
                        || url.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    if (url.Contains("http://")|| url.Contains("https://"))
                    {
                        toVisit.Add(url);
                    }
                    
                }
            }
        }
        /*
        private void addCrawlerResultsToDatabase()
        {
            conn = new OracleConnection(connectionString);
            conn.Open();
            int count = 0;
            OracleCommand cmd;

            while (count < 3000)
            {
                cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = "INSERT_NEW_CRAWLER_RESULT";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("page_url", OracleDbType.Varchar2, DBNull.Value, ParameterDirection.Input).Value = listView1.GetItemAt(0, count);
                cmd.Parameters.Add("page_content", OracleDbType.Varchar2, DBNull.Value, ParameterDirection.Input).Value = listView2.GetItemAt(2, count);
                cmd.ExecuteNonQuery();
                count++;
            }
            */
    }
}



