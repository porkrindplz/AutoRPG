using System;
using _Scripts.Utilities;
using TMPro;
using UnityEngine;

namespace _Scripts.UI
{
    public class ToolTip : Singleton<ToolTip>
    {

        [SerializeField] RectTransform rectTransform;
        [SerializeField] TextMeshProUGUI text;
        protected override void Awake()
        {
            base.Awake();
            HideToolTip();
        }
        

        protected void Update()
        {
            if(rectTransform.gameObject.activeSelf)
            {
                rectTransform.position = Input.mousePosition;
            }

        }
    
        public void ShowToolTip(string message)
        {
            if (message == null)
            {
                HideToolTip();
                return;
            }

            rectTransform.gameObject.SetActive(true);
            text.text = message;
        }
    
        public void HideToolTip()
        {
            text.text = "";
            rectTransform.gameObject.SetActive(false);
        }
    }
}
