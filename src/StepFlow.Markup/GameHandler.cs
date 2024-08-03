using System.Diagnostics.Metrics;
using System.Drawing;
using System.Numerics;
using StepFlow.Common.Exceptions;
using StepFlow.Core;
using StepFlow.Core.Components;
using StepFlow.Core.Elements;
using StepFlow.Core.Tracks;
using StepFlow.Domains.Components;
using StepFlow.Domains.Elements;
using StepFlow.Intersection;
using StepFlow.Markup.Services;
using StepFlow.Master;
using StepFlow.Master.Proxies.Elements;
using StepFlow.Master.Proxies.Tracks;
using StepFlow.Master.Scripts;

namespace StepFlow.Markup;

public sealed class GameHandler
{
	public GameHandler(
		RectangleF placeBounds,
		IDrawer drawer,
		IControl control
	)
	{
		Drawer = drawer ?? throw new ArgumentNullException(nameof(drawer));
		Control = control ?? throw new ArgumentNullException(nameof(control));
		Place = placeBounds;

		var builder = new PlaygroundBuilder();
		PlayMasters = new[]
		{
			builder.CreateState0(),
			builder.CreateState1(),
		}.ToDictionary(x => x.Name ?? throw new InvalidOperationException(), x => new PlayMaster(x));
		playMasterName = PlayMasters.Keys.First();

		PlayMaster.PlayerCharacterPush(builder.CreatePlayerCharacter0());

		Meter.CreateObservableGauge("Time", () => PlayMaster.TimeAxis.Current);
		Meter.CreateObservableGauge("Commands", () => PlayMaster.TimeAxis.Source.Current);
		Meter.CreateObservableGauge("Frame", () => Frame.TotalMilliseconds);
		Meter.CreateObservableGauge("Shapes", () => PlayMaster.Playground.Context.IntersectionContext.Count);
		Init();

		TacticPanel = new(Control, Drawer, Place, PlayMaster);
	}

	private Meter Meter { get; } = new Meter("Game.Gameplay");

	private TimeSpan Frame { get; set; }

	private string? playMasterName;

	private string PlayMasterName
	{
		get => playMasterName ?? throw new InvalidOperationException();
		set
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				throw new ArgumentException(nameof(value));
			}

			if (!PlayMasters.ContainsKey(value))
			{
				throw new ArgumentOutOfRangeException(nameof(value));
			}
			
