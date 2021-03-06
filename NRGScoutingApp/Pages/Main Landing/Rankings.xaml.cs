﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace NRGScoutingApp {
    /*ADD Ranking Chooser Replacement for iOS 
     *like a picker acts as the distribution center to choose the type
     */
    public partial class Rankings : INotifyPropertyChanged {
        public Rankings () {
            InitializeComponent ();
            rankPicker.SelectedIndex = 0;
        }

        public static string teamSend;

        MatchFormat.CHOOSE_RANK_TYPE rankChoice;

        //Initializes the ranking object
        Ranker mainRank = new Ranker (Preferences.Get ("matchEventsString", ""));

        void rankTypeDelta (object sender, System.EventArgs e) {
            switch (rankPicker.SelectedIndex) {
                case 0:
                    rankChoice = MatchFormat.CHOOSE_RANK_TYPE.overallRank;
                    break;
                case 1:
                    rankChoice = MatchFormat.CHOOSE_RANK_TYPE.pick1; //Hatch
                    break;
                case 2:
                    rankChoice = MatchFormat.CHOOSE_RANK_TYPE.pick2; //Cargo
                    break;
                case 3:
                    rankChoice = MatchFormat.CHOOSE_RANK_TYPE.climb; //Climb
                    break;
                case 4:
                    rankChoice = MatchFormat.CHOOSE_RANK_TYPE.drop1; //Lvl1
                    break;
                case 5:
                    rankChoice = MatchFormat.CHOOSE_RANK_TYPE.drop2; //lvl2
                    break;
                case 6:
                    rankChoice = MatchFormat.CHOOSE_RANK_TYPE.drop3; //lvl3
                    break;
                default:
                    rankChoice = MatchFormat.CHOOSE_RANK_TYPE.overallRank;
                    break;
            }
            updateEvents ();
        }

        protected override void OnAppearing () {
            updateEvents ();
        }

        //Updates events with given enum
        private void updateEvents () {
            //Updates string data from matches
            mainRank.setData (Preferences.Get ("matchEventsString", ""));
            //Gets all data and sets it into ascending order based on each team's average time
            Dictionary<string, double> x = mainRank.getRank (rankChoice);
            var y = from pair in x
            orderby pair.Value ascending
            select pair;
            setListVisibility (y.Count ());
            List<RankStruct> ranks = new List<RankStruct> ();
            foreach (var s in y) {
                ranks.Add (new RankStruct { Key = s.Key, Value = s.Value, color = getTeamColor (s.Key) });
                Console.WriteLine (getTeamColor (s.Key).ToString ());
            }
            listView.ItemsSource = ranks;
        }

        public class RankStruct {
            public string Key { get; set; }
            public double Value { get; set; }
            public Color color { get; set; }
        }

        private Color getTeamColor (String team) {
            return mainRank.getColors () [team];
        }

        /*
         * Sets the visibility of the list based on boolean and the sad error opposite
         * So if list.IsVisible = true, then sadNoMatch.IsVisible = false
         */
        private void setListVisibility (int setList) {
            listView.IsVisible = setList > 0;
            sadNoMatch.IsVisible = !listView.IsVisible;
        }

        async void teamClicked (object sender, Xamarin.Forms.ItemTappedEventArgs e) {
            var x = (listView.ItemsSource as IEnumerable<RankStruct>).ToList ();
            String item = x.Find (y => y.Equals (e.Item)).Key;
            teamSend = item;
            await Navigation.PushAsync (new RankingsDetailView (mainRank.returnTeamTimes (item)) { Title = item });
        }
    }
}