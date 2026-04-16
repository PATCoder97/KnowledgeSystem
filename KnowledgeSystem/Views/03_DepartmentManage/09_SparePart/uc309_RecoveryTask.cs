using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._09_SparePart
{
    public class uc309_RecoveryTask : uc309_RecoveryMgmt
    {
        public uc309_RecoveryTask()
        {
            Name = nameof(uc309_RecoveryTask);
            Dock = DockStyle.Fill;
        }

        protected override bool IsTaskView => true;
    }
}
