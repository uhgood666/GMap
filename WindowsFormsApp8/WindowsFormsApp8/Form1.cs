using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Net.Http.Headers;
using GMap.NET;
using GMap.NET.WindowsForms.ToolTips;
using System.Drawing.Drawing2D;
using System.IO;
 
namespace WindowsFormsApp8
{
    public partial class Form1 : Form
    {
        private GMapOverlay markersOverlay = new GMapOverlay("markers");
        private static readonly HttpClient client = new HttpClient();
        private object gmap;

        public Form1()
        {
            InitializeComponent();
        }

        private static async Task ProcessRepositories()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(
            //new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            //client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

            var stringTask = client.GetStringAsync("http://maps.googleapis.com/maps/api/directions/xml?origin={0},&destination={1}&sensor=false&language=ru&mode={2}");

            var msg = await stringTask;
            Console.Write(msg);
        }
        static async Task Main(string[] args)
        {
            await ProcessRepositories();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //показывать или скрывать красный крестик в центре
            gMapControl1.ShowCenter = false;

            gMapControl1.Bearing = 0;
            // Для перетаскивания ПКМ 
            gMapControl1.CanDragMap = true;

            //Тоже перетаскивание ЛКМ только
            gMapControl1.DragButton = MouseButtons.Left;

            gMapControl1.GrayScaleMode = true;

            //MarkersEnabled - Если параметр установлен в True,
            //любые маркеры, заданные вручную будет показаны.
            //Если нет, они не появятся.
            gMapControl1.MarkersEnabled = true;

            //Значение максимального приближения.
            gMapControl1.MaxZoom = 18;

            //Значение минимального приближения.
            gMapControl1.MinZoom = 2;

            //Центр приближения/удаления для
            //курсора мыши.
            gMapControl1.MouseWheelZoomType =
                GMap.NET.MouseWheelZoomType.MousePositionAndCenter;

            //Отказываемся от негативного режима.
            gMapControl1.NegativeMode = false;

            //Разрешаем полигоны.
            gMapControl1.PolygonsEnabled = true;

            //Разрешаем маршруты.
            gMapControl1.RoutesEnabled = true;

            //Скрываем внешнюю сетку карты
            //с заголовками.
            gMapControl1.ShowTileGridLines = false;

            //При загрузке карты будет использоваться 
            //2х кратное приближение.
            gMapControl1.Zoom = 12;

            gMapControl1.Overlays.Add(markersOverlay);

            //Указываем что будем использовать карты Google.
            gMapControl1.MapProvider =
                GMap.NET.MapProviders.GMapProviders.GoogleMap;
            GMap.NET.GMaps.Instance.Mode =
                GMap.NET.AccessMode.ServerOnly;
            gMapControl1.Position = new GMap.NET.PointLatLng(58.5966, 49.6601);// точка в центре карты при открытии (Киров)
            gMapControl1.MouseClick += new MouseEventHandler(gMapControl1_MouseClick);
        }
        private void gMapControl1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                GMapOverlay PositionsForUser = new GMapOverlay("PositionsForUser");
                gmap.Overlays.Add(PositionsForUser);

                // Широта - latitude - lat - с севера на юг
                double x = gmap.FromLocalToLatLng(e.X, e.Y).Lng;
                // Долгота - longitude - lng - с запада на восток
                double y = gmap.FromLocalToLatLng(e.X, e.Y).Lat;

                textBox2.Text = x.ToString();
                textBox1.Text = y.ToString();

                // Добавляем метку на слой
                GMarkerGoogle MarkerWithMyPosition = new GMarkerGoogle(new PointLatLng(y, x), GMarkerGoogleType.blue_pushpin);
                MarkerWithMyPosition.ToolTip = new GMapRoundedToolTip(MarkerWithMyPosition);
                MarkerWithMyPosition.ToolTipText = "Метка пользователя";
                PositionsForUser.Markers.Add(MarkerWithMyPosition);

