using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BlackjackGA.Representation;
using BlackjackGA.Engine;
using BlackjackGA.Utils;

namespace BlackjackGA
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            BasicStrategy basicStrategy = new BasicStrategy();
            TestConditions conditions = new TestConditions();

            double average;
            double deviation;
            double coef;

            var game = new Game(basicStrategy, conditions);
            double money = game.GetStrategyScore(conditions.NumHandsToPlay);
            /*
            Console.WriteLine("Usted ha ganado: " + money + " pesos");
            double x = (money / conditions.NumHandsToPlay*conditions.BetSize);
            Console.WriteLine("El house edge es: " + x);*/

            game.GetStatistics(out average, out deviation, out coef);
            Console.WriteLine("El average es: " + average);
            double x = (average / conditions.NumHandsToPlay * conditions.BetSize);
            Console.WriteLine("El house edge es: " + x);
            Console.WriteLine("La desviacion es: " + deviation);
            Console.WriteLine("El coef de variacion es: " + coef);
        }
    }
}
