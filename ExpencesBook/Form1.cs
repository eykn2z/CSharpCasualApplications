﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExpencesBook
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void ファイルFToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            AddData();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();
            categoryDataSet1.CategoryDataTable.AddCategoryDataTableRow("給料", "入金");
            categoryDataSet1.CategoryDataTable.AddCategoryDataTableRow("食費", "出金");
            categoryDataSet1.CategoryDataTable.AddCategoryDataTableRow("雑費", "出金");
            categoryDataSet1.CategoryDataTable.AddCategoryDataTableRow("住居", "出金");
        }

        private void AddData()
        {
            if (tabControl1.SelectedTab == tabList)
            {
                ItemForm frmItem = new ItemForm(categoryDataSet1);
                DialogResult drRet = frmItem.ShowDialog();

                if (drRet == DialogResult.OK)
                {
                    moneyDataSet.MoneyDataTable.AddMoneyDataTableRow(
                        frmItem.monCalendar.SelectionRange.Start,
                        frmItem.cmbCategory.Text,
                        frmItem.txtItem.Text,
                        int.Parse(frmItem.mtxtMoney.Text),
                        frmItem.txtRemarks.Text
                        );
                }
            }

        }

        private void SaveData()
        {
            string path = "MoneyData.csv";
            string strData = "";
            System.IO.StreamWriter sw = new System.IO.StreamWriter(
                path,
                false,
                System.Text.Encoding.Default);
            foreach (MoneyDataSet.MoneyDataTableRow drMoney
                in moneyDataSet.MoneyDataTable)
            {
                strData = drMoney.日付.ToString() + ","
                    + drMoney.分類 + ","
                    + drMoney.品名 + ","
                    + drMoney.金額.ToString() + ","
                    + drMoney.備考;
                sw.WriteLine(strData);
            }
            sw.Close();

        }
        private void LoadData()
        {
            string path = "MoneyData.csv";
            string delimStr = ",";
            char[] delimiter = delimStr.ToCharArray();
            string[] strData;
            string strLine;
            bool fileExists = System.IO.File.Exists(path);
            if (fileExists)
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(
                    path,
                    System.Text.Encoding.Default);
                while (sr.Peek() >= 0)//if data exists
                {
                    strLine = sr.ReadLine();
                    strData = strLine.Split(delimiter);
                    moneyDataSet.MoneyDataTable.AddMoneyDataTableRow(
                        DateTime.Parse(strData[0]),
                        strData[1],
                        strData[2],
                        int.Parse(strData[3]),
                        strData[4]);
                }
                sr.Close();
            }
        }
        
        private void UpdateData()
        {
            if (tabControl1.SelectedTab== tabList)
            {
                int nowRow = dgv.CurrentRow.Index;
                DateTime oldDate
                    = DateTime.Parse(dgv.Rows[nowRow].Cells[0].Value.ToString());
                string oldCategory = dgv.Rows[nowRow].Cells[1].Value.ToString();
                string oldItem = dgv.Rows[nowRow].Cells[2].Value.ToString();
                int oldMoney
                    = int.Parse(dgv.Rows[nowRow].Cells[3].Value.ToString());
                string oldRemarks = dgv.Rows[nowRow].Cells[4].Value.ToString();
                ItemForm frmItem = new ItemForm(categoryDataSet1,
                    oldDate,
                    oldCategory,
                    oldItem,
                    oldMoney,
                    oldRemarks);
                DialogResult drRet = frmItem.ShowDialog();
                if (drRet == DialogResult.OK)
                {
                    dgv.Rows[nowRow].Cells[0].Value
                        = frmItem.monCalendar.SelectionRange.Start;
                    dgv.Rows[nowRow].Cells[1].Value = frmItem.cmbCategory.Text;
                    dgv.Rows[nowRow].Cells[2].Value = frmItem.txtItem.Text;
                    dgv.Rows[nowRow].Cells[3].Value = int.Parse(frmItem.mtxtMoney.Text);
                    dgv.Rows[nowRow].Cells[4].Value = frmItem.txtRemarks.Text;

                }
            }


        }
        private void DeleteDate()
        {
            if (tabControl1.SelectedTab == tabList)
            {
                int nowRow = dgv.CurrentRow.Index;
                dgv.Rows.RemoveAt(nowRow);//delete nowRow
            }
        }

        private void 追加AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddData();
        }

        private void buttonEnd_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void 終了XToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveData();
        }

        private void 保存SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveData();
        }

        private void buttonChange_Click(object sender, EventArgs e)
        {
            UpdateData();
        }

        private void 変更CToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateData();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            DeleteDate();
        }

        private void 削除DToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteDate();
        }

        private void CalcSummary()
        {
            string expression;
            summaryDataSet.SumDataTable.Clear();
            foreach (MoneyDataSet.MoneyDataTableRow drMoney in moneyDataSet.MoneyDataTable)
            {
                expression = "日付='" + drMoney.日付.ToShortDateString() + "'";//CREATE string "日付='2017/12/31'"
                SummaryDataSet.SumDataTableRow[] curDR
                    = (SummaryDataSet.SumDataTableRow[])
                    summaryDataSet.SumDataTable.Select(expression);//SEARCH with expression
                if (curDR.Length==0)
                {
                    CategoryDataSet.CategoryDataTableRow[] selectedDataRow;
                    selectedDataRow = (CategoryDataSet.CategoryDataTableRow[])
                        categoryDataSet1.CategoryDataTable.Select("分類='" + drMoney.分類 + "'");//CHANGE datatype (@)#
                    if (selectedDataRow[0].入出金分類=="入金")
                    {
                        summaryDataSet.SumDataTable.AddSumDataTableRow(drMoney.日付,
                            drMoney.金額,
                            0);
                    }
                    else if(selectedDataRow[0].入出金分類 == "出金")
                    {
                        summaryDataSet.SumDataTable.AddSumDataTableRow(drMoney.日付,
                            0,
                            drMoney.金額);
                    }
                }
                else
                {
                    CategoryDataSet.CategoryDataTableRow[] selectedDataRow;
                    selectedDataRow = (CategoryDataSet.CategoryDataTableRow[])
                        categoryDataSet1.CategoryDataTable.Select("分類='" + drMoney.分類 + "'");
                    if (selectedDataRow[0].入出金分類 == "入金")
                    {
                        curDR[0].入金合計 += drMoney.金額;
                    }
                    else if(selectedDataRow[0].入出金分類=="出金")
                    {
                        curDR[0].出金合計 += drMoney.金額;
                    }
                }
            }

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CalcSummary();
        }

        private void 一覧表示VToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(tabList);
        }

        private void 集計表示SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(tabSummary);
        }
    }
}
