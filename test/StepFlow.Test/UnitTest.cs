using StepFlow.Master;

namespace StepFlow.Test
{
	public class UnitTest
	{
		[Fact]
		public void Test()
		{
			var playMaster = new PlayMaster();
			playMaster.Execute("AddComponent(Playground, \"Scheduled\")");
		}
	}
}