using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemToolkit.ViewModel
{
    public class TreeNode : BaseModel
    {
        private string _name;
        private bool _isExpanded;

        protected TreeNode(IEnumerable<TreeNode> children = null)
        {
            Children = children == null
                ? new ObservableCollection<TreeNode>()
                : new ObservableCollection<TreeNode>(children);
        }

        public TreeNode(string name, IEnumerable<TreeNode> children = null)
        {
            _name = name;

            Children = children == null
                ? new ObservableCollection<TreeNode>()
                : new ObservableCollection<TreeNode>(children);
        }

        public virtual string Name { get => _name; set => Set(ref _name, value, "Name"); }
        public bool IsExpanded { get => _isExpanded; set => Set(ref _isExpanded, value, "IsExpanded"); }
        public ObservableCollection<TreeNode> Children { get; set; }
    }
}
