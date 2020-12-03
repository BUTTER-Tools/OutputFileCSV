using PluginContracts;
using System.Drawing;
using System.Windows.Forms;
using TSOutputWriter;
using System;
using OutputHelperLib;
using System.Collections.Generic;
using System.IO;


namespace OutputFileCSVPlugin
{
    public class OutputFileCSVPlugin : OutputPlugin
    {

        public string[] InputType { get; } = { "String", "OutputArray" };
        public string OutputType { get; } = "None";

        public bool KeepStreamOpen { get; } = true;
        public ThreadsafeOutputWriter Writer { get; set;  }
        public Dictionary<int, string> OutputHeaderData { get; set; }
        public bool headerWritten { get; set; } = false;
        public bool InheritHeader { get; } = true;
        public FileMode fileMode { get; set; } = FileMode.Create;


        #region IPlugin Details and Info
        public string PluginName { get; } = "Save Output to CSV";
        public string PluginType { get; } = "Save Output File(s)";
        public string PluginVersion { get; } = "1.0.5";
        public string PluginAuthor { get; } = "Ryan L. Boyd (ryan@ryanboyd.io)";
        public string PluginDescription { get; } = "This plugin will save your output to a CSV file. Also able to accept strings as input, for example, if you want to create a CSV file out of a folder full of individual .txt files.";
        public bool TopLevel { get; } = false;
        public string PluginTutorial { get; } = "https://youtu.be/YRZfR3SOmX8";

        public Icon GetPluginIcon
        {
            get
            {
                return Properties.Resources.icon;
            }
        }
        #endregion


        #region Settings and ChangeSettings() Method

        public string OutputLocation { get; set; } = "";
        public string SelectedEncoding { get; set; } = "utf-8";
        private string Delimiter = ",";
        private string Quote = "\"";



        public void ChangeSettings()
        {



            using (var form = new SettingsForm_OutputFileCSV(OutputLocation, SelectedEncoding, Delimiter, Quote))
            {


                form.Icon = Properties.Resources.icon;
                form.Text = PluginName;


                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    SelectedEncoding = form.SelectedEncoding;
                    OutputLocation = form.TextFileToSave;
                    Delimiter = form.Delimiter;
                    Quote = form.Quote;
                }
            }



        }
        #endregion


        //sets GetTextList with the files to be analyzed
        public Payload RunPlugin(Payload Input)
        {

                //Payload pData = Input;

                //what we do if we got a List<string> as input
                if (Input.Type == "OutputArray")
                {
                    for (int counter = 0; counter < Input.StringArrayList.Count; counter++)
                        {

                            //https://stackoverflow.com/a/59250
                            //Sets up the array that we want to write as output by prepending the file details / segment details
                            string[] FileDetails = new string[3] { Quote + Input.FileID.Replace(Quote, Quote + Quote) + Quote, Input.SegmentNumber[counter].ToString(), ""};
                            if (Input.SegmentID.Count == Input.StringArrayList.Count)
                            {
                                FileDetails[2] = Quote + Input.SegmentID[counter].Replace(Quote, Quote + Quote) + Quote;
                            }
                            else if (Input.SegmentID.Count == 1)
                            {
                                FileDetails[2] = Quote + Input.SegmentID[0].Replace(Quote, Quote + Quote) + Quote;
                            }



                            string[] ArrayToWrite = new string[FileDetails.Length + Input.StringArrayList[counter].Length];

                            //make sure that items aren't null
                            for (int i = 0; i < Input.StringArrayList[counter].Length; i++) if (Input.StringArrayList[counter][i] == null) Input.StringArrayList[counter][i] = "";
                            //make sure that quoting happens properly
                            for (int i = 0; i < Input.StringArrayList[counter].Length; i++) Input.StringArrayList[counter][i] = Quote + Input.StringArrayList[counter][i].Replace(Quote, Quote + Quote) + Quote;

                            Array.Copy(FileDetails, ArrayToWrite, FileDetails.Length);
                            Array.Copy(Input.StringArrayList[counter], 0, ArrayToWrite, FileDetails.Length, Input.StringArrayList[counter].Length);

                            //perform the actual write
                            Writer.WriteString(string.Join(Delimiter, ArrayToWrite));
                        }
                    }
                else if (Input.Type == "String")
                {
                   

                    for (int counter = 0; counter < Input.StringList.Count; counter++)
                    {
                        string[] ArrayToWrite = new string[4] { "\"" + Input.FileID.Replace(Quote, Quote + Quote) + "\"",
                                                            (Input.SegmentNumber[counter]).ToString(),
                                                            "",
                                                            Quote + Input.StringList[counter].Replace(Quote, Quote + Quote) + Quote
                                                            };
                        if (Input.SegmentID.Count > 0) ArrayToWrite[2] = Input.SegmentID[counter];

                        Writer.WriteString(string.Join(Delimiter, ArrayToWrite));
                    }
                }

                return (new Payload());


        }

        #region Write Header
        public void WriteHeader()
        {

            headerWritten = true;

            if (Writer != null) { 
               
                
                #region Build and Write Header
                string[] HeaderArray = new string[OutputHeaderData.Keys.Count + 3];
                HeaderArray[0] = "TextID";
                HeaderArray[1] = "Segment";
                HeaderArray[2] = "SegmentID";

                for (int counter = 0; counter < OutputHeaderData.Keys.Count; counter++)
                {
                    HeaderArray[counter + 3] = Quote + OutputHeaderData[counter].Replace(Quote, Quote + Quote) + Quote;
                }
                
                Writer.WriteString(string.Join(Delimiter, HeaderArray));

                #endregion
            }


        }
        #endregion


        public void Initialize()
        {
                WriteHeader();
        }



        public bool InspectSettings()
        {
            if (string.IsNullOrEmpty(OutputLocation))
            {
                return false;
            }
            else
            {
                return true;
            }
                
        }

        public Payload FinishUp(Payload Input)
        {
            //this is essentially returning an empty Payload, since that's what is returned from the
            //RunPlugin method 
            return (this.RunPlugin(Input));
        }



        #region Import/Export Settings
        public void ImportSettings(Dictionary<string, string> SettingsDict)
        {
            OutputLocation = SettingsDict["OutputLocation"];
            SelectedEncoding = SettingsDict["SelectedEncoding"];
            Delimiter = SettingsDict["Delimiter"];
            Quote = SettingsDict["Quote"];
        }

        public Dictionary<string, string> ExportSettings(bool suppressWarnings)
        {
            Dictionary<string, string> SettingsDict = new Dictionary<string, string>();
            SettingsDict.Add("OutputLocation", OutputLocation);
            SettingsDict.Add("SelectedEncoding", SelectedEncoding);
            SettingsDict.Add("Delimiter", Delimiter);
            SettingsDict.Add("Quote", Quote);
            return (SettingsDict);
        }
        #endregion


    }



}
