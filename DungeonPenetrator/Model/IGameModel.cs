using Model.Active;
using Model.Passive;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Model
{
    public interface IGameModel
    {
        double GameWidth { get; }
        double GameHeight { get; }
        double TileSize { get; }
        Point LevelExit { get; }
        int LevelCounter { get; set; }

        BossEnemy Boss { get; set; }
        Player MyPlayer { get; set; }
        List<FlyingEnemy> FlyingMonster { get; set; }
        List<ShootingEnemy> ShootingMonster { get; set; }
        List<TrackingEnemy> TrackingMonster { get; set; }
        List<LavaProp> Lava { get; set; }
        List<WaterProp> Water { get; set; }
        List<WallProp> Wall { get; set; }
        List<Powerups> Powerup { get; set; }

        public bool LevelFinished { get; set; }
    }
}
