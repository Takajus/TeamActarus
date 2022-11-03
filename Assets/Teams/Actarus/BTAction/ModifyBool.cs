using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace Actarus
{
	[TaskCategory("Actarus")]
	public class Fire : Action
	{
		ActarusController AC;
		public override void OnAwake()
        {
			AC = gameObject.GetComponent<ActarusController>();
        }

		public override TaskStatus OnUpdate()
		{
			this.Owner.SetVariableValue("outFire", true);
			return TaskStatus.Success;
		}
	}
}