using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface ISelectable
{
    public event Action ItemSelected;
    public event Action ItemDeselected;
    public bool IsSelected { get; }
}
