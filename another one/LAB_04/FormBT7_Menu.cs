using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;

namespace LAB_04
{
    public partial class FormBT7_Menu : Form
    {
        public FormBT7_Menu()
        {
            InitializeComponent();
        }

        //Khai báo
        private string url_All = "https://nt106.uitiot.vn/api/v1/monan/all";
        private string url_My = "https://nt106.uitiot.vn/api/v1/monan/my-dishes";
        private string url_getAll;
        List<Food> foodList = new List<Food>();
        public class FoodResponse
        {
            public List<Food> data { get; set; }
            public Pagination pagination { get; set; }
        }

        public class Pagination
        {
            public int current { get; set; }
            public int pageSize { get; set; }
            public int total { get; set; }
        }

        public class FoodRequest
        {
            public int current { get; set; }
            public int pageSize { get; set; }
        }

        public class Food
        {
            string id { get; set; }
            public string ten_mon_an { get; set; }
            public long gia { get; set; }
            public string mo_ta { get; set; }
            public string hinh_anh { get; set; }
            public string dia_chi { get; set; }
            public string nguoi_dong_gop { get; set; }
        }

        //Hàm lấy danh sách tất cả món ăn
        private async void GetFood(int a, int b, string url, FlowLayoutPanel flp)
        {
            cb_crPage.Text = a.ToString();
            cb_pageSize.Text = b.ToString();
            var request = new FoodRequest
            {
                current = a,
                pageSize = b,
            };

            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            using (HttpClient client = new HttpClient())
            {
                var accessToken = GetInfoUser()[2];
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                var response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    var monanResponse = JsonConvert.DeserializeObject<FoodResponse>(responseString);

                    foodList = monanResponse.data;
                    DisplayMenu(foodList, flp);
                }
                else
                    throw new Exception("Failed to fetch data");
            }
        }

        //Hàm lấy ngẫu nhiên 1 món ăn
        private async void GetRandFood()
        {
            Random rd = new Random();
            int id_rd = rd.Next(0, 100);
            var request = new
            {
                id = 1 //id_rd
            };
            url_getAll = $"https://nt106.uitiot.vn/api/v1/monan/{1}";
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            using (HttpClient client = new HttpClient())
            {
                var accessToken = GetInfoUser()[2];
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                var response = await client.PostAsync(url_getAll, content);
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    var monanResponse = JsonConvert.DeserializeObject<Food>(responseString);

                    DisplayFood(monanResponse.ten_mon_an, monanResponse.gia, monanResponse.dia_chi, monanResponse.nguoi_dong_gop, monanResponse.hinh_anh);
                }
                else
                    MessageBox.Show("Get fail");
            }
        }

        //Hiển thị các panel món ăn
        private void DisplayMenu(List<Food> list, FlowLayoutPanel flp)
        {
            lb_welcome.Text = $"Welcome, {GetInfoUser()[0]}";
            if (list.Count == 0)
            {
                MessageBox.Show("Danh sách rỗng!");
            }
            else
            {
                foreach (Food food in list)
                {
                    //Tạo Panel
                    Panel panel = new Panel();
                    panel.Size = new System.Drawing.Size(762, 154);

                    //
                    flp.Controls.Add(panel);

                    //Nội dung Panel
                    FormBT7_DisplayFood form = new FormBT7_DisplayFood(food.ten_mon_an, food.gia, food.dia_chi, food.nguoi_dong_gop, food.hinh_anh);
                    form.TopLevel = false;
                    form.FormBorderStyle = FormBorderStyle.None;
                    form.Dock = DockStyle.Fill;
                    panel.Controls.Add(form);
                    panel.Tag = form;
                    form.BringToFront();
                    form.Show();
                }
            }

        }

        //Hiển thị món ăn lấy ngẫu nhiên
        private void DisplayFood(string name, long price, string addr, string contri, string img)
        {
            FormBT7_DisplayFood f = new FormBT7_DisplayFood(name, price, addr, contri, img);
            f.Show();
        }

        //Hàm lấy accessToken
        public String[] GetInfoUser()
        {
            String[] info = null;
            try
            {
                using (StreamReader sr = new StreamReader("UserInfo.txt"))
                {
                    string line = sr.ReadLine();
                    string[] parts = line.Split(" - ");
                    info = parts;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return info;
        }

        public void LoadForm(int a, int b)
        {
            flowLayoutPanel_All.Controls.Clear();
            flowLayoutPanel_MyAdd.Controls.Clear();
            GetFood(a, b, url_All, flowLayoutPanel_All);
            GetFood(a, b, url_My, flowLayoutPanel_MyAdd);
        }

        //Tìm món ăn theo trang
        private void btn_Search_Click(object sender, EventArgs e)
        {

            int pagecurr = int.Parse(cb_crPage.Text);
            int pagesize = int.Parse(cb_pageSize.Text);
            LoadForm(pagecurr, pagesize);
        }

        //Thêm món ăn mới
        private void button_AddFood_Click(object sender, EventArgs e)
        {
            FormBT7_AddFood form = new FormBT7_AddFood();
            form.f = this;
            form.Show();
        }

        private void FormBT7_Menu_Load(object sender, EventArgs e)
        {
            LoadForm(1, 5);
        }

        private void button_FindRand_Click(object sender, EventArgs e)
        {
            GetRandFood();
        }
    }
}
