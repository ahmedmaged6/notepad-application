using System.Drawing.Printing;

namespace MegaPad
{
    public partial class notepadForm : Form
    {
        // Variables Declarartion
        private bool fileAlreadySaved;
        private bool fileUpdated;
        private string currentFileName;
        private Stack<string> undoStack = new Stack<string>();
        private Stack<string> redoStack = new Stack<string>();
        private int cursorPosition = 0;
        //********************************************************************************************************
        public notepadForm()
        {
            InitializeComponent();
            richTextBox1.TextChanged += richTextBox1_TextChanged;
            richTextBox1.SelectionChanged += richTextBox1_TextChanged;
        }
        //********************************************************************************************************
        private void notepadForm_load(object sender, EventArgs e)
        {
            fileAlreadySaved = false;
            fileUpdated = false;
            currentFileName = "";
        }

        //********************************************************************************************************


        //----------FILE-----------//


        //**New Tab**//
        private void newTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileUpdated)
            {
                DialogResult result = MessageBox.Show("Do you want to save your changes?", "File Save", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                switch (result)
                {
                    case DialogResult.Yes:
                        SaveFileUpdated();
                        ClearScreen();

                        break;
                    case DialogResult.No:
                        ClearScreen();
                        break;
                }
            }
            else
                ClearScreen();
        }

        //-------------------------------------------------------------------------------------------------------

        //** Open **//
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Text Files (*.txt)|*.txt";

            DialogResult result = ofd.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (Path.GetExtension(ofd.FileName) == ".txt")
                {
                    richTextBox1.LoadFile(ofd.FileName, RichTextBoxStreamType.PlainText);
                }
                this.Text = Path.GetFileName(ofd.FileName) + " - Megapad";

