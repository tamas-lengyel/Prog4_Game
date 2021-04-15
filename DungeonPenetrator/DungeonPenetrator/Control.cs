using Logic;
using Model;
using Renderer;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DungeonPenetrator
{
    internal class Control : FrameworkElement
    {
        IGameLogic gameLogic;
        ILoadingLogic loadigLogic;
        IGameModel model;
        ISaveGameRepository saveGameRepo;
        IHighscoreRepository highscoreRepo;
        GameRenderer renderer;

        public Control()
        {
            Loaded += Control_Loaded;
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            model = new GameModel();
            saveGameRepo = new SaveGameRepository();
            highscoreRepo = new HighscoreRepository();
            loadigLogic = new LoadingLogic(model, saveGameRepo, highscoreRepo);
            model = loadigLogic.Play();
            gameLogic = new GameLogic(model);
            renderer = new GameRenderer(model);

            

            Window win = Window.GetWindow(this);
            if (win != null)
            {
                win.KeyDown += Win_KeyDown;
                MouseLeftButtonDown += Control_MouseLeftButtonDown;
            }

            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (renderer != null)
            {
                drawingContext.DrawDrawing(renderer.BuildDrawing());
            }
        }

        private void Control_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Win_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