			playMasterName = value;
		}
	}

	private Dictionary<string, PlayMaster> PlayMasters { get; }

	private PlayMaster PlayMaster => PlayMasters[PlayMasterName];

	private IControl Control { get; }

	public IDrawer Drawer { get; }

	public RectangleF Place { get; }

	private TacticPanel TacticPanel { get; }

	private static Size CellSize => new(Playground.CELL_SIZE_DEFAULT, Playground.CELL_SIZE_DEFAULT);

	private static int PlaygroundToGlobal(int value) => (value + 1) * Playground.CELL_SIZE_DEFAULT;

	private static Point PlaygroundToGlobal(int x, int y) => new(PlaygroundToGlobal(x), PlaygroundToGlobal(y));

	private static Point PlaygroundToGlobal(Point position) => PlaygroundToGlobal(position.X, position.Y);

	private static Rectangle CreateCell(int x, int y) => new(PlaygroundToGlobal(x, y), CellSize);

	private static Rectangle CreateCell(Point position) => CreateCell(position.X, position.Y);

	private static Vector2 CreateRotate(float radians)
	{
		return Vector2.Transform(
			new Vector2(1, 0),
			Matrix3x2.CreateRotation(radians)
		);
	}

	public void Init()
	{
		
	}

	private void CreateRoom(Point location, Size size, int width)
	{
		var top = new Rectangle[size.Width];
		var bottom = new Rectangle[size.Width];
		for (var iX = 0; iX < size.Width; iX++)
		{
			top[iX] = CreatePixel(new(iX, 0));
			bottom[iX] = CreatePixel(new(iX, size.Height - 1));
		}

		var left = new Rectangle[size.Height - 2];
		var right = new Rectangle[size.Height - 2];
		for (var iY = 0; iY < size.Height - 2; iY++)
		{
			left[iY] = CreatePixel(new(0, iY + 1));
			right[iY] = CreatePixel(new(size.Width - 1, iY + 1));
		}

		PlayMaster.CreateObstruction.Execute(new() { Bounds = top, Kind = ObstructionKind.Tiles, View = ObstructionView.DarkWall, Weight = Material.MAX_WEIGHT, });
		PlayMaster.CreateObstruction.Execute(new() { Bounds = bottom, Kind = ObstructionKind.Tiles, View = ObstructionView.DarkWall, Weight = Material.MAX_WEIGHT, });
		PlayMaster.CreateObstruction.Execute(new() { Bounds = left, Kind = ObstructionKind.Tiles, View = ObstructionView.DarkWall, Weight = Material.MAX_WEIGHT, });
		PlayMaster.CreateObstruction.Execute(new() { Bounds = right, Kind = ObstructionKind.Tiles, View = ObstructionView.DarkWall, Weight = Material.MAX_WEIGHT, });

		Rectangle CreatePixel(Point position) => new Rectangle(
			location.X + position.X * width,
			location.Y + position.Y * width,
			width,
			width
		);
	}

	private void CreateCellObstruction(Point position, int? strength, ObstructionView view, int weight = Material.MAX_WEIGHT)
	{
		PlayMaster.CreateObstruction.Execute(new()
		{
			Bounds = new[] { CreateCell(position) },
			Strength = strength,
			Kind = ObstructionKind.Single,
			View = view,
			Weight = weight,
		});
	}

	private void PlayerCharacterSetCourse(float? course)
	{
		PlayMaster.PlayerCharacterSetCourse.Execute(new() { Course = course, });
	}

	private void CreateProjectile(float radians, PlayerAction action)
	{
		PlayMaster.PlayerCharacterCreateProjectile.Execute(new() { Radians = radians, Action = action });
	}

	private void CreateEnemy(Rectangle bounds, int visionSize, Vector2 beginVector, CollisionBehavior collisionBehavior, StateParameters[]? states)
	{
		PlayMaster.CreateEnemy.Execute(new()
		{
			Bounds = bounds,
			Vision = Rectangle.FromLTRB(
				bounds.Left - visionSize,
				bounds.Top - visionSize,
				bounds.Right + visionSize,
				bounds.Bottom + visionSize
			),
			ReleaseItem = ItemKind.AddStrength,
			Course = beginVector,
			CollisionBehavior = collisionBehavior,
			States = states
		});
	}

	private const int TIME_TICKS_STEP = 10;

	private int currentTicks = TimeTick.TICKS_PER_FRAME;

	private int CurrentTicks
	{
		get => currentTicks;
		set
		{
			currentTicks = value;

			if (currentTicks < 0)
			{
				currentTicks = 0;
			}
			else if (currentTicks > TimeTick.TICKS_PER_FRAME)
			{
				currentTicks = TimeTick.TICKS_PER_FRAME;
			}
		}
	}

	public void Update()
	{
		if (Control.OnSwitchDebug())
		{
			IsDebug = !IsDebug;
		}

		var currentTicks = Control.OnTactic() ? 0 : CurrentTicks;

		var timeOffset = Control.GetTimeOffset();
		switch (timeOffset)
		{
			case TimeOffset.None:
				break;
			case TimeOffset.Up:
				CurrentTicks += TIME_TICKS_STEP;
				break;
			case TimeOffset.Down:
				CurrentTicks -= TIME_TICKS_STEP;
				break;
			default: throw EnumNotSupportedException.Create(timeOffset);
		}

		if (Control.IsUndo())
		{
			for (var i = 0; i < TimeTick.TICKS_PER_FRAME * 5; i++)
			{
				PlayMaster.TimeAxis.Revert();
			}
		}
		else
		{
			var sw = System.Diagnostics.Stopwatch.StartNew();
			for (var i = 0; i < currentTicks; i++)
			{
				var transaction = PlayMaster.TimeAxis.BeginTransaction();

				// TODO
				if (PlayMaster.Playground.Items.OfType<PlayerCharacter>().Any())
				{
					PlayerCharacterSetCourse(Control.GetPlayerCourse());

					if (Control.GetPlayerAction() is { } playerAction)
					{
						switch (playerAction)
						{
							case PlayerAction.Main:
							case PlayerAction.Auxiliary:
								var playerCharacter = PlayMaster.Playground.GetPlayerCharacterRequired();
								var center = playerCharacter.GetBodyRequired().Current.Bounds.GetCenter();
								CreateProjectile(Control.GetPlayerRotate(new(center.X, center.Y)), playerAction);
								break;
							default: throw EnumNotSupportedException.Create(playerAction);
						};
					} 
				}

				PlayMaster.TakeStep.Execute(null);

				UpdateTrack();

				transaction.Commit();

				// TODO Temp
				if (PlayMaster.NextPlayground is { } nextPlayground && nextPlayground != PlayMasterName)
				{
					var playerDto = PlayMaster.PlayerCharacterPop();
					PlayMasterName = nextPlayground;
					if (playerDto is { })
					{
						PlayMaster.PlayerCharacterPush(playerDto);
					}
					break;
				}
			}

			Frame = sw.Elapsed;
		}

		TacticPanel.Update();
	}

	private List<TrackUnit> TrackUnits { get; } = new();

	private void UpdateTrack()
	{
		var trackUnitsProxy = PlayMaster.CreateListProxy(TrackUnits);

		foreach (var material in PlayMaster.Playground.Items)
		{
			var trackBuilderProxy = (ITrackBuilderProxy?)PlayMaster.CreateProxy(material.Track);

			if (trackBuilderProxy?.GetTrackForBuild() is { } trackChange && material.Body?.Current is { } shape)
			{
				RectangleF bounds = shape.Bounds;
				var location = (Vector2)bounds.Location;
				var size = (Vector2)bounds.Size;
				var radius = size / 2;
				trackUnitsProxy.Add(new(PlayMaster.Playground.Context)
				{
					Center = location + radius,
					Radius = radius,
					Change = trackBuilderProxy.Change,
				});
			}
		}

		var index = 0;
		while (index < trackUnitsProxy.Count)
		{
			var trackUnitProxy = (ITrackUnitProxy)PlayMaster.CreateProxy(trackUnitsProxy[index]);
			if (trackUnitProxy.Change())
			{
				index++;
			}
			else
			{
				trackUnitsProxy.RemoveAt(index);
			}
		}
	}

	private bool IsDebug { get; set; } = false;

	public void Draw()
	{
		var floorTileSize = new Size(40, 40);
		for (var iX = 0; iX < Place.Width; iX += floorTileSize.Width)
		{
			for (var iY = 0; iY < Place.Height; iY += floorTileSize.Height)
			{
				Drawer.Draw(Texture.Floor, new(new Point(iX, iY), floorTileSize));
			}
		}

		var playground = PlayMaster.Playground;

		foreach (var place in playground.Items.OfType<Place>())
		{
			CreateTexture(place.Body?.Current.Bounds ?? Rectangle.Empty, Texture.PoisonPlace, null);
		}

		// TODO Temp
		if (PlayMaster.Playground.Items.OfType<PlayerCharacter>().Any())
		{
			var playerCharacter = playground.GetPlayerCharacterRequired();
			CreateTexture(playerCharacter?.Body?.Current.Bounds ?? Rectangle.Empty, Texture.Character, playerCharacter?.Strength);
			CreateBorder(playerCharacter?.Body, Color.Red); 
		}

		foreach (var barrier in playground.Items.OfType<Obstruction>())
		{
			var textureView = ToTexture(barrier.View, barrier.Strength);
			switch (barrier.Kind)
			{
				case ObstructionKind.Single:
					CreateTexture(barrier.Body?.Current.Bounds ?? Rectangle.Empty, textureView, barrier.Strength);
					break;
				case ObstructionKind.Tiles:
					foreach (var bounds in barrier.Body?.Current ?? Enumerable.Empty<Rectangle>())
					{
						CreateTexture(bounds, textureView, barrier.Strength);
					}
					break;
				default: throw EnumNotSupportedException.Create<ObstructionKind>(barrier.Kind);
			}
		}

		foreach (var projectile in playground.Items.OfType<Projectile>())
		{
			var textureName = projectile.Damage.Kind switch
			{
				DamageKind.None => Texture.Projectile,
				DamageKind.Fire => Texture.ProjectileFire,
				DamageKind.Poison => Texture.ProjectilePoison,
				DamageKind.Fire | DamageKind.Poison => Texture.ProjectileAll,
				_ => Texture.Projectile,
			};

			CreateTexture(projectile.Body?.Current.Bounds ?? Rectangle.Empty, textureName, null);
		}

		foreach (var item in playground.Items.OfType<Item>())
		{
			var textureName = item.Kind switch
			{
				ItemKind.Fire => Texture.ItemFire,
				ItemKind.Poison => Texture.ItemPoison,
				ItemKind.Speed => Texture.ItemSpeed,
				ItemKind.AttackSpeed => Texture.ItemAttackSpeed,
				ItemKind.AddStrength => Texture.ItemAddStrength,
				_ => Texture.ItemUnknown,
			};

			CreateTexture(item.Body?.Current.Bounds ?? Rectangle.Empty, textureName, null);
		}

		foreach (var enemy in playground.Items.OfType<Enemy>())
		{
			CreateTexture(enemy.Body?.Current.Bounds ?? Rectangle.Empty, Texture.Enemy, enemy.Strength);
			CreateBorder(enemy.Body, Color.Red);
			CreateBorder(enemy.Vision, Color.Yellow);
		}

		foreach (var playgroundSwitch in playground.Items.OfType<WormholeSwitch>())
		{
			CreateTexture(playgroundSwitch.Body?.Current.Bounds ?? Rectangle.Empty, Texture.PoisonPlace);
		}

		foreach (var trackUnit in TrackUnits.Where(x => x.GetChangeRequired().Thickness > 0))
		{
			var bounds = CreateRectangle(trackUnit.Center, trackUnit.Radius + new Vector2(trackUnit.GetChangeRequired().Thickness));
			CreateTexture(bounds, Texture.Circle, Color.Black);
		}

		foreach (var trackUnit in TrackUnits)
		{
			var bounds = CreateRectangle(trackUnit.Center, trackUnit.Radius);
			CreateTexture(bounds, Texture.Circle);
		}

		TacticPanel.Draw();
	}

	private static RectangleF CreateRectangle(Vector2 center, Vector2 radius) => new(
		(PointF)(center - radius),
		(SizeF)(radius * 2)
	);

	private static Texture ToTexture(ObstructionView view, Scale? scale) => view switch
	{
		ObstructionView.None => Texture.ObstructionNone,
		ObstructionView.DarkWall => Texture.ObstructionDarkWall,
		ObstructionView.Bricks => ToPct(scale) > 0.5f ? Texture.ObstructionBricks : Texture.ObstructionBricksDamaged,
		ObstructionView.Boards => Texture.ObstructionBoards,
		_ => throw EnumNotSupportedException.Create(view),
	};

	private static float ToPct(Scale? scale) => scale is { } instance ? (float)instance.Value / instance.Max : 0;

	private void CreateTexture(Rectangle bounds, Texture texture, Scale? strength)
	{
		if (!bounds.IsEmpty)
		{
			Drawer.Draw(texture, bounds);
			if (strength is not null && IsDebug)
			{
				var strengthBounds = new Rectangle(
					bounds.Left,
					bounds.Top,
					bounds.Width,
					0
				);

				Drawer.DrawString(
					strength.Value.ToString(),
					strengthBounds,
					HorizontalAlign.Center,
					VerticalAlign.Bottom,
					Color.Red
				);
			}
		}
	}

	private void CreateTexture(RectangleF bounds, Texture texture, Color? color = null)
	{
		Drawer.Draw(
			texture,
			bounds,
			color
		);
	}

	private void CreateBorder(Collided? collided, Color color)
	{
		if (collided is { } && IsDebug)
		{
			var bounds = collided.Current.Bounds;
			Drawer.Polygon(
				new PointF[]
				{
					new(bounds.Left, bounds.Top),
					new(bounds.Right, bounds.Top),
					new(bounds.Right, bounds.Bottom),
					new(bounds.Left, bounds.Bottom),
				},
				color
			);
		}
	}
}
