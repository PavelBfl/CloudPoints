using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Numerics;
using StepFlow.Common;
using StepFlow.Common.Exceptions;
using StepFlow.Core.Elements;
using StepFlow.Core.Tracks;
using StepFlow.Domains.Elements;
using StepFlow.Intersection;
using StepFlow.Markup.Services;
using StepFlow.Master;
using StepFlow.Master.Proxies.Elements;
using StepFlow.Master.Proxies.Tracks;

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
		PlayMasters = new PlayMasters()
		{
			builder.CreateState2("P0", null, null, null, null),
		};
		PlayMasters.CurrentKey = PlayMasters.Keys.FirstOrDefault();
		PlayMasters.Current?.PlayerCharacterPush(builder.CreatePlayerCharacter0());

		Meter.CreateObservableGauge("Time", () => PlayMasters.Current?.TimeAxis.Current ?? 0);
		Meter.CreateObservableGauge("Commands", () => PlayMasters.Current?.TimeAxis.Source.Current ?? 0);
		Meter.CreateObservableGauge("Frame", () => Frame.TotalMilliseconds);
		Init();

		TacticPanel = new(Control, Drawer, Place, PlayMasters);
	}

	private Meter Meter { get; } = new Meter("Game.Gameplay");

	private TimeSpan Frame { get; set; }

	private PlayMasters PlayMasters { get; }

	private IControl Control { get; }

	public IDrawer Drawer { get; }

	public RectangleF Place { get; }

	private TacticPanel TacticPanel { get; }

	public void Init()
	{

	}

	#region Debug
	private bool IsDebug { get; set; } = false;

	private void DebugHandle()
	{
		if (Control.OnSwitchDebug())
		{
			IsDebug = !IsDebug;
		}
	}
	#endregion

	#region Ticks
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

	private int TicksHandle()
	{
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

		return Control.OnTactic() ? 0 : CurrentTicks;
	}
	#endregion

	#region Game play
	public void GamePlay()
	{
		var currentTicks = TicksHandle();

		for (var i = 0; i < currentTicks; i++)
		{
			if (PlayMasters.Current is { } master)
			{
				var switchMaster = GamePlay(master);
				if (switchMaster)
				{
					return;
				}
			}
		}
	}

	public bool GamePlay(PlayMaster master)
	{
		var transaction = master.TimeAxis.BeginTransaction();

		// TODO
		if (master.Playground.GetPlayerCharacter() is { } playerCharacter)
		{
			GamePlay(master, playerCharacter);
		}

		master.TakeStep.Execute(null);

		UpdateTrack();

		transaction.Commit();

		// TODO Temp
		if (TryPopWormhole(out var wormhole, out var player))
		{
			PlayMasters.CurrentKey = wormhole.Destination;

			var bounds = GetBounds(player.BodyCurrent);
			var localPosition = GetPosition(bounds.Size, wormhole.Horizontal, wormhole.Vertical);
			var newPosition = new Point((int)wormhole.Position.X, (int)wormhole.Position.Y);

			var offset = new Point(
				-bounds.X + newPosition.X - localPosition.X,
				-bounds.Y + newPosition.Y - localPosition.Y
			);

			player.BodyNext.Clear();
			player.BodyCurrent.OffsetWith(offset);

			PlayMasters.Current?.PlayerCharacterPush(player);
			return true;
		}
		else
		{
			return false;
		}
	}

	public void GamePlay(PlayMaster master, PlayerCharacter character)
	{
		master.PlayerCharacterSetCourse.Execute(new()
		{
			Course = Control.GetPlayerCourseHorizontal(),
			Jump = Control.GetJump(),
		});

		if (Control.GetPlayerAction() is { } playerAction)
		{
			switch (playerAction)
			{
				case PlayerAction.Main:
				case PlayerAction.Auxiliary:
					var center = character.Body.GetCurrentRequired().Bounds.GetCenter();
					master.PlayerCharacterCreateProjectile.Execute(new() { Radians = Control.GetPlayerRotate(new(center.X, center.Y)), Action = playerAction });
					break;
				default: throw EnumNotSupportedException.Create(playerAction);
			};
		}
	}
	#endregion

	public void Update()
	{
		DebugHandle();

		if (Control.IsUndo())
		{
			for (var i = 0; i < TimeTick.TICKS_PER_FRAME * 5; i++)
			{
				PlayMasters.Current?.TimeAxis.Revert();
			}
		}
		else
		{
			var sw = System.Diagnostics.Stopwatch.StartNew();

			GamePlay();

			Frame = sw.Elapsed;
		}

		TacticPanel.Update();
	}

	private bool TryPopWormhole([MaybeNullWhen(false)] out WormholeDto wormhole, [MaybeNullWhen(false)] out PlayerCharacterDto player)
	{
		if (PlayMasters.Current is { } playMaster && playMaster.Wormhole is { } destination && playMaster.PlayerCharacterPop() is { } target)
		{
			playMaster.Wormhole = null;
			wormhole = destination;
			player = target;
			return true;
		}
		else
		{
			wormhole = default;
			player = default;
			return false;
		}
	}

	private static Point GetPosition(Size size, HorizontalAlign horizontal, VerticalAlign vertical)
	{
		return new Point(
			horizontal switch
			{
				HorizontalAlign.Left => 0,
				HorizontalAlign.Center => size.Width / 2,
				HorizontalAlign.Right => size.Width,
				_ => throw EnumNotSupportedException.Create(horizontal),
			},
			vertical switch
			{
				VerticalAlign.Top => 0,
				VerticalAlign.Center => size.Height / 2,
				VerticalAlign.Bottom => size.Height,
				_ => throw EnumNotSupportedException.Create(vertical),
			}
		);
	}

	private static Rectangle GetBounds(IEnumerable<Rectangle> rectangles)
	{
		var result = Rectangle.Empty;
		foreach (var rect in rectangles)
		{
			if (result.IsEmpty)
			{
				result = rect;
			}
			else
			{
				result = Rectangle.Union(result, rect);
			}
		}

		return result;
	}

	private List<TrackUnit> TrackUnits { get; } = new();

	private void UpdateTrack()
	{
		if (PlayMasters.Current is { } playMaster)
		{
			var trackUnitsProxy = playMaster.CreateListProxy(TrackUnits);

			foreach (var material in playMaster.Playground.Items)
			{
				var trackBuilderProxy = (ITrackBuilderProxy?)playMaster.CreateProxy(material.Track);

				if (trackBuilderProxy?.GetTrackForBuild() is { } trackChange && material.Body?.Current is { } shape)
				{
					RectangleF bounds = shape.Bounds;
					var location = (Vector2)bounds.Location;
					var size = (Vector2)bounds.Size;
					var radius = size / 2;
					trackUnitsProxy.Add(new(playMaster.Playground.Context)
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
				var trackUnitProxy = (ITrackUnitProxy)playMaster.CreateProxy(trackUnitsProxy[index]);
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
	}

	public void Draw()
	{
		var playMaster = PlayMasters.Current;
		if (playMaster is null)
		{
			return;
		}

		var floorTileSize = new Size(40, 40);
		for (var iX = 0; iX < Place.Width; iX += floorTileSize.Width)
		{
			for (var iY = 0; iY < Place.Height; iY += floorTileSize.Height)
			{
				Drawer.Draw(Texture.Floor, new(new Point(iX, iY), floorTileSize));
			}
		}

		var playground = playMaster.Playground;

		foreach (var place in playground.Items.OfType<Place>())
		{
			CreateTexture(place.Body?.GetCurrentRequired().Bounds ?? Rectangle.Empty, Texture.PoisonPlace, null);
		}

		// TODO Temp
		if (playMaster.Playground.Items.OfType<PlayerCharacter>().Any())
		{
			var playerCharacter = playground.GetPlayerCharacterRequired();
			CreateTexture(playerCharacter.Body.Current?.Bounds ?? Rectangle.Empty, Texture.Character, playerCharacter.Strength);
			CreateBorder(playerCharacter.Body?.Current, Color.Red);
		}

		foreach (var barrier in playground.Items.OfType<Obstruction>())
		{
			var textureView = ToTexture(barrier.View, barrier.Strength);
			switch (barrier.Kind)
			{
				case ObstructionKind.Single:
					CreateTexture(barrier.Body?.GetCurrentRequired().Bounds ?? Rectangle.Empty, textureView, barrier.Strength);
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

			CreateTexture(projectile.Body?.GetCurrentRequired().Bounds ?? Rectangle.Empty, textureName, null);
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

			CreateTexture(item.Body?.GetCurrentRequired().Bounds ?? Rectangle.Empty, textureName, null);
		}

		foreach (var enemy in playground.Items.OfType<Enemy>())
		{
			CreateTexture(enemy.Body?.GetCurrentRequired().Bounds ?? Rectangle.Empty, Texture.Enemy, enemy.Strength);
			CreateBorder(enemy.Body?.Current, Color.Red);
			CreateBorder(enemy.Vision, Color.Yellow);
		}

		foreach (var playgroundSwitch in playground.Items.OfType<Wormhole>())
		{
			CreateTexture(playgroundSwitch.Body?.GetCurrentRequired().Bounds ?? Rectangle.Empty, Texture.Door);
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

		foreach (var route in playMaster.Playground.Items.Select(x => x.Route))
		{
			if (route is { })
			{
				foreach (var segment in route.Path)
				{
					Drawer.Curve(segment, Color.Green);
					Drawer.Line(new(segment.Begin.X, segment.Begin.Y), new(segment.BeginControl.X, segment.BeginControl.Y), Color.Red);
					Drawer.Line(new(segment.BeginControl.X, segment.BeginControl.Y), new(segment.EndControl.X, segment.EndControl.Y), Color.Red);
					Drawer.Line(new(segment.EndControl.X, segment.EndControl.Y), new(segment.End.X, segment.End.Y), Color.Red);
				}
			}
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

	private void CreateBorder(Shape? shape, Color color)
	{
		if (shape is { } && IsDebug)
		{
			var bounds = shape.Bounds;
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