                fileAlreadySaved = true;
                fileUpdated = false;
                currentFileName = ofd.FileName;
            }
        }

        //-------------------------------------------------------------------------------------------------------

        //** Save **//
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileUpdated();

        }

        private void SaveFileUpdated()
        {
            if (fileAlreadySaved)
            {
                if (Path.GetExtension(currentFileName) == ".txt")
                {
                    richTextBox1.SaveFile(currentFileName, RichTextBoxStreamType.PlainText);
                }

                fileUpdated = false;
            }
            else
            {
                if (fileUpdated)
                    SaveFile();
                else
                    ClearScreen();

            }

        }
        private void ClearScreen()
        {
            richTextBox1.Clear();
            fileUpdated = false;
            this.Text = "Megapad";
        }
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        private void SaveFile()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Text Files (*.txt)|*.txt";

            DialogResult result = sfd.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (Path.GetExtension(sfd.FileName) == ".txt")
                {
                    richTextBox1.SaveFile(sfd.FileName, RichTextBoxStreamType.PlainText);
                }
                this.Text = Path.GetFileName(sfd.FileName) + " - Megapad";


                fileAlreadySaved = true;
                fileUpdated = false;
                currentFileName = sfd.FileName;
            }
        }

        //-------------------------------------------------------------------------------------------------------

        //** Print **//
        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.DrawString("Hello, World!", new Font("Arial", 12), Brushes.Black, 10, 10);
        }
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            PrintDocument printDocument = new PrintDocument();

            printDialog.Document = printDocument;

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                printDocument.PrintPage += new PrintPageEventHandler(PrintDocument_PrintPage);
                printDocument.Print();
            }

        }

        //-------------------------------------------------------------------------------------------------------

        //** Exit **//
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!fileAlreadySaved)
            {
                DialogResult result = MessageBox.Show("Do you want to save your changes before closing?", "Exit Confirmation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    SaveFile();
                    Application.Exit();
                }
                else if (result == DialogResult.Cancel)
                {

                    return;
                }
                else
                    Application.Exit();

            }

        }
        private void notepadForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!fileAlreadySaved)
            {
                DialogResult result = MessageBox.Show("Do you want to save your changes before closing?", "Exit Confirmation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    SaveFile();
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }

            }
        }


        //-------------------------------------------------------------------------------------------------------



        //********************************************************************************************************

        //----------Edit-----------//
        private void RestoreText(string text)
        {
            richTextBox1.Text = text;
            richTextBox1.SelectionStart = Math.Min(cursorPosition, text.Length);
        }
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            fileUpdated = true;
            string currentText = richTextBox1.Text;
            if (undoStack.Count == 0 || undoStack.Peek() != currentText)
            {
                undoStack.Push(currentText);
                redoStack.Clear();
            }
            // Update character count
            UpdateCharCount(richTextBox1.Text.Length);

            // Update line and column information
            int line, col;
            GetLineColumn(out line, out col);
            UpdateStatus($"Lin: {line}, Column: {col}");

        }
        private void richTextBox1_SelectionChanged(object sender, EventArgs e)
        {
            cursorPosition = richTextBox1.SelectionStart;

        }
        //-------------------------------------------------------------------------------------------------------

        //** Undo **//
        private void Undo()
        {


            if (undoStack.Count > 1)
            {
                string currentText = undoStack.Pop();
                redoStack.Push(undoStack.Pop());
                RestoreText(undoStack.Peek());
            }

        }
        private void undoToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Undo();

        }

        //-------------------------------------------------------------------------------------------------------

        //** Redo **//
        private void Redo()
        {
            if (redoStack.Count > 0)
            {
                undoStack.Push(redoStack.Pop());
                RestoreText(undoStack.Peek());
            }
        }
        private void redoToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Redo();
        }


        //-------------------------------------------------------------------------------------------------------

        //** Select All **//
        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectAll();

        }

        //-------------------------------------------------------------------------------------------------------

        //********************************************************************************************************

        //----------View-----------//
        //** Zoom In **//
        private void zoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.ZoomFactor += 0.1f;
            zoomLevel += 10;
            UpdateZoomLabel();

        }

        //-------------------------------------------------------------------------------------------------------
        //** Zoom In **//
        private void zoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.ZoomFactor = Math.Max(0.1f, richTextBox1.ZoomFactor - 0.1f);
            zoomLevel -= 10;
            UpdateZoomLabel();
        }
        //-------------------------------------------------------------------------------------------------------

        //** Restore to Default **//

        private void restoreDefaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.ZoomFactor = 1.0f;
        }

        //********************************************************************************************************

        //----------About-----------//
        private void megapadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Megapad\n" +
                "Version 1.0.0\n" +
                "Copyright © 2023 MEGA Solutions\n\n" +
                "Megapad is a powerful and feature-rich text editor designed to meet all your text editing needs. Whether you're writing code, taking notes, or drafting documents, Megapad has you covered. \n\n" +
                "Acknowledgments:\n" +
                "-Megapad uses the following open - source libraries:\n" +
                "-Newtonsoft.Json for JSON serialization (https://www.newtonsoft.com/json)\n" +
                "-NLog for logging(https://nlog-project.org/)\n\n" +
                "Contact:\n For support or inquiries, please contact support@megasolutions.com\n\n" +
                "Thank you for choosing Megapad!\n\n",
               "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        //********************************************************************************************************

        //----------Status Bar -----------//

        private void UpdateStatus(string text)
        {
            charCountLabel.Text = text;
        }

        private void UpdateCharCount(int count)
        {
            lineColLabel.Text = $"Characters: {count}";
        }

        private void GetLineColumn(out int line, out int col)
        {
            int caretPosition = richTextBox1.SelectionStart;
            line = richTextBox1.GetLineFromCharIndex(caretPosition) + 1;
            col = caretPosition - richTextBox1.GetFirstCharIndexFromLine(line - 1) + 1;
        }

        private int zoomLevel = 100;
        private void UpdateZoomLabel()
        {
            zoomStatusLabel.Text = $"Zoom: {zoomLevel}%";
        }
        private void statusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip1.Visible = !statusStrip1.Visible;
            statusBarToolStripMenuItem.Checked = statusStrip1.Visible;

        }


        //********************************************************************************************************

        //----------Mouse Hoover-----------//
        private void fileToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            fileToolStripMenuItem.ShowDropDown();
        }

        private void editToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {

            editToolStripMenuItem.ShowDropDown();
        }

        private void viewToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {

            viewToolStripMenuItem.ShowDropDown();
        }

        private void aboutToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            aboutToolStripMenuItem.ShowDropDown();

        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}