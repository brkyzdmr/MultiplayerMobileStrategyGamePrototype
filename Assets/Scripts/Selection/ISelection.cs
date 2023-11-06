using UnityEngine;

namespace MMSGP.Selection
{
    public interface ISelection
    {
        void AddSelected(GameObject gameObject);
        void DeselectAll();
    }
}