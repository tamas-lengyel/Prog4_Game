﻿using Model.Active;
using Model.Passive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Model
{
    class GameModel : IGameModel
    {
        public double GameWidth { get; set; }
        public double GameHeight { get; set; }
        public double TileSize { get; set; }
        public Point LevelExit { get; set; }
        public int PlayerHealhBar { get; set; }
        public int LevelCounter { get; set; }
        public BossEnemy Boss { get; set; }
        public Player MyPlayer { get; set; }
        public List<FlyingEnemy> FlyingFucker { get; set; }
        public List<ShootingEnemy> ShootingFucker { get; set; }
        public List<TrackingEnemy> TrackingFucker { get; set; }
        public List<LavaProp> Lava { get; set; }
        public List<WaterProp> Water { get; set; }
        public List<WallProp> Wall { get; set; }
        public List<Powerups> Powerup { get; set; }

        public GameModel(double w, double h)
        {
            GameHeight = h;
            GameWidth = w;

            LevelExit = new Point(w / 2, 0);
        }
    }
}
