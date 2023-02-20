using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using StepFlow.Core;
using StepFlow.Entities;

namespace StepFlow.ViewModel
{
	public class ParticlesCollectionVm
	{
		public IParticleVm this[Particle key]
		{
			get
			{
				if (key is null)
				{
					throw new ArgumentNullException(nameof(key));
				}

				return ByModel[key];
			}
		}

		public Particle this[IParticleVm key]
		{
			get
			{
				if (key is null)
				{
					throw new ArgumentNullException(nameof(key));
				}

				return ByViewModel[key];
			}
		}

		private Dictionary<Particle, IParticleVm> ByModel { get; } = new Dictionary<Particle, IParticleVm>();

		private Dictionary<IParticleVm, Particle> ByViewModel { get; } = new Dictionary<IParticleVm, Particle>();

		public int Count => ByModel.Count;

		public void Add(Particle particle, IParticleVm particleVm)
		{
			if (particle is null)
			{
				throw new ArgumentNullException(nameof(particle));
			}

			if (particleVm is null)
			{
				throw new ArgumentNullException(nameof(particleVm));
			}

			ByModel.Add(particle, particleVm);
			ByViewModel.Add(particleVm, particle);
		}

		public bool Remove(Particle particle)
		{
			if (particle is null)
			{
				throw new ArgumentNullException(nameof(particle));
			}

			if (ByModel.Remove(particle, out var particleVm))
			{
				ByViewModel.Remove(particleVm);
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool Remove(IParticleVm particleVm)
		{
			if (particleVm is null)
			{
				throw new ArgumentNullException(nameof(particleVm));
			}

			if (ByViewModel.Remove(particleVm, out var particle))
			{
				ByModel.Remove(particle);
				return true;
			}
			else
			{
				return false;
			}
		}
	}

	public class WorldVm : WrapperVm<World>
	{
		public WorldVm(World source) : base(source, true)
		{
			foreach (var node in Source.Place.Values)
			{
				nodes.Add(node.Position, new NodeVm(this, node));
			}

			TimeAxis = new AxisVm();
		}

		internal Dictionary<Particle, IParticleVm> Particles { get; } = new Dictionary<Particle, IParticleVm>();

		private Dictionary<Point, NodeVm> nodes = new Dictionary<Point, NodeVm>();

		public IReadOnlyDictionary<Point, NodeVm> Nodes => nodes;

		public AxisVm TimeAxis { get; }

		public ICollection<PieceVm> Pieces { get; } = new HashSet<PieceVm>();

		private PieceVm? current = null;

		public PieceVm? Current
		{
			get => current;
			set
			{
				if (!Equals(Current, value))
				{
					OnPropertyChanging();

					if (Current is { })
					{
						Current.IsMark = false;
					}

					current = value;

					if (Current is { })
					{
						Current.IsMark = true;
					}

					OnPropertyChanged();
				}
			}
		}

		public void TakeStep()
		{
			Source.TakeStep();

			var keys = Source.Particles.OfType<Piece>().Union(Particles.Keys.OfType<Piece>()).ToArray();
			foreach (var piece in keys)
			{
				var pieceContainsVm = Particles.ContainsKey(piece);
				var pieceContainsM = Source.Particles.Contains(piece);
				if (pieceContainsVm && !pieceContainsM)
				{
					if (Particles.Remove(piece, out var particleVm))
					{
						var pieceVm = (PieceVm)particleVm;
						pieceVm.IsMark = false;
						pieceVm.Current = null;
						pieceVm.Next = null;
						Pieces.Remove(pieceVm);
					}
				}
				else if (!pieceContainsVm && pieceContainsM)
				{
					Particles.Add(piece, new PieceVm(this, piece));
				}
			}

			foreach (var piece in Particles.Values.OfType<PieceVm>())
			{
				piece.TakeStep();
			}
		}

		public void Save()
		{
			using var context = new FlowContext();
			context.InitCurrentId();

			var worldEntity = context.Worlds.Add(new WorldEntity()
			{
				Id = context.GetId(),
			}).Entity;

			var links = new Dictionary<object, EntityBase>();
			foreach (var particle in Source.Particles)
			{
				switch (particle)
				{
					case Node hexNode:
						var hexNodeEntity = context.HexNodes.Add(new HexNodeEntity()
						{
							Id = context.GetId(),
							Col = hexNode.Position.X,
							Row = hexNode.Position.Y,
							Owner = worldEntity,
						}).Entity;
						links.Add(hexNode, hexNodeEntity);
						break;
					case Piece piece:
						var pieceEntity = context.Pieces.Add(new PieceEntity()
						{
							Id = context.GetId(),
							Owner = worldEntity,
						}).Entity;
						links.Add(piece, pieceEntity);
						break;
					default:
						break;
				}
			}

			foreach (var piece in Source.Particles.OfType<Piece>())
			{
				if (piece.Current is { } current)
				{
					var pieceEntity = (PieceEntity)links[piece];
					var hexNodeEntity = (HexNodeEntity)links[current];

					pieceEntity.Current = hexNodeEntity;
				}
			}

			context.SaveChanges();
		}

		public static World Load(int worldId)
		{
			using var context = new FlowContext();

			var result = new World(0, 0, HexOrientation.Flat, false);
			foreach (var hexNodeEntity in context.HexNodes.Where(x => x.OwnerId == worldId))
			{
				result.Place.Add(new Node(null, new Point(
					x: hexNodeEntity.Col,
					y: hexNodeEntity.Row
				)));
			}

			return result;
		}
	}
}
