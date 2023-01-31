using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace StepFlow.Entities
{
	public class FlowContext : DbContext
	{
		private const string FILE_NAME = "Save.db";
		private const string CONNECTION_STRING = "Filename=" + FILE_NAME;

		public FlowContext()
		{
			Database.EnsureCreated();
		}

		[AllowNull]
		public DbSet<WorldEntity> Worlds { get; set; }

		[AllowNull]
		public DbSet<ParticleEntity> Particles { get; set; }

		[AllowNull]
		public DbSet<HexNodeEntity> HexNodes { get; set; }

		[AllowNull]
		public DbSet<PieceEntity> Pieces { get; set; }

		public void InitCurrentId() => CurrentId = LoadCurrentId();

		private int LoadCurrentId()
		{
			var tables = new IQueryable<EntityBase>[]
			{
				Worlds, Particles, HexNodes, Pieces
			};

			var result = tables.Select(x => x.Max(y => y.Id)).Max() ?? 0;

			return result;
		}

		public int? CurrentId { get; private set; }

		public int GetId()
		{
			var currentId = CurrentId ?? LoadCurrentId();
			currentId++;
			CurrentId = currentId;
			return currentId;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlite(CONNECTION_STRING);
		}
	}
}
