using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ServerApi.Models.Meteorological;
using CCWin;
using System.IO;

namespace MeteoClientApp
{
    public partial class FormCopy : CCSkinMain
    {
        string IP = "http://128.5.10.57:62340/";
        string UrlOfMeteoPicture = "api/MeteoPicture/MeteoPicture";
        string UrlOfChartRead = "api/MeteoChartRead/ChartRead";
        string UrlOfMeteoDataInput = "api/MeteoDataInput/MeteoDataInput";
        string UrlOfGetMissionList = "api/GetMeteoMissionList/GetMissionList";
        string UrlOfMeteoProducGeneration = "api/MeteoProducGeneration/MeteoProducGeneration";
        string UrlOfWaveRefill = "api/MeteoRefill/MeteoRefill";
        StationData NowStation;
        MissionInfo NowMission;
        List<MissionInfo> missionList;
        List<StationData> stationList = new List<StationData>();
        int selectedID = 0;
        PictureForm pictureForm = new PictureForm();
        public FormCopy()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FunClass.PostMeteoData(IP + UrlOfMeteoDataInput, NowStation,NowMission);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //读取产品模型
            StreamReader sr = new StreamReader("ServerIP.txt");
            //StringBuilder sb = new StringBuilder();
            //while (sr.EndOfStream == false)
            //{
            //    sb.AppendLine(sr.ReadLine());
            //}
            IP = sr.ReadLine();
            sr.Close();
            textBox1.Text = IP;
            pictureForm.Show();

