using UnityEngine;

namespace RhinoGame
{
    public class UIBillboard : MonoBehaviour
    {
        public bool scaleWithDistance = false;
        public float scaleMultiplier = 1f;

        private Transform camTrans;
        private Transform trans;
        private float size;

        void Awake()
        {
            trans = transform;
        }

        void Start()
        {
            if (Camera.main != null)
            {
                camTrans = Camera.main.transform;
            }
        }

        void Update()
        {
            if (camTrans == null)
                return;

            transform.LookAt(
                trans.position + camTrans.rotation * Vector3.forward,
                camTrans.rotation * Vector3.up
            );

            if (!scaleWithDistance)
                return;

            size = (camTrans.position - transform.position).magnitude;
            transform.localScale = Vector3.one * (size * (scaleMultiplier / 100f));
        }
    }
}
