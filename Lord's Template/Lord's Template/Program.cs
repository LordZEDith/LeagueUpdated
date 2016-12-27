using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;
using SharpDX;
using static LeagueSharp.Common.Items;
using System.Drawing;

namespace Lord_s_Template
{
    class Program
    {
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnLoad;
        }

        public static Obj_AI_Hero p;

        private static string News = "Welcome to ";

        public static Obj_AI_Hero Player = ObjectManager.Player;

        public static Spell Q, W, E, R;

        public static Menu Config;

        public static Orbwalking.Orbwalker orbwalker;


        public static void Game_OnLoad(EventArgs args)

        {
            if (Player.ChampionName != "")
            {
                return;
            }
            //Spells
            Q = new Spell(SpellSlot.Q, 135f);
            W = new Spell(SpellSlot.W, 625f);
            E = new Spell(SpellSlot.E, 1000f);
            R = new Spell(SpellSlot.R, 375f);

            E.SetSkillshot(0.3f, 30, 500, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.195f, 100, 1750, false, SkillshotType.SkillshotCone);

            Game.PrintChat("<font size='30'>Lord's </font> <font color='#b756c5'>by LordZEDith</font>");
            Game.PrintChat("<font color='#b756c5'>NEWS: </font>" + Program.News);

            //Events
            MainMenu();
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_Update;
        }

        public static void Game_Update(EventArgs args)
        {
            //  if (Player.IsDead) return;
            //Activates Combo
            switch (orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:

                    Combos();

                    break;


                //Activates Laneclear
                case Orbwalking.OrbwalkingMode.LaneClear:

                    Laneclear();

                    break;


            }
        }

        public static void Drawing_OnDraw(EventArgs args)
        {
            //Draw W
            if (Config.Item("DrawW").GetValue<bool>() && W.IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, 630, Color.Black);
            }

            //Draw E
            if (Config.Item("DrawE").GetValue<bool>() && E.IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, 900, Color.BlueViolet);
            }

            if (Config.Item("DrawR").GetValue<bool>() && R.IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, 475, Color.Red);
            }
        }





        public static void Combos()
        {
            //Target
            var Target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

            //Q Combo
            if (Config.Item("Q").GetValue<bool>() && Q.IsReady() && Target.IsValidTarget())
            {
                Q.Cast(Target);
            }

            if (Config.Item("Q").GetValue<bool>() && Q.IsReady() && Target.IsValidTarget())
            {
                Q.Cast(Target);
            }

            //W Combo
            if (Config.Item("W").GetValue<bool>() && W.IsReady() && Target.IsValidTarget())
            {
                W.Cast(Target);
            }
            if (Config.Item("W").GetValue<bool>() && W.IsReady() && Target.IsValidTarget())
            {
                W.Cast(Target);
            }

            //E Combo                
            if (E.IsReady() && Config.Item("E").GetValue<bool>() && Target.IsValidTarget(E.Range))
            {
                switch (Config.Item("EMode", true).GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        {

                            E.Cast(Target);


                        }
                        break;
                    case 1:
                        {

                            E.Cast(Target);


                        }
                        break;

                }
            }

            //R Combo
            if (Config.Item("R").GetValue<bool>() && R.IsReady() && Target.IsValidTarget() && Target.HealthPercent < Config.Item("RHP").GetValue<Slider>().Value && Config.Item("r" + Target.ChampionName.ToLower(), true).GetValue<bool>())
            {
                R.Cast(Target);
            }

        }

        public static void Laneclear()
        {

            if (Q.IsReady() && Config.Item("WLane").GetValue<bool>())
            {
                var minions = MinionManager.GetMinions(ObjectManager.Player.Position, W.Range, MinionTypes.All,
                MinionTeam.NotAlly);

                var minioncount = W.GetLineFarmLocation(minions);
                if (minions == null || minions.Count == 0)
                {
                    return;
                }

                if (minioncount.MinionsHit >= Config.Item("Min").GetValue<Slider>().Value)
                {
                    W.Cast(minioncount.Position);
                }
            }
        }



        public static void MainMenu()
        {
            //MainMenu
            Config = new Menu("Lord's ", "Lord's ", true).SetFontStyle(FontStyle.Bold, SharpDX.Color.SkyBlue);
            Config.AddToMainMenu();

            //Orbwalker Menu
            Menu orbwalkerMenu = new Menu("Orbwalker", "Orbwalker");
            orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            Config.AddSubMenu(orbwalkerMenu);

            var targetSelectorMenu = new Menu("Target Selector", "TargetSelector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            //Combo
            var Combo = new Menu("Combo", "Combo");
            {
                Combo.AddItem(new MenuItem("Combo Settings:", "Combo Settings:"));
                Combo.AddItem(new MenuItem("Q", "Use Q in Combo")).SetValue(true);
                Combo.AddItem(new MenuItem("W", "Use W in Combo")).SetValue(true);
                Combo.AddItem(new MenuItem("EMode", "Use E Mode:", true).SetValue(new StringList(new[] { "", "" })));
                Combo.AddItem(new MenuItem("E", "Use E in Combo")).SetValue(true);
                Combo.AddItem(new MenuItem("R", "Use R in Combo")).SetValue(true);
                Combo.AddItem(new MenuItem("RHP", "Use R if Enemy % HP").SetValue(new Slider(50)));

            }

            Config.AddSubMenu(Combo);

            //Ultimate Menu
            var whitelist = new Menu("Ultimate Whitelist", "Ultimate Whitelist");
            {
                foreach (var hero in HeroManager.Enemies)
                {
                    whitelist.AddItem(new MenuItem("r" + hero.ChampionName.ToLower(), "Use [R]:  " + hero.ChampionName, true).SetValue(
                            true));
                }

            }
            Config.AddSubMenu(whitelist);
            //Clear
            var Clear = new Menu("Lane Clear", "Lane Clear");
            {
                Clear.AddItem(new MenuItem("Laneclear Settings:", "Laneclear Settings:"));
                Clear.AddItem(new MenuItem("QLane", "Use Q in Laneclear")).SetValue(false);
                Clear.AddItem(new MenuItem("WLane", "Use W in Laneclear")).SetValue(true);
                Clear.AddItem(new MenuItem("Min", "[W] Min. Minion Count").SetValue(new Slider(3, 1, 5)));
            }
            Config.AddSubMenu(Clear);



            //DrawMenu
            var DrawMenu = new Menu("Drawings", "Drawings");
            {
                DrawMenu.AddItem(new MenuItem("Draw Settings:", "Draw Settings:"));
                DrawMenu.AddItem(new MenuItem("DrawW", "Draw W Range")).SetValue(true);
                DrawMenu.AddItem(new MenuItem("DrawE", "Draw E Range")).SetValue(true);
                DrawMenu.AddItem(new MenuItem("DrawR", "Draw R Range")).SetValue(true);

            }
            Config.AddSubMenu(DrawMenu);

        }

    }
}

