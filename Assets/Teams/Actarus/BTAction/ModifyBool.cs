using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace Actarus
{
	[TaskCategory("Actarus")]
	public class Fire : ActarusAction
	{
        public override TaskStatus OnUpdate()
		{
			this.Owner.SetVariableValue("outFire", true);
			return TaskStatus.Success;
		}
	}
}