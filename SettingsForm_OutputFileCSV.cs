using System.Text;
using System.Windows.Forms;
using System.IO;

namespace OutputFileCSVPlugin
{
    internal partial class SettingsForm_OutputFileCSV : Form
    {


        #region Get and Set Options

        public string TextFileToSave { get; set; }
        public string SelectedEncoding { get; set; }
        public string Delimiter { get; set; }
        public string Quote { get; set; }

       #endregion



        public SettingsForm_OutputFileCSV(string TextFileDirectory, string SelectedEncoding, string Delimiter, string Quote)
        {
            InitializeComponent();

            foreach (var encoding in Encoding.GetEncodings())
            {
                EncodingDropdown.Items.Add(encoding.Name);
            }

            try
            {
                EncodingDropdown.SelectedIndex = EncodingDropdown.FindStringExact(SelectedEncoding);
            }
            catch
            {
                EncodingDropdown.SelectedIndex = EncodingDropdown.FindStringExact(Encoding.Default.BodyName);
            }

            CSVDelimiterTextbox.Text = Delimiter;
            CSVQuoteTextbox.Text = Quote;
            SelectedFileTextbox.Text = TextFileDirectory;

        }






        private void SetFolderButton_Click(object sender, System.EventArgs e)
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Title = "Please choose the output location for your CSV file";
                dialog.FileName = "BUTTER-CSVOutput.csv";
                dialog.Filter = "Comma-Separated Values (CSV) File (*.csv)|*.csv";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        if (File.Exists(dialog.FileName.ToString()))
                        {
                            if (DialogResult.Yes == MessageBox.Show("BUTTER is about to overwrite your selected file. Are you ABSOLUTELY sure that you want to do this? All data currently contained in the selected file will immediately be deleted if you select \"Yes\".", "Overwrite File?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                            {
                                using (var myFile = File.Create(dialog.FileName.ToString())) {}
                                SelectedFileTextbox.Text = dialog.FileName.ToString();
                            }
                            else
                            {
                                SelectedFileTextbox.Text = "";
                            }
                        }
                        else
                        {
                            using (var myFile = File.Create(dialog.FileName.ToString())) { }
                            SelectedFileTextbox.Text = dialog.FileName.ToString();
                        }
                        
                        
                        
                    }
                    catch
                    {
                        MessageBox.Show("BUTTER does not appear to be able to create this output file. Do you have write permissions for this file? Is the file already open in another program?", "Cannot Create File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        SelectedFileTextbox.Text = "";
                    }
                    
                }
            }
        }


        private void OKButton_Click(object sender, System.EventArgs e)
        {
            this.SelectedEncoding = EncodingDropdown.SelectedItem.ToString();
            this.TextFileToSave = SelectedFileTextbox.Text;
            if (CSVQuoteTextbox.Text.Length > 0)
            {
                this.Quote = CSVQuoteTextbox.Text;
            }
            else
            {
                this.Quote = "\"";
            }
            if (CSVDelimiterTextbox.Text.Length > 0)
            {
                this.Delimiter = CSVDelimiterTextbox.Text;
            }
            else
            {
                this.Delimiter = ",";
            }
            

            this.DialogResult = DialogResult.OK;

        }
    }
}
