using Model.Active;
using Model.Passive;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Model
{
    public interface IGameModel
    {
        double GameWidth { get; set; }
        double GameHeight { get; set; }
        double TileSize { get; set; }
        Point LevelExit { get; set; }
        int PlayerHealhBar { get; set; }
        int LevelCounter { get; set; }

        BossEnemy Boss { get; set; }
        Player MyPlayer { get; set; }
        List<FlyingEnemy> FlyingFucker { get; set; }
        List<ShootingEnemy> ShootingFucker { get; set; }
        List<TrackingEnemy> TrackingFucker { get; set; }
        List<LavaProp> Lava { get; set; }
        List<WaterProp> Water { get; set; }
        List<WallProp> Wall { get; set; }
        List<Powerups> Powerup { get; set; }
    }
}
