using DuckGame;
using System.Reflection;

[assembly: AssemblyTitle("Yee Hat")]
[assembly: AssemblyCompany("KodenejmBerni")]
[assembly: AssemblyDescription("This mod adds more Yee to your life.")]
[assembly: AssemblyVersion("1.0.0.0")]

namespace DuckGame.YeeHatMod
{
    public class YeeHatMod : Mod
    {
		protected override void OnPostInitialize()
		{
			Teams.core.teams.Add(new Team("Yee", Mod.GetPath<YeeHatMod>("yee_hat"), false, false, new Vec2()));
			base.OnPostInitialize();
		}
	}
}
