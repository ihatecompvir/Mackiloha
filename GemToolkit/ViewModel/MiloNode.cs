using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neutronium.MVVMComponents;
using Mackiloha;

namespace GemToolkit.ViewModel
{
    public class MiloNode : TreeNode
    {
        private readonly MiloObject _milo;

        public MiloNode(MiloObject milo, IEnumerable<MiloNode> children = null) : base(children)
        {
            _milo = milo;
        }

        public MiloObject Milo() => this._milo;
        public MiloObjectBytes MiloBytes() => this._milo as MiloObjectBytes;

        public override string Name { get => _milo.Name; set { _milo.Name = value; AnnouceChange(_milo, "Name"); } }
        public string Type => _milo.Type;

        public bool IsDirectory =>
            ((string)_milo.Type).EndsWith("Dir",
                StringComparison.CurrentCultureIgnoreCase);

        public bool IsMilo => true;
    }
}
