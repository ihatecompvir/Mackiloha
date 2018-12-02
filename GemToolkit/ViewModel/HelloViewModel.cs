using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using Mackiloha;

namespace GemToolkit.ViewModel
{
    public class HelloViewModel : BaseModel
    {
        private string _state = "idle";
        private string _miloPath;
        private bool _groupByType = true;

        public string State { get => _state; set => Set(ref _state, value, "State"); }
        public string MiloPath { get => _miloPath; set => Set(ref _miloPath, value, "MiloPath"); }
        public MiloObjectDir Milo { get; set; }
        public bool GroupByType { get => _groupByType; set { Set(ref _groupByType, value, "GroupByType"); CreateNodes(); } }

        public ObservableCollection<TreeNode> TreeNodes { get; set; } = new ObservableCollection<TreeNode>();
        
        public void CreateNodes()
        {
            TreeNodes.Clear();

            MiloNode ConvertToNode(MiloObject obj)
            {
                /* // Won't be used until TBRB era
                var asDir = obj as MiloObjectDir;

                if (asDir != null)
                {
                    var children = asDir.Entries
                        //.Where(x => x.Name != obj.Name)
                        .Where(x => x is MiloObjectBytes)
                        .Select(x => ConvertToNode(obj));

                    return new MiloNode(obj, children);
                }*/

                return new MiloNode(obj);
            }
            
            var miloNodes = Milo.Entries
                    //.Where(x => x.Name != milo.Name || x.Type != milo.Type)
                    .Where(x => x is MiloObjectBytes)
                    .Select(x => ConvertToNode(x));

            if (GroupByType)
            {
                foreach (var groupNode in miloNodes.GroupBy(x => x.Type)
                    .OrderBy(y => y.Key)
                    .Select(z => new TreeNode(z.Key, z.OrderBy(q => q.Name))))
                {
                    TreeNodes.Add(groupNode);
                }
            }
            else
            {
                foreach (var n in miloNodes) TreeNodes.Add(n);
            }
        }
    }
}
