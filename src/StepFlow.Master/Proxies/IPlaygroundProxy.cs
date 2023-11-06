using System.Collections.Generic;
using System.Drawing;
using StepFlow.Master.Proxies.Border;
using StepFlow.Master.Proxies.Components;

namespace StepFlow.Master.Proxies
{
    public interface IPlaygroundProxy
	{
		IList<ISubjectProxy> Subjects { get; }
		IBorderedProxy CreateBordered();
		Point CreatePoint(int x, int y);
		Rectangle CreateRectangle(int x, int y, int width, int height);
		ISubjectProxy CreateSubject();
		IEnumerable<(ICollidedProxy, ICollidedProxy)> GetCollision();

		void CreateRoom(Rectangle rectangle, int width);
		void CreateCharacter(Rectangle rectangle, int strengthValue, bool player);
		void CreateItem(Rectangle rectangle, string kind);
		void CreateSentryGun(Rectangle size, Rectangle vision, int strengthValue);
	}
}
