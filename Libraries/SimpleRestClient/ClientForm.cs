//
// Copyright (c) 2010 james@crispdesign.net, achim@sones.de
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
// of the Software, and to permit persons to whom the Software is furnished to
// do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Net;

namespace RestClient
{
	public partial class ClientForm : Form
	{
		void MakeRequest()
		{

			ResponseTextBox.Text = "Please wait...";

			var url             = UrlTextBox.Text;
			var method          = VerbComboBox.Text;
			var requestHeader   = RequestHeaderTextBox.Text;
            var requestBody     = RequestBodyTextBox.Text;

			string reponseAsString = "";

			try
			{
				var request     = (HttpWebRequest) WebRequest.Create(url);
				request.Method  = method;
                request.Accept  = (String) AcceptComboBox.Text;

                SetRequest(request, requestHeader, requestBody);

				var response    = (HttpWebResponse) request.GetResponse();

				reponseAsString = ConvertResponseToString(response);
			}
			catch (Exception ex)
			{
				reponseAsString += "ERROR: " + ex.Message;
			}

			ResponseTextBox.Text = reponseAsString;
		}

        void SetRequest(HttpWebRequest request, string requestHeader, string requestBody)
		{

            if (requestHeader.Length > 0)
                request.Headers.Add(requestHeader);

			if (requestBody.Length > 0)
			{
				using (Stream requestStream = request.GetRequestStream())
				using (StreamWriter writer = new StreamWriter(requestStream))
				{
					writer.Write(requestBody);
				}
			}

		}

		string ConvertResponseToString(HttpWebResponse response)
		{
			string result = "Status code: " + (int)response.StatusCode + " " + response.StatusCode + "\r\n";

			foreach (string key in response.Headers.Keys)
			{
				result += string.Format("{0}: {1} \r\n", key, response.Headers[key]);
			}

			result += "\r\n";
			result += new StreamReader(response.GetResponseStream()).ReadToEnd();

			return result;
		}

		public ClientForm()
		{
			InitializeComponent();
		}

		void ClientForm_Load(object sender, EventArgs e)
		{
			VerbComboBox.SelectedIndex = 0;
            AcceptComboBox.SelectedIndex = 1;
			UrlTextBox.Text = "http://localhost:9975/";
		}

		void ExecuteButton_Click(object sender, EventArgs e)
		{
			MakeRequest();
		}

		void UrlTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				MakeRequest();
				e.Handled = true;
			}
		}

        private void label6_Click(object sender, EventArgs e)
        {

        }
	}
}
