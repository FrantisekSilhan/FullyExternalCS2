using CS2Cheat.Data.Entity;
using CS2Cheat.Data.Game;
using CS2Cheat.Graphics;
using CS2Cheat.Utils;
using SharpDX;
using Color = SharpDX.Color;

namespace CS2Cheat.Features {
	public static class AimBotVisuals {
		public static int number = 0;
		// chatgpt and github copilot is retarded
		public static void Draw(Graphics.Graphics graphics, float aimBotFov) {
			var screenSize = graphics.GameProcess.WindowRectangleClient.Size;
			var aspectRatio = screenSize.Width / (float)screenSize.Height;

			float fovRadius = CalculateRadiusFromFov(aimBotFov, screenSize);

			var pointCenter = new Vector2(screenSize.Width / 2, screenSize.Height / 2);

			if (number % 100 == 0)
			Console.WriteLine($"fovRadius: {fovRadius}\naspectRatio: {aspectRatio}\naimBotFov: {aimBotFov}\n");
			number++;

			graphics.DrawCircle(Color.Green, pointCenter, fovRadius);
		}

		private static float CalculateRadiusFromFov(float fov, Size screenSize) {
			float halfScreenWidth = screenSize.Width / 2f;
			float fovRadius = halfScreenWidth / (float)Math.Tan(fov / 2f);
			return fovRadius;
		}
	}
}
