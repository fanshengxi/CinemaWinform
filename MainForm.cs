using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Demo0425
{
    public partial class MainForm : Form
    {
        string[] parentTitles = new string[] { "影院信息浏览", "影片浏览", "新增影片" , "场次计划列表", "新增播放场次", "售票" , "统计分析" };
        Dictionary<string, int> titleDic = new Dictionary<string, int>();
        DataClasses1DataContext dataClasses1 = null;
        int cinemaId = -1;
        string cinemaName = "";
        string filmName = "";
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            treeView1.ExpandAll();//展开treeview所有项
            treeView1.SelectedNode = treeView1.Nodes[0].Nodes[0];//默认选中第一项
            this.Hide_Title();//隐藏tabcontrol中的所有tab标签

            this.fill_titleDic();

            dataClasses1 = new DataClasses1DataContext();

            this.getTab0();
        }

        private void getTab0()
        {
            if (dataGridView1.RowCount > 1) dataGridView1.Rows.Clear();
            var tab1List = dataClasses1.t_cinema
                .Select(item => new
                {
                    影院ID = item.cinema_id,
                    影院名称 = item.cinema_name,
                    电话 = item.cinema_tel,
                    地址 = item.cinema_addr
                })
                .OrderBy(tc => tc.影院ID);

            foreach (var item in tab1List)
            {
                DataGridViewRow row = new DataGridViewRow();
                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                cell.Value = item.影院ID;
                row.Cells.Add(cell);

                DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
                cell2.Value = item.影院名称;
                row.Cells.Add(cell2);

                DataGridViewTextBoxCell cell3 = new DataGridViewTextBoxCell();
                cell3.Value = item.电话;
                row.Cells.Add(cell3);

                DataGridViewTextBoxCell cell4 = new DataGridViewTextBoxCell();
                cell4.Value = item.地址;
                row.Cells.Add(cell4);

                DataGridViewButtonCell cell5 = new DataGridViewButtonCell();
                cell5.Value = "查看放映厅";
                row.Cells.Add(cell5);

                dataGridView1.Rows.Add(row);
            }
        }

        private void fill_titleDic()
        {
            titleDic.Add("影院管理", 0);
            titleDic.Add("影片管理", 1);
            titleDic.Add("播放管理", 3);
            titleDic.Add("统计分析", 6);
        }

        private void Hide_Title()
        {
            for (int i = 0; i < tabControl1.TabPages.Count; i++)
            {
                tabControl1.TabPages[i].Text = "";
            }
            tabControl1.SizeMode = TabSizeMode.Fixed;
            tabControl1.ItemSize = new System.Drawing.Size(0, 1);
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string listSelected = ((TreeView)sender).SelectedNode.Text;
            Console.WriteLine(listSelected);

            if (titleDic.ContainsKey(listSelected))
            {
                Dictionary<string, int>.KeyCollection keyCol = titleDic.Keys;
                foreach (string key in keyCol)
                {
                    if (listSelected.Equals(key)) tabControl1.SelectedIndex = titleDic[key];
                }
            }
            else if (parentTitles.Contains(listSelected))
            {
                for (int i = 0; i < parentTitles.Count(); i++)
                {
                    if (listSelected.Equals(parentTitles[i])) tabControl1.SelectedIndex = i;
                }
            }
            else
            {
                Console.WriteLine("标签栏错误!!!!!");
                return;
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4)
            {
                cinemaId = int.Parse(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                cinemaName = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();

                Console.WriteLine("你选择的影院:"+cinemaName);

                tabControl1.SelectedIndex = 7;
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabControl tabControl = (TabControl)sender;

            switch (tabControl.SelectedIndex)
            {
                case 0:
                    cinemaId = -1;//重置
                    cinemaName = "";
                    this.getTab0();
                    break;

                case 1:
                    this.getTab1();
                    break;

                case 2:
                    //清空所有新增界面内容
                    textBox2.Clear();
                    if (comboBox3.SelectedItem != null)
                    {
                        comboBox3.SelectedIndex = 0;
                    }
                    
                    richTextBox1.Clear();
                    monthCalendar1.SelectionStart = DateTime.Now;
                    if (pictureBox1.Image != null)
                    {
                        pictureBox1.Image = null;
                        pictureBox1.Refresh();
                    }
                    this.getTab2();
                    break;

                case 3:
                    this.getTab3();
                    break;

                case 7:
                    this.getTab7();
                    treeView1.SelectedNode = null;
                    break;

                case 8:
                    textBox3.Clear();
                    if (comboBox4.SelectedItem != null)
                        comboBox4.SelectedIndex = 0;
                    richTextBox2.Clear();
                    monthCalendar2.SelectionStart = DateTime.Now;
                    if (pictureBox2.Image != null)
                    {
                        pictureBox2.Image = null;
                        pictureBox2.Refresh();
                    }
                    
                    this.getTab8();
                    treeView1.SelectedNode = null;
                    break;

                case 9:
                    if (comboBox8.SelectedItem != null)
                        comboBox8.SelectedIndex = 0;
                    if (comboBox9.SelectedItem != null)
                        comboBox9.SelectedIndex = 0;
                    if (comboBox10.SelectedItem != null)
                        comboBox10.SelectedIndex = 0;
                    textBox4.Clear();
                    this.getTab9();
                    treeView1.SelectedNode = null;
                    break;

                default:

                    break;
            }
        }

        private void getTab9()
        {
            if (combo8Data == null) combo8Data = getFilmList();
            comboBox8.DataSource = combo8Data;

            if (combo9Data == null) combo9Data = getCinemaList();
            comboBox9.DataSource = combo9Data;
        }

        private List<string> getRoomList(string cName)
        {
            List<String> list = dataClasses1.t_room.Where(tr=>dataClasses1.t_cinema.Where(tc=>tc.cinema_name.Equals(cName)).First<t_cinema>().cinema_id==tr.cinema_id).Select(tc => tc.room_name).ToList<string>();
            list.Insert(0, "放映厅列表");

            return list;
        }

        DateTime selectPlanDate = DateTime.Now;
        private void getTab3()
        {
            if (combo5Data == null) combo5Data = getCinemaList();
            comboBox5.DataSource = combo5Data;

            if (combo6Data == null) combo6Data = getFilmList();
            comboBox6.DataSource = combo6Data;

            if (combo7Data == null) combo7Data = getPlanStatusList();
            comboBox7.DataSource = combo7Data;

            monthCalendar3.SelectionStart= DateTime.Now;

            //&& DateTime.Compare(Convert.ToDateTime(selectPlanDate.ToString("yyyy-MM-dd")), p.end_time) < 0
            //List<t_plan> list = dataClasses1.t_plan.Where(p=>DateTime.Compare(Convert.ToDateTime(selectPlanDate.ToString("yyyy-MM-dd")),p.begin_time)>0).OrderBy(tp=>tp.begin_time).ToList();
            List<t_plan> list = dataClasses1.t_plan.Where(p => p.begin_time>selectPlanDate).OrderBy(tp => tp.begin_time).ToList();
            List<tPlanViewData> list2 = getTPlanList(list);

            fillDataInPlan(list2);
        }

        private void fillDataInPlan(List<tPlanViewData> list)
        {
            if(dataGridView4.RowCount>0) dataGridView4.Rows.Clear();
            
            foreach (tPlanViewData tp in list)
            {
                DataGridViewRow row = new DataGridViewRow();
                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                cell.Value = tp.cinemaName;
                row.Cells.Add(cell);

                DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
                cell2.Value = tp.filmName;
                row.Cells.Add(cell2);

                DataGridViewTextBoxCell cell3 = new DataGridViewTextBoxCell();
                cell3.Value = tp.roomName;
                row.Cells.Add(cell3);

                DataGridViewTextBoxCell cell4 = new DataGridViewTextBoxCell();
                cell4.Value = tp.start;
                row.Cells.Add(cell4);

                DataGridViewTextBoxCell cell5 = new DataGridViewTextBoxCell();
                cell5.Value = tp.end;
                row.Cells.Add(cell5);

                DataGridViewTextBoxCell cell6 = new DataGridViewTextBoxCell();
                cell6.Value = tp.money;
                row.Cells.Add(cell6);

                DataGridViewTextBoxCell cell7 = new DataGridViewTextBoxCell();
                cell7.Value = tp.tickets;
                row.Cells.Add(cell7);

                DataGridViewButtonCell cell8 = new DataGridViewButtonCell();
                cell8.Value = tp.status;
                row.Cells.Add(cell8);

                if (!string.IsNullOrEmpty(tp.command))
                {
                    DataGridViewButtonCell cell9 = new DataGridViewButtonCell();
                    cell9.Value = tp.command;
                    row.Cells.Add(cell9);
                }
                dataGridView4.Rows.Add(row);
            }
        }

        private List<tPlanViewData> getTPlanList(List<t_plan> list)
        {
            List<tPlanViewData> list2 = new List<tPlanViewData>();
            foreach (t_plan item in list)
            {
                tPlanViewData tp = new tPlanViewData
                {
                    planId = item.plan_id,
                    cinemaName = dataClasses1.t_cinema.Where(tc => tc.cinema_id == dataClasses1.t_room.Where(tr => tr.room_id == item.room_id).First<t_room>().cinema_id).First<t_cinema>().cinema_name,
                    roomName = dataClasses1.t_room.Where(tr => tr.room_id == item.room_id).First<t_room>().room_name,
                    filmName = dataClasses1.t_film.Where(tf => tf.film_id == item.film_id).First<t_film>().film_name,
                    start = item.begin_time,
                    end = item.end_time,
                    money = (float)item.ticket_price,
                    status = dataClasses1.t_dict.Where(td => td.enum_type == 4 && td.enum_id == item.plan_status).First<t_dict>().enum_value,
                };
                tp.tickets = tp.status.Equals("未开始") ? dataClasses1.t_seat.Where(ts=>ts.room_id == dataClasses1.t_room.Where(troom=>troom.room_name.Equals(tp.roomName)).First<t_room>().room_id).Count() - dataClasses1.t_reserve.Where(tr => tr.plan_id == tp.planId && tr.reserve_status == 1).Count() + dataClasses1.t_reserve.Where(tr => tr.plan_id == tp.planId && tr.reserve_status == 2).Count() : 0;
                if (tp.status.Equals("未开始"))
                {
                    if (tp.tickets > 0) tp.command = "售票";
                }
                else if (tp.status.Equals("已开始")) tp.command = "结束播放";
                else if (tp.status.Equals("已结束")) tp.command = "";
                list2.Add(tp);
            }
            return list2;
        }

        struct tPlanViewData
        {
            public int planId { get; set; }
            public string cinemaName { get; set;}
            public string filmName { get; set; }
            public string roomName { get; set; }
            public DateTime start { get; set; }
            public DateTime end { get; set; }
            public float money { get; set; }
            public int tickets { get; set; }
            public string status { get; set; }
            public string command { get; set; }
        }

        private void getTab8()
        {
            if (combo4Data == null) combo4Data = getFilmTypeList();
            comboBox4.DataSource = combo4Data;

            t_film selectedFilm = dataClasses1.t_film.Where(film => film.film_name.Equals(filmName)).First<t_film>();
            filmModifyId = selectedFilm.film_id;
            textBox3.Text = selectedFilm.film_name;
            richTextBox2.Text = selectedFilm.film_desc;
            comboBox4.SelectedIndex = selectedFilm.film_type_id + 1;
            if (selectedFilm.film_up_time != null)
            {
                monthCalendar2.SelectionStart = (DateTime)selectedFilm.film_up_time;
            }
            
            switch (selectedFilm.film_status)
            {
                case 0:
                    radioButton3.Checked = true;
                    break;

                case 1:
                    radioButton4.Checked = true;
                    break;

                case 2:
                    radioButton5.Checked = true;
                    break;

                default:
                    break;
            }

            if (selectedFilm.film_pic_url != null)
            {
                pictureBox2.Image = Image.FromFile(selectedFilm.film_pic_url);
                picUrl2 = selectedFilm.film_pic_url;
            }

        }

        List<string> combo1Data = null;
        List<string> combo2Data = null;
        List<string> combo3Data = null;
        List<string> combo4Data = null;
        List<string> combo5Data = null;
        List<string> combo6Data = null;
        List<string> combo7Data = null;
        List<string> combo8Data = null;
        List<string> combo9Data = null;
        List<string> combo10Data = null;
        private void getTab2()
        {
            if (combo3Data == null) combo3Data = getFilmTypeList();
            comboBox3.DataSource = combo3Data;
        }

        int FilmListHead = 0;
        int FilmNum = 0;
        IQueryable<FilmItem> filmList = null;

        private List<String> getFilmList()
        {
            List<String> list = dataClasses1.t_film.Select(tf => tf.film_name).ToList<string>();
            list.Insert(0, "影片列表");

            return list;
        }

        private List<String> getPlanStatusList()
        {
            List<String> list = dataClasses1.t_dict.Where(td => td.enum_type == 4).Select(td => td.enum_value).ToList<string>();
            list.Insert(0, "状态");

            return list;
        }

        private List<String> getCinemaList()
        {
            List<String> list = dataClasses1.t_cinema.Select(tc => tc.cinema_name).ToList<string>();
            list.Insert(0, "影院列表");

            return list;
        }

        private List<String> getFilmTypeList()
        {
            List<String> list = dataClasses1.t_dict.Where(td => td.enum_type == 1).Select(td => td.enum_value).ToList<string>();
            list.Insert(0, "影片类型");

            return list;
        }

        private List<String> getFilmStatusList()
        {
            List<String> list = dataClasses1.t_dict.Where(td => td.enum_type == 2).Select(td => td.enum_value).ToList<string>();
            list.Insert(0, "状态");

            return list;
        }

        private void getTab1()
        {
            if (combo1Data == null) combo1Data = getFilmTypeList();
            comboBox1.DataSource = combo1Data;

            if (combo2Data == null) combo2Data = getFilmStatusList();
            comboBox2.DataSource = combo2Data;

            //初始化数据表格
            filmList = dataClasses1.t_film
                .Select(tf => new FilmItem 
                { name = tf.film_name,
                    type = dataClasses1.t_dict.Where(td => td.enum_type == 1 && td.enum_id == tf.film_type_id)
                                        .First().enum_value,
                    img = Image.FromFile(tf.film_pic_url),
                    desc = tf.film_desc,
                    status = dataClasses1.t_dict.Where(tdick => tdick.enum_type == 2 && tdick.enum_id == tf.film_status)
                                        .First().enum_value
                });
            FilmNum = filmList.Count();

            this.fillDataInList();
        }

        private void fillDataInList()
        {
            if (dataGridView3.RowCount > 1) dataGridView3.Rows.Clear();
            foreach (FilmItem item in (filmList.Skip(FilmListHead*10).Take(10)))
            {
                DataGridViewRow row = new DataGridViewRow();
                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                cell.Value = item.name;
                row.Cells.Add(cell);

                DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
                cell2.Value = item.type;
                row.Cells.Add(cell2);

                DataGridViewImageCell cell3 = new DataGridViewImageCell();
                cell3.Value = item.img;
                row.Height = 150;
                cell3.ImageLayout = DataGridViewImageCellLayout.Zoom;
                row.Cells.Add(cell3);

                DataGridViewTextBoxCell cell4 = new DataGridViewTextBoxCell();
                cell4.Value = item.desc.Substring(0, item.desc.Length >= 10 ? 10 : 0) + "...";
                row.Cells.Add(cell4);

                DataGridViewTextBoxCell cell5 = new DataGridViewTextBoxCell();
                cell5.Value = item.status;
                row.Cells.Add(cell5);

                DataGridViewButtonCell cell6 = new DataGridViewButtonCell();
                cell6.Value = item.status.Equals("已上架") ? "下架" : "上架";
                row.Cells.Add(cell6);

                DataGridViewButtonCell cell7 = new DataGridViewButtonCell();
                cell7.Value = "修改";
                row.Cells.Add(cell7);

                dataGridView3.Rows.Add(row);
            }
        }

        private void getTab7()
        {
            var roomList = dataClasses1.t_room
                .Where(tr => tr.cinema_id == cinemaId)
                .Select(item =>
                new {
                    影院ID = item.cinema_id,
                    影院名称 = cinemaName,
                    放映厅ID = item.room_id,
                    放映厅名称 = item.room_name,
                    座位数量 = dataClasses1.t_seat.Where(ts => ts.room_id==item.room_id).Count()
                })
                .OrderBy(tr => tr.放映厅ID);
            dataGridView2.DataSource = roomList;
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox1.Text)) textBox1.Text = "影片名称";
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text.Equals("影片名称"))
            {
                textBox1.Text = "";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!button4.Enabled) button4.Enabled = true;
            button3.Enabled = (--FilmListHead >= 1);
            this.fillDataInList();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(!button3.Enabled) button3.Enabled = true;
            button4.Enabled = (++FilmListHead < (FilmNum%10==0? FilmNum % 10: (FilmNum % 10) +1));
            this.fillDataInList();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            filmList = dataClasses1.t_film
                .Select(tf => new FilmItem 
                { name = tf.film_name,
                    type = dataClasses1.t_dict.Where(td => td.enum_type == 1 && td.enum_id == tf.film_type_id)
                                        .First().enum_value,
                    img = Image.FromFile(tf.film_pic_url),
                    desc = tf.film_desc,
                    status = dataClasses1.t_dict.Where(tdick => tdick.enum_type == 2 && tdick.enum_id == tf.film_status)
                                        .First().enum_value
                }).Where(tf =>tf.name.Contains(textBox1.Text.Equals("影片名称")?"":textBox1.Text)
                    &&tf.type.Contains(comboBox1.SelectedValue.ToString().Equals("影片类型")?"": comboBox1.SelectedValue.ToString()) 
                    && tf.status.Contains(comboBox2.SelectedValue.ToString().Equals("状态") ? "" : comboBox2.SelectedValue.ToString()));
            FilmNum = filmList.Count();
            FilmListHead = 0;
            this.fillDataInList();

            button3.Enabled = false;
            if (filmList.Count() < 10) button4.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //跳转到新增界面
            tabControl1.SelectedIndex = 2;
            treeView1.SelectedNode = null;
        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string filmName = ((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value.ToString();
            if (e.ColumnIndex == 5) // 上架、下架按钮
            {
                string operate = ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                string msg = "你确定要" + operate + "影片:" + filmName +"吗？";
                DialogResult dialogResult = MessageBox.Show(msg, "提示", MessageBoxButtons.OKCancel);
                if (dialogResult == DialogResult.OK)
                {
                    t_film tf = dataClasses1.t_film.Where(t => t.film_name.Equals(filmName)).First<t_film>();
                    tf.film_status = dataClasses1.t_dict.Where(td => td.enum_type==2 && td.enum_value.Equals("已"+operate)).First<t_dict>().enum_id;
                    dataClasses1.SubmitChanges();
                    FilmListHead = 0;
                    this.fillDataInList();
                }
            }
            else if(e.ColumnIndex == 6) // 修改按钮
            {
                this.filmName = filmName;
                //跳转到修改界面
                tabControl1.SelectedIndex = 8;
            }
        }

        string picUrl = "";
        private void button5_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "请选择上传的图片";
            openFileDialog.Filter = "|*.png;*.jpg";
            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                picUrl = openFileDialog.FileName;
                pictureBox1.Image = Image.FromFile(picUrl);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "请选择上传的视频";
            openFileDialog.Filter = "|*.mp4";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Console.WriteLine(openFileDialog.FileName);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //提交
            string fName = textBox2.Text;
            //string type = comboBox3.SelectedItem.ToString();
            string desc = richTextBox1.Text;
            int status = radioButton1.Checked ? 0 : 1;

            int type = comboBox3.SelectedIndex;
                      
            DateTime dateTime = monthCalendar1.SelectionStart;

            if(String.IsNullOrEmpty(fName)||String.IsNullOrEmpty(desc)|| type == 0)
            {
                MessageBox.Show("请填写新增电影信息!");
                return;
            }

            if (String.IsNullOrEmpty(picUrl))
            {
                MessageBox.Show("请选择上传图片!");
                return;
            }

            if (dateTime < DateTime.Now)
            {
                MessageBox.Show("上架时间不得早于今天");
                return;
            }

            t_film newFilm = new t_film()
            {
                film_desc = desc,
                film_name = fName,
                film_type_id = type-1,
                film_status = status,
                film_pic_url = picUrl,
                create_time = DateTime.Now,
                film_up_time = dateTime
            };

            dataClasses1.t_film.InsertOnSubmit(newFilm);
            dataClasses1.SubmitChanges();

            Console.WriteLine("提交新增影片:"+fName);
            tabControl1.SelectedIndex = 1;
            treeView1.SelectedNode = treeView1.Nodes[1].Nodes[0];
            //tabControl1.TabPages[2].Controls.Clear();
        }

        int filmModifyId = -1;

        private void button8_Click(object sender, EventArgs e)
        {
            //修改

            t_film modifyFilm = dataClasses1.t_film.Where(tf => tf.film_id == filmModifyId).First<t_film>();

            string fName = textBox3.Text;
            string desc = richTextBox2.Text;
            int status = -1;

            if (radioButton3.Checked) status = 0;
            else if (radioButton4.Checked) status = 1;
            else status = 2;

            int type = comboBox4.SelectedIndex;

            DateTime dateTime = monthCalendar2.SelectionStart;

            if (String.IsNullOrEmpty(fName) || String.IsNullOrEmpty(desc) || type == 0)
            {
                //Console.WriteLine(fName)

                MessageBox.Show("请将该电影信息填写完整!");
                return;
            }

            if (String.IsNullOrEmpty(picUrl2))
            {
                MessageBox.Show("请选择上传图片!");
                return;
            }

            if (dateTime < DateTime.Now)
            {
                MessageBox.Show("上架时间不得早于今天");
                return;
            }

            modifyFilm.film_desc = desc;
            modifyFilm.film_name = fName;
            modifyFilm.film_type_id = type-1;
            modifyFilm.film_status = status;
            modifyFilm.film_pic_url = picUrl2;
            modifyFilm.create_time = DateTime.Now;
            modifyFilm.film_up_time = dateTime;

            dataClasses1.SubmitChanges();

            Console.WriteLine("提交修改影片:" + fName);
            tabControl1.SelectedIndex = 1;
            treeView1.SelectedNode = treeView1.Nodes[1].Nodes[0];
            filmModifyId = -1;
            
        }

        string picUrl2 = "";
        private void button10_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "请选择上传的图片";
            openFileDialog.Filter = "|*.png;*.jpg";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                picUrl2 = openFileDialog.FileName;
                pictureBox2.Image = Image.FromFile(picUrl2);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "请选择上传的视频";
            openFileDialog.Filter = "|*.mp4";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Console.WriteLine(openFileDialog.FileName);
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            //查询
            selectPlanDate = monthCalendar3.SelectionStart;

            Console.WriteLine(selectPlanDate);
            Console.WriteLine(selectPlanDate.AddDays(1));
            //Console.WriteLine(DateTime.Now);

            List<t_plan> list = dataClasses1.t_plan
                .Where(p => p.begin_time > selectPlanDate && p.begin_time < selectPlanDate.AddDays(1))
                .OrderBy(tp => tp.begin_time).ToList();
            if (comboBox5.SelectedIndex != 0)
            {
                int cid = dataClasses1.t_cinema.Where(tc => tc.cinema_name.Equals(comboBox5.Text)).First<t_cinema>().cinema_id;
                list = list.Where(p => dataClasses1.t_room.Where(tr => tr.room_id == p.room_id).First<t_room>().cinema_id == cid).ToList<t_plan>();
            }
            if (comboBox6.SelectedIndex != 0)
            {
                int fid = dataClasses1.t_film.Where(tf => tf.film_name.Equals(comboBox6.Text)).First<t_film>().film_id;
                list = list.Where(p => p.film_id == fid).ToList<t_plan>();
            }
            if (comboBox7.SelectedIndex != 0)
            {
                int sid = dataClasses1.t_dict.Where(td => td.enum_value.Equals(comboBox7.Text)).First<t_dict>().enum_id;
                list = list.Where(p=>p.plan_status==sid).ToList<t_plan>();
            }
            List<tPlanViewData> list2 = getTPlanList(list);

            fillDataInPlan(list2);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            //新增
            tabControl1.SelectedIndex = 9;
        }

        private void button14_Click(object sender, EventArgs e)
        {
            treeView1.SelectedNode = treeView1.Nodes[2].Nodes[0];
        }

        private void button13_Click(object sender, EventArgs e)
        {
            DateTime startTime = Convert.ToDateTime(dateTimePicker1.Text);
            DateTime endTime = Convert.ToDateTime(dateTimePicker2.Text);
            if (comboBox8.SelectedIndex == 0 || comboBox9.SelectedIndex == 0 || comboBox10.SelectedIndex == 0||string.IsNullOrEmpty(textBox4.Text))
            {
                MessageBox.Show("请完成信息填写");
                return;
            }
            if (endTime < startTime)
            {
                MessageBox.Show("结束时间不得早于开始时间");
                return;
            }

            int c = dataClasses1.t_plan.Where(tp => tp.room_id == dataClasses1.t_room.Where(tr => tr.room_name.Equals(comboBox10.SelectedText)).First<t_room>().room_id).Where(plan=> (startTime > plan.begin_time && startTime < plan.end_time) || (endTime > plan.begin_time || endTime < plan.end_time) || (startTime < plan.begin_time && endTime > plan.end_time)).Count();

            if (c != 0)
            {
                MessageBox.Show("该时间段已有播放计划，请重新选择时间段");
            }
        }

        private void comboBox9_SelectedIndexChanged(object sender, EventArgs e)
        {
            Console.WriteLine(comboBox9.SelectedText);
            if (combo10Data == null) combo10Data = getRoomList(comboBox9.SelectedText);
            comboBox10.DataSource = combo10Data;
        }
    }
    struct FilmItem
    {
        public string name { get; set; }//影片名称
        public string type { get; set; }//影片类别
        public Image img { get; set; }//图片
        public string desc { get; set; }//详情
        public string status { get; set; }//状态
    }
}
