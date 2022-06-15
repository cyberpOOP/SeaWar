using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SeaWarFinal
{
    public partial class Form1 : Form
    {
        private const int mapSize = 11;
        private const int cellSize = 30;
        private string alphabet = " ABCDEFGHJI";

        private int[,] myMap = new int[mapSize, mapSize];
        private int[,] enemyMap = new int[mapSize, mapSize];
        private Button[,] myButtons = new Button[mapSize, mapSize];
        private Button[,] enemyButtons = new Button[mapSize, mapSize];

        private int enemyCount;
        private int playerCount;

        private bool isPlaying = false;
        private bool final = false;

        private int playerShips;

        private Enemy enemy;

        public Form1()
        {
            this.Text = "Sea Battle";
            this.Width = (mapSize+3)*cellSize * 2;
            this.Height = (mapSize+3)*cellSize;

            InitializeComponent();
            Init();

        }

        private void Restart(object sender, EventArgs e)
        {
            this.Controls.Clear();
            Init();
        }
        private void Init()
        {
            isPlaying = false;
            final = false;
            playerShips = 20;
            enemy = new Enemy(myMap, enemyMap, myButtons);
            CreateUI();
            CreateMaps();
        }

        private void CreateUI()
        {
            Label playerL = new Label();
            Label enemy = new Label();

            playerL.Text = "Player's map";
            enemy.Text = "Enemy's map";
            playerL.Location = new Point(mapSize * cellSize / 2, mapSize * cellSize + 20);
            enemy.Location = new Point(380 + mapSize * cellSize / 2, mapSize * cellSize + 20);

            Button gameStart = new Button();
            gameStart.Text = "Play";
            gameStart.Location = new Point((playerL.Location.X + enemy.Location.X) / 2, mapSize * cellSize + 20);
            gameStart.Click += new EventHandler(Start);

            this.Controls.Add(playerL);
            this.Controls.Add(enemy);
            this.Controls.Add(gameStart);
        }
        private void Start(object sender, EventArgs e)
        {
            if(playerShips == 0)
                isPlaying = true;
            final = false;
        }

        private void CreateMaps()
        {

            //my map
            for(int i=0; i<mapSize; i++)
            {
                for(int j=0; j<mapSize; j++)
                {
                    myMap[i, j] = 0;
                    Button button = new Button();
                    button.BackColor = Color.White;
                    button.Size = new Size(cellSize, cellSize);
                    button.Location = new Point(50+i*cellSize, j*cellSize);
                    button.Click += new EventHandler(Ships);
                    myButtons[i, j] = button;

                    if(i==0 || j == 0)
                    {
                        button.BackColor = Color.Gray;
                        if (i == 0 && j > 0)
                            button.Text = j.ToString();
                        if(j == 0 && i>0)
                            button.Text = alphabet[i].ToString();
                    }   

                    this.Controls.Add(button);
                    
                }
            }

            int[,] tempMap = new int[mapSize, mapSize];
            //enemy map
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    tempMap[i, j] = 0;
                    Button button = new Button();
                    button.BackColor = Color.White;
                    button.Size = new Size(cellSize, cellSize);
                    button.Location = new Point(400 + i * cellSize, j * cellSize);
                    button.Click += new EventHandler(Shoot);
                    enemyButtons[i, j] = button;

                    if (i == 0 || j == 0)
                    {
                        button.BackColor = Color.Gray;
                        if (i == 0 && j > 0)
                            button.Text = j.ToString();
                        if (j == 0 && i > 0)
                            button.Text = alphabet[i].ToString();
                    }

                    this.Controls.Add(button);
                    
                }
            }
            
            enemyMap = enemy.Map(tempMap);
        }

        private void Ships(object sender, EventArgs e)
        {
            Button clickedbutton = sender as Button;

            if ((playerShips > 0 || clickedbutton.BackColor == Color.Blue) && !final)
            {
                if (!isPlaying && clickedbutton.BackColor != Color.Gray)
                {
                    //Console.WriteLine("My points:" + (clickedbutton.Location.X - 50) / cellSize + " " + clickedbutton.Location.Y / cellSize);

                    if (myMap[(clickedbutton.Location.X - 50) / cellSize, clickedbutton.Location.Y / cellSize] == 0)
                    {
                        clickedbutton.BackColor = Color.Blue;
                        // debug: check for proper coordinates
                        //myButtons[(clickedbutton.Location.X - 50) / cellSize, clickedbutton.Location.Y / cellSize].Text = "1";
                        myMap[(clickedbutton.Location.X - 50) / cellSize, clickedbutton.Location.Y / cellSize] = 1;
                        playerShips--;
                    }
                    else
                    {
                        clickedbutton.BackColor = Color.White;
                        myMap[(clickedbutton.Location.X - 50) / cellSize, clickedbutton.Location.Y / cellSize] = 0;
                        playerShips++;
                    }
                }
            }
        }

        private void Shoot(object sender, EventArgs e)
        {
            Button clickedbutton = sender as Button;
            bool playerTurn = playerShoot(enemyMap, clickedbutton); // false->bot move, true->player move

            Button start = new Button();
            start.Text = "Play again";
            start.Location = new Point(355, 380);
            start.Click += new EventHandler(Restart);

            Label result = new Label();
            result.Location = new Point(start.Location.X+100, start.Location.Y);

            enemyCount = 0;
            playerCount = 0;

            for(int i=0; i<mapSize; i++)
                for(int j=0; j<mapSize; j++)
                {
                    if (enemyMap[i,j] == 1) enemyCount++;
                    if (myMap[i, j] == 1) playerCount++;
                }

            if(isPlaying && !final)
                if (enemyCount == 0)
                {
                    result.Text = "Player won";
                    this.Controls.Add(result);
                    this.Controls.Add(start);

                    final = true;
                    isPlaying = false;
                }
                else if (playerCount == 0)
                {
                    for (int i = 0; i < mapSize; i++)
                        for (int j = 0; j < mapSize; j++)
                            enemyMap[i, j] = -1;
                    result.Text = "Bot won";
                    this.Controls.Add(result);
                    this.Controls.Add(start);

                    final = true;
                    isPlaying = false;
                }
                else
                {
                    if (!playerTurn)
                    {
                        //Test();
                        enemy.Shoot();
                    }
                    else
                    {
                        playerShoot(enemyMap, clickedbutton);
                    }
                }

            
        }

        public bool playerShoot(int[,] map, Button clickedbutton)
        {
            bool hit = false;
            int delta = 0;

            if (clickedbutton.BackColor == Color.Gray)
                return true;
            if (isPlaying)
            {
                //Console.WriteLine((clickedbutton.Location.X - 400) / cellSize +" " + clickedbutton.Location.Y / cellSize);
                if (clickedbutton.Location.X > 400)
                {
                    delta = 400;
                    
                    if (map[(clickedbutton.Location.X - delta) / cellSize, clickedbutton.Location.Y / cellSize] == 1)
                    {
                        hit = true;
                        map[(clickedbutton.Location.X - delta) / cellSize, clickedbutton.Location.Y / cellSize] = -2;
                        clickedbutton.BackColor = Color.Red;
                        clickedbutton.Text = "X";
                    }
                    else if (map[(clickedbutton.Location.X - delta) / cellSize, clickedbutton.Location.Y / cellSize] == 0)
                    {
                        hit = false;
                        clickedbutton.BackColor = Color.Black;
                        map[(clickedbutton.Location.X - delta) / cellSize, clickedbutton.Location.Y / cellSize] = -1;
                    }
                    else if (map[(clickedbutton.Location.X - delta) / cellSize, clickedbutton.Location.Y / cellSize] < 0)
                        hit = true;
                }
            }


            return hit;
        }

        

    }
}
