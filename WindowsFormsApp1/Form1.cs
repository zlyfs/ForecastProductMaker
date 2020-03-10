using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ServerApi.Models.Wave;

namespace ClientApp
{
    public partial class Form1 : Form
    {
        string IP = "http://localhost:62340/";
        string UrlOfWavePicture = "api/WavePicture/WavePicture";
        string UrlOfChartRead = "api/WaveChartRead/ChartRead";
        string UrlOfWaveDataInput = "api/WaveDataInput/WaveDataInput";
        string UrlOfGetMissionList = "api/GetWaveMissionList/GetMissionList";
        StationData NowStation;
        MissionInfo NowMission;
        List<MissionInfo> missionList;
        List<StationData> stationList = new List<StationData>();
        int selectedID = 0;
        PictureForm pictureForm;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FunClass.PostWaveData(IP + UrlOfWaveDataInput, NowStation,NowMission);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            IP = textBox1.Text;
            pictureForm = new PictureForm();
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
            pictureForm.ChangePicture(FunClass.GetPicture(IP + UrlOfWavePicture, NowStation.stationID.ToString("00") + "DAY", 1,NowMission.forecastFilesHead),NowStation);
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
            pictureForm.ChangePicture(FunClass.GetPicture(IP + UrlOfWavePicture, NowStation.stationID.ToString("00") + "DAY", 1, NowMission.forecastFilesHead),NowStation);
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
                row = dataGridView1.CurrentCell.RowIndex + 1;
                col = dataGridView1.CurrentCell.ColumnIndex + 1;
                NowStation = stationList[row-1];
            }
            catch { NowStation = stationList[0]; }
            //当所选行不是预报值时
            if (col - 2 <= 0 | col - 2 > 7)
            {
                //不做图片操作
                pictureForm.ChangePicture(null,null);
            }
            else
            {
                if (col - 2 > 0 & col - 2 <= NowStation.forecastPrescription)
                {
                    //变更图片
                    pictureForm.ChangePicture(FunClass.GetPicture(IP + UrlOfWavePicture, NowStation.stationID.ToString("00") + "DAY", col - 2, NowMission.forecastFilesHead),NowStation);
                }
                else
                {
                    //超出预报时效
                    pictureForm.ChangePicture(ClientApp.Properties.Resources.OutOfPrescription,null);
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
                if (temp.forecastPrescription < 5) dataGridView1[7, i].ReadOnly = false;
                if (temp.forecastPrescription < 4) dataGridView1[6, i].ReadOnly = false;
                if (temp.forecastPrescription < 3) dataGridView1[5, i].ReadOnly = false;
                if (temp.forecastPrescription < 2) dataGridView1[4, i].ReadOnly = false;
                i++;
            }
        }

        private void RefreshDataGridView()
        {
            //更新datagridview，加载站点信息
            dataGridView1.DataSource = stationList;
            //dataGridView1.DataMember = "stationName";
            dataGridView1.Columns["stationID"].Visible = false;
            dataGridView1.Columns["coordinateX"].Visible = false;
            dataGridView1.Columns["coordinateY"].Visible = false;
            dataGridView1.Columns["missionID"].Visible = false;
            dataGridView1.Columns["photoHead"].Visible = false;
            dataGridView1.Columns["forecastPrescription"].Visible = false;
            //dataGridView1.Columns["forecastFilesHead"].Visible = false;
            dataGridView1.Columns["stationName"].ReadOnly = true;
            dataGridView1.Columns["stationName"].HeaderCell.Value = "站点名称";
            dataGridView1.Columns["forecastValue1"].HeaderCell.Value = "24小时预报";
            dataGridView1.Columns["forecastValue2"].HeaderCell.Value = "48小时预报";
            dataGridView1.Columns["forecastValue3"].HeaderCell.Value = "72小时预报";
            dataGridView1.Columns["forecastValue4"].HeaderCell.Value = "96小时预报";
            dataGridView1.Columns["forecastValue5"].HeaderCell.Value = "120小时预报";
            setReadOnly(stationList);
            
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if(NowMission!=null&NowMission!=null)
            {
                try
                {
                    if (!bool.Parse(FunClass.PostWaveData(IP + UrlOfWaveDataInput, NowStation, NowMission)))
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
    }
}
