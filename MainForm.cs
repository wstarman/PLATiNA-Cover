using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace PLATiNA_Cover
{
    public partial class MainForm : Form
    {
        public List<Covering> coverings = new List<Covering>(0);
        public int selectedCoveringIndex = -1;
        bool _suppress = false;

        Covering selectedCovering { get { return coverings[selectedCoveringIndex]; } }

        public MainForm()
        {
            InitializeComponent();
            TopMost = true;
            MaximizeBox = false;
            openFileDialogImage.Title = "Select Image";
            openFileDialogImage.Filter = "Image file|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All files|*.*";
            if (!Directory.Exists("./saves"))
            {
                Directory.CreateDirectory("./saves");
            }
            UpdateFileList();
            if (File.Exists("./saves/default"))
            {
                comboBoxFileName.SelectedItem = textBoxFileName.Text = "default";
                LoadFile();
            }
        }

        private void btAddCovering_Click(object sender, EventArgs e)
        {
            AddCovering();
        }

        private void btRemoveCovering_Click(object sender, EventArgs e)
        {
            RemoveCovering(selectedCoveringIndex);
        }
        private void AddCovering()
        {
            var covering = new Covering();
            coverings.Add(covering);
            selectedCoveringIndex = coverings.Count - 1;
            covering.Show();
            UpdateCovering();
            coveringSelect.Items.Add($"{coverings.Count}");
            coveringSelect.SelectedIndex = selectedCoveringIndex;
            BringToFront();
        }

        private void RemoveCovering(int index)
        {
            if (coverings.Count == 0) return;
            coverings[index].Close();
            coverings.RemoveAt(index);
            coveringSelect.Items.RemoveAt(index);
            coveringSelect.SelectedIndex = selectedCoveringIndex = Math.Min(index, coverings.Count - 1);
            for (int i = 0; i < coverings.Count; i++)
            {
                coveringSelect.Items[i] = $"{i + 1}";
            }
            UpdateUI();
        }

        private void UpdateCovering()
        {
            textBoxColorCode.Text = $"#{(int)numericUpDownR.Value:X2}{(int)numericUpDownG.Value:X2}{(int)numericUpDownB.Value:X2}";
            if (_suppress) return;
            if (coverings.Count == 0) return;
            selectedCovering.Bounds = new Rectangle(
                (int)numericUpDownX.Value,
                (int)numericUpDownY.Value,
                (int)numericUpDownW.Value,
                (int)numericUpDownH.Value
            );

            selectedCovering.BackColor = Color.FromArgb(
                (int)numericUpDownR.Value, (int)numericUpDownG.Value, (int)numericUpDownB.Value
            );
            selectedCovering.Opacity = Math.Min((float)numericUpDownA.Value / 255.0f, 0.999);

            selectedCovering.imagePath = textBoxImagePath.Text;
            selectedCovering.keepAspectRatio = checkBoxKeepRatio.Checked;
        }
        private void UpdateUI()
        {
            if (coverings.Count == 0) return;
            _suppress = true;
            var covering = selectedCovering;
            numericUpDownX.Value = covering.Bounds.X;
            numericUpDownY.Value = covering.Bounds.Y;
            numericUpDownW.Value = covering.Bounds.Width;
            numericUpDownH.Value = covering.Bounds.Height;

            numericUpDownR.Value = covering.BackColor.R;
            numericUpDownG.Value = covering.BackColor.G;
            numericUpDownB.Value = covering.BackColor.B;
            numericUpDownA.Value = (int)Math.Round(covering.Opacity * 255);
            textBoxColorCode.Text = $"#{covering.BackColor.R:X2}{covering.BackColor.G:X2}{covering.BackColor.B:X2}";
            textBoxImagePath.Text = covering.imagePath;
            checkBoxKeepRatio.Checked = covering.keepAspectRatio;
            _suppress = false;
        }

        private void numericUpDownX_ValueChanged(object sender, EventArgs e)
        {
            UpdateCovering();
        }

        private void numericUpDownY_ValueChanged(object sender, EventArgs e)
        {
            UpdateCovering();
        }

        private void numericUpDownW_ValueChanged(object sender, EventArgs e)
        {
            UpdateCovering();
        }

        private void numericUpDownH_ValueChanged(object sender, EventArgs e)
        {
            UpdateCovering();
        }

        private void CoveringSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedCoveringIndex = coveringSelect.SelectedIndex;
            UpdateUI();
            UpdateCovering();
        }

        private void numericUpDownR_ValueChanged(object sender, EventArgs e)
        {
            UpdateCovering();
        }

        private void numericUpDownG_ValueChanged(object sender, EventArgs e)
        {
            UpdateCovering();
        }

        private void numericUpDownB_ValueChanged(object sender, EventArgs e)
        {
            UpdateCovering();
        }

        private void numericUpDownA_ValueChanged(object sender, EventArgs e)
        {
            UpdateCovering();
        }

        private void textBoxColorCode_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Color color = ColorTranslator.FromHtml(textBoxColorCode.Text);
                numericUpDownR.Value = color.R;
                numericUpDownG.Value = color.G;
                numericUpDownB.Value = color.B;
                UpdateCovering();
            }
            catch (Exception)
            {
            }
        }

        private void buttonPlatte_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                Color color = colorDialog1.Color;
                numericUpDownR.Value = color.R;
                numericUpDownG.Value = color.G;
                numericUpDownB.Value = color.B;
                UpdateCovering();
            }
        }

        private void buttonImageLoad_Click(object sender, EventArgs e)
        {
            if (openFileDialogImage.ShowDialog() == DialogResult.OK)
            {
                textBoxImagePath.Text = openFileDialogImage.FileName;
            }
        }

        private void buttonImageRemove_Click(object sender, EventArgs e)
        {
            textBoxImagePath.Text = "";
        }

        private void textBoxImagePath_TextChanged(object sender, EventArgs e)
        {
            UpdateCovering();
        }
        private void checkBoxKeepRatio_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCovering();
        }

        private void buttonFileSave_Click(object sender, EventArgs e)
        {
            if (textBoxFileName.Text == "") return;
            string filePath = $"./saves/{textBoxFileName.Text}";
            var saveData = new List<CoveringStyle>();
            coverings.ForEach(covering => { saveData.Add(CoveringStyle.GetCoveringStyle(covering)); });
            File.WriteAllText(filePath, JsonConvert.SerializeObject(saveData));
            UpdateFileList();
            comboBoxFileName.SelectedItem = textBoxFileName.Text;
        }

        private void UpdateFileList()
        {
            string[] files = Directory.GetFiles("./saves");
            comboBoxFileName.Items.Clear();
            foreach (string file in files)
            {
                comboBoxFileName.Items.Add(Path.GetFileName(file));
            }
        }

        private void comboBoxFileName_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBoxFileName.Text = comboBoxFileName.Text;
        }

        private void buttonFileLoad_Click(object sender, EventArgs e)
        {
            LoadFile();
        }
        private void LoadFile()
        {
            var filePath = $"./saves/{textBoxFileName.Text}";
            if (File.Exists(filePath))
            {
                var itemCount = coverings.Count;
                for (int i = 0; i <= itemCount; i++)
                {
                    RemoveCovering(0);
                    Console.WriteLine(i);
                }
                var saveData = JsonConvert.DeserializeObject<CoveringStyle[]>(
                    File.ReadAllText(filePath)
                );
                for (int i = 0; i < saveData.Length; i++)
                {
                    AddCovering();
                    CoveringStyle.SetCoveringFromStyle(selectedCovering, saveData[i]);
                    Console.WriteLine(i);
                }
                UpdateUI();
            }
        }

        private void buttonFileDelete_Click(object sender, EventArgs e)
        {
            var filePath = $"./saves/{textBoxFileName.Text}";
            if (File.Exists(filePath)) {
                File.Delete(filePath);
            }
            UpdateFileList();
        }
    }
}
