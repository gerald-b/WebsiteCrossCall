using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net;
using System.Text.RegularExpressions;

namespace WebsiteCrossCall
{
    public partial class frmMain : Form
    {
        private List<WebCallEntry> wce = new List<WebCallEntry>();

        public frmMain()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = String.Empty;

            
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            btnCheck.Enabled = false;
            btnCheck.Text = "Läuft....";
            lbxOutput.Items.Clear();
            wce.Clear();

            WebProxy wp = new WebProxy();
            ICredentials credentials = new NetworkCredential("username", "password");
            wp.Address = new Uri("http://proxyserver:port");
            wp.Credentials = credentials;

            WebClient wc = new WebClient();
            wc.Proxy = wp;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; //TLS 1.2
            String content = wc.DownloadString(txtWebsite.Text);
            String[] contentLines = content.Split('\n');
            foreach (String curLine in contentLines)
            {
                //Regex myReg = new Regex(@"[h]{1}[t]{2}[p]{1}[s]{0,1}[:]{1}[\/]{2}");
                //Regex myReg = new Regex(@"[H]{1}[T]{2}[P]{1}[S]{0,1}[:]{1}[\/]{2}");
                //if (myReg.IsMatch(curLine.ToUpper()))
                if (Regex.Match(curLine.ToUpper(), @"HTTP[S]{0,1}:\/\/", RegexOptions.None).Success)
                // if (Regex.Match(curLine.ToUpper(), @"[h]{1}[t]{2}[p]{1}[s]{0,1}[:]{1}[\/]{2}").Success)
                {
                    foreach (int i in WebsiteCrossCall.Program.AllIndexesOf(curLine.ToUpper(), "HTTP"))
                    {
                        int spos = i;
                        int epos = -1;
                        String _outp = String.Empty;

                        epos = curLine.Substring(spos + 9).ToUpper().IndexOf("/"); 
                        if (-1 == epos)
                        {
                            epos = curLine.Substring(spos + 9).ToUpper().IndexOf("\"");
                        }

                        if (-1 == epos)
                        {
                            _outp = curLine.Substring(spos);
                        }
                        else
                        {
                            _outp = curLine.Substring(spos, epos + 9);
                        }
                        bool entryIsFound = false;
                        foreach (WebCallEntry entry in wce)
                        {
                            if (_outp == entry._match)
                            {
                                entryIsFound = true;
                                break;
                            }
                        }
                        if (!entryIsFound) { wce.Add(new WebCallEntry(_outp, curLine)); }
                    }
                }
            }

            foreach (WebCallEntry entry in wce)
            {
                lbxOutput.Items.Add(entry._match);
            }
            lbxOutput.Sorted = true;

            toolStripStatusLabel1.Text = wce.Count.ToString() + " Einträg(e) gefunden!";
            btnCheck.Text = "Check"; 
            btnCheck.Enabled = true;
        }

        private void lbxOutput_DoubleClick(object sender, EventArgs e)
        {
            String a = lbxOutput.Items[lbxOutput.SelectedIndex].ToString();
            WebCallEntry mySelectedEntry = null;
            foreach(WebCallEntry entry in wce)
            {
                if (a == entry._match)
                {
                    mySelectedEntry = entry;
                    break;
                }
            }

            if (null != mySelectedEntry)
            {
                MessageBox.Show(mySelectedEntry._match + Environment.NewLine + Environment.NewLine + mySelectedEntry._line, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
