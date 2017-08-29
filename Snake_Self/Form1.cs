using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Snake_Self
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen; //форма по центру экрана
            this.TopMost = true; //форма поверх остального
            //this.DoubleBuffered = true; // для прорисовки двойная буферизация именно формы - не пригодилась, давала заметные тормоза, графика почти отсутствовала
            //посему навесил двойную буферизацию на сам метод графики e.Graphics.
            
            this.Paint += new PaintEventHandler(Paint_Field); //привязываем отрисовку поля
            this.Paint += new PaintEventHandler(Program_Paint); // привязываем обработчик прорисовки формы
            this.KeyDown += new KeyEventHandler(Program_KeyDown); // привязываем обработчик нажатий на кнопки
          
            timer.Interval = 200; // таймер срабатывает раз в 200 милисекунд
            timer.Tick += new EventHandler(timer_Tick); // привязываем обработчик таймера
            timer.Start(); // запускаем таймер

            ReDraw.Interval = 20; //тут нужно поиграться, это обновление соответствует 50 кадрам в секунду, возможно излишне и можно подогнать под "человеческие" 25 кадров = 40мс
            ReDraw.Tick += new EventHandler(re_draw);
            ReDraw.Start();

            // делаем змею из трех сегментов, с начальными координатами внизу и по-центру формы
            snake.Add(new coord(W / 2, H - 3));
            snake.Add(new coord(W / 2, H - 2));
            snake.Add(new coord(W / 2, H - 1));

            apple = new coord(rand.Next(W), rand.Next(H)); // координаты яблока
        }


        //отрисовка поля действия
        void Paint_Field(object sender, PaintEventArgs e)
        {
            DoubleBuffered = true;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed; //поставил на всякий, чтоб много не жрало ресурсов т.к. игра динамическая
            
            e.Graphics.DrawImage(Properties.Resources.Snake_Field as Bitmap, 110 , 20 , 680, 680); //картинка всего поля, условные 20 на 20 квадратов
            e.Graphics.DrawRectangle(new Pen(Color.Black), 100, 10, 700, 700); //просто рамка "для красоты"
        }


        //класс для координат
        public class coord
        {
            public int X;
            public int Y;
            public coord(int x, int y)
            {
                X = x; Y = y;
            }
        }


        //============ общие переменные
        Timer timer = new Timer();//таймер, на котором повешены игровые действия
        Timer ReDraw = new Timer();//таймер для перерисовки, по-сути избыточен, используется "для красоты" - при работе с паузой выводим графику паузы, не стопаем этот таймер
        
        Random rand = new Random();
        
        // условная ширина "поля действия" в клетках, высота и размер клетки в пикселях - т.к. исходную картинку тайла нашёл и вырезал под 34*34, то 34 пикселя
        public int W = 20, H = 20, S = 34;
        // собственно змея: список сегментов(нулевой индекс в списке - голова змеи)  
        List<coord> snake = new List<coord>();
        coord apple; // координаты яблока
        int way = 0; // направление движения змеи: 0 - вверх, 1 - вправо, 2 - вниз, 3 - влево
        int apples = 0; // количество собранных яблок
        int stage = 1; // уровень игры
        int score = 0; // набранные очки в игре
        bool Paused = false; //флаг паузы
        //============


        //обработка клавиатуры
        void Program_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Up:
                    if (way != 2)
                        way = 0;
                    break;
                case Keys.Right:
                    if (way != 3)
                        way = 1;
                    break;
                case Keys.Down:
                    if (way != 0)
                        way = 2;
                    break;
                case Keys.Left:
                    if (way != 1)
                        way = 3;
                    break;
                case Keys.P:
                    if (!Paused)
                    {
                        Paused = !Paused;
                        timer.Stop();
                        //ReDraw.Stop();
                    }                        
                    else
                    {
                        Paused = !Paused;
                        timer.Start();
                        //ReDraw.Start();
                    }
                    break;
            }
        }
        //конец обработки клавиатуры


        //перерисовка формы
        void re_draw(object sender, EventArgs e)
        {
            Invalidate();
        }
        //перерисовка формы


        //разобраться с обработкой графики паузы, при снятии с паузы долго не пропадает надпись
        void Game_Paused(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawString("Paused", new Font("Arial", 72, FontStyle.Bold), Brushes.Black, new Point(300, 300));
        //тут код для графики во время паузы, пока не задействован - даёт задержку
        }


        void timer_Tick(object sender, EventArgs e)
        {                     
            // запоминаем координаты головы змеи
            int x = snake[0].X, y = snake[0].Y;
            // в зависимости от направления вычисляем где будет голова на следующем шаге
            // сделал чтобы при достижении края формы голова появлялась с противоположной стороны 
            // и змея продолжала движение
            // направление движения змеи: 0 - вверх, 1 - вправо, 2 - вниз, 3 - влево
            switch (way)
            {
                case 0:
                    y--;
                    if (y < 0 && x == W-1)
                    {
                        y = 0;
                        x--;
                        way = 3;
                    }
                    if (y < 0)
                    { 
                    y = 0;
                    x++;
                    way = 1;
                    }
                    break;
                case 1:
                    x++;
                    if (x >= W && y == H-1)
                    {
                        x = W - 1;
                        y--;
                        way = 0;
                    }
                    if (x >= W)
                    {
                        x = W-1;
                        y++;
                        way = 2;
                    }
                    break;
                case 2:
                    y++;

                    if (y >= H && x==0)
                    {
                        y = H - 1;
                        x++;
                        way = 1;
                    }


                    if (y >= H)
                    {
                        y = H - 1;
                        x--;
                        way = 3;
                    }
                    break;
                case 3:
                    x--;
                    if (x < 0 && y == 0)
                    {
                        x = 0;
                        y++;
                        way = 2;
                    }
                    if (x < 0)
                    {
                        x = 0;
                        y--;
                        way = 0;
                    
                    }
                    
                    break;
            }
            coord c = new coord(x, y); // сегмент с новыми координатами головы
            snake.Insert(0, c); // вставляем его в начало списка сегментов змеи(змея выросла на один сегмент)
            if (snake[0].X == apple.X && snake[0].Y == apple.Y) // если координаты головы и яблока совпали
            {
                apple = new coord(rand.Next(W), rand.Next(H)); // располагаем яблоко в новых случайных координатах
                for (int m = 1; m <= snake.Count - 1; m++)
                {
                    if (snake[m].X == apple.X && snake[m].Y == apple.Y)
                    { apple = new coord(rand.Next(W), rand.Next(H)); }
                }

                apples++; // увеличиваем счетчик собранных яблок
                score += stage; // увеличиваем набранные очки в игре: за каждое яблоко прибавляем количество равное номеру уровня
                if (apples % 5 == 0) // после каждого пятого яблока
                {
                    stage++; // повышаем уровень
                    if (timer.Interval > 10) //обязательно проверка >10, иначе таймер выдаст ArgumentOutOfRangeException
                        timer.Interval -= 10; // и уменьшаем интервал срабатывания яблока
                }
            }
            else // если координаты головы и яблока не совпали - убираем последний сегмент змеи(т.к. ранее добавляли новую голову)
                snake.RemoveAt(snake.Count - 1);

            for (int k = 1; k <= snake.Count-1; k++)
            {
                if (snake[0].X == snake[k].X && snake[0].Y == snake[k].Y)//обрабатываем столкновение с головой - если одновременно координаты головы и другого сегмента совпали.. 
                {
                    timer.Stop();
                    ReDraw.Stop();//стопаем все таймеры, они нам при гейм овере не нужны
                    System.Console.Beep(440, 250);//сигнал при столкновении
                    System.Threading.Thread.Sleep(2000);//2 секунды до перезапуска
                    
                    Application.Restart();//собственно, сам перезапуск после 2 сек
                 }
            }
            
            //Invalidate(); // перерисовываем, т.е. идет вызов Program_Paint //вынес отдельно
        }

       
        void Program_Paint(object sender, PaintEventArgs e)
        {
            DoubleBuffered = true;
         
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
            e.Graphics.DrawImage(Properties.Resources.Tail_Apple as Bitmap, new Rectangle(110 + apple.X * S, 20 + apple.Y * S, S, S));

            for (int i = 0; i < snake.Count; i++)
                e.Graphics.DrawImage(Properties.Resources.Tail_Snake as Bitmap, new Rectangle(110 + snake[i].X * S, 20 + snake[i].Y * S, S, S));

            // сообщение о количестве собранных яблок, уровне и количестве очков
            string state = "Apples:" + apples.ToString() + "\n" +
                "Stage:" + stage.ToString() + "\n" + "Score:" + score.ToString();
            // выводим это сообщение в левом верхнем углу
            e.Graphics.DrawString(state, new Font("Arial", 12, FontStyle.Italic), Brushes.Black, new Point(5, 5));

            //тут простая графика для паузы, вынос отдельно даёт задержку
            if (Paused)
            {                
                e.Graphics.DrawString("Paused", new Font("Arial", 72, FontStyle.Bold), Brushes.Black, new Point(260, 300));
            }    
        }
        

    }
}
