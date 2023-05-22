using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using StepFlow.ViewModel.Commands;

namespace StepFlow.ViewModel
{
	public static class WrapperProvider
	{
		public static IContainer GetContainer(this GamePlay.IDataContainer dataContainer)
		{
			if (dataContainer is null)
			{
				throw new ArgumentNullException(nameof(dataContainer));
			}

			if (dataContainer.Data is null)
			{
				var container = new Container();
				dataContainer.Data = container;
				return container;
			}
			else
			{
				return (IContainer)dataContainer.Data;
			}
		}

		public static IComponent GetOrCreate(this object model)
		{
			if (model is null)
			{
				throw new ArgumentNullException(nameof(model));
			}

			var container = ((GamePlay.IDataContainer)model).GetContainer();

			var component = container.Components["ViewModel"];

			if (component is null)
			{
				component = model switch
				{
					GamePlay.Commands.MoveCommand moveCommand => new MoveCommandVm(moveCommand),
					GamePlay.Commands.CreateCommand createCommand => new CreateCommand(createCommand),
					GamePlay.Node node => new NodeVm(node),
					GamePlay.Piece piece => new PieceVm(piece),
					_ => throw Exceptions.Builder.CreateUnknownModel(),
				};
				container.Add(component, "ViewModel");
			}

			return component;
		}

		public static T GetOrCreate<T>(this object model)
			where T : notnull, IComponent
			=> (T)GetOrCreate(model);
	}
}