            ReloadData();

        }
        private void ReloadData()
        {
            //初始化MissionList
            missionList = FunClass.GetMissionList(IP+UrlOfGetMissionList);
            if (missionList == null)
            {
                MessageBox.Show("网络异常or任务列表数据异常or任务为空");
                return;
            }
            //加载第一个mission的stationList
            stationList = FunClass.GetStationData(IP + UrlOfChartRead,missionList[0]);
            if (stationList == null)
            {
                MessageBox.Show("网络异常or站点列表数据异常or站点为空");
                return;
            }
            //赋予当前站点
            NowStation = stationList[0];
            //初始化datagridview，加载站点信息
            RefreshDataGridView();
            //下拉框选择任务初始化
            comboBox1.DataSource = missionList;
            comboBox1.ValueMember = "missionID";
            comboBox1.DisplayMember = "missionName";
            pictureForm.ChangePicture(FunClass.GetPicture(IP + UrlOfMeteoPicture, NowStation.photoHead,1, NowMission.forecastFilesHead),NowStation);
        }
        /// <summary>
        /// 任务下拉框进行选择时的操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedID = comboBox1.SelectedIndex;
            NowMission = missionList[selectedID];
            //加载相应stationlist
            stationList = FunClass.GetStationData(IP + UrlOfChartRead, NowMission);
            if (stationList == null)
            {
                MessageBox.Show("网络异常or站点列表数据异常or站点为空");
                ReloadData();
                return;
            }
            //赋予当前站点
            NowStation = stationList[0];            
            RefreshDataGridView();
            pictureForm.ChangePicture(FunClass.GetPicture(IP + UrlOfMeteoPicture, NowStation.photoHead, 1, NowMission.forecastFilesHead),NowStation);
        }


        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataSelected();
        }
        //当有数据被选中时的操作
        private void DataSelected()
        {//预报值范围从col3~7
            int row = 0;
            int col = 0;
            try
            {
                row = dataGridView0.CurrentCell.RowIndex + 1;
                col = dataGridView0.CurrentCell.ColumnIndex + 1;
                NowStation = stationList[row-1];
            }
            catch { NowStation = stationList[0]; }
            //当所选行不是预报值时
            if (col - 2 <= 0 | col - 2 > 7)
            {
                //不做图片操作
                pictureForm.ChangePicture(MeteoClientApp.Properties.Resources.Nothing, null);
                //pictureForm.ChangePicture(null,null);
            }
            else
            {
                if (col - 2 > 0 & col - 2 <= NowStation.visibilityPrescription)
                {
                    //变更图片
                    pictureForm.ChangePicture(FunClass.GetPicture(IP + UrlOfMeteoPicture, NowStation.photoHead, col - 2, NowMission.forecastFilesHead),NowStation);
                    //pictureForm.ChangePicture(FunClass.GetPicture(IP + UrlOfMeteoPicture, NowStation.stationID.ToString("00")+"_DAY", col - 2, NowMission.forecastFilesHead), NowStation);
                }
                else
                {
                    //超出预报时效
                    pictureForm.ChangePicture(MeteoClientApp.Properties.Resources.Nothing, null);
                }
            }
            //当所选数据在预报时效内时,显示相应图片

        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            DataSelected();
        }

        private void setReadOnly(List<StationData> stationList)
        {
            int i = 0;
            foreach(var temp in stationList)
            {
                if (temp.visibilityPrescription < 5)
                {
                    dataGridView0[6, i].ReadOnly = true;
                    dataGridView0[6, i].Style.ForeColor = Color.Transparent;
                }
                if (temp.visibilityPrescription < 4)
                {
                    dataGridView0[5, i].ReadOnly = true;
                    dataGridView0[5, i].Style.ForeColor = Color.Transparent;
                }
                if (temp.visibilityPrescription < 3)
                {
                    dataGridView0[4, i].ReadOnly = true;
                    dataGridView0[4, i].Style.ForeColor = Color.Transparent;
                }
                if (temp.visibilityPrescription < 2)
                {
                    dataGridView0[3, i].ReadOnly = true;
                    dataGridView0[3, i].Style.ForeColor = Color.Transparent;
                }
                i++;
            }
        }

        private void RefreshDataGridView()
        {
            //更新datagridview，加载站点信息
            dataGridView0.DataSource = stationList;
            //dataGridView1.DataMember = "stationName";
            dataGridView0.Columns["stationID"].Visible = false;
            dataGridView0.Columns["coordinateX"].Visible = false;
            dataGridView0.Columns["coordinateY"].Visible = false;
            dataGridView0.Columns["missionID"].Visible = false;
            dataGridView0.Columns["photoHead"].Visible = false;
            dataGridView0.Columns["visibilityPrescription"].Visible = false;
            //dataGridView1.Columns["forecastFilesHead"].Visible = false;
            dataGridView0.Columns["stationName"].ReadOnly = true;
            dataGridView0.Columns["stationName"].HeaderCell.Value = "站点名称";
            dataGridView0.Columns["visibility"].HeaderCell.Value = "24-48小时预报";
            //dataGridView0.Columns["forecastValue2"].HeaderCell.Value = "48小时预报";
            //dataGridView0.Columns["forecastValue3"].HeaderCell.Value = "72小时预报";
            //dataGridView0.Columns["forecastValue4"].HeaderCell.Value = "96小时预报";
            //dataGridView0.Columns["forecastValue5"].HeaderCell.Value = "120小时预报";
            setReadOnly(stationList);
            
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if(NowMission!=null&NowMission!=null)
            {
                try
                {
                    if (!bool.Parse(FunClass.PostMeteoData(IP + UrlOfMeteoDataInput, NowStation, NowMission)))
                    {
                        MessageBox.Show("数据更新失败");
                    }
                }
                catch
                {
                    MessageBox.Show("数据更新失败");
                }
            }



        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if(this.WindowState==FormWindowState.Minimized)
            {
                pictureForm.WindowState = FormWindowState.Minimized;
            }
            if(this.WindowState==FormWindowState.Normal)
            {
                pictureForm.WindowState = FormWindowState.Normal;
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string msg = FunClass.MeteoProducGeneration(IP + UrlOfMeteoProducGeneration, NowMission);
            MessageBox.Show(msg);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var tip = MessageBox.Show("确定重新加载客观预报数据？", "重载客观数据", MessageBoxButtons.YesNo);
            if (tip.Equals(DialogResult.Yes))
            {
                stationList = FunClass.ReloadStationNFData(IP + UrlOfWaveRefill, NowMission);
                if (stationList == null)
                {
                    MessageBox.Show("客观数据更新失败，请查看错误日志。");
                    return;
                }
                else
                {
                    RefreshDataGridView();
                }

            }
        }
    }
}
