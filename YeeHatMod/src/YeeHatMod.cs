using DuckGame;
using System.Reflection;
using System.Collections.Generic;
using System.Threading;

[assembly: AssemblyTitle("Yee Hat")]
[assembly: AssemblyCompany("KodenejmBerni")]
[assembly: AssemblyDescription("This mod adds more Yee to your life.")]
[assembly: AssemblyVersion("1.1.0.0")]

namespace DG.YeeHatMod
{
    internal class Main : IAutoUpdate
    {
        public void Update()
        {
            if (Level.current != null)
            {
                IEnumerable<Thing> teamHatList = Level.current.things[typeof(TeamHat)];
                foreach (TeamHat teamHat in teamHatList)
                    if (teamHat.team != null && !(teamHat is YeeTeamHat) && teamHat.team.name == YeeTeamHat.teamName)
                    {
                        YeeTeamHat newHat = new YeeTeamHat(teamHat.x, teamHat.y, teamHat.team);
                        Duck duck = teamHat.equippedDuck;
                        Level.Add(newHat);
                        if (duck != null)
                        {
                            duck.Equip(newHat, false);
                            duck.Fondle(newHat);

                            if (Level.current is TeamSelect2)
                            {
                                TeamSelect2 lobby = (TeamSelect2)Level.current;
                                foreach (ProfileBox2 profileBox in lobby._profiles)
                                    if (ReferenceEquals(profileBox.duck, duck))
                                    {
                                        profileBox._hatSelector.hat = newHat;
                                        break;
                                    }
                            }
                        }
                        Level.Remove(teamHat);
                    }
            }
        }
    }
    internal class YeeTeamHat : TeamHat
    {

        public static string teamName = "Yee";
        public static string imagePath = "yee_hat.png";
        public static string musicalSoundPath = "yee_musical_sound.wav";
        public static string originalSoundPath = "yee_original_sound.wav";


        public YeeTeamHat(float xpos, float ypos, Team t)
            : base(xpos, ypos, t)
        { }

        public override void Terminate()
        {
            if (equippedDuck != null)
                equippedDuck._netQuack = new NetSoundEffect("quack");
            base.Terminate();
        }

        public override void Equip(Duck duck)
        {
            base.Equip(duck);
            duck._netQuack = new NetSoundEffect(Mod.GetPath<YeeHatMod>(originalSoundPath));
        }

        public override void UnEquip()
        {
            if (equippedDuck != null)
                equippedDuck._netQuack = new NetSoundEffect("quack");
            base.UnEquip();
        }

        public override void Quack(float volume, float pitch)
        {
            if (equippedDuck.crouch)
            {
                SFX.Play(Mod.GetPath<YeeHatMod>(musicalSoundPath), volume, pitch);
            }
            else
            {
                SFX.Play(Mod.GetPath<YeeHatMod>(originalSoundPath), volume, pitch);
            }
        }
    }

    public class YeeHatMod : Mod
    {
        internal static Main main = new Main();

        public YeeHatMod() : base() { }

        protected override void OnPostInitialize()
        {
            Team newTeam_temp = Team.Deserialize(Mod.GetPath<YeeHatMod>(YeeTeamHat.imagePath));
            Team newTeam = new Team(YeeTeamHat.teamName, Mod.GetPath<YeeHatMod>(YeeTeamHat.imagePath));
            newTeam._capeTexture = newTeam_temp.capeTexture;
            newTeam._rockTexture = newTeam_temp.rockTexture;
            newTeam._customParticles = newTeam_temp.customParticles;
            newTeam._metadata = newTeam_temp.metadata;

            Teams.AddExtraTeam(newTeam);
            base.OnPostInitialize();

            Thread thread = new Thread(ExecuteOnceLoaded);
            thread.Start();
        }

        private void ExecuteOnceLoaded()
        {
            while (Level.current == null || !(Level.current is TitleScreen || Level.current is TeamSelect2))
                Thread.Sleep(200);

            AutoUpdatables.Add(main);

        }

    }
}