                // Сохранение наших координат (текстовик, цсв, бд, текстбокс, строки, лист)
                FileStream fileStream = new FileStream(@"Date\Координаты_ВыбранныеПользователем.txt", FileMode.Append, FileAccess.Write);
                StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.GetEncoding(1251));
                streamWriter.WriteLine(y + ";" + x);
                streamWriter.Close();
            }
        }
        private void gMapControl1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                PointLatLng point = gMapControl1.FromLocalToLatLng(e.X, e.Y);
                GMapMarker marker = new GMarkerGoogle(point, GMarkerGoogleType.green);
                markersOverlay.Markers.Add(marker);
            }
        }
        private void gMapControl1_MouseMove(object sender, MouseEventArgs e) //Отслеживание положения мыши на карте, и вывод координат в правый нижний угол.
        {

            label1.Text = gMapControl1.FromLocalToLatLng(e.X, e.Y).Lat.ToString() + " " + gMapControl1.FromLocalToLatLng(e.X, e.Y).Lng.ToString();

        }
        
        private void button3_Click_1(object sender, EventArgs e)
        {
            markersOverlay.Markers.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            string url = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=true_or_false&language=ru",
    Uri.EscapeDataString(textBox3.Text));//Запрос к API геокодирования Google.
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);//Получаем ответ от интернет-ресурса.
            System.Net.WebResponse response = request.GetResponse();//Экземпляр класса System.IO.Stream для чтения данных из интернет-ресурса.
            System.IO.Stream dataStream = response.GetResponseStream();//Инициализируем новый экземпляр класса System.IO.StreamReader для указанного потока.
            System.IO.StreamReader sreader = new System.IO.StreamReader(dataStream);//Инициализируем новый экземпляр класса System.IO.StreamReader для указанного потока.
            string responsereader = sreader.ReadToEnd();//Считывает поток от текущего положения до конца.
            response.Close();//Закрываем поток ответа.
            System.Xml.XmlDocument xmldoc = new System.Xml.XmlDocument();
            xmldoc.LoadXml(responsereader);

            if (xmldoc.GetElementsByTagName("status")[0].ChildNodes[0].InnerText == "OK")
            {
                //Получение широты и долготы.
                System.Xml.XmlNodeList nodes = xmldoc.SelectNodes("//location");
                //Переменные широты и долготы.
                double latitude = 0.0;
                double longitude = 0.0;
                //Получаем широту и долготу.
                foreach (System.Xml.XmlNode node in nodes)
                {
                    latitude =
                       System.Xml.XmlConvert.ToDouble(node.SelectSingleNode("lat").InnerText.ToString());
                    longitude =
                       System.Xml.XmlConvert.ToDouble(node.SelectSingleNode("lng").InnerText.ToString());
                }
                //Варианты получения информации о найденном объекте.
                //Вариант 1.
                string formatted_address =
                   xmldoc.SelectNodes("//formatted_address").Item(0).InnerText.ToString();
                //Создаем новый список маркеров, с указанием компонента 
                //в котором они будут использоваться и названием списка.
                GMapOverlay markersOverlay = new GMapOverlay("markers");//МОЁ
                GMap.NET.WindowsForms.Markers.GMarkerGoogle markerG = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(new GMap.NET.PointLatLng(latitude, longitude), GMap.NET.WindowsForms.Markers.GMarkerGoogleType.blue_pushpin);
                markerG.ToolTip = new GMap.NET.WindowsForms.ToolTips.GMapRoundedToolTip(markerG);
                markersOverlay.Markers.Add(markerG);//Добавляем маркеры в список маркеров.
                gMapControl1.Overlays.Clear();//Очищаем список маркеров компонента.
                gMapControl1.Overlays.Add(markersOverlay);//Добавляем в компонент, список маркеров.
                gMapControl1.Refresh();//Обновляем карту.
            }
        }
      
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void gMapControl1_Load(object sender, EventArgs e)
        {

        }

        private void GMapRoute_MyRoute(GMapRoute item, MouseEventArgs e)
        {
            //Маршрут
            GMapRoute MyRoute = new GMapRoute("MyRoute");

            //Рандомный цвет
            Random rnd = new Random();
            Color randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            MyRoute.Stroke = new Pen(randomColor);

            // Жирность - ширина пера
            MyRoute.Stroke.Width = 10;

            //Отображение маршрута-
            MyRoute.Stroke.DashStyle = System.Drawing.Drawing2D.DashStyle.Custom;

            //Отображение конца линии старта и финиша

            MyRoute.Stroke.StartCap = LineCap.RoundAnchor;
            MyRoute.Stroke.EndCap = LineCap.RoundAnchor;
            
        }
        GMapOverlay NewMarsh1 = new GMapOverlay("ggg");
        private void button4_Click(object sender, EventArgs e)
        {
            var x1 = Convert.ToSingle(textBox1.Text);
            var y1 = Convert.ToSingle(textBox2.Text);

            var x2 = Convert.ToSingle(textBox4.Text);
            var y2 = Convert.ToSingle(textBox5.Text);

            using (var stream = new FileInfo(@"Date\south-fed-district-latest.routerdb").Open(FileMode.Open))
            {
                var routeDb = RouterDb.Deserialize(stream);
                var profile = Vehicle.Car.Shortest();
                var router = new Router(routeDb);

                var start = router.Resolve(profile, x1, y1);
                var end = router.Resolve(profile, x2, y2);

                var route = router.Calculate(profile, start, end);

                List<PointLatLng> put = new List<PointLatLng>();

                foreach (var itm in route.Shape)
                {
                    PointLatLng pt = new PointLatLng(Convert.ToDouble(itm.Latitude), Convert.ToDouble(itm.Longitude));
                    put.Add(pt);
                }
                GMapRoute r = new GMapRoute(put, "fggg");
                
                r.Stroke = new Pen(Color.Green, 1);

                NewMarsh1.Routes.Add(r);
            }
        }
    }
    }


