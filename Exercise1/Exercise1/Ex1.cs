﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab04
{
    public partial class Ex1 : Form
    {
        public Ex1()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            string html = getHTML(txtUrl.Text);
            rtbOutput.Text = html;
        }
        private string getHTML(string szURL)
        {
            // Create a request for the URL. 
            WebRequest request = WebRequest.Create(szURL);
            // Get the response.   
            WebResponse response = request.GetResponse();
            // Get the stream containing content returned by the server.  
            Stream dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.   
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.   
            string responseFromServer = reader.ReadToEnd();
            // Close the response.
            response.Close();
            return responseFromServer;
        }
    }
}
